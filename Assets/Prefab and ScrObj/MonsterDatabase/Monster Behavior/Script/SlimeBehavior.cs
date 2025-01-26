using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Slime Behavior", menuName = "Scriptable Object/Monster/Behavior/Slime")]
public class SlimeBehavior : MonsterBehavior
{
    public override void Attack(int x, int y, GridMap otherSide, List<GridTile> atkArea, int damage)
    {
        if (CombatManager.turn % 3 == 0)
        {
            AttackPattern.PlusAtack(x, y, otherSide, atkArea, damage, 1);
        }
        else
        {
            AttackPattern.CellAtack(x, y, otherSide, atkArea, damage);
        }
    }
    public override void CancelAttack(int x, int y, GridMap otherSide, List<GridTile> atkArea, int damage)
    {
        if (CombatManager.turn % 3 == 0)
        {
            AttackPattern.CancelPlusAtack(x, y, otherSide, atkArea, damage, 1);
        }
        else
        {
            AttackPattern.CancelCellAtack(x, y, otherSide, atkArea, damage);
        }
    }
}
