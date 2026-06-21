using UnityEngine;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;

    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Garante que começa transparente
        fadeCanvasGroup.alpha = 0;
        fadeCanvasGroup.interactable = false;
        fadeCanvasGroup.blocksRaycasts = false;
    }

    public IEnumerator FadeOut()
    {
        fadeCanvasGroup.blocksRaycasts = true;
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1;
    }

    public IEnumerator FadeIn()
    {
        yield return null;
        
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = 1 - Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0;
        fadeCanvasGroup.blocksRaycasts = false;
    }
}