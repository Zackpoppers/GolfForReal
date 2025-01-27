using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public GameObject cardPrefab;
    private GameManager gameManager;
    public Transform deckAndDiscardPile;
    public Image deckImage;
    public Transform inDeckCardsParent; // Parent for the cards when their in the deck
    public Transform discardedCardsParent; // Parent for the cards when their in the discard pile
    public Transform discardTransform;

    public bool deckCardDrawn = false;
    public bool discardSwitchedWithCardInHand = false;

    public List<Card> deck = new List<Card>();
    private List<Card> discardPile = new List<Card>();

    public Sprite cardBackSprite;

    private Vector3 discardPileSize = new Vector3(1.4f, 1.96f, 1f);
    private Vector3 cardInHandSize = new Vector3(1.25f, 1.75f, 1f);

    private void Awake()
    {
        InitializeDeck();
        ShuffleDeck();
    }

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    /// <summary>
    /// Creates all 5
    /// </summary>
    private void InitializeDeck()
    {
        string[] suits = {"clubs", "diamonds", "hearts", "spades"};

        for (int value = 1; value <= 13; value++)
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
                    Sprite faceSprite = GetCardSprite(ValueToString(value), suit);

                    newCard.SetCard(value, suit, faceSprite, cardBackSprite);

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

    private string ValueToString(int val)
    {
        if (val >= 2 && val <= 10) return val.ToString();
        else if (val == 11) return "Jack";
        else if (val == 12) return "Queen";
        else if (val == 13) return "King";
        else if (val == 1 || val == 14) return "Ace";
        else if (val == 0) return "Joker";
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

    public void OnDeckClick()
    {
        DrawAndDiscardCard(animate:true, playerClicked:true);
    }

    /// <summary>
    /// Draws a card from the deck and discards it into the discard pile face-up
    /// </summary>
    /// <param name="playerClicked">Whether the player clicked the deck to do this or not (False if the program automatically called this method)</param>
    public void DrawAndDiscardCard(bool playerClicked = false, bool animate = false, bool goToNextTurn = false)
    {
        if (deck.Count == 0)
        {
            Debug.Log("No cards in the deck to draw!");
            return;
        }
        else if (deck.Count == 1) deckImage.gameObject.SetActive(false);

        if (deckCardDrawn && playerClicked) // Deck has been drawn and is clicked again
        {
            Debug.Log("Deck card already drawn");
            return;
        }
        else
        {
            SetDeckDrawable(!playerClicked); // Make the deck non-drawable if the player just drew from it
        }


        Card drawnCard = deck[0]; // Take top card
        deck.RemoveAt(0);
        drawnCard.SetFacingUp(true);

        if (animate) StartCoroutine(gameManager.MoveToPosition(drawnCard.gameObject, discardTransform.position, 0.3f, goToNextTurn));

        DiscardCard(drawnCard, !animate); // Add card into discard pile
    }

    /// <summary>
    /// Sets the "canDrawDeck" variable and makes the deck sprite fade if the deck is not drawable
    /// </summary>
    /// <param name="canDrawCard"></param>
    public void SetDeckDrawable(bool canDrawCard)
    {
        deckCardDrawn = !canDrawCard;
        deckImage.color = new Color(1, 1, 1, deckCardDrawn ? 0.3f : 1f);
    }

    public void DiscardCard(Card card, bool movePos = true)
    {
        if (card == null) return;
        discardPile.Add(card);

        card.transform.SetParent(discardedCardsParent); // Set the parent of the card to the discard pile
        if (movePos) card.transform.position = discardTransform.position; // Place it where the discard pile is

        card.transform.rotation = deckAndDiscardPile.transform.rotation;
        card.transform.localScale = discardPileSize;
        card.SetFacingUp(true);

        UpdateVisuals();
    }

    public Card TakeTopCard()
    {
        if (discardPile.Count == 0) return null;

        Card topCard = discardPile[discardPile.Count - 1];
        discardPile.RemoveAt(discardPile.Count - 1);
        topCard.transform.localScale = cardInHandSize;
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

    }

    /// <summary>
    /// Finds the card's face-up sprite based on the rank and suit
    /// </summary>
    /// <param name="rank">The rank of the card</param>
    /// <param name="suit">The suit of the card</param>
    /// <returns>The sprite found</returns>
    public Sprite GetCardSprite(string rank, string suit)
    {
        // Format the file name based on the card naming convention
        string cardFileName = $"{rank}_of_{suit.ToLower()}";
        if (rank.Equals("Jack") || rank.Equals("King") || rank.Equals("Queen")) cardFileName += "2";

        // Load the sprite from the Resources folder
        Sprite cardSprite = Resources.Load<Sprite>($"Cards/{cardFileName}");

        // Check if the sprite was found
        if (cardSprite == null)
        {
            Debug.LogError($"Card sprite not found: {cardFileName}");
        }

        return cardSprite;
    }
}