using UnityEngine;
using UnityEngine.AI;
using ithappy.Animals_FREE;

public class DogCompanionAI : MonoBehaviour, IInteractable
{
    public float followDistance = 3f;
    [Header("UI Interaction")]
    public GameObject promptUI; 
    private CreatureMover m_Mover;
    private NavMeshAgent m_Agent;
    public Transform m_Player;
    private bool m_IsTamed = false;

    private void Awake()
    {
        m_Mover = GetComponent<CreatureMover>();
        m_Agent = GetComponent<NavMeshAgent>();
        
        m_Agent.updatePosition = false; 
        m_Agent.updateRotation = false;
    }

    private void Update()
    {
        if (!m_IsTamed || m_Player == null) return;

        m_Agent.nextPosition = transform.position;
        m_Agent.SetDestination(m_Player.position);

        Vector3 direction = m_Agent.desiredVelocity.normalized;
        float distance = Vector3.Distance(transform.position, m_Player.position);

        Vector2 moveAxis = Vector2.zero;
        if (distance > followDistance)
        {
            moveAxis = new Vector2(0, 1); 
        }

        m_Mover.SetInput(moveAxis, transform.position + direction, distance > followDistance * 2, false);
    }

    public bool CanInteract() => !m_IsTamed;

    public void Interact(Interactor interactor)
    {
        m_IsTamed = true;

        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }
    }
}