using UnityEngine;

public class Interactor : MonoBehaviour
{
    
    public float distance = 6f;
    // No Inspector, vamos escolher o que o raio deve ignorar (o jogador)
    public LayerMask ignoreLayer; 
    public GameObject interactionPrompt; // UI para mostrar quando um objeto interagível é detectado

    private bool DetectInteraction(out IInteractable interactable)
    {
        interactable = null;
        
        // Criamos o raio a partir da câmera
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        
        // Desenha o raio no editor (Cena) para teste
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, 2f);

        RaycastHit hit;

        // O símbolo '~' antes de ignoreLayer diz ao Unity: 
        // "Acerte em TUDO, EXCETO no que estiver na ignoreLayer"
        if (Physics.Raycast(ray, out hit, distance, ~ignoreLayer))
        {
            Debug.Log("O raio atravessou o player e bateu em: " + hit.collider.name);
            
            interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                return true;
            } else
            {
                return false;
            }
        } else
        {
            return false;
        }
    }
    void Update()
    {
        if( DetectInteraction(out IInteractable interactable))
        {
            if (interactable.CanInteract())
            {
                interactionPrompt.SetActive(true); // Mostra o prompt de interação
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("O Unity detectou a tecla E!");
                    Debug.Log("Interagível detectada! Executando interação...");
                    interactable.Interact(this);
                }

            } else {
                interactionPrompt.SetActive(true);  // Esconde o prompt de interação
                Debug.Log("Objeto detectado, mas não interagível no momento.");
            }
        } else
        {
            interactionPrompt.SetActive(false); // Esconde o prompt de interação
        }

    }
}
