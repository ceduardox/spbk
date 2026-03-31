using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public enum Graphics { VeryLow, Low, Medium, High, VeryHigh, Ultra }
public enum Language { Spanish, English, Portugues, Frances, Aleman, Chino }
public enum ScreeMode { Windows, FullScreen }

public class GamePlayerPrefs : MonoBehaviour
{
    public static GamePlayerPrefs instance;

    public Graphics _GraphicsQuality;
    public Language _LanguageGame;
    public ScreeMode _FullScreen;
    public SettingsUI Settings;

    public int linkedAccount;
    public int autologin;
    int GraphicsQuality;
    int LanguageGame;
    int FullScreen;

    public LoginScreen LoginUIScreen;
    public TMP_Dropdown languageDropDown;
    public GameObject Fllag;
    public Sprite[] flags;
    //public string[] avatar;
    //[Header("Animations Objects initial Position")]
    //public GameObject[] Screens;
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
        //setPositionsAllScreens();
    }

    void Start()
    {
        loadGraphicsInt();
        // Agregar el listener al evento onValueChanged
        Settings.gameObject.SetActive(true);
        SettingsUI.Instance.FullScreen.onValueChanged.AddListener(delegate {
            ToggleFullScreenValueChanged(SettingsUI.Instance.FullScreen.isOn);
        });

        // Cargar el estado de pantalla completa al iniciar
        bool isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1; // Valor predeterminado: pantalla completa
        Screen.fullScreen = isFullScreen;
        SettingsUI.Instance. FullScreen.isOn = isFullScreen;
        Settings.gameObject.SetActive(false);


        //languageDropDown.value = loadLanguageInt();
        Invoke("changeLanguageStart", 1f);
        //setPositionsAllScreens();

    }
    private void OnEnable()
    {
        CLog.Log(name);
        //loadLanguageInt();
        languageDropDown.value = loadLanguageInt();
    }
    public void closeSesion()
    {
        PlayerPrefs.SetInt("autolog",0);
        autologin = PlayerPrefs.GetInt("autolog");
        //LoginUIScreen.instance.autologin = autologin;
        PlayfabManager.instance.ForgetPlayFabCredentials();
        SetLinkedAccount(0);
        Save.saveLogin("", "");
        LoginUIScreen.LoginEmailInput.text = "";
        LoginUIScreen.LoginPasswordInput.text = "";
        CLog.Log(" PlayFab Credentials Cleaned");

        //ScreenManager.profileInfo.FocusScreen(ScreenManager.loginScreen);
        ScreenManager.instance.CloseAllScreens();
        ScreenManager.loginScreen.gameObject.SetActive(true);

    }
    public void saveGraphicsEnum(int value)
    {
        PlayerPrefs.SetInt("qualityIndex", value);
        GraphicsQuality = value;
        switch (GraphicsQuality)
        {
            case 0:
                _GraphicsQuality = Graphics.VeryLow;
                break;
            case 1:
                _GraphicsQuality = Graphics.Low;
                break;
            case 2:
                _GraphicsQuality = Graphics.Medium;
                break;
            case 3:
                _GraphicsQuality = Graphics.High;
                break;
            case 4:
                _GraphicsQuality = Graphics.VeryHigh;
                break;
            case 5:
                _GraphicsQuality = Graphics.Ultra;
                break;
        }
    }
    public int loadGraphicsInt()
    {
        GraphicsQuality = PlayerPrefs.GetInt("qualityIndex");
        if (GameLauncher.instance.isServer && GameLauncher.instance.modeServerDedicado)
            GraphicsQuality = 0;
        QualitySettings.SetQualityLevel(GraphicsQuality);
        switch (GraphicsQuality)
        {
            case 0:
                _GraphicsQuality = Graphics.VeryLow;
                return 0;
            case 1:
                _GraphicsQuality = Graphics.Low;
                return 1;
            case 2:
                _GraphicsQuality = Graphics.Medium;
                return 2;
            case 3:
                _GraphicsQuality = Graphics.High;
                return 3;
            case 4:
                _GraphicsQuality = Graphics.VeryHigh;
                return 4;
            case 5:
                _GraphicsQuality = Graphics.Ultra;
                return 5;
        }
        return 2;
    }
    public void saveLanguageEnum(int value)
    {
        PlayerPrefs.SetInt("languageIndex", value);
        LanguageGame = value;
        switch (LanguageGame)
        {
            case 0:
                _LanguageGame = Language.Spanish;
                break;
            case 1:
                _LanguageGame = Language.English;
                break;
            case 2:
                _LanguageGame = Language.Portugues;
                break;
            case 3:
                _LanguageGame = Language.Frances;
                break;
            case 4:
                _LanguageGame = Language.Aleman;
                break;
            case 5:
                _LanguageGame = Language.Chino;
                break;
        }
    }
    public int loadLanguageInt()
    {
        LanguageGame = PlayerPrefs.GetInt("languageIndex");
        //LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[LanguageGame];
        switch (LanguageGame)
        {
            case 0:
                _LanguageGame = Language.Spanish;
                TranslateUI.langActual = "es";
                TranslateUI.instance.changeLang();
                return 0;
            case 1:
                _LanguageGame = Language.English;
                TranslateUI.langActual = "en";
                TranslateUI.instance.changeLang();
                return 1;
            case 2:
                _LanguageGame = Language.Portugues;
                TranslateUI.langActual = "po";
                TranslateUI.instance.changeLang();
                return 2;
            case 3:
                _LanguageGame = Language.Frances;
                TranslateUI.langActual = "fr";
                TranslateUI.instance.changeLang();
                return 3;
            case 4:
                _LanguageGame = Language.Aleman;
                TranslateUI.langActual = "al";
                TranslateUI.instance.changeLang();
                return 4;
            case 5:
                _LanguageGame = Language.Chino;
                TranslateUI.langActual = "ch";
                TranslateUI.instance.changeLang();
                return 5;
        }
        return 0;
    }
    public void ToggleFullScreenValueChanged(bool isOn)
    {
        bool isFullScreen = SettingsUI.Instance.FullScreen.isOn;

        // Configurar el estado de pantalla completa
        Screen.fullScreen = isFullScreen;

        // Guardar el estado de pantalla completa
        PlayerPrefs.SetInt("FullScreen", isFullScreen ? 1 : 0);
        GamePlayerPrefs.instance.saveScreenEnum(isFullScreen ? 1 : 0);
        setFullScreen(isFullScreen);

        PlayerPrefs.Save();
    }


    public void saveScreenEnum(int value)
    {
        PlayerPrefs.SetInt("fullScreenIndex", value);
        FullScreen = value;
        switch (FullScreen)
        {
            case 0:
                _FullScreen = ScreeMode.Windows;
                break;
            case 1:
                _FullScreen = ScreeMode.FullScreen;
                break;
        }
    }
    public static void setFullScreen(bool _value)
    {
        if (GameLauncher.instance.modeServerDedicado)
        {
            Screen.fullScreen = false;
            return;
        }
       Screen.fullScreen = _value;

    }
    public void changeLanguageStart()
    {
        switch (languageDropDown.value)
        {
            case 0:
                TranslateUI.langActual = "es";
                break;
            case 1:
                TranslateUI.langActual = "en";
                break;
            case 2:
                TranslateUI.langActual = "po";
                break;
            case 3:
                TranslateUI.langActual = "fr";
                break;
            case 4:
                TranslateUI.langActual = "al";
                break;
            case 5:
                TranslateUI.langActual = "ch";
                break;
        }
        TranslateUI.instance.changeLang();
        instance.saveLanguageEnum(languageDropDown.value);
        Fllag.GetComponent<Image>().sprite = flags[languageDropDown.value];
    }

    //public void setPositionsAllScreens()
    //{
    //    for (int i = 0; i < Screens.Length; i++)
    //    {
    //        int j = 0;
    //        UIEffects uiscript = Screens[i].gameObject.GetComponent<UIEffects>();
    //        for (j = 0; j < uiscript.DOMoveItem.Length; j++)
    //        {
    //            uiscript.DOMoveItem[j].originalPosition = uiscript.DOMoveItem[j].itemObject.transform.localPosition;
    //        }
    //        for (j = 0; j < uiscript.DOMoveItem.Length; j++)
    //        {
    //            uiscript.DOMoveItem[j].initialPosition = uiscript.DOMoveItem[j].originalPosition + uiscript.DOMoveItem[j].moveToPosition;
    //        }
    //    }
    //}
    //public void itemsStartPositionAllScreens()
    //{
        
    //    for (int i = 0; i < Screens.Length; i++)
    //    {
    //        UIEffects uiscript = Screens[i].gameObject.GetComponent<UIEffects>();
    //        for (int j = 0; j < uiscript.DOMoveItem.Length; j++)
    //        {
    //            RectTransform r = uiscript.DOMoveItem[j].itemObject.GetComponent<RectTransform>();
    //            Vector2 anchore = r.anchoredPosition;
    //            anchore = uiscript.DOMoveItem[j].initialPosition;
    //            r.anchoredPosition = anchore;
    //            //uiscript.DOMoveItem[i].originalPosition = uiscript.DOMoveItem[i].itemObject.transform.localPosition;
    //        }

    //    }

    //}
    //public void itemsStartPositionAllScreensAnimated()
    //{
    //    //r2.DOAnchorPos(DOMoveItem[i].initialPosition, DOMoveItem[i].expandDuration).SetEase(DOMoveItem[i].expandeEffect).SetDelay(DOMoveItem[i].delayOut);
    //    for (int i = 0; i < Screens.Length; i++)
    //    {
    //        UIEffects uiscript = Screens[i].gameObject.GetComponent<UIEffects>();
    //        for (int j = 0; j < uiscript.DOMoveItem.Length; j++)
    //        {
    //            RectTransform r = uiscript.DOMoveItem[j].itemObject.GetComponent<RectTransform>();
    //            Vector2 anchore = r.anchoredPosition;
    //            anchore = uiscript.DOMoveItem[j].initialPosition;
    //            r.anchoredPosition = anchore;
    //            //r.DOAnchorPos(uiscript.DOMoveItem[j].originalPosition, uiscript.DOMoveItem[i].expandDuration).SetEase(uiscript.DOMoveItem[i].expandeEffect).SetDelay(uiscript.DOMoveItem[i].delayIn);
    //            //r.DOAnchorPos(uiscript.DOMoveItem[j].originalPosition, 0.5f).SetEase(Ease.InSine);
    //        }

    //    }

    //}
    //  GUARDAR COMO CUENTA LINKEADA, PARA NO LOGEAR COMO GUEST
    public void SetLinkedAccount(int intBool)
    {
        PlayerPrefs.SetInt("linkedAccount", intBool);
        linkedAccount = PlayerPrefs.GetInt("linkedAccount");
    }
    // CHEKEAR SI TIENE CUENTA LINKEADA
    public bool isLinkedAccount()
    {

        linkedAccount = PlayerPrefs.GetInt("linkedAccount");
        if (linkedAccount == 0) 
            return false;
        else
            return true;
    }


}
