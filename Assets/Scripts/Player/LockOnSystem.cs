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
    [SerializeField] private Transform lockOnIndicator;

    private Transform currentTarget;
    private bool isLockedOn = false;
    private PlayerInteract playerInteract;
    private Quaternion originalCameraTargetRotation;

    void Start()
    {
        if (playerTransform != null)
        {
            playerInteract = playerTransform.GetComponent<PlayerInteract>();
        }
        if (playerController == null && playerTransform != null)
        {
            playerController = playerTransform.GetComponent<ThirdPersonController>();
        }
        if (cameraTarget == null && playerController != null && playerController.CinemachineCameraTarget != null)
        {
            cameraTarget = playerController.CinemachineCameraTarget.transform;
        }
        if (cameraTarget != null)
        {
            originalCameraTargetRotation = cameraTarget.rotation;
        }
    }

    void Update()
    {
        if ((Keyboard.current != null && Keyboard.current.lKey.wasPressedThisFrame) || (Mouse.current != null && Mouse.current.middleButton.wasPressedThisFrame))
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
            }
            else
            {
                if (cameraTarget != null && playerController != null)
                {
                    // Calculate direction from camera to target
                    Vector3 directionToTarget = (currentTarget.position - cameraTarget.position).normalized;
                    
                    // Create target rotation using LookRotation
                    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
                    
                    // Get current rotation
                    Quaternion currentRotation = cameraTarget.rotation;
                    
                    // Smoothly interpolate using Slerp (more stable for rotations)
                    Quaternion smoothedRotation = Quaternion.Slerp(currentRotation, targetRotation, smoothSpeed * Time.deltaTime);
                    
                    // Extract yaw and pitch from the smoothed rotation
                    Vector3 eulerAngles = smoothedRotation.eulerAngles;
                    float yaw = eulerAngles.y;
                    float pitch = eulerAngles.x;
                    
                    // Normalize pitch for proper clamping
                    if (pitch > 180) pitch -= 360;
                    
                    playerController.SetCameraRotation(yaw, pitch);
                }

                if (playerInteract != null)
                {
                    Vector3 directionToTargetFromPlayer = (currentTarget.position - playerTransform.position).normalized;
                    playerInteract.SetDirection(directionToTargetFromPlayer);
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
        // Clear indicator
        if (lockOnIndicator != null)
        {
            LockOnIndicator indicator = lockOnIndicator.GetComponent<LockOnIndicator>();
            if (indicator != null)
            {
                indicator.ClearTarget();
            }
        }
        
        isLockedOn = false;
        currentTarget = null;
        if (playerController != null)
        {
            playerController.LockCameraPosition = false;
        }
        if (cameraTarget != null)
        {
            cameraTarget.rotation = originalCameraTargetRotation;
        }
        if (playerInteract != null)
        {
            playerInteract.ResetDirection();
        }
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