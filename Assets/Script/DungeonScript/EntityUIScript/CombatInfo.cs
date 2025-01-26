using UnityEngine;

public class CombatInfo : MonoBehaviour
{
    [SerializeField] private MonsterInfo info;
    private int maxHP = 3;
    private int hp = 3;

    public int GetCurrentHP()
    {
        return hp;
    }

    public MonsterInfo GetInfo()
    {
        return info;
    }

    private void OnEnable()
    {
        maxHP = info.maxHP;
        hp = maxHP;
    }
}
