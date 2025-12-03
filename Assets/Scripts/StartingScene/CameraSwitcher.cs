using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public GameObject mainCam;
    public GameObject actionCam;
    private Animator actionAnim;
    public bool isMainCameraActive = false;
    [SerializeField] private GameObject pressToStart;
    public GameObject lInv;
    public GameObject rInv;
    void Start()
    {
        SetActionCamera();
        actionAnim = actionCam.GetComponent<Animator>();
    }

    public void SetMainCamera()
    {
        mainCam.SetActive(true);
        actionCam.SetActive(false);
    }

    public void SetActionCamera()
    {
        mainCam.SetActive(false);
        actionCam.SetActive(true);
    }
    public void PlayActionCameraAnimation()
    {
        actionAnim.SetTrigger("CamAct");
        Invoke("inv", 6f);
        pressToStart.SetActive(false);
    }
    private void inv()
    {
        isMainCameraActive = true;
        SetMainCamera();
        lInv.SetActive(true);
        rInv.SetActive(true);
    }
}
