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
            stamina.IncreaseMaxStamina(amount);
            Debug.Log($"Cristal coletado! Estamina máxima aumentada em {amount}.");
        } 
        else 
        {
            Debug.LogWarning("StaminaPowerUp: O jogador nãoasdad tem o componente Stamina!");
        }
    }
}