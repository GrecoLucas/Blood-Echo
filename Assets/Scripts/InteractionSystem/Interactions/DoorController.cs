using UnityEngine;
using Unity.AI.Navigation; // Necessário para acessar o NavMeshLink[cite: 11]

public class DoorController : MonoBehaviour, IInteractable
{
    public Transform doorMesh;
    public NavMeshLink navLink; // Arraste o NavMeshLink para cá no Inspector
    public float openAngle = 90f;
    public float speed = 3f;
    public bool isLocked = true;

    private bool isOpen = false;
    private Quaternion closedRot;
    private Quaternion openRot;

    public bool CanInteract()
    {
        return true;
    }

    void Start()
    {
        closedRot = doorMesh.rotation;
        openRot = Quaternion.Euler(doorMesh.eulerAngles + new Vector3(0, openAngle, 0));
        
        // Garante que o link comece desativado, pois a porta inicia fechada
        if (navLink != null)
        {
            navLink.activated = false;
        }
    }

    void Update()
    {
        if (isLocked) return;

        Quaternion target = isOpen ? openRot : closedRot;
        doorMesh.rotation = Quaternion.Slerp(doorMesh.rotation, target, Time.deltaTime * speed);
    }

    private void ToggleDoor()
    {
        isOpen = !isOpen; 
        
        // O NavMeshLink agora funciona estritamente baseado no abrir/fechar
        if (navLink != null)
        {
            navLink.activated = isOpen; 
            Debug.Log(isOpen ? "Porta aberta: Link ativado." : "Porta fechada: Link desativado.");
        }
    }

    public void UnlockDoor()
    {
        isLocked = false; 
        Debug.Log("Porta destrancada! Interaja para abrir.");
    }

    public void Interact(Interactor interactor)
    {
        if (isLocked)
        {
            Debug.Log("A porta está trancada!");
            return;
        }
        ToggleDoor(); 
    }
}