using System;
using System.Linq;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Card : MonoBehaviour
{
    public int value;
    public string suit;
    public bool isFaceUp = true;

    [SerializeField] private AudioClip flipSoundClip;
    [SerializeField] private AudioClip drawSoundClip;

    public Image cardImage;
    private Sprite faceSprite;
    private Sprite backSprite;

    public void SetCard(int cardValue, string cardSuit, Sprite newFaceSprite, Sprite newBackSprite)
    {
        value = cardValue;
        suit = cardSuit;
        faceSprite = newFaceSprite;
        backSprite = newBackSprite;
        isFaceUp = false;
        UpdateCardVisual();
    }

    private void UpdateCardVisual() => cardImage.sprite = isFaceUp ? faceSprite : backSprite;

    public void Flip()
    {
        isFaceUp = !isFaceUp;
        UpdateCardVisual();

        
    }

    public void SetFacingUp(bool facingUp)
    {
        isFaceUp = facingUp;
        UpdateCardVisual();
    }

    public void OnClick()
    {
        GameObject parent = gameObject.transform.parent.parent.gameObject;
        Debug.Log($"{gameObject.name} clicked on {parent.name}");
        if (AttachedToPlayer())
        {
            PlayerManager playerManager = parent.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.OnCardClicked(this);
            }
            SoundFXManager.instance.PlaySoundFXClip(drawSoundClip, transform, 1.0f);
        }
    }

    public void OnRightClick(BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;
        if (pointerData.button != PointerEventData.InputButton.Right) return; // Return if it's not right click

        GameObject parent = gameObject.transform.parent.parent.gameObject;
        Debug.Log($"{gameObject.name} left clicked on {parent.name}");
        if (AttachedToPlayer())
        {
            PlayerManager playerManager = parent.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.OnCardClicked(this, true);
            }
            SoundFXManager.instance.PlaySoundFXClip(flipSoundClip, transform, 1.0f);
        }
    }

    /// <summary>
    /// Checks if the card is in a player's hand
    /// </summary>
    /// <returns>If the card is in a player's hand</returns>
    private bool AttachedToPlayer()
    {
        GameObject parent = gameObject.transform.parent.parent.gameObject; // parent -> canvas, parent of parent -> player
        return parent.name.Contains("Player");
    }

}