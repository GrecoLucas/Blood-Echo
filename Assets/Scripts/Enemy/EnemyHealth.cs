using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 50f;
    public float currentHealth;
    public bool destroyOnDeath = true;
    public float destroyDelay = 1.5f;

    private bool isDead;
    private Animator animator;

    public bool IsDead => isDead;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
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

    private void Die(){
            if (isDead) return;
            isDead = true;

            EnemyAI enemyAI = GetComponent<EnemyAI>();
            if (enemyAI != null) enemyAI.enabled = false;

            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.isStopped = true;
                agent.enabled = false;
            }

            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            if (animator != null)
                animator.SetTrigger("Die");

            // Destrói após a animação terminar
            if (destroyOnDeath)
                Destroy(gameObject, destroyDelay);
    }
}
