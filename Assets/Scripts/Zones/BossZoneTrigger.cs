using UnityEngine;

public class BossZoneTrigger : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameObject bossUIContainer; // O objeto pai da barra de vida na UI
    [SerializeField] private GameObject barrier; // A barreira que impede a saída do jogador

    private void Start()
    {
        if (bossUIContainer != null)
            bossUIContainer.SetActive(false);

        if (barrier != null)
            barrier.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou tem a tag Player
        if (other.CompareTag("Player"))
        {
            ShowBossUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Quando o jogador sair da zona, esconde a UI do boss
        if (other.CompareTag("Player"))
        {
            HideBossUI();
        }
    }

    private void ShowBossUI()
    {
        if (bossUIContainer != null)
        {
            bossUIContainer.SetActive(true); // Faz a barra de vida aparecer
        }

        if (barrier != null)
        {
            barrier.SetActive(true); // Ativa a barreira física
        }
    }

    private void HideBossUI()
    {
        if (bossUIContainer != null)
        {
            bossUIContainer.SetActive(false);
        }
    }
}