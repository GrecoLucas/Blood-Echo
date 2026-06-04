using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para carregar cenas

public class MainMenu : MonoBehaviour
{
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
}