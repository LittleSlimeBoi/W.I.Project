using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;
    public Animator transition;
    public Image image;
    public float transitionTime = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }else
        {
            Destroy(gameObject);
        }
    }

    public void LoadDungeonScene()
    {
        StartCoroutine(LoadLevel("Dungeon Scene"));
        GameManager.State = GameManager.GameState.DungeonScene;
    }

    public void LoadCombatScene()
    {
        StartCoroutine(LoadLevel("Combat Scene"));
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
        GameManager.State = GameManager.GameState.MainMenu;
    }

    public void LoadEndScreen()
    {
        StartCoroutine(LoadLevel("End Screen"));
        GameManager.State = GameManager.GameState.EndScreen;
    }

    IEnumerator LoadLevel(string sceneName)
    {
        Color color = image.color;
        color.a = 1;
        image.color = color;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        transition.SetTrigger("End");

        if (sceneName != "Dungeon Scene") 
        { 
            DungeonManager.Instance.SavePositionInDungeon(); 
        }

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (sceneName == "Dungeon Scene") DungeonManager.Instance.OnLoadDungeon();
        else DungeonManager.Instance.OnUnloadDungeon();

        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);

        color.a = 0;
        image.color = color;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
