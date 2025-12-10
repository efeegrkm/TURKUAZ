using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
// Cot out tek çalýþýyor.
public class ClickDetector : MonoBehaviour
{
    public static ClickDetector ClickDetectorInstance;
    public Camera mainCamera;
    public BoxCollider2D caravanCollider;
    public GameObject caravanScene;
    public GameObject MoonShine;
    public GameObject Caravan;
    [SerializeField] private GameObject Smoke;
    int ActiveState = 0;

    [SerializeField] GameObject OutButton;
    [SerializeField] BoxCollider2D musicBoxCollider;
    [SerializeField] SpriteRenderer musicboxSprite;
    [SerializeField] Sprite sp1;
    [SerializeField] Sprite sp2;

    [SerializeField] BoxCollider2D tentCollider;
    [SerializeField] GameObject Tent;
    [SerializeField] GameObject TentScene;
    [SerializeField] CapsuleCollider2D CDCol;
    [SerializeField] GameObject CDMask;
    public bool[] CollectiblesACK;
    private int searchingCD = 0;
    public static int CDCount = AudioManager.cdCount;

    [SerializeField] BoxCollider2D cdPutCol;
    [SerializeField] GameObject unputMask;

    [SerializeField] BoxCollider2D bookCol;
    [SerializeField] BoxCollider2D bookPageLeftCol;
    [SerializeField] BoxCollider2D bookPageRightCol;
    private BookManager bookManager;
    [SerializeField] Animator bookAnim;
    [SerializeField] GameObject book;
    Boolean bookMode = false;
    bool musicskipable = true;

    [SerializeField] BoxCollider2D carCollider;
    [SerializeField] GameObject inCarScene;
    public BoxCollider2D inCarLightingCollider;
    [SerializeField] Sprite lightedCarIn;
    [SerializeField] Sprite darkCar;
    [SerializeField] ScriptPrinter printer;
    private CameraSwitcher cameraSwitcher;

    public BoxCollider2D beachPathCol;
    public BoxCollider2D inCarGorkemCol;

    private Queue<Dialogue> dialogueQueue = new Queue<Dialogue>();
    private bool gorkemsleepFlag = true;

    public InventoryManager im;
    public AudioManager am;

    private int CDputted = 0;

    //sfx
    public AudioClip clickSound;
    public AudioClip CDput;
    public AudioClip CDtake;
    public AudioClip bookOpen;
    public AudioClip bookClose;
    public AudioClip doorOpen;
    public AudioClip doorlocked;
    public AudioClip carDoorOpen;
    public AudioClip carDoorClose;
    public AudioClip unlock;
    public AudioClip switchSound;

    private Coroutine clickCoroutine;
    private Coroutine CotCoroutine;
    private bool clickOnDelay = false;

    public BoxCollider2D vankeyCollider;
    public GameObject vankey;
    private bool caravanLocked = true;

    //Gorkem Awoke scene:
    private bool inCarGorkemAwokeSceneDone = false;
    [SerializeField] private Animator GorkemCarAction;
    private bool carQuitAvoidance = false;//Sahne bitince kapatmayi unutma.
    private bool carLighted = false;
    private string avoidanceReason = "";
    [SerializeField] private BoxCollider2D rightFrontSeatCol;
    private bool seatChangable = false;
    public bool lightTurnable = true;
    private bool gorkemAwoke = false;

    [SerializeField] private Animator blackoutAnim;
    [SerializeField] private GameObject esraCar;
    [SerializeField] private Sprite esraTurnSprite;
    private bool playHug = false;
    private bool playExitHug = false;
    private bool carAvailable = true;
    public bool clickable = true;

    [SerializeField] private AudioClip paperSound;
    private bool[] diagFlags = new bool[5];

    private int sceneState = 1; 

    public DecisionMaker dm;
    private bool[] dmFlags = new bool[3] { false, false, false };

    public Canvas inventoryCanvas;

    [SerializeField] Collider2D LighterCol;
    [SerializeField] GameObject Lighter;
    [SerializeField] Collider2D BlanketCol;
    [SerializeField] GameObject Blanket;
    bool blanketOpen = false;
    public bool LighterTaken = false;
    [SerializeField] AudioClip BlanketAudio;
    [SerializeField] AudioClip BookOpenSFX;
    private void Awake()
    {
        if(ClickDetectorInstance == null)
        {
            ClickDetectorInstance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    void Start()
    {
        cameraSwitcher = this.gameObject.GetComponent<CameraSwitcher>();
        mainCamera = Camera.main;
        CollectiblesACK = new bool[CDCount];
        bookManager = this.gameObject.GetComponent<BookManager>();
        for (int i = 0; i < diagFlags.Length; i++)
        {
            diagFlags[i] = true;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            clickOnDelay = false;
        }
        if (!clickOnDelay && clickable && Input.GetMouseButtonDown(0))
        {
            clickOnDelay = true;
            if (printer.dialogueActive)
            {
                if (dmFlags[0] == true)
                {
                    dmFlags[0] = false;
                    Decidor decidor1 = new Decidor("Yine de sahile inilsin mi?", "GO_BEACH_SCENE", "NO_ACTION");
                }
                if (dmFlags[1] == true)
                {
                    dmFlags[1] = false;
                    Decidor decidor1 = new Decidor("Sahile inilsin mi?", "GO_BEACH_SCENE", "NO_ACTION");
                }
                if (playHug)
                {
                    clickable = false;
                    playHugg();
                }
                else if (playExitHug)
                {
                    clickable = false;
                    playExitHugg();
                }
                nextDialogue();
            }
            else if(cameraSwitcher.isMainCameraActive)
                CheckClick(Input.mousePosition);
            else
            {
                cameraSwitcher.PlayActionCameraAnimation();
            }
        }
    }
    void playHugg()
    {
        playHug = false;
        inCarScene.GetComponent<Animator>().SetBool("Hug", true);
        esraCar.SetActive(false);
        Invoke("inve", 6f);
        Invoke("inve2",7f);
        //UI interactable false
        Invoke("whilehugDialogue", 8f);
    }
    void inve2()
    {
        esraCar.SetActive(true);
    }
    void playExitHugg()
    {
        sceneState = 3;
        playExitHug = false;
        inCarScene.GetComponent<Animator>().SetBool("Hug", true);
        esraCar.SetActive(false);
        Invoke("i2", 0.5f);
        Invoke("i1", 8f);
    }
    void i1()
    {
        clickable = true;
        blackoutAnim.SetBool("Blackout", false);
        am.PlaySFX(carDoorClose);
        OutButton.SetActive(false);
        inCarScene.SetActive(false);
        MoonShine.SetActive(true);
        Caravan.SetActive(true);
        im.HighlightFG();
        am.raiseAmbianceVolume();
        ActiveState = 0;
    }
    void i2()
    {
        blackoutAnim.SetBool("Blackout", true);
        carAvailable = false;
    }
    void nextDialogue()
    {
        if (dialogueQueue.Count > 0)
        {
            Dialogue nextDialogue = dialogueQueue.Dequeue();
            printer.PrintDialogue(nextDialogue.speaker, nextDialogue.content, 20, Color.white, 0.02f);
            checkDialogueTriggeredAction(nextDialogue);
        }
        else
        {
            printer.closeDialogue();
            printer.dialogueActive = false;
        }
    }
    private void checkDialogueTriggeredAction(Dialogue dial)
    {
        if(dial.dialogueLocation.Equals("AwokenCarTalkBehindSeat") && dial.dialogueNumber == 5)
        {
            GorkemCarAction.SetBool("Dance", false);
        }
        else if(dial.dialogueLocation.Equals("AwokenCarTalkBehindSeat") && dial.dialogueNumber == 6)
        {
            am.lowerMusicVolume();
        }
        else if (dial.dialogueLocation.Equals("AwokenCarTalkBehindSeat") && dial.dialogueNumber == 16)
        {
            seatChangable = true;
            carQuitAvoidance = true;
            avoidanceReason = "frontPass";
            lightTurnable = true;
        }
        else if (dial.dialogueLocation.Equals("AwokenCarTalkFrontSeat") && dial.dialogueNumber == 8)
        {
            esraCar.GetComponent<SpriteRenderer>().sprite = esraTurnSprite;
        }
        else if (dial.dialogueLocation.Equals("AwokenCarTalkFrontSeat") && dial.dialogueNumber == 18)
        {
            playHug = true;
        }
        else if (dial.dialogueLocation.Equals("whileHugFrontSeat") && dial.dialogueNumber == 13)
        {
            im.AddToInventory(3);
            am.PlaySFX(clickSound);
        }
        else if (dial.dialogueLocation.Equals("whileHugFrontSeat") && dial.dialogueNumber == 18)
        {
            playExitHug = true;
            am.raiseMusicVolume();
        }
    }
    private void inve()
    {
        inCarScene.GetComponent<Animator>().SetBool("Hug", false);
    }
    public void CheckClick(Vector2 screenPosition)
    {
        if (clickCoroutine != null) return;

        clickCoroutine = StartCoroutine(CheckClickRoutine(screenPosition));
    }
    private void revertCarLights() {
        if (!lightTurnable)
        {
            printer.PrintDialogue("ESRA", "Suanda isigi kapatmak istemiyorum. Gorkem dans ediyor:)", 20, Color.white, 0.02f);
            return;
        }
        if (inCarScene.GetComponent<SpriteRenderer>().sprite == darkCar)
        {
            GorkemCarAction.SetBool("light",true);
            carLighted = true;
            if(vankey!=null)
                vankey.SetActive(true);
        }
        else
        {
            carLighted = false;
            GorkemCarAction.SetBool("light",false);
            if (vankey!=null)
                vankey.SetActive(false);
        }
    }
    private IEnumerator CheckClickRoutine(Vector2 screenPosition)
    {
        Vector2 worldPoint;
        worldPoint = mainCamera.ScreenToWorldPoint(screenPosition); 
        if (printer.dialogueWriting)
        {
            clickCoroutine = null;
            yield break;
        }
        if (!bookMode)
        {
            if(ActionMethots.Instance != null && ActionMethots.Instance.actionLevel == 4)
            {
                if (BlanketCol.OverlapPoint(worldPoint))
                {
                    if (blanketOpen)
                    {
                        blanketOpen = false;
                        Blanket.SetActive(false);
                        am.PlaySFX(BlanketAudio);
                    }
                    else
                    {
                        printer.PrintDialogue("SYSTEM", "Battaniye acildi.", 20, Color.white, 0.02f);
                        am.PlaySFX(BlanketAudio);
                        blanketOpen = true;
                        Blanket.SetActive(true);
                    }
                }
                if (LighterCol.OverlapPoint(worldPoint))
                {
                    if (!LighterTaken)
                    {
                        LighterTaken = true;
                        Lighter.SetActive(false);
                        im.AddToInventory(7);
                        printer.PrintDialogue("SYSTEM", "Çakmak bulundu.", 20, Color.white, 0.02f);
                        ActionMethots.Instance.actionLevel = 5;
                        am.PlaySFX(clickSound);
                    }
                    else
                    {
                        if (blanketOpen)
                        {
                            blanketOpen = false;
                            Blanket.SetActive(false);
                            am.PlaySFX(BlanketAudio);
                        }
                        else
                        {
                            printer.PrintDialogue("SYSTEM", "Battaniye acildi.", 20, Color.white, 0.02f);
                            am.PlaySFX(BlanketAudio);
                            blanketOpen = true;
                            Blanket.SetActive(true);
                        }
                    }
                }
            }
            
            if (rightFrontSeatCol.OverlapPoint(worldPoint))
            {
                if(carLighted == false && seatChangable == true)
                {
                    StartCoroutine(initiateRightFrontSeatScene());
                }
                else if(carLighted == true && seatChangable == true)
                {
                    printer.PrintDialogue("ESRA", "Once isigi kapatayim.", 20, Color.white, 0.02f);
                }
            }
            if (inCarLightingCollider.OverlapPoint(worldPoint))
            {
                if (lightTurnable)
                    am.PlaySFX(switchSound);
                revertCarLights();
            }
            else if (vankey!=null && vankeyCollider.OverlapPoint(worldPoint))
            {
                im.AddToInventory(2);
                printer.PrintDialogue("SYSTEM", "Bir anahtar bulundu.", 20, Color.white, 0.02f);
                am.PlaySFX(clickSound);
                Destroy(vankey);
            }
            else if (caravanCollider.OverlapPoint(worldPoint))
            {
                if (!caravanLocked)
                {
                    am.PlaySFX(doorOpen);
                    OutButton.SetActive(true);
                    caravanScene.SetActive(true);
                    MoonShine.SetActive(false);
                    Smoke.transform.SetParent(null);
                    Caravan.SetActive(false);
                    ActiveState = 1;
                    am.lowerAmbianceVolume();
                }
                else
                {
                    if (im.selectedSlot!=null && im.selectedSlot.item != null && im.selectedSlot.item.itemName !=null&& im.selectedSlot.item.itemName.Equals("VanKey"))
                    {
                        printer.PrintDialogue("SYSTEM", "Karavanýn kilidi acildi.", 20, Color.white, 0.02f);
                        im.UseSelectedSlot();
                        am.PlaySFX(unlock);
                        caravanLocked = false;
                    }
                    else
                    {
                        printer.PrintDialogue("SYSTEM", "Karavan kilitli.", 20, Color.white, 0.02f);
                        am.PlaySFX(doorlocked);
                    }
                }
            }
            else if (musicBoxCollider.OverlapPoint(worldPoint))
            {
                if (musicskipable)
                {
                    musicskipable = false;
                    musicboxSprite.sprite = sp2;
                    am.musicSource.Pause();
                    am.PlaySFX(CDput);
                    am.displayMusicInfo("/");
                    yield return new WaitForSeconds(1f);
                    inv1();
                }
            }
            else if (tentCollider.OverlapPoint(worldPoint))
            {
                OutButton.SetActive(true);
                TentScene.SetActive(true);
                MoonShine.SetActive(false);
                Smoke.transform.SetParent(null);
                Caravan.SetActive(false);
                ActiveState = 2;
            }
            else if (!CollectiblesACK[0] && CDCol.OverlapPoint(worldPoint))
            {
                CDMask.SetActive(true);
                CollectiblesACK[0] = true;
                am.PlaySFX(clickSound);
                printer.PrintDialogue("SYSTEM", "Yeni bir CD bulundu !!! Yeni CD'ler için gözün açýk olsun...", 20, Color.white, 0.02f);
                searchingCD++;
                im.AddToInventory(0);
            }

            else if (cdPutCol.OverlapPoint(worldPoint))
            {
                if(CDputted==1)
                {
                    printer.PrintDialogue("SYSTEM", "CD-1 alindi.", 20, Color.white, 0.02f);
                    im.AddToInventory(0);
                    am.currentCD = 0;
                    unputMask.SetActive(true);
                    CDputted = 0;
                    am.musicSource.Stop();
                    am.PlaySFX(CDtake);
                    am.CD1Index = 0;
                    am.CurrentSong = -1;
                    am.noMusic = true;
                }
                else if (im.selectedSlot != null && im.selectedSlot.item != null && im.selectedSlot.item.itemName.Equals("CD1"))
                {
                    unputMask.SetActive(false);
                    printer.PrintDialogue("SYSTEM", "CD-1 takildi.", 20, Color.white, 0.05f);
                    am.PlaySFX(CDput);
                    im.UseSelectedSlot();
                    am.currentCD = 1;
                    CDputted = 1;
                }
                else if(searchingCD == 0) 
                {
                    printer.PrintDialogue("ESRA", "Henüz hic CD bulamadým. Görkem birinin cadýrdaki tulumunda oldugunu soylemisti.", 20, Color.white, 0.02f);
                }
                else if (searchingCD >= CDCount)
                {
                    printer.PrintDialogue("GORKEM", "Aþkým tüm CD'lerimi ve favori þarkilarimizi bulmuþsun!!! Seni çok seviyorummmm:)", 20, Color.white, 0.02f);
                }
            }
            else if (carCollider.OverlapPoint(worldPoint))
            {
                if (!carAvailable)
                {
                    printer.PrintDialogue("ESRA", "Arabada bir iþim kalmadi.", 20, Color.white, 0.02f);
                    yield break;
                }
                am.PlaySFX(carDoorOpen);
                yield return new WaitForSeconds(0.3f);
                am.lowerAmbianceVolume();
                im.LowlightFG();
                inCarScene.SetActive(true);

                if(carLighted)
                    GorkemCarAction.SetBool("light", true);
                else
                    GorkemCarAction.SetBool("light", false);

                if (am.CurrentSong == -1 && am.noMusic == false)
                {
                    Debug.Log("Car clicked with music on");
                    if (carLighted == false)
                    {
                        Debug.Log("ýþýk açýlýyor.");
                        revertCarLights();
                    }
                    GorkemCarAction.SetBool("Dance", true);
                    lightTurnable = false;
                    carQuitAvoidance = true;
                    avoidanceReason = "Disari cikmak istemiorm Gorkem'i uyandirmayi basardim.";
                    gorkemAwoke = true;
                }
                OutButton.SetActive(true);
                MoonShine.SetActive(false);
                Caravan.SetActive(false);
                ActiveState = 3;
            }
            else if (inCarGorkemCol.OverlapPoint(worldPoint))
            {
                if (am.CurrentSong == -1 && am.noMusic == false && !inCarGorkemAwokeSceneDone)
                {
                    StartCoroutine(initiateCarAwokenScene());
                    inCarGorkemAwokeSceneDone = true;
                }
                else if (gorkemsleepFlag && gorkemAwoke == false)
                {

                    Dialogue[] d = new Dialogue[14];
                    gorkemsleepFlag = false;
                    d[0] = new Dialogue("Askim uyudun mu napiyosun?", "ESRA");
                    d[1] = new Dialogue("12 oldu saat meteor yagmuru basladi. Sahile oturup izleyelim.", "ESRA");
                    d[2] = new Dialogue("ZZalýmlaaaggrr aagghh...", "GORKEM");
                    d[3] = new Dialogue("Sassagalin ulan beni hayin kOPekLerrAGGGgG...", "GORKEM");
                    d[4] = new Dialogue("Cikitaga mugz... hrhhhohh......", "GORKEM");
                    d[5] = new Dialogue("Bu nasi bi rüya bu Gorkem kendine gel ALOO...", "ESRA");
                    d[6] = new Dialogue("Kalk karavana gecelim arabada nasil uyuyakalabiliosun anlamiyorum ki...", "ESRA");
                    d[7] = new Dialogue("HAGGAA Korsanlar.. HURAAGGHHH?!?! IMDAAAGGTT!!!!", "GORKEM");
                    d[8] = new Dialogue("Calilardalar!!!", "GORKEM");
                    d[9] = new Dialogue("Denizdelerrrrr!!!", "GORKEM");
                    d[10] = new Dialogue("Askimmm NOLUYO ALO KALK Hadiii!!!", "ESRA");
                    d[11] = new Dialogue("muck:)", "ESRA");
                    d[12] = new Dialogue("Hrrrhhooaþþohh:)", "GORKEM");
                    d[13] = new Dialogue("Yok operek de uyanmiosun... Sana karavandaki muzik calardan ayacak biseyler acayim en iyisi.", "ESRA");
                    for(int i = 0; i < 14; i++)
                    {
                        d[i].assignIDLoc("SleepyCarTalk", i);
                        dialogueQueue.Enqueue(d[i]);
                    }
                    sceneState = 2;
                    nextDialogue();
                }
                else if(gorkemAwoke == false)
                {
                    printer.PrintDialogue("GORKEM", "Anlamsýz homurtular...", 20, Color.white, 0.02f);
                }
                else
                {
                    printer.PrintDialogue("GORKEM", "Gel hadi yildizlari kacirioruz.", 20, Color.white, 0.02f);

                }
            }
            else if(beachPathCol.OverlapPoint(worldPoint))
            {
                switch (sceneState)
                {
                    case 1:
                        printer.PrintDialogue("ESRA", "Sahile inen patika... Yildizlari izlemek icin harika bir yer, Gorkem arabada uzanýyordu onunla gitmeyi tercih ederim.", 20, Color.white, 0.02f);
                        break;
                    case 2:
                        printer.PrintDialogue("ESRA", "Sahile inen patika... Sahile inmeden önce Gorkemi uyandirmaliyim.", 20, Color.white, 0.02f);
                        break;
                    case 3:
                        if(ActionMethots.Instance == null)
                        {
                                printer.PrintDialogue("ESRA", "Sahile inmeden once kitap sayfalarini eklemeyi tercih ederim. Ama baska zaman da yapabilirim.", 20, Color.white, 0.02f);
                            dmFlags[0] = true;
                        }
                        else
                        {
                            new Decidor("Sahile inilsin mi?", "GO_BEACH_SCENE", "NO_ACTION");
                        }
                        break;
                    case 4:
                        if (ActionMethots.Instance == null)
                        {
                            printer.PrintDialogue("ESRA", "Kitap sayfalarini ekledigime gore artik sahile inebilirim... Gorkem beni bekliyor.", 20, Color.white, 0.02f);
                            dmFlags[1] = true;
                        }
                        else
                        {
                            new Decidor("Sahile inilsin mi?", "GO_BEACH_SCENE", "NO_ACTION");
                        }
                        break;
                }
            }
        }
        else
        {
            if (bookPageRightCol.OverlapPoint(worldPoint))
            {
                bookManager.NextPage();
            }
            else if (bookPageLeftCol.OverlapPoint(worldPoint))
            {
                bookManager.PreviousPage();
            }
            else
            {
                if(bookManager.currentPage == BookManager.maxPage-1)
                {
                    bookManager.PreviousPage();
                    yield return new WaitForSeconds(0.5f);
                    inv2();
                }
                else
                {
                    inv2();
                }
            }
        }
        if (bookCol.OverlapPoint(worldPoint))
        {
            if(im.selectedSlot != null && im.selectedSlot.item != null)
            {
                if (im.selectedSlot.item.itemName.Equals("paper1"))
                {
                    sceneState = 4;
                    im.UseSelectedSlot();
                    am.PlaySFX(paperSound);
                    bookManager.assignWallETexts();
                    printer.PrintDialogue("SYSTEM", "Kitaba yeni sayfalar eklendi.", 20, Color.white, 0.02f);
                }
            }
            else
            {
                am.PlaySFX(bookOpen);
                book.SetActive(true);
                if (bookManager.currentPage == BookManager.maxPage - 1)
                {
                    bookManager.bookAnim.SetBool("onLastPage", true);
                }
                bookMode = true;
            }
        }
        im.selectedSlot = null;
        im.UpdateSelectionUI();
        clickCoroutine = null;
        yield break;
    }
    private IEnumerator initiateRightFrontSeatScene()
    {
        Debug.Log("Right front seat scene initiated");
        //Assumpt lights off.
        blackoutAnim.SetBool("Blackout",true);
        yield return new WaitForSeconds(3f);
        //Ön koltuk geçiþ sesi.
        am.musicSource.Stop();
        esraCar.SetActive(true);
        blackoutAnim.SetBool("Blackout", false);
        yield return new WaitForSeconds(3f);
        frontSceneDialogue();
        //Sarýlma sahnesi. ortadaki yeri söktürdüðümüz iyi oldu mention. vs vs dialoglar.
        //Wall-e mention garajda bulunun kayýp sayfanin verilmesi.
        //Karavanda bir isin yoksa gel sahile inelim diyalogu.
        lightTurnable = true;
        carQuitAvoidance = false;
        avoidanceReason = "";
        yield break;
    }
    private void whilehugDialogue()
    {
        if (diagFlags[1] == false) return;
        diagFlags[1] = false;
        clickable = true;
        Dialogue[] d = new Dialogue[19];
        d[0] = new Dialogue("Biraz daha iyi misin?", "ESRA");
        d[1] = new Dialogue("Kesinlikle:)", "GORKEM");
        d[2] = new Dialogue("Þu ortadaki kol koyma yerini sokturtmek hayatimizda aldigimiz en iyi ucuncu karardi.", "GORKEM");
        d[3] = new Dialogue("Budur yani yillardir aradigim sarilma konforu...", "GORKEM");
        d[4] = new Dialogue("Biraz dolandirildik gibi OSTIM'de sanki.d", "ESRA");
        d[5] = new Dialogue("Canim sag olsun", "GORKEM");
        d[6] = new Dialogue("Olsun askim:)", "ESRA");
        d[7] = new Dialogue("En iyi aldigimiz ikinci en iyi karar neymis?(Birinci bariz)", "ESRA");
        d[8] = new Dialogue("Hmhm", "GORKEM");
        d[9] = new Dialogue("Evin garajinda Wall-e'mizi yapmak tabi ki", "GORKEM");
        d[10] = new Dialogue("MDOEKDOEEKOEKEEKEK evet o da barizmiþ", "ESRA");
        d[11] = new Dialogue("Ha bu arada...", "GORKEM");
        d[12] = new Dialogue("Garajda Wall-e yi yaparkenki notlarimiz resimlerimiz olan sayfalari unutmusuz. Evden çýkarken buldum.", "GORKEM");
        d[13] = new Dialogue("Karavandaki kitaba ekleyelim bunlari da bi ara al sana vereyim.", "GORKEM");
        d[14] = new Dialogue("Aha kaybettik sanýyordum. Süper yapistiririm ben bunlarý.", "ESRA");
        d[15] = new Dialogue("Sahile izlemeye inelim mi artýk ne diosun?", "GORKEM");
        d[16] = new Dialogue("Rahatladiysan inelim hadi", "ESRA");
        d[17] = new Dialogue("Gelmeden belki bi karavana uðrayýp kitabýmýza sayfalarý eklerim geç istersen sen yavastan canim.", "ESRA");
        d[18] = new Dialogue("Tamam bekliyorum seni sahilde gel bi kez daha sarilip cikalim.", "GORKEM");
        for (int i = 0; i < 19; i++)
        {
            d[i].assignIDLoc("whileHugFrontSeat", i);
            dialogueQueue.Enqueue(d[i]);
        }
        nextDialogue();
    }
    private void frontSceneDialogue()
    {
        if (diagFlags[2] == false) return;
        diagFlags[2] = false;
        Dialogue[] d = new Dialogue[19];
        d[0] = new Dialogue("Ee ne gordun dinliyorum...", "ESRA");
        d[1] = new Dialogue("Baya dehþete dusmus gibisin kabusumsu bsiler mi TLOU ruyalarindan mi?", "ESRA");
        d[2] = new Dialogue("Ya hayýr.", "GORKEM");
        d[3] = new Dialogue("Öyle aþýrý korkunc oldugundan degil de... Nasýl anlatsam...", "GORKEM");
        d[4] = new Dialogue("Her þey aþýrý gercekciydi. Bi adam gordum hintli gibi konusuyor ama boyle kalin bi sesi vardi.", "GORKEM");
        d[5] = new Dialogue("Sismanca bsi boyle biraz bay yengec'e benziyordu. Ama asil beni geren ruyam degil.", "GORKEM");
        d[6] = new Dialogue("Sanki...", "GORKEM");
        d[7] = new Dialogue("Suanda ruyamdaki olay orgusunun icindeyiz gibi hissediyorum.", "GORKEM");
        d[8] = new Dialogue("Ne demek istiyorsun?", "ESRA");
        d[9] = new Dialogue("Her sey guzel basladi. Sakin bi aksamda arabada muzik dinliyorduk. Hatta ne tesaduftur... Manifestlioduk.", "GORKEM");
        d[10] = new Dialogue("Arabadan disari adim attigimda seni kaybettim. Bacaklarým tutmuyordu. Hintce fisiltilar her yanimi sardi kulagim çýnlýyordu...", "GORKEM");
        d[11] = new Dialogue("Sonra ciglik atmaya basladin. Arabanin altina saklanmistin. Cadirda saklaniyor ustune sur ez sunu diye bana bagiriyor deli gibi agliyordun", "GORKEM");
        d[12] = new Dialogue("Tam ne oldugunu anlamaya calisirken ayagima bi kanca saplandi. Beni ucurumdan assagiya denizin icine dogru cekmeye basladi.", "GORKEM");
        d[13] = new Dialogue("Hintli sisman korsani iste o an gordum.", "GORKEM");
        d[14] = new Dialogue("Arabada uyukalmak sana yaramio askm bu nasi ruya dfhgjdhsjf.", "ESRA");
        d[15] = new Dialogue("Evet iste de ne bileyim arabanýn etrafinda biseyler var gibi hissediyorum kancasinin arabama surtus sesini duyuyor gibi oluyorum.", "GORKEM");
        d[16] = new Dialogue("Bi deniz havasi alsak iyi gelir sana eminim. Yildizlar da çok guzel hava mukemmel gel biraz kafan dagilsin.", "ESRA");
        d[17] = new Dialogue("Ama once...", "ESRA");
        d[18] = new Dialogue("Gel biraz sarilalim:)", "ESRA");
        for (int i = 0; i < 19; i++)
        {
            d[i].assignIDLoc("AwokenCarTalkFrontSeat", i);
            dialogueQueue.Enqueue(d[i]);
        }
        nextDialogue();
    }
    private IEnumerator initiateCarAwokenScene()
    {
        if (diagFlags[3] == false) yield break;
        diagFlags[3] = false;
        Dialogue[] d = new Dialogue[17];
        d[0] = new Dialogue("Yav kapat sunu gozunu seveyim bebem bu ne ya uyandim daa.", "GORKEM");
        d[1] = new Dialogue("Hangi CD den buldun bunu abidik gubidik kim doldurdu bu CD'yi? ", "GORKEM");
        d[2] = new Dialogue("Kapatmami ister gibi durmuosun .d", "ESRA");
        d[3] = new Dialogue("Ayrica...", "ESRA");
        d[4] = new Dialogue("CD'yi de sen doldurdun MDOEKDOEEKOEKEEKEK", "ESRA");
        d[5] = new Dialogue("...", "GORKEM");
        d[6] = new Dialogue("Tamam fena sarki degil kabul ettim.", "GORKEM");
        d[7] = new Dialogue("Ne zamanlardi 24 mü 25 mi o yýllarda patladilardi.", "GORKEM");
        d[8] = new Dialogue("Yazik oldu o aralar bi konserde twerk attilar die gözaltina aldilardi bunlari. Bidaha da toparlanamadilar:(", "GORKEM");
        d[9] = new Dialogue("Ne zamanlari...Kotu gunlerdi Hayko'yu tutuklamalari benim icin son damla oldu. Neyse evlendik falan rahatladi gibi ortalik baya.", "ESRA");
        d[10] = new Dialogue(":)", "GORKEM");
        d[11] = new Dialogue(":)", "ESRA");
        d[12] = new Dialogue("Her neyse asiri garip ruyalar gordum. Ama rahatsiz edici bi his birakti icimde.","GORKEM");
        d[13] = new Dialogue("Ne demek istiyorsun ne gordun?", "ESRA");
        d[14] = new Dialogue("Istersen yanima gel oyle anlatayim sonra da sahile yildizlari seyretmeye ineriz.", "GORKEM");
        d[15] = new Dialogue("Olurr biraz arabada duracaksak isigi da kapatalim mi? Yildizlar zor gozukuyor boyle.", "ESRA");
        d[16] = new Dialogue("Tabi kapat gel yanima bekliyorum:)", "GORKEM");
        for(int i = 0; i<17; i++)
        {
            d[i].assignIDLoc("AwokenCarTalkBehindSeat", i);
            dialogueQueue.Enqueue(d[i]);
        }
        nextDialogue();
        yield break;
    }
    public void CoTOut()
    {
        if (CotCoroutine != null || clickable == false) return;
        CotCoroutine = StartCoroutine(CoTCoroutine());
    }
    private IEnumerator CoTCoroutine()
    {
        if (ActiveState == 1)//caravan out
        {
            OutButton.SetActive(false);
            caravanScene.SetActive(false);
            MoonShine.SetActive(true);
            Caravan.SetActive(true);
            Smoke.transform.SetParent(Caravan.transform);
        }
        else if (ActiveState == 2)//tent out
        {
            OutButton.SetActive(false);
            TentScene.SetActive(false);
            MoonShine.SetActive(true);
            Caravan.SetActive(true);
            Smoke.transform.SetParent(Caravan.transform);
        }
        else if (ActiveState == 3)//car out
        {
            if (carQuitAvoidance)
            {
                if (avoidanceReason.Equals("frontPass"))
                {
                    if (carLighted)
                        printer.PrintDialogue("ESRA", "Suanda arabadan cikmak istemiyorum. On koltuga atlayacagim");
                    else
                        printer.PrintDialogue("ESRA", "Suanda arabadan cikmak istemiyorum. Isigi kapatip on koltuga atlayacagim");
                }
                else
                {
                    printer.PrintDialogue("ESRA", avoidanceReason);
                }
            }
            else
            {
                am.PlaySFX(carDoorClose);
                yield return new WaitForSeconds(0.2f);
                OutButton.SetActive(false);
                inCarScene.SetActive(false);
                MoonShine.SetActive(true); 
                Caravan.SetActive(true);
                im.HighlightFG();
            }
        }
        am.raiseAmbianceVolume();
        ActiveState = 0;
        CotCoroutine = null;
        yield break;
    }
    void inv1()
    {
        musicboxSprite.sprite = sp1;
        am.musicSource.UnPause();
        am.nextMusic();
        musicskipable = true;
    }
    void inv2()
    {
        book.SetActive(false);
        bookMode = false;
    }
    public void accessBeachDebug()
    {
        SceneTransitionManager.Instance.SceneTransitionTo(1);
    }
}