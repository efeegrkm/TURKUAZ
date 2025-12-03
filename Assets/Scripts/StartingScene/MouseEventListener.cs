using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
public class MouseEventListener : MonoBehaviour
{
    public Image startImage;
    public Animator blackoutAnim;
    public Animator caravanAnim;
    public float duration = 1f;    
    [SerializeField] bool isVisible = false;

    public void startButton()
    {
        // araba çalıştırma ses efekti başlat.
        StartCoroutine(FadeOut());
        Invoke("caravanStart", 1f);
        Invoke("blackoutStart", 8f);
        Invoke("nextScene", 11f);
    }
    void nextScene()
    {
        SceneManager.LoadScene("InCarScene");
    }
    void caravanStart()
    {
        caravanAnim.SetTrigger("StartRun");
    }
    void blackoutStart()
    {
        blackoutAnim.SetTrigger("Blackout");
    }
    public void moonButton()
    {
        StopAllCoroutines(); // Önceki fade işlemi varsa iptal et

        if (isVisible)
        {
            StartCoroutine(FadeOut());
            isVisible = false;
        }
        else
        {
            StartCoroutine(FadeIn());
            isVisible = true;
        }

    }

    IEnumerator FadeIn()
    {
        Color color = startImage.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / duration); // 0 → 1
            startImage.color = color;
            yield return null;
        }

        color.a = 1f;
        startImage.color = color;
    }

    IEnumerator FadeOut()
    {
        Color color = startImage.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(1f - (elapsed / duration)); // 1 → 0
            startImage.color = color;
            yield return null;
        }

        color.a = 0f;
        startImage.color = color;
    }
}
