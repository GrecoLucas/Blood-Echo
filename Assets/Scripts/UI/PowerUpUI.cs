using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarterAssets;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


public class PowerUpUI : MonoBehaviour
{
    public static PowerUpUI Instance;

    [Header("References")]
    public GameObject powerUpCanvas;
    public Image icon;
    public TMP_Text powerUpName;
    public TMP_Text description;
    public StarterAssets.ThirdPersonController playerController; // Reference to the player's movement script


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        powerUpCanvas.SetActive(false);   
    }

    // Update is called once per frame
    void Update()
    {
        if (!powerUpCanvas.activeSelf) return;
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
        if (closePressed)
        {
            HidePowerUp();
            powerUpCanvas.SetActive(false);
        }
    }

    public void ShowPowerUp(PowerUpEffect effect)
    {
        icon.sprite = effect.icon;
        powerUpName.text = effect.powerUpName;
        description.text = effect.powerUpDescription;

        powerUpCanvas.SetActive(true);

        Time.timeScale = 0f;
        // Disable player movement
        if (playerController != null)
        {
            playerController.LockCameraPosition = true;

        }
    }

    private void HidePowerUp()
    {
        powerUpCanvas.SetActive(false);

        Time.timeScale = 1f;
        // Enable player movement
        if (playerController != null)
        {
            playerController.LockCameraPosition = false;
        }
    }
}
