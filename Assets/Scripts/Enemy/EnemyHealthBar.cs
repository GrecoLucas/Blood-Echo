using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow;
    
    private bool isHighlighted;

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
    
    public void SetHighlight(bool highlighted)
    {
        isHighlighted = highlighted;
        
        // Aumenta o tamanho da barra quando destacada
        if (highlighted)
        {
            transform.localScale = new Vector3(1.1f, 1.1f, 1f);
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }
}