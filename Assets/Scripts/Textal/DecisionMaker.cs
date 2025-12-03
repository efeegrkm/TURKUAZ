using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
public class DecisionMaker : MonoBehaviour
{
    public static DecisionMaker Instance;
    public ClickDetector clickDetector;
    public AudioManager am;
    public AudioClip click;
    public TextMeshProUGUI questionTMP;
    public GameObject decisionPanel;
    public Button YesBut, NoBut;
    private Decidor curDecidor;
    private string decisionQuestion;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public void askQuestion(Decidor decidor)
    {
        clickDetector.clickable = false;
        curDecidor = decidor;
        this.decisionQuestion = decidor.question;
        questionTMP.text = decisionQuestion;
        openInteract();
        decisionPanel.SetActive(true);
    }
    public void yesPressed()
    {
        am.PlaySFX(click);
        shutInteract();
        curDecidor.receiveAnswer(true);
        closePanel();
    }
    public void noPressed()
    {
        am.PlaySFX(click);
        shutInteract();
        curDecidor.receiveAnswer(false);
        closePanel();
    }
    private void closePanel()
    {
        decisionPanel.SetActive(false);
        clickDetector.clickable = true;
    }
    private void shutInteract()
    {
        YesBut.interactable = false;
        NoBut.interactable = false;
    }
    private void openInteract()
    {
        YesBut.interactable = true;
        NoBut.interactable = true;
    }
}
