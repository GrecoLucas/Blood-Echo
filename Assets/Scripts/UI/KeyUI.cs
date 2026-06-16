using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class KeyUI : MonoBehaviour
{
    public GameObject keyIconPrefab; // Prefab do ícone da chave
    public Transform keyIconContainer; // Container para os ícones das chaves
    private List<GameObject> keyIcons = new List<GameObject>(); // Lista para armazenar os ícones das chaves
    private Inventory _inventory;
    void Start()
    {
        _inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Inventory>();
        if (_inventory == null)
        {
            Debug.LogError("KeyUI: Inventory not found on Player object.");
        }
        //keyPanel.SetActive(false); // hide until player has a key
    }
    
    void Update()
    {
        if (_inventory == null)
        {
            Debug.LogError("KeyUI: Inventory not found on Player object.");
            return;
        }
        int keyCount = _inventory.KeyCount;

        while( keyIcons.Count < keyCount)
        {
            GameObject newIcon = Instantiate(keyIconPrefab, keyIconContainer);
            newIcon.SetActive(true);
            keyIcons.Add(newIcon);
        }
        while( keyIcons.Count > keyCount)
        {
            GameObject iconToRemove = keyIcons[keyIcons.Count - 1];
            keyIcons.RemoveAt(keyIcons.Count - 1);
            Destroy(iconToRemove);
        }

    }

}
