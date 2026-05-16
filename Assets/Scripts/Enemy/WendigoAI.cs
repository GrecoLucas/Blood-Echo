using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class WendigoAI : MonoBehaviour
{
    public enum AIState { Patrolling, Screaming, Chasing, Attacking }
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

    [Header("Chase & Attack Settings")]
    public float maintainRadius = 1.5f; // Diminuído para ele chegar mais perto
    public float attackDistance = 2.0f; // Distância para iniciar o ataque
    public float attackCooldown = 3.0f; // Tempo entre um combo e outro
    public float comboDuration = 2.5f;  // Tempo que dura a sua animação Attack1 + Attack2 juntas
    private bool canAttack = true;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform playerTarget;
    private float timeSinceLastSawPlayer;
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
                CheckForAttack(); // Verifica se está perto o suficiente para bater
                
                if (agent.velocity.magnitude <= 0.1f)
                {
                    FaceTarget(canSeePlayer ? playerTarget.position : lastKnownPosition);
                }
                break;
                
            case AIState.Attacking:
                // Continua a olhar para o player enquanto faz o combo de ataques
                FaceTarget(playerTarget.position);
                break;
        }

        UpdateAnimations();
    }

    // --- NOVA LÓGICA DE ATAQUE ---
    void CheckForAttack()
    {
        if (!canAttack) return;

        float distToActualPlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (distToActualPlayer <= attackDistance)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        // 1. Muda o estado e para o monstro
        currentState = AIState.Attacking;
        agent.isStopped = true;
        canAttack = false;

        // 2. Aciona o Trigger que você criou no Animator
        animator.SetTrigger("attack");

        // 3. Espera o tempo das animações do combo terminarem
        // Ajuste 'comboDuration' no Inspector de acordo com a duração das suas 2 animações
        yield return new WaitForSeconds(comboDuration);

        // 4. Volta a perseguir
        agent.isStopped = false;
        currentState = AIState.Chasing;

        // 5. Espera o cooldown para poder atacar novamente (evita spam de ataques)
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    // -----------------------------

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
        lastKnownPosition = playerTarget.position;
        currentState = AIState.Chasing;
    }

    void HandleChase(bool canSeePlayer)
    {
        float distToActualPlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (canSeePlayer)
        {
            timeSinceLastSawPlayer = 0;
            lastKnownPosition = playerTarget.position; 
            agent.stoppingDistance = maintainRadius; 
            agent.SetDestination(playerTarget.position); 
        }
        else
        {
            timeSinceLastSawPlayer += Time.deltaTime;

            if (timeSinceLastSawPlayer > memoryTime || distToActualPlayer > giveUpDistance)
            {
                ReturnToPatrol();
                return;
            }
            
            agent.stoppingDistance = 0.5f; 
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
        // Se estiver a gritar ou a atacar, a velocidade nas pernas é 0
        if (currentState == AIState.Screaming || currentState == AIState.Attacking)
        {
            animator.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
            return;
        }

        if (currentState == AIState.Chasing)
        {
            float dist = Vector3.Distance(transform.position, playerTarget.position);
            agent.speed = (dist > maintainRadius + 1.5f) ? chaseSpeed : patrolSpeed;
        }
        else if (currentState == AIState.Patrolling)
        {
            agent.speed = patrolSpeed;
        }

        // Mistura as animações de Locomotion (Blend Tree) suavemente
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