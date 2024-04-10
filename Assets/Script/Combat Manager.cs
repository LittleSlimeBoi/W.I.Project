using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    public static int turn = 1;
    public Board board;
    public HandPanel handPanel;
    public DropZone dropZone;
    public CancelPanel cancelPanel;
    public StatBar hpBar;
    public StatBar manaBar;
    public Player player;
    public SpawnArea enemySpawner;
    public GridMap playerGrid;
    public GridMap enemyGrid;

    public LevelLoader loader;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        board.InitBoard();
        handPanel.FlushHand();

        board.DrawCard(5);
        for (int i = 0; i < board.hand.Count; i++)
        {
            handPanel.Display(i);
        }
    }

    public void OnEndTurn()
    {
        // Clear hand
        board.Flush();
        handPanel.FlushHand();

        // Player take damage
        player.TakeDamge(playerGrid.grid[player.GetPosX(), player.GetPosY()].DamageIncoming);
        if (player.IsDead)
        {
            // Load lose screen
            loader.LoadEndScreen();
        }

        // Refresh player grid
        foreach(GridTile tile in playerGrid.grid)
        {
            tile.DamageIncoming = 0;
            tile.Targeted = 0;
            tile.UpdateTileOnNewTurn();
        }

        if(enemySpawner.NoMoreMonster() && !enemySpawner.WaitingToSpawn())
        {
            // Load win scene
            loader.LoadEndScreen();
        }
        else
        {
            NewTurn();
        }
    }

    public void NewTurn()
    {
        turn++;
        // Refill player's mana
        player.RefillMana();
        // Draw 5 cards
        board.DrawCard(5);
        for (int i = 0; i < board.hand.Count; i++)
        {
            handPanel.Display(i);
        }
        // Monster turn
        if(enemySpawner.NoMoreMonster() && enemySpawner.WaitingToSpawn())
        {
            enemySpawner.SpawnMonster(4);
        }
        enemySpawner.AllMonstersAction();
        // Update tile
        playerGrid.UpdateGridOnNewTurn();
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
                    if (enemyGrid.grid[enemyGrid.selectedX, enemyGrid.selectedY].Occupied)
                    {
                        for(int i = 1; i < enemySpawner.transform.childCount; i++)
                        {
                            Monster temp = enemySpawner.transform.GetChild(i).GetComponent<Monster>();
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
                    int gridIndex = 0;
                    foreach(GridTile tile in enemyGrid.grid)
                    {
                        if(tile.Selectable && tile.Occupied)
                        {
                            for (int i = 1; i < enemySpawner.transform.childCount; i++)
                            {
                                Monster temp = enemySpawner.transform.GetChild(i).GetComponent<Monster>();
                                if (temp.Position == (gridIndex / enemyGrid.Height) + (gridIndex % enemyGrid.Height * enemyGrid.Width))
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
                        gridIndex++;
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
            dropZone.Cancel();
            handPanel.Play(handIndex);
            board.Discard(handIndex);
        }
    }
}
