using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    public GridMap gridMap;
    private List<MonsterCombatManager> spawnList = new();
    private List<MonsterCombatManager> monsterList = new();
    public DataBase dataBase;

    private void Start()
    {
        AddToSpawnList(dataBase.monsterDataBase[0], 3);
        AddToSpawnList(dataBase.monsterDataBase[1], 1);
        AddToSpawnList(dataBase.monsterDataBase[0], 2);
        AddToSpawnList(dataBase.monsterDataBase[1], 2);

        SpawnMonster(4);
    }

    public void AddToSpawnList(MonsterCombatManager monster, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            spawnList.Add(monster);
        }
    }

    public void SpawnMonster(int amount)
    {
        int remainingSlot = gridMap.Width * gridMap.Height - monsterList.Count;
        int spawnAmount = Mathf.Min(spawnList.Count, amount, remainingSlot);

        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject monsterObj = Instantiate(spawnList[0].gameObject);
            monsterObj.transform.SetParent(this.transform);
            MonsterCombatManager slot = monsterObj.GetComponent<MonsterCombatManager>();
            slot.SetGrid();
            slot.InitMonster(slot.info, monsterList.Count);
            slot.OccupyAt(gridMap.GetRandomPos(), true);

            slot.PrepareToMove();

            monsterList.Add(slot);
            spawnList.RemoveAt(0);
        }
    }

    public void DespawnMonster(int index)
    {
        monsterList.RemoveAt(index);

        // Update field index for other monsters
        for (int i = index; i < monsterList.Count; i++)
        {
            monsterList[i].fieldIndex = i;
        }
    }

    public void AllMonstersAction()
    {
        foreach(MonsterCombatManager monster in monsterList)
        {
            monster.Move();
            monster.Attack();

            monster.PrepareToMove();
        }
    }

    public void AllMonstersCancelAttack()
    {
        foreach (MonsterCombatManager monster in monsterList)
        {
            monster.CancelAttack();
        }
    }

    public bool NoMoreMonster()
    {
        return monsterList.Count == 0;
    }

    public bool WaitingToSpawn()
    {
        return spawnList.Count > 0;
    }
}
