using UnityEngine;

public class MeatItem : MonoBehaviour, IInteractable
{
    // Permite interagir se o objeto estiver ativo na cena
    public bool CanInteract() => true;

    public void Interact(Interactor interactor)
    {
        // Adiciona a carne ao inventário
        Inventory.Instance.AddMeat();
        Debug.Log("Você pegou a Carne!");
        
        // Destrói o objeto da cena após pegar
        Destroy(gameObject); 
    }
}