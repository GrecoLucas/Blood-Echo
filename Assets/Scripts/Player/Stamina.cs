using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class Stamina : MonoBehaviour
{
	[Header("Stamina Settings")]
	[SerializeField] private float maxStamina = 100f;
	[SerializeField] private float currentStamina = 100f;
	[SerializeField] private float drainPerSecond = 20f;
	[SerializeField] private float regenPerSecond = 15f;
	[SerializeField] private float regenDelay = 1f;

	[Header("Input")]
	[SerializeField] private bool useFire3Input = true;

	[Header("UI References")]
	[SerializeField] private Slider staminaSlider;
	[SerializeField] private TMPro.TMP_Text staminaText;
	[SerializeField] private Slider maxStaminaSlider;
	private float lastDrainTime;
	private StarterAssetsInputs _input;

	public float CurrentStamina => currentStamina;
	public float MaxStamina => maxStamina;
	public bool HasStamina => currentStamina > 0.01f;

	private void Awake()
	{
		TryAutoBind();
		maxStamina = Mathf.Max(1f, maxStamina);
		currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
		_input = GetComponent<StarterAssetsInputs>();

		if (staminaSlider != null)
		{
            // Usar a variável maxStamina em vez de um número fixo
			staminaSlider.maxValue = maxStamina;
			staminaSlider.value = currentStamina;
		}
		if (maxStaminaSlider != null)
        {
            // Usar a variável maxStamina em vez de um número fixo
            maxStaminaSlider.maxValue = maxStamina;
            maxStaminaSlider.value = maxStamina;
        }
		UpdateUI();
	}

	private void OnValidate()
	{
		TryAutoBind();
		maxStamina = Mathf.Max(1f, maxStamina);
		currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
		UpdateUI();
	}

	private void Update()
	{
		// A pedido do usuário: correr (sprint) não gasta mais stamina. 
        // Apenas o regenDelay continua funcionando (após um rolamento, ele espera antes de regenerar).

		if (Time.time - lastDrainTime >= regenDelay && currentStamina < maxStamina)
		{
			currentStamina = Mathf.Clamp(currentStamina + regenPerSecond * Time.deltaTime, 0f, maxStamina);
			UpdateUI();
		}
	}

	public bool TryUseStamina(float amount)
	{
		if (amount <= 0f)
		{
			return true;
		}

		if (currentStamina < amount)
		{
			return false;
		}

		currentStamina = Mathf.Clamp(currentStamina - amount, 0f, maxStamina);
		lastDrainTime = Time.time;
		UpdateUI();
		return true;
	}

	public void SetStamina(float value)
	{
		currentStamina = Mathf.Clamp(value, 0f, maxStamina);
		UpdateUI();
	}

	public void SetMaxStamina(float value, bool refill = true)
	{
		maxStamina = Mathf.Max(1f, value);
		currentStamina = refill ? maxStamina : Mathf.Clamp(currentStamina, 0f, maxStamina);
		UpdateUI();
	}

	private void UpdateUI()
	{

	    if (staminaSlider != null)
	    {
	        staminaSlider.maxValue = maxStamina;
	        staminaSlider.value = currentStamina;

	        RectTransform rt = staminaSlider.GetComponent<RectTransform>();
	        if (rt != null)
	        {
	            // Força a alteração apenas da LARGURA (Axis.Horizontal)
	            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxStamina);
	        }
	    }

	    if (maxStaminaSlider != null)
	    {
	        maxStaminaSlider.maxValue = maxStamina;
	        maxStaminaSlider.value = maxStamina;

	        RectTransform rtBg = maxStaminaSlider.GetComponent<RectTransform>();
	        if (rtBg != null)
	        {
	            // Força a alteração da largura do fundo também
	            rtBg.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxStamina);
	        }
	    }

	    if (staminaText != null)
	    {
	        staminaText.text = $"{Mathf.CeilToInt(currentStamina)}/{Mathf.CeilToInt(maxStamina)}";
	    }
	}

	private void TryAutoBind()
	{
		if (staminaSlider == null)
		{
			staminaSlider = GetComponent<Slider>();
			if (staminaSlider == null)
			{
				staminaSlider = GetComponentInChildren<Slider>();
			}
		}
	}
	public void IncreaseMaxStamina(float amount)
    {
        maxStamina += amount;
        
        currentStamina = maxStamina; 
        
        UpdateUI(); 
        
        Debug.Log($"Novo limite de Estamina: {maxStamina}");
    }
}
