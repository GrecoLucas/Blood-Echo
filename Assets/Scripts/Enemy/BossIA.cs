using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    [Header("Referências")]
    public NavMeshAgent agent;
    public Transform player;
    public Animator animator;
    public DamageDealer damageDealer;

    [Header("Configurações de Detecção")]
    public float raioDeteccao = 15.0f;

    [Header("Configurações de Combate Melee")]
    public float distanciaAtaqueMelee = 2.5f;
    public float cooldownAtaqueMelee = 1.5f;
    private float tempoUltimoAtaqueMelee;

    [Header("Configurações de Magia (Range)")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float rangeAttackDistance = 15f;
    public float rangeAttackCooldown = 4f;
    private float lastRangeAttackTime;
    private Vector3 posicaoInicial;
    private Quaternion rotacaoInicial;
    private bool playerJaEstavaMorto;
    private bool proximoAtaqueMeleeEh1 = true;
    private bool isStunned;
    private Coroutine stunRoutine;

    void Start()
    {
        posicaoInicial = transform.position;
        rotacaoInicial = transform.rotation;
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isStunned) return;
        if (player == null) return;
        // Se o boss estiver morto, não faz nada
        EnemyHealth myHealth = GetComponent<EnemyHealth>();
        if (myHealth != null && myHealth.IsDead) return;

        // Verifica status do player
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        bool playerMorto = playerHealth != null && playerHealth.currentHealth <= 0;

        float distanciaParaPlayer = Vector3.Distance(transform.position, player.position);

        // Lógica de Teleporte ao morrer
        if (playerMorto)
        {
            if (!playerJaEstavaMorto)
            {
                TeleportToOrigin();
                playerJaEstavaMorto = true;
            }
            if (agent != null) agent.isStopped = true;
            return;
        }
        else
        {
            playerJaEstavaMorto = false;
        }

        // Lógica de I.A. Principal
        if (distanciaParaPlayer <= raioDeteccao)
        {
            // Se estiver fora de alcance de qualquer ataque, persegue
            if (distanciaParaPlayer > distanciaAtaqueMelee && distanciaParaPlayer > rangeAttackDistance)
            {
                agent.isStopped = false; 
                agent.SetDestination(player.position);
            }
            else
            {
                PararERotacionar();
                TentarAtacar(distanciaParaPlayer);
            }
        }
        else
        {
            // Se o player sair do raio de detecção, o boss para
            if (agent != null) agent.isStopped = true;
        }

        // Sincroniza animação de movimento
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void TentarAtacar(float distancia)
    {
        // 1. Prioridade: Ataque Melee se estiver muito perto
        if (distancia <= distanciaAtaqueMelee)
        {
            if (Time.time >= tempoUltimoAtaqueMelee + cooldownAtaqueMelee)
            {
                if (proximoAtaqueMeleeEh1)
                {
                    animator.SetTrigger("Attack1");
                }
                else
                {
                    animator.SetTrigger("Attack2");
                }

                proximoAtaqueMeleeEh1 = !proximoAtaqueMeleeEh1;
                tempoUltimoAtaqueMelee = Time.time;
            }
        }
        // 2. Prioridade: Ataque de Magia se estiver no range
        else if (distancia <= rangeAttackDistance)
        {
            if (Time.time >= lastRangeAttackTime + rangeAttackCooldown)
            {
                animator.SetTrigger("RangeAttack");
                lastRangeAttackTime = Time.time;
            }
            else 
            {
                // Se a magia está em cooldown, ele continua tentando chegar perto para o melee
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
        }
    }

    public void PerformRangeAttack()
    {
        if (player == null) return;
        
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        bool playerMorto = playerHealth != null && playerHealth.currentHealth <= 0;
        float distanciaAtual = Vector3.Distance(transform.position, player.position);

        if (playerMorto || distanciaAtual > rangeAttackDistance + 5f)
        {
            Debug.Log("Boss: Ataque cancelado porque o player morreu ou está longe.");
            return;
        }

        if (projectilePrefab != null && shootPoint != null)
        {
            GameObject projectileObj = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
            EnemyProjectile projectile = projectileObj.GetComponent<EnemyProjectile>();
            if (projectile != null)
            {
                // Mira levemente acima do pé do player (no peito)
                Vector3 targetPos = player.position + Vector3.up * 1.2f; 
                projectile.Setup(targetPos);
            }
        }
    }

    #region Lógica de Base (Movimento)
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

    public void TeleportToOrigin()
    {
        if (agent != null) agent.enabled = false;
        transform.position = posicaoInicial;
        transform.rotation = rotacaoInicial;
        if (agent != null) agent.enabled = true;
    }

    public void StartDealingDamage() { if(damageDealer != null) damageDealer.StartDealingDamage(); }
    public void EndDealingDamage() { if(damageDealer != null) damageDealer.EndDealingDamage(); }
    public void OnStun(float duration)
    {
        if (stunRoutine != null) StopCoroutine(stunRoutine);
        stunRoutine = StartCoroutine(StunForSeconds(duration));
    }
    private IEnumerator StunForSeconds(float duration)
    {
        isStunned = true;

        if (agent != null) agent.isStopped = true; // Para o movimento físico

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsStunned", true); // ATIVA O BOOL NO ANIMATOR
        }

        yield return new WaitForSeconds(duration); // Espera o tempo exato

        if (animator != null)
        {
            animator.SetBool("IsStunned", false); // DESATIVA O BOOL
        }

        if (agent != null) agent.isStopped = false; // Retoma movimento

        isStunned = false;
        stunRoutine = null;
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, raioDeteccao);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, distanciaAtaqueMelee);
        Gizmos.color = Color.blue; Gizmos.DrawWireSphere(transform.position, rangeAttackDistance);
    }
}
