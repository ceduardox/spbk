using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using SocketIO;

public class LoginUScreen : MasterScreen
{
    public static LoginUScreen instance;

    [Header("GUI Login Inputs Objects")]
    #region LOGIN
    public TMP_InputField LoginEmailInput;
    public TMP_InputField LoginPasswordInput;

    public Toggle tgAutologin;
    public int autologin;
    public Button btnLogin;
    public Button btnRegister;
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }

    private void Start()
    {
        tgAutologin.onValueChanged.AddListener(delegate { toggleAutologin(tgAutologin); });
        if (autologin == 1)
        {
            tgAutologin.isOn = true;
            PlayfabManager.instance.Login(LoginEmailInput.text, LoginPasswordInput.text);
        }
        else
        {
            tgAutologin.isOn = false;
        }
        //GamePlayerPrefs.instance.itemsStartPositionAllScreens();
    }
    bool block = false;
    void OnGUI()
    {

        if (Event.current.keyCode == KeyCode.Return && LoginEmailInput.text != "" && LoginPasswordInput.text != "")
        {
            if (!block)
            {
                LoginOnClick();
                block = true;
                Invoke("resetEnter", 2);
            }

        }

    }
    void resetEnter()
    {
        block = false;
    }
    void toggleAutologin(Toggle tg)
    {
        if (tg.isOn)
        {
            PlayerPrefs.SetInt("autolog", 1);
            GamePlayerPrefs.instance.autologin = 1;
        }
        else
        {
            PlayerPrefs.SetInt("autolog", 0);
            GamePlayerPrefs.instance.autologin = 0;

        }

    }
    bool firsTime = true;
    private void OnEnable()
    {
        
        autologin = PlayerPrefs.GetInt("autolog");
        if (firsTime)
        {
            
            RestoreButtons();
            CLog.Log("SOY: :" + name+" "+ PlayFab.PlayFabClientAPI.IsClientLoggedIn());
            if (PlayFab.PlayFabClientAPI.IsClientLoggedIn())
            {
              //  PlayfabManager.instance.registroLogSuccess(null);
            }
            firsTime = true;
        }
        else
        {
            if (GameLauncher.ConnectionStatus == ConnectionStatus.Connected)//|| GameLauncher.ConnectionStatus == ConnectionStatus.Failed)
            {
                if (LobbyUI._instance != null)
                    GetComponent<UIScreen>().FocusScreen(LobbyUI._instance.GetComponent<UIScreen>());
                else if (GameLauncher.instance != null && GameLauncher.instance.mainScreen != null)
                    GetComponent<UIScreen>().FocusScreen(GameLauncher.instance.mainScreen);
            }
            else
                GetComponent<UIScreen>().FocusScreen(GameLauncher.instance.mainScreen);
        }

        //GamePlayerPrefs.instance.autologin = PlayerPrefs.GetInt("autolog");

    }
   
    public void LoginOnClick()
    {
        LoginPref.sabePref();
        HiddenButtons();
        PlayfabManager.instance.Login(LoginEmailInput.text, LoginPasswordInput.text);
        
    }
   
    public void QuitGame()
    {
        Application.Quit();
    }

    public void HiddenButtons()
    {
        btnLogin.interactable = false;
        btnRegister.interactable = false;
    }
    public void RestoreButtons()
    {
        btnLogin.interactable = true;
        btnRegister.interactable = true;
    }
    public void cargarLoading()
    {
        Invoke("CargarLoading", 0.5f);
    }
    public void CargarLoading()
    {
        PlayfabManager.instance.loadingScreen.GetComponent<LoadingPointsAnim>().setLoading("Cargando5", 0);
        gameObject.SetActive(false);
        Invoke("forzarApagado", 3);
    }
    void forzarApagado()
    {
        PlayfabManager.instance.loadingScreen.GetComponent<LoadingPointsAnim>().hide();
    }
    
}
