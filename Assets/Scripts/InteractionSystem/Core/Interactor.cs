using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using StarterAssets;

public class Interactor : MonoBehaviour
{
    
    public float distance = 6f;
    [Tooltip("Raio de proximidade para interação (não precisa mirar)")]
    public float proximityRadius = 3f;
    // No Inspector, vamos escolher o que o raio deve ignorar (o jogador)
    public LayerMask ignoreLayer; 
    public GameObject interactionPrompt; // UI para mostrar quando um objeto interagível é detectado

    private StarterAssetsInputs _input;

    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        if (_input == null)
        {
            _input = GetComponentInParent<StarterAssetsInputs>();
        }
    }

    /// Detecta se o botão de interação foi pressionado (teclado E, gamepad □)
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

    private bool DetectInteraction(out IInteractable interactable)
    {
        interactable = null;
        
        // 1) Tenta raycast da câmera (funciona bem com mouse)
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, 2f);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance, ~ignoreLayer))
        {            
            interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                return true;
            }
        }

        // 2) Fallback: procura por proximidade (melhor para gamepad)
        interactable = FindNearestInteractable();
        return interactable != null;
    }

    /// Procura o IInteractable mais próximo dentro do proximityRadius usando OverlapSphere.
    private IInteractable FindNearestInteractable()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, proximityRadius, ~ignoreLayer);
        IInteractable nearest = null;
        float minDist = float.MaxValue;

        foreach (var col in colliders)
        {
            IInteractable target = col.GetComponent<IInteractable>();
            if (target != null && target.CanInteract())
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = target;
                }
            }
        }

        return nearest;
    }

    void Update()
    {
        if( DetectInteraction(out IInteractable interactable))
        {
            if (interactable.CanInteract())
            {

                if (IsInteractPressed())
                {
                    Debug.Log("Interactor: Interacting with " + interactable.ToString());
                    interactable.Interact(this);
                }

            } 
        }

    }
}
