using UnityEngine;

public class MonsterDungeonManager : CharacterDungeonManager
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelLoader.Instance.LoadCombatScene();
        }
    }
}
