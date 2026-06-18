using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using StarterAssets;

public class MapController : MonoBehaviour
{
    [SerializeField] private UIDocument mapDocument;
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 mapCenter;

    [Range(1, 15)]
    [SerializeField] private float mapMultiplier = 7f;

    private VisualElement _root;
    private VisualElement _playerRepresentation;
    private bool _isOpen = false;
    private StarterAssetsInputs _input;

    public void Setup(UIDocument document, GameObject playerObject)
    {
        mapDocument = document;
        player = playerObject;

        _input = GetComponent<StarterAssetsInputs>();

        if (mapDocument != null)
        {
            _root = mapDocument.rootVisualElement;
            _root.style.display = DisplayStyle.None;
            _playerRepresentation = _root.Q<VisualElement>("Player");
        }
    }

    void Start()
    {
        // Verifica se o mapDocument existe antes de tentar acessá-lo
        if (mapDocument != null)
        {
            _root = mapDocument.rootVisualElement;
            
            // Verifica se o visual tree (UXML) foi devidamente carregado
            if (_root != null)
            {
                _root.style.display = DisplayStyle.None;
                _playerRepresentation = _root.Q<VisualElement>("Player");
            }
            else
            {
                Debug.LogWarning("O MapDocument foi atribuído, mas não possui um arquivo de interface carregado.");
            }
        }
        
        _input = GetComponent<StarterAssetsInputs>();
    }

    void Update()
    {
        bool mapPressed = false;

        if (_input != null && _input.map)
        {
            _input.map = false; // consume the input
            mapPressed = true;
        }
        else if (_input == null && Input.GetKeyDown(KeyCode.M))
        {
            mapPressed = true;
        }

        if (mapPressed)
        {
            _isOpen = !_isOpen;
            _root.style.display = _isOpen ? DisplayStyle.Flex : DisplayStyle.None;

            Time.timeScale = _isOpen ? 0f : 1f;
        }
    }
    private void LateUpdate()
    {
        if (_playerRepresentation == null || player == null) return;

        _playerRepresentation.style.translate = new Translate(
            (player.transform.position.z - mapCenter.z) * mapMultiplier,
            (player.transform.position.x - mapCenter.x) * mapMultiplier,
            0);
        _playerRepresentation.style.rotate = new Rotate(
            new Angle(player.transform.rotation.eulerAngles.y +90f));
    }
}