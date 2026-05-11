using UnityEngine;
using TMPro;
using System.Collections;
using StarterAssets;

public class NpcController : MonoBehaviour, IInteractable
{
    [Header("UI References")]
    public GameObject npcMenuCanvas; 
    public TextMeshProUGUI dialogueText;
    public NpcMenu npcMenu; 
    [Header("Visual Effects")]
    public GameObject potionInHand;
    private bool isDead = false;
    [Header("Dialogue Settings")]
    public float typingSpeed = 0.05f;
    private int dialogueIndex = 0;
    private bool isTyping = false;
    private string currentFullText = "";
    private bool isCinematicRunning = false;
    private string[] dialogues = {
        "Thank goodness... someone to help.",
        "This king is crazy. I was just a simple servant, but his curse poisoned everyone in this castle.",
        "Luckily I have a strong mind and was able to run... but I am hurt. I need a potion.",
        "I will never let anyone enter the King's chamber. The key stays with me to protect everyone.",
        "I need healing... please..."
    };

    public bool CanInteract() => true;

    public void Interact(Interactor interactor)
    {
        // Se o NPC estiver morto ou a cena final estiver rodando, ignore o input
        if (isDead || isCinematicRunning) return;

        // Checa se o player tem a poção e se o diálogo comum terminou
        if (Inventory.Instance.HasGreenPotion && dialogueIndex >= dialogues.Length - 1)
        {
            StartCoroutine(DeathSequence());
            return;
        }

        // Lógica normal de avanço de diálogo (isso continua funcionando até a cena final)
        if (!npcMenuCanvas.activeSelf) 
        {
            npcMenuCanvas.SetActive(true);
            dialogueIndex = 0;
            StartCoroutine(TypeText(dialogues[dialogueIndex]));
        }
        else if (!isTyping)
        {
            AdvanceDialogue();
        }
    }
    private void AdvanceDialogue()
    {
        dialogueIndex++;

        if (dialogueIndex < dialogues.Length)
        {
            StopAllCoroutines();
            StartCoroutine(TypeText(dialogues[dialogueIndex]));
        }
        else
        {
            // Mantém a última frase ("I need healing") sem fechar o menu
            dialogueIndex = dialogues.Length - 1;
            StopAllCoroutines();
            StartCoroutine(TypeText(dialogues[dialogueIndex]));
        }
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        currentFullText = text;
        dialogueText.text = "";
        
        foreach (char c in text.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        isTyping = false;
    }

    IEnumerator DeathSequence()
    {
        isCinematicRunning = true; // TRAVA O INPUT AQUI
        Inventory.Instance.RemoveGreenPotion(); 

        if (potionInHand != null) 
        {
            potionInHand.SetActive(true);
        }

        // 1. NPC começa a beber
        if (npcMenu != null) npcMenu.TriggerDrink(); 

        // Espera a animação de beber começar a fazer efeito
        yield return new WaitForSeconds(2.5f);

        string finalLine = "Another one... that the curse swallows...";
        yield return StartCoroutine(TypeText(finalLine));

        yield return new WaitForSeconds(1.0f);

        // 4. Morte definitiva
        if (npcMenu != null) npcMenu.TriggerNpcDeath(); 

        isDead = true; // NPC agora é considerado morto

        yield return new WaitForSeconds(3f);
        Inventory.Instance.AddKey();
        npcMenuCanvas.SetActive(false);
        this.enabled = false;
    }
}