using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets; // Importante para resetar a câmera e o mouse

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Settings")]
    public Vector3 lastBonfirePosition = Vector3.zero;
    public GameObject playerHealth;
    public GameObject PlayerPotions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerHealth = player;
            Potions potionsScript = player.GetComponentInChildren<Potions>();
            if (potionsScript != null) PlayerPotions = potionsScript.gameObject;

            // Forçamos o novo jogador a travar o mouse e ativar a câmera de look
            StarterAssetsInputs inputs = player.GetComponent<StarterAssetsInputs>();
            if (inputs != null)
            {
                inputs.cursorLocked = true;
                inputs.cursorInputForLook = true;
                
                // Forçamos o estado do cursor no Windows
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (lastBonfirePosition == Vector3.zero)
            {
                // Procuramos na cena qual fogueira é a default
                BonfireController[] bonfires = FindObjectsByType<BonfireController>(FindObjectsSortMode.None);
                foreach (var b in bonfires)
                {
                    if (b.isDefaultBonfire)
                    {
                        lastBonfirePosition = b.playerSp != null ? b.playerSp.position : b.transform.position;
                        break;
                    }
                }
            }

            // Teleporta o jogador se tivermos uma posição salva
            if (lastBonfirePosition != Vector3.zero)
            {
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false; 
                player.transform.position = lastBonfirePosition;
                if (cc != null) cc.enabled = true;
            }
        }
    }

    public void RestAtBonfire(Vector3 position)
    {
        lastBonfirePosition = position;
        RestorePlayer();
    }

    private void RestorePlayer()
    {
        if (playerHealth != null)
        {
            PlayerHealth health = playerHealth.GetComponent<PlayerHealth>();
            if (health != null) health.ResetHealth();
        }

        if (PlayerPotions != null)
        {
            Potions potions = PlayerPotions.GetComponent<Potions>();
            if (potions != null) potions.RestorePotions();
        }
    }

    public void RespawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && lastBonfirePosition != Vector3.zero)
        {
            // 1. Teleporte (Desativar CharacterController é essencial para funcionar)
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.transform.position = lastBonfirePosition;

            if (cc != null) cc.enabled = true;

            // 2. Restaura Vida e Poções
            RestorePlayer();

            // 3. Reativa os Inputs de Câmera/Mouse
            StarterAssetsInputs inputs = player.GetComponent<StarterAssetsInputs>();
            if (inputs != null)
            {
                inputs.cursorLocked = true;
                inputs.cursorInputForLook = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

        }
    }
}