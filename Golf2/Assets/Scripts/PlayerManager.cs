using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public List<Card> playerHand = new List<Card>();
    private CardManager cardManager;

    public Vector2 cardSpacing = new Vector2(2.5f, 3.5f);
    public Vector2 startOffset = new Vector2(-2.25f, 2.0f);

    private void Start()
    {
        cardManager = FindObjectOfType<CardManager>();
        DrawStartingHand();
    }

    private void DrawStartingHand()
    {
        float totalWidth = (3 - 1) * cardSpacing.x;
        float totalHeight = cardSpacing.y;

        for (int i = 0; i < 6; i++)
        {
            Card drawnCard = cardManager.DrawCard();
            if (drawnCard != null)
            {
                playerHand.Add(drawnCard);

                int row = i / 3;
                int col = i % 3;

                float xPosition = -totalWidth / 2 + col * cardSpacing.x;
                float yPosition = startOffset.y + row * totalHeight;

                drawnCard.transform.position = new Vector3(xPosition, yPosition, 0);
            }
        }
    }

    public void OnCardClicked(Card clickedCard)
    {
        if (cardManager != null && cardManager.drawnCard != null && playerHand.Contains(clickedCard))
        {
            ReplaceCard(clickedCard);
        }
    }

    private void ReplaceCard(Card oldCard)
    {
        if (cardManager == null || cardManager.drawnCard == null) return;

        int index = playerHand.IndexOf(oldCard);
        if (index >= 0)
        {
            Card newCard = cardManager.drawnCard;
            playerHand[index] = newCard;
            newCard.transform.position = oldCard.transform.position;
            newCard.gameObject.SetActive(true);
            cardManager.DiscardCard(oldCard);
            cardManager.drawnCard = null;
        }
    }
}