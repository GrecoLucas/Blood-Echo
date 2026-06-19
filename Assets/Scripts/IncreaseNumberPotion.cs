using UnityEngine;

[CreateAssetMenu(fileName = "IncreaseNumberPotion", menuName = "PowerUps/IncreaseNumberPotion")]
public class IncreaseNumberPotion : PowerUpEffect
{
    [Header("Potion Upgrade Settings")]
    [Tooltip("Quanto de vida a mais a poção vai curar depois de pegar este item.")]
    public float healIncreaseAmount = 50f;

    public override void ApplyEffect(GameObject player)
    {
        if (player == null) return;

        // Procura o script Potions no Player
        Potions potionsScript = player.GetComponentInChildren<Potions>();
        if (potionsScript != null)
        {
            potionsScript.healAmount += healIncreaseAmount;
            Debug.Log("Poções melhoradas! Nova cura: " + potionsScript.healAmount);
        }
        else
        {
            Debug.LogWarning("IncreaseNumberPotion: O jogador não tem o script Potions.");
        }
    }
}
