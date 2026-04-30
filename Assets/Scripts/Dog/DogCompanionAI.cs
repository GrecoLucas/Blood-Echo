using UnityEngine;
using UnityEngine.AI; // Necessário para o NavMesh
using ithappy.Animals_FREE;

public class DogCompanionAI : MonoBehaviour, IInteractable
{
    public float followDistance = 3f;
    
    private CreatureMover m_Mover;
    private NavMeshAgent m_Agent;
    public Transform m_Player;
    private bool m_IsTamed = false;

    private void Awake()
    {
        m_Mover = GetComponent<CreatureMover>();
        m_Agent = GetComponent<NavMeshAgent>();
        
        // Importante: O NavMeshAgent calcula o caminho, 
        // mas o CreatureMover cuidará de mover e rotacionar
        m_Agent.updatePosition = false; 
        m_Agent.updateRotation = false;
    }

    private void Update()
    {
        if (!m_IsTamed || m_Player == null) return;

        // 1. Define o destino no NavMesh
        m_Agent.nextPosition = transform.position;
        m_Agent.SetDestination(m_Player.position);

        // 2. Calcula a direção baseada no caminho do NavMesh (desvia de obstáculos)
        Vector3 direction = m_Agent.desiredVelocity.normalized;
        float distance = Vector3.Distance(transform.position, m_Player.position);

        Vector2 moveAxis = Vector2.zero;
        if (distance > followDistance)
        {
            // Transforma a direção do mundo para o espaço local do cachorro
            moveAxis = new Vector2(0, 1); 
        }

        // 3. Passa a direção e o alvo para o Mover rotacionar e animar corretamente[cite: 9]
        // Usamos transform.position + direction como target para ele olhar para onde está andando
        m_Mover.SetInput(moveAxis, transform.position + direction, distance > followDistance * 2, false);
    }

    public bool CanInteract() => !m_IsTamed;

    public void Interact(Interactor interactor)
    {
        m_IsTamed = true;
    }
}