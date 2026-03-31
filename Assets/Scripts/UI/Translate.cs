using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEngine.Localization;
//using UnityEngine.Localization.Settings;

public class Translate : MonoBehaviour
{
    // Start is called before the first frame update

    //Dictionary<keys, List<string>> text;

    //enum keys
    //{
    //    //Aceptar,Sel alir

    //};
    public TMP_Dropdown m_Dropdown;
    /*public void SetSelectedLocale(Locale idioma)
    {
        LocalizationSettings.SelectedLocale = idioma;
    }*/
    void Start()
    {
        m_Dropdown = GetComponent<TMP_Dropdown>();
    }
    public void DropdownLanguageValueChanged(TMP_Dropdown change)
    {
        int abc = change.value;
        if (change.value == 0)
        {
            //SetSelectedLocale(LocalizationSettings.AvailableLocales.Locales[1]);
        }
        else if(change.value == 1)
        {
            //SetSelectedLocale(LocalizationSettings.AvailableLocales.Locales[0]);
        }
    }

}
