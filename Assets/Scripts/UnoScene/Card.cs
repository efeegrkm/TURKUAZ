using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Kart prefab'ýna eklenecek script.
/// - data: kartýn modeli
/// - buton: prefab üzerindeki Button (zorunlu)
/// - label: opsiyonel UI Text (kart üzerindeki sayý/isim için)
/// </summary>
[RequireComponent(typeof(Button))]
public class Card : MonoBehaviour
{
    public CardData data;

    [Header("Optional UI refs (assign on prefab)")]
    public Button button;      // Prefab'ta Button mutlaka olmalý
    private TextMeshProUGUI label;
    public GameObject cardObject;
    public Transform cardTransform;

    private void Awake()
    {
        label = this.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        cardObject = this.gameObject;
        cardTransform = cardObject.transform;
        // Eðer inspector'da button yoksa otomatik al
        if (button == null) button = GetComponent<Button>();

        // Güvenli listener yönetimi
        if (button != null)
        {
            button.onClick.RemoveListener(CallPlayCard);
            button.onClick.AddListener(CallPlayCard);
        }

        // Ýlk görsel güncellemesi (data varsa)
        RefreshVisual();
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(CallPlayCard);
    }

    /// <summary>
    /// Baþlangýç/çekme sýrasýnda çaðýr: data zaten DeckManager tarafýndan set edilmiþ olabilir.
    /// interactable: bu kartýn týklanýp týklanamayacaðýný belirler (bot kartlarý false olmalý).
    /// </summary>
    public void Initialize(CardData cardData, bool interactable = true)
    {
        data = cardData;
        RefreshVisual();
        SetInteractable(interactable);
    }

    /// <summary>
    /// Label vb. görselleri data'ya göre günceller.
    /// Opsiyonel; prefab'ýnda label yoksa çalýþmaz ama güvenlidir.
    /// </summary>
    public void RefreshVisual()
    {
        switch (data.color) { 
            case CardColor.Red:
                cardObject.GetComponent<Image>().color = Color.red;
                break;
            case CardColor.Green:
                cardObject.GetComponent<Image>().color = Color.green;
                break;
            case CardColor.Blue:
                cardObject.GetComponent<Image>().color = Color.blue;
                break;
            case CardColor.Yellow:
                cardObject.GetComponent<Image>().color = Color.yellow;
                break;
            default:
                cardObject.GetComponent<Image>().color = Color.white;
                break;
        }
        if (label == null) return;
        if (data == null) { label.text = ""; return; }

        if (data.type == CardType.Number)
            label.text = data.number.ToString();
        else
            switch (data.type.ToString()) {    
                case "Skip":
                    label.text = "Skip";
                    break;
                case "Reverse":
                    label.text = "Rev";
                    break;
                case "DrawTwo":
                    label.text = "+2";
                    break;
                case "Wild":
                    label.text = "W";
                    break;
                case "WildDrawFour":
                    label.text = "+4";
                    break;
                default:
                    label.text = "";
                    break;
            }
    }

    /// <summary>
    /// Buton týklanabilirliðini ayarlar (bot kartlarý için false).
    /// </summary>
    public void SetInteractable(bool interactable)
    {
        if (interactable)
        {
            button.enabled = true;
        }
        else
        {
            button.enabled= false;
        }
    }

    /// <summary>
    /// Butona basýlýnca çaðrýlýr. UNOManager.PlayCard() içeriði doðrular.
    /// </summary>
    public void CallPlayCard()
    {
        // Safety: sadece button týklanmasý ile geliyorsa buton zaten interactable olmalý.
        if (UNOManager.Instance == null)
        {
            Debug.LogWarning("UNOManager yok, CallPlayCard() geçersiz.");
            return;
        }

        UNOManager.Instance.PlayCard(this);
    }
}
