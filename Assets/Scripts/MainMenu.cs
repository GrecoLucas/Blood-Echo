using UnityEngine;
using UnityEngine.SceneManagement; 
using FMODUnity; // Biblioteca necessária para integrar com o FMOD

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject controlsPanel;

    [Header("FMOD Audio")]
    [SerializeField] private EventReference musicEvent; // Permite escolher o evento de música no Inspector
    private FMOD.Studio.EventInstance musicInstance;    // Instância para podermos controlar (dar play/stop)

    void Start()
    {
        controlsPanel.SetActive(false);

        // Verifica se um evento foi selecionado no Inspector para evitar erros
        if (!musicEvent.IsNull)
        {
            // Cria a instância de áudio e inicia a reprodução
            musicInstance = RuntimeManager.CreateInstance(musicEvent);
            musicInstance.start();
        }
        else
        {
            Debug.LogWarning("Faltou colocar o Evento de Música do FMOD no Inspector do MainMenu!");
        }
    }

    private void Update()
    {
        if (controlsPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseControls();
        }
    }
    
    // Função chamada pelo botão "Jogar"
    public void PlayGame()
    {
        // Para a música de forma suave (Fade Out) antes de trocar de cena
        StopMusic(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Função chamada pelo botão "Sair"
    public void LeaveGame()
    {
        Debug.Log("The game is quitting!"); 
        
        // Para a música imediatamente ao fechar o jogo
        StopMusic(FMOD.Studio.STOP_MODE.IMMEDIATE);
        
        Application.Quit(); 
    }

    public void OpenControls()
    {
        controlsPanel.SetActive(true);
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
    }
    private void StopMusic(FMOD.Studio.STOP_MODE stopMode)
    {
        musicInstance.stop(stopMode);
        musicInstance.release(); // Libera a instância da memória (muito importante no FMOD!)
    }

    // Boa prática: Garante que a música vai parar caso o objeto seja destruído abruptamente
    private void OnDestroy()
    {
        StopMusic(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}