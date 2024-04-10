using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler
{
    [HideInInspector] public Card card;
    public Image box;
    public Color boxColor;
    private int playerPosX, playerPosY;

    public void OnDrop(PointerEventData eventData)
    {
        if (card == null)
        {
            card = eventData.pointerDrag.transform.GetComponent<Card>();
            if (card.mouseEvent.CanDrag())
            {
                playerPosX = Card.player.GetPosX();
                playerPosY = Card.player.GetPosY();
                CardMouseEvent.isDropped = true;

                card.mouseEvent.parentReturnTo = transform;
                card.transform.localScale = new Vector2(2f, 2f);

                transform.root.Find("Hand Panel").GetComponent<HandPanel>().UpdateRaycastOnDrop();
                card.AssignValueToTile(playerPosX, playerPosY);
            }
            else
            {
                card = null;
            }
        }
    }

    public void Cancel()
    {
        CardMouseEvent.isDropped = false;
        card.transform.localScale = new Vector2(1.2f, 1.2f);
        if (card.mouseEvent.Temp != null)
        {
            card.mouseEvent.parentReturnTo = card.mouseEvent.Temp.transform.parent;
            card.mouseEvent.ReturnToHand();
        }

        transform.root.Find("Hand Panel").GetComponent<HandPanel>().UpdateRaycastOnDrop();
        card.UnassignValueToTile(playerPosX, playerPosY);
        Card.player.mySide.CancelSelectedTile();
        Card.player.otherSide.CancelSelectedTile();

        card = null;
    }
}
