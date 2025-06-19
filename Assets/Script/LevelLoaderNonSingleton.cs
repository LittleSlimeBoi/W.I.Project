using UnityEngine;

public class LevelLoaderNonSingleton : MonoBehaviour
{
    public void LoadMainMenu()
    {
        LevelLoader.Instance.LoadMainMenu();
    }

    public void LoadDungeonScene()
    {
        LevelLoader.Instance.LoadDungeonScene();
    }

    public void QuitGame()
    {
        LevelLoader.Instance.QuitGame();
    }
}
