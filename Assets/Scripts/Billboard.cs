using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Header("Configurações de Visibilidade")]
    [SerializeField] private CanvasGroup canvasGroup;
    [Tooltip("A que distância o texto começa a aparecer?")]
    public float distanciaMax = 6f;
    [Tooltip("A que distância o texto fica 100% visível?")]
    public float distanciaMin = 2f;

    private Transform mainCameraTransform;

    void Start()
    {
        mainCameraTransform = Camera.main.transform;

        // Se você esqueceu de arrastar o CanvasGroup, o script tenta achar sozinho
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
    }

    void LateUpdate()
    {
        if (mainCameraTransform == null) return;

        // 1. Lógica de Rotação (Billboard)
        transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward,
                         mainCameraTransform.rotation * Vector3.up);

        // 2. Lógica de Transparência por Distância
        ControlarFadePorDistancia();
    }

    private void ControlarFadePorDistancia()
    {
        if (canvasGroup == null) return;

        // Calcula a distância entre o texto e a câmera
        float distancia = Vector3.Distance(transform.position, mainCameraTransform.position);

        // Cálculo matemático do Alpha (Transparência)
        // Usamos a fórmula: alpha = 1 - ((distancia - dMin) / (dMax - dMin))
        float alphaCalculado = 1 - ((distancia - distanciaMin) / (distanciaMax - distanciaMin));
        
        // Aplica o valor e garante que ele fique entre 0 (invisível) e 1 (visível)
        canvasGroup.alpha = Mathf.Clamp01(alphaCalculado);
    }
}