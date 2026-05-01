using UnityEngine;
using StarterAssets;

[CreateAssetMenu(fileName = "SprintPowerUp", menuName = "PowerUps/SprintPowerUp")]
public class SprintPowerUp : PowerUpEffect
{
    public float SprintSpeedIncrease = 1f;

    public override void ApplyEffect(GameObject player)
    {
        if (player == null) return;

        var controller = player.GetComponentInChildren<ThirdPersonController>();
        if (controller != null)
        {
            controller.AddSprintSpeedBonus(SprintSpeedIncrease);
            Debug.Log($"Sprint powerup applied: +{SprintSpeedIncrease} sprint speed.");
        }
        else
        {
            Debug.LogWarning("SprintPowerUp: player has no ThirdPersonController component.", player);
        }
    }
}
