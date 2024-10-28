using System;
using System.Xml.Serialization;
using UnityEngine;

public class HandPanel : MonoBehaviour
{
    public Board board;
    public Card cardPrefab;
    public Transform handContent;

    private void Start()
    {
        DropZone.OnCardDrop += UpdateRaycastOnCardDrop;
    }

    private void OnDestroy()
    {
        DropZone.OnCardDrop -= UpdateRaycastOnCardDrop;
    }

    // Display drawed cards
    public void Display(int index)
    {
        GameObject cardObj = Instantiate(cardPrefab.gameObject);
        cardObj.transform.SetParent(handContent);

        CardInfo card = board.hand[index];
        Card slot = cardObj.GetComponent<Card>();
        slot.InitCard(card, index);
    }

    // Remove played card
    public void Play(int index)
    {
        Card slot = handContent.GetChild(index).GetComponent<Card>();
        slot.RemoveCard();

        // Update hand index for other cards in hand
        for (int i = index; i < handContent.childCount; i++)
        {
            slot = handContent.GetChild(i).GetComponent<Card>();
            slot.handIndex = i;
        }
    }

    // Remove all card on end turn
    public void FlushHand()
    {
        foreach(Transform child in handContent)
        {
            Destroy(child.gameObject);
        }
        handContent.DetachChildren();
    }

    // Allow hand panel to work with cancel panel
    public void UpdateRaycastOnCardDrop()
    {
        handContent.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = !(CardMouseEvent.isDropped);
    }
}
