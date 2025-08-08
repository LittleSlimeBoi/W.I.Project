using UnityEngine;

public class MonsterDungeonManager : CharacterDungeonManager
{
    // Using is Trigger on collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelLoader.Instance.LoadCombatScene();
        }
    }
    // Not using is Trigger on collider
    //private void OnCollisionEnter2D(Collision2D other)
    //{
    //    if (other.collider.CompareTag("Player"))
    //    {
    //        LevelLoader.Instance.LoadCombatScene();
    //    }
    //}
}
