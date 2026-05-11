using UnityEngine;
using System.Collections;

public class NpcMenu : MonoBehaviour
{
    public GameObject targetNpc;
    public GameObject player;
    private Animator npcAnimator = null;
    public bool deathTriggered = false;

    public void TriggerDrink()
    {
        if (targetNpc != null)
        {
            npcAnimator = targetNpc.GetComponent<Animator>();
            if (npcAnimator != null)
            {
                npcAnimator.SetTrigger("Drink");
            }
        }
    }

    public void TriggerNpcDeath()
    {
        if (targetNpc != null)
        {
            npcAnimator = targetNpc.GetComponent<Animator>();
            if (npcAnimator != null)
            {
                npcAnimator.SetTrigger("Die"); // Gatilho para cair no chão
                deathTriggered = true;
            }
        }
    }
}