using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 50f;
    public float currentHealth;
    public bool destroyOnDeath = true;
    public float destroyDelay = 1.5f;

    private bool isDead;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
    
        isDead = true;
    
        // Desativa a lógica e o agente
        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null) enemyAI.enabled = false;
    
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }
    
        // DESAPARECER NA HORA: Desativa o objeto visualmente
        // Isso faz com que ele suma da tela imediatamente, mesmo antes do Destroy
        gameObject.SetActive(false); 
    
        if (destroyOnDeath)
        {
            // Agora o delay pode ser 0 ou qualquer valor, 
            // pois o objeto já está invisível e desativado
            Destroy(gameObject, 0f); 
        }
    }
}