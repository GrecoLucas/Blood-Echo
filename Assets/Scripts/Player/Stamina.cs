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
			staminaSlider.maxValue = 300f;
			staminaSlider.value = currentStamina;
			Debug.Log($"StaminaBarUI: Updated stamina bar. Current: {currentStamina}, Max: {maxStamina}, FillAmount: {staminaSlider.value}");
		}
		if (maxStaminaSlider != null)
        {
            maxStaminaSlider.maxValue = 300f;
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
		// Detecta sprint via Input System (gamepad Circle) ou Input antigo (LeftShift/Fire3)
		bool sprinting = false;
		if (_input != null)
		{
			sprinting = _input.sprint;
		}
		else
		{
			sprinting = useFire3Input
				? Input.GetAxis("Fire3") == 1f || Input.GetKey(KeyCode.LeftShift)
				: Input.GetKey(KeyCode.LeftShift);
		}

		if (sprinting && currentStamina > 0f)
		{
			currentStamina = Mathf.Clamp(currentStamina - drainPerSecond * Time.deltaTime, 0f, maxStamina);
			lastDrainTime = Time.time;
			UpdateUI();
			return;
		}

		if (Time.time - lastDrainTime >= regenDelay && currentStamina < maxStamina)
		{
			Debug.Log($"{name} ({GetInstanceID()}): Regenerating stamina: {currentStamina}/{maxStamina}");
			currentStamina = Mathf.Clamp(currentStamina + regenPerSecond * Time.deltaTime, 0f, maxStamina);
			Debug.Log($"{name} ({GetInstanceID()}): Regenerated stamina: {currentStamina}/{maxStamina}");
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
			//staminaSlider.maxValue = 300f;
			staminaSlider.value = currentStamina;
			//Debug.Log($"StaminaBarUI: Updated stamina bar. Current: {currentStamina}, Max: {maxStamina}, FillAmount: {staminaSlider.value}");
		}

		if (staminaText != null)
		{
			staminaText.text = $"{Mathf.CeilToInt(currentStamina)}/{Mathf.CeilToInt(maxStamina)}";
		}
		if (maxStaminaSlider != null)
        {
            //maxStaminaSlider.maxValue = 300f;
            maxStaminaSlider.value = maxStamina;
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
			Debug.Log($"Increasing max stamina by {amount}. Current max: {maxStamina}");
			float newMaxStamina = maxStamina + amount;
			// currentStamina += amount; // heal the added amount too
			
			// Tell the UI to update
			FindAnyObjectByType<StaminaBarUI>().UpdateStaminaBar(newMaxStamina);
			//SetMaxStamina(newMaxStamina,true);
			maxStamina = newMaxStamina;
			Debug.Log($"New max stamina: {maxStamina}");
			//UpdateUI();
			
    }
}
