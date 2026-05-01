using UnityEngine;
[CreateAssetMenu(fileName = "HeavyAttackPowerUp", menuName = "PowerUps/HeavyAttackPowerUp")]

public class HeavyAttackPowerUp : PowerUpEffect
{

    public override void ApplyEffect(GameObject player)
    {
        if (player == null) return;

        var inventory = player.GetComponentInChildren<Inventory>();
        if (inventory != null)
        {
            inventory.AddHeavyAttack();
            Debug.Log("Heavy Attack powerup applied: Player can now perform heavy attacks.");
        }
        else
        {
            Debug.LogWarning("HeavyAttackPowerUp: player has no Inventory component.", player);
        }
    }
}
