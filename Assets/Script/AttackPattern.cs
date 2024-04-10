using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class AttackPattern
{
    public static void CellAtack(int x, int y, GridMap map, int damage)
    {
        if (map.IsValidPosition(x, y))
        {
            map.grid[x, y].Targeted++;
            map.grid[x, y].DamageIncoming += damage;
        }
    }
    public static void CancelCellAtack(int x, int y, GridMap map, int damage)
    {
        if (map.IsValidPosition(x, y))
        {
            map.grid[x, y].Targeted--;
            map.grid[x, y].DamageIncoming -= damage;
        }
    }

    /* Direction:       
     *       2                 1     2
     *      /\                 \\   //
     * 1 <- || -> 3           
     *      \/                 //   \\
     *       4                 4     3
     */
    public static void TriangleAtack(int x, int y, GridMap map, int damage, int direction)
    {
        CellAtack(x, y, map, damage);
        if (direction != 1) CellAtack(x + 1, y, map, damage);
        if (direction != 2) CellAtack(x, y + 1, map, damage);
        if (direction != 3) CellAtack(x - 1, y, map, damage);
        if (direction != 4) CellAtack(x, y - 1, map, damage);
    }
    public static void LineAtack(int x, int y, GridMap map, int damage, int direction, int length)
    {
        switch (direction)
        {
            case 1: 
                for(int i = 0; i < length; i++)
                {
                    CellAtack(x - i, y, map, damage);
                }
                break;
            case 2:
                for (int i = 0; i < length; i++)
                {
                    CellAtack(x, y - i, map, damage);
                }
                break;
            case 3:
                for (int i = 0; i < length; i++)
                {
                    CellAtack(x + i, y, map, damage);
                }
                break;
            case 4:
                for (int i = 0; i < length; i++)
                {
                    CellAtack(x, y + i, map, damage);
                }
                break;
        }
    }
    public static void DiagonalAtack(int x, int y, GridMap map, int damage, int direction, int length)
    {
        switch (direction)
        {
            case 1:
                for (int i = 0; i < length; i++)
                {
                    CellAtack(x - i, y - i, map, damage);
                }
                break;
            case 2:
                for (int i = 0; i < length; i++)
                {
                    CellAtack(x + i, y - i, map, damage);
                }
                break;
            case 3:
                for (int i = 0; i < length; i++)
                {
                    CellAtack(x + i, y + i, map, damage);
                }
                break;
            case 4:
                for (int i = 0; i < length; i++)
                {
                    CellAtack(x - i, y + i, map, damage);
                }
                break;
        }
    }
    public static void PlusAtack(int x, int y, GridMap map, int damage, int length)
    {
        CellAtack(x, y, map, damage);
        for(int i = 1; i <= length; i++)
        {
            CellAtack(x + i, y, map, damage);
            CellAtack(x, y + i, map, damage);
            CellAtack(x - i, y, map, damage);
            CellAtack(x, y - i, map, damage);
        }
    }
    public static void CrossAtack(int x, int y, GridMap map, int damage, int length)
    {
        CellAtack(x, y, map, damage);
        for (int i = 1; i <= length; i++)
        {
            CellAtack(x + i, y + i, map, damage);
            CellAtack(x - i, y + i, map, damage);
            CellAtack(x - i, y - i, map, damage);
            CellAtack(x + i, y - i, map, damage);
        }
    }
    public static void RectAtack(int x, int y, GridMap map, int damage, int direction, int width, int height)
    {
        switch (direction)
        {
            case 1:
                for (int i = 0; i < height; i++)
                {
                    LineAtack(x, y - i, map, damage, 1, width);
                }
                break;
            case 2:
                for (int i = 0; i < height; i++)
                {
                    LineAtack(x, y - i, map, damage, 3, width);
                }
                break;
            case 3:
                for (int i = 0; i < height; i++)
                {
                    LineAtack(x, y + i, map, damage, 3, width);
                }
                break;
            case 4:
                for (int i = 0; i < height; i++)
                {
                    LineAtack(x, y + i, map, damage, 1, width);
                }
                break;
        }
    }
    public static void CircleAtack(int x, int y, GridMap map, int damage, int direction, int length)
    {
        switch (direction)
        {
            case 1:
                LineAtack(x - 1, y, map, damage, 1, length - 1);
                LineAtack(x, y - 1, map, damage, 2, length - 1);
                LineAtack(x - length, y - 1, map, damage, 2, length - 1);
                LineAtack(x - 1, y - length, map, damage, 1, length - 1);
                break;
            case 2:
                LineAtack(x + 1, y, map, damage, 3, length - 1);
                LineAtack(x, y - 1, map, damage, 2, length - 1);
                LineAtack(x + length, y - 1, map, damage, 2, length - 1);
                LineAtack(x + 1, y - length, map, damage, 3, length - 1);
                break;
            case 3:
                LineAtack(x + 1, y, map, damage, 3, length - 1);
                LineAtack(x, y + 1, map, damage, 4, length - 1);
                LineAtack(x + length, y + 1, map, damage, 4, length - 1);
                LineAtack(x + 1, y + length, map, damage, 3, length - 1);
                break;
            case 4:
                LineAtack(x - 1, y, map, damage, 1, length - 1);
                LineAtack(x, y + 1, map, damage, 4, length - 1);
                LineAtack(x - length, y + 1, map, damage, 4, length - 1);
                LineAtack(x - 1, y + length, map, damage, 1, length - 1);
                break;
        }
    }

    public static void CancelTriangleAtack(int x, int y, GridMap map, int damage, int direction)
    {
        CancelCellAtack(x, y, map, damage);
        if (direction != 1) CancelCellAtack(x + 1, y, map, damage);
        if (direction != 2) CancelCellAtack(x, y + 1, map, damage);
        if (direction != 3) CancelCellAtack(x - 1, y, map, damage);
        if (direction != 4) CancelCellAtack(x, y - 1, map, damage);
    }
    public static void CancelLineAtack(int x, int y, GridMap map, int damage, int direction, int length)
    {
        switch (direction)
        {
            case 1:
                for (int i = 0; i < length; i++)
                {
                    CancelCellAtack(x - i, y, map, damage);
                }
                break;
            case 2:
                for (int i = 0; i < length; i++)
                {
                    CancelCellAtack(x, y - i, map, damage);
                }
                break;
            case 3:
                for (int i = 0; i < length; i++)
                {
                    CancelCellAtack(x + i, y, map, damage);
                }
                break;
            case 4:
                for (int i = 0; i < length; i++)
                {
                    CancelCellAtack(x, y + i, map, damage);
                }
                break;
        }
    }
    public static void CancelDiagonalAtack(int x, int y, GridMap map, int damage, int direction, int length)
    {
        switch (direction)
        {
            case 1:
                for (int i = 0; i < length; i++)
                {
                    CancelCellAtack(x - i, y - i, map, damage);
                }
                break;
            case 2:
                for (int i = 0; i < length; i++)
                {
                    CancelCellAtack(x + i, y - i, map, damage);
                }
                break;
            case 3:
                for (int i = 0; i < length; i++)
                {
                    CancelCellAtack(x + i, y + i, map, damage);
                }
                break;
            case 4:
                for (int i = 0; i < length; i++)
                {
                    CancelCellAtack(x - i, y + i, map, damage);
                }
                break;
        }
    }
    public static void CancelPlusAtack(int x, int y, GridMap map, int damage, int length)
    {
        CancelCellAtack(x, y, map, damage);
        for (int i = 1; i <= length; i++)
        {
            CancelCellAtack(x + i, y, map, damage);
            CancelCellAtack(x, y + i, map, damage);
            CancelCellAtack(x - i, y, map, damage);
            CancelCellAtack(x, y - i, map, damage);
        }
    }
    public static void CancelCrossAtack(int x, int y, GridMap map, int damage, int length)
    {
        CancelCellAtack(x, y, map, damage);
        for (int i = 1; i <= length; i++)
        {
            CancelCellAtack(x + i, y + i, map, damage);
            CancelCellAtack(x - i, y + i, map, damage);
            CancelCellAtack(x - i, y - i, map, damage);
            CancelCellAtack(x + i, y - i, map, damage);
        }
    }
    public static void CancelRectAtack(int x, int y, GridMap map, int damage, int direction, int width, int height)
    {
        switch (direction)
        {
            case 1:
                for (int i = 0; i < height; i++)
                {
                    CancelLineAtack(x, y - i, map, damage, 1, width);
                }
                break;
            case 2:
                for (int i = 0; i < height; i++)
                {
                    CancelLineAtack(x, y - i, map, damage, 3, width);
                }
                break;
            case 3:
                for (int i = 0; i < height; i++)
                {
                    CancelLineAtack(x, y + i, map, damage, 3, width);
                }
                break;
            case 4:
                for (int i = 0; i < height; i++)
                {
                    CancelLineAtack(x, y + i, map, damage, 1, width);
                }
                break;
        }
    }
    public static void CancelCircleAtack(int x, int y, GridMap map, int damage, int direction, int length)
    {
        switch (direction)
        {
            case 1:
                CancelLineAtack(x - 1, y, map, damage, 1, length - 1);
                CancelLineAtack(x, y - 1, map, damage, 2, length - 1);
                CancelLineAtack(x - length, y - 1, map, damage, 2, length - 1);
                CancelLineAtack(x - 1, y - length, map, damage, 1, length - 1);
                break;
            case 2:
                CancelLineAtack(x + 1, y, map, damage, 3, length - 1);
                CancelLineAtack(x, y - 1, map, damage, 2, length - 1);
                CancelLineAtack(x + length, y - 1, map, damage, 2, length - 1);
                CancelLineAtack(x + 1, y - length, map, damage, 3, length - 1);
                break;
            case 3:
                CancelLineAtack(x + 1, y, map, damage, 3, length - 1);
                CancelLineAtack(x, y + 1, map, damage, 4, length - 1);
                CancelLineAtack(x + length, y + 1, map, damage, 4, length - 1);
                CancelLineAtack(x + 1, y + length, map, damage, 3, length - 1);
                break;
            case 4:
                CancelLineAtack(x - 1, y, map, damage, 1, length - 1);
                CancelLineAtack(x, y + 1, map, damage, 4, length - 1);
                CancelLineAtack(x - length, y + 1, map, damage, 4, length - 1);
                CancelLineAtack(x - 1, y + length, map, damage, 1, length - 1);
                break;
        }
    }

}
