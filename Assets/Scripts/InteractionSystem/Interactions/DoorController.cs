using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    public Transform doorMesh;
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
    }

    public void UnlockDoor()
    {
        isLocked = false;
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
