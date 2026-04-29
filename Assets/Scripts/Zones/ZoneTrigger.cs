using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    public string zoneName; // O nome que aparecerá (ex: "A Forja Escura")
    private bool hasEntered = false;

private void OnTriggerEnter(Collider other)
{
    // Adicione esta linha abaixo para testar no console do Unity
    Debug.Log("Algo encostou na zona: " + other.name); 

    if (other.CompareTag("Player") && !hasEntered)
    {
        ZoneDisplayManager.Instance.ShowZoneName(zoneName);
        hasEntered = true;
    }
}

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasEntered = false; // Permite mostrar o nome de novo se ele voltar depois
        }
    }
}