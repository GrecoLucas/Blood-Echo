using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para carregar cenas

public class MainMenu : MonoBehaviour
{
    public GameObject controlsPanel;
    // Inicializa o painel de controles como desativado
    void Start()
    {
        controlsPanel.SetActive(false);
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
        // Carrega a próxima cena na fila do Build Settings.
        // Você também pode usar SceneManager.LoadScene("NomeDaSuaCena");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Função chamada pelo botão "Sair"
    public void LeaveGame()
    {
        Debug.Log("The game is quitting!"); // Isso só aparece no editor da Unity
        Application.Quit(); // Isso fecha o jogo real depois de compilado
    }

    public void OpenControls()
    {
        controlsPanel.SetActive(true);
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
    }
}