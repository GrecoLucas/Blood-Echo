using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private Vector3 dungeonSpawnPosition = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 area1ReturnPosition = new Vector3(-18, 0, 22);
    public static bool returningFromDungeon = false;    
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
        CharacterController cc = GetComponent<CharacterController>();

        if (scene.name == "Dungeons")
        {
            if (cc != null) cc.enabled = false;

            Debug.Log("A mover player para: " + dungeonSpawnPosition);
            transform.position = dungeonSpawnPosition;
            Debug.Log("Posição atual do player: " + transform.position);
            if (cc != null) cc.enabled = true;
        }
        if (scene.name == "Area1" && returningFromDungeon)
        {
            if (cc != null) cc.enabled = false;
            Debug.Log("A mover para Area1 return: " + area1ReturnPosition);
            transform.position = area1ReturnPosition;
            Debug.Log("Posição depois: " + transform.position);
            if (cc != null) cc.enabled = true;

            returningFromDungeon = false;
        }
    }
}