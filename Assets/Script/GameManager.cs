using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MainMenu,
        DungeonScene,
        CombatScene,
        EndScene
    }
    public static GameManager Instance;
    public static GameState State;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(Instance);
        }
    }

}
