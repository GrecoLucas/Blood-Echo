using UnityEngine;
[CreateAssetMenu(fileName = "StaminaPowerUp", menuName = "PowerUps/StaminaPowerUp")]
public class StaminaPowerUp : PowerUpEffect
{
    public float amount;
    public enum PowerUpType { StaminaBoost, PermanentStaminaIncrease }
    public PowerUpType powerUpType;

    public override void ApplyEffect(GameObject player)
    {
        Stamina stamina = player.GetComponentInChildren<Stamina>();
        if (stamina != null)
        {
            Debug.Log($"Applying {powerUpType} power-up with amount {amount} to player {player.name}");
            switch (powerUpType)
            {
                case PowerUpType.StaminaBoost:
                    stamina.SetStamina(stamina.CurrentStamina + amount);
                    break;
                case PowerUpType.PermanentStaminaIncrease:
                    stamina.IncreaseMaxStamina(amount);
                    break;
            }
            
        } else {
            Debug.LogWarning("StaminaPowerUp: Player does not have a Stamina component!");
        }
    }
}
