using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private EnemyHealth enemyHealth; // Referência ao seu script existente

    void Start()
    {
        // Inicializa o slider com a vida máxima
        slider.maxValue = enemyHealth.maxHealth;
        slider.value = enemyHealth.currentHealth;
    }

    void Update()
    {
        // Mantém o slider atualizado com a vida atual
        slider.value = enemyHealth.currentHealth;
        
        // Opcional: Esconde a barra se o inimigo morrer
        if (enemyHealth.currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}