using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    [Header("Interface")]
    public GameObject gameOverPanel;
    public Button tryAgainButton;

    [Header("Auto Find (optional)")]
    public string gameOverPanelName = "GameOverPanel";
    public string tryAgainButtonName = "TryAgainButton";

    [Header("Player")]
    public StarterAssetsInputs playerInputs;

    void Awake()
    {
        AutoWireReferences();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("GameOverMenu: gameOverPanel nao foi atribuido no Inspector.");
        }
    }

    

    public void ShowGameOver()
    {
        if (gameOverPanel == null)
        {
            AutoWireReferences();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("GameOverMenu: sem painel de Game Over. Atribui gameOverPanel no Inspector.");
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

    private void AutoWireReferences()
    {
        if (playerInputs == null)
        {
            playerInputs = FindFirstObjectByType<StarterAssetsInputs>();
        }

        if (gameOverPanel == null && !string.IsNullOrWhiteSpace(gameOverPanelName))
        {
            gameOverPanel = FindSceneObjectByName(gameOverPanelName);
        }

        if (tryAgainButton == null)
        {
            if (gameOverPanel != null)
            {
                Button[] buttons = gameOverPanel.GetComponentsInChildren<Button>(true);
                foreach (Button btn in buttons)
                {
                    if (btn != null && btn.name == tryAgainButtonName)
                    {
                        tryAgainButton = btn;
                        break;
                    }
                }

                if (tryAgainButton == null && buttons.Length > 0)
                {
                    tryAgainButton = buttons[0];
                }
            }

            if (tryAgainButton == null && !string.IsNullOrWhiteSpace(tryAgainButtonName))
            {
                GameObject buttonObject = FindSceneObjectByName(tryAgainButtonName);
                if (buttonObject != null)
                {
                    tryAgainButton = buttonObject.GetComponent<Button>();
                }
            }
        }

        if (tryAgainButton != null)
        {
            tryAgainButton.onClick.RemoveListener(TryAgain);
            tryAgainButton.onClick.AddListener(TryAgain);
        }
        else
        {
            Debug.LogWarning("GameOverMenu: botao Try Again nao encontrado automaticamente.");
        }
    }

    private GameObject FindSceneObjectByName(string objectName)
    {
        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform t in allTransforms)
        {
            if (t == null) continue;
            if (!t.gameObject.scene.IsValid()) continue;
            if (t.hideFlags != HideFlags.None) continue;

            if (t.name == objectName)
            {
                return t.gameObject;
            }
        }

        return null;
    }
}