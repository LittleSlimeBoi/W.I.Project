using UnityEngine;

public class PlayerCombatManager : CharacterCombatManager
{
    public StatBar manaBar;
    public int maxMana;
    public int mana;

    protected override void Start()
    {
        base.Start();
        OnStart();
    }

    public void OnStart()
    {
        Position = mySide.anchorPosX + mySide.anchorPosY * mySide.Width;
        Card.player = this;
        Debug.Log(Position);
    }

    public void UseMana(int cost)
    {
        mana -= cost;
        manaBar.UpdateStatBar();
    }

    public void RefillMana()
    {
        mana = maxMana;
        manaBar.UpdateStatBar();
    }

    public override int GetMaxStat(StatBar.StatType statType)
    {
        return (statType == StatBar.StatType.HP) ? maxHP : maxMana;
    }

    public override int GetCurrentStat(StatBar.StatType statType)
    {
        return (statType == StatBar.StatType.HP) ? hp : mana;
    }
}
