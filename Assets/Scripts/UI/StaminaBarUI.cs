using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI  : MonoBehaviour
{
    public Slider maxStaminaSlider;
    public float maxBarWidth = 300f; 

    public void UpdateStaminaBar(float max)
    {
        if (maxStaminaSlider != null)
        {
            //maxStaminaSlider.maxValue = maxBarWidth;
            maxStaminaSlider.value = max;
        }
        else{
            Debug.LogWarning("StaminaBarUI: maxStaminaSlider is not assigned.");
        }

    }
}
