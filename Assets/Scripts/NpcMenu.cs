using UnityEngine;
using System.Collections;

public class NpcMenu : MonoBehaviour
{
    public GameObject targetNpc;
    public GameObject player;
    private Animator npcAnimator = null;
    private Animator playerAnimator = null;
    public bool deathTriggered = false;

    public bool IsDeathTriggered()
    {
        return deathTriggered;
    }

    private IEnumerator SequencePlayerAttacks()
{
    playerAnimator.SetTrigger("Draw");
    
    // Wait for the "Draw" animation to complete
    // Replace "Draw" with the exact name of the Animator state for the draw animation
/*     yield return new WaitUntil(() => 
        playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Draw Sword") && 
        playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
     */
    // Now set the attack trigger
    playerAnimator.SetBool("Armed", true);

    yield return new WaitUntil(() => 
    playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Armed Locomotion") && 
    playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

    playerAnimator.SetTrigger("Attack1");
    
}
    public void TriggerNpcDeath()
    {
        if (targetNpc != null)
        {
            npcAnimator = targetNpc.GetComponent<Animator>();
            if (npcAnimator != null)
            {
                npcAnimator.SetTrigger("Die");
                deathTriggered = true;
            }
            playerAnimator = player.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("Draw");
                playerAnimator.SetBool("Armed", true);
                playerAnimator.SetTrigger("Attack1");
                //StartCoroutine(SequencePlayerAttacks());
            }
        }
    }
}
