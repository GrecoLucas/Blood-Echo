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

    [Header("Configurações de Patrulha")]
    public Transform[] pontosPatrulha;
    public float tempoEsperaPatrulha = 2.0f;
    public float distanciaChegadaPatrulha = 0.5f;

    private int indicePontoPatrulha;
    private bool emEsperaPatrulha;
    private float fimEsperaPatrulha;
    private bool patrulhaInicializada;
    private Vector3 posicaoInicial;
    private Quaternion rotacaoInicial;
    private bool playerJaEstavaMorto;

    void Start()
    {
        posicaoInicial = transform.position;
        rotacaoInicial = transform.rotation;
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

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
            PatrulharPorPontos();
            return;
        }
        else
        {
            playerJaEstavaMorto = false;
        }

        // Lógica de I.A. Principal
        if (distanciaParaPlayer <= raioDeteccao)
        {
            patrulhaInicializada = false;
            emEsperaPatrulha = false;

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
            PatrulharPorPontos();
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
                animator.SetTrigger("Attack1"); 
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

    #region Lógica de Base (Movimento e Patrulha)
    void PatrulharPorPontos()
    {
        if (agent == null || !agent.isOnNavMesh || pontosPatrulha == null || pontosPatrulha.Length == 0)
        {
            if (agent != null) agent.isStopped = true;
            return;
        }

        if (!patrulhaInicializada)
        {
            indicePontoPatrulha = EncontrarPontoPatrulhaMaisProximo();
            DefinirDestinoPatrulhaAtual();
            patrulhaInicializada = true;
            return;
        }

        if (emEsperaPatrulha)
        {
            agent.isStopped = true;
            if (Time.time >= fimEsperaPatrulha)
            {
                emEsperaPatrulha = false;
                indicePontoPatrulha = (indicePontoPatrulha + 1) % pontosPatrulha.Length;
                DefinirDestinoPatrulhaAtual();
            }
            return;
        }

        agent.isStopped = false;
        bool chegouAoDestino = !agent.pathPending && agent.remainingDistance <= (agent.stoppingDistance + distanciaChegadaPatrulha);
        if (chegouAoDestino)
        {
            emEsperaPatrulha = true;
            fimEsperaPatrulha = Time.time + tempoEsperaPatrulha;
        }
    }

    void DefinirDestinoPatrulhaAtual()
    {
        if (pontosPatrulha[indicePontoPatrulha] != null)
        {
            agent.isStopped = false;
            agent.SetDestination(pontosPatrulha[indicePontoPatrulha].position);
        }
    }

    int EncontrarPontoPatrulhaMaisProximo()
    {
        int melhorIndice = 0;
        float menorDistancia = float.MaxValue;
        for (int i = 0; i < pontosPatrulha.Length; i++)
        {
            if (pontosPatrulha[i] == null) continue;
            float distancia = Vector3.SqrMagnitude(transform.position - pontosPatrulha[i].position);
            if (distancia < menorDistancia) { menorDistancia = distancia; melhorIndice = i; }
        }
        return melhorIndice;
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

    public void TeleportToOrigin()
    {
        if (agent != null) agent.enabled = false;
        transform.position = posicaoInicial;
        transform.rotation = rotacaoInicial;
        if (agent != null) agent.enabled = true;
        patrulhaInicializada = false;
        emEsperaPatrulha = false;
    }

    public void StartDealingDamage() { if(damageDealer != null) damageDealer.StartDealingDamage(); }
    public void EndDealingDamage() { if(damageDealer != null) damageDealer.EndDealingDamage(); }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, raioDeteccao);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, distanciaAtaqueMelee);
        Gizmos.color = Color.blue; Gizmos.DrawWireSphere(transform.position, rangeAttackDistance);
    }
}
