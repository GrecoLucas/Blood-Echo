using UnityEngine;
using TMPro;
using StarterAssets;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class NoteController : MonoBehaviour, IInteractable
{
    public Transform noteMesh;
    public KeyCode closeKey;
    public GameObject noteCanvas;
    public GameObject exitCanvas;
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
        exitCanvas.SetActive(true);
        Time.timeScale = 0f;
        // Disable player movement
        if (playerController != null)
        {
            playerController.LockCameraPosition = true;

        }
    }

    private void DisableNote(){
            // Hide the note UI
            noteCanvas.SetActive(false);
            exitCanvas.SetActive(false);
            noteTextComponent.text = null;
            isOpen = false;
            Time.timeScale = 1f;
            // Enable player movement
            if (playerController != null)
            {
                playerController.LockCameraPosition = false;
            }
            
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
            bool closePressed = false;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && (Keyboard.current.tabKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame))
            {
                closePressed = true;
            }
            if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                closePressed = true;
            }
#endif
            // Fallback para o closeKey definido no Inspector
            if (Input.GetKeyDown(closeKey))
            {
                closePressed = true;
            }

            if (closePressed)
            {
                 DisableNote();
            }
        }
    }


}
