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
        public List<int> actionAccaptenceLevels = new();
    }

    [Header("Settings")]
    [SerializeField] private bool isInputActive = true;

    [Header("Clickable Objects")]
    [SerializeField] private List<ClickableZone> clickableZones = new();

    
    private ScriptPrinter printer;
    private InventoryManager im;

    private Camera mainCamera;

    public bool actionInProgress = false;
    public bool extra = false;

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
                if(printer.dialogueWriting)
                {
                    printer.SkipTyping();
                }
                else
                    Dialogues.Instance.NextDialogue();
                return;
            }
            if (actionInProgress) return;
            DetectClick();
        }
    }
    private void handleDecisionFlags()
    {
        if(extra)
        {
            extra = false;
            new Decidor("Ben de seni seviyorum canim :)", "METO", "METOM");
            ActionMethots.Instance.extraFinish = true;
            return;
        }
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
                if(isActionPermited(zone.actionAccaptenceLevels))
                    StartCoroutine(HandleZoneAction(zone.action));
                return;
            }
        }
        cleanInventorySelection();
    }
    private bool isActionPermited(List<int> actionLevels)
    {
        int currentLevel = ActionMethots.Instance.actionLevel;
        if (actionLevels == null)
        {
            Debug.Log("Null action level.");
            return false;
        }
        if (actionLevels.Count == 0)
        {
            Debug.Log("No permited action level.");
            return false;
        }
        foreach(int level in actionLevels)
        {
            if (level == currentLevel)
                return true;
        }
        return false;
    }
    // ACTION WRAPPER — bütün actionlar buradan geçer
    private IEnumerator HandleZoneAction(UnityEvent action)
    {
        actionInProgress = true;

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
    private void cleanInventorySelection()
    {
        im.selectedSlot = null;
        im.UpdateSelectionUI();
    }
}
