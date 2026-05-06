using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using StarterAssets;

public class PlayerInteract : MonoBehaviour
{
    public float distance = 6f;
    [Tooltip("Raio de proximidade para interação (não precisa mirar)")]
    public float proximityRadius = 3f;
    // No Inspector, vamos escolher o que o raio deve ignorar (o jogador)
    public LayerMask ignoreLayer; 

    private Vector3 customDirection;
    private bool useCustomDirection;
    private StarterAssetsInputs _input;

    public void SetDirection(Vector3 dir)
    {
        customDirection = dir;
        useCustomDirection = true;
    }

    public void ResetDirection()
    {
        useCustomDirection = false;
    }

    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
    }

    /// <summary>
    /// Detecta se o botão de interação foi pressionado (teclado E, gamepad □)
    /// </summary>
    private bool IsInteractPressed()
    {
        // 1) Via Input System flag
        if (_input != null && _input.interact)
        {
            return true;
        }

#if ENABLE_INPUT_SYSTEM
        // 2) Fallback: leitura direta do teclado
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            return true;
        }

        // 3) Fallback: leitura direta do gamepad (□ = buttonWest)
        if (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame)
        {
            return true;
        }
#else
        if (Input.GetKeyDown(KeyCode.E))
        {
            return true;
        }
#endif

        return false;
    }

    void Update()
    {
        if (IsInteractPressed())
        {
            Debug.Log("PlayerInteract: Interact pressed!");

            // 1) Tenta raycast da câmera (funciona bem com mouse)
            Vector3 direction = useCustomDirection ? customDirection : Camera.main.transform.forward;
            Ray ray = new Ray(Camera.main.transform.position, direction);
            
            Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, 2f);

            RaycastHit hit;
            bool found = false;

            if (Physics.Raycast(ray, out hit, distance, ~ignoreLayer))
            {
                Door door = hit.collider.GetComponentInParent<Door>();
                if (door != null)
                {
                    door.ToggleDoor();
                    found = true;
                }
            }

            // 2) Se o raycast não encontrou nada, tenta por proximidade
            if (!found)
            {
                Door nearestDoor = FindNearestInteractable<Door>();
                if (nearestDoor != null)
                {
                    Debug.Log("PlayerInteract: Found door by proximity!");
                    nearestDoor.ToggleDoor();
                }
            }
        }
    }

    /// <summary>
    /// Procura o componente T mais próximo do player dentro do proximityRadius.
    /// </summary>
    private T FindNearestInteractable<T>() where T : Component
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, proximityRadius, ~ignoreLayer);
        T nearest = null;
        float minDist = float.MaxValue;

        foreach (var col in colliders)
        {
            T target = col.GetComponentInParent<T>();
            if (target != null)
            {
                float dist = Vector3.Distance(transform.position, target.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = target;
                }
            }
        }

        return nearest;
    }
}