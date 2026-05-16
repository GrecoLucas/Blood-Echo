using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class WendigoAI : MonoBehaviour
{
    public enum AIState { Patrolling, Screaming, Chasing }
    public AIState currentState = AIState.Patrolling;

    [Header("Movement Speeds")]
    public float patrolSpeed = 2.5f;
    public float chaseSpeed = 6.0f;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float waitTimeAtPoint = 2f;
    private int currentPointIndex = 0;
    private bool isWaiting = false;

    [Header("Detection Settings")]
    public float viewRadius = 15f;
    [Range(0, 360)] public float viewAngle = 90f;
    public LayerMask obstacleMask;
    public float memoryTime = 5f;
    public float giveUpDistance = 25f;

    [Header("Chase Distance Settings")]
    public float maintainRadius = 3.0f;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform playerTarget;
    private float timeSinceLastSawPlayer;
    
    // NOVO: Guarda as coordenadas de onde o jogador foi visto pela última vez
    private Vector3 lastKnownPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerTarget = p.transform;

        agent.speed = patrolSpeed;
        agent.stoppingDistance = 0.5f; 

        if (patrolPoints.Length > 0)
        {
            GotoNextPoint();
        }
    }

    void Update()
    {
        if (playerTarget == null) return;

        bool canSeePlayer = CheckFieldOfView();

        switch (currentState)
        {
            case AIState.Patrolling:
                HandlePatrol(canSeePlayer);
                break;
            case AIState.Screaming:
                FaceTarget(playerTarget.position);
                break;
            case AIState.Chasing:
                HandleChase(canSeePlayer);
                
                // Se parou, fica a olhar para o sítio onde está o jogador (ou onde acha que ele está)
                if (agent.velocity.magnitude <= 0.1f)
                {
                    FaceTarget(canSeePlayer ? playerTarget.position : lastKnownPosition);
                }
                break;
        }

        UpdateAnimations();
    }

    void HandlePatrol(bool canSeePlayer)
    {
        if (canSeePlayer)
        {
            StartCoroutine(ScreamThenChase());
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !isWaiting)
        {
            StartCoroutine(WaitAndGoToNextPoint());
        }
    }

    IEnumerator ScreamThenChase()
    {
        currentState = AIState.Screaming;
        agent.isStopped = true;
        animator.SetTrigger("scream");
        
        yield return new WaitForSeconds(2.5f); 

        agent.isStopped = false;
        agent.speed = chaseSpeed;
        
        // Regista onde o jogador estava no momento do grito
        lastKnownPosition = playerTarget.position;
        
        currentState = AIState.Chasing;
    }

    void HandleChase(bool canSeePlayer)
    {
        float distToActualPlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (canSeePlayer)
        {
            // --- ELE VÊ O JOGADOR ---
            timeSinceLastSawPlayer = 0;
            
            // Atualiza constantemente a última posição conhecida
            lastKnownPosition = playerTarget.position; 
            
            // Mantém a distância de segurança para não entrar dentro do jogador
            agent.stoppingDistance = maintainRadius; 
            
            // Segue ativamente
            agent.SetDestination(playerTarget.position); 
        }
        else
        {
            // --- PERDEU O JOGADOR DE VISTA ---
            timeSinceLastSawPlayer += Time.deltaTime;

            if (timeSinceLastSawPlayer > memoryTime || distToActualPlayer > giveUpDistance)
            {
                ReturnToPatrol();
                return;
            }
            
            // Como vai investigar um local vazio (onde tu sumiste), permitimos que ele chegue bem perto
            agent.stoppingDistance = 0.5f; 
            
            // Segue para a ÚLTIMA POSIÇÃO CONHECIDA, e não para onde o jogador está escondido agora
            agent.SetDestination(lastKnownPosition); 
        }
    }

    void ReturnToPatrol()
    {
        currentState = AIState.Patrolling;
        agent.speed = patrolSpeed;
        agent.stoppingDistance = 0.5f; 
        GotoNextPoint();
    }

    void UpdateAnimations()
    {
        // A animação de grito continua isolada, mas a locomoção agora é fluida
        if (currentState == AIState.Screaming)
        {
            animator.SetFloat("Speed", 0f); // Para as pernas durante o grito
            return; 
        }
    
        // Calcula a velocidade desejada no momento do Chase
        if (currentState == AIState.Chasing)
        {
            float dist = Vector3.Distance(transform.position, playerTarget.position);
            
            // Define a velocidade do agente baseada na distância
            if (dist > maintainRadius + 1.5f) 
            {
                agent.speed = chaseSpeed;
            }
            else 
            {
                agent.speed = patrolSpeed;
            }
        }
        else if (currentState == AIState.Patrolling)
        {
            agent.speed = patrolSpeed;
        }
    
        // A MÁGICA ACONTECE AQUI:
        // Pega na velocidade real em que o agente se está a mover
        float currentSpeed = agent.velocity.magnitude;
    
        animator.SetFloat("Speed", currentSpeed, 0.1f, Time.deltaTime);
    }

    void FaceTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0; 
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    bool CheckFieldOfView() {
        if (playerTarget == null) return false;
        
        float dist = Vector3.Distance(transform.position, playerTarget.position);
        if (dist < viewRadius) {
            Vector3 dir = (playerTarget.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dir) < viewAngle / 2f) {
                if (!Physics.Raycast(transform.position + Vector3.up * 1.5f, dir, dist, obstacleMask)) return true;
            }
        }
        return false;
    }

    void GotoNextPoint() {
        if (patrolPoints.Length == 0) return;
        agent.isStopped = false; 
        agent.destination = patrolPoints[currentPointIndex].position;
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
    }

    IEnumerator WaitAndGoToNextPoint() {
        isWaiting = true;
        agent.isStopped = true; 
        yield return new WaitForSeconds(waitTimeAtPoint);
        if(currentState == AIState.Patrolling) GotoNextPoint();
        isWaiting = false;
    }
}