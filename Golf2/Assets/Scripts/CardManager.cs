using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform deckTransform;
    public Transform inDeckCardsParent; // Parent for the cards when their in the deck
    public Transform discardedCardsParent; // Parent for the cards when their in the discard pile
    public Transform discardTransform;

    public List<Card> deck = new List<Card>();
    private List<Card> discardPile = new List<Card>();

    private void Awake()
    {
        InitializeDeck();
        ShuffleDeck();
    }

    private void InitializeDeck()
    {
        string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };

        foreach (string suit in suits)
        {
            for (int value = 1; value <= 13; value++)
            {
                GameObject newCardObj = Instantiate(cardPrefab, deckTransform.position, Quaternion.identity);
                Card newCard = newCardObj.GetComponent<Card>();
                if (newCard != null)
                {
                    newCardObj.name = $"{CardValueToString(value)} of {suit}";
                    newCardObj.transform.SetParent(inDeckCardsParent);
                    newCard.SetCard(value, suit);
                    deck.Add(newCard);
                    newCardObj.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Changes the card value to its string format (11 -> "Jack", 12 -> "Queen")
    /// </summary>
    /// <param name="value">Int value of card you want the string for</param>
    private string CardValueToString(int value)
    {
        switch (value)
        {
            case 0:
                return "Joker";
            case 1:
                return "One";
            case 2:
                return "Two";
            case 3:
                return "Three";
            case 4:
                return "Four";
            case 5:
                return "Five";
            case 6:
                return "Six";
            case 7:
                return "Seven";
            case 8:
                return "Eight";
            case 9:
                return "Nine";
            case 10:
                return "Ten";
            case 11:
                return "Jack";
            case 12:
                return "Queen";
            case 13:
                return "King";
            default:
                return null;
        }
    }

    private void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public void DrawAndDiscardCard()
    {
        if (deck.Count == 0)
        {
            Debug.Log("No cards in the deck to draw!");
            return;
        }
        else if (deck.Count == 1) deckTransform.gameObject.SetActive(false);


        Card drawnCard = deck[0];
        deck.RemoveAt(0);
        DiscardCard(drawnCard);
    }

    public void DiscardCard(Card card)
    {
        if (card == null) return;
        discardPile.Add(card);

        card.transform.SetParent(discardedCardsParent); // Set the parent of the card to the discard pile
        card.transform.position = discardTransform.position; // Place it where the discard pile is
        card.transform.rotation = Quaternion.identity;
        UpdateVisuals();
    }

    public Card TakeTopCard()
    {
        if (discardPile.Count == 0) return null;

        Card topCard = discardPile[discardPile.Count - 1];
        discardPile.RemoveAt(discardPile.Count - 1);
        UpdateVisuals();
        return topCard;
    }

    private void UpdateVisuals()
    {
        if (discardPile.Count > 0)
        {
            for (int i = 0; i < discardPile.Count - 1; i++)
            {
                discardPile[i].gameObject.SetActive(false);
            }

            discardPile[discardPile.Count - 1].gameObject.SetActive(true);
        }
/*        if (deckTransform.GetComponent<SpriteRenderer>() != null)
        {
            deckTransform.GetComponent<SpriteRenderer>().enabled = deck.Count > 0;
        }

        if (discardTransform.GetComponent<SpriteRenderer>() != null)
        {
            discardTransform.GetComponent<SpriteRenderer>().enabled = discardPile.Count > 0;
            

        }*/

    }
}