using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    [Header("Arraste os objetos da Hierarquia aqui")]
    public GameObject meatIcon;
    public GameObject potionIcon;

    void Update()
    {
        // Verifica se o Inventário existe para evitar erros
        if (Inventory.Instance != null)
        {
            // Ativa o ícone se tiver o item, desativa se não tiver
            if (meatIcon != null)
            {
                meatIcon.SetActive(Inventory.Instance.HasMeat);
            }

            if (potionIcon != null)
            {
                potionIcon.SetActive(Inventory.Instance.HasGreenPotion);
            }
        }
    }
}