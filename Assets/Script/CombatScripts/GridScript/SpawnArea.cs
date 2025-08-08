using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    public GridMap gridMap;
    [SerializeField] private Transform renderSpace;
    [SerializeField] private GameObject blankRendererPrefab;
    [SerializeField] private GameObject blankMonsterPrefab;
    private List<CombatInfo> spawnList = new();
    private List<MonsterCombatManager> monsterList = new();

    [SerializeField] private MonsterInfo slime;
    [SerializeField] private MonsterInfo bat;

    private void Start()
    {
        List<CombatInfo> combatInfos = DungeonManager.currentRoom?.GetMonsterInfoInRoom();
        if (combatInfos?.Count > 0)
        {
            foreach (CombatInfo info in combatInfos)
            {
                if (info.GetCurrentHP() == 0) continue;
                AddToSpawnList(info, 2);
            }
        }
        else
        {
            AddToSpawnList(new CombatInfo(slime, 2, 2), 1);
        }
        SpawnMonsterAtRandom(4);
    }

    public void AddToSpawnList(CombatInfo info, int amount = 1)
    {
        for(int i = 0; i < amount; i++)
        {
            spawnList.Add(info);
        }
    }

    public void SpawnMonsterAtRandom(int amount)
    {
        int remainingSlot = gridMap.Width * gridMap.Height - monsterList.Count;
        int spawnAmount = Mathf.Min(spawnList.Count, amount, remainingSlot);

        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject monsterObj = Instantiate(blankMonsterPrefab, transform);
            GameObject monsterRdr = Instantiate(blankRendererPrefab, renderSpace);
            MonsterCombatManager slot = monsterObj.GetComponent<MonsterCombatManager>();
            slot.SetGrid(gridMap.side);
            slot.InitMonster(spawnList[0].GetInfo(), monsterRdr, monsterList.Count, spawnList[0].GetCurrentHP());
            slot.OccupyAt(gridMap.GetRandomPos(), true);

            slot.PrepareToMove();

            monsterList.Add(slot);
            spawnList.RemoveAt(0);
        }
    }

    public void SpawnMonsterAt(int pos)
    {
        GameObject monsterObj = Instantiate(blankMonsterPrefab, transform);
        GameObject monsterRdr = Instantiate(blankRendererPrefab, renderSpace);
        MonsterCombatManager slot = monsterObj.GetComponent<MonsterCombatManager>();
        slot.SetGrid(gridMap.side);
        slot.InitMonster(spawnList[0].GetInfo(), monsterRdr, monsterList.Count, spawnList[0].GetCurrentHP());
        slot.OccupyAt(pos, true);

        slot.PrepareToMove();

        monsterList.Add(slot);
        spawnList.RemoveAt(0);
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
