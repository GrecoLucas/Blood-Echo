using UnityEngine;
using UnityEngine.UI;
using StarterAssets;
public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthSlider; // Arraste seu Heathbar aqui
    public Slider maxHealthSlider;
    public GameOverMenu gameOverMenu;
    private ThirdPersonController _controller;    
    private bool isDead;

    void Start()
    {
        _controller = GetComponent<ThirdPersonController>();
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
        if (maxHealthSlider != null)
        {
            maxHealthSlider.maxValue = 300f;
            maxHealthSlider.value = maxHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        if (_controller != null && _controller.IsInvincible) 
        {
            Debug.Log("Esquivou! Dano ignorado por I-Frames.");
            return;
        }

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

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

private void Die()
{
    Debug.Log("O Jogador Morreu!");

    // Em vez de usar a variável 'gameOverMenu' que pode estar nula,
    // usamos o Singleton que criamos anteriormente.
    if (GameOverMenu.Instance != null)
    {
        GameOverMenu.Instance.ShowGameOver();
    }
    else
    {
        // Caso o Singleton ainda não tenha sido iniciado por algum motivo
        GameOverMenu menu = FindFirstObjectByType<GameOverMenu>();
        if (menu != null)
        {
            menu.ShowGameOver();
        }
        else
        {
            Debug.LogError("ERRO: Não encontrei nenhum GameOverMenu na cena!");
        }
    }
}
    public void ResetHealth() 
    {
        currentHealth = maxHealth;
        isDead = false;

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    public void IncreaseMaxHealth(float amount)
    {
        
        float newMaxHealth = maxHealth + amount;
        // currentHealth += amount; // heal the added amount too
        
        // Tell the UI to update
        FindAnyObjectByType<HealthBarUI>().UpdateHealthBar(maxHealth, newMaxHealth);
        maxHealth = newMaxHealth;
    }
}