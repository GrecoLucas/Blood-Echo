using UnityEngine;
using StarterAssets; // <- Adicionado para conseguirmos "conversar" com o controle do personagem
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class BonfireController : MonoBehaviour, IInteractable
{
    public Transform bonefireMesh;
    public GameObject menuFogueira;
    public GameObject exitCanvas;
    public KeyCode closeKey;
    // Variável para guardar os controles do jogador e pedir o mouse emprestado
    private StarterAssetsInputs playerInputs; 
    private bool isMenuOpen = false;
    [Header("Bonfire VFX")]
    public GameObject fireObject;
    public bool isDefaultBonfire = false; // Marque como TRUE apenas na fogueira inicial da fase
    [Header("Spawn Point")]
    public Transform playerSp;
    private void ShowMenu(){
        menuFogueira.SetActive(true);
        exitCanvas.SetActive(true);
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
        exitCanvas.SetActive(false);
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
        ActivateThisBonfire();

        if(GameManager.Instance != null)
        {
            Vector3 spawnPos = playerSp != null ? playerSp.position : transform.position;
            GameManager.Instance.RestAtBonfire(spawnPos);
        }

        ShowMenu();
    }
    private void ActivateThisBonfire()
    {
        // Desativa o fogo de todas as fogueiras na cena
        BonfireController[] allBonfires = FindObjectsByType<BonfireController>(FindObjectsSortMode.None);
        foreach (var b in allBonfires)
        {
            if (b.fireObject != null) b.fireObject.SetActive(false);
        }

        // Ativa apenas o fogo desta
        if (fireObject != null) fireObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        if (isMenuOpen)
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
            if (Input.GetKeyDown(closeKey))
            {
                closePressed = true;
            }

            if (closePressed)
            {
                 DisableMenu();
            }
        }  
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.lastBonfirePosition != Vector3.zero)
            {
                float distancia = Vector3.Distance(GameManager.Instance.lastBonfirePosition, playerSp.position);

                // Se a distância for muito pequena (quase zero), é a mesma fogueira
                if (distancia < 0.1f)
                {
                    if (fireObject != null) fireObject.SetActive(true);
                    return; // Encontramos a ativa, não precisamos checar o 'isDefault'
                }
                else
                {
                    // Se o GameManager já tem uma posição salva e não é esta, apagamos o fogo
                    if (fireObject != null) fireObject.SetActive(false);
                    return;
                }
            }
        }

        // usamos a lógica da fogueira default que você já tinha
        if (fireObject != null)
        {
            fireObject.SetActive(isDefaultBonfire);
        }

        if (isDefaultBonfire && GameManager.Instance != null && playerSp != null)
        {
            GameManager.Instance.lastBonfirePosition = playerSp.position;
        }
    }
}
