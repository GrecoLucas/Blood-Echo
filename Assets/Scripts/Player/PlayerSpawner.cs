using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private Vector3 dungeonSpawnPosition = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 area1ReturnPosition = new Vector3(-18, 0, 22);
    [SerializeField] private MapController mapController;
    [SerializeField] private FogOfWarUI fogOfWarUI;
    public static bool returningFromDungeon = false;    
    public static bool enteringDungeon = false;

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
            UIDocument mapDocument = FindObjectOfType<UIDocument>();
            mapController.Setup(mapDocument, gameObject);
            fogOfWarUI.Setup(mapDocument, gameObject);
            mapController.enabled = true;

            // Find all dungeon notes and set them up with the player reference
            GameObject[] dungeonNotes = GameObject.FindGameObjectsWithTag("DungeonNote");
            foreach (GameObject note in dungeonNotes)
            {
                NoteController noteController = note.GetComponent<NoteController>();
                if (noteController != null)
                    noteController.Setup(gameObject);
                else
                    Debug.LogWarning("DungeonNote object '" + note.name + "' is missing a NoteController component!");
            }
            
            if (enteringDungeon)
            {
                if (cc != null) cc.enabled = false;

                Debug.Log("A mover player para: " + dungeonSpawnPosition);
                transform.position = dungeonSpawnPosition;
                Debug.Log("Posição atual do player: " + transform.position);
                if (cc != null) cc.enabled = true;

                StartCoroutine(ResetEnteringFlag());
            }
        }
        if (scene.name == "Area1" && returningFromDungeon)
        {
            if (cc != null) cc.enabled = false;
            Debug.Log("A mover para Area1 return: " + area1ReturnPosition);
            transform.position = area1ReturnPosition;
            Debug.Log("Posição depois: " + transform.position);
            if (cc != null) cc.enabled = true;

            mapController.enabled = false;
            StartCoroutine(ResetReturningFlag());
        }
    }

    private IEnumerator ResetEnteringFlag()
    {
        yield return new WaitForEndOfFrame();
        enteringDungeon = false;
    }

    private IEnumerator ResetReturningFlag()
    {
        yield return new WaitForEndOfFrame();
        returningFromDungeon = false;
    }
}