using System.Collections.Generic;
using UnityEngine;

public class MonsterCombatManager : CharacterCombatManager
{
    public MonsterInfo info;
    protected int damage;
    protected int atkX, atkY;
    [HideInInspector] public List<GridTile> atkArea;
    [HideInInspector] public List<GridTile> moveArea;

    [HideInInspector] public int fieldIndex;

    public void OnDeath()
    {
        CancelAttack();
        mySide.grid[GetPosX(), GetPosY()].IsOccupied = false;

        transform.SetParent(null);
        Destroy(myRenderer);
        Destroy(gameObject);
    }

    // Prepare to move at turn's start: Add all tiles in monster's moveRange to moveArea
    public void PrepareToMove()
    {
        for(int i = -info.moveRange; i <= info.moveRange; i++)
        {
            int x = GetPosX() + i;
            if (!mySide.IsValidX(x)) { continue; }
            for (int j = Mathf.Abs(i) - info.moveRange; j <= info.moveRange - Mathf.Abs(i); j++)
            {
                int y = GetPosY() + j;
                if (!mySide.IsValidY(y)) { continue; }

                // Add condition here
                moveArea.Add(mySide.grid[x, y]);
            }
        }
    }
    // Move at turn's end: Select a random unoccupied tile in moveArea to move into to
    public void Move()
    {
        int newPosition;
        int r = Random.Range(0, moveArea.Count);
        if(moveArea.Count > 0 )
        {
            while ((!moveArea[r].IsWalkable || moveArea[r].IsOccupied) && moveArea.Count != 0)
            {
                moveArea.RemoveAt(r);
                r = Random.Range(0, moveArea.Count);
            }
            if (moveArea.Count != 0)
            {
                newPosition = moveArea[r].TileIndex;
                MoveTo(newPosition);
            }
        }
        moveArea.Clear();
    }

    // Mark attack range
    public void Attack()
    {
        damage = info.baseAtk + bonusAtk;
        atkX = TranslatePosX();
        atkY = TranslatePosY();
        info.behavior.Attack(atkX, atkY, otherSide, atkArea, damage);
    }

    // Cancel attack mark when death/ stun
    public void CancelAttack()
    {
        info.behavior.CancelAttack(atkX, atkY, otherSide, atkArea, damage);
    }

    public void InitMonster(MonsterInfo monsterInfo, GameObject monsterRenderer, int fieldIndex, int hp)
    {
        info = monsterInfo;
        myRenderer = monsterRenderer;
        myRenderer.GetComponent<SpriteRenderer>().sprite = info.monsterIcon;
        myRenderer.GetComponent<SpriteRenderer>().flipX = mySide.side == GridSide.Enemy;
        myRenderer.GetComponent<Animator>().runtimeAnimatorController = info.controller;
        name = info.monsterName;
        maxHP = info.maxHP;
        characterDes.sprite = info.monsterDes;

        this.fieldIndex = fieldIndex;
        this.hp = (hp != 0) ? hp : maxHP;
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
