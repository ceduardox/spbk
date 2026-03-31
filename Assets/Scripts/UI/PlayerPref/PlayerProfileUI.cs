using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerProfileUI : MonoBehaviour
{
    public static PlayerProfileUI instance;
    public TextMeshProUGUI DisplayName;
    public TextMeshProUGUI DisplayLastLogin;
    public TextMeshProUGUI DisplayLvl;
    public TextMeshProUGUI DisplayClan;
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
    public void showProfile(string displayname, string lastlogin, int lvl)
    {
        DisplayName.text = displayname;
        DisplayLastLogin.text = lastlogin;
        DisplayLvl.text = "Nivel "+lvl.ToString();
        //DisplayClan.text = clan;
    }
    public void showProfileClan(string clan)
    {
        DisplayClan.text = clan;
    }
    private void OnDisable()
    {
        CLog.Log("desactivo ventana");
        DisplayName.text = null;
        DisplayLastLogin.text = null;
        DisplayLvl.text = null;
        DisplayClan.text = null;
    }
}
