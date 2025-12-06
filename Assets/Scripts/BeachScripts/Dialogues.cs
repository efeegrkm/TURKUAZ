using System.Collections.Generic;
using UnityEngine;

public class Dialogues : MonoBehaviour
{
    public static Dialogues Instance;
    private Queue<Dialogue> dialogueQueue = new();
    ScriptPrinter printer;
    private void Awake()
    {
        printer = ScriptPrinter.Instance;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void StartDialogue(string dialogueID)
    {
        Debug.Log($"Starting dialogue: {dialogueID}");
        switch(dialogueID)
        {
            case "Gorkem_LB":
                GorkemLeftBeachDial();
                break;
            case "ShowCaveToGorkem":
                ShowCaveToGorkem();
                break;
            default:
                Debug.LogWarning($"Dialogue ID {dialogueID} not found.");
                break;
        }
    }

    // --- DIALOGUE SYSTEM ---
    public void EnqueueDialogue(Dialogue d)
    {
        dialogueQueue.Enqueue(d);
    }

    public void EnqueueDialogueList(Dialogue[] dList)
    {
        foreach (var d in dList)
            dialogueQueue.Enqueue(d);
    }

    public void NextDialogue()
    {
        if (dialogueQueue.Count > 0)
        {
            Dialogue next = dialogueQueue.Dequeue();
            printer.PrintDialogue(next.speaker, next.content, 20, Color.white, 0.02f);
            CheckDialogueTriggeredAction(next);
        }
        else
        {
            printer.closeDialogue();
            printer.dialogueActive = false;
        }
    }

    private void CheckDialogueTriggeredAction(Dialogue dial)
    {
        if(dial.dialogueLocation == "GorkemLeftBeachDial")
        {
            //actions
        }
    }

    //Dialogues:
    private void GorkemLeftBeachDial()
    {
        Dialogue[] dList = new Dialogue[]
        {
            new Dialogue("GORKEM", "I guess it's time to head back to the van."),
            new Dialogue("GORKEM", "Esra must be waiting for me there."),
        };
        for(int i = 0; i < dList.Length; i++)
        {
            dList[i].assignIDLoc("GorkemLeftBeachDial", i);
        }
        EnqueueDialogueList(dList);
        NextDialogue();
    }
    private void ShowCaveToGorkem()
    {
        //fade in gorkem appear fade out.
        Dialogue[] dList = new Dialogue[]
        {
            new Dialogue("ESRA", "Look, Gorkem! There's a cave over there."),
            new Dialogue("GORKEM", "Hmm, interesting. Should we check it out?"),
        };
        for(int i = 0; i < dList.Length; i++)
        {
            dList[i].assignIDLoc("ShowCaveToGorkem", i);
        }
        EnqueueDialogueList(dList);
        NextDialogue();
    }
}
