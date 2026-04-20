using UnityEngine;

public class KeyController : MonoBehaviour, IInteractable
{
    [SerializeField] DoorController doorController;
    public Transform keyMesh;

    public bool CanInteract()
    {
        return true;
    }

    public void Interact(Interactor interactor)
    {
        
        Destroy(gameObject);
        doorController.UnlockDoor();
        Debug.Log("Chave coletada!");
    }
}
