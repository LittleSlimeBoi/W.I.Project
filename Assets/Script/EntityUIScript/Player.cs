using UnityEngine;

public class Player : Character
{
    public int maxHP;
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
        /*
        GridMap swap = mySide;
        mySide = otherSide;
        otherSide = swap;
        */
        position = mySide.anchorPosX + mySide.anchorPosY * mySide.Width;
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

    public override int GetMaxStat(string statName)
    {
        return (statName == "HP") ? maxHP : maxMana;
    }

    public override int GetCurrentStat(string statName)
    {
        return (statName == "HP") ? hp : mana;
    }
}
