using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int value;
    public string suit;
    public bool isFaceUp = true;

    private TextMeshPro textComponent;
    private PlayerManager playerManager;

    private void Awake()
    {
        textComponent = GetComponentInChildren<TextMeshPro>();
        playerManager = FindObjectOfType<PlayerManager>();
        UpdateCardVisual();
    }

    public void SetCard(int cardValue, string cardSuit)
    {
        value = cardValue;
        suit = cardSuit;
        isFaceUp = true;
        UpdateCardVisual();
    }

    private void UpdateCardVisual()
    {
        if (textComponent != null)
        {
            string valueText = value switch
            {
                1 => "A",
                11 => "J",
                12 => "Q",
                13 => "K",
                _ => value.ToString()
            };

            textComponent.text = isFaceUp ? $"{valueText} {suit[0]}" : "";
        }
    }

    public void OnClick()
    {
        if (playerManager != null)
        {
            playerManager.OnCardClicked(this);
        }
    }
}