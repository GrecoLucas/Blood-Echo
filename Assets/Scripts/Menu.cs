using UnityEngine;
using StarterAssets; // <- Adicionado para conseguirmos "conversar" com o controle do personagem
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class Menu : MonoBehaviour
{
    [Header("Interface")]
    public GameObject menuFogueira;

    private bool jogadorPerto = false;
    
    // Variável para guardar os controles do jogador e pedir o mouse emprestado
    private StarterAssetsInputs playerInputs; 

    void Update()
    {
        if (jogadorPerto)
        {
            bool pressE = false;

            #if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null) pressE = Keyboard.current.eKey.wasPressedThisFrame;
            #else
            pressE = Input.GetKeyDown(KeyCode.E);
            #endif

            if (pressE)
            {
                AbrirOuFecharMenu();
            }
        }
    }

    public void AbrirOuFecharMenu()
    {
        bool menuAberto = menuFogueira.activeSelf;
        menuFogueira.SetActive(!menuAberto);

        if (!menuAberto) // Abrindo o menu
        {
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
        else // Fechando o menu
        {
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = true;
            // Pega as configurações de controle do jogador silenciosamente
            playerInputs = other.GetComponent<StarterAssetsInputs>();
            Debug.Log("Pressione 'E'");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
            
            // Segurança: Se o jogador sair empurrado de perto, fecha o menu automaticamente
            if (menuFogueira.activeSelf)
            {
                AbrirOuFecharMenu();
            }
            playerInputs = null;
        }
    }
}