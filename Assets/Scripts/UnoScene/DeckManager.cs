using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// DeckManager: Sadece number kartlardan oluþan bir UNO destesi oluþturur,
/// karýþtýrýr ve prefab olarak çekilecek kartlarý instantiate eder.
/// Ayrýca dýþarýdan gelen CardData listelerini desteye ekleyebilecek bir helper saðlar.
/// </summary>
public class DeckManager : MonoBehaviour
{
    [Tooltip("Kart prefab'ý (Card scriptine sahip)")]
    public GameObject cardPrefab;

    [Tooltip("Deck görsel/anchor transformu (isteðe baðlý). Instantiate edilen kartlar buranýn altýna konur).")]
    public Transform deckArea;

    // iç destemiz (CardData modeli)
    private List<CardData> deck = new List<CardData>();

    /// <summary>
    /// Oluþtur: Sadece number kartlar (0 birer, 1-9 ikiþer) - action/wild yok.
    /// </summary>
    public void CreateDeck()
    {
        deck.Clear();

        CardColor[] colors = new CardColor[] {
            CardColor.Red, CardColor.Green, CardColor.Blue, CardColor.Yellow
        };

        foreach (var color in colors)
        {
            // 0 -> bir adet
            deck.Add(new CardData { color = color, type = CardType.Number, number = 0 });

            // 1..9 -> her birinden iki adet (UNO standardý)
            for (int num = 1; num <= 9; num++)
            {
                deck.Add(new CardData { color = color, type = CardType.Number, number = num });
                deck.Add(new CardData { color = color, type = CardType.Number, number = num });
            }

            // Action kartlarý: Skip, Reverse, DrawTwo -> her birinden iki adet
            deck.Add(new CardData { color = color, type = CardType.Skip });
            deck.Add(new CardData { color = color, type = CardType.Skip });

            deck.Add(new CardData { color = color, type = CardType.Reverse });
            deck.Add(new CardData { color = color, type = CardType.Reverse });

            deck.Add(new CardData { color = color, type = CardType.DrawTwo });
            deck.Add(new CardData { color = color, type = CardType.DrawTwo });
        }

        Shuffle(deck);
    }

    /// <summary>
    /// Karýþtýrma (Fisher-Yates benzeri, UnityEngine.Random kullanarak)
    /// </summary>
    void Shuffle(List<CardData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            var tmp = list[i];
            list[i] = list[rand];
            list[rand] = tmp;
        }
    }

    /// <summary>
    /// Desteden bir kart çeker. Eðer deste boþsa null döner.
    /// Bu metoda gelen çaðrý genelde UNOManager tarafýndan yapýlmalý;
    /// discard'dan refill gerekiyorsa UNOManager onu yönetir.
    /// </summary>
    public Card DrawCard()
    {
        if (deck.Count == 0)
        {
            Debug.LogWarning("DeckManager.DrawCard(): Deste boþ!");
            return null;
        }

        CardData data = deck[0];
        deck.RemoveAt(0);

        // kart prefab'ýný instantiate et
        GameObject newCardObj;
        if (deckArea != null)
            newCardObj = Instantiate(cardPrefab, deckArea);
        else
            newCardObj = Instantiate(cardPrefab);

        Card card = newCardObj.GetComponent<Card>();
        if (card == null)
        {
            Debug.LogError("Card prefab'ýnda 'Card' scripti bulunamadý!");
            Destroy(newCardObj);
            return null;
        }

        card.data = data;
        card.RefreshVisual();
        return card;
    }

    /// <summary>
    /// Dýþýdan verilen CardData listesini destenin altýna ekler.
    /// Genelde discard'dan toplanan kart verileri buraya gönderilir.
    /// </summary>
    /// <param name="cards">Eklenecek CardData listesi</param>
    /// <param name="shuffleAfter">Eklendikten sonra desteyi karýþtýr mý?</param>
    public void AddCardsToBottom(List<CardData> cards, bool shuffleAfter = true)
    {
        if (cards == null || cards.Count == 0) return;

        // CardData'larý destenin sonuna ekle
        deck.AddRange(cards);

        if (shuffleAfter)
            Shuffle(deck);
    }

    /// <summary>
    /// Destede kalan kart sayýsý (UI için kullanýþlý)
    /// </summary>
    public int CardsRemaining => deck.Count;
}
