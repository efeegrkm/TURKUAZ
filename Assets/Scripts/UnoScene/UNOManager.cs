using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UNOManager : MonoBehaviour
{
    public static UNOManager Instance;

    [Header("UI / Transforms")]
    public Transform playerHandArea;
    public Transform botHandArea;
    public Transform discardPileArea;

    [Header("Game refs")]
    public DeckManager deckManager;
    public Player player;
    public Bot bot;
    private bool punish = false;

    [HideInInspector]
    public Card topCard; // En üstteki kart (visual Card objesi)

    public bool isPlayerTurn = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        deckManager.CreateDeck();
        // Baþlangýç ellerini ver (Player/ Bot içindeki metodlar kendi içinde deck'ten çekerlerse null kontrolü gerekir)
        player.DrawStartingCards();
        bot.DrawStartingCards();

        // Ýlk kartý ortaya koy
        Card firstCard = deckManager.DrawCard();
        while (firstCard.data.type != CardType.Number)
        {
            firstCard = deckManager.DrawCard();
        }
        if (firstCard == null)
        {
            // Eðer ilk kart için deck boþsa discard'tan refill dene (nadir)
            RefillDeckFromDiscard();
            firstCard = deckManager.DrawCard();
        }

        if (firstCard != null)
        {
            PlaceCardOnPile(firstCard);
        }
        else
        {
            Debug.LogError("Baþlangýç kartý alýnamadý — deste tamamen boþ!");
        }

        isPlayerTurn = true;
    }

    /// <summary>
    /// Oyuncu manuel olarak bir kart oynadýðýnda çaðrýlýr (Card prefabýndaki CallPlayCard -> UNOManager.PlayCard)
    /// </summary>
    public void PlayCard(Card card)
    {
        // sadece oyuncunun sýrasýysa PlayCard çaðrýlmýþ olmalý, ama güvenlik kontrolü
        if (!isPlayerTurn)
        {
            Debug.Log("Þu anda oyuncu sýrasý deðil.");
            return;
        }

        if (card == null)
        {
            Debug.LogWarning("PlayCard: null card.");
            return;
        }

        if (IsValidMove(card))
        {
            // Masaya koy ve elimizden çýkar
            PlaceCardOnPile(card);

            // Eðer bu kart oyuncunun elinden geldiyse player list'inden çýkar
            if (player.hand.Contains(card)) player.hand.Remove(card);
            CheckGameOver();
            if (card.data.type == CardType.Number)
                EndTurn();
            else ExecuteOperation(card.data.type);
        }
        else
        {
            Debug.Log("Bu kart oynanamaz!");
        }
    }
    public void ExecuteOperation(CardType type)
    {
        if(type == CardType.Skip || type == CardType.Reverse)
        {
            if (isPlayerTurn) Debug.Log("Sýra yine playerda!");
            else
            {
                Debug.Log("Sýra yine botda");
                StartCoroutine(ExecuteBotTurn());
            }
        }
        if (type == CardType.DrawTwo)
        {
            if (isPlayerTurn)
            {
                Debug.Log("Bot+2");
                bot.DrawCard();
                bot.DrawCard();
                EndTurn();
            }
            else
            {
                Debug.Log("Player+2");
                punish = true;
                PlayerDrawCard();
                PlayerDrawCard();
                punish = false;
                EndTurn();
            }
        }
    }

    /// <summary>
    /// Sadece number versiyonda: eðer topCard null ise her kart oynanabilir,
    /// aksi halde renk eþleþmesi veya ayný sayý eþleþmesi gerekiyor.
    /// </summary>
    public bool IsValidMove(Card card)
    {
        if (card == null) return false;
        if (topCard == null) return true;

        // renk eþleþiyorsa
        if (card.data.color == topCard.data.color) return true;

        // ikisi de number ise ayný sayý olmalý
        if (card.data.type == CardType.Number && topCard.data.type == CardType.Number &&
            card.data.number == topCard.data.number) return true;
        if(card.data.type != CardType.Number && card.data.type == topCard.data.type) return true;

        return false;
    }

    /// <summary>
    /// Kartý görsel olarak discard üzerine yerleþtirir ve topCard olarak ayarlar.
    /// </summary>
    public void PlaceCardOnPile(Card card)
    {
        if (card == null) return;

        // Parent discard alaný
        card.transform.SetParent(discardPileArea, false);
        card.transform.SetAsLastSibling();
        // sýfýrlamak isteðe baðlý (UI layout'ýna göre)
        card.transform.localPosition = Vector3.zero;

        topCard = card;
    }

    /// <summary>
    /// Oyuncu desteye týklayýp kart çekmek istediðinde bu metod çaðrýlýr.
    /// Eðer çekilen kart oynanamazsa sýra botta geçer.
    /// </summary>
    public void PlayerDrawCard()
    {
        if (!isPlayerTurn && !punish)
        {
            Debug.Log("Þu an sýrasý sende deðil, çekemezsin.");
            return;
        }

        // desteden çek
        Card drawn = deckManager.DrawCard();
        if (drawn == null)
        {
            // deck boþsa discard'dan refill dene
            RefillDeckFromDiscard();
            drawn = deckManager.DrawCard();
        }

        if (drawn == null)
        {
            Debug.Log("Deste boþ — baþka çekilecek kart yok.");
            // isteðe baðlý: oyunu bitirebilirsin veya sadece sýra deðiþtir
            EndTurn();
            return;
        }

        // Görsel olarak oyuncunun eline koy
        drawn.transform.SetParent(playerHandArea, false);
        drawn.transform.localPosition = Vector3.zero;

        // Player.hand listesine ekle (Player sýnýfý yoksa directly eklenir)
        if (!player.hand.Contains(drawn)) player.hand.Add(drawn);

        // Eðer çekilen kart oynanabiliyorsa oyuncu oynayabilir (biz otomatik oynamýyoruz)
        if (IsValidMove(drawn))
        {
            Debug.Log("Çektiðin kart oynanabilir. Ýstersen oyna.");
            // oyuncu isterse týklayýp oynayacak; yoksa sýra biter
        }
        else
        {
            // oynanamazsa sýra bot'a geçer
            EndTurn();
        }
    }

    /// <summary>
    /// Sýra bitiþi: botun sýrasýysa bot turn coroutine'i baþlatýlýr.
    /// </summary>
    public void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        if (!isPlayerTurn)
        {
            // Bot turu coroutine ile çalýþsýn (bot.BotTurn IEnumerator olmalý)
            StartCoroutine(ExecuteBotTurn());
        }
        else
        {
            // oyuncu sýrasý baþladý
            Debug.Log("Sýra: Sen");
        }
    }

    IEnumerator ExecuteBotTurn()
    {
        // küçük bir bekleme ver (daha doðal görünür)
        yield return new WaitForSeconds(0.6f);

        // Bot kendi coroutine'ini çaðýr (eðer bot.BotTurn bir IEnumerator ise)
        if (bot != null)
        {
            yield return StartCoroutine(bot.BotTurn());
        }

        // BotTurn sonunda kontrol: bot script'i kart oynadýysa PlaceCardOnPile kullanmalý ve bot elinden çýkarmalýdýr.
        // Burada sadece sýrayý tekrar oyuncuya çeviriyoruz (bot kodu ayrýca EndTurn çaðýrmazsa)
        if (!isPlayerTurn)
        {
            isPlayerTurn = true;
            Debug.Log("Bot hamlesini tamamladý. Sýra: Sen");
        }
    }

    /// <summary>
    /// Oyun bitti mi diye kontrol eder.
    /// </summary>
    public void CheckGameOver()
    {
        if (player.hand.Count == 0)
        {
            Debug.Log("Oyuncu kazandý!");
            // isteðe baðlý: oyunu durdur, UI göster vs.
        }
        else if (bot.hand.Count == 0)
        {
            Debug.Log("Bot kazandý!");
        }
    }

    #region Deck / Discard Refill Helpers

    /// <summary>
    /// Eðer destede kart kalmadýysa discard pile içindeki kartlarý (topCard dýþýnda)
    /// gönderip tekrar karýþtýrarak desteyi doldurur.
    /// </summary>
    void RefillDeckFromDiscard()
    {
        // discardPileArea altýndaki tüm kartlarý topla (topCard dýþýndakiler)
        List<CardData> toReturn = new List<CardData>();
        // collect children as Cards — skip the one equal to topCard
        List<Transform> children = new List<Transform>();
        foreach (Transform t in discardPileArea) children.Add(t);

        foreach (var t in children)
        {
            Card c = t.GetComponent<Card>();
            if (c == null) continue;
            // skip current topCard (son oynanan)
            if (c == topCard) continue;

            // add card data
            toReturn.Add(c.data);

            // destroy visual (we'll re-create visuals when cards are drawn)
            Destroy(c.gameObject);
        }

        // If nothing to return, can't refill
        if (toReturn.Count == 0)
        {
            Debug.Log("Refill: Discard'dan geri koyulacak kart yok.");
            return;
        }

        // DeckManager'a ver (DeckManager'da AddCardsToBottom metodu olmalý)
        deckManager.AddCardsToBottom(toReturn);

        Debug.Log($"Refill: {toReturn.Count} kart discard'dan desteye eklendi ve karýþtýrýldý.");
    }

    #endregion
}
