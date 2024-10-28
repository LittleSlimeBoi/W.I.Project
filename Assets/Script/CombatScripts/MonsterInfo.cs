using UnityEngine;

[CreateAssetMenu(fileName = "New Monster", menuName = "Monster")]
public class MonsterInfo : ScriptableObject
{
    public string monsterName;
    public int maxHP;
    public int baseAtk;
    public Sprite monsterIcon;
    public Sprite monsterDes;
    public int moveRange;
}
