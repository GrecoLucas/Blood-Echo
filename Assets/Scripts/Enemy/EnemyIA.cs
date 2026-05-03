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

    [Header("Configurações de Patrulha")]
    [Tooltip("Lista de pontos que o inimigo vai percorrer em sequência")]
    public Transform[] pontosPatrulha;
    [Tooltip("Tempo que o inimigo espera em cada ponto antes de avançar")]
    public float tempoEsperaPatrulha = 2.0f;
    [Tooltip("Distância para considerar que chegou ao destino da patrulha")]
    public float distanciaChegadaPatrulha = 0.5f;

    private int indicePontoPatrulha;
    private bool emEsperaPatrulha;
    private float fimEsperaPatrulha;
    private bool patrulhaInicializada;
    private Vector3 posicaoInicial;
    private Quaternion rotacaoInicial;
    private bool playerJaEstavaMorto; // Para garantir que o TP só aconteça uma vez

    void Start()
    {
        posicaoInicial = transform.position;
        rotacaoInicial = transform.rotation;
        if (agent == null) agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (player == null) return;

        // Verifica se o jogador tem o script de vida e se está morto
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        bool playerMorto = playerHealth != null && playerHealth.currentHealth <= 0;

        float distanciaParaPlayer = Vector3.Distance(transform.position, player.position);

        // Lógica de Teleporte Automático: Se o player morreu e ainda não demos TP
        if (playerMorto)
        {
            if (!playerJaEstavaMorto)
            {
                TeleportToOrigin();
                playerJaEstavaMorto = true;
            }
        }
        else
        {
            playerJaEstavaMorto = false;
        }

        // 1. Lógica de Detecção: Ele só age se o player estiver dentro do raio e VIVO
        if (distanciaParaPlayer <= raioDeteccao && !playerMorto)
        {
            patrulhaInicializada = false;
            emEsperaPatrulha = false;

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
            // Fora do raio de deteção, o inimigo patrulha por pontos.
            PatrulharPorPontos();
        }

        // 2. Sincroniza a animação de caminhada
        // Se o agente estiver parado pelo script, a velocidade será 0 e o Animator fará o Idle
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void PatrulharPorPontos()
    {
        if (agent == null || !agent.isOnNavMesh)
        {
            return;
        }

        if (pontosPatrulha == null || pontosPatrulha.Length == 0)
        {
            agent.isStopped = true;
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

        bool chegouAoDestino = !agent.pathPending &&
                              agent.remainingDistance <= (agent.stoppingDistance + distanciaChegadaPatrulha);

        if (chegouAoDestino)
        {
            emEsperaPatrulha = true;
            fimEsperaPatrulha = Time.time + tempoEsperaPatrulha;
        }
    }

    void DefinirDestinoPatrulhaAtual()
    {
        Transform pontoAtual = pontosPatrulha[indicePontoPatrulha];

        if (pontoAtual == null)
        {
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(pontoAtual.position);
    }

    int EncontrarPontoPatrulhaMaisProximo()
    {
        int melhorIndice = 0;
        float menorDistancia = float.MaxValue;

        for (int i = 0; i < pontosPatrulha.Length; i++)
        {
            if (pontosPatrulha[i] == null)
            {
                continue;
            }

            float distancia = Vector3.SqrMagnitude(transform.position - pontosPatrulha[i].position);
            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                melhorIndice = i;
            }
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

    void TentarAtacar()
    {
        if (Time.time >= tempoUltimoAtaque + cooldownAtaque)
        {
            animator.SetTrigger("Attack1"); 
            tempoUltimoAtaque = Time.time;
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
    public void TeleportToOrigin()
    {
        if (agent != null) agent.enabled = false;
        transform.position = posicaoInicial;
        transform.rotation = rotacaoInicial;
        if (agent != null) agent.enabled = true;
        patrulhaInicializada = false;
        emEsperaPatrulha = false;
    }
}