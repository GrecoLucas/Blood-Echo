using UnityEngine;

public class PowerUpController : MonoBehaviour, IInteractable
{
    public Transform powerUpMesh;
    public PowerUpEffect powerUpEffect; // Enum para definir o tipo de power-up
    public bool CanInteract()
    {
        return true;
    }

    public void Interact(Interactor interactor)
    {
        Debug.Log("Power-up coletado!");
        Destroy(gameObject); 
        powerUpEffect.ApplyEffect(interactor.gameObject); 
    }
    
}
