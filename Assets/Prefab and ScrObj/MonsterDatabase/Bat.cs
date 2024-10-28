public class Bat : MonsterCombatManager
{
    public override void Attack()
    {
        base.Attack();

        if (CombatManager.turn % 3 == 0)
        {
            AttackPattern.CrossAtack(atkX, atkY, this, damage, 1);
        }
        else
        {
            AttackPattern.TriangleAtack(atkX, atkY, this, damage, 1);
        }
    }
    public override void CancelAttack()
    {
        base.CancelAttack();

        if (CombatManager.turn % 3 == 0)
        {
            AttackPattern.CancelCrossAtack(atkX, atkY, this, damage, 1);
        }
        else
        {
            AttackPattern.CancelTriangleAtack(atkX, atkY, this, damage, 1);
        }
    }
}
