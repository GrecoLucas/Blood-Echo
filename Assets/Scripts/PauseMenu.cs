using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FMODUnity; // IMPORTANTE: Adicionado o namespace do FMOD

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    [Header("Interface")]
    public GameObject pausePanel;
    public GameObject controlsPanel;
    public GameObject hudPanel; // NOVO: Arraste o seu Canvas ou Painel de Vida/Stamina para cá no Inspector
    
    public Button resumeButton;
    public Button mainMenuButton;
    public Button controlsButton;
    public Button quitButton;
    public Button exitControlsButton;

    [Header("Player")]
    public StarterAssetsInputs playerInputs;

    private bool _isPaused = false;

    // NOVO: Referência para o controle global de áudio do FMOD
    private FMOD.Studio.Bus masterBus; 

    void Awake()
    {
        if (Instance == null) Instance = this;

        // NOVO: Inicializa a referência do Bus principal do FMOD (Controla todos os sons).
        // Nota: Se quiser pausar APENAS a música, mude "bus:/" para o nome do seu bus de música, ex: "bus:/Music"
        masterBus = RuntimeManager.GetBus("bus:/");

        if (pausePanel != null) pausePanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);

        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveListener(Resume);
            resumeButton.onClick.AddListener(Resume);
        }

        if (mainMenuButton != null) // Not implemented yet
        {
            mainMenuButton.onClick.RemoveListener(GoToMainMenu);
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(QuitGame);
            quitButton.onClick.AddListener(QuitGame);
        }

        if (controlsButton != null)
        {
            controlsButton.onClick.RemoveListener(ShowControls);
            controlsButton.onClick.AddListener(ShowControls);
        }

        if (exitControlsButton != null)
        {
            exitControlsButton.onClick.RemoveListener(HideControls);
            exitControlsButton.onClick.AddListener(HideControls);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!_isPaused) Pause();
            else Resume(); // NOVO: Permite despausar apertando 'P' novamente
        }
    }

    public void Pause()
    {
        if (pausePanel == null)
        {
            Debug.LogError("ERRO: O painel de Pause não foi arrastado no Inspector!");
            return;
        }

        _isPaused = true;
        pausePanel.SetActive(true);
        
        // NOVO: Esconde a barra de vida/stamina
        if (hudPanel != null) hudPanel.SetActive(false);

        // NOVO: Pausa todo o áudio do FMOD
        masterBus.setPaused(true);

        Time.timeScale = 0f;

        if (playerInputs == null) playerInputs = FindFirstObjectByType<StarterAssetsInputs>();

        if (playerInputs != null)
        {
            playerInputs.cursorLocked = false;
            playerInputs.cursorInputForLook = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        _isPaused = false;
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            controlsPanel.SetActive(false);
        } 

        // NOVO: Mostra novamente a barra de vida/stamina
        if (hudPanel != null) hudPanel.SetActive(true);

        // NOVO: Despausa o áudio do FMOD
        masterBus.setPaused(false);

        Time.timeScale = 1f;

        if (playerInputs != null)
        {
            playerInputs.cursorLocked = true;
            playerInputs.cursorInputForLook = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GoToMainMenu()
    {
        // Time.timeScale = 1f;
        //SceneManager.LoadScene("MainMenu"); // change
    }

    public void ShowControls()
    {
        if (controlsPanel != null) controlsPanel.SetActive(true);
    }

    public void HideControls()
    {
        if (controlsPanel != null) controlsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
        Debug.Log("Quit"); // only visible in editor
    }
}