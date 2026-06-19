using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Settings")]
    public Vector3 lastBonfirePosition = Vector3.zero;
    public Quaternion lastBonfireRotation = Quaternion.identity; 
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

            if (lastBonfirePosition == Vector3.zero)
            {
                BonfireController[] bonfires = FindObjectsByType<BonfireController>(FindObjectsSortMode.None);
                foreach (var b in bonfires)
                {
                    if (b.isDefaultBonfire)
                    {
                        lastBonfirePosition = b.playerSp != null ? b.playerSp.position : b.transform.position;
                        lastBonfireRotation = b.playerSp != null ? b.playerSp.rotation : b.transform.rotation; 
                        break;
                    }
                }
            }

            if (lastBonfirePosition != Vector3.zero)
            {
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false; 
                
                player.transform.position = lastBonfirePosition;
                // APLICANDO A ROTAÇÃO NO JOGADOR
                player.transform.rotation = lastBonfireRotation; 
                
                if (cc != null) cc.enabled = true;
            }
        }
    }

    public void RestAtBonfire(Vector3 position, Quaternion rotation) 
    {
        lastBonfirePosition = position;
        lastBonfireRotation = rotation; 
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
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.transform.position = lastBonfirePosition;
            // APLICANDO A ROTAÇÃO AO RENASCER
            player.transform.rotation = lastBonfireRotation; 

            if (cc != null) cc.enabled = true;

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