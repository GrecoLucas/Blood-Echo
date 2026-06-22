using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
                StartCoroutine(LoadSceneWithFade("Area1"));
            }
            else
            {
                other.gameObject.SetActive(false);
                PlayerSpawner.enteringDungeon = true;
                StartCoroutine(LoadSceneWithFade("Dungeons"));
            }
        }
    }

    IEnumerator LoadSceneWithFade(string sceneName)
    {
        yield return StartCoroutine(SceneFader.Instance.FadeOut());
        SceneManager.LoadScene(sceneName);
    }
}
