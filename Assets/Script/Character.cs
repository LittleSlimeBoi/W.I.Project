using System.Collections;
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

        DropZone.OnCardDrop += UpdateRaycastOnCardDrop;
    }

    private void OnDestroy()
    {
        DropZone.OnCardDrop -= UpdateRaycastOnCardDrop;
    }

    private void UpdateRaycastOnCardDrop()
    {
        characterIcon.raycastTarget = !CardMouseEvent.isDropped;
    }

    public void SetGrid()
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
        if (damage > 0) StartCoroutine(HurtAnimation());
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
    public void OccupyAt(int newPosition, bool spawn = false)
    {
        position = newPosition;
        int x = GetPosX() - (mySide.Width / 2);
        int y = (mySide.Height / 2) - GetPosY();
        mySide.grid[GetPosX(), GetPosY()].Occupied = true;

        Vector3 move = new Vector3(x*100, y*100) + offset;
        StartCoroutine(RunAnimation(move, spawn));
    }

    public IEnumerator RunAnimation(Vector3 endPos, bool spawn = false)
    {
        animator.SetBool("Idle", false);
        float duration = 2f;
        float tElapsed = 0;
        float p;
        Vector3 startPos = transform.localPosition;
        while (tElapsed < duration)
        {
            tElapsed += Time.deltaTime;
            p = tElapsed / duration;
            Vector3 move = Vector3.Lerp(startPos, endPos, p);
            transform.SetLocalPositionAndRotation(move, Quaternion.identity);
            if (!spawn) yield return null;
        }
        animator.SetBool("Idle", true);
    }
    
    public IEnumerator HurtAnimation()
    {
        animator.SetBool("Hurt", true);
        float duration = 1f;
        float tElapsed = 0;
        float p;
        Vector3 A = transform.localPosition - new Vector3(10, 0);
        Vector3 B = transform.localPosition + new Vector3(10, 0);
        Vector3 startPos = transform.localPosition;
        while (tElapsed < duration)
        {
            tElapsed += Time.deltaTime;
            if(tElapsed < 0.6f)
            {
                p = 0.5f + Mathf.Sin(Mathf.PI / 2 * tElapsed / 0.1f) / 2;
                Vector3 move = Vector3.Lerp(A, B, p);
                transform.SetLocalPositionAndRotation(move, Quaternion.identity);
                yield return null;
            }
            yield return null;
        }
        transform.SetLocalPositionAndRotation(startPos, Quaternion.identity);
        animator.SetBool("Hurt", false);
    }

    public bool IsDead => hp <= 0;

    public virtual int GetMaxStat(StatBar.StatType statType)
    {
        return hp;
    }

    public virtual int GetCurrentStat(StatBar.StatType statType)
    {
        return hp;
    }

}
