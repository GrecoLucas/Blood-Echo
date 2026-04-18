using UnityEngine;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    public Transform doorMesh;
    public float openAngle = 90f;
    public float speed = 3f;

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
        Quaternion target = isOpen ? openRot : closedRot;
        doorMesh.rotation = Quaternion.Slerp(doorMesh.rotation, target, Time.deltaTime * speed);
    }

    private void ToggleDoor()
    {
        isOpen = !isOpen;
    }

    public void Interact(Interactor interactor)
    {
        ToggleDoor();
    }
}
