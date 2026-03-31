using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AudioEnviromentControl : MonoBehaviour
{
    [SerializeField]
    private static int totalLaps = 0;
   
    public static int TotalLaps { 
        get { return totalLaps; } 

        set { 
            totalLaps = value;
            print(value);
        } 
    }



    private void Start()
    {
        
        AudioManager.PlayMusic("intro");
        OnLastLap += AddSpeedMusic;
        OnMenu += NormalSpeedMusic;
    }

    public static void menuEnabled(string music)
    {
            AudioManager.StopMusic();
            AudioManager.PlayMusic(music);
        
        
        //this.gameObject.GetComponent<AudioManager>().masterMixer.SetFloat("MusicPitch", 1.0f);

    }

    public static void trackEnabled(string music)
    {
        AudioManager.StopMusic();
        AudioManager.PlayMusic(music);

    }

    public static void trackDisabled(string music)
    {

        AudioManager.StopMusic();
        AudioEnviromentControl.OnMenu?.Invoke();
        AudioManager.PlayMusic("menu");

    }

    
    private void AddSpeedMusic(int lastLap)
    {
        if (lastLap==TotalLaps)
        {
            print(totalLaps );
            gameObject.GetComponent<AudioManager>().musicMixer.audioMixer.SetFloat("MusicPitch",1.04f);
        }

    }

    
    private void NormalSpeedMusic()
    {
        try
        {
            print(totalLaps);
            CLog.Log("IS ACTIVE: " + gameObject);

            if (gameObject &&
                gameObject.GetComponent<AudioManager>())
                gameObject.GetComponent<AudioManager>().musicMixer.audioMixer.SetFloat("MusicPitch", 1.0f);
            
        }
        catch  (System.Exception e)
        {
            CLog.LogError("ERROR: " + e); 
        }
    }


    private void OnDisable()
    {
        OnLastLap -= AddSpeedMusic;
        OnMenu -= NormalSpeedMusic;
    }
    private void OnDestroy()
    {
        OnLastLap -= AddSpeedMusic;
        OnMenu -= NormalSpeedMusic;
    }

    public delegate void LastLap(int a);
    public static LastLap OnLastLap;

    public delegate void menu();
    public static menu OnMenu;
}
