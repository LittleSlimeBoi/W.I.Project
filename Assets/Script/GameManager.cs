using System.Xml.Serialization;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MainMenu,
        DungeonScene,
        CombatScene,
        EndScreen
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
    private void Update()
    {
        switch (State)
        {
            case GameState.MainMenu:
                
                break;
            case GameState.DungeonScene:
                break;
            case GameState.CombatScene:
                break;
            case GameState.EndScreen:
                HandleEndScreen();
                break;
        }
    }

    public void HandleEndScreen()
    {
        if(State == GameState.EndScreen)
        {
            if(DungeonManager.Instance != null)
            {
                Destroy(DungeonManager.Instance.gameObject);
            }
        }
    }
}
