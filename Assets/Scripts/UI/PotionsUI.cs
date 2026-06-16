using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PotionsUI : MonoBehaviour
{
    [Header("References")]
    public Image overlayFill; // the dark overlay Image with fill on the healing child
    public GameObject potionNumberText;

    [Header("Animation")]
    public float animationSpeed = 3f; // how fast the fill drops

    private TMP_Text _potionNumberTMP;
    private UnityEngine.UI.Text _potionNumberLegacy;
    private float _targetFill = 1f;

    void Start()
    {
        if (potionNumberText != null)
        {
            _potionNumberTMP = potionNumberText.GetComponent<TMP_Text>();
            _potionNumberLegacy = potionNumberText.GetComponent<UnityEngine.UI.Text>();
        }
    }
    // Called by Potions every time a potion is used or restored
    public void UpdateVisual(int potionCount, int maxPotions)
    {
        if (overlayFill == null) return;

        // fill goes from 1 (full/no potions used) down to 0 (all used)
        // each potion consumed drops fill by (1 / maxPotions)
        // overlayFill.fillAmount = (float)potionCount / maxPotions;

        if (_potionNumberTMP != null)
        {
            _potionNumberTMP.text = potionCount.ToString();
        }
        else if (_potionNumberLegacy != null)
        {
            _potionNumberLegacy.text = potionCount.ToString();
        }

        // Animate the fill smoothly
        _targetFill = (float)potionCount / maxPotions;
        StopAllCoroutines();
        StartCoroutine(AnimateFill(_targetFill));
    }


    private IEnumerator AnimateFill(float target)
    {
        while (!Mathf.Approximately(overlayFill.fillAmount, target))
        {
            overlayFill.fillAmount = Mathf.MoveTowards(
                overlayFill.fillAmount, 
                target, 
                animationSpeed * Time.deltaTime
            );
            yield return null;
        }

        overlayFill.fillAmount = target; // snap to exact value at the end
    }
}