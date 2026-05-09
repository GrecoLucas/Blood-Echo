using UnityEngine;
using System.Collections.Generic;
using StarterAssets;

public class DamageDealer : MonoBehaviour
{
    [Header("Parry Settings")]
    public bool canBeParried = false; 
    public float stunDuration = 2.0f; 
    [Header("Effects")]
    [SerializeField] private GameObject hitSparkPrefab;

    public enum DamageOwner { Enemy, Player }
    public float damage = 10f;
    public DamageOwner owner = DamageOwner.Enemy;
    
    private bool canDealDamage = false;
    private bool heavyDamage = false;
    private List<GameObject> hasHit = new List<GameObject>();

    private void OnTriggerEnter(Collider other) 
    {
        if (!canDealDamage) return;
        if (other.transform.root == transform.root) return;

        if (owner == DamageOwner.Player)
        {
            EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
            if (enemy != null && !hasHit.Contains(enemy.gameObject))
            {
                float finalDamage = heavyDamage ? damage * 2 : damage;
                enemy.TakeDamage(finalDamage);
                hasHit.Add(enemy.gameObject);
                SpawnHitSpark(other);
            }
        }
        else if (owner == DamageOwner.Enemy)
        {
            StarterAssets.ThirdPersonController playerController = other.GetComponentInParent<StarterAssets.ThirdPersonController>();
            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

            if (playerController != null && playerHealth != null && !hasHit.Contains(playerHealth.gameObject))
            {
                // VERIFICAÇÃO DE PARRY
                if (canBeParried && playerController.IsParrying)
                {
                    playerController.PlayParryVFX();                    
                    ApplyStunToOwner(); 
                    hasHit.Add(playerHealth.gameObject);
                    return; 
                }

                playerHealth.TakeDamage(damage);
                hasHit.Add(playerHealth.gameObject);
                SpawnHitSpark(other);
            }
        }
    }
    public void StartDealingDamage() { canDealDamage = true; heavyDamage = false; hasHit.Clear(); }
    public void StartDealingHeavyDamage() { canDealDamage = true; heavyDamage = true; hasHit.Clear(); }
    public void EndDealingDamage() { canDealDamage = false; }
    public void AddDamageBonus(float bonus)
    {
        damage += bonus;
    }

    private void ApplyStunToOwner()
    {
        // Busca o BossAI no objeto pai para aplicar o stun
        BossAI bossAI = GetComponentInParent<BossAI>();
        if (bossAI != null)
        {
            bossAI.OnStun(stunDuration);
        }
    }

    private void SpawnHitSpark(Collider other)
    {
        if (hitSparkPrefab == null) return;
        Vector3 contactPoint = other.ClosestPoint(transform.position);
        GameObject spark = Instantiate(hitSparkPrefab, contactPoint, Quaternion.identity);
        Destroy(spark, 2f);
    }
}