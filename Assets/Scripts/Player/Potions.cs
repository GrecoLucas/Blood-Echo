using UnityEngine;
using TMPro;
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

    [Header("Referências da UI")]
    public GameObject potionNumberText;
    public GameObject healingObject1;  
    public GameObject healingObject2;
    public GameObject healingObject3;  
    public GameObject healingObject4;   
    public GameObject emptyObject;    
    private KeyCode useKey = KeyCode.H; 
    private static readonly int DrinkTrigger = Animator.StringToHash("Drink");

    TMP_Text potionNumberTMP;
    UnityEngine.UI.Text potionNumberLegacy;
    private StarterAssetsInputs _input;

    void Start()
    {
        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
        }

        if (potionNumberText != null)
        {
            potionNumberTMP = potionNumberText.GetComponent<TMP_Text>();
            potionNumberLegacy = potionNumberText.GetComponent<UnityEngine.UI.Text>();
        }

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
                {
                    playerAnimator.SetTrigger(DrinkTrigger);
                }

                playerHealth.Heal(healAmount);

                potionCount--;
                if (potionCount < 0) potionCount = 0;

                UpdatePotionVisual();
            }
        }
    }

    void UpdatePotionVisual()
    {
        if (healingObject1 != null) healingObject1.SetActive(potionCount >= 4);
        if (healingObject2 != null) healingObject2.SetActive(potionCount == 3);
        if (healingObject3 != null) healingObject3.SetActive(potionCount == 2);
        if (healingObject4 != null) healingObject4.SetActive(potionCount == 1);
        if (emptyObject != null) emptyObject.SetActive(potionCount <= 0);
        if (potionNumberTMP != null) {potionNumberTMP.text = potionCount.ToString();}
    }

    public void RestorePotions()
    {
        potionCount = maxPotions;
        UpdatePotionVisual();
    }
}