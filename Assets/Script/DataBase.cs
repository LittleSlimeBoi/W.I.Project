using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data Base", menuName = "DataBase/CombatScene")]
public class DataBase : ScriptableObject
{
    public List<CardInfo> cardDataBase = new();
    public List<MonsterCombatManager> monsterDataBase = new();
}
