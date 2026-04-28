using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public static GameOverMenu Instance; // Singleton para fácil acesso

    [Header("Interface")]
    public GameObject gameOverPanel;
    public Button tryAgainButton;

    [Header("Player")]
    public StarterAssetsInputs playerInputs;

    void Awake()
    {
        // Configuração do Singleton
        if (Instance == null) Instance = this;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (tryAgainButton != null)
        {
            tryAgainButton.onClick.RemoveListener(TryAgain);
            tryAgainButton.onClick.AddListener(TryAgain);
        }
    }

    public void ShowGameOver()
    {
        // Se o painel não existir, o jogo não pausa para não travar
        if (gameOverPanel == null) {
            Debug.LogError("ERRO: O painel de GameOver não foi arrastado no Inspector!");
            return;
        }

        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Pausa DEPOIS de mostrar o painel

        // Tentamos encontrar os inputs caso a referência tenha se perdido no reload
        if (playerInputs == null) playerInputs = FindFirstObjectByType<StarterAssetsInputs>();

        if (playerInputs != null)
        {
            playerInputs.cursorLocked = false;
            playerInputs.cursorInputForLook = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void TryAgain()
    {
        // 1. Volta o tempo ao normal
        Time.timeScale = 1f;
    
        // 2. Esconde o menu
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    
        // 3. Chama o respawn manual no GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RespawnPlayer();
        }
    }
}