using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler
{
    [HideInInspector] public Card card;
    public Image box;
    public Color boxColor;
    private int playerCurrentX, playerCurrentY;

    public static event Action OnCardDrop;

    private void Start()
    {
        CancelPanel.OnCancel += ReturnCardToHand;
    }

    private void OnDestroy()
    {
        CancelPanel.OnCancel -= ReturnCardToHand;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (card == null)
        {
            card = eventData.pointerDrag.transform.GetComponent<Card>();
            if (card.mouseEvent.CanDrag())
            {
                playerCurrentX = Card.player.GetPosX();
                playerCurrentY = Card.player.GetPosY();
                CardMouseEvent.isDropped = true;

                card.mouseEvent.parentReturnTo = transform;
                card.transform.localScale = new Vector2(2f, 2f);

                card.AssignValueToTile(true, playerCurrentX, playerCurrentY);
                OnCardDrop?.Invoke();
            }
            else
            {
                card = null;
            }
        }
    }

    public void ReturnCardToHand()
    {
        CardMouseEvent.isDropped = false;
        card.transform.localScale = new Vector2(1.2f, 1.2f);
        if (card.mouseEvent.Temp != null)
        {
            card.mouseEvent.parentReturnTo = card.mouseEvent.Temp.transform.parent;
            card.mouseEvent.ReturnToHand();
        }

        card.AssignValueToTile(false, playerCurrentX, playerCurrentY);
        Card.player.mySide.CancelSelectedTile();
        Card.player.otherSide.CancelSelectedTile();

        OnCardDrop?.Invoke();

        card = null;
    }
}
