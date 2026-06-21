using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "LevelChange"){
            if(SceneManager.GetActiveScene().name == "Dungeons"){
                other.gameObject.SetActive(false);
                PlayerSpawner.returningFromDungeon = true;
                SceneManager.LoadScene("Area1");
            }
            else
            {
                other.gameObject.SetActive(false);
                PlayerSpawner.enteringDungeon = true;
                SceneManager.LoadScene("Dungeons");
            }
        }
    }
}
