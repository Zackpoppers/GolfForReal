using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform deckAndDiscardPile;
    public Image deckImage;
    public Transform inDeckCardsParent; // Parent for the cards when their in the deck
    public Transform discardedCardsParent; // Parent for the cards when their in the discard pile
    public Transform discardTransform;

    public bool deckCardDrawn = false;

    public List<Card> deck = new List<Card>();
    private List<Card> discardPile = new List<Card>();

    public List<Sprite> cardSprites;
    public Sprite cardBackSprite;

    private void Awake()
    {
        InitializeDeck();
        ShuffleDeck();
    }

    private void InitializeDeck()
    {
        string[] suits = { "clubs", "diamonds", "hearts", "spades" };

        for (int value = 2; value <= 14; value++)
        {
            foreach (string suit in suits)
            {
                GameObject newCardObj = Instantiate(cardPrefab, inDeckCardsParent.position, Quaternion.identity);
                Card newCard = newCardObj.GetComponent<Card>();

                if (newCard != null)
                {
                    newCardObj.name = GetCardName(value, suit);
                    newCardObj.transform.SetParent(inDeckCardsParent);

                    // setting card with sprite
                    int spriteIndex = GetSpriteIndex(value, suit);
                    Sprite faceSprite = (spriteIndex >= 0 && spriteIndex < cardSprites.Count)
                        ? cardSprites[spriteIndex]
                        : null;

                    newCard.SetCard(NormalizeValue(value), suit, faceSprite, cardBackSprite);

                    deck.Add(newCard);
                    newCardObj.SetActive(false);
                }
            }
        }
    }

    private string GetCardName(int value, string suit)
    {
        // my ocd
        return $"{ValueToString(value)} of {suit}";
    }

    private int NormalizeValue(int val)
    {
        // Ace=1
        return (val == 14) ? 1 : val;
    }

    private string ValueToString(int val)
    {
        if (val >= 2 && val <= 10) return val.ToString();
        if (val == 11) return "Jack";
        if (val == 12) return "Queen";
        if (val == 13) return "King";
        if (val == 1 || val == 14) return "Ace";
        return null;
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

    public void DrawAndDiscardCard(bool playerClicked = false)
    {
        if (deck.Count == 0)
        {
            Debug.Log("No cards in the deck to draw!");
            return;
        }
        else if (deck.Count == 1) deckImage.gameObject.SetActive(false);

        if (deckCardDrawn && playerClicked)
        {
            Debug.Log("Deck card already drawn");
            return;
        }
        else
        {
            SetDeckDrawable(playerClicked);
        }


        Card drawnCard = deck[0];
        deck.RemoveAt(0);
        DiscardCard(drawnCard);
    }

    public void SetDeckDrawable(bool canDrawCard)
    {
        deckCardDrawn = canDrawCard;
        deckImage.color = new Color(1, 1, 1, deckCardDrawn ? 0.3f : 1.0f);
    }

    public void DiscardCard(Card card)
    {
        if (card == null) return;
        discardPile.Add(card);

        card.transform.SetParent(discardedCardsParent); // Set the parent of the card to the discard pile
        card.transform.position = discardTransform.position; // Place it where the discard pile is
        card.transform.rotation = deckAndDiscardPile.transform.rotation;
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

    private int GetSpriteIndex(int value, string suit)
    {
        // just check the sprites/cards folder to understand!!!

        int rank = value - 2; // 2->0, 3->1,..., 14 (Ace)->12
        int suitOffset = suit switch
        {
            "clubs" => 0,
            "diamonds" => 1,
            "hearts" => 2,
            "spades" => 3,
            _ => 0
        };
        return rank * 4 + suitOffset;
    }
}