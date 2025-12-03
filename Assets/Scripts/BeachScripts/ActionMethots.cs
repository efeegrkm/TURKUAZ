using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ActionMethots : MonoBehaviour
{
    private DecisionMaker dm;
    private SceneTransitionManager stm;
    private B_ClickDetector cd;
    private BeachManager bm;
    private ScriptPrinter sp;
    public void Awake()
    {
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
    public void interactLGork()
    {
        cd.decisionFlags[0] = true;
        sp.PrintDialogue("ESRA", "Bu bakisi nerde gorsem tanirim yine biseylere heveslenmis... Hadi hayirlisi.");
    }
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
        Debug.Log("act");
        actionFinishedSignal();
    }
    public void collectStickR()
    {
        Debug.Log("act");
        actionFinishedSignal();
    }
    public void collectCloth()
    {
        Debug.Log("act");
        actionFinishedSignal();
    }
    public void enterCave()
    {
        Debug.Log("act");
        actionFinishedSignal();
    }
    public void actionFinishedSignal()
    {
        cd.NotifyActionFinished();
    }
    //Helpers-----------------
    
}
