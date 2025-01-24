using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform deckTransform;
    public Transform discardTransform;

    private List<Card> deck = new List<Card>();
    private List<Card> discardPile = new List<Card>();
    public Card drawnCard;

    private void Awake()
    {
        InitializeDeck();
        ShuffleDeck();
        UpdateVisuals();
    }

    public void DrawCardFromDeck()
    {
        if (deck.Count == 0 || drawnCard != null) return;

        drawnCard = DrawCard();
        if (drawnCard != null)
        {
            drawnCard.transform.position = new Vector3(0, -4, 0);
        }
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
                    newCard.SetCard(value, suit);
                    deck.Add(newCard);
                    newCardObj.SetActive(false);
                }
            }
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

    public Card DrawCard()
    {
        if (deck.Count == 0) return null;

        Card drawnCard = deck[0];
        deck.RemoveAt(0);
        drawnCard.gameObject.SetActive(true);
        UpdateVisuals();
        return drawnCard;
    }

    public void DiscardCard(Card card)
    {
        discardPile.Add(card);

        foreach (var discarded in discardPile)
        {
            discarded.gameObject.SetActive(false);
        }

        card.transform.position = discardTransform.position;
        card.gameObject.SetActive(true);
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (deckTransform.GetComponent<SpriteRenderer>() != null)
        {
            deckTransform.GetComponent<SpriteRenderer>().enabled = deck.Count > 0;
        }

        if (discardTransform.GetComponent<SpriteRenderer>() != null)
        {
            discardTransform.GetComponent<SpriteRenderer>().enabled = discardPile.Count > 0;
        }
    }
}