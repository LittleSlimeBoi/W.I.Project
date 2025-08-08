using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCombatManager : MonoBehaviour
{
    public StatBar hpBar;
    public int maxHP;
    public int hp;
    public int bonusAtk = 0;
    private float runAnimDur = 0.5f;
    private float hurtAnimDur = 0.2f;
    public Image characterDes;
    public GridMap mySide;
    public GridMap otherSide;
    private Vector3 offset;

    [SerializeField] protected GameObject myRenderer;
    public int Position { get; set; }

    protected virtual void Start()
    {
        float offsetX = (mySide.Width % 2 == 0) ? mySide.TileSize / 2 : 0;
        float offsetY = (mySide.Height % 2 == 0) ? mySide.TileSize / 2 : 0;
        offset = new Vector3(offsetX, offsetY, 0);

        DropZone.OnCardDrop += UpdateRaycastOnCardDrop;
    }

    private void OnDestroy()
    {
        DropZone.OnCardDrop -= UpdateRaycastOnCardDrop;
    }

    private void UpdateRaycastOnCardDrop()
    {
        gameObject.GetComponent<Image>().raycastTarget = !CardMouseEvent.isDropped;
    }

    public void SetGrid(GridSide side)
    {
        if (side == GridSide.Enemy)
        {
            mySide = transform.root.Find("Grid Area/Enemy Side/Enemy Grid").GetComponent<GridMap>();
            otherSide = transform.root.Find("Grid Area/Player Side/Player Grid").GetComponent<GridMap>();
        }
        else
        {
            otherSide = transform.root.Find("Grid Area/Enemy Side/Enemy Grid").GetComponent<GridMap>();
            mySide = transform.root.Find("Grid Area/Player Side/Player Grid").GetComponent<GridMap>();
        }
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
        if (damage > 0) StartCoroutine(HurtAnimation(hurtAnimDur));
    }

    
    public int GetPosX()
    {
        return Position % mySide.Width;
    }
    public int GetPosY()
    {
        return Position / mySide.Width;
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
        mySide.grid[x, y].IsOccupied = false;
    }
    public void OccupyAt(int newPosition, bool spawning = false)
    {
        Position = newPosition;
        int x = GetPosX() - (mySide.Width / 2);
        int y = (mySide.Height / 2) - GetPosY();
        mySide.grid[GetPosX(), GetPosY()].IsOccupied = true;

        Vector3 move = new Vector3(x * mySide.TileSize, y * mySide.TileSize) + offset;
        StartCoroutine(RunAnimation(move, runAnimDur, spawning));
    }

    public IEnumerator RunAnimation(Vector3 endPos, float runAnimDur, bool spawning = false)
    {
        myRenderer.GetComponent<Animator>().SetBool("Idle", false);
        float tElapsed = 0;
        float p;
        Vector3 startPos = transform.localPosition;
        transform.localPosition = endPos;
        while (tElapsed < runAnimDur)
        {
            tElapsed += Time.deltaTime;
            p = tElapsed / runAnimDur;
            Vector3 move = Vector3.Lerp(startPos, endPos, p);
            myRenderer.transform.localPosition = move;
            if (!spawning) yield return null;
        }
        myRenderer.GetComponent<Animator>().SetBool("Idle", true);
    }
    
    public IEnumerator HurtAnimation(float hurtAnimDur)
    {
        myRenderer.GetComponent<Animator>().SetBool("Hurt", true);
        float tElapsed = 0;
        float p;
        Vector3 A = transform.localPosition - new Vector3(mySide.TileSize / 10, 0);
        Vector3 B = transform.localPosition + new Vector3(mySide.TileSize / 10, 0);
        Vector3 startPos = transform.localPosition;
        while (tElapsed < hurtAnimDur)
        {
            tElapsed += Time.deltaTime;
            if(tElapsed < 0.6f)
            {
                p = 0.5f + Mathf.Sin(Mathf.PI / 2 * tElapsed / 0.1f) / 2;
                Vector3 move = Vector3.Lerp(A, B, p);
                myRenderer.transform.localPosition = move;
                yield return null;
            }
            yield return null;
        }
        transform.localPosition = startPos;
        myRenderer.GetComponent<Animator>().SetBool("Hurt", false);
    }

    public bool IsDead => hp <= 0;

    public virtual int GetMaxStat(StatBar.StatType statType)
    {
        return maxHP;
    }

    public virtual int GetCurrentStat(StatBar.StatType statType)
    {
        return hp;
    }

}
