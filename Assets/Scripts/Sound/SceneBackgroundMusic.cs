using UnityEngine;
using FMODUnity;

public class SceneBackgroundMusic : MonoBehaviour
{
    [Tooltip("Selecione o evento de música do FMOD para esta cena.")]
    [SerializeField] private EventReference backgroundMusicEvent;

    private FMOD.Studio.EventInstance bgmInstance;

    void Start()
    {
        // Inicia a música de fundo se um evento tiver sido selecionado
        if (!backgroundMusicEvent.IsNull)
        {
            bgmInstance = RuntimeManager.CreateInstance(backgroundMusicEvent);
            bgmInstance.start();
        }
    }

    private void OnDestroy()
    {
        // Garante que a música seja parada e a memória liberada caso a cena seja descarregada de outras formas
        if (bgmInstance.isValid())
        {
            bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            bgmInstance.release();
        }
    }
}
