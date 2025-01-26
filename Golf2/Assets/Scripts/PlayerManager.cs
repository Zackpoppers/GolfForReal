using System;
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
        cardManager = GameObject.FindWithTag("CardManager").GetComponent<CardManager>();
        DrawStartingHand();
    }

    private void DrawStartingHand()
    {
        float totalWidth = (3 - 1) * cardSpacing.x;
        float totalHeight = cardSpacing.y;
        RotateHand(false);

        for (int i = 0; i < 6; i++)
        {
            cardManager.DrawAndDiscardCard();
            Card drawnCard = cardManager.TakeTopCard(); // Gets top card in discard pile
            if (drawnCard != null)
            {
                drawnCard.transform.SetParent(gameObject.transform);
                drawnCard.gameObject.SetActive(true);
                playerHand.Add(drawnCard);

                int row = i / 3;
                int col = i % 3;

                float xPosition = -totalWidth / 2 + col * cardSpacing.x + gameObject.transform.position.x;
                float yPosition = startOffset.y + row * totalHeight + gameObject.transform.position.y;

                drawnCard.transform.position = new Vector3(xPosition, yPosition, 0);
            }
        }
        RotateHand(true);

    }

    public void OnCardClicked(Card clickedCard)
    {
        Console.WriteLine($"{clickedCard.name} clicked!");
        if (playerHand.Contains(clickedCard))
        {
            Console.WriteLine($"Card found and being replaced...");
            ReplaceCard(clickedCard);
        }
    }

    private void ReplaceCard(Card oldCard)
    {
        if (cardManager == null) return;
        RotateHand(false);
        int index = playerHand.IndexOf(oldCard);

        Card newCard = cardManager.TakeTopCard(); // Gets the last item in discardPile list
        playerHand[index] = newCard;

        newCard.transform.position = oldCard.transform.position;
        newCard.gameObject.SetActive(true);

        cardManager.DiscardCard(oldCard);
        oldCard.gameObject.transform.rotation = Quaternion.identity;
        RotateHand(true);
    }

    private int GetPlayerNumber()
    {
        String playerNumberString = gameObject.name.Split("Player")[1];
        int parsedNumber = int.Parse(playerNumberString);
        Debug.Log($"Returning {parsedNumber}");
        return parsedNumber;
    }

    private void RotateHand(bool basedOnPlayerNumber)
    {
        if (basedOnPlayerNumber) gameObject.transform.rotation = Quaternion.Euler(0, 0, (90 * (GetPlayerNumber() - 1)));
        else gameObject.transform.rotation = Quaternion.identity;
    }
}