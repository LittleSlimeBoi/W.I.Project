using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private HandPanel handPanel;
    public List<CardInfo> deck = new();
    [HideInInspector] public List<CardInfo> drawPile = new();
    [HideInInspector] public List<CardInfo> discardPile = new();
    [HideInInspector] public List<CardInfo> hand = new();
    [HideInInspector] public int handSize = 7;

    // Add card to deck as reward/ Add temporary card to draw pile and discard pile
    public void AddCard(List<CardInfo> list, CardInfo card, int amount = 1)
    {
        for(int i = 0; i < amount; i++)
        {
            list.Add(card);
        }
    }

    // Remove card from deck/ Remove card temporarily from draw pile and discard pile
    public void RemoveCard(List<CardInfo> list, CardInfo card)
    {
        list.Remove(card);
    }

    // Start a game
    public void InitBoard()
    {
        drawPile.Clear();
        discardPile.Clear();
        hand.Clear();

        drawPile.AddRange(deck);
        Shuffle();
    }

    // Refill draw pile
    public void Refill()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();
    }

    // Shuffle draw pile
    public void Shuffle()
    {
        int n = drawPile.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n);
            CardInfo swap = drawPile[n];
            drawPile[n] = drawPile[k];
            drawPile[k] = swap;
        }
    }

    // Draw card
    public void DrawCard(int amount)
    {
        if(drawPile.Count >= amount)
        {
            hand.AddRange(drawPile.GetRange(0, amount));
            drawPile.RemoveRange(0, amount);
        }
        else
        {
            int leftover = amount - drawPile.Count;

            hand.AddRange(drawPile);
            drawPile.Clear();

            Refill();
            Shuffle();

            hand.AddRange(drawPile.GetRange(0, leftover));
            drawPile.RemoveRange(0, leftover);
        }

        handPanel.blocker.gameObject.SetActive(true);
        for (int i = hand.Count - amount; i < hand.Count; i++)
        {
            handPanel.Display(i, hand);
        }
    }

    // Discard when played
    public void Discard(int index)
    {
        handPanel.Play(index);
        discardPile.Add(hand[index]);
        hand.RemoveAt(index);
    }

    // Discard all at turn's end
    public void Flush()
    {
        discardPile.AddRange(hand);
        hand.Clear();
        handPanel.FlushHand();
    }

}
