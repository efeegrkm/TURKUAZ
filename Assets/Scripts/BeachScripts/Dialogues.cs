using System.Collections.Generic;
using UnityEngine;

public class Dialogues : MonoBehaviour
{
    public static Dialogues Instance;
    private Queue<Dialogue> dialogueQueue = new();
    ScriptPrinter printer;

    //det
    private bool det1 = true;
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
            case "Gorkem_LB1":
                GorkemLeftBeachDial1();
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
        if(dial.dialogueLocation == "GorkemLeftBeachDial1")
        {
            if(dial.dialogueNumber == 33) 
            {
                ActionMethots.Instance.actionLevel = 1;
            }
        }
        if(dial.dialogueLocation == "ShowCaveToGorkem")
        {
            if(dial.dialogueNumber == 1) 
            {
                ActionMethots.Instance.actionLevel = 2;
            }
        }
        if(dial.dialogueLocation == "askGorkemForIgnite")
        {
            if(dial.dialogueNumber == 24)
            {
                ActionMethots.Instance.actionLevel = 4;
            }
        }
        if(dial.dialogueLocation == "extra")
        {
            if(dial.dialogueNumber == 5)
            {
                B_ClickDetector.instance.extra = true;
            }
        }
    }

    //Dialogues:
    private void GorkemLeftBeachDial1()
    {
        Dialogue[] dList = new Dialogue[]
        {
            new Dialogue("Pisst Gorkem!!!", "ESRA"),
            new Dialogue("ASKIMM bak ne buldum!!!", "GORKEM"),
            new Dialogue("Kimin ki acaba burada durmasi cok garip", "GORKEM"),
            new Dialogue("Ya bebem ellesmesene her gordugun seye...", "ESRA"),
            new Dialogue("Kamp falan yapýodur belki birileri gelmistir oturma ustune.", "ESRA"),
            new Dialogue(":(", "GORKEM"),
            new Dialogue("Sanmiyorum çok eski duruyor gel tahtalarina baksana.", "GORKEM"),
            new Dialogue("Hmm", "ESRA"),
            new Dialogue("En yakin köye 60 km uzaktayiz ve ayrica köyde mola verdigimizde kahveci burayla ilgili bir seyler anlatmisti...", "GORKEM"),
            new Dialogue("Buralardan eskiden kuvars çýkartiliyormus. Balikcilik da eskiden bi okadar yayginmis bu civarda.", "GORKEM"),
            new Dialogue("Dedigine gore zamanla madenciler arasinda korku salan bir soylenti ortaya cikmis.", "GORKEM"),
            new Dialogue("Gece vardiyasina kalan madenciler kuvarslarin yagmalandigi maden duvarlarina hintce harfler oyulduguna dair bazý hikayeler anlatmaya baslamis...", "GORKEM"),
            new Dialogue("Hepsinin agzýnda tek bir isim...", "GORKEM"),
            new Dialogue("iri yapili bir korsan...", "GORKEM"),
            new Dialogue("RAAT-KA-RAJ!!!", "GORKEM"),
            new Dialogue("Senin ruyanin nerden esinlendigi belli oldu.", "ESRA"),
            new Dialogue("...", "GORKEM"),
            new Dialogue("Biraz etkilenmis olabilrm...", "GORKEM"),
            new Dialogue("Her neyse 50 yillik hikaye. Sonuc olarak ozamandan beri buralar terkedilmis durumda.", "GORKEM"),
            new Dialogue("Bu da muhtemelen ozamanlardan kalma. Yani diyecegim o ki...", "GORKEM"),
            new Dialogue("Hayýr...", "ESRA"),
            new Dialogue("Ya nolcak bi tur atsak söz simdi bakýcam her yerine delik melik var mý?", "GORKEM"),
            new Dialogue("Yani simdi haklisin birine ait olmadigi kesin terkedildigi ortada.", "ESRA"),
            new Dialogue("Ve...", "ESRA"),
            new Dialogue("Ve:)", "GORKEM"),
            new Dialogue("Ben de sakin denizde yildizlari izledigim bir aksamimiz olsun isterdim. En azindan bitane alcak para birikene kadar...", "ESRA"),
            new Dialogue("Seni cok seviyorum :)", "GORKEM"),
            new Dialogue("Ben de seni seviyorum canim :)", "ESRA"),
            new Dialogue("Ama ufak bi sorunumuz var...", "GORKEM"),
            new Dialogue("Teknenin kurekleri yok.", "GORKEM"),
            new Dialogue("...", "ESRA"),
            new Dialogue("La havle", "ESRA"),
            new Dialogue("Sahilin devamina bir goz atip geliyorum belki ise yarar biseyler buluruz. Sen de saglam mi bak orasina burasina teknenin.", "ESRA"),
            new Dialogue("Tamam canim dikkatli ol. Cagir hemen bir sey gorursen","GORKEM")
        };
        for(int i = 0; i < dList.Length; i++)
        {
            dList[i].assignIDLoc("GorkemLeftBeachDial1", i);
        }
        EnqueueDialogueList(dList);
        NextDialogue();
    }
    private void ShowCaveToGorkem()
    {
        //fade in gorkem appear fade out.
        Dialogue[] dList = new Dialogue[]
        {
            new Dialogue("OHA...", "GORKEM"),
            new Dialogue("Finallerden sonra cagrina icabet edip sana hikayeler anlatacagim ama kisaca...", "GORKEM"),
            new Dialogue("Iceriyi aydinlatacak biseyimiz olsa super olurdu. Etraftan ise yarar biseyler bulabilir miyiz bakalim.", "GORKEM"),
        };
        for(int i = 0; i < dList.Length; i++)
        {
            dList[i].assignIDLoc("ShowCaveToGorkem", i);
        }
        EnqueueDialogueList(dList);
        NextDialogue();
    }
    public void extraDialogue()
    {
        Dialogue[] dList = new Dialogue[]
        {
            new Dialogue("Cakmagi buldum bebegim gel hadi mesaleyi yakip magarayi kesfedelim...", "ESRA"),
            new Dialogue("Askim...", "GORKEM"),
            new Dialogue("Kotu bi haberim var...", "GORKEM"),
            new Dialogue("Maalesef finallerim bitmeden magarayi kesfedip antagonistimizle tanisamayacagiz...", "GORKEM"),
            new Dialogue("Buraya kadar oynadigin icin tesekkur ederim mesaleyi yakip diyaloglari kesfedebilirsin ama magara hazýr degil mlsf", "GORKEM"),
            new Dialogue("Seni cok seviyorum bebegim nice yillarimiz olsun:)", "GORKEM"),
        };
        for (int i = 0; i < dList.Length; i++)
        {
            dList[i].assignIDLoc("extra", i);
        }
        EnqueueDialogueList(dList);
        NextDialogue();
    }
    public void askIfFoundSmthng()
    {
        if(det1 == true)
        {
            det1 = false;
            Dialogue[] dList = new Dialogue[]
            {
                new Dialogue("Botun durumu iyi gozukuyor sende durumlar nasýl?", "GORKEM"),
                new Dialogue("Magaraya bu durumda goz atmak pek mumkun gozukmuyor.", "ESRA"),
                new Dialogue("Neyse kumsala uzanýp izleriz en kotu daha gece uzun...", "GORKEM"),
                new Dialogue("Pof", "ESRA"),
            };
            for (int i = 0; i < dList.Length; i++)
            {
                dList[i].assignIDLoc("askIfFoundSomething", i);
            }
            EnqueueDialogueList(dList);
            NextDialogue();
        }
        else
        {
            printer.PrintDialogue("GORKEM", "Kumsala oturup izleyedebiliriz daha gece uzun:)");
        }
    }
    public void askGorkemForIgnite()
    {
            Dialogue[] dList = new Dialogue[]
            {
                new Dialogue("Durumlar nasil?", "ESRA"),
                new Dialogue("Valla biraz yama yapmam gerekti onun dýsýnda gayet saglam duruyor", "GORKEM"),
                new Dialogue("Sen naptin bulabildin mi iþe yarar biseyler?", "GORKEM"),
                new Dialogue("Ne demek yama yapmam gerekti delik mi vardi", "ESRA"),
                new Dialogue("Olm su sakasi yapma bana vururum seni söle bsi varsa", "ESRA"),
                new Dialogue("dkfkdsjkjkd dalga geciom", "GORKEM"),
                new Dialogue("Neyse", "ESRA"),
                new Dialogue("Mesale yaptim ben de.", "ESRA"),
                new Dialogue("...", "GORKEM"),
                new Dialogue("MESALE MI YAPTIN???!!!", "GORKEM"),
                new Dialogue("VAY ANASINI... Lara Croft musun bebem sen!!!", "GORKEM"),
                new Dialogue("Yapioz bsiler", "ESRA"),
                new Dialogue("Ama mesaleyi tutusturacak bisey lazim.", "ESRA"),
                new Dialogue("Eeee...", "GORKEM"),
                new Dialogue("Sey...", "GORKEM"),
                new Dialogue("Noldu?", "ESRA"),
                new Dialogue("Cadirda cakmak vardi senin tulumun oralarda oynuyordum...", "GORKEM"),
                new Dialogue("Gecen hafta kaybettiydim.", "GORKEM"),
                new Dialogue("Canimin ici mesale yaptim MEÞALE cakmagimiz mi vardi gercekten basindan beri neden cakmakla bakivermedik iceriye...", "ESRA"),
                new Dialogue("Unuttum pardun:(", "GORKEM"),
                new Dialogue("...", "ESRA"),
                new Dialogue("Alistim ya.", "ESRA"),
                new Dialogue("Neyse mesaleyle magara gezmek daha havali dfjdsjkfdskfsd", "ESRA"),
                new Dialogue("Cadirin orda mi kaybettim dedin", "ESRA"),
                new Dialogue("Evet cadirda senin tulumunun orlarda oynamistim en son", "GORKEM"),
            };
            for (int i = 0; i < dList.Length; i++)
            {
                dList[i].assignIDLoc("askGorkemForIgnite", i);
            }
            EnqueueDialogueList(dList);
            NextDialogue();
        
    }
}
