using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public ScriptPrinter sp;
    public static int cdCount = 2;
    public AudioSource ambianceSource;
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public int currentCD = 0;//0 for none, 1 for CD1, 2 for CD2
    [SerializeField] 
    private List<AudioClip> CD1Musics = new List<AudioClip>();
    [SerializeField]
    private List<string> CD1musicInfos = new List<string>(); 
    [SerializeField]
    private List<AudioClip> CD2Musics = new List<AudioClip>();
    [SerializeField]
    private List<string> CD2musicInfos = new List<string>();
    public int CD1Index = 0;
    public int CD2Index = 0;
    public int CurrentSong = -1;
    public bool noMusic = true;
    [SerializeField]
    private GameObject infSpace;
    [SerializeField]
    private TextMeshProUGUI authorTMP;
    [SerializeField]
    private TextMeshProUGUI musicTMP;

    [SerializeField]
    private List<AudioClip> ambiances = new List<AudioClip>(2);

    public void Awake()
    {
        if(Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    public void nextMusic()
    {
        switch(currentCD)
        {
            case 1:
                displayMusicInfo(CD1musicInfos[CD1Index]);
                CurrentSong = CD1Index;
                musicSource.GetComponent<AudioSource>().clip = CD1Musics[CD1Index];
                musicSource.GetComponent<AudioSource>().Play();
                if (CD1Index < CD1Musics.Count-1)
                {
                    CD1Index++;
                }
                else
                {
                    CurrentSong = -1;
                    CD1Index = 0;
                    noMusic = false;
                }
                break;
            case 2:
                displayMusicInfo(CD2musicInfos[CD2Index]);
                CurrentSong = CD2Index + CD1Musics.Count-1;
                musicSource.GetComponent<AudioSource>().clip = CD2Musics[CD2Index];
                musicSource.GetComponent<AudioSource>().Play();
                if (CD2Index < CD2Musics.Count-1)
                {
                    CD2Index++;
                }
                else
                {
                    CurrentSong = -1;
                    CD2Index = 0;
                    noMusic = false;
                }
                break;
            default:
                sp.PrintDialogue("SYSTEM","CD takili degil.");
                break;
        }
    }
    public void displayMusicInfo(string musicInfo)
    {
        infSpace.SetActive(false);
        string[] inf = musicInfo.Split('/');
        if (inf.Length != 2)
        {
            throw new InvalidOperationException("Wrong music info format");
        }
        string author = inf[1];
        string songName = inf[0];
        authorTMP.text = author;
        musicTMP.text = songName;
        infSpace.SetActive(true);
        infSpace.GetComponent<Animator>().SetTrigger("turn");
    }
    public void lowerMusicVolume()
    {
        musicSource.GetComponent<AudioSource>().volume = 0.3f;
    }
    public void raiseMusicVolume()
    {
        musicSource.GetComponent<AudioSource>().volume = 1f;
    }
    public void lowerAmbianceVolume()
    {
        ambianceSource.GetComponent<AudioSource>().volume = 0.5f;
    }
    public void raiseAmbianceVolume()
    {
        ambianceSource.GetComponent<AudioSource>().volume = 1f;
    }
    public void ChangeAmbiance(int ambIndex)
    {
        ambianceSource.GetComponent<AudioSource>().clip = ambiances[ambIndex];
        ambianceSource.GetComponent<AudioSource>().Play();
    }
    public void PlayMusic(AudioClip newClip)
    {
        musicSource.GetComponent<AudioSource>().clip = newClip;
        musicSource.GetComponent<AudioSource>().Play();
    }
    public void PlaySFX(AudioClip newClip)
    {
        sfxSource.GetComponent<AudioSource>().PlayOneShot(newClip);
    }
}
