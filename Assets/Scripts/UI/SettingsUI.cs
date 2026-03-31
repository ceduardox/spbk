using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using TMPro;
using System;
//using UnityEngine.Localization;
//using UnityEngine.Localization.Settings;

public class SettingsUI : MonoBehaviour
{

    public static SettingsUI Instance;
    public TMP_Dropdown QualityGraphics;
    public TMP_Dropdown LanguageGame;
    public Toggle FullScreen;

    [Header("BUTTONS MENU CURRENT")]
    public ButtonMenu[] MenuNavegation;

    [System.Serializable]
    public struct ButtonMenu
    {
        public Button BtnMenu;
        public GameObject PanelMenu;
    }

    public GameObject BotonConfig;
    public bool EstaEnAndroid = false;
    public int ValorBoton = 1;
    private void Awake()
    {
        if(Instance== null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        QualityGraphics.value = GamePlayerPrefs.instance.loadGraphicsInt();
        LanguageGame.value = GamePlayerPrefs.instance.loadLanguageInt();


        if (SystemInfo.deviceType == DeviceType.Handheld) // Reconocimiento Android
        {
            EstaEnAndroid = true;

        }
        if (EstaEnAndroid == true)
        {
            BotonConfig.SetActive(false);
            //MenuTerms[0].PanelMenu.SetActive(false);
        }
    }
    public void currentMenu(Button btn)
    {
        if (EstaEnAndroid == true)
        {
            ValorBoton = 1;
        }
        else
        {
            ValorBoton = 0;
        }
        for (int i = ValorBoton; i < MenuNavegation.Length; i++)
        {
            if (MenuNavegation[i].BtnMenu.name == btn.name)
            {
                MenuNavegation[i].BtnMenu.interactable = false;
                MenuNavegation[i].PanelMenu.SetActive(true);
            }
            else
            {
                MenuNavegation[i].BtnMenu.interactable = true;
                MenuNavegation[i].PanelMenu.SetActive(false);
            }
        }
    }
    public void DropdownQualityValueChanged()
    {
        QualitySettings.SetQualityLevel(QualityGraphics.value);

        GamePlayerPrefs.instance.saveGraphicsEnum(QualityGraphics.value);
    }
    public void DropdownLanguageValueChanged()
    {
        //LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[LanguageGame.value];
        //GamePlayerPrefs.instance.saveLanguageEnum(LanguageGame.value);
        switch (LanguageGame.value)
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
        GamePlayerPrefs.instance.saveLanguageEnum(LanguageGame.value);
        //Fllag.GetComponent<Image>().sprite = flags[languageDropDown.value];
    }
    public void DeleteAccount()
    {
        StartCoroutine(_DeleteAccount());
    }
    public IEnumerator _DeleteAccount()
    {
        PopUpManager._instance.setPopUp("Borrar Cuenta", "Serás llevado al sitio oficial, para solicitar la eliminación de la cuenta", IconosPopUp.questioin, true, 20);


        yield return new WaitWhile(() => PopUpManager._instance.popUpState == PopUpStates.Wait);

        if (PopUpManager._instance.popUpState == PopUpStates.Ok)
        {
            Application.OpenURL(VersionNv.version.UrlDeleteAccount);
        }
    }

}
