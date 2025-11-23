using UnityEngine;

public class MonsterDungeonManager : CharacterDungeonManager
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            LevelLoader.Instance.LoadCombatScene();
        }
    }
}
