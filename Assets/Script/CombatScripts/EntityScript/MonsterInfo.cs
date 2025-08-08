using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Info", menuName = "Scriptable Object/Monster/Info")]
public class MonsterInfo : ScriptableObject
{
    public string monsterName;
    public int maxHP;
    public int baseAtk;
    public Sprite monsterIcon;
    public Sprite monsterDes;
    public int moveRange;
    public MonsterBehavior behavior;
    public RuntimeAnimatorController controller;
}
