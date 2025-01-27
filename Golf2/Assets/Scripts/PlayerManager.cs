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
    int randomStartingFaceUpCardIndex1;
    int randomStartingFaceUpCardIndex2;

    private void Start()
    {
        cardManager = GameObject.FindWithTag("CardManager").GetComponent<CardManager>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        DrawStartingHand();

        // Pick two random cards to start face-up (will be flipped after the first turn)
        System.Random random = new System.Random();
        randomStartingFaceUpCardIndex1 = random.Next(0, 5);
        randomStartingFaceUpCardIndex2 = randomStartingFaceUpCardIndex1;
        while (randomStartingFaceUpCardIndex2 == randomStartingFaceUpCardIndex1) randomStartingFaceUpCardIndex2 = random.Next(0, 5);

        playerHand[randomStartingFaceUpCardIndex1].SetFacingUp(true);
        playerHand[randomStartingFaceUpCardIndex2].SetFacingUp(true);
    }

    /// <summary>
    /// Draws 6 cards (all face-down) for the player to begin the game with
    /// </summary>
    private void DrawStartingHand()
    {
        float totalWidth = (3 - 1) * cardSpacing.x;
        float totalHeight = cardSpacing.y;
        //RotateHand(false);

        for (int i = 0; i < 6; i++)
        {
            cardManager.DrawAndDiscardCard();
            Card drawnCard = cardManager.TakeTopCard(); // Gets top card in discard pile
            drawnCard.SetFacingUp(false); // Makes sure the card is face-down
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

    /// <summary>
    /// Replaces the card or flips it based on if it's a valid move and which button was pressed
    /// </summary>
    /// <param name="clickedCard">The card that was clicked</param>
    /// <param name="rightClick">If the click was a right click to flip the card or not</param>
    public void OnCardClicked(Card clickedCard, bool rightClick = false)
    {
        if (gameManager.gameOver) return;

        bool firstRound = gameManager.globalTurnCount == 0;
        bool coroutineRunning = gameManager.rotating || gameManager.switchingCards;
        int cardIndex = playerHand.IndexOf(clickedCard);

        if (playerHand.Contains(clickedCard) && // Player has the card
            gameManager.currentPlayerTurn == GetPlayerNumber() - 1 && // Has to be this players turn
            !coroutineRunning && // Cannot have any coroutine (basically animations) running (rotating camera, swapping cards, etc)
            (!clickedCard.isFaceUp || firstRound)) // AND card has to be face-down unless alowed by being the first round
        {

            EnlargeCards(false);
            if (rightClick) clickedCard.SetFacingUp(true);
            else ReplaceCard(clickedCard);

            gameManager.NextTurn();
        }

        // Flip over the 2 random starting cards
        if (firstRound)
        {
            if (cardIndex != randomStartingFaceUpCardIndex1) playerHand[randomStartingFaceUpCardIndex1].SetFacingUp(false);
            if (cardIndex != randomStartingFaceUpCardIndex2) playerHand[randomStartingFaceUpCardIndex2].SetFacingUp(false);
        }


    }

    /// <summary>
    /// Swaps the positions of the input card and the card on the top of the discard pile
    /// </summary>
    /// <param name="oldCard">The card in the player's hand that should be swapped with the discard pile card</param>
    private void ReplaceCard(Card oldCard)
    {
        if (cardManager == null) return;
        int index = playerHand.IndexOf(oldCard);

        Card newCard = cardManager.TakeTopCard(); // Gets the last item in discardPile list
        if (newCard == null) return;

        playerHand[index] = newCard;
        cardManager.discardSwitchedWithCardInHand = true;
        StartCoroutine(gameManager.SwapPositions(oldCard.gameObject, newCard.gameObject, gameManager.cardSwapDuration));
        newCard.transform.rotation = oldCard.transform.rotation;
        newCard.transform.localScale = oldCard.transform.localScale;
        newCard.transform.SetParent(gameObject.transform.GetChild(0)); // Set card to the CANVAS of the player object


        cardManager.DiscardCard(oldCard, false);
    }

    /// <summary>
    /// Scales all the cards up by 20% if true, back to normal if false
    /// </summary>
    /// <param name="enlarge">Whether you want to scale by 20% or go back to the original scale</param>
    public void EnlargeCards(bool enlarge)
    {
        Vector3 scalingVector = enlarge ? new Vector3(1.2f, 1.2f, 1.2f) : Vector3.one;
        gameObject.transform.localScale = scalingVector;
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

    /// <summary>
    /// Calculates the player's score based on the cards in their hand
    /// </summary>
    /// <returns>An int based on the score they recieved</returns>
    public int CalculateScore()
    {
        int score = 0;

        for (int i = 0; i < playerHand.Count; i++)
        {
            Card card = playerHand[i];

            if (card.value <= 2) // Ace or two
            {
                score -= card.value;
                continue;
            }
            else if (card.value == 13) continue; // King

            if (i != 0 && i != 3 && playerHand[i - 1].value == card.value) continue; // Matching card to left
            if (i != 2 && i != 5 && playerHand[i + 1].value == card.value) continue; // Matching card to right
            if (i <= 2 && playerHand[i + 3].value == card.value) continue; // Matching card above
            if (i >= 3 && playerHand[i - 3].value == card.value) continue; // Matching card below

            score += card.value >= 10 ? 10 : card.value;

        }

        return score;
    }
}