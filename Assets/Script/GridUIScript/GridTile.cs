using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [HideInInspector] public string tileName;
    [HideInInspector] public int tileIndex;
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] protected Image tileImage;
    [SerializeField] private GameObject selectHighlight;
    [SerializeField] private GameObject targetHighlight;
    [SerializeField] private GameObject fragileHighlight;
    [SerializeField] private GameObject anchorHighlight;
    [SerializeField] private GameObject darkenHighlight;
    [SerializeField] private GameObject blockHighlight;
    public GameObject pickedHighlight;
    private int damageIncoming = 0;
    private int isTargeted = 0;
    private bool isWalkable = true;
    private bool isOccupied;
    private bool isFragile;
    private bool isAnchor;
    private bool isSelectable;
    private bool isSelected;

    public bool Walkable { get => isWalkable; set => isWalkable = value; }
    public bool Occupied { get => isOccupied; set => isOccupied = value; }
    public bool Fragile { get => isFragile; set => isFragile = value; }
    public bool Anchor { get => isAnchor; set => isAnchor = value; }
    public int DamageIncoming { get => damageIncoming; set => damageIncoming = value; }
    public int Targeted { get => isTargeted; set => isTargeted = value; }
    public bool Selectable { get => isSelectable; set => isSelectable = value; }
    public bool Selected { get => isSelected; set => isSelected = value; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ActivateSelectHighlight(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ActivateSelectHighlight(false);
    }

    public void ActivateSelectHighlight(bool isHighlight)
    {
        selectHighlight.SetActive(isHighlight && (!CardMouseEvent.isDropped || isSelectable));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isSelectable)
        {
            isSelected = true;
            transform.parent.GetComponent<GridMap>().TrackSelectedTile();
            CombatManager.Instance.UseCard();
        }
    }

    public void UpdateTileOnDamageCal()
    {
        fragileHighlight.SetActive(isFragile);
        blockHighlight.SetActive(!isWalkable);
    }

    public void UpdateTileOnNewTurn()
    {
        blockHighlight.SetActive(!isWalkable);
        targetHighlight.SetActive(isTargeted > 0);
    }

    public void UpdateTileOnCharacterDeath()
    {
        targetHighlight.SetActive(isTargeted > 0);
    }

    public void UpdateTileOnCardPlay(bool enable)
    {
        darkenHighlight.SetActive(!isSelectable && enable);
    }

    public void UpdateOnSetAnchor()
    {
        anchorHighlight.SetActive(isAnchor);
    }

    public void ColorTile(int x, int y)
    {
        if ((x + y) % 2 == 0) tileImage.color = baseColor;
        else tileImage.color = offsetColor;
    }
}
