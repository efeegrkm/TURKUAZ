using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
//Inventory canvas main camera atamasý yapýlacak.

public class SceneTransitionManager : MonoBehaviour
{
    // 1. Singleton Eriþimi
    public static SceneTransitionManager Instance;
    private BeachManager bmInstance;
    private ClickDetector cdInstance;
    private InventoryManager imInstance;
    private GameObject blackoutCanvas;
    [SerializeField] private GameObject eventsystem;

    [Header("Ayarlar")]
    [Tooltip("Blackout animasyonunu kontrol eden Animator")]
    [SerializeField] private Animator blackoutImage;
    [SerializeField] private AudioClip stepsSFX;
    [SerializeField] private AudioManager am;

    [Tooltip("Animasyonun tam kararmasý veya açýlmasý için gereken süre (Saniye)")]
    [SerializeField] public float transitionDuration = 3.5f;
    [SerializeField] private float stepsDelay = 1.5f;

    // Durum Yönetimi
    private Dictionary<int, bool> sceneLoaded = new Dictionary<int, bool>();
    private GameObject CurrentSceneRoot;
    public int currentSceneID = 0;

    // Olay senkronizasyonu için geçici ID tutucu
    private int sceneToActivateID = -1;

    public void Awake()
    {
        // --- Singleton ve Kalýcýlýk Ayarlarý ---
        bmInstance = BeachManager.Instance;
        cdInstance = ClickDetector.ClickDetectorInstance;
        blackoutCanvas = blackoutImage.gameObject.transform.parent.gameObject;
        imInstance = InventoryManager.Instance;

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(blackoutImage.transform.parent.gameObject);
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(am.gameObject);
        DontDestroyOnLoad(eventsystem);

        // --- Olay Aboneliði ---
        // Yeni sahneler yüklendiðinde kök objeyi yakalamak için
        SceneManager.sceneLoaded += OnSceneLoaded;

        // --- Baþlangýç Durumu ---
        // 0-10 arasý sahneler için dictionary'i baþlat
        for (int i = 0; i <= 10; i++)
        {
            sceneLoaded[i] = false;
        }

        // Oyunun baþladýðý mevcut sahneyi kaydet
        int startingSceneId = SceneManager.GetActiveScene().buildIndex;
        if (startingSceneId >= 0 && startingSceneId <= 10)
        {
            sceneLoaded[startingSceneId] = true;
            CurrentSceneRoot = GetSceneRoot(startingSceneId);
        }
    }

    private void OnDestroy()
    {
        // Memory leak olmamasý için olay aboneliðini kaldýr
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // --- Olay Yöneticisi: Sahne Yüklendiðinde Çalýþýr ---
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Sahne açýldý.");
        // Sadece bizim Additive olarak yüklediðimiz ve beklediðimiz sahne ise çalýþýr
        if (mode == LoadSceneMode.Additive && scene.buildIndex == sceneToActivateID)
        {
            int bind = scene.buildIndex;
            GameObject targetRoot = GetSceneRoot(bind);
            
            if (targetRoot != null)
            {
                // Yeni sahneyi görünür yap
                targetRoot.SetActive(true);
                // Sistemi güncelle
                CurrentSceneRoot = targetRoot;
            }
            camDeployForNewScene(scene);
            // ------------------------------------
            // ID'yi sýfýrla, iþimiz bitti
            sceneToActivateID = -1;
        }
    }
    private void sceneOpenActions(int bin)
    {
        switch (bin)
        {
            case 0:
                am.ChangeAmbiance(0);
                currentSceneID = 0;
                B_ClickDetector.instance.NotifyActionFinished();
                break;
            case 1:
                am.ChangeAmbiance(1);
                currentSceneID = 1;
                B_ClickDetector.instance.NotifyActionFinished();
                break;
        }
    }
    private void camDeployForNewScene(Scene scene)
    {
        //----------Kamera atamasý-------------
        if (bmInstance == null)
        {
            bmInstance = BeachManager.Instance;
        }
        if (cdInstance == null)
        {
            cdInstance = ClickDetector.ClickDetectorInstance;
        }
        if (imInstance == null)
        {
            imInstance = InventoryManager.Instance;
        }
        if (scene.buildIndex == 1)
        {
            blackoutCanvas.GetComponent<Canvas>().worldCamera = bmInstance.mainCamera;
            cdInstance.inventoryCanvas.worldCamera = bmInstance.mainCamera;
        }
        else if (scene.buildIndex == 0)
        {
            blackoutCanvas.GetComponent<Canvas>().worldCamera = cdInstance.mainCamera;
            cdInstance.inventoryCanvas.worldCamera = cdInstance.mainCamera;
        }
        sceneOpenActions(scene.buildIndex);
    }
    // --- Dýþarýdan Çaðrýlan Ana Metot ---
    public void SceneTransitionTo(int id)
    {
        // Eðer zaten o sahnedeysek iþlem yapma (Opsiyonel güvenlik)
        Scene aktifSahne = SceneManager.GetActiveScene();
        if (aktifSahne.IsValid() && aktifSahne.buildIndex == id) return;

        StartCoroutine(SceneTransitionToCoroutine(id));
    }

    // --- Ana Geçiþ Mantýðý (Coroutine) ---
    private IEnumerator SceneTransitionToCoroutine(int id)
    {
        // ADIM 1: Ekraný Karart (Fade In)
        fadeIn();
        yield return new WaitForSeconds(transitionDuration);
        // --- EKRAN ÞU AN SÝYAH, GÜVENLE DEÐÝÞÝKLÝK YAPABÝLÝRÝZ ---

        // ADIM 2: Mevcut Sahneyi Dondur (Disable)
        if (CurrentSceneRoot != null)
        {
            CurrentSceneRoot.SetActive(false);
        }

        // ADIM 3: Yeni Sahneyi Hazýrla
        if (sceneLoaded.ContainsKey(id) && sceneLoaded[id])
        {
            // A) SAHNE ZATEN YÜKLÜ:
            // Bellekten çaðýr, etkinleþtir ve aktif sahne yap
            Scene hedefSahne = SceneManager.GetSceneByBuildIndex(id);
            GameObject targetRoot = GetSceneRoot(id);

            if (targetRoot != null)
            {
                targetRoot.SetActive(true);
                CurrentSceneRoot = targetRoot; // Root'u güncelle
            }
            SceneManager.SetActiveScene(hedefSahne);
            camDeployForNewScene(hedefSahne);
        }
        else
        {
            // B) SAHNE YÜKLÜ DEÐÝL:
            // OnSceneLoaded olayýnýn bu sahneyi beklemesini söyle
            sceneToActivateID = id;

            // Arka planda yüklemeyi baþlat ve bitene kadar bekle
            yield return SceneManager.LoadSceneAsync(id, LoadSceneMode.Additive);

            // Yükleme bittiðinde 'OnSceneLoaded' otomatik çalýþacak,
            // root'u bulacak ve 'CurrentSceneRoot'u güncelleyecektir.

            sceneLoaded[id] = true;
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(id));
        }

        // Ufak bir bekleme (Sahne oluþturma iþlemlerinin render thread'e yansýmasý için)
        yield return null;

        // ADIM 4: Ekraný Aç (Fade Out)
        if (blackoutImage != null)
        {
            fadeOut();
            yield return new WaitForSeconds(transitionDuration);
        }

    }
    public void fadeIn()
    {
        if (blackoutImage != null)
        {
            StartCoroutine(fadeInCo());
        }
    }
    public void fadeOut()
    {
        if (blackoutImage != null)
        {
            StartCoroutine(fadeOutCO());
        }
    }
    public IEnumerator fadeInCo()
    {
        blackoutImage.SetBool("Blackout", true);
        yield return new WaitForSeconds(stepsDelay);
        am.PlaySFX(stepsSFX);
        yield return new WaitForSeconds(transitionDuration - stepsDelay);
        yield break;
    }
    public IEnumerator fadeOutCO()
    {
        am.PlaySFX(stepsSFX);
        yield return new WaitForSeconds(stepsDelay);
        blackoutImage.SetBool("Blackout", false);
        yield return new WaitForSeconds(transitionDuration-stepsDelay);
        yield break;
    }
    // --- Yardýmcý Metot: Sahne Kökünü Bulma ---
    private GameObject GetSceneRoot(int id)
    {
        Scene hedefSahne = SceneManager.GetSceneByBuildIndex(id);
        if (!hedefSahne.IsValid()) return null;

        GameObject[] rootObjects = hedefSahne.GetRootGameObjects();
        foreach (GameObject rootObj in rootObjects)
        {
            if (rootObj.CompareTag("SceneRoot"))
            {
                return rootObj;
            }
        }
        Debug.LogError($"Sahne ID {id} içinde 'SceneRoot' etiketli obje bulunamadý!");
        return null;
    }
}