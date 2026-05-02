using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance; // easy access from anywhere
    public List<PowerUpEffect> items = new List<PowerUpEffect>();
    private bool hasHeavyAttack;
    public bool HasHeavyAttack => hasHeavyAttack;

    private void Awake()
    {
        // Singleton so any script can call Inventory.Instance
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void AddHeavyAttack()
    {
        hasHeavyAttack = true;
        
    }

    public void AddItem(PowerUpEffect item)
    {
        items.Add(item);
    }

    public void RemoveItem(PowerUpEffect item)
    {
        items.Remove(item);
    }

    public bool HasItem(PowerUpEffect item)
    {
        return items.Contains(item);
    }
}