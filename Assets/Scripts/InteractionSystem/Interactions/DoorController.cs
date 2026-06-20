using UnityEngine;
using Unity.AI.Navigation; // Necessário para acessar o NavMeshLink[cite: 11]

public class DoorController : MonoBehaviour, IInteractable
{
    public NavMeshLink navLink; // Arraste o NavMeshLink para cá no Inspector
    public float openAngle = 90f;
    public float speed = 3f;
    public bool isLocked;

    private bool isOpen = false;
    private Quaternion closedRot;
    private Quaternion openRot;
    private bool _keyConsumed = false;

    public bool CanInteract()
    {
        return true;
    }

    private bool wasLocked;

    void Start()
    {
        wasLocked = isLocked;
        closedRot = transform.rotation;
        openRot = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
        
        // Garante que o link comece desativado, pois a porta inicia fechada
        if (navLink != null)
        {
            navLink.activated = false;
        }
    }

    void Update()
    {
        if (isLocked){
            return;}

        Quaternion target = isOpen ? openRot : closedRot;
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * speed);
    }

    private void ToggleDoor()
    {
        isOpen = !isOpen; 
        Debug.Log(isOpen ? "Porta aberta!" : "Porta fechada!");
        if (isOpen && wasLocked && !_keyConsumed)
        {
            _keyConsumed = true;
            var inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Inventory>();
            if (inventory != null)
                inventory.RemoveKey();
            Debug.Log("Porta aberta! Chave removida do inventário.");
        }

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
    }

    public void Interact(Interactor interactor)
    {
        if (isLocked)
        {
            // DOOR LOCKED - Do nothing
        }
        else {
            ToggleDoor(); 
        }
        
    }
}