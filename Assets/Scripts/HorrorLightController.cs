using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class HorrorLightController : MonoBehaviour
{
    [Header("Lights")]
    [Tooltip("If empty, the script will auto-find Lights on this GameObject and children")]
    public Light[] lights;

    [Header("Intensity")]
    public float minIntensity = 0.8f;       // Fire never goes dark
    public float maxIntensity = 1.4f;       // Subtle upper bound
    [Tooltip("Intensity multiplier used for strong flashes")]
    public float flashIntensityMultiplier = 1.2f;   // No dramatic flashes for fire

    [Header("Desync Between Lights")]
    public float initialDelayMin = 0f;
    public float initialDelayMax = 0.4f;    // Small desync so flames feel independent

    [Header("Stable Phase")]
    public float stableDurationMin = 0.1f;  // Fire rarely holds perfectly still
    public float stableDurationMax = 0.4f;
    public float stableJitterAmount = 0.15f;            // Frequent small intensity shifts
    public float stableJitterIntervalMin = 0.03f;       // Fast jitter = flickering flame
    public float stableJitterIntervalMax = 0.09f;

    [Header("Off Phase")]
    public bool useHardOff = false;         // Fire never cuts out hard
    public bool instantOff = false;
    public float fadeToOffMin = 0.05f;
    public float fadeToOffMax = 0.1f;
    public float offHoldMin = 0.0f;         // Fire doesn't hold at zero
    public float offHoldMax = 0.05f;

    [Header("Flicker Phase")]
    public int flickerPulsesMin = 2;
    public int flickerPulsesMax = 4;
    public float flickerPulseDurationMin = 0.03f;
    public float flickerPulseDurationMax = 0.07f;
    public float flickerPauseMin = 0.01f;
    public float flickerPauseMax = 0.04f;

    [Header("Recover Phase")]
    public float recoverDurationMin = 0.1f;
    public float recoverDurationMax = 0.3f; // Quick recovery keeps it feeling alive

    [Header("Color")]
    public bool randomizeColor = true;
    public Color minColor = new Color(1f, 0.3f, 0f);       // Deep orange
    public Color maxColor = new Color(1f, 0.85f, 0.3f);    // Bright warm yellow
    [Tooltip("How long each colour transition takes (seconds)")]
    public float colorTransitionDurationMin = 1.5f;
    public float colorTransitionDurationMax = 3.5f;
    [Tooltip("How long to hold a colour before picking a new one")]
    public float colorHoldDurationMin = 2.0f;
    public float colorHoldDurationMax = 5.0f;

    float[] baseIntensities;
    Color[] baseColors;
    Color[] currentColors; // tracks the active colour per light for smooth handoff

    void Awake()
    {
        if (lights == null || lights.Length == 0)
            lights = GetComponentsInChildren<Light>(true);

        baseIntensities = new float[lights.Length];
        baseColors = new Color[lights.Length];
        currentColors = new Color[lights.Length];
        for (int i = 0; i < lights.Length; i++)
        {
            baseIntensities[i] = lights[i].intensity;
            baseColors[i] = lights[i].color;
            currentColors[i] = lights[i].color;
        }
    }

    void OnEnable()
    {
        StopAllCoroutines();
        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] == null) continue;
            StartCoroutine(LightLoop(i));
            if (randomizeColor)
                StartCoroutine(ColorLoop(i));
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] == null) continue;
            lights[i].intensity = baseIntensities[i];
            lights[i].color = baseColors[i];
            currentColors[i] = baseColors[i];
        }
    }

    // Colour changes on its own slow independent loop, completely separate from intensity
    IEnumerator ColorLoop(int index)
    {
        Light currentLight = lights[index];
        if (currentLight == null) yield break;

        while (true)
        {
            // Hold current colour for a while before transitioning
            yield return new WaitForSeconds(Random.Range(colorHoldDurationMin, colorHoldDurationMax));

            Color startColor = currentColors[index];
            Color targetColor = Color.Lerp(minColor, maxColor, Random.value);
            float duration = Random.Range(colorTransitionDurationMin, colorTransitionDurationMax);
            float t = 0f;

            while (t < duration)
            {
                t += Time.deltaTime;
                float p = Mathf.SmoothStep(0f, 1f, t / duration);
                currentColors[index] = Color.Lerp(startColor, targetColor, p);
                if (lights[index] != null)
                    lights[index].color = currentColors[index];
                yield return null;
            }

            currentColors[index] = targetColor;
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
        // For fire, "off" is just a brief dip, never darkness
        float dipTarget = Mathf.Clamp(minIntensity, minIntensity, minIntensity + 0.1f);

        if (useHardOff && instantOff)
        {
            if (lights[index] != null)
                lights[index].intensity = dipTarget;
        }
        else
        {
            yield return SmoothSingle(index, dipTarget, Random.Range(fadeToOffMin, fadeToOffMax));
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
            // Dip back to minIntensity (not zero) between pulses
            yield return SmoothSingle(index, minIntensity, pulseDownDuration);

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

        if (duration <= 0f)
        {
            currentLight.intensity = targetIntensity;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / duration);
            currentLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, p);
            yield return null;
        }

        currentLight.intensity = targetIntensity;
    }
}