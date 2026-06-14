using UnityEngine;
using TMPro;
using FMODUnity;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using StarterAssets;

public class Potions : MonoBehaviour
{
    [Header("Configurações de Cura")]
    public PlayerHealth playerHealth;
    public float healAmount = 50f;
    public int maxPotions = 4;
    public int potionCount = 4;

    [Header("Animator")]
    public Animator playerAnimator;

    [Header("Sound")]
    [SerializeField] private StudioEventEmitter healSoundEmitter;

    [Header("UI references")]
    public PotionsUI potionsUI; // drag the PotionsUI component here

    private KeyCode useKey = KeyCode.H;
    private static readonly int DrinkTrigger = Animator.StringToHash("Drink");
    private StarterAssetsInputs _input;

    void Start()
    {
        if (playerAnimator == null)
            playerAnimator = GetComponent<Animator>();

        if (healSoundEmitter == null)
            healSoundEmitter = GetComponent<StudioEventEmitter>();

        _input = GetComponent<StarterAssetsInputs>();

        UpdatePotionVisual();
    }

    void Update()
    {
        bool healPressed = false;

        // Input System unificado (teclado H / gamepad Triângulo/△)
        if (_input != null && _input.heal)
        {
            _input.heal = false; // Consome o input
            healPressed = true;
        }
        // Fallback para Input antigo
        else if (_input == null && Input.GetKeyDown(useKey))
        {
            healPressed = true;
        }

        // Verifica se apertou a tecla e se ainda tem poção disponível
        if (healPressed && potionCount > 0)
        {
            Debug.Log("Attempting to use potion: " + potionCount + " remaining");
            TryUsePotion();
        }
    }

    void TryUsePotion()
    {
        if (playerHealth != null)
        {
            if (playerHealth.currentHealth < playerHealth.maxHealth)
            {
                if (playerAnimator != null)
                    playerAnimator.SetTrigger(DrinkTrigger);

                if (healSoundEmitter != null)
                    healSoundEmitter.Play();

                playerHealth.Heal(healAmount);

                potionCount--;
                if (potionCount < 0) potionCount = 0;

                UpdatePotionVisual();
            }
        }
    }

    void UpdatePotionVisual()
    {
        if (potionsUI != null)
            potionsUI.UpdateVisual(potionCount, maxPotions);
    }

    public void RestorePotions()
    {
        potionCount = maxPotions;
        UpdatePotionVisual();
    }
}