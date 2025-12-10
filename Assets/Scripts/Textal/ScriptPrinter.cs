using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScriptPrinter : MonoBehaviour
{
    public static ScriptPrinter Instance;
    [SerializeField] GameObject Dialogue;
    [SerializeField] Image IconImage;
    [SerializeField] Sprite GorkemIcon;
    [SerializeField] Sprite SystemIcon;
    [SerializeField] Sprite EsraIcon;
    [SerializeField] private TextMeshProUGUI TextSpace;
    public float textTypeInterval = 0.02f; 
    private string dialogueText;
    private int fontSize = 20;
    private Color fontColor = Color.white;

    public bool dialogueActive = false;
    public bool dialogueWriting = false;

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public void PrintDialogue(string speaker, string dialogue, int fontSize, Color fontColor, float textTypeInterval)
    {
        this.fontColor = fontColor;
        this.fontSize = fontSize;
        this.textTypeInterval = textTypeInterval;
        dialogueText = dialogue;
        Dialogue.SetActive(true);
        assignSpeakerIcon(speaker);
        StopAllCoroutines(); 
        StartCoroutine(typeDialogue());
    }
    public void PrintDialogue(string speaker, string dialogue)
    {
        dialogueText = dialogue;
        Dialogue.SetActive(true);
        assignSpeakerIcon(speaker);
        StopAllCoroutines();
        StartCoroutine(typeDialogue());
    }
    private void assignSpeakerIcon(string speaker)
    {
        switch (speaker)
        {
            case "GORKEM":
                IconImage.sprite = GorkemIcon;
                break;
            case "SYSTEM":
                IconImage.sprite = SystemIcon;
                break;
            case "ESRA":
                IconImage.sprite = EsraIcon;
                break;
        }
    }

    private IEnumerator typeDialogue()
    {
        TextSpace.color = fontColor;
        TextSpace.fontSize = fontSize;
        TextSpace.text = "";

        dialogueWriting = true;
        foreach (char letter in dialogueText)
        {
            TextSpace.text += letter;
            yield return new WaitForSeconds(textTypeInterval); 
        }
        dialogueWriting = false;
        dialogueActive = true;
    }
    public void SkipTyping()
    {
        StopAllCoroutines();
        TextSpace.text = dialogueText;
        dialogueWriting = false;
    }
    public void closeDialogue()
    {
        Dialogue.SetActive(false);
    }
}
