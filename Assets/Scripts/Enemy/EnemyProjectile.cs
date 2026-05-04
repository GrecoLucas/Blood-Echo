using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float damage = 20f;
    public float lifetime = 5f;
    private Vector3 moveDirection;
    public GameObject impactEffectPrefab; 
    public void Setup(Vector3 targetPosition)
    {
        // Calcula a direção ignorando o Y para não ir para o chão se o spawn for alto
        Vector3 spawnPos = transform.position;
        moveDirection = (targetPosition - spawnPos).normalized;
        
        // Rotaciona a esfera para olhar para o alvo
        transform.forward = moveDirection;
        
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private bool dealtDamage = false;
    private void OnTriggerEnter(Collider other)
    {
        if (dealtDamage) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemies")) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
            if (player != null)
            {
                dealtDamage = true;
                player.TakeDamage(damage);

                if (impactEffectPrefab != null) {
                    Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
                }

                Debug.Log("Projétil acertou o Player!");
            }
            Destroy(gameObject);
        }
    }
}
