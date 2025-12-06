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
    public void GoToVanScene()
    {
        Decidor decidor = new Decidor("Kayalýklara geri gidilsin mi?", "GO_VAN_SCENE", "NO_ACTION");
    }
    //-------------------------------
    public void interactLGork()
    {
        cd.decisionFlags[0] = true;
        sp.PrintDialogue("ESRA", "Bu bakisi nerde gorsem tanirim yine biseylere heveslenmis... Hadi hayirlisi.", 20, Color.white, 0.015f);
    }
    //-------------------------------
    public void interactShoreBoat()
    {
        Debug.Log("act");
        actionFinishedSignal();
    }
    //-------------------------------
    public void goToRightBeach()
    {
        StartCoroutine(gtrbHelper());
    }
    public IEnumerator gtrbHelper()
    {
        stm.fadeIn();
        yield return new WaitForSeconds(stm.transitionDuration);
        bm.activeBeachPart('R');
        stm.fadeOut();
        yield return new WaitForSeconds(stm.transitionDuration);
        actionFinishedSignal();
        yield break;
    }
    //-------------------------------
    public void goToLeftBeach()
    {
        StartCoroutine(gtlbHelper());
    }
    public IEnumerator gtlbHelper()
    {
        stm.fadeIn();
        yield return new WaitForSeconds(stm.transitionDuration);
        bm.activeBeachPart('L');
        stm.fadeOut();
        yield return new WaitForSeconds(stm.transitionDuration);
        actionFinishedSignal();
        yield break;
    }
    //-------------------------------
    public void collectStickL()
    {
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
        clothObject.SetActive(false);
        ScriptPrinter.Instance.PrintDialogue("ESRA", "Bez isimize yarayabilir.", 20, Color.white, 0.015f);
        InventoryManager.Instance.AddToInventory(6);
        actionFinishedSignal();
    }
    public void torchSlot()
    {
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
                if (InventoryManager.Instance.selectedSlot.item.itemName.Equals("Lighter"))
                {
                    InventoryManager.Instance.UseSelectedSlot();
                    TorchLight.SetActive(true);
                    ScriptPrinter.Instance.PrintDialogue("ESRA", "Mesalemiz hazir. Magaraya girebiliriz.", 20, Color.white, 0.015f);
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
                    InventoryManager.Instance.UseSelectedSlot();
                    ScriptPrinter.Instance.PrintDialogue("ESRA", "Yakýlmaya hazir gibi duruyor...Cakmak var mý ki Gorkemde.", 20, Color.white, 0.015f);
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
        if (!caveShown)
        {
            ScriptPrinter.Instance.PrintDialogue("ESRA", "Abow ilginc duruyor.", 20, Color.white, 0.015f);
            B_ClickDetector.instance.decisionFlags[1] = true;
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
    public void actionFinishedSignal()
    {
        cd.NotifyActionFinished();
    }
    
}
