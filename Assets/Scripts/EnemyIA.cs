using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Animator animator;
    public DamageDealer damageDealer;

    [Header("Configurações de Detecção")]
    [Tooltip("Distância que o inimigo consegue ver o jogador")]
    public float raioDeteccao = 10.0f;

    [Header("Configurações de Combate")]
    public float distanciaAtaque = 2.0f;
    public float cooldownAtaque = 1.5f;
    private float tempoUltimoAtaque;

    void Update()
    {
        if (player == null) return;

        float distanciaParaPlayer = Vector3.Distance(transform.position, player.position);

        // 1. Lógica de Detecção: Ele só age se o player estiver dentro do raio
        if (distanciaParaPlayer <= raioDeteccao)
        {
            // Se estiver longe o suficiente para andar, mas perto o suficiente para ter visto
            if (distanciaParaPlayer > distanciaAtaque)
            {
                agent.isStopped = false; 
                agent.SetDestination(player.position);
            }
            else
            {
                PararERotacionar();
                TentarAtacar();
            }
        }
        else
        {
            // Se o player estiver fora do raio, o inimigo fica parado (Idle)
            agent.isStopped = true;
            agent.velocity = Vector3.zero; // Garante que ele não deslize
        }

        // 2. Sincroniza a animação de caminhada
        // Se o agente estiver parado pelo script, a velocidade será 0 e o Animator fará o Idle
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void PararERotacionar()
    {
        agent.isStopped = true; 
        Vector3 direcao = (player.position - transform.position).normalized;
        direcao.y = 0; 
        if (direcao.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direcao);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    void TentarAtacar()
    {
        if (Time.time >= tempoUltimoAtaque + cooldownAtaque)
        {
            animator.SetTrigger("Attack1"); 
            tempoUltimoAtaque = Time.time;
            Debug.Log("Inimigo atacou o player!");
        }
    }

    // --- ESSA PARTE É MÁGICA: Desenha o círculo de visão no Unity Editor ---
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioDeteccao);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }

    public void StartDealingDamage() 
    {
        if(damageDealer != null) damageDealer.StartDealingDamage();
    }
    
    public void EndDealingDamage() 
    {
        if(damageDealer != null) damageDealer.EndDealingDamage();
    }
}