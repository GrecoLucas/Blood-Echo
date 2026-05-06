using UnityEngine;
using StarterAssets; // <- Adicionado para conseguirmos "conversar" com o controle do personagem
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class NpcController : MonoBehaviour, IInteractable
{
    public Transform npcMesh;
    public GameObject npcMenuCanvas;
    public GameObject exitCanvas;
    public KeyCode closeKey;
    public StarterAssets.ThirdPersonController playerController; // Reference to the player's movement script
    private StarterAssetsInputs playerInputs; 
    private bool isOpen = false;

    public bool CanInteract()
    {
        return true;
    }
    private void ShowNpcMenu(){
        npcMenuCanvas.SetActive(true);
        exitCanvas.SetActive(true);
        Time.timeScale = 0f;
        // Disable player movement
        if (playerController != null)
        {
            playerController.LockCameraPosition = true;

        }

        if (playerInputs != null)
        {
            playerInputs.cursorLocked = false;
            playerInputs.cursorInputForLook = false;
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void DisableNpcMenu(){
        npcMenuCanvas.SetActive(false);
        exitCanvas.SetActive(false);
        isOpen = false;
        Time.timeScale = 1f;
        // Enable player movement
        if (playerController != null)
        {
            playerController.LockCameraPosition = false;
        }
                // Devolve o controle da câmera e esconde o mouse
        
        if (playerInputs != null)
        {
            playerInputs.cursorLocked = true;
            playerInputs.cursorInputForLook = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void Interact(Interactor interactor)
    {
        
        isOpen = true;
        ShowNpcMenu();
    }

    void Update()
    {
        bool closePressed = false;

        if (isOpen)
        {
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
            if (Input.GetKeyDown(closeKey))
            {
                closePressed = true;
            }
        }

        if (closePressed)
        {
            DisableNpcMenu();
        }

        if (isOpen && npcMenuCanvas.GetComponent<NpcMenu>().IsDeathTriggered())
        {
            DisableNpcMenu();
        }
        
    }
}
