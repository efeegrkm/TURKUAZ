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
                Debug.Log("Gorkem_LB Dialogue triggered");
                //Dialogues.Instance.StartDialogue("Gorkem_LB");
                B_ClickDetector.instance.NotifyActionFinished();
                break;
            case "NO_ACTION":
                B_ClickDetector.instance.NotifyActionFinished();
                break;
        }
    }
    
}
