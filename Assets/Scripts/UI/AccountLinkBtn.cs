using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AccountLinkBtn : MonoBehaviour
{
    
    public GameObject registerPanel;
    public GameObject linkAccPanel;

    public void OnAccountLinkBtnClick()
    {
        this.gameObject.SetActive(true);
        if (PlayfabManager.instance.isGuest())
        {
            registerPanel.SetActive(true);
            linkAccPanel.SetActive(false);
            CLog.Log("is Guest");
        }
        else
        {
            registerPanel.SetActive(false);
            linkAccPanel.SetActive(true);
            CLog.Log("is user linked account");
        }
    }

    public void SetLinkedAccountPanel()
    {
        registerPanel.SetActive(false);
        linkAccPanel.SetActive(true);
    }
}
