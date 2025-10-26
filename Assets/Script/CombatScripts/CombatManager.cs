using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class CombatManager : MonoBehaviour
{
    public enum CombatState
    {
        YourTurn,
        EnemyTurn
    }

    public static CombatManager Instance;

    public static int turn = 0;
    public Board board;
    public HandPanel handPanel;
    public DropZone dropZone;
    public CancelPanel cancelPanel;
    public PlayerCombatManager player;
    public SpawnArea enemySpawner;
    public GridMap playerGrid;
    public GridMap enemyGrid;
    public Button endturnButton;

    //public CombatState state = CombatState.YourTurn;
    //public static event Action<CombatState> OnCombatStateChange;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        board.InitBoard();
        handPanel.FlushHand();

        NewTurn();
    }

    /*
    public void UpdateCombatState(CombatState newState)
    {
        state = newState;
        switch (newState)
        {
            case CombatState.YourTurn:
                break;
            case CombatState.EnemyTurn:
                break;
        }

        OnCombatStateChange?.Invoke(newState);
    }
    */

    public void DebugWin()
    {
        DungeonManager.currentRoom.roomState = RoomState.Complete;
        LevelLoader.Instance.LoadDungeonScene();
    }

    // Button in game use this function
    public void OnEndTurn()
    {
        endturnButton.interactable = false;
        // Clear hand
        board.Flush();

        // Player take damage
        player.TakeDamge(playerGrid.grid[player.GetPosX(), player.GetPosY()].DamageIncoming);
        if (player.IsDead)
        {
            // Load lose screen
            LevelLoader.Instance.LoadEndScreen();
        }

        // Refresh player grid
        foreach(GridTile tile in playerGrid.grid)
        {
            tile.UpdateTileOnNewTurn();
        }

        if(enemySpawner.NoMoreMonster() && !enemySpawner.WaitingToSpawn())
        {
            // Load win scene
            DungeonManager.currentRoom.roomState = RoomState.Complete;
            LevelLoader.Instance.LoadDungeonScene();
        }
        else
        {
            NewTurn();
        }
    }

    public void NewTurn()
    {
        if (turn != 0) enemySpawner.AllMonstersCancelAttack();
        turn++;

        player.RefillMana();

        board.DrawCard(5);
        // Monster turn
        if(enemySpawner.NoMoreMonster() && enemySpawner.WaitingToSpawn())
        {
            enemySpawner.SpawnMonsterAtRandom(4);
        }
        enemySpawner.AllMonstersAction();
        // Update tile
        playerGrid.UpdateGridOnNewTurn();

        StartCoroutine(EnableEndButton(1.2f));
    }

    public void UseCard()
    {
        if (playerGrid.IsFinish() && enemyGrid.IsFinish())
        {
            int handIndex = dropZone.card.handIndex;

            // Handle attack first
            if (enemyGrid.hasSelectedTile)
            {
                // If attacks single target
                if (!dropZone.card.info.isAOE)
                {
                    if (enemyGrid.grid[enemyGrid.selectedX, enemyGrid.selectedY].IsOccupied)
                    {
                        for(int i = 1; i < enemySpawner.transform.childCount; i++)
                        {
                            MonsterCombatManager temp = enemySpawner.transform.GetChild(i).GetComponent<MonsterCombatManager>();
                            if (temp.Position == enemyGrid.selectedX + (enemyGrid.selectedY * enemyGrid.Width))
                            {
                                temp.TakeDamge(dropZone.card.info.atkPower + player.bonusAtk);

                                if (temp.IsDead)
                                {
                                    enemySpawner.DespawnMonster(temp.fieldIndex);
                                    temp.OnDeath();
                                    playerGrid.UpdateGridOnCharacterDeath();
                                }
                                break;
                            }
                        }
                    }
                }
                else // AOE attack
                {
                    foreach(GridTile tile in enemyGrid.grid)
                    {
                        if(tile.IsSelectable && tile.IsOccupied)
                        {
                            for (int i = 1; i < enemySpawner.transform.childCount; i++)
                            {
                                MonsterCombatManager temp = enemySpawner.transform.GetChild(i).GetComponent<MonsterCombatManager>();
                                if (temp.Position == tile.TileIndex)
                                {
                                    temp.TakeDamge(dropZone.card.info.atkPower + player.bonusAtk);

                                    if (temp.IsDead)
                                    {
                                        enemySpawner.DespawnMonster(temp.fieldIndex);
                                        temp.OnDeath();
                                        playerGrid.UpdateGridOnCharacterDeath();
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            // Handle movement
            if (playerGrid.hasSelectedTile)
            {
                int newPosition = playerGrid.selectedX + playerGrid.selectedY * playerGrid.Width;
                player.MoveTo(newPosition);
            }

            // Use mana
            player.UseMana(dropZone.card.info.cost);

            // Refresh tile and grid + remove card from hand + add to discard pile
            dropZone.ReturnCardToHand();
            board.Discard(handIndex);
        }
    }

    public IEnumerator EnableEndButton(float waitDuration)
    {
        yield return new WaitForSeconds(waitDuration);
        endturnButton.interactable = true;
    }

}
