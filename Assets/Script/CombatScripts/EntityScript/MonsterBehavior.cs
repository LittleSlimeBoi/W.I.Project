using System.Collections.Generic;
using UnityEngine;

public class MonsterBehavior : ScriptableObject
{
    public virtual void Attack(int x, int y, GridMap otherSide, List<GridTile> atkArea, int damage)
    {

    }
    public virtual void CancelAttack(int x, int y, GridMap otherSide, List<GridTile> atkArea, int damage)
    {

    }
}
