using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public List<Card> playerHand = new List<Card>();
    private CardManager cardManager;
    private GameManager gameManager;

    public Vector2 cardSpacing = new Vector2(2.5f, 3.5f);
    public Vector2 startOffset = new Vector2(-2.25f, 2.0f);

    private void Start()
    {
        cardManager = GameObject.FindWithTag("CardManager").GetComponent<CardManager>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        DrawStartingHand();
    }

    private void DrawStartingHand()
    {
        float totalWidth = (3 - 1) * cardSpacing.x;
        float totalHeight = cardSpacing.y;
        //RotateHand(false);

        for (int i = 0; i < 6; i++)
        {
            cardManager.DrawAndDiscardCard();
            Card drawnCard = cardManager.TakeTopCard(); // Gets top card in discard pile
            if (drawnCard != null)
            {
                drawnCard.transform.SetParent(gameObject.transform.GetChild(0).transform);
                drawnCard.gameObject.SetActive(true);
                playerHand.Add(drawnCard);

                int row = i / 3;
                int col = i % 3;

                float xPosition = -totalWidth / 2 + col * cardSpacing.x + gameObject.transform.position.x;
                float yPosition = startOffset.y + row * totalHeight + gameObject.transform.position.y;

                drawnCard.transform.position = new Vector3(xPosition, yPosition, 0);
            }
        }
        gameObject.transform.rotation = Quaternion.Euler(0, 0, (90 * (GetPlayerNumber() - 1)));

    }

    public void OnCardClicked(Card clickedCard)
    {
        if (playerHand.Contains(clickedCard) && gameManager.currentPlayerTurn == GetPlayerNumber() - 1)
        {
            ReplaceCard(clickedCard);
            gameManager.NextTurn();
        }
    }

    private void ReplaceCard(Card oldCard)
    {
        if (cardManager == null) return;
        int index = playerHand.IndexOf(oldCard);

        Card newCard = cardManager.TakeTopCard(); // Gets the last item in discardPile list
        if (newCard == null) return;

        playerHand[index] = newCard;
        newCard.transform.position = oldCard.transform.position;
        newCard.transform.rotation = oldCard.transform.rotation;
        newCard.transform.SetParent(gameObject.transform.GetChild(0)); // Set card to the CANVAS of the player object

        cardManager.DiscardCard(oldCard);
    }

    /// <summary>
    /// Returns the # of the player in their object name
    /// </summary>
    /// <returns>int based on the number of the player</returns>
    private int GetPlayerNumber()
    {
        String playerNumberString = gameObject.name.Split("Player")[1];
        return int.Parse(playerNumberString);
    }
}