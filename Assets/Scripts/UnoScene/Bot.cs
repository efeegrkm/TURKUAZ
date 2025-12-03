using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//ilk kart number olmalý
//Bot skip ve revde tek kart atýp sýrayý veriyor.
//Bot +2 kartý atýnca player sýra onda olmadýðý için 2 kartý alamýyor.
public class Bot : MonoBehaviour
{
    public List<Card> hand = new List<Card>();
    public Transform handArea;

    public void DrawStartingCards()
    {
        for (int i = 0; i < 7; i++)
            DrawCard();
    }

    public void DrawCard()
    {
        Card card = UNOManager.Instance.deckManager.DrawCard();
        if (card == null)
        {
            Debug.LogWarning("Bot.DrawCard(): Deste boþ, kart çekilemedi.");
            return;
        }

        // görsel parent
        card.transform.SetParent(handArea, false);
        card.transform.localPosition = Vector3.zero;

        // Bot kartlarý týklanamaz olmalý
        card.SetInteractable(false);
        card.RefreshVisual();

        hand.Add(card);
    }

    public IEnumerator BotTurn()
    {
        // Bekleme süresi, gerçekçi görünmesi için
        yield return new WaitForSeconds(0.6f);

        // 1) Elden oynanabilir kart ara
        Card toPlay = null;
        foreach (var c in hand)
        {
            if (UNOManager.Instance.IsValidMove(c))
            {
                toPlay = c;
                break;
            }
        }

        if (toPlay != null)
        {
            // Bot oynuyor
            hand.Remove(toPlay);
            toPlay.SetInteractable(false);
            UNOManager.Instance.PlaceCardOnPile(toPlay);
            UNOManager.Instance.CheckGameOver();
            UNOManager.Instance.ExecuteOperation(toPlay.data.type);
            yield break; // bot hamlesi bitti (UNOManager üstten EndTurn çaðýracak)
        }

        // 2) Oynanacak kart yoksa desteden çek
        Card drawn = UNOManager.Instance.deckManager.DrawCard();
        if (drawn == null)
        {
            // deck boþsa refill veya pass (UNOManager.Re... sorumluluðunda)
            Debug.Log("Bot çekemedi: deste boþ.");
            yield break;
        }

        // görsel olarak bot eline koy
        drawn.transform.SetParent(handArea, false);
        drawn.transform.localPosition = Vector3.zero;
        drawn.SetInteractable(false);
        drawn.RefreshVisual();
        hand.Add(drawn);

        // Eðer çekilen kart oynanabiliyorsa bot hemen oynar
        if (UNOManager.Instance.IsValidMove(drawn))
        {
            // kýsa bekleme
            yield return new WaitForSeconds(0.4f);

            hand.Remove(drawn);
            drawn.SetInteractable(false);
            UNOManager.Instance.PlaceCardOnPile(drawn);
            UNOManager.Instance.CheckGameOver();
            UNOManager.Instance.ExecuteOperation(drawn.data.type);
            yield break;
        }

        // yoksa bot hamlesi bitti (UNOManager.EndTurn() çaðrýsýný caller yapmalý)
    }
}
