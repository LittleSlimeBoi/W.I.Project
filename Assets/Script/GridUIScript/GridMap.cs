using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridMap: MonoBehaviour
{
    public GridTile[,] grid;
    [SerializeField] private int width, height;
    [SerializeField] private GridTile tile;
    [SerializeField] private string s;
    [HideInInspector] public int anchorPosX, anchorPosY;
    public int selectableTileCount = 0;
    public int selectedX = 0, selectedY = 0;
    public bool hasSelectedTile;

    public int Width { get => width; set => width = value; }
    public int Height { get => height; set => height = value; }

    private void Start()
    {
        GenerateGrid(width, height);
        SetAnchorTile((width - 1) / 2, (height - 1) / 2);
    }

    public void GenerateGrid(int width, int height)
    {
        grid = new GridTile[width, height];
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(width*100, height*100);

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                grid[i, j] = Instantiate(tile);
                grid[i, j].gameObject.transform.SetParent(this.transform);
                grid[i, j].ColorTile(i, j);
                grid[i, j].name = $"{s}-GridTile {i} {j}";
            }
        }
    }

    public void UpdateGridOnCardPlay(bool enable)
    {
        foreach(GridTile tile in grid)
        {
            tile.UpdateTileOnCardPlay(enable);
        }
    }

    public void UpdateGridOnNewTurn()
    {
        foreach (GridTile tile in grid)
        {
            tile.UpdateTileOnNewTurn();
        }
    }

    public void UpdateGridOnDamageCal()
    {
        foreach (GridTile tile in grid)
        {
            tile.UpdateTileOnDamageCal();
        }
    }

    public void UpdateGridOnCharacterDeath()
    {
        foreach (GridTile tile in grid)
        {
            tile.UpdateTileOnCharacterDeath();
        }
    }

    public bool IsValidPosition(int x, int y)
    {
        return (x >= 0) && (x < width) && (y >= 0) && (y < height);
    }

    public int GetRandomPos()
    {
        List<Tuple<int, int>> temp = new();
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                if (!grid[i, j].Occupied) temp.Add(new Tuple<int, int>(i, j));
            }
        }
        int rng = UnityEngine.Random.Range(0, temp.Count - 1);
        Tuple<int, int> pair = temp[rng];
        return (pair.Item1 + pair.Item2 * width);
    }

    public void SetAnchorTile(int x, int y)
    {
        grid[anchorPosX, anchorPosY].Anchor = false;
        grid[anchorPosX, anchorPosY].UpdateOnSetAnchor();
        anchorPosX = x;
        anchorPosY = y;
        grid[anchorPosX, anchorPosY].Anchor = true;
        grid[anchorPosX, anchorPosY].UpdateOnSetAnchor();
    }

    public bool IsFinish()
    {
        return selectableTileCount == 0 || hasSelectedTile;
    }

    public void TrackSelectedTile()
    {
        // Unmark old tile
        grid[selectedX, selectedY].Selected = false;
        grid[selectedX, selectedY].pickedHighlight.SetActive(false);
        // Search for new tile
        // Note: foreach in 2d array is fucky-wucky
        int i = 0;
        foreach (GridTile tile in grid)
        {
            if (!tile.Selected)
            {
                i++;
            }
            else if(i < width * height)
            {
                selectedX = i / height;
                selectedY = i % height;
                break;
            }
            else
            {
                grid[selectedX, selectedY].Selected = true;
                break;
            }
        }
        grid[selectedX, selectedY].pickedHighlight.SetActive(true);
        hasSelectedTile = true;
    }

    public void CancelSelectedTile()
    {
        grid[selectedX, selectedY].Selected = false;
        grid[selectedX, selectedY].pickedHighlight.SetActive(false);

        hasSelectedTile = false;
    }
}