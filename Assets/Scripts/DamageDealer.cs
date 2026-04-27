using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private GameObject hitSparkPrefab;
    public enum DamageOwner
    {
        Enemy,
        Player
    }

    public float damage = 10f;
    public DamageOwner owner = DamageOwner.Enemy;
    private bool canDealDamage = false;

    // Esta lista evita que o mesmo golpe dê dano várias vezes no mesmo frame
    private System.Collections.Generic.List<GameObject> hasHit = new System.Collections.Generic.List<GameObject>();

    private void OnTriggerEnter(Collider other) {
        if (!canDealDamage) return;

        // 1. Evitar acertar a si mesmo (compara as tags ou o objeto pai principal)
        if (other.transform.root == transform.root) return;

        // 2. Se o DONO é o PLAYER, ele quer acertar um ENEMY
        if (owner == DamageOwner.Player)
        {
            EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
            
            // Se bateu em algo que tem vida e ainda não batemos nesse golpe
            if (enemy != null && !hasHit.Contains(enemy.gameObject))
            {
                enemy.TakeDamage(damage);
                hasHit.Add(enemy.gameObject);
                SpawnHitSpark(other);
            }
        }
        // 3. Se o DONO é o ENEMY, ele quer acertar o PLAYER
        else if (owner == DamageOwner.Enemy)
        {
            PlayerHealth player = other.GetComponentInParent<PlayerHealth>();

            if (player != null && !hasHit.Contains(player.gameObject))
            {
                player.TakeDamage(damage);
                hasHit.Add(player.gameObject);
                SpawnHitSpark(other);
            }
        }
    }
    // Chamado via Animation Event
    public void StartDealingDamage() 
    {
        canDealDamage = true;
        hasHit.Clear(); // Limpa a lista para o novo golpe
    }

    public void EndDealingDamage() 
    {
        canDealDamage = false;
    }
    private void SpawnHitSpark(Collider other)
    {
        if (hitSparkPrefab == null) return;

        Vector3 contactPoint = other.ClosestPoint(transform.position);

        GameObject spark = Instantiate(hitSparkPrefab, contactPoint, Quaternion.identity);

        spark.transform.forward = contactPoint - other.bounds.center;
    }
}