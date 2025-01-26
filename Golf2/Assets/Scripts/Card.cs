using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int value;
    public string suit;
    public bool isFaceUp = true;

    private TextMeshPro textComponent;

    private void Awake()
    {
        textComponent = GetComponentInChildren<TextMeshPro>();
        UpdateCardVisual();
    }

    public void SetCard(int cardValue, string cardSuit)
    {
        value = cardValue;
        suit = cardSuit;
        isFaceUp = true;
        textComponent.gameObject.SetActive(isFaceUp);
        UpdateCardVisual();
    }

    private void UpdateCardVisual()
    {
        if (textComponent != null)
        {
            string valueText = value switch
            {
                0 => "Joker",
                1 => "Ace",
                11 => "Jack",
                12 => "Queen",
                13 => "King",
                _ => value.ToString()
            };

            textComponent.text = isFaceUp ? $"{valueText}\n{suit[0]}{suit[1]}{suit[2]}" : "";
        }
    }

    public void Flip()
    {
        isFaceUp = !isFaceUp;
        textComponent.gameObject.SetActive(isFaceUp);
    }

    public void OnClick()
    {
        GameObject parent = gameObject.transform.parent.gameObject;

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