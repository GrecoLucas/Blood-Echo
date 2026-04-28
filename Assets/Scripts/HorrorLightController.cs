using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class HorrorLightController : MonoBehaviour
{
    [Header("Lights")]
    [Tooltip("If empty, the script will auto-find Lights on this GameObject and children")]
    public Light[] lights;

    [Header("Intensity")]
    public float minIntensity = 0f;
    public float maxIntensity = 1.5f;
    [Tooltip("Intensity multiplier used for strong flashes")]
    public float flashIntensityMultiplier = 3f;

    [Header("Desync Between Lights")]
    public float initialDelayMin = 0f;
    public float initialDelayMax = 1.5f;

    [Header("Stable Phase")]
    public float stableDurationMin = 0.8f;
    public float stableDurationMax = 2.2f;
    public float stableJitterAmount = 0.06f;
    public float stableJitterIntervalMin = 0.05f;
    public float stableJitterIntervalMax = 0.2f;

    [Header("Off Phase")]
    public bool useHardOff = true;
    public bool instantOff = false;
    public float fadeToOffMin = 0.04f;
    public float fadeToOffMax = 0.2f;
    public float offHoldMin = 0.25f;
    public float offHoldMax = 1.4f;

    [Header("Flicker Phase")]
    public int flickerPulsesMin = 3;
    public int flickerPulsesMax = 8;
    public float flickerPulseDurationMin = 0.02f;
    public float flickerPulseDurationMax = 0.08f;
    public float flickerPauseMin = 0.01f;
    public float flickerPauseMax = 0.06f;

    [Header("Recover Phase")]
    public float recoverDurationMin = 0.3f;
    public float recoverDurationMax = 1.2f;

    [Header("Color (optional)")]
    public bool randomizeColor = false;
    public Color minColor = Color.black;
    public Color maxColor = Color.white;

    float[] baseIntensities;
    Color[] baseColors;

    void Awake()
    {
        if (lights == null || lights.Length == 0)
            lights = GetComponentsInChildren<Light>(true);

        baseIntensities = new float[lights.Length];
        baseColors = new Color[lights.Length];
        for (int i = 0; i < lights.Length; i++)
        {
            baseIntensities[i] = lights[i].intensity;
            baseColors[i] = lights[i].color;
        }
    }

    void OnEnable()
    {
        StopAllCoroutines();
        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] == null) continue;
            StartCoroutine(LightLoop(i));
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
        // restore base values
        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] == null) continue;
            lights[i].intensity = baseIntensities[i];
            lights[i].color = baseColors[i];
        }
    }

    IEnumerator LightLoop(int index)
    {
        Light currentLight = lights[index];
        if (currentLight == null) yield break;

        yield return new WaitForSeconds(Random.Range(initialDelayMin, initialDelayMax));

        while (true)
        {
            yield return StablePhase(index);
            yield return OffPhase(index);
            yield return FlickerPhase(index);
            yield return RecoverPhase(index);
        }
    }

    IEnumerator StablePhase(int index)
    {
        Light currentLight = lights[index];
        if (currentLight == null) yield break;

        float duration = Random.Range(stableDurationMin, stableDurationMax);
        float elapsed = 0f;
        float baseIntensity = Mathf.Clamp(baseIntensities[index], minIntensity, maxIntensity);

        while (elapsed < duration)
        {
            float jitter = Random.Range(-stableJitterAmount, stableJitterAmount);
            float target = Mathf.Clamp(baseIntensity + jitter, minIntensity, maxIntensity);
            float stepDuration = Random.Range(stableJitterIntervalMin, stableJitterIntervalMax);
            yield return SmoothSingle(index, target, stepDuration);
            elapsed += stepDuration;
        }
    }

    IEnumerator OffPhase(int index)
    {
        if (useHardOff && instantOff)
        {
            if (lights[index] != null)
                lights[index].intensity = minIntensity;
        }
        else
        {
            yield return SmoothSingle(index, minIntensity, Random.Range(fadeToOffMin, fadeToOffMax));
        }

        yield return new WaitForSeconds(Random.Range(offHoldMin, offHoldMax));
    }

    IEnumerator FlickerPhase(int index)
    {
        Light currentLight = lights[index];
        if (currentLight == null) yield break;

        int pulses = Random.Range(flickerPulsesMin, flickerPulsesMax + 1);
        float flashPeak = Mathf.Clamp(maxIntensity * flashIntensityMultiplier, minIntensity, maxIntensity * 8f);

        for (int pulse = 0; pulse < pulses; pulse++)
        {
            float pulseUpTarget = Random.Range(maxIntensity * 0.75f, flashPeak);
            float pulseUpDuration = Random.Range(flickerPulseDurationMin, flickerPulseDurationMax);
            yield return SmoothSingle(index, pulseUpTarget, pulseUpDuration);

            float pulseDownDuration = Random.Range(flickerPulseDurationMin, flickerPulseDurationMax);
            if (useHardOff)
                yield return SmoothSingle(index, minIntensity, pulseDownDuration);
            else
                yield return SmoothSingle(index, Mathf.Clamp(minIntensity + 0.08f, minIntensity, maxIntensity), pulseDownDuration);

            yield return new WaitForSeconds(Random.Range(flickerPauseMin, flickerPauseMax));
        }
    }

    IEnumerator RecoverPhase(int index)
    {
        float target = Mathf.Clamp(baseIntensities[index], minIntensity, maxIntensity);
        yield return SmoothSingle(index, target, Random.Range(recoverDurationMin, recoverDurationMax));
    }

    IEnumerator SmoothSingle(int index, float targetIntensity, float duration)
    {
        Light currentLight = lights[index];
        if (currentLight == null) yield break;

        float startIntensity = currentLight.intensity;
        Color startColor = currentLight.color;
        Color targetColor = randomizeColor ? Random.ColorHSV() : baseColors[index];

        if (duration <= 0f)
        {
            currentLight.intensity = targetIntensity;
            if (randomizeColor)
                currentLight.color = Color.Lerp(minColor, maxColor, Random.value);
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / duration);
            currentLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, p);
            if (randomizeColor)
                currentLight.color = Color.Lerp(startColor, targetColor, p);
            yield return null;
        }

        currentLight.intensity = targetIntensity;
        if (randomizeColor)
            currentLight.color = targetColor;
    }
}
