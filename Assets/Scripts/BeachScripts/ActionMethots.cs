using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ActionMethots : MonoBehaviour
{
    public static ActionMethots Instance;
    private DecisionMaker dm;
    private SceneTransitionManager stm;
    private B_ClickDetector cd;
    private BeachManager bm;
    private ScriptPrinter sp;
    private int beachStickCount = 0;
    [SerializeField] GameObject clothObject;
    [SerializeField] GameObject stick1;
    [SerializeField] GameObject stick2;
    [SerializeField] GameObject TorchHandle;
    [SerializeField] GameObject TorchHandleCloth;
    [SerializeField] GameObject TorchLight;
    [SerializeField] GameObject gorkemLookingCave;
    bool TorchLit = false;
    bool handlePutted = false;
    bool clothPutted = false;
    public bool caveShown = false;
    bool onRightScene = false;

    [SerializeField] GameObject RightShade;
    //SFX
    [SerializeField] AudioClip clickSFX;
    AudioManager am => AudioManager.Instance;
    public int actionLevel = 0;
    public bool extraFinish = false;
    public void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        sp = ScriptPrinter.Instance;
        dm = DecisionMaker.Instance;
        bm = BeachManager.Instance;
        cd = B_ClickDetector.instance;
        stm = SceneTransitionManager.Instance;
    }
    // --------Action Methots-----------
    private void GoToVanScene()
    {
        Decidor decidor = new Decidor("Kayalýklara geri gidilsin mi?", "GO_VAN_SCENE", "NO_ACTION");
        clickSound();
    }
    //-------------------------------
    public void interactLGork()
    {
        if(extraFinish)
        {
            sp.PrintDialogue("GORKEM", "Finallerden sonra gorusuruz bebegim umarim guzel gecer ikimizin de sýnavlari:)", 20, Color.white, 0.015f);
            actionFinishedSignal();
            return;
        }
        if (actionLevel == 0)
        {
            cd.decisionFlags[0] = true;
            sp.PrintDialogue("ESRA", "Bu bakisi nerde gorsem tanirim yine biseylere heveslenmis... Hadi hayirlisi.", 20, Color.white, 0.015f);
        }
        else if (actionLevel == 1)
        {
            sp.PrintDialogue("GORKEM", "Simdilik saglam gozukuyor...", 20, Color.white, 0.015f);
            actionFinishedSignal();
        }
        else if (actionLevel == 2)
        {
            Dialogues.Instance.askIfFoundSmthng();
            actionFinishedSignal();
        }
        else if (actionLevel == 3)
        {
            Dialogues.Instance.askGorkemForIgnite();
            actionFinishedSignal();
        }
        else if(actionLevel == 4)
        {
            sp.PrintDialogue("GORKEM", "Cadirda, senin tulumunun uzerinde oynamistim en son:(");
            actionFinishedSignal();
        }
        else if(actionLevel == 5)
        {
            Dialogues.Instance.extraDialogue();
            actionFinishedSignal();
        }
    }
    //-------------------------------
    public void interactShoreBoat()
    {
        if(actionLevel==0)
        {
            sp.PrintDialogue("ESRA", "Sahile vurmus bir balikci teknesi. Buraya nasil geldi acaba?", 20, Color.white, 0.015f);
            actionFinishedSignal();
            return;
        }
        else if(actionLevel==1)
        {
            sp.PrintDialogue("ESRA", "Muhtemelen citir hasarli kureksiz bir balikci teknesi.d", 20, Color.white, 0.015f);
            actionFinishedSignal();
            return;
        }
        else if(actionLevel == 2 || actionLevel == 3|| actionLevel == 4)
        {
            sp.PrintDialogue("ESRA", "Kurekleri eksik bir balikci teknesi Gorkem güvenli olup olmadýgýný kontrol ediyor...", 20, Color.white, 0.015f);
            actionFinishedSignal();
            return;
        }
            actionFinishedSignal();
    }
    //-------------------------------
    public void goToRightBeach()
    {
        if (actionLevel == 0)
        {
            sp.PrintDialogue("ESRA", "Sahilin devamina ilerlemeden Gorkemle konussam iyi olur.", 20, Color.white, 0.015f);
            actionFinishedSignal();
            return;
        }
        StartCoroutine(gtrbHelper());
    }
    public IEnumerator gtrbHelper()
    {
        Debug.Log("Going to right beach");
        stm.fadeIn();
        yield return new WaitForSeconds(stm.transitionDuration);
        bm.activeBeachPart('R');
        stm.fadeOut();
        yield return new WaitForSeconds(stm.transitionDuration);
        actionFinishedSignal();
        RightShade.SetActive(false);
        onRightScene = true;
        yield break;
    }
    //-------------------------------
    public void handleLeftTransition()
    {
        if (onRightScene)
        {
            goToLeftBeach();
        }
        else
        {
            GoToVanScene();
        }
    }
    private void goToLeftBeach()
    {
        StartCoroutine(gtlbHelper());
    }
    private IEnumerator gtlbHelper()
    {
        Debug.Log("Going to left beach");
        stm.fadeIn();
        yield return new WaitForSeconds(stm.transitionDuration);
        bm.activeBeachPart('L');
        stm.fadeOut();
        yield return new WaitForSeconds(stm.transitionDuration);
        actionFinishedSignal();
        RightShade.SetActive(true);
        onRightScene = false;
        yield break;
    }
    //-------------------------------
    public void collectStickL()
    {
        clickSound();
        stick1.SetActive(false);
        beachStickCount++;
        if (beachStickCount == 1)
        {
            ScriptPrinter.Instance.PrintDialogue("ESRA", "Bir tane odun parçasý buldum. Bir tane daha bulursam mesale sapý olarak kullanabiliriz.", 20, Color.white, 0.015f);
            InventoryManager.Instance.AddToInventory(4);
        }
        else if (beachStickCount == 2)
        {
            ScriptPrinter.Instance.PrintDialogue("SYSTEM", "Kalin mesale sapi uretildi.", 20, Color.white, 0.015f);
            InventoryManager.Instance.UseItemWith(4);
            InventoryManager.Instance.AddToInventory(5);
        }
        else
        {
            ScriptPrinter.Instance.PrintDialogue("ESRA", "Baþka odun parçasýna gerek yok sanýrým.", 20, Color.white, 0.015f);
        }
        actionFinishedSignal();
    }
    //-------------------------------
    public void collectStickR()
    {
        clickSound();
        stick2.SetActive(false);
        beachStickCount++;
        if (beachStickCount == 1)
        {
            ScriptPrinter.Instance.PrintDialogue("ESRA", "Bir tane odun parçasý buldum. Bir tane daha bulursam mesale sapý olarak kullanabiliriz.", 20, Color.white, 0.015f);
            InventoryManager.Instance.AddToInventory(4);
        }
        else if (beachStickCount == 2)
        {
            ScriptPrinter.Instance.PrintDialogue("SYSTEM", "Kalin mesale sapi uretildi.", 20, Color.white, 0.015f);
            InventoryManager.Instance.UseItemWith(4);
            InventoryManager.Instance.AddToInventory(5);
        }
        else
        {
            ScriptPrinter.Instance.PrintDialogue("ESRA", "Baþka odun parçasýna gerek yok sanýrým.", 20, Color.white, 0.015f);
        }
        actionFinishedSignal();
    }
    public void collectCloth()
    {
        clickSound();
        clothObject.SetActive(false);
        ScriptPrinter.Instance.PrintDialogue("ESRA", "Bez isimize yarayabilir.", 20, Color.white, 0.015f);
        InventoryManager.Instance.AddToInventory(6);
        actionFinishedSignal();
    }
    public void torchSlot()
    {
        if(actionLevel==1)
        {
            ScriptPrinter.Instance.PrintDialogue("ESRA", "Mesale koymak icin yapilmis gibi duruyor.", 20, Color.white, 0.015f);
            actionFinishedSignal();
            return;
        }
        else if(actionLevel == 4)
        {
            ScriptPrinter.Instance.PrintDialogue("ESRA", "Cadirdaki cakmagi bulabilirsem yakabilirim sanýrým.", 20, Color.white, 0.015f);
            actionFinishedSignal();
            return;
        }
            bool hasStick = InventoryManager.Instance.HasItem(4);
        bool hasThickStick = InventoryManager.Instance.HasItem(5);
        bool hasCloth = InventoryManager.Instance.HasItem(6);
        bool hasLighter = InventoryManager.Instance.HasItem(7);

        if (TorchLit)
        {
            ScriptPrinter.Instance.PrintDialogue("ESRA", "Bitecegi yok, baya bi bizi goturur.", 20, Color.white, 0.015f);
            actionFinishedSignal();
            return;
        }
        else if (clothPutted)
        {
            if(hasLighter)
            {
                if (InventoryManager.Instance.selectedSlot != null && InventoryManager.Instance.selectedSlot.item != null &&
                    InventoryManager.Instance.selectedSlot.item.itemName != null && InventoryManager.Instance.selectedSlot.item.itemName.Equals("Lighter"))
                {
                    clickSound();
                    InventoryManager.Instance.UseSelectedSlot();
                    TorchLight.SetActive(true);
                    ScriptPrinter.Instance.PrintDialogue("ESRA", "Mesalemiz hazir. Magaraya girebiliriz... Derrdim amma Gorkemin finalleri bitsin sonra girelim hic zahmet etmiyim cagirmaya.", 20, Color.white, 0.015f);

                    TorchLit = true;
                    actionFinishedSignal();
                    return;
                }
                else
                {
                    ScriptPrinter.Instance.PrintDialogue("ESRA", "Yanmaya hazýr bir meþale...", 20, Color.white, 0.015f);
                    actionFinishedSignal();
                    return;
                }
            }
            else
            {
                ScriptPrinter.Instance.PrintDialogue("ESRA", "Cakmaga ihtiyacimiz var. Belki Gorkemde vardir.", 20, Color.white, 0.015f);
                actionFinishedSignal();
                return;
            }
        }
        else if (hasStick && !hasThickStick)
        {
            if (InventoryManager.Instance.selectedSlot!=null&& InventoryManager.Instance.selectedSlot.item!=null&&
                InventoryManager.Instance.selectedSlot.item.itemName!=null && InventoryManager.Instance.selectedSlot.item.itemName.Equals("stick"))
            {
                ScriptPrinter.Instance.PrintDialogue("ESRA", "Bir dal daha bulabilirsem sanýrým buraya tam uyacak.", 20, Color.white, 0.015f);
                actionFinishedSignal();
            }
            else
            {
                ScriptPrinter.Instance.PrintDialogue("ESRA", "Mesale koymak icin tasarlanmis. Bu civarda insan yapimi bir magara... Cok ilginc.", 20, Color.white, 0.015f);
                actionFinishedSignal();
            }
            return;
        }
        else if (hasThickStick && !handlePutted)
        {
            if (InventoryManager.Instance.selectedSlot != null && InventoryManager.Instance.selectedSlot.item != null &&
                InventoryManager.Instance.selectedSlot.item.itemName != null && InventoryManager.Instance.selectedSlot.item.itemName.Equals("ThickStick"))
            {
                clickSound();
                InventoryManager.Instance.UseSelectedSlot();
                ScriptPrinter.Instance.PrintDialogue("ESRA", "Cuk oturdu.", 20, Color.white, 0.015f);
                TorchHandle.SetActive(true);
                handlePutted = true;
            }
            else
            {
                ScriptPrinter.Instance.PrintDialogue("ESRA", "Mesale sapýmýz buraya cok yakisirdi", 20, Color.white, 0.015f);
            }
            actionFinishedSignal();
            return;
        }
        else if (handlePutted)
        {
            if (hasCloth) {
                if (InventoryManager.Instance.selectedSlot != null && InventoryManager.Instance.selectedSlot.item != null &&
                    InventoryManager.Instance.selectedSlot.item.itemName != null && InventoryManager.Instance.selectedSlot.item.itemName.Equals("cloth"))
                {
                    clickSound();
                    InventoryManager.Instance.UseSelectedSlot();
                    ScriptPrinter.Instance.PrintDialogue("ESRA", "Yakýlmaya hazir gibi duruyor... Gorkemle konussam iyi olur.", 20, Color.white, 0.015f);
                    actionLevel = 3;
                    TorchHandleCloth.SetActive(true);
                    clothPutted = true;
                }
                else
                {
                    ScriptPrinter.Instance.PrintDialogue("ESRA", "Bezi sapa sarabilirim gibi...", 20, Color.white, 0.015f);
                }
                actionFinishedSignal();
                return;
            }
            else
            {
                ScriptPrinter.Instance.PrintDialogue("ESRA", "Belki mesalenin ustune yakacak bir seyler bulabilirim.", 20, Color.white, 0.015f);
            }
            actionFinishedSignal();
            return;
        }
        else if (clothPutted)
        {
            ScriptPrinter.Instance.PrintDialogue("ESRA", "Cakmaga ihtiyacimiz var. Belki Gorkemde vardir.", 20, Color.white, 0.015f);
            actionFinishedSignal();
            return;
        }
        else
        {
            ScriptPrinter.Instance.PrintDialogue("ESRA", "Mesale koymak icin tasarlanmis...", 20, Color.white, 0.015f);
        }
        actionFinishedSignal();
    }
    public void enterCave()
    {
        if (actionLevel == 1)
        {
            ScriptPrinter.Instance.PrintDialogue("ESRA", "Abow ilginc duruyor.", 20, Color.white, 0.015f);
            B_ClickDetector.instance.decisionFlags[1] = true;
            return;
        }
        else if (actionLevel == 3)
        {
            ScriptPrinter.Instance.PrintDialogue("ESRA", "Mesaleyi yakacak bir sey bulmadan iceriye giremeyiz...", 20, Color.white, 0.015f);
            actionFinishedSignal();
            return;
        }
        else if (actionLevel == 4)
        {
            ScriptPrinter.Instance.PrintDialogue("ESRA", "Gorkem cadirda cakmagý kaybetmis. Onu bulursam mesaleyi tutusturabilir ve iceriyi aydinlatabilirim.", 20, Color.white, 0.015f);
            actionFinishedSignal();
            return;
        }
        if (!caveShown)
        {
            
        }
        else
            StartCoroutine(enterCaveHelper());
    }
    private IEnumerator enterCaveHelper()
    {
        if (TorchLit)
        {
            Debug.Log("Entering cave with lit torch.");
            actionFinishedSignal();
            yield break;
        }
        else
        {
            ScriptPrinter.Instance.PrintDialogue("ESRA", "Icerisi cok karanlik...", 20, Color.white, 0.015f);
            actionFinishedSignal();
            yield break;
        }    
    }
    public void wrongStick()
    {
        if(actionLevel == 3)
        {
            ScriptPrinter.Instance.PrintDialogue("ESRA", "Simdilik odunlarla isim kalmadi", 20, Color.white, 0.015f);
            actionFinishedSignal();return;
        }
        ScriptPrinter.Instance.PrintDialogue("GORKEM", "O odun parçasýnin isimize yarayacagini sanmiyorum.", 20, Color.white, 0.015f);
        actionFinishedSignal();
    }
    public void wrongStick2()
    {
        ScriptPrinter.Instance.PrintDialogue("ESRA", "Bunu kaldirabilcegimi sanmiyorum... Zaten neden bunu istedim ki?", 20, Color.white, 0.015f);
        actionFinishedSignal();
    }
    public void actionFinishedSignal()
    {
        cd.NotifyActionFinished();
    }
    void clickSound()
    {
        am.PlaySFX(clickSFX);
    }
}
