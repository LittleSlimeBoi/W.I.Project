public class Bat : Monster
{
    public override void Attack()
    {
        base.Attack();

        if (CombatManager.turn % 3 == 0)
        {
            AttackPattern.CrossAtack(atkX, atkY, otherSide, damage, 1);
        }
        else
        {
            AttackPattern.TriangleAtack(atkX, atkY, otherSide, damage, 1);
        }
    }
    public override void CancelAttack()
    {
        base.CancelAttack();

        if (CombatManager.turn % 3 == 0)
        {
            AttackPattern.CancelCrossAtack(atkX, atkY, otherSide, damage, 1);
        }
        else
        {
            AttackPattern.CancelTriangleAtack(atkX, atkY, otherSide, damage, 1);
        }
    }
}
