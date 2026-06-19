using UnityEngine;
using FMODUnity;

public class PowerUpController : MonoBehaviour, IInteractable
{   
    [Header("FMOD Audio")]
    public EventReference pickupSoundEvent;
    
    public Transform powerUpMesh;
    public PowerUpEffect powerUpEffect;

    public bool CanInteract()
    {
        return true;
    }

    public void Interact(Interactor interactor)
    {
        // Usa PlayOneShot para que o som toque até o fim mesmo se o objeto for destruído imediatamente!
        if (!pickupSoundEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(pickupSoundEvent, transform.position);
        }

        Debug.Log("Power-up coletado!");
         
        powerUpEffect.ApplyEffect(interactor.gameObject); 

        if (PowerUpUI.Instance != null)
            PowerUpUI.Instance.ShowPowerUp(powerUpEffect);
        
        Destroy(gameObject);
    }
}
