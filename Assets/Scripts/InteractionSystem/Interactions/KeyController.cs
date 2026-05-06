using UnityEngine;

public class KeyController : MonoBehaviour, IInteractable
{
    [SerializeField] DoorController doorController;
    private Inventory _inventory;
    public Transform keyMesh;

    public void Start()
    {
        _inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Inventory>();
        if (_inventory == null)
        {
            Debug.LogError("KeyController: Inventory not found on Player object.");
        }
    }

    public bool CanInteract()
    {
        return true;
    }

    public void Interact(Interactor interactor)
    {
        
        doorController.UnlockDoor();
        _inventory.AddKey();
        Destroy(gameObject);
        Debug.Log("Chave coletada!");
    }
}
