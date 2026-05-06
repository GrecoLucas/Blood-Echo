using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class CoolDownUI : MonoBehaviour
{
    public Image cooldownOverlay;       // the dark fill image
    public GameObject cooldownPanel;    // the whole UI element

    private ThirdPersonController _controller;
    private Inventory _inventory;

    private void Start()
    {
        
        _controller = GameObject.FindGameObjectWithTag("Player")?.GetComponent<ThirdPersonController>();
        if (_controller == null)
        {
            Debug.LogError("CooldownUI: ThirdPersonController not found on Player object.");
        }
        cooldownPanel.SetActive(false); // hide until player has heavy attack
        _inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Inventory>();
        if (_inventory == null)
        {
            Debug.LogError("CooldownUI: Inventory not found on Player object.");
        }
    }

    private void Update()
    {
        if (_inventory == null)
        {
            return;
        }
        // only show when player has heavy attack unlocked
        if (!_inventory.HasHeavyAttack)
        {
            return;
        }

        cooldownPanel.SetActive(true);
        float cooldownProgress = Mathf.Clamp01(_controller.HeavyAttackTimer / _controller.HeavyAttackCooldown);

        // 1 = fully on cooldown, 0 = ready
        cooldownOverlay.fillAmount = 1f - cooldownProgress;


    }
}