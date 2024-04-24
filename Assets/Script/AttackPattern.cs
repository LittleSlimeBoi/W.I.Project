using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class AttackPattern
{
    public static void CellAtack(int x, int y, Monster mon, int damage)
    {
        if (mon.otherSide.IsValidPosition(x, y))
        {
            mon.otherSide.grid[x, y].Targeted++;
            mon.otherSide.grid[x, y].DamageIncoming += damage;
            mon.atkArea.Add(mon.otherSide.grid[x, y]);
        }
    }
    public static void CancelCellAtack(int x, int y, Monster mon, int damage)
    {
        if (mon.otherSide.IsValidPosition(x, y))
        {
            mon.otherSide.grid[x, y].Targeted--;
            mon.otherSide.grid[x, y].DamageIncoming -= damage;
            mon.atkArea.Remove(mon.otherSide.grid[x, y]);
        }
    }

    /* Direction:       
     *       2                 1     2
     *      /\                 \\   //
     * 1 <- || -> 3           
     *      \/                 //   \\
     *       4                 4     3
     */
    public static void TriangleAtack(int x, int y, Monster mon, int damage, int direction)
    {
        CellAtack(x, y, mon, damage);
        if (direction != 1) CellAtack(x + 1, y, mon, damage);
        if (direction != 2) CellAtack(x, y + 1, mon, damage);
        if (direction != 3) CellAtack(x - 1, y, mon, damage);
        if (direction != 4) CellAtack(x, y - 1, mon, damage);
    }
    public static void LineAtack(int x, int y, Monster mon, int damage, int direction, int length)
    {
        switch (direction)
        {
            case 1: 
                for(int i = 0; i < length; i++)
                {
                    CellAtack(x - i, y, mon, damage);
                }
                break;
            case 2:
                for (int i = 0; i < length; i++)
                {
                    CellAtack(x, y - i, mon, damage);
                }
                break;
            case 3:
                for (int i = 0; i < length; i++)
                {
                    CellAtack(x + i, y, mon, damage);
                }
                break;
            case 4:
                for (int i = 0; i < length; i++)
                {
                    CellAtack(x, y + i, mon, damage);
                }
                break;
        }
    }
    public static void DiagonalAtack(int x, int y, Monster mon, int damage, int direction, int length)
    {
        switch (direction)
        {
            case 1:
                for (int i = 0; i < length; i++)
                {
                    CellAtack(x - i, y - i, mon, damage);
                }
                break;
            case 2:
                for (int i = 0; i < length; i++)
                {
                    CellAtack(x + i, y - i, mon, damage);
                }
                break;
            case 3:
                for (int i = 0; i < length; i++)
                {
                    CellAtack(x + i, y + i, mon, damage);
                }
                break;
            case 4:
                for (int i = 0; i < length; i++)
                {
                    CellAtack(x - i, y + i, mon, damage);
                }
                break;
        }
    }
    public static void PlusAtack(int x, int y, Monster mon, int damage, int length)
    {
        CellAtack(x, y, mon, damage);
        for(int i = 1; i <= length; i++)
        {
            CellAtack(x + i, y, mon, damage);
            CellAtack(x, y + i, mon, damage);
            CellAtack(x - i, y, mon, damage);
            CellAtack(x, y - i, mon, damage);
        }
    }
    public static void CrossAtack(int x, int y, Monster mon, int damage, int length)
    {
        CellAtack(x, y, mon, damage);
        for (int i = 1; i <= length; i++)
        {
            CellAtack(x + i, y + i, mon, damage);
            CellAtack(x - i, y + i, mon, damage);
            CellAtack(x - i, y - i, mon, damage);
            CellAtack(x + i, y - i, mon, damage);
        }
    }
    public static void RectAtack(int x, int y, Monster mon, int damage, int direction, int width, int height)
    {
        switch (direction)
        {
            case 1:
                for (int i = 0; i < height; i++)
                {
                    LineAtack(x, y - i, mon, damage, 1, width);
                }
                break;
            case 2:
                for (int i = 0; i < height; i++)
                {
                    LineAtack(x, y - i, mon, damage, 3, width);
                }
                break;
            case 3:
                for (int i = 0; i < height; i++)
                {
                    LineAtack(x, y + i, mon, damage, 3, width);
                }
                break;
            case 4:
                for (int i = 0; i < height; i++)
                {
                    LineAtack(x, y + i, mon, damage, 1, width);
                }
                break;
        }
    }
    public static void CircleAtack(int x, int y, Monster mon, int damage, int direction, int length)
    {
        switch (direction)
        {
            case 1:
                LineAtack(x - 1, y, mon, damage, 1, length - 1);
                LineAtack(x, y - 1, mon, damage, 2, length - 1);
                LineAtack(x - length, y - 1, mon, damage, 2, length - 1);
                LineAtack(x - 1, y - length, mon, damage, 1, length - 1);
                break;
            case 2:
                LineAtack(x + 1, y, mon, damage, 3, length - 1);
                LineAtack(x, y - 1, mon, damage, 2, length - 1);
                LineAtack(x + length, y - 1, mon, damage, 2, length - 1);
                LineAtack(x + 1, y - length, mon, damage, 3, length - 1);
                break;
            case 3:
                LineAtack(x + 1, y, mon, damage, 3, length - 1);
                LineAtack(x, y + 1, mon, damage, 4, length - 1);
                LineAtack(x + length, y + 1, mon, damage, 4, length - 1);
                LineAtack(x + 1, y + length, mon, damage, 3, length - 1);
                break;
            case 4:
                LineAtack(x - 1, y, mon, damage, 1, length - 1);
                LineAtack(x, y + 1, mon, damage, 4, length - 1);
                LineAtack(x - length, y + 1, mon, damage, 4, length - 1);
                LineAtack(x - 1, y + length, mon, damage, 1, length - 1);
                break;
        }
    }

    public static void CancelTriangleAtack(int x, int y, Monster mon, int damage, int direction)
    {
        CancelCellAtack(x, y, mon, damage);
        if (direction != 1) CancelCellAtack(x + 1, y, mon, damage);
        if (direction != 2) CancelCellAtack(x, y + 1, mon, damage);
        if (direction != 3) CancelCellAtack(x - 1, y, mon, damage);
        if (direction != 4) CancelCellAtack(x, y - 1, mon, damage);
    }
    public static void CancelLineAtack(int x, int y, Monster mon, int damage, int direction, int length)
    {
        switch (direction)
        {
            case 1:
                for (int i = 0; i < length; i++)
                {
                    CancelCellAtack(x - i, y, mon, damage);
                }
                break;
            case 2:
                for (int i = 0; i < length; i++)
                {
                    CancelCellAtack(x, y - i, mon, damage);
                }
                break;
            case 3:
                for (int i = 0; i < length; i++)
                {
                    CancelCellAtack(x + i, y, mon, damage);
                }
                break;
            case 4:
                for (int i = 0; i < length; i++)
                {
                    CancelCellAtack(x, y + i, mon, damage);
                }
                break;
        }
    }
    public static void CancelDiagonalAtack(int x, int y, Monster mon, int damage, int direction, int length)
    {
        switch (direction)
        {
            case 1:
                for (int i = 0; i < length; i++)
                {
                    CancelCellAtack(x - i, y - i, mon, damage);
                }
                break;
            case 2:
                for (int i = 0; i < length; i++)
                {
                    CancelCellAtack(x + i, y - i, mon, damage);
                }
                break;
            case 3:
                for (int i = 0; i < length; i++)
                {
                    CancelCellAtack(x + i, y + i, mon, damage);
                }
                break;
            case 4:
                for (int i = 0; i < length; i++)
                {
                    CancelCellAtack(x - i, y + i, mon, damage);
                }
                break;
        }
    }
    public static void CancelPlusAtack(int x, int y, Monster mon, int damage, int length)
    {
        CancelCellAtack(x, y, mon, damage);
        for (int i = 1; i <= length; i++)
        {
            CancelCellAtack(x + i, y, mon, damage);
            CancelCellAtack(x, y + i, mon, damage);
            CancelCellAtack(x - i, y, mon, damage);
            CancelCellAtack(x, y - i, mon, damage);
        }
    }
    public static void CancelCrossAtack(int x, int y, Monster mon, int damage, int length)
    {
        CancelCellAtack(x, y, mon, damage);
        for (int i = 1; i <= length; i++)
        {
            CancelCellAtack(x + i, y + i, mon, damage);
            CancelCellAtack(x - i, y + i, mon, damage);
            CancelCellAtack(x - i, y - i, mon, damage);
            CancelCellAtack(x + i, y - i, mon, damage);
        }
    }
    public static void CancelRectAtack(int x, int y, Monster mon, int damage, int direction, int width, int height)
    {
        switch (direction)
        {
            case 1:
                for (int i = 0; i < height; i++)
                {
                    CancelLineAtack(x, y - i, mon, damage, 1, width);
                }
                break;
            case 2:
                for (int i = 0; i < height; i++)
                {
                    CancelLineAtack(x, y - i, mon, damage, 3, width);
                }
                break;
            case 3:
                for (int i = 0; i < height; i++)
                {
                    CancelLineAtack(x, y + i, mon, damage, 3, width);
                }
                break;
            case 4:
                for (int i = 0; i < height; i++)
                {
                    CancelLineAtack(x, y + i, mon, damage, 1, width);
                }
                break;
        }
    }
    public static void CancelCircleAtack(int x, int y, Monster mon, int damage, int direction, int length)
    {
        switch (direction)
        {
            case 1:
                CancelLineAtack(x - 1, y, mon, damage, 1, length - 1);
                CancelLineAtack(x, y - 1, mon, damage, 2, length - 1);
                CancelLineAtack(x - length, y - 1, mon, damage, 2, length - 1);
                CancelLineAtack(x - 1, y - length, mon, damage, 1, length - 1);
                break;
            case 2:
                CancelLineAtack(x + 1, y, mon, damage, 3, length - 1);
                CancelLineAtack(x, y - 1, mon, damage, 2, length - 1);
                CancelLineAtack(x + length, y - 1, mon, damage, 2, length - 1);
                CancelLineAtack(x + 1, y - length, mon, damage, 3, length - 1);
                break;
            case 3:
                CancelLineAtack(x + 1, y, mon, damage, 3, length - 1);
                CancelLineAtack(x, y + 1, mon, damage, 4, length - 1);
                CancelLineAtack(x + length, y + 1, mon, damage, 4, length - 1);
                CancelLineAtack(x + 1, y + length, mon, damage, 3, length - 1);
                break;
            case 4:
                CancelLineAtack(x - 1, y, mon, damage, 1, length - 1);
                CancelLineAtack(x, y + 1, mon, damage, 4, length - 1);
                CancelLineAtack(x - length, y + 1, mon, damage, 4, length - 1);
                CancelLineAtack(x - 1, y + length, mon, damage, 1, length - 1);
                break;
        }
    }

}
