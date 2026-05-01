using UnityEngine;
[CreateAssetMenu(fileName = "DamagePowerUp", menuName = "PowerUps/DamagePowerUp")]
public class DamagePowerUp : PowerUpEffect
{
    public float DamageIncrease = 1f;

    public override void ApplyEffect(GameObject player)
    {
        if (player == null) return;

        var damageDealer = player.GetComponentInChildren<DamageDealer>();
        if (damageDealer != null)
        {
            damageDealer.AddDamageBonus(DamageIncrease);
            Debug.Log($"Damage powerup applied: +{DamageIncrease} damage.");
        }
        else
        {
            Debug.LogWarning("DamagePowerUp: player has no DamageDealer component.", player);
        }
    }
}


