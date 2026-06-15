using UnityEngine;

public abstract class PowerUpEffect: ScriptableObject
{
    [Header("UI")]
    public Sprite icon;
    public string powerUpName = "Power-Up";
    public string powerUpDescription = "Description of the power-up effect.";

    public abstract void ApplyEffect(GameObject player);
    
}
