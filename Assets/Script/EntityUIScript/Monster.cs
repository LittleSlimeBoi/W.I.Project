using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : Character
{
    public MonsterInfo info;
    protected int damage;
    protected int atkX, atkY;
    public List<GridTile> atkArea;

    [HideInInspector] public int fieldIndex;

    public void OnDeath()
    {
        CancelAttack();
        mySide.grid[GetPosX(), GetPosY()].Occupied = false;

        transform.SetParent(null);
        Destroy(gameObject);
    }

    // Move at each turn's start: Select a random Tile in move range that is safe, if cant move then stay in the same tile
    public virtual void Move()
    {
        int x, y, newPosition, safeBlock = 0;
        do
        {
            int xIncrement = Random.Range(-info.moveRange, info.moveRange + 1);
            int yIncrement = Random.Range(Mathf.Abs(xIncrement) - info.moveRange, info.moveRange - Mathf.Abs(xIncrement) + 1);
            int tempX = GetPosX() + xIncrement;
            int tempY = GetPosY() + yIncrement;

            if (tempX < 0) xIncrement = -tempX;
            else if (tempX >= mySide.Width) xIncrement = mySide.Width - tempX;
            if (tempY < 0) yIncrement = -tempY;
            else if (tempY >= mySide.Height) yIncrement = mySide.Height - tempY;

            newPosition = Position + xIncrement + yIncrement * mySide.Width;
            x = newPosition % mySide.Width;
            y = newPosition / mySide.Width;
            safeBlock++;
            if (!mySide.IsValidPosition(x, y)) continue;
        }
        while ((!mySide.grid[x, y].Walkable || mySide.grid[x, y].Occupied) && (safeBlock < 50));

        if (safeBlock < 50) MoveTo(newPosition);
    }

    // Mark attack range
    public virtual void Attack()
    {
        damage = info.baseAtk + bonusAtk;
        atkX = TranslatePosX();
        atkY = TranslatePosY();
    }

    // Cancel attack mark when death/ stun
    public virtual void CancelAttack()
    {

    }

    public void InitMonster(MonsterInfo monsterInfo, int fieldIndex)
    {
        info = monsterInfo;
        name = info.monsterName;
        hp = info.maxHP;
        characterIcon.sprite = info.monsterIcon;
        characterDes.sprite = info.monsterDes;

        this.fieldIndex = fieldIndex;
    }

    public override int GetMaxStat(StatBar.StatType statType)
    {
        return info.maxHP;
    }

    public override int GetCurrentStat(StatBar.StatType statType)
    {
        return hp;
    }
}
