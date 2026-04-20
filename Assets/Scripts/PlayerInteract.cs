using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float distance = 6f;
    // No Inspector, vamos escolher o que o raio deve ignorar (o jogador)
    public LayerMask ignoreLayer; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("O Unity detectou a tecla E!");

            // Criamos o raio a partir da câmera
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            
            // Desenha o raio no editor (Cena) para teste
            Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, 2f);

            RaycastHit hit;

            // O símbolo '~' antes de ignoreLayer diz ao Unity: 
            // "Acerte em TUDO, EXCETO no que estiver na ignoreLayer"
            if (Physics.Raycast(ray, out hit, distance, ~ignoreLayer))
            {

                Door door = hit.collider.GetComponentInParent<Door>();

                if (door != null)
                {
                    door.ToggleDoor();
                }
            }
        }
    }
}