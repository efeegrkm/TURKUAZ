using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;
public class BookManager : MonoBehaviour
{
    public int currentPage = 0;
    public static int maxPage = 3; 
    public Animator bookAnim;
    [SerializeField] ScriptPrinter sp;
    public GameObject left;
    public GameObject right;
    private TextMeshProUGUI leftPage;
    private TextMeshProUGUI rightPage;
    private string[] leftPageTexts = new string[20];
    private string[] rightPageTexts = new string[20];
    bool rightflipable = true;
    bool leftflipable = false;
    public AudioClip pageFlip;
    public AudioManager am;
    [SerializeField] Image leftFull;
    [SerializeField] Image rightFull;
    [SerializeField] Sprite[] pageImages = new Sprite[30];

    bool a = false;
    void Start()
    {
        leftPage = left.GetComponent<TextMeshProUGUI>();
        rightPage = right.GetComponent<TextMeshProUGUI>();
        assignBaseTexts();
        currentPage = 0;
        setPageText();
    }
    private void assignBaseTexts()
    {
        maxPage = 4;
        leftPageTexts[0] = "18 Haziran 2034 \n Bugün sabah Kas sahilde kullanilmayan bir incirlikte uyandik. Rotamiz belli:" +
            "\n Kastan Finikeye dogru sahil yolundan portakal bahçeleri ve deniz manzaralariyla ilerleyecegiz.";
        leftPageTexts[1] = "Elmali yakininda, Likya yolu rotasinda güzel bir yuruyusun ardindan kizgin tasta orman içi bir et mangal arasi verecegiz. Yayla tarafinda guzel bir karavan aksamiyla da gunu bitiririz diye dusunuyorum.";
        leftPageTexts[2] = "I00L";
        leftPageTexts[3] = "Kahvaltiyi hazirladik, karavanimizin onune sofrayi kurduk, Gorkem incirleri de masaya koydu... Koymaz olaydý... Ilk yedigim incirin icinden ari cikti dilimi soktu bogazim sisti. Apar topar Kas devlet hastahanesine gittik. Butun gun hastahanede gecti. Cok mutsuzum agagagagag";
        

        rightPageTexts[0] = "Finikeye vardiktan sonra muhtemelen cisim gelir. Ufak bir ihtiyac molasi veririz. Yani ben veririm, Gorkem beni bekler. Molanin ardindan ormanlarin ve kivrimli yollarin icerisinden Elmaliya dogru ilerleyecegiz.";
        rightPageTexts[1] = "Cok heyecenliyim... Simdi ben karavanda kahvaltiyi hazirlarken Gorkem de bana incir toplayacagini soledi!!! askim binim...";
        rightPageTexts[2] = "\n \n \n Sabah kahvaltimizdan bir kare. \n \n Felaketten dakikalar once...";
        rightPageTexts[3] = " ";
    }
    public void assignWallETexts()
    {
        maxPage = 10;
        rightPageTexts[3] = "25 Ekim 2034 \n Ilerledik mi? Sayilir... Paletleri takilip duran pasli kahve tankimiza ne kadar ilerledik denebilirse o kadar ilerledik. Mesaiden gelince ilk gorusme noktamiz garaj oldu. Ama hevesimiz kirilmadi, umarim haftasonu elle tutulur bi noktaya gelebiliriz.";
        leftPageTexts[4] = "I03L";// Resim sayfasi olacak Pasli teneke tanki yaninda cokmus Gorkem resmi.
        rightPageTexts[4] = "27 Ekim 2034 \n Saat gecenin 2si dun neredeyse tum gun LIPO pillerin palete guc vermesi icin ugrastim. Gorkem de takti gozleri donebilir yapicam goruntu islemesine baslicam diye... Iyi yap dedim daha adamin kafasini takmadik ugrastigi islere bak. Her neyse verimli bir gundu.";
        leftPageTexts[5] = "I04L"; // Paletleri sokulmus Wall-e resmi.
        rightPageTexts[5] = "11 Kasim 2034 \n Sanirim basardik... 3 axisde donen paletler ve gozler, assagi yukari dokundugu seyleri taniyabilen kollar, her sey cinlilerden caldigimiz self learning API ve Hurdalikta buldugumuz eski 2027 model bir tesla optimus iyon bataryasiyla mukemmel calisiyor.";
        leftPageTexts[6] = "20 Kasim 2034 \n Wall-e ekstrem davranislar gostermeye basladi. Gecen gun pilinin azaldigini kendiliginden fark edip kapanmasini engellemek icin kendi kendini parcalayip pilini degistirmeye kalkisti. En azindan teorimiz bu... Geldigimizde batarya kutusu dagilmis halde bulduk.";
        rightPageTexts[6] = "I05R"; // Esra wall-e'yi tutarken Gorkem kapatmaya calisiyor resmi.
        leftPageTexts[7] = "1 Aralik 2034 \n Sanirim korkmaya basladim... son 3 4 gundur ne zaman onu kapatmaya calissak geri cekilmeye ve otomatik sarj unitesine kapatma tusunu iceride birakacak sekilde girmeye basladi. YSA'sini coktan gelismeye kapattik ancak bana hala gun gectikce akillaniyor gibi geliyor.";
        rightPageTexts[7] = "I06R";
        leftPageTexts[8] = "2 Aralik 2034 \n Bbu...Bu bardagi tasiran son damlaydi. Bugun isten geldiginde Gorkem wall-e'den kahve getirmesini istedi. Kahveyi getirdiginde Gorkem'in yanlislikla kahveyi Wall'enin uzerine dokmesiyle ortalik karisti. Gorkemin ayagini paleti ile ezip kahveyi Gorkemin tisortune sildi...";
        rightPageTexts[8] = "I07R";
        leftPageTexts[9] = "3 Aralik 2034 \n Wall-e nin siddete basvurmasinin ardindan zorla tutup salterini indirdik. Bugun 7 Aralik yildonumu planimiz icin karavanla 1 haftalik bir deniz kenari seyahatine cikacagiz. Wall-e'yi son olanlardan sonra Captur'un arkasina tiktik. Bu da demek ki bizimle geliyor.";
        rightPageTexts[9] = " ";
    }
    public void NextPage()
    {
        am.sfxSource.Stop();
        if (rightflipable && currentPage < maxPage-2)
        {
            am.PlaySFX(pageFlip);
            Invoke("closePages", 0.1f);
            leftflipable = true;
            currentPage++;
            bookAnim.SetBool("flip",true);
            leftPage.text = "";
            rightPage.text = "";
            Invoke("setPageText", 0.3f);
        }
        else if(rightflipable)
        {
            am.PlaySFX(pageFlip);
            Invoke("closePages", 0.1f);
            currentPage++;
            bookAnim.SetBool("flip",true);
            inv();
            rightflipable = false;
            leftPage.text = "";
            rightPage.text = "";
            Invoke("setPageText", 0.3f);
        }
        else
        {
            if(maxPage < 9)
                sp.PrintDialogue("SYSTEM", "Kitabýn sayfalarý yýrtýlmýþ:( Kayýp sayfalarý bulmak için gözünü açýk tut !!!", 20, Color.white, 0.02f);
            else
                sp.PrintDialogue("SYSTEM", "Hala bazý kitap sayfalarý kayýp... Kayýp sayfalarý bulmak için gözünü açýk tut !!!", 20, Color.white, 0.02f);
        }
    }
    public void PreviousPage()
    {
        am.sfxSource.Stop();
        if (leftflipable && currentPage > 1)
        {
            am.PlaySFX(pageFlip);
            Invoke("closePages", 0.1f);
            bookAnim.SetBool("onLastPage", false);
            rightflipable = true;
            currentPage--;
            bookAnim.SetBool("flipReversed",true);
            leftPage.text = "";
            rightPage.text = "";
            Invoke("setPageText", 0.3f);
        }
        else if(leftflipable)
        {
            am.PlaySFX(pageFlip);
            Invoke("closePages", 0.1f);
            currentPage--;
            bookAnim.SetBool("flipReversed", true);
            leftflipable = false;
            leftPage.text = "";
            rightPage.text = "";
            Invoke("setPageText", 0.3f);
        }
        else
        {
            sp.PrintDialogue("ESRA", "Buranin öncesinde sayfalara tek tarafli yazmiþim sinirden açamýyorum:/", 20, Color.white, 0.02f);
        }
    }
    private void inv()
    {
        bookAnim.SetBool("onLastPage", true);
    }
    private void closePages()
    {
        left.SetActive(false);
        right.SetActive(false);
    }
    private void openPages()
    {
        left.SetActive(true);
        right.SetActive(true);
    }
    private void setPageText()
    {
        openPages();
        spa();
        if(a == true)
        {
            bookAnim.SetBool("flipReversed", false);
            bookAnim.SetBool("flip", false);
        }
        if (a == false)
        {
            a = true;
        }
    }
    private void activateImage(int imageID, char pageSide)
    {
        Sprite imToAct = pageImages[imageID];
        if (pageSide == 'R')
        {
            rightFull.enabled = true;
            rightFull.sprite = imToAct;
        }
        if (pageSide == 'L')
        {
            leftFull.enabled = true;
            leftFull.sprite = imToAct;
        }
    }
    public void spa()
    {
        bool rightIsImage = Regex.IsMatch(rightPageTexts[currentPage], @"^I(0[0-9]|1[0-9]|2[0-9]|30)(L|R)$");
        bool leftIsImage = Regex.IsMatch(leftPageTexts[currentPage], @"^I(0[0-9]|1[0-9]|2[0-9]|30)(L|R)$");

        if (rightIsImage)
        {
            int imageID = int.Parse(rightPageTexts[currentPage].Substring(1, 2));
            char PageSide = rightPageTexts[currentPage][3];
            rightPage.text = " ";
            activateImage(imageID, PageSide);
            if (!leftIsImage)
            {
                leftFull.enabled = false;
                leftPage.text = leftPageTexts[currentPage];
            }
        }
        if (leftIsImage)
        {
            int imageID = int.Parse(leftPageTexts[currentPage].Substring(1, 2));
            char PageSide = leftPageTexts[currentPage][3];
            leftPage.text = " ";
            activateImage(imageID, PageSide);

            if (!rightIsImage)
            {
                rightFull.enabled = false;
                rightPage.text = rightPageTexts[currentPage];
            }
        }
        if (!leftIsImage && !rightIsImage)
        {
            leftFull.enabled = false;
            rightFull.enabled = false;
            rightPage.text = rightPageTexts[currentPage];
            leftPage.text = leftPageTexts[currentPage];
        }
    }
}
