using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public float damage = 10f;
    private bool canDealDamage = false;

    // Esta lista evita que o mesmo golpe dê dano várias vezes no mesmo frame
    private System.Collections.Generic.List<GameObject> hasHit = new System.Collections.Generic.List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Encostei em algo chamado: " + other.name);
        if (canDealDamage && other.CompareTag("Player") && !hasHit.Contains(other.gameObject))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health == null) health = other.GetComponentInParent<PlayerHealth>();

            if (health != null)
            {
                health.TakeDamage(damage);
                hasHit.Add(other.gameObject); // Registra que já bateu neste jogador
                Debug.Log("Inimigo causou dano ao jogador!");
            }
        }
    }

    // Chamado via Animation Event
    public void StartDealingDamage() 
    {
        Debug.Log("Inimigo começou a causar dano!");
        canDealDamage = true;
        hasHit.Clear(); // Limpa a lista para o novo golpe
    }

    public void EndDealingDamage() 
    {
        Debug.Log("Inimigo parou de causar dano!");
        canDealDamage = false;
    }
}