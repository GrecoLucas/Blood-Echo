using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthSlider; // Arraste seu Heathbar aqui
    public GameOverMenu gameOverMenu;

    private bool isDead;

    void Start()
    {
        if (gameOverMenu == null)
        {
            gameOverMenu = FindFirstObjectByType<GameOverMenu>();
        }

        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        Debug.Log($"Player took {amount} damage!");
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        Debug.Log($"Player healed {amount} health!");
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("O Jogador Morreu!");

        if (gameOverMenu != null)
        {
            gameOverMenu.ShowGameOver();
        }
        else
        {
            Debug.LogError("GameOverMenu nao encontrado. Adiciona o script na cena ou liga o campo gameOverMenu no PlayerHealth.");
        }
    }
}