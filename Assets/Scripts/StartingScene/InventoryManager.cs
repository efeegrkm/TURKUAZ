using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
//Smoke pixelize.
[Serializable]
public class InventoryItem
{
    public string itemName;
    public GameObject iconPrefab;
    // Inspector'dan doldurulabilmesi için boþ ctor yeterli
}
[Serializable]
public class Slot
{
    [HideInInspector] public int index; // runtime için (opsiyonel)
    public InventoryItem item;          // null ise boþ
    public Button button;               // UI buton (inspectordan baðla)
    public Transform iconTransform;             // slot iconunu gösterecek Image (inspectordan baðla)
    public GameObject selectionHighlight; // seçili gösterge (opsiyonel, inspectordan baðla)
    private GameObject currentIconObject;

    // slot UI'ýný güncelle
    public void RefreshUI()
    {
        if (iconTransform != null)
        {
            if (item != null && item.iconPrefab != null)
            {
                UnityEngine.Object.Destroy(currentIconObject);
                currentIconObject = UnityEngine.Object.Instantiate(item.iconPrefab, iconTransform.position, item.iconPrefab.transform.rotation, iconTransform);
            }
            else
            {
                UnityEngine.Object.Destroy(currentIconObject);
            }
        }
    }

    public bool IsEmpty() => (item==null || item.itemName==null || item.itemName.Equals(""));

    public void SetItem(InventoryItem newItem)
    {
        item = newItem;
        RefreshUI();
    }

    public void ClearItem()
    {
        item = null;
        RefreshUI();
    }
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public GameObject inventoryUI;
    [Header("Slots (Inspector: set size = 10 ve her slot için Button/Image baðla)")]
    public Slot[] slots = new Slot[10];

    [Header("Item pool — inspector'dan ekleyebileceðin örnek itemler")]
    public InventoryItem[] itemPool;

    [Tooltip("Parametresiz AddToInventory çaðrýldýðýnda hangi item eklenecek (itemPool index'i)")]
    public int defaultItemToAddIndex = 0;

    [Header("Seçili Slot")]
    public Slot selectedSlot; // runtime güncellenir

    [SerializeField]
    private AudioClip woodTap;
    public AudioManager am;
    [SerializeField] Image FGL;
    [SerializeField] Image FGR;
    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(inventoryUI);
    }
    void Start()
    {
        indexArrangement();
    }
    public void indexArrangement()
    {
        // slot indekslerini ayarla ve butonlara listener ekle
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;

            slots[i].index = i;
            // Güvenli referans yakalama için local deðiþken
            int idx = i;
            if (slots[i].button != null)
            {
                // Önce varsa eski listener'larý kaldýr
                slots[i].button.onClick.RemoveAllListeners();
                slots[i].button.onClick.AddListener(() => SelectSlot(idx));
            }

            // Ýlk UI durumunu güncelle
            slots[i].RefreshUI();

            // ensure selection highlight off initially
            if (slots[i].selectionHighlight != null)
                slots[i].selectionHighlight.SetActive(false);
            ClearAllSlots();
        }
    }
    // Inspector'daki slot butonlarýna basýldýðýnda çaðrýlýr (veya runtime çaðýr)
    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return;
        selectedSlot = slots[index];
        UpdateSelectionUI();
        am.PlaySFX(woodTap);
    }

    // Tüm slotlarýn selection highlight'ýný güncelle
    public void UpdateSelectionUI()
    {
        foreach (var s in slots)
        {
            if (s == null) continue;
            if (s.selectionHighlight != null)
                s.selectionHighlight.SetActive(s == selectedSlot);
        }
    }

    // Parametresiz çaðrý default itemPool[index] ekler
    public void AddToInventory()
    {
        AddToInventory(defaultItemToAddIndex);
    }

    // itemPool içinden index ile ekleme (UI otomatik güncellenir)
    public void AddToInventory(int itemPoolIndex)
    {
        if (itemPool == null || itemPool.Length == 0)
        {
            Debug.LogWarning("InventoryManager: itemPool boþ.");
            return;
        }

        if (itemPoolIndex < 0 || itemPoolIndex >= itemPool.Length)
        {
            Debug.LogWarning("InventoryManager: geçersiz itemPoolIndex.");
            return;
        }

        InventoryItem toAdd = itemPool[itemPoolIndex];
        if (toAdd == null)
        {
            Debug.LogWarning("InventoryManager: seçilen item null.");
            return;
        }

        // Ýlk boþ slotu bul
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;
            if (slots[i].IsEmpty())
            {
                slots[i].SetItem(toAdd);
                Debug.Log($"Added '{toAdd.itemName}' to slot {i}.");
                return;
            }
        }

        Debug.Log("Inventory is full. No empty slots.");
    }

    // Direkt belirli bir slot indexine item eklemek istersen:
    public bool AddToSlot(int slotIndex, InventoryItem item)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return false;
        if (slots[slotIndex] == null) return false;

        if (slots[slotIndex].IsEmpty())
        {
            slots[slotIndex].SetItem(item);
            return true;
        }

        return false; // dolu
    }

    // Seçili slottaki item'i kullan/sil
    public void UseSelectedSlot()
    {
        if (selectedSlot == null)
        {
            Debug.Log("No slot selected.");
            return;
        }

        if (selectedSlot.IsEmpty())
        {
            Debug.Log("Selected slot is empty.");
            return;
        }

        // Burada item kullaným mantýðýný (effect/consume vs equip) ekleyebilirsin.
        Debug.Log($"Using item '{selectedSlot.item.itemName}' from selected slot.");
        // Kullanýldýktan sonra sil:
        selectedSlot.ClearItem();
    }

    // Yardýmcý debug: tüm slotlarý temizle
    [ContextMenu("ClearAllSlots")]
    public void ClearAllSlots()
    {
        foreach (var s in slots)
            s?.ClearItem();
    }

    public void HighlightFG()
    {
        FGL.enabled = false;
        FGR.enabled = false;
    }
    public void LowlightFG()
    {
        FGL.enabled = true;
        FGR.enabled = true;
    }

    public bool UseItemWith(int inventoryPoolIndex)
    {
        if (inventoryPoolIndex < 0 || inventoryPoolIndex >= itemPool.Length)
        {
            Debug.LogWarning("InventoryManager: Geçersiz itemPoolIndex sorgusu.");
            return false;
        }
        InventoryItem targetItem = itemPool[inventoryPoolIndex];
        if (targetItem == null) return false;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && !slots[i].IsEmpty())
            {
                if (slots[i].item.itemName == targetItem.itemName)
                {
                    Debug.Log($"Auto-used item: {targetItem.itemName} from slot {i}");

                    slots[i].ClearItem();
                    return true;
                }
            }
        }
        return false;
    }
    public bool HasItem(int inventoryPoolIndex)
    {
        if (inventoryPoolIndex < 0 || inventoryPoolIndex >= itemPool.Length)
        {
            Debug.LogWarning("InventoryManager: Geçersiz itemPoolIndex sorgusu.");
            return false;
        }
        InventoryItem targetItem = itemPool[inventoryPoolIndex];
        if (targetItem == null) return false;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && !slots[i].IsEmpty())
            {
                if (slots[i].item.itemName == targetItem.itemName)
                {
                    return true;
                }
            }
        }
        return false;
    }
}

