using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Splash : MonoBehaviour
{
    [SerializeField] private VideoPlayer player;
  //  [SerializeField] private Image loadingBarImage;
    //[SerializeField] private Transform icono;
    [SerializeField] private Slider slider;
    [SerializeField] private VideoPlayer backGround;
    [SerializeField] private Sprite[] backGrounds;
    private int currentSpriteIndex = 0;

    private void OnEnable() => player.loopPointReached += VideoFinished;
    private void Awake()
    {
        slider?.gameObject.SetActive(false);

        if (Application.platform == RuntimePlatform.WindowsPlayer
            || Application.platform == RuntimePlatform.WindowsPlayer
            || Application.platform == RuntimePlatform.WindowsServer)
        {
            string[] args = System.Environment.GetCommandLineArgs();

            if (args.Length == 13)
            {
                SceneManager.LoadScene(Managers.LevelManager.LAUNCH_SCENE);

                //LoadLaunchScene();
            }
            else
            {

                player?.Play();
            }
        }
        else
        {
            player?.Play();
        }
    }

    private void VideoFinished(VideoPlayer source) => LoadLaunchScene();
    public void LoadLaunchScene()=> StartCoroutine(LoadSceneAsync(Managers.LevelManager.LAUNCH_SCENE));

    Coroutine first, second;
    private IEnumerator LoadSceneAsync()
    {
        backGround.gameObject.SetActive(true);
        slider.gameObject.SetActive(true);
        yield return new WaitForSeconds(0);

    }

    private IEnumerator LoadSceneAsync(int sceneId)
    {
        first= StartCoroutine(LoadSceneAsync());
        yield return new WaitForSeconds(0);
        
        StartCoroutine(LoadSceneAsync(sceneId, true));
        yield return null;
    }
    private IEnumerator LoadSceneAsync(int sceneId, bool t)
    {

        
        //yield return new WaitForSeconds(2);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        operation.priority = 0;
        operation.allowSceneActivation = false;
        

        float elapsedTime = 0f;
        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / .9f);
            CLog.Log("PROGRESO: " + operation.progress+" - "+ progressValue);

         //   icono.localPosition = new Vector3(-500 + 1000 * progressValue, icono.localPosition.y, icono.localPosition.z);
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= 2f)
            {
                //currentSpriteIndex = (currentSpriteIndex + 1) % backGrounds.Length;
                //backGround.GetComponent<Image>().sprite = backGrounds[currentSpriteIndex];
                
                elapsedTime = 0f;
            }
            slider.value = progressValue;
            yield return null;
            if (!backGround.isPlaying)
                operation.allowSceneActivation = true;

        }
    }
}
