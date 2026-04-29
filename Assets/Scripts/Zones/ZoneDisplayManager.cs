using UnityEngine;
using TMPro;
using System.Collections;

public class ZoneDisplayManager : MonoBehaviour
{
    public static ZoneDisplayManager Instance; // Singleton para fácil acesso
    public TextMeshProUGUI zoneText;
    public float fadeDuration = 1.5f;
    public float stayDuration = 2.0f;

    private Coroutine activeFade;

    private void Awake() => Instance = this;

    public void ShowZoneName(string name)
    {
        if (activeFade != null) StopCoroutine(activeFade);
        activeFade = StartCoroutine(FadeSequence(name));
    }

    IEnumerator FadeSequence(string name)
    {
        zoneText.text = name;
        
        // Fade In
        yield return StartCoroutine(Fade(0, 1));
        
        // Tempo na tela
        yield return new WaitForSeconds(stayDuration);
        
        // Fade Out
        yield return StartCoroutine(Fade(1, 0));
    }

    IEnumerator Fade(float start, float end)
    {
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(start, end, elapsed / fadeDuration);
            zoneText.color = new Color(zoneText.color.r, zoneText.color.g, zoneText.color.b, alpha);
            yield return null;
        }
    }
}