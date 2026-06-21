using UnityEngine;
using UnityEngine.UIElements;

public class FogOfWarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIDocument mapDocument;
    [SerializeField] private GameObject player;

    [Header("Map Settings")]
    [SerializeField] private int textureWidth = 951;
    [SerializeField] private int textureHeight = 1066;
    [SerializeField] private float worldWidth = 100f;
    [SerializeField] private float worldHeight = 100f;
    [SerializeField] private Vector3 mapCenter;
    [SerializeField] private float revealRadius = 15f;

    private Texture2D _fogTexture;
    private Color[] _pixels;
    private VisualElement _fogElement;
    private bool _isInitialized = false;

    public void Setup(UIDocument document, GameObject playerObject)
    {
        mapDocument = document;
        player = playerObject;

        if (mapDocument == null)
        {
            Debug.LogWarning("FogOfWarUI: mapDocument is null in Setup!");
            return;
        }

        // Find the fog element in the UXML
        _fogElement = mapDocument.rootVisualElement.Q<VisualElement>("Fog");

        if (_fogElement == null)
        {
            Debug.LogWarning("FogOfWarUI: Could not find 'Fog' element in UXML!");
            return;
        }

        // Create a fully black fog texture
        _fogTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        _pixels = new Color[textureWidth * textureHeight];

        for (int i = 0; i < _pixels.Length; i++)
            _pixels[i] = Color.black;

        _fogTexture.SetPixels(_pixels);
        _fogTexture.Apply();

        _fogElement.style.backgroundImage = new StyleBackground(_fogTexture);

        _isInitialized = true;
    }

    void LateUpdate()
    {
        if (!_isInitialized || player == null) return;
        RevealAtPosition(player.transform.position);
    }

    void RevealAtPosition(Vector3 worldPos)
    {
        int texX = Mathf.RoundToInt(((worldPos.z - mapCenter.z) / worldWidth + 0.5f) * textureWidth);
        int texY = Mathf.RoundToInt((-(worldPos.x - mapCenter.x) / worldHeight + 0.5f) * textureHeight);

        int radius = Mathf.RoundToInt((revealRadius / worldWidth) * textureWidth);

        bool changed = false;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (x * x + y * y > radius * radius) continue;

                int px = texX + x;
                int py = texY + y;

                if (px < 0 || px >= textureWidth  || py < 0 || py >= textureHeight) continue;

                int index = py * textureWidth + px;

                if (_pixels[index].a > 0)
                {
                    float dist = Mathf.Sqrt(x * x + y * y);
                    float alpha = Mathf.Clamp01((dist - radius * 0.7f) / (radius * 0.3f));
                    _pixels[index] = new Color(0, 0, 0, alpha);
                    changed = true;
                }
            }
        }

        if (changed)
        {
            _fogTexture.SetPixels(_pixels);
            _fogTexture.Apply();
        }
    }
}   