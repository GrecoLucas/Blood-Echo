using UnityEngine;

public class LockOnIndicator : MonoBehaviour
{
    [SerializeField] private float heightOffset = 2.5f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.3f;
    
    private Transform targetEnemy;
    private Vector3 originalPosition;
    private float bobOffset;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (targetEnemy == null)
        {
            gameObject.SetActive(false);
            return;
        }

        // Position above enemy
        Vector3 targetPos = targetEnemy.position + Vector3.up * heightOffset;
        Collider targetCol = null;
        foreach (var col in targetEnemy.GetComponentsInChildren<Collider>())
        {
            if (!col.isTrigger)
            {
                targetCol = col;
                break;
            }
        }
        if (targetCol == null) targetCol = targetEnemy.GetComponentInChildren<Collider>();
        if (targetCol != null)
        {
            // Use the same height as other enemies (from pivot), but center it on the collider's X/Z
            targetPos = new Vector3(targetCol.bounds.center.x, targetEnemy.position.y + heightOffset, targetCol.bounds.center.z);
        }
        else
        {
            targetPos = targetEnemy.position + Vector3.up * heightOffset;
        }
        
        transform.position = targetPos;

        // Bob up and down
        bobOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position += Vector3.up * bobOffset;

        // Rotate around its axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform enemy)
    {
        targetEnemy = enemy;
        gameObject.SetActive(true);
    }

    public void ClearTarget()
    {
        targetEnemy = null;
        gameObject.SetActive(false);
    }
}
