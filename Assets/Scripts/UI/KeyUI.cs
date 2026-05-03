using UnityEngine;
using UnityEngine.UI;

public class KeyUI : MonoBehaviour
{
    public Image keyOverlay; 
    public GameObject keyPanel;
    private Inventory _inventory;
    void Start()
    {
        _inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Inventory>();
        if (_inventory == null)
        {
            Debug.LogError("KeyUI: Inventory not found on Player object.");
        }
        keyPanel.SetActive(false); // hide until player has a key
    }
    
    void Update()
    {
        if (_inventory == null)
        {
            Debug.LogError("KeyUI: Inventory not found on Player object.");
            return;
        }
        
        if (!_inventory.HasKey)
        {
            keyPanel.SetActive(false);
            return;
        }

        keyPanel.SetActive(true);
    }

}
