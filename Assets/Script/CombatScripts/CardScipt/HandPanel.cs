using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPanel : MonoBehaviour
{
    public Card cardPrefab;
    public Transform handContent;
    public Transform blocker;

    bool mutexlock = false;

    private void Start()
    {
        DropZone.OnCardDrop += UpdateRaycastOnCardDrop;
    }

    private void OnDestroy()
    {
        DropZone.OnCardDrop -= UpdateRaycastOnCardDrop;
    }

    // Display drawed cards
    public void Display(int index, List<CardInfo> hand)
    {
        StartCoroutine(DrawCard(index, hand));
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
        GetComponent<CanvasGroup>().blocksRaycasts = !CardMouseEvent.isDropped;
    }

    private IEnumerator DrawCard (int index, List<CardInfo> hand)
    {
        while (mutexlock)
        {
            yield return null;
        }
        mutexlock = true;

        GameObject cardObj = Instantiate(cardPrefab.gameObject, transform);
        CardInfo card = hand[index];
        Card slot = cardObj.GetComponent<Card>();
        slot.InitCard(card, index);

        Vector2 pointA = new(-1200, 350);
        Vector2 pointB = new(0, 350);
        Transform ct = cardObj.transform;
        ct.localPosition = pointA;

        for (int i = 1; i < ct.childCount; i++)
        {
            ct.GetChild(i).gameObject.SetActive(false);
        }

        float timeAB = 0;
        while (timeAB < 0.4f)
        {
            ct.localPosition = Vector2.Lerp(pointA, pointB, timeAB / 0.4f);
            if (timeAB > 0.3f)
            {
                float angleY = Mathf.PingPong(180 * (timeAB - 0.3f) / 0.1f, 90);
                if (angleY >= 75)
                {
                    for (int i = 1; i < ct.childCount; i++)
                    {
                        ct.GetChild(i).gameObject.SetActive(true);
                    }
                }
                ct.rotation = Quaternion.Euler(0, angleY, 0);
            }
            timeAB += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        ct.SetParent(handContent);
        ct.rotation = Quaternion.Euler(0, 0, 0);

        mutexlock = false;
        blocker.gameObject.SetActive(index != hand.Count - 1);
    }
}
