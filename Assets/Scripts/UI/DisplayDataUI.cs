using System.Collections;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;

public class DisplayDataUI : MonoBehaviour
{



    public static DisplayDataUI instance;
    public enum estado { unfolded, folded }
    public int currentLvlOnDisable;
    [Header("GUI Menu Display Objects")]
    #region GUIDISPLAY
    public TextMeshProUGUI GuiDisplayTel;
    public TextMeshProUGUI GuiDisplayTnl;
    public TextMeshProUGUI GuiDisplayCups;
    public TextMeshProUGUI GuiDisplayVT;
    public Image GuiImageProfile;
    public TextMeshProUGUI GuiDisplayName;
    public TextMeshProUGUI GuiDisplayLevel;
    public TextMeshProUGUI GuiDisplayLevelPercent;
    public UnityEngine.UI.Image GuiDisplayLevelBar;
    public UnityEngine.UI.Slider SliderPercent;
    public string imageName;
    public bool checStatus;
    public GameObject popUpTEL;
    public ProfileImageAvatar _profileAvatars;
    public GameObject warningAccNotLinkedIcon;
    public GameObject background;
    [Header("MOVE EFFECTS")]
    public DOMoveEffect[] DOMoveItem;
    #endregion
    public void startDisplay()
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
        //for (int i = 0; i < DOMoveItem.Length; i++)
        //{
        //    DOMoveItem[i].originalPosition = DOMoveItem[i].itemObject.transform.position;
        //}
        //for (int i = 0; i < DOMoveItem.Length; i++)
        //{
        //    DOMoveItem[i].initialPosition = (Vector2)DOMoveItem[i].itemObject.transform.position + DOMoveItem[i].moveToPosition;
        //}
    }

    public void showProfile()
    {
        ProfileInfo.instance.GetInfoProfile();
    }
    private void Start()
    {
            Debug.Log($"Sin Avatar1");
        if (string.IsNullOrEmpty(PlayfabManager.instance.urlAvatar))
        {
            setImageAvatar("2");
            _profileAvatars.gameObject.SetActive(true);
            ProfileImageAvatar.instance.currentUrlImage = "2";
            ProfileImageAvatar.instance.changePlayfavAvatar();
            Debug.Log($"Sin Avatar");

        }
        else
        {
            setImageAvatar(PlayfabManager.instance.urlAvatar);
            Debug.Log($"Sin Avatar2 '{PlayfabManager.instance.urlAvatar}'");
        }
        _profileAvatars.gameObject.SetActive(false);

        currentLvlOnDisable = PlayfabManager.instance.getLevel();
    }
    public void setImageAvatar(string url)
    {
        InterfaceManager.Instance.setImage(url, GuiImageProfile);
        //StopAllCoroutines();
        //StartCoroutine(loadaLLImageProfile(url));
    }
    //IEnumerator loadaLLImageProfile(string MediaUrl)
    //{
    //
    //
    //    request = UnityWebRequestTexture.GetTexture(MediaUrl);
    //    yield return request.SendWebRequest();

    //        Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
    //        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
    //        GuiImageProfile.GetComponent<Image>().overrideSprite = sprite;

    //}

    //IEnumerator loadaLLImageProfile(string _url)
    //{
    //    WWW www = new WWW(_url);
    //    yield return www;
    //    if (!File.Exists(Application.persistentDataPath + imageName))
    //    {
    //        if (www.error == null)
    //        {
    //            Texture2D texture = www.texture;
    //            GuiImageProfile.GetComponent<RawImage>().texture = texture;
    //            byte[] dataByte = texture.EncodeToPNG();
    //            File.WriteAllBytes(Application.persistentDataPath + imageName + "png", dataByte);

    //        }
    //    }
    //    else
    //    if (File.Exists(Application.persistentDataPath + imageName))
    //    {
    //        byte[] uploadByte = File.ReadAllBytes(Application.persistentDataPath + imageName);
    //        Texture2D texture = new Texture2D(10, 10);
    //        texture.LoadImage(uploadByte);
    //        GuiImageProfile.GetComponent<RawImage>().texture = texture;
    //    }

    //}
    public void nextScreen(UIScreen screen)
    {
        //startMoveEffects(estado.folded);
        //GamePlayerPrefs.instance.itemsStartPositionAllScreensAnimated();
        background.SetActive(true);
        startMoveEffects(estado.folded);
        background.GetComponent<RawImage>().DOFade(0f, 0);
        background.GetComponent<RawImage>().DOFade(1f, 0.5f).OnComplete(() => changeScreen(screen));
    }
    void changeScreen(UIScreen screen)
    {
        background.SetActive(false);
        UIScreen.Focus(screen);
    }
    public void startMoveEffects(estado estadoActual)
    {
        if (estadoActual == estado.unfolded)
        {
            for (int i = 0; i < DOMoveItem.Length; i++)
            {
                RectTransform r2 = DOMoveItem[i].itemObject.GetComponent<RectTransform>();
                r2.DOAnchorPos(DOMoveItem[i].originalPosition, DOMoveItem[i].expandDuration).SetEase(DOMoveItem[i].expandeEffect).SetDelay(DOMoveItem[i].delayIn);
            }
        }
        else if (estadoActual == estado.folded)
        {
            for (int i = 0; i < DOMoveItem.Length; i++)
            {
                RectTransform r2 = DOMoveItem[i].itemObject.GetComponent<RectTransform>();
                r2.DOAnchorPos(DOMoveItem[i].initialPosition, DOMoveItem[i].expandDuration).SetEase(DOMoveItem[i].expandeEffect).SetDelay(DOMoveItem[i].delayOut);

                //DOMoveItem[i].itemObject.transform.DOMove(DOMoveItem[i].initialPosition, DOMoveItem[i].expandDuration).SetEase(DOMoveItem[i].expandeEffect).SetDelay(DOMoveItem[i].delay);
            }
        }
        //if (estadoActual == estado.unfolded)
        //{
        //    for (int i = 0; i < DOMoveItem.Length; i++)
        //    {
        //        DOMoveItem[i].itemObject.transform.DOMove(DOMoveItem[i].originalPosition, DOMoveItem[i].expandDuration).SetEase(DOMoveItem[i].expandeEffect).SetDelay(DOMoveItem[i].delayIn);
        //    }
        //}
        //else if (estadoActual == estado.folded)
        //{
        //    for (int i = 0; i < DOMoveItem.Length; i++)
        //    {
        //        DOMoveItem[i].itemObject.transform.DOMove(DOMoveItem[i].initialPosition, DOMoveItem[i].collapseDuration).SetEase(DOMoveItem[i].collapseEffect);
        //    }
        //}
    }
    private void OnEnable()
    {
        _profileAvatars.loadImageProfiles();
        StopAllCoroutines();
        StartCoroutine(enableScreen());
        //checkLevel();
    }
    IEnumerator enableScreen()
    {
        yield return new WaitForSeconds(0.3f);
        itemsStartPosition();
        startMoveEffects(estado.unfolded);
    }
    public void itemsStartPosition()
    {
        for (int i = 0; i < DOMoveItem.Length; i++)
        {
            DOMoveItem[i].itemObject.transform.position = DOMoveItem[i].initialPosition;
        }
    }

    public int getTel()
    {
        return string.IsNullOrEmpty(GuiDisplayTel.text) ? 0 : int.Parse(GuiDisplayTel.text);
    }


    Coroutine status = null;
    private void LateUpdate()
    {
        if (checStatus)
        {
            checStatus = false;
            if (status == null&&!GameLauncher.instance.isServer) status = StartCoroutine(statusCoins());
        }



    }

    int lastTLN = -1;
    int lastXP = -1;
    int lastTEL = -1;
    int lastCup = -1;
    int lasthearth = -1;


    IEnumerator statusCoins()
    {
        yield return new WaitWhile(() => !PlayFab.PlayFabClientAPI.IsClientLoggedIn());

        PlayfabClientCurrency.GetCurrencyTel();
        yield return new WaitWhile(() => DataEconomy.ECONOMYSTATUS == EconomyStatus.DOWNLOADING);
        Busines.GetExpAndCoups();
        yield return new WaitWhile(() => DataEconomy.ECONOMYSTATUS == EconomyStatus.DOWNLOADING);

        if (DataEconomy.ECONOMYSTATUS == EconomyStatus.OK)
        {
            //AudioManager.PlayAndFollow("coinSFX", behaviour.transform, AudioManager.MixerTarget.SFX);
            //CLog.Log("TRIAGO: " + PlayfabManager.instance.TEL + " - " + PlayfabManager.instance.TNL);
            /*if (PlayfabManager.instance.TEL == 0)
            {
                yield return new WaitForSeconds(1);
                status = null;
                //checStatus = true;
                yield break;
            }*/
            AudioManager.Play("coinSFX", AudioManager.MixerTarget.UI);
            //GuiDisplayTel.text = PlayfabManager.instance.TEL.ToString();

            if (lastTLN != PlayfabManager.instance.TNL)
            {
                StartCoroutine(UpdateStats(GuiDisplayTnl, null, lastTLN, PlayfabManager.instance.TNL));
                lastTLN = PlayfabManager.instance.TNL;
            }

            if (lastCup != PlayfabManager.instance.Coups)
            {
                StartCoroutine(UpdateStats(GuiDisplayCups, null, lastCup, PlayfabManager.instance.Coups));
                lastCup = PlayfabManager.instance.Coups;
            }

            if (lastXP != PlayfabManager.instance.Xp)
            {
                StartCoroutine(UpdateStats(null, GuiDisplayLevelBar, lastXP, PlayfabManager.instance.Xp));
                lastXP = PlayfabManager.instance.Xp;
                Invoke("checkLevel",1F);
            }
            if (lasthearth != PlayfabManager.instance.VidasIntentos)
            {
                StartCoroutine(UpdateStats(GuiDisplayVT, null, lasthearth, PlayfabManager.instance.VidasIntentos));
                lasthearth = PlayfabManager.instance.VidasIntentos;
            }
            if (lastTEL != PlayfabManager.instance.TEL)
            {
                StartCoroutine(UpdateStats(GuiDisplayTel, null, lastTEL, PlayfabManager.instance.TEL));
                lastTEL = PlayfabManager.instance.TEL;
            }

            GuiDisplayName.text = PlayfabManager.instance.displayName;
            UpdateLinkedAccountWarning();
            yield return new WaitForSeconds(1);
        }
        else
        {

        }
        status = null;
        enabled = false;

    }

    private string changeFormat(float lastTLN)
    {
        if (lastTLN > 9999)
        {
            double valorEnMiles = (double)lastTLN / 1000;
            return valorEnMiles.ToString("0.0") + "K";
        }
        return lastTLN.ToString();
    }

    public void checKStatus()
    {
        checStatus = true;
        enabled = true;
    }

    public void UpdateLinkedAccountWarning()
    {
        
        if (PlayfabManager.instance.isGuest())
        {
            warningAccNotLinkedIcon.SetActive(true);
        }
        else
        {
            warningAccNotLinkedIcon.SetActive(false);
        }
    }

    IEnumerator UpdateStats(TMP_Text _text, UnityEngine.UI.Image _bar, float _lastValu, int _targetValue)
    {
        float marginLevel = 0; ;
        float actual;
        int lastLevel;
        setValues();
       
        while (true)
        {
            // CLog.Log("MANDE APAGAR " + _text + " "+ _lastValu + " " + _targetValue);

            _lastValu = Mathf.Lerp(_lastValu, _targetValue, Time.deltaTime * 4);

            if (_targetValue - _lastValu <= 1)
                _lastValu = _targetValue;



            //if (_text != null) _text.text = ((int)_lastValu).ToString();
            if (_text != null) _text.text = changeFormat(_lastValu);
            else
            {
                _lastValu = Mathf.Lerp(_lastValu, _targetValue, Time.deltaTime * 4);
                float tmp = (_lastValu - PlayfabManager.levels[PlayfabManager.instance.getLevel()]);
                _bar.fillAmount = tmp / marginLevel;
                SliderPercent.value = _bar.fillAmount;
                GuiDisplayLevelPercent.text = (float)Math.Round(SliderPercent.value, 2) * 100 + " %";
                GuiDisplayLevel.text = PlayfabManager.instance.getLevel().ToString();

                //// Apagar pantalla de carga
                PlayfabManager.instance.loadingScreen.GetComponent<LoadingPointsAnim>().hide();

            }


            yield return 0;
            if (_lastValu == _targetValue) yield break;
        }

        void setValues()
        {
            if (_bar)
            {
                CLog.Log("Level es: " + PlayfabManager.instance.getLevel() + " " + PlayfabManager.levels.Length);
                lastLevel = PlayfabManager.instance.getLevel();
                marginLevel = PlayfabManager.instance.getLevel() < PlayfabManager.levels.Length - 1 ?
                    PlayfabManager.levels[PlayfabManager.instance.getLevel() + 1] - PlayfabManager.levels[PlayfabManager.instance.getLevel()] : PlayfabManager.levels[PlayfabManager.instance.getLevel()] - PlayfabManager.levels[PlayfabManager.instance.getLevel() - 1];
                actual = _lastValu - PlayfabManager.levels[PlayfabManager.instance.getLevel()];
            }
        }

    }

    Coroutine popUp;
    public void buyTEL()
    {

#if UNITY_ANDROID || UNITY_IOS
        popUpTEL.SetActive(true);
#else
        if (popUp == null)
            StartCoroutine(buyTELCoroutine());
#endif
    }
    IEnumerator buyTELCoroutine()
    {
        //PopUpManager._instance.setPopUp("Atención", "Para compar TEL debes seguir el siguiente enlace: " + VersionNv.version.UrlBuyTel + "\n¿Continuar?", IconosPopUp.questioin, true);
        PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_WARNING), TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_BUYTEL).Replace("XXX", VersionNv.version.UrlBuyTel.ToString()), IconosPopUp.questioin, true);

        yield return new WaitWhile(() => PopUpManager._instance.popUpState == PopUpStates.Wait);
        if (PopUpManager._instance.popUpState == PopUpStates.Ok)
        {
            Application.OpenURL(VersionNv.version.UrlBuyTel);
        }
        popUp = null;
    }
    void checkLevel()
    {
        int clvl = PlayfabManager.instance.getLevel();
        if (clvl > currentLvlOnDisable)
        {
            currentLvlOnDisable = clvl;
            PlayfabManager.instance.congratsPanel.SetActive(true);
        }
    }
    
    private void OnDisable()
    {
        CLog.Log("MANDE APAGAR");
    }
}
