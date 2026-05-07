using UnityEngine;

public class DogPuzzle : MonoBehaviour, IInteractable
{
    [Header("Objetos do Puzzle")]
    public GameObject pocaoNaBoca; 
    public GameObject pocaoNoChao;
    public GameObject carneNaBoca;   
    [Header("UI (Opcional)")]
    public GameObject promptUI;
    private bool puzzleResolvido = false;

    // Só permite interagir se o puzzle ainda não foi resolvido
    public bool CanInteract() => !puzzleResolvido;

    public void Interact(Interactor interactor)
    {
        // Verifica no Inventário se o jogador tem a carne
        if (Inventory.Instance.HasMeat)
        {
            ResolverPuzzle();
        }
    }

    private void ResolverPuzzle()
    {
        puzzleResolvido = true;
        
        // Remove a carne do inventário do jogador
        Inventory.Instance.RemoveMeat();
        
        // Esconde a UI
        if (promptUI != null) promptUI.SetActive(false);

        // 1. Esconde a poção que estava na boca do cachorro
        if (pocaoNaBoca != null)
        {
            pocaoNaBoca.SetActive(false);
        }

        // 2. Faz a poção aparecer no chão
        if (pocaoNoChao != null)
        {
            pocaoNoChao.SetActive(true);
        }

        // 3. A carne aparece na boca do cachorro
        if (carneNaBoca != null)
        {
            carneNaBoca.SetActive(true);
        }
    }
}