using UnityEngine;
[CreateAssetMenu(fileName = "StaminaPowerUp", menuName = "PowerUps/StaminaPowerUp")]
public class StaminaPowerUp : PowerUpEffect
{
    public float amount;

    public override void ApplyEffect(GameObject player)
    {
        Stamina stamina = player.GetComponentInChildren<Stamina>();
        if (stamina != null)
        {
            stamina.SetStamina(stamina.CurrentStamina + amount);
        }
    }
}
