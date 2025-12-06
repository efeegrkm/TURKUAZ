using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class B_ClickDetector : MonoBehaviour
{
    public static B_ClickDetector instance;
    public bool[] decisionFlags = new bool[10];
    [System.Serializable]
    public class ClickableZone
    {
        public string name;
        public Collider2D collider;
        public UnityEvent action;
    }

    [Header("Settings")]
    [SerializeField] private bool isInputActive = true;

    [Header("Clickable Objects")]
    [SerializeField] private List<ClickableZone> clickableZones = new();

    
    private ScriptPrinter printer;
    private InventoryManager im;

    private Camera mainCamera;

    public bool actionInProgress = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        im = InventoryManager.Instance;
        printer = ScriptPrinter.Instance;
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!isInputActive) return;   

        if (Input.GetMouseButtonDown(0))
        {
            if (printer.dialogueActive)
            {
                handleDecisionFlags();
                if (!printer.dialogueWriting)
                    Dialogues.Instance.NextDialogue();
                return;
            }
            if (actionInProgress) return;
            DetectClick();
        }
    }
    private void handleDecisionFlags()
    {
        for (int i = 0; i < decisionFlags.Length; i++)
        {
            if (decisionFlags[i])
            {
                decisionFlags[i] = false;
                switch(i)
                {
                    case 0:
                        new Decidor("Gorkeme seslen.", "GORKEM_SPEAK_LB", "NO_ACTION");
                        break;
                    case 1:
                        new Decidor("Gorkem'i cagir.", "GORKEM_SHOW_CAVE_RB", "NO_ACTION");
                        break;
                }

            }
        }
    }
    private void DetectClick()
    {
        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        foreach (var zone in clickableZones)
        {
            if (zone.collider != null && zone.collider.OverlapPoint(worldPoint))
            {
                StartCoroutine(HandleZoneAction(zone.action));
                return;
            }
        }
        im.selectedSlot = null;
        im.UpdateSelectionUI();
    }

    // ACTION WRAPPER — bütün actionlar buradan geçer
    private IEnumerator HandleZoneAction(UnityEvent action)
    {
        actionInProgress = true;

        // Action bir coroutine tetikliyorsa coroutine bitene kadar beklemek için:
        // UnityEvent maalesef direkt coroutine döndüremez,
        // bu yüzden kullanıcı coroutinelerini ActionManager gibi dış scriptlerde koşturur.
        // Burada sadece küçük bir gecikme laiss edelim (opsiyonel).
        // Eğer action coroutine tetikletiyorsa OnActionFinished() manuel çağrılacak.

        action.Invoke();

        yield return null;
    }

    // DIŞ SCRIPTLERİN ÇAĞIRABİLECEĞİ "Action Bitti" FUNCT
    public void NotifyActionFinished()
    {
        actionInProgress = false;
    }
    public void SetInputState(bool active)
    {
        isInputActive = active;
    }
}
