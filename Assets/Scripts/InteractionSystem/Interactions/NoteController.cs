using UnityEngine;
using TMPro;

public class NoteController : MonoBehaviour, IInteractable
{
    public Transform noteMesh;
    public KeyCode closeKey;
    public GameObject noteCanvas;
    public TMP_Text noteTextComponent;
    public string noteContent;
    public StarterAssets.ThirdPersonController playerController; // Reference to the player's movement script

    private bool isOpen = false;
    public bool CanInteract()
    {
        return true;
    }

    private void ShowNote(){
            // Show the note UI
            noteTextComponent.text = noteContent;
            noteCanvas.SetActive(true);
            // Disable player movement
            if (playerController != null)
            {
                playerController.enabled = false;
            }
    }

    private void DisableNote(){
            // Hide the note UI
            noteCanvas.SetActive(false);
            noteTextComponent.text = null;
            isOpen = false;

            // Enable player movement
            if (playerController != null)
            {
                playerController.enabled = true;
            }
            return;
    }


    public void Interact(Interactor interactor)
    {
        isOpen = true;
        ShowNote();
    }

    void Update()
    {
        if (isOpen)
        {
            
            if ( Input.GetKeyDown(closeKey)){
                 DisableNote();
            }
        }

    }


}
