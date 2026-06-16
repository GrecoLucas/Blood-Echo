using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private Vector3 dungeonSpawnPosition = new Vector3(0, 0, 0);

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
            Debug.Log("Scene carregada: " + scene.name);

        if (scene.name == "Dungeons")
        {
            CharacterController cc = GetComponent<CharacterController>();

            if (cc != null) cc.enabled = false;

            Debug.Log("A mover player para: " + dungeonSpawnPosition);
            transform.position = dungeonSpawnPosition;
            Debug.Log("Posição atual do player: " + transform.position);
            if (cc != null) cc.enabled = true;
        }
    }
}