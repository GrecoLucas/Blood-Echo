using UnityEngine;

[CreateAssetMenu(fileName = "HealthPowerUp", menuName = "PowerUps/HealthPowerUp")]
public class HealthPowerUp : PowerUpEffect
{
    public float amount;
    public bool isHealing = true; // Se for false, o efeito será de dano
    public override void ApplyEffect(GameObject player)
    {
        PlayerHealth health = player.GetComponentInChildren<PlayerHealth>();
        if (health != null)
        {
            if (isHealing)
                health.Heal(amount); // Cura o jogador
            else
            {
                health.TakeDamage(amount); // Aplica dano ao jogador
                Debug.Log($"Player levou {amount} pontos de dano!");
            }
        }
        else
        {
            Debug.LogError("PlayerHealth component nao encontrado no jogador. Certifique-se de que o script PlayerHealth esteja anexado ao jogador.");
        }
    }
}
