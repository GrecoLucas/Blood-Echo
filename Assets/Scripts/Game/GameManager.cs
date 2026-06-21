using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Settings")]
    public Vector3 lastBonfirePosition = Vector3.zero;
    public Quaternion lastBonfireRotation = Quaternion.identity; 
    public string lastBonfireScene = "";
    public bool isRespawning = false;
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

            StarterAssetsInputs inputs = player.GetComponent<StarterAssetsInputs>();
            if (inputs != null)
            {
                inputs.cursorLocked = true;
                inputs.cursorInputForLook = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (lastBonfirePosition == Vector3.zero || string.IsNullOrEmpty(lastBonfireScene))
            {
                BonfireController[] bonfires = FindObjectsByType<BonfireController>(FindObjectsSortMode.None);
                foreach (var b in bonfires)
                {
                    if (b.isDefaultBonfire)
                    {
                        lastBonfirePosition = b.playerSp != null ? b.playerSp.position : b.transform.position;
                        lastBonfireRotation = b.playerSp != null ? b.playerSp.rotation : b.transform.rotation; 
                        lastBonfireScene = scene.name;
                        break;
                    }
                }
            }

            if (lastBonfirePosition != Vector3.zero && lastBonfireScene == scene.name)
            {
                if ((scene.name == "Area1" && PlayerSpawner.returningFromDungeon) ||
                    (scene.name == "Dungeons" && PlayerSpawner.enteringDungeon))
                {
                    // Do nothing, PlayerSpawner will position the player
                }
                else
                {
                    CharacterController cc = player.GetComponent<CharacterController>();
                    if (cc != null) cc.enabled = false; 
                    
                    player.transform.position = lastBonfirePosition;
                    // APLICANDO A ROTAÇÃO NO JOGADOR
                    player.transform.rotation = lastBonfireRotation; 
                    
                    if (cc != null) cc.enabled = true;
                }
            }
            
            isRespawning = false;
        }
    }

    public void RestAtBonfire(Vector3 position, Quaternion rotation) 
    {
        lastBonfirePosition = position;
        lastBonfireRotation = rotation; 
        lastBonfireScene = SceneManager.GetActiveScene().name;
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
            if (!string.IsNullOrEmpty(lastBonfireScene) && SceneManager.GetActiveScene().name != lastBonfireScene)
            {
                isRespawning = true;
                RestorePlayer();
                SceneManager.LoadScene(lastBonfireScene);
                return;
            }

            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.transform.position = lastBonfirePosition;
            // APLICANDO A ROTAÇÃO AO RENASCER
            player.transform.rotation = lastBonfireRotation; 

            if (cc != null) cc.enabled = true;

            // --- NOVO: Forçar a música de ambiente a recomeçar ---
            BackGroundSound[] bgSounds = FindObjectsByType<BackGroundSound>(FindObjectsSortMode.None);
            foreach (var bg in bgSounds)
            {
                Collider col = bg.GetComponent<Collider>();
                if (col != null && col.bounds.Contains(player.transform.position))
                {
                    // Finge que o jogador acabou de entrar no trigger para forçar o Play()
                    bg.SendMessage("OnTriggerEnter", player.GetComponent<Collider>(), SendMessageOptions.DontRequireReceiver);
                }
            }

            // --- NOVO: Forçar o som da fogueira atual a recomeçar ---
            BonfireController[] bonfires = FindObjectsByType<BonfireController>(FindObjectsSortMode.None);
            foreach (var b in bonfires)
            {
                // Se a posição de respawn da fogueira for igual à nossa lastBonfirePosition
                if (b.playerSp != null && Vector3.Distance(b.playerSp.position, lastBonfirePosition) < 0.1f)
                {
                    b.SetFireState(true);
                    break;
                }
            }
            // -----------------------------------------------------

            RestorePlayer();

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