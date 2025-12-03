using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public List<Card> hand = new List<Card>();
    public Transform handArea;
    public double playerStartX = 61.60;
    public double playerStartY = 166.62;
    public double cardSpacing = 60.0;

    /// <summary>Baþlangýç kartlarý (7 kart) çeker.</summary>
    public void DrawStartingCards()
    {
        for (int i = 0; i < 7; i++)
        {
            Card card = DrawCard();//61.60, 166.62
            RectTransform rt = card.GetComponent<RectTransform>();
            if (card != null)
            {
                // Kartlarý yatay olarak yan yana diz
                if (rt != null)
                {
                    rt.anchoredPosition = new Vector2((float)(playerStartX + i * cardSpacing), (float)playerStartY);
                }
            }
        }
    }
    /// <summary>
    /// Desteden bir kart çeker ve görsel olarak player's handArea'ya koyar.
    /// Eðer DeckManager.DrawCard() null dönerse (deste boþ) güvenli þekilde çýkar.
    /// </summary>
    public Card DrawCard()
    {
        Card card = UNOManager.Instance.deckManager.DrawCard();
        if (card == null)
        {
            Debug.LogWarning("Player.DrawCard(): Deste boþ, kart çekilemedi.");
            return card;
        }

        // Parent ayarla ve görsel pozisyonu (UI layout varsa bunu býrakabilirsin)
        card.transform.SetParent(handArea, false);
        card.transform.localPosition = Vector3.zero;

        // Bu kart oyuncuya ait -> týklanabilir olsun
        card.SetInteractable(true);
        card.RefreshVisual();

        hand.Add(card);
        return card;
    }

    public void RemoveCard(Card card)
    {
        if (hand.Contains(card)) hand.Remove(card);
    }
}
