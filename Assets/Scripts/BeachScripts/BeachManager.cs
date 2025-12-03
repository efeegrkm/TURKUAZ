using UnityEngine;

public class BeachManager : MonoBehaviour
{
    public Camera mainCamera;
    public static BeachManager Instance;
    [SerializeField] GameObject leftBeachRoot;
    [SerializeField] GameObject rightBeachRoot;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }else
        {
            Destroy(this);
        }
    }
    public void activeBeachPart(char c)
    {
        if (c == 'L')
        {
            leftBeachRoot.SetActive(true);
            rightBeachRoot.SetActive(false);
        }
        else if (c == 'R')
        {
            leftBeachRoot.SetActive(false);
            rightBeachRoot.SetActive(true);
        }
    }
}
