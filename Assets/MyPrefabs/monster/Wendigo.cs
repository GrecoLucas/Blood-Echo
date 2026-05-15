using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class WendigoPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float waitTimeAtPoint = 2f;

    private NavMeshAgent agent;
    private Animator animator;
    private int currentPointIndex = 0;
    private bool isWaiting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Começar a patrulhar logo no início se houver pontos
        if (patrolPoints.Length > 0)
        {
            GotoNextPoint();
        }
        else
        {
            Debug.LogWarning("O Wendigo não tem pontos de patrulha assinalados!");
        }
    }

    void Update()
    {
        // Verifica se o monstro chegou ao destino e não está já à espera
        if (!agent.pathPending && agent.remainingDistance < 0.5f && !isWaiting)
        {
            StartCoroutine(WaitAndGoToNextPoint());
        }

        // Atualizar o Animator: se a velocidade do agente for maior que 0.1, ele está a andar
        bool isMoving = agent.velocity.magnitude > 0.1f;
        animator.SetBool("isWalking", isMoving);
    }

    void GotoNextPoint()
    {
        if (patrolPoints.Length == 0) return;

        // Define o destino do agente para a posição do ponto atual
        agent.destination = patrolPoints[currentPointIndex].position;

        // Escolhe o próximo ponto no array (volta ao zero quando chegar ao fim)
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
    }

    IEnumerator WaitAndGoToNextPoint()
    {
        isWaiting = true;
        
        // Espera o tempo definido
        yield return new WaitForSeconds(waitTimeAtPoint);
        
        // Vai para o próximo ponto
        GotoNextPoint();
        
        isWaiting = false;
    }
}