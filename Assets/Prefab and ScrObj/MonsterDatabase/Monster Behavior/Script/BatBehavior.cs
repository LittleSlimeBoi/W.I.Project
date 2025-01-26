using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bat Behavior", menuName = "Scriptable Object/Monster/Behavior/Bat")]
public class BatBehavior : MonsterBehavior
{
    public override void Attack(int x, int y, GridMap otherSide, List<GridTile> atkArea, int damage)
    {
        if (CombatManager.turn % 3 == 0)
        {
            AttackPattern.CrossAtack(x, y, otherSide, atkArea, damage, 1);
        }
        else
        {
            AttackPattern.TriangleAtack(x, y, otherSide, atkArea, damage, 1);
        }
    }
    public override void CancelAttack(int x, int y, GridMap otherSide, List<GridTile> atkArea, int damage)
    {
        if (CombatManager.turn % 3 == 0)
        {
            AttackPattern.CancelCrossAtack(x, y, otherSide, atkArea, damage, 1);
        }
        else
        {
            AttackPattern.CancelTriangleAtack(x, y, otherSide, atkArea, damage, 1);
        }
    }
}
