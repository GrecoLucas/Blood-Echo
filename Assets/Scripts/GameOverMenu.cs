using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    [Header("Interface")]
    public GameObject gameOverPanel;
    public Button tryAgainButton;

    [Header("Player")]
    public StarterAssetsInputs playerInputs;

    void Awake()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (tryAgainButton != null)
        {
            tryAgainButton.onClick.RemoveListener(TryAgain);
            tryAgainButton.onClick.AddListener(TryAgain);
        }
    }

    

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;

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
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}