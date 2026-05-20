using UnityEngine;
using UnityEngine.AI;


/// Inimigo de Stealth com cone de visão, raio de audição e máquina de estados.
///
/// ESTADOS:
///   Patrol   → anda entre pontos, velocidade normal/lenta
///   Alert    → ouviu algo, vai investigar o som, velocidade média
///   Chase    → viu o jogador, persegue e ataca, velocidade alta
///   Return   → perdeu o jogador, volta a patrulhar (em alerta temporário)
///
/// DETEÇÃO:
///   • Visão   – cone à frente (ângulo + distância) com Raycast para obstáculos
///   • Audição – raio à volta do inimigo (sem linha de visão necessária)
///               O player deve chamar NotifySound(Vector3 pos, float raio) neste componente
///               ou usar o método estático EnemyStealth.AlertNearby(pos, raio).
public class EnemyStealth : MonoBehaviour
{
    // Referências
    [Header("Referências")]
    public NavMeshAgent agent;
    public Transform    player;
    public Animator     animator;
    public DamageDealer damageDealer;

    // Visão
    [Header("Configurações de Visão")]
    [Tooltip("Distância máxima do cone de visão")]
    public float distanciaVisao    = 12f;
    [Tooltip("Ângulo total do cone (ex: 90 = 45° para cada lado)")]
    public float anguloVisao       = 90f;
    [Tooltip("Layer(s) que bloqueiam a linha de visão (paredes, obstáculos)")]
    public LayerMask layerObstaculo;

    // Audição
    [Header("Configurações de Audição")]
    [Tooltip("Raio em que o inimigo ouve sons produzidos pelo jogador")]
    public float raioAudicao = 6f;

    // Combate
    [Header("Configurações de Combate")]
    public float distanciaAtaque = 2f;
    public float cooldownAtaque  = 1.5f;
    private float tempoUltimoAtaque;

    // Alerta
    [Header("Configurações de Alerta")]
    [Tooltip("Quanto tempo (s) o inimigo fica em estado de alerta após perder o jogador")]
    public float duracaoAlerta = 5f;
    [Tooltip("Quanto tempo (s) o inimigo investiga o último ponto antes de voltar a patrulhar")]
    public float duracaoInvestigacao = 4f;
    [Tooltip("Distância para considerar que chegou ao ponto de investigação")]
    public float distanciaChegadaInvestigacao = 0.8f;

    // Velocidades
    [Header("Velocidades")]
    public float velocidadePatrulha    = 2f;
    public float velocidadeAlerta      = 3.5f;
    public float velocidadePerseguicao = 5f;

    // Patrulha 
    [Header("Configurações de Patrulha")]
    [Tooltip("Lista de pontos que o inimigo percorre em sequência")]
    public Transform[] pontosPatrulha;
    [Tooltip("Tempo de espera em cada ponto")]
    public float tempoEsperaPatrulha = 2f;
    [Tooltip("Distância para considerar que chegou ao ponto")]
    public float distanciaChegadaPatrulha = 0.5f;


    public enum Estado { Patrol, Alert, Chase, Return }
    private Estado estadoAtual = Estado.Patrol;

    private Vector3    posicaoInicial;
    private Quaternion rotacaoInicial;
    private bool       playerJaEstavaMorto;

    // Patrulha
    private int   indicePontoPatrulha;
    private bool  emEsperaPatrulha;
    private float fimEsperaPatrulha;
    private bool  patrulhaInicializada;

    // Alerta / investigação
    private Vector3 pontoInvestigacao;
    private float   fimAlerta;
    private bool    chegouAoInvestigar;
    private float   fimInvestigacao;


    void Start()
    {
        posicaoInicial = transform.position;
        rotacaoInicial = transform.rotation;
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.speed = velocidadePatrulha;
    }

    void Update()
    {
        if (player == null) return;

        // Verificar se o player morreu 
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        bool playerMorto = playerHealth != null && playerHealth.currentHealth <= 0;

        if (playerMorto)
        {
            if (!playerJaEstavaMorto)
            {
                TeleportToOrigin();
                playerJaEstavaMorto = true;
            }
            animator.SetFloat("Speed", 0f);
            return;
        }
        else
        {
            playerJaEstavaMorto = false;
        }

        switch (estadoAtual)
        {
            case Estado.Patrol: UpdatePatrol(); break;
            case Estado.Alert:  UpdateAlert();  break;
            case Estado.Chase:  UpdateChase();  break;
            case Estado.Return: UpdateReturn(); break;
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    //  ESTADO: PATROL
    void UpdatePatrol()
    {
        agent.speed = velocidadePatrulha;

        // Verifica deteção
        if (PodeVerPlayer())
        {
            EntrarEmChase();
            return;
        }
        if (PodeOuvirPlayer())
        {
            EntrarEmAlerta(player.position);
            return;
        }

        PatrulharPorPontos();
    }

    //  ESTADO: ALERT  (ouviu algo, vai investigar)
    void UpdateAlert()
    {
        agent.speed = velocidadeAlerta;

        // Durante investigação, continua a verificar se vê o player
        if (PodeVerPlayer())
        {
            EntrarEmChase();
            return;
        }

        // Chegou ao ponto de investigação?
        if (!chegouAoInvestigar)
        {
            bool chegou = !agent.pathPending &&
                          agent.remainingDistance <= (agent.stoppingDistance + distanciaChegadaInvestigacao);
            if (chegou)
            {
                chegouAoInvestigar = true;
                fimInvestigacao = Time.time + duracaoInvestigacao;
                agent.isStopped = true;
            }
        }
        else
        {
            // Aguarda na posição, olha à volta (rotação lenta)
            transform.Rotate(Vector3.up, 60f * Time.deltaTime);

            if (Time.time >= fimInvestigacao)
            {
                // Fim do alerta → Return
                EntrarEmReturn();
            }
        }

        // Timeout de segurança: se demorou demasiado sem chegar
        if (Time.time >= fimAlerta && !chegouAoInvestigar)
        {
            EntrarEmReturn();
        }
    }

    //  ESTADO: CHASE  (persegue e ataca)
    void UpdateChase()
    {
        agent.speed = velocidadePerseguicao;

        if (PodeVerPlayer())
        {
            // Continua a ver → atualiza destino
            fimAlerta = Time.time + duracaoAlerta; // reinicia o timeout

            float dist = Vector3.Distance(transform.position, player.position);
            if (dist > distanciaAtaque)
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
            // Perdeu de vista
            pontoInvestigacao = player.position; // último local conhecido
            EntrarEmReturn();
        }
    }


    void UpdateReturn()
    {
        agent.speed = velocidadeAlerta;

        // Ainda pode apanhar o player durante o alerta
        if (PodeVerPlayer())
        {
            EntrarEmChase();
            return;
        }
        if (PodeOuvirPlayer())
        {
            EntrarEmAlerta(player.position);
            return;
        }

        // Vai ao último ponto conhecido
        if (!chegouAoInvestigar)
        {
            agent.isStopped = false;
            agent.SetDestination(pontoInvestigacao);

            bool chegou = !agent.pathPending &&
                          agent.remainingDistance <= (agent.stoppingDistance + distanciaChegadaInvestigacao);
            if (chegou)
            {
                chegouAoInvestigar = true;
                fimInvestigacao = Time.time + duracaoInvestigacao;
                agent.isStopped = true;
            }
        }
        else
        {
            // Fica parado um momento antes de voltar
            transform.Rotate(Vector3.up, 60f * Time.deltaTime);

            if (Time.time >= fimInvestigacao || Time.time >= fimAlerta)
            {
                EntrarEmPatrol();
            }
        }
    }


    void EntrarEmChase()
    {
        estadoAtual = Estado.Chase;
        fimAlerta   = Time.time + duracaoAlerta;
        agent.isStopped = false;
    }

    void EntrarEmAlerta(Vector3 posicaoSom)
    {
        estadoAtual           = Estado.Alert;
        pontoInvestigacao     = posicaoSom;
        chegouAoInvestigar    = false;
        fimAlerta             = Time.time + duracaoAlerta + duracaoInvestigacao;
        agent.isStopped       = false;
        agent.SetDestination(pontoInvestigacao);
    }

    void EntrarEmReturn()
    {
        estadoAtual        = Estado.Return;
        chegouAoInvestigar = false;
        fimAlerta          = Time.time + duracaoAlerta;
        agent.isStopped    = false;
        agent.SetDestination(pontoInvestigacao);
    }

    void EntrarEmPatrol()
    {
        estadoAtual          = Estado.Patrol;
        patrulhaInicializada = false;
        emEsperaPatrulha     = false;
        agent.isStopped      = false;
        agent.speed          = velocidadePatrulha;
    }


    // Cone de visão com Raycast para obstáculos.
    bool PodeVerPlayer()
    {
        Vector3 dirParaPlayer = player.position - transform.position;
        float distancia = dirParaPlayer.magnitude;

        if (distancia > distanciaVisao) return false;

        float angulo = Vector3.Angle(transform.forward, dirParaPlayer);
        if (angulo > anguloVisao / 2f) return false;

        // Verifica linha de visão (ignora o próprio collider)
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f,
                            dirParaPlayer.normalized,
                            out RaycastHit hit,
                            distancia,
                            layerObstaculo))
        {
            return false; // há obstáculo no meio
        }

        return true;
    }

    bool PodeOuvirPlayer()
    {
        return Vector3.Distance(transform.position, player.position) <= raioAudicao;
    }


    public void NotifySound(Vector3 posicaoSom, float raioSom)
    {
        if (estadoAtual == Estado.Chase) return; // já está em perseguição, ignora
        float dist = Vector3.Distance(transform.position, posicaoSom);
        if (dist <= raioSom)
        {
            EntrarEmAlerta(posicaoSom);
        }
    }

    /// Alerta todos os inimigos EnemyStealth num raio.
    public static void AlertNearby(Vector3 posicaoSom, float raioSom)
    {
        foreach (EnemyStealth e in FindObjectsOfType<EnemyStealth>())
        {
            e.NotifySound(posicaoSom, raioSom);
        }
    }


    void PatrulharPorPontos()
    {
        if (agent == null || !agent.isOnNavMesh) return;

        if (pontosPatrulha == null || pontosPatrulha.Length == 0)
        {
            agent.isStopped = true;
            return;
        }

        if (!patrulhaInicializada)
        {
            indicePontoPatrulha  = EncontrarPontoPatrulhaMaisProximo();
            DefinirDestinoPatrulhaAtual();
            patrulhaInicializada = true;
            return;
        }

        if (emEsperaPatrulha)
        {
            agent.isStopped = true;
            if (Time.time >= fimEsperaPatrulha)
            {
                emEsperaPatrulha    = false;
                indicePontoPatrulha = (indicePontoPatrulha + 1) % pontosPatrulha.Length;
                DefinirDestinoPatrulhaAtual();
            }
            return;
        }

        agent.isStopped = false;
        bool chegou = !agent.pathPending &&
                      agent.remainingDistance <= (agent.stoppingDistance + distanciaChegadaPatrulha);
        if (chegou)
        {
            emEsperaPatrulha = true;
            fimEsperaPatrulha = Time.time + tempoEsperaPatrulha;
        }
    }

    void DefinirDestinoPatrulhaAtual()
    {
        if (pontosPatrulha[indicePontoPatrulha] == null) return;
        agent.isStopped = false;
        agent.SetDestination(pontosPatrulha[indicePontoPatrulha].position);
    }

    int EncontrarPontoPatrulhaMaisProximo()
    {
        int   melhorIndice   = 0;
        float menorDistancia = float.MaxValue;
        for (int i = 0; i < pontosPatrulha.Length; i++)
        {
            if (pontosPatrulha[i] == null) continue;
            float d = Vector3.SqrMagnitude(transform.position - pontosPatrulha[i].position);
            if (d < menorDistancia) { menorDistancia = d; melhorIndice = i; }
        }
        return melhorIndice;
    }

    void PararERotacionar()
    {
        agent.isStopped = true;
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(dir),
                                                  Time.deltaTime * 5f);
    }

    void TentarAtacar()
    {
        if (Time.time >= tempoUltimoAtaque + cooldownAtaque)
        {
            animator.SetTrigger("Attack1");
            tempoUltimoAtaque = Time.time;
        }
    }

    public void TeleportToOrigin()
    {
        if (agent != null) agent.enabled = false;
        transform.position   = posicaoInicial;
        transform.rotation   = rotacaoInicial;
        if (agent != null) agent.enabled = true;
        patrulhaInicializada = false;
        emEsperaPatrulha     = false;
        EntrarEmPatrol();
    }

    // Damage Dealer (chamado por Animation Events)
    public void StartDealingDamage() { if (damageDealer != null) damageDealer.StartDealingDamage(); }
    public void EndDealingDamage()   { if (damageDealer != null) damageDealer.EndDealingDamage();   }


    //  GIZMOS  (visíveis no Editor)
    private void OnDrawGizmosSelected()
    {
        // Raio de audição
        Gizmos.color = new Color(0f, 0.8f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, raioAudicao);

        // Cone de visão
        Gizmos.color = Color.yellow;
        float metadeAngulo = anguloVisao / 2f;
        Vector3 limEsq = Quaternion.Euler(0, -metadeAngulo, 0) * transform.forward * distanciaVisao;
        Vector3 limDir = Quaternion.Euler(0,  metadeAngulo, 0) * transform.forward * distanciaVisao;
        Gizmos.DrawLine(transform.position, transform.position + limEsq);
        Gizmos.DrawLine(transform.position, transform.position + limDir);
        Gizmos.DrawWireSphere(transform.position, distanciaVisao); // alcance máximo

        // Distância de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);

        // Estado atual (cor do eixo Y)
        Gizmos.color = estadoAtual switch
        {
            Estado.Chase  => Color.red,
            Estado.Alert  => Color.yellow,
            Estado.Return => Color.cyan,
            _             => Color.green
        };
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2f);
    }
}