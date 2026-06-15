using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    public string zoneName; // O nome que aparecerá (ex: "A Forja Escura")
    private bool hasEntered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || hasEntered)
        {
            return;
        }

        ZoneDisplayManager displayManager = ZoneDisplayManager.Instance;
        if (displayManager == null)
        {
            Debug.LogWarning("ZoneDisplayManager não foi encontrado na cena. Adicione o gerenciador da UI da zona.");
            return;
        }

        if (string.IsNullOrWhiteSpace(zoneName))
        {
            Debug.LogWarning("ZoneTrigger está sem zoneName configurado.");
            return;
        }

        displayManager.ShowZoneName(zoneName);
        hasEntered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasEntered = false; // Permite mostrar o nome de novo se ele voltar depois
        }
    }
}