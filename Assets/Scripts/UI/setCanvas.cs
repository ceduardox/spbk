using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class setCanvas : MonoBehaviour
{
    public bool desktop=true;
    [Header("Holders mobile")]
    public GameObject uiMobile;
    public GameObject uiDesktop;
    [Header("Objects mobile")]
    public GameObject IHM1;
    public Image iconItem1;
    public GameObject IHM2;
    public Text countItem2;
    public Image iconItem2;
    public GameObject IHM3;
    public Text countItem3;
    public Image iconItem3;

    private void Awake()
    {
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            CLog.Log($"device used {SystemInfo.deviceType}");
            uiMobile.SetActive(true);
            desktop = false;
        }
        else
        {
            //desktop = false;
            //uiMobile.SetActive(true);
            uiDesktop.SetActive(true);
        }



    }
}
