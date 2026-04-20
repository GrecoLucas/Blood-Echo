using UnityEngine;

public class DamageDealer : MonoBehaviour
{
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
                Debug.Log($"[SUCESSO] Player deu {damage} de dano em {enemy.name}");
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
                Debug.Log($"[DANO] Inimigo deu {damage} de dano no Player");
            }
        }
    }
    // Chamado via Animation Event
    public void StartDealingDamage() 
    {
        Debug.Log($"[DamageDealer] StartDealingDamage -> owner={owner}");
        canDealDamage = true;
        hasHit.Clear(); // Limpa a lista para o novo golpe
    }

    public void EndDealingDamage() 
    {
        Debug.Log($"[DamageDealer] EndDealingDamage -> owner={owner}");
        canDealDamage = false;
    }
}