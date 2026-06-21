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
            // Para todos os sons atuais com fade out antes de trocar de cena
            FMODUnity.RuntimeManager.GetBus("bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

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
