using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int value;
    public string suit;
    public bool isFaceUp = true;

    public Image cardImage;
    private Sprite faceSprite;
    private Sprite backSprite;

    public void SetCard(int cardValue, string cardSuit, Sprite newFaceSprite, Sprite newBackSprite)
    {
        value = cardValue;
        suit = cardSuit;
        faceSprite = newFaceSprite;
        backSprite = newBackSprite;
        isFaceUp = true;
        UpdateCardVisual();
    }

    private void UpdateCardVisual()
    {
        if (cardImage != null)
        {
            cardImage.sprite = isFaceUp ? faceSprite : backSprite;
        }
    }

    public void Flip()
    {
        isFaceUp = !isFaceUp;
        UpdateCardVisual();
    }

    public void OnClick()
    {
        GameObject parent = gameObject.transform.parent.parent.gameObject; // parent -> canvas, parent of parent -> player

        Debug.Log($"{gameObject.name} clicked on {parent.name}");
        if (parent.name.Contains("Player"))
        {
            PlayerManager playerManager = parent.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.OnCardClicked(this);
            }
        }
    }
}