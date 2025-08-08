using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridMap : MonoBehaviour
{
    public GridTile[,] grid;
    [SerializeField] private GridTile tile;
    public GridSide side;
    [SerializeField] private GridLayoutGroup layoutGroup;
    [HideInInspector] public int anchorPosX, anchorPosY;
    public int selectableTileCount = 0;
    public int selectedX = 0, selectedY = 0;
    public bool hasSelectedTile;

    public int Width { get; private set; }
    public int Height { get; private set; }
    public float TileSize { get; private set; } = 2;

    private void Start()
    {
        GenerateGrid(5, 5);
        //Debug.Log(Width);
    }

    public void GenerateGrid(int width, int height)
    {
        if (width == 0 || height == 0)
        {
            width = 5;
            height = 5;
            Debug.Log("This room don't have width or height");
        }
        Width = width;
        Height = height;
        grid = new GridTile[width, height];
        GetComponent<RectTransform>().sizeDelta = new Vector2(width * TileSize, height * TileSize);
        Vector2 tile2d = new Vector2(TileSize, TileSize);
        layoutGroup.cellSize = tile2d;

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                grid[i, j] = Instantiate(tile, transform);
                grid[i, j].ColorTile(i, j);
                grid[i, j].name = $"{side}-GridTile {i} {j}";
                grid[i, j].TileIndex = (j * height) + i;
                grid[i, j].SetSize(tile2d);
                grid[i, j].transform.localScale = Vector2.one;
            }
        }

        SetAnchorTile((width - 1) / 2, (height - 1) / 2);
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

    public bool IsValidX(int x)
    {
        return (x >= 0) && (x < Width);
    }
    public bool IsValidY(int y)
    {
        return (y >= 0) && (y < Height);
    }
    public bool IsValidPosition(int x, int y)
    {
        return IsValidX(x) && IsValidY(y);
    }

    public int GetRandomPos()
    {
        List<Vector2Int> temp = new();
        for (int j = 0; j < Height; j++)
        {
            for (int i = 0; i < Width; i++)
            {
                if (!grid[i, j].IsOccupied) temp.Add(new Vector2Int(i, j));
            }
        }
        int rng = Random.Range(0, temp.Count - 1);
        Vector2Int pair = temp[rng];
        return (pair.x + pair.y * Width);
    }

    public void SetAnchorTile(int x, int y)
    {
        grid[anchorPosX, anchorPosY].IsAnchor = false;
        grid[anchorPosX, anchorPosY].UpdateOnSetAnchor();
        anchorPosX = x;
        anchorPosY = y;
        grid[anchorPosX, anchorPosY].IsAnchor = true;
        grid[anchorPosX, anchorPosY].UpdateOnSetAnchor();
    }

    public bool IsFinish()
    {
        return selectableTileCount == 0 || hasSelectedTile;
    }

    public void TrackSelectedTile()
    {
        // Unmark old tile
        grid[selectedX, selectedY].IsSelected = false;
        grid[selectedX, selectedY].pickedHighlight.SetActive(false);
        // Search for new tile
        // Note: foreach in 2d array is fucky-wucky
        int i = 0;
        foreach (GridTile tile in grid)
        {
            if (!tile.IsSelected)
            {
                i++;
            }
            else if(i < Width * Height)
            {
                selectedX = i / Height;
                selectedY = i % Height;
                break;
            }
            else
            {
                grid[selectedX, selectedY].IsSelected = true;
                break;
            }
        }
        grid[selectedX, selectedY].pickedHighlight.SetActive(true);
        hasSelectedTile = true;
    }

    public void CancelSelectedTile()
    {
        grid[selectedX, selectedY].IsSelected = false;
        grid[selectedX, selectedY].pickedHighlight.SetActive(false);

        hasSelectedTile = false;
    }
}

public enum GridSide
{
    Player,
    Enemy
}