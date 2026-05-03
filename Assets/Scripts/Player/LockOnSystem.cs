using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class LockOnSystem : MonoBehaviour
{
    [SerializeField] private float detectionRange = 30f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private ThirdPersonController playerController;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float smoothSpeed = 2f;
    [SerializeField] private float lockOnTargetHeightOffset = 0.4f;
    [SerializeField] private float lockOnCameraYawOffset = 0f;
    [SerializeField] private float lockOnCameraPitchOffset = -0.4f;
    [Header("Dynamic Pitch")]
    [Tooltip("When enabled, pitch offset will interpolate between Near and Far values based on target distance.")]
    [SerializeField] private bool dynamicPitchByDistance = true;
    [Tooltip("Pitch offset applied when the target is very close (e.g. -1).")]
    [SerializeField] private float pitchOffsetNear = -1f;
    [Tooltip("Pitch offset applied when the target is at detectionRange (e.g. 1).")]
    [SerializeField] private float pitchOffsetFar = 1f;
    [SerializeField] private bool lockCameraWhileLockedOn = true;
    [SerializeField] private Transform lockOnIndicator;

    private Transform currentTarget;
    private bool isLockedOn = false;
    private PlayerInteract playerInteract;
    private Quaternion originalCameraTargetRotation;
    private StarterAssetsInputs _input;

    void Start()
    {
        if (playerTransform != null)
            playerInteract = playerTransform.GetComponent<PlayerInteract>();

        if (playerController == null && playerTransform != null)
            playerController = playerTransform.GetComponent<ThirdPersonController>();

        if (cameraTarget == null && playerController != null && playerController.CinemachineCameraTarget != null)
            cameraTarget = playerController.CinemachineCameraTarget.transform;

        if (playerTransform != null)
            _input = playerTransform.GetComponent<StarterAssetsInputs>();
    }

    void Update()
    {
        bool lockOnPressed = false;

        // Input System unificado (teclado L / mouse middle / gamepad R3)
        if (_input != null && _input.lockOn)
        {
            _input.lockOn = false; // Consome o input
            lockOnPressed = true;
        }
        // Fallback para Input direto
        else if (_input == null)
        {
            if ((Keyboard.current != null && Keyboard.current.lKey.wasPressedThisFrame) || (Mouse.current != null && Mouse.current.middleButton.wasPressedThisFrame))
            {
                lockOnPressed = true;
            }
        }

        if (lockOnPressed)
        {
            if (isLockedOn)
            {
                CancelLockOn();
            }
            else
            {
                FindAndLockOn();
            }
        }
    }

    void LateUpdate()
    {
        if (isLockedOn)
        {
            if (currentTarget == null || !IsTargetValid())
            {
                CancelLockOn();
                   return;
            }
            else
            {
                Vector3 targetFocusPoint = currentTarget.position + Vector3.up * lockOnTargetHeightOffset;
                Collider targetCol = null;
                foreach (var col in currentTarget.GetComponentsInChildren<Collider>())
                {
                    if (!col.isTrigger)
                    {
                        targetCol = col;
                        break;
                    }
                }
                if (targetCol == null) targetCol = currentTarget.GetComponentInChildren<Collider>();
                if (targetCol != null)
                {
                    // Use the same height as other enemies (from pivot), but center it on the collider's X/Z
                    targetFocusPoint = new Vector3(targetCol.bounds.center.x, currentTarget.position.y + lockOnTargetHeightOffset, targetCol.bounds.center.z);
                }
                if (cameraTarget != null && playerController != null)
                {
                    // Calculate direction from camera to target
                    Vector3 directionToTarget = (targetFocusPoint - cameraTarget.position).normalized;
                    
                    // Create target rotation using LookRotation
                    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
                    
                    // Get current rotation
                    Quaternion currentRotation = cameraTarget.rotation;
                    
                    // Smoothly interpolate using Slerp (more stable for rotations)
                    Quaternion smoothedRotation = Quaternion.Slerp(currentRotation, targetRotation, smoothSpeed * Time.deltaTime);
                    
                    // Extract yaw and pitch from the smoothed rotation
                    Vector3 eulerAngles = smoothedRotation.eulerAngles;
                    float yaw = eulerAngles.y + lockOnCameraYawOffset;

                    float chosenPitchOffset = lockOnCameraPitchOffset;
                    if (dynamicPitchByDistance && currentTarget != null && playerTransform != null)
                    {
                        float dist = Vector3.Distance(playerTransform.position, currentTarget.position);
                        float t = Mathf.InverseLerp(0f, detectionRange, dist);
                        // interpolate from near (-1) to far (1)
                        chosenPitchOffset = Mathf.Lerp(pitchOffsetNear, pitchOffsetFar, t);
                    }

                    float pitch = eulerAngles.x + chosenPitchOffset;
                    
                    // Normalize pitch for proper clamping
                    if (pitch > 180) pitch -= 360;
                    
                    playerController.SetCameraRotation(yaw, pitch);
                }

                if (playerInteract != null)
                {
                    Vector3 directionToTargetFromPlayer = (targetFocusPoint - playerTransform.position).normalized;
                    playerInteract.SetDirection(directionToTargetFromPlayer);
                }

                // Vira o jogador para o inimigo (apenas no eixo Y)
                Vector3 dirToEnemy = targetFocusPoint - playerTransform.position;
                dirToEnemy.y = 0f;
                if (dirToEnemy.sqrMagnitude > 0.001f)
                {
                    Quaternion targetPlayerRotation = Quaternion.LookRotation(dirToEnemy);
                    playerTransform.rotation = Quaternion.Slerp(
                        playerTransform.rotation,
                        targetPlayerRotation,
                        smoothSpeed * Time.deltaTime);
                }

                if (lockCameraWhileLockedOn && playerController != null)
                {
                    playerController.LockCameraPosition = true;
                }
            }
        }
    }

    private void FindAndLockOn()
    {
        Collider[] allColliders = Physics.OverlapSphere(playerTransform.position, detectionRange, enemyLayer);

        // Filter to only colliders that have EnemyHealth component
        System.Collections.Generic.List<Transform> validEnemies = new System.Collections.Generic.List<Transform>();
        foreach (var col in allColliders)
        {
            EnemyHealth health = col.GetComponentInParent<EnemyHealth>();
            if (health != null && !validEnemies.Contains(health.transform))
            {
                validEnemies.Add(health.transform);
            }
        }

        if (validEnemies.Count > 0)
        {
            Transform nearest = null;
            float minDist = float.MaxValue;
            foreach (var enemyTransform in validEnemies)
            {
                float dist = Vector3.Distance(playerTransform.position, enemyTransform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = enemyTransform;
                }
            }
            if (nearest != null)
            {
                currentTarget = nearest;
                isLockedOn = true;

                // Salva rotação no momento da ativação (não no Start)
                if (cameraTarget != null)
                    originalCameraTargetRotation = cameraTarget.rotation;

                if (playerController != null)
                {
                    playerController.IsLockedOn = true;
                    if (lockCameraWhileLockedOn)
                        playerController.LockCameraPosition = true;
                }
                
                // Update indicator
                if (lockOnIndicator != null)
                {
                    LockOnIndicator indicator = lockOnIndicator.GetComponent<LockOnIndicator>();
                    if (indicator != null)
                    {
                        indicator.SetTarget(nearest);
                    }
                }
            }
        }
           else
           {
               isLockedOn = false;
               currentTarget = null;
           }
    }

    private void SwitchToNextTarget()
    {
        Collider[] allColliders = Physics.OverlapSphere(playerTransform.position, detectionRange, enemyLayer);
        System.Collections.Generic.List<Transform> validEnemies = new System.Collections.Generic.List<Transform>();
        foreach (var col in allColliders)
        {
            EnemyHealth health = col.GetComponentInParent<EnemyHealth>();
            if (health != null && !validEnemies.Contains(health.transform))
            {
                validEnemies.Add(health.transform);
            }
        }

        if (validEnemies.Count <= 1)
        {
            CancelLockOn();
            return;
        }

        // Find current target index
        int currentIndex = -1;
        for (int i = 0; i < validEnemies.Count; i++)
        {
            if (validEnemies[i] == currentTarget)
            {
                currentIndex = i;
                break;
            }
        }

        if (currentIndex == -1)
        {
            CancelLockOn();
            return;
        }

        // Get next target
        int nextIndex = (currentIndex + 1) % validEnemies.Count;
        currentTarget = validEnemies[nextIndex];

        // Camera will be rotated in LateUpdate
    }

    private void CancelLockOn()
    {
        if (lockOnIndicator != null)
        {
            LockOnIndicator indicator = lockOnIndicator.GetComponent<LockOnIndicator>();
            if (indicator != null)
                indicator.ClearTarget();
        }

        isLockedOn = false;
        currentTarget = null;

        if (playerController != null)
        {
            playerController.LockCameraPosition = false;
            playerController.OnLockOnCancelled(); // reseta IsLockedOn + sprint preso

            // Sincroniza _cinemachineTargetYaw/Pitch com a rotação salva
            // para evitar salto de câmera no frame seguinte
            if (cameraTarget != null)
            {
                Vector3 savedEuler = originalCameraTargetRotation.eulerAngles;
                float pitch = savedEuler.x > 180f ? savedEuler.x - 360f : savedEuler.x;
                playerController.SetCameraRotation(savedEuler.y, pitch);
            }
        }

        if (playerInteract != null)
            playerInteract.ResetDirection();
    }

    private bool IsTargetValid()
    {
        if (currentTarget == null) return false;

        float dist = Vector3.Distance(playerTransform.position, currentTarget.position);
        if (dist > detectionRange) return false;

        EnemyHealth health = currentTarget.GetComponent<EnemyHealth>();
        if (health != null && health.IsDead) return false;

        return true;
    }
}