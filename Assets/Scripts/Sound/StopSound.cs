using UnityEngine;
using FMODUnity;

public class PararSomCastelo : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter emissorDoTerreno;

    // Disparado no momento exato em que o jogador ENTRA no cubo
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Manda o FMOD dar Play no evento
            emissorDoTerreno.Play();
        }
    }

    // Disparado no momento exato em que o jogador SAI do cubo
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Manda o FMOD parar, respeitando os segundos de fade out que você configurou
            emissorDoTerreno.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}