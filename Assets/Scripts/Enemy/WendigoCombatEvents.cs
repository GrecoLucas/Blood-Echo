using UnityEngine;

public class WendigoCombatEvents : MonoBehaviour
{
    [Header("Damage Dealers")]
    // Arraste os objetos DamageAreaL e DamageAreaR da Hierarchy para cá
    public DamageDealer leftHandDamage;
    public DamageDealer rightHandDamage;
    
    // Chame este evento no frame em que o ataque da mão ESQUERDA deve dar dano
    public void EnableLeftHandDamage()
    {
        if (leftHandDamage != null) leftHandDamage.StartDealingDamage();
    }

    // Chame este evento no frame em que o ataque da mão DIREITA deve dar dano
    public void EnableRightHandDamage()
    {
        if (rightHandDamage != null) rightHandDamage.StartDealingDamage();
    }
    
    // Chame este evento quando o braço ESQUERDO terminar o movimento de ataque
    public void DisableLeftHandDamage()
    {
        if (leftHandDamage != null) leftHandDamage.EndDealingDamage();
    }

    // Chame este evento quando o braço DIREITO terminar o movimento de ataque
    public void DisableRightHandDamage()
    {
        if (rightHandDamage != null) rightHandDamage.EndDealingDamage();
    }
}