using UnityEngine;

public class MonsterDungeonManager : CharacterDungeonManager
{
    [SerializeField] private MonsterCombatManager monster;
    private int maxHP;
    private int hp;

    private void OnEnable()
    {
        maxHP = monster.info.maxHP;
        hp = maxHP;
    }
}
