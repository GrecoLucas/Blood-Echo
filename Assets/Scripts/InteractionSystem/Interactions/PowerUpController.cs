using UnityEngine;

public class PowerUpController : MonoBehaviour, IInteractable
{
    public Transform powerUpMesh;
    public PowerUpEffect powerUpEffect;
    public bool CanInteract()
    {
        return true;
    }

    public void Interact(Interactor interactor)
    {
        Debug.Log("Power-up coletado!");
         
        powerUpEffect.ApplyEffect(interactor.gameObject); 

        if (PowerUpUI.Instance != null)
            PowerUpUI.Instance.ShowPowerUp(powerUpEffect);
        
        Destroy(gameObject);
    }
    
}
