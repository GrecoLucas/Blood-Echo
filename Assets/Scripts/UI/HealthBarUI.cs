using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI  : MonoBehaviour
{
    public Slider maxHealthSlider;
    public float maxBarWidth = 300f; 

    public void UpdateHealthBar(float current, float max)
    {
        if (maxHealthSlider != null)
        {
            //maxHealthSlider.maxValue = maxBarWidth;
            maxHealthSlider.value = max;
            Debug.Log($"HealthBarUI: Updated health bar. Current: {current}, Max: {max}, FillAmount: {maxHealthSlider.value}");
        }
        else{
            Debug.LogWarning("HealthBarUI: healthBarFill is not assigned.");
        }

    }
}
