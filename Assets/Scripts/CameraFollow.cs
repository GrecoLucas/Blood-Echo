using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Distance and Height")]
    public float distance = 5f;
    public float height   = 2f;

    [Header("Sensitivity")]
    [Range(0.05f, 0.5f)]
    public float sensitivity = 0.2f;

    [Header("Vertical Limits")]
    public float minPitch = -15f;
    public float maxPitch =  60f;

    private float yaw;
    private float pitch = 20f;
    private InputAction lookAction;

    void Start()
    {
        if (player == null) return;

        var playerInput = player.GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogWarning("CameraFollow: o Player precisa do componente PlayerInput.");
            return;
        }

        lookAction = playerInput.actions["Look"];
        if (lookAction == null)
        {
            Debug.LogWarning("CameraFollow: acao 'Look' nao encontrada no Input Actions.");
            return;
        }

        yaw = transform.eulerAngles.y;
    }

    void LateUpdate()
    {
        if (player == null || lookAction == null) return;

        Vector2 look = lookAction.ReadValue<Vector2>();
        float lookScale = Time.deltaTime * 60f;
        float finalSensitivity = Mathf.Clamp(sensitivity, 0.05f, 0.5f);

        yaw   += look.x * finalSensitivity * lookScale;
        pitch -= look.y * finalSensitivity * lookScale;
        pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset      = rotation * new Vector3(0f, 0f, -distance);

        // Posicao final: atras e acima do player
        transform.position = player.position + Vector3.up * height + offset;

        // Olha para o centro do player (um pouco acima dos pes)
        transform.LookAt(player.position + Vector3.up * (height * 0.5f));
    }
}