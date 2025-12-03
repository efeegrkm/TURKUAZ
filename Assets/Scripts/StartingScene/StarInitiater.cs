using UnityEngine;

public class StarInitiater : MonoBehaviour
{
    public static StarInitiater Instance;
    SceneTransitionManager stm;
    [Header("Prefab")]
    public GameObject fallingStarPrefab;
    public GameObject inVan;
    public GameObject inTent;

    [Header("Spawn Settings")]
    public float spawnInterval = 3f;
    public float inVanspawnInterval = 5f;
    public float normspawnInterval = 3f;

    private float timer = 0f;
    public CameraSwitcher cameraSwitcher;
    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
        stm = SceneTransitionManager.Instance;
    }
    void Update()
    {
        if (inVan.activeSelf)
            spawnInterval = inVanspawnInterval;
        else 
            spawnInterval = normspawnInterval;
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnStar();
            timer = 0f;
        }
    }

    void SpawnStar()
    {
        int curSceneID = stm.currentSceneID;
        float randomX;
        float randomY;
        if (curSceneID == 0 && inVan.activeSelf||inTent.activeSelf)
        {
            randomX = Random.Range(0f, 35f);
            randomY = Random.Range(0f, 33f);
            Vector3 spawnPos = new Vector3(randomX, randomY, 0f);
            GameObject star = Instantiate(fallingStarPrefab, spawnPos, Quaternion.identity);

            star.GetComponent<SpriteRenderer>().sortingOrder = 5;
            FallingStar fallingStar = star.GetComponent<FallingStar>();
            if (fallingStar != null)
            {
                fallingStar.fallSpeed = Random.Range(40f, 60f);
                fallingStar.fallDuration = 1f;
            }
        }
        else if (curSceneID == 1 || cameraSwitcher.isMainCameraActive)
        {
            randomX = Random.Range(0f, 120f);

            Vector3 spawnPos = new Vector3(randomX, 30f, 0f);

            GameObject star = Instantiate(fallingStarPrefab, spawnPos, Quaternion.identity);

            FallingStar fallingStar = star.GetComponent<FallingStar>();
            if (fallingStar != null)
            {
                fallingStar.fallSpeed = Random.Range(50f, 70f);
                fallingStar.fallDuration = Random.Range(1f, 3.5f);
            }
        }
    }
}
