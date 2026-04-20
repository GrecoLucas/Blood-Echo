using UnityEngine;
using StarterAssets; // <- Adicionado para conseguirmos "conversar" com o controle do personagem
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class BonfireController : MonoBehaviour, IInteractable
{
    public Transform bonefireMesh;
    public GameObject menuFogueira;
    public KeyCode closeKey;
    // Variável para guardar os controles do jogador e pedir o mouse emprestado
    private StarterAssetsInputs playerInputs; 
    private bool isMenuOpen = false;

    private void ShowMenu(){
        menuFogueira.SetActive(true);
        Time.timeScale = 0f; // Pausa o jogo
        
        // Pede para o StarterAssets parar de controlar a câmera e soltar o mouse
        if (playerInputs != null)
        {
            playerInputs.cursorLocked = false;
            playerInputs.cursorInputForLook = false;
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void DisableMenu(){
        isMenuOpen = false;
        menuFogueira.SetActive(false);
        Time.timeScale = 1f; // Volta o jogo
        
        // Devolve o controle da câmera e esconde o mouse
        if (playerInputs != null)
        {
            playerInputs.cursorLocked = true;
            playerInputs.cursorInputForLook = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool CanInteract()
    {
        return true;
    }


    public void Interact(Interactor interactor)
    {
        isMenuOpen = true;
        ShowMenu();
    }
    // Update is called once per frame
    void Update()
    {
        if (isMenuOpen)
        {
            if ( Input.GetKeyDown(closeKey)){
                 DisableMenu();
            }
        }  
    }
}
