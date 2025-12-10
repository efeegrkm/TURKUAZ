using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
public class Decidor
{
    public string question;
    public bool answered = false;
    public bool decision;
    private DecisionMaker dm;
    private string yesAction;
    private string noAction;
    
    public Decidor(string question, string yesAction, string noAction)
    {
        dm = DecisionMaker.Instance;
        this.question = question;
        this.yesAction = yesAction;
        this.noAction = noAction;
        dm.askQuestion(this);
    }
    public int getDecision()
    {
        if(answered)
        {
            return decision ? 1 : 0;
        }
        else
        {
            return -1;
        }
    }
    public void receiveAnswer(bool decision)
    {
        this.answered = true;
        this.decision = decision;
        string act = decision ? yesAction : noAction;
        performAction(act);
    }
    private void performAction(string action)
    {
        switch(action)
        {
            case "GO_BEACH_SCENE":
                SceneTransitionManager.Instance.SceneTransitionTo(1);
                break;
            case "GO_VAN_SCENE":
                SceneTransitionManager.Instance.SceneTransitionTo(0);
                B_ClickDetector.instance.NotifyActionFinished();
                break;
            case "GORKEM_SPEAK_LB":
                Dialogues.Instance.StartDialogue("Gorkem_LB1");
                B_ClickDetector.instance.NotifyActionFinished();
                break;
            case "GORKEM_SHOW_CAVE_RB":
                Dialogues.Instance.StartDialogue("ShowCaveToGorkem");
                ActionMethots.Instance.caveShown = true;
                B_ClickDetector.instance.NotifyActionFinished();
                break;
            case "METO":
                ScriptPrinter.Instance.PrintDialogue("ESRA", "Ben de seni cok seviyorum bebegim.", 20, Color.white, 0.02f);
                B_ClickDetector.instance.NotifyActionFinished();
                break;
            case "METOM":
                ScriptPrinter.Instance.PrintDialogue("ESRA", "Sana deli asigim ne sevmesi:)", 20, Color.white, 0.02f);
                B_ClickDetector.instance.NotifyActionFinished();
                break;
            case "NO_ACTION":
                if(B_ClickDetector.instance != null)
                {
                    B_ClickDetector.instance.NotifyActionFinished();
                }
                break;
        }
    }
    
}
