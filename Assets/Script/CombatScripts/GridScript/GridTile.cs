using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [HideInInspector] public string tileName;
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] protected Image tileImage;
    [SerializeField] private GameObject selectHighlight;
    [SerializeField] private GameObject targetHighlight;
    [SerializeField] private GameObject fragileHighlight;
    [SerializeField] private GameObject anchorHighlight;
    [SerializeField] private GameObject darkenHighlight;
    [SerializeField] private GameObject blockHighlight;
    public GameObject pickedHighlight;

    public int TileIndex { get; set; }
    public bool IsWalkable { get; set; } = true;
    public bool IsOccupied { get; set; }
    public bool IsFragile { get; set; }
    public bool IsAnchor { get; set; }
    public int DamageIncoming { get; set; } = 0;
    public int Targeted { get; set; } = 0;
    public bool IsSelectable { get; set; }
    public bool IsSelected { get; set; }

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
        selectHighlight.SetActive(isHighlight && (!CardMouseEvent.isDropped || IsSelectable));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsSelectable)
        {
            IsSelected = true;
            transform.parent.GetComponent<GridMap>().TrackSelectedTile();
            CombatManager.Instance.UseCard();
        }
    }

    public void UpdateTileOnDamageCal()
    {
        fragileHighlight.SetActive(IsFragile);
        blockHighlight.SetActive(!IsWalkable);
    }

    public void UpdateTileOnNewTurn()
    {
        blockHighlight.SetActive(!IsWalkable);
        targetHighlight.SetActive(Targeted > 0);
    }

    public void UpdateTileOnCharacterDeath()
    {
        targetHighlight.SetActive(Targeted > 0);
    }

    public void UpdateTileOnCardPlay(bool enable)
    {
        darkenHighlight.SetActive(!IsSelectable && enable);
    }

    public void UpdateOnSetAnchor()
    {
        anchorHighlight.SetActive(IsAnchor);
    }

    public void ColorTile(int x, int y)
    {
        if ((x + y) % 2 == 0) tileImage.color = baseColor;
        else tileImage.color = offsetColor;
    }

    public void SetSize(Vector2 tileSize)
    {
        GetComponent<RectTransform>().sizeDelta = tileSize;
        selectHighlight.GetComponent<RectTransform>().sizeDelta  = tileSize;
        targetHighlight.GetComponent<RectTransform>().sizeDelta  = tileSize;
        fragileHighlight.GetComponent<RectTransform>().sizeDelta = tileSize;
        anchorHighlight.GetComponent<RectTransform>().sizeDelta  = tileSize;
        darkenHighlight.GetComponent<RectTransform>().sizeDelta  = tileSize;
        blockHighlight.GetComponent<RectTransform>().sizeDelta   = tileSize;
        pickedHighlight.GetComponent<RectTransform>().sizeDelta  = tileSize;

    }
}
