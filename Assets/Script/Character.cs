using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public StatBar hpBar;
    public int hp;
    public int bonusAtk = 0;
    public int position;
    public Image characterIcon;
    public Image characterDes;
    public GridMap mySide;
    public GridMap otherSide;
    private Vector3 offset;

    public Animator animator;

    protected virtual void Start()
    {
        //setGrid();
        int offsetX = (mySide.Width % 2 == 0) ? 50 : 0;
        int offsetY = (mySide.Height % 2 == 0) ? 50 : 0;
        offset = new Vector3(offsetX, offsetY, 0);
    }
    
    private void Update()
    {
        characterIcon.raycastTarget = !CardMouseEvent.isDropped;
    }

    public void setGrid()
    {
        mySide = transform.root.Find("Grid Area/Enemy Side/Enemy Grid").GetComponent<GridMap>();
        otherSide = transform.root.Find("Grid Area/Player Side/Player Grid").GetComponent<GridMap>();
    }

    // Unmark old tile + mark new tile + tranform
    public void MoveTo(int newPosition)
    {
        UnOccupy();
        OccupyAt(newPosition);
    }

    public void TakeDamge(int damage)
    {
        hp -= damage;
        hpBar.UpdateStatBar();
    }

    public int Position { get => position; set => position = value; }
    public int GetPosX()
    {
        return position % mySide.Width;
    }
    public int GetPosY()
    {
        return position / mySide.Width;
    }
    public int TranslatePosX()
    {
        return GetPosX() - mySide.anchorPosX + otherSide.anchorPosX;
    }
    public int TranslatePosY()
    {
        return GetPosY() - mySide.anchorPosY + otherSide.anchorPosY;
    }
    public void UnOccupy()
    {
        int x = GetPosX();
        int y = GetPosY();
        mySide.grid[x, y].Occupied = false;
    }
    public void OccupyAt(int newPosition)
    {
        position = newPosition;
        int x = GetPosX() - (mySide.Width / 2);
        int y = (mySide.Height / 2) - GetPosY();
        mySide.grid[GetPosX(), GetPosY()].Occupied = true;

        Vector3 move = new Vector3(x*100, y*100) + offset;
        transform.SetLocalPositionAndRotation(move, Quaternion.identity);
    }

    public bool IsDead => hp <= 0;

    public virtual int GetMaxStat(string statName)
    {
        return hp;
    }

    public virtual int GetCurrentStat(string statName)
    {
        return hp;
    }

}
