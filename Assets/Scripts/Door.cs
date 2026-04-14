using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform doorMesh;
    public float openAngle = 90f;
    public float speed = 3f;

    private bool isOpen = false;
    private Quaternion closedRot;
    private Quaternion openRot;

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

    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }
}