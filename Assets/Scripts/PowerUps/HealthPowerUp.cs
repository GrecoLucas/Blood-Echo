using UnityEngine;

[CreateAssetMenu(fileName = "HealthPowerUp", menuName = "PowerUps/HealthPowerUp")]
public class HealthPowerUp : PowerUpEffect
{
    public float amount;
    public enum PowerUpType { Heal, Damage, PermanentHealthIncrease }
    public PowerUpType powerUpType;
    public override void ApplyEffect(GameObject player)
    {
        PlayerHealth health = player.GetComponentInChildren<PlayerHealth>();
        if (health != null)
        {
            switch (powerUpType)
            {
                case PowerUpType.Heal:
                    health.Heal(amount);
                    break;
                case PowerUpType.Damage:
                    health.TakeDamage(amount);
                    break;
                case PowerUpType.PermanentHealthIncrease:
                    health.IncreaseMaxHealth(amount);
                    break;
            }


        }
    }
    
}
