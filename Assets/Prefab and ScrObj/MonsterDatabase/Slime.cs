public class Slime : Monster
{
    public override void Attack()
    {
        base.Attack();

        if (CombatManager.turn % 3 == 0)
        {
            AttackPattern.PlusAtack(atkX, atkY, this, damage, 1);
        }
        else
        {
            AttackPattern.CellAtack(atkX, atkY, this, damage);
        }
    }
    public override void CancelAttack()
    {
        base.CancelAttack();

        if (CombatManager.turn % 3 == 0)
        {
            AttackPattern.CancelPlusAtack(atkX, atkY, this, damage, 1);
        }
        else
        {
            AttackPattern.CancelCellAtack(atkX, atkY, this, damage);
        }
    }
}
