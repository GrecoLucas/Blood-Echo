using UnityEngine;

public class GreenPotionItem : MonoBehaviour, IInteractable
{
    // Permite interagir sempre que o objeto estiver na cena
    public bool CanInteract() => true;

    public void Interact(Interactor interactor)
    {
        // Adiciona a poção verde ao inventário do jogador
        Inventory.Instance.AddGreenPotion();
        
        Debug.Log("Você pegou a Poção Verde (Veneno)!");
        
        // Destrói o objeto da poção do chão/cenário
        Destroy(gameObject); 
    }
}