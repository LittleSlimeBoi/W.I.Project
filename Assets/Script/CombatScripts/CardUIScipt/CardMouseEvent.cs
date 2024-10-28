using UnityEngine;
using UnityEngine.EventSystems;

public class CardMouseEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private bool isSelected;
    public static bool canHover = true;
    public static bool isDragging;
    public static bool isDropped;
    [SerializeField] private static DropZone dropZone;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject EmptyCard;
    [HideInInspector] public Transform parentReturnTo = null;
    private GameObject temp;

    private void Start()
    {
        dropZone = transform.root.Find("Drop Zone").GetComponent<DropZone>();
    }

    //Transform + Enlarge
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (canHover && !isDropped)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, 115);
            isSelected = true;
        }
    }

    //Transform back to hand
    public void OnPointerExit(PointerEventData eventData)
    {
        if (canHover && !isDropped)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, 0);
            isSelected = false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CanDrag() && isSelected)
        {
            canHover = false;
            temp = Instantiate(EmptyCard);
            temp.transform.SetParent(transform.parent);
            temp.transform.SetSiblingIndex(transform.GetSiblingIndex());
            parentReturnTo = transform.parent;
            transform.SetParent(transform.root);
            canvasGroup.blocksRaycasts = false;

            dropZone.box.color = dropZone.boxColor;

            isDragging = true;
        }
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (CanDrag() && isSelected)
        {
            transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (CanDrag() && isSelected)
        {
            isSelected = false;
            canHover = true;
            canvasGroup.blocksRaycasts = true;

            dropZone.box.color = Color.clear;

            ReturnToHand();

            isDragging = false;
        }
    }

    public bool CanDrag()
    {
        bool con1 = Card.player.mana - gameObject.GetComponent<Card>().info.cost >= 0;
        //bool con2 = CombatManager.Instance.state == CombatManager.CombatState.YourTurn;
        return con1;// && con2;
    }

    public void ReturnToHand()
    {
        transform.SetParent(parentReturnTo);
        transform.SetSiblingIndex(temp.transform.GetSiblingIndex());

        if (!isDropped)
        {
            temp.transform.SetParent(null);
            Destroy(temp);
        }
    }

    public GameObject Temp { get { return temp; } }

}
