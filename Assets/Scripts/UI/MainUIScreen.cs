using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainUIScreen : MasterScreen
{  
    private DateTime dateCreated;
    private DateTime dateCurrent;
    //[SerializeField] GameObject panelPromo;
    void Awake()
    {
        //UIScreen.Focus(GetComponent<UIScreen>());
    }
    private void Start()
    {
        AudioEnviromentControl.menuEnabled("menu");
        //Busines.getItemLimitedEditionQuantity(Catalogos.Karts);
        ////if (PlayfabManager.instance.isFirstAccount)
        ////    PlayfabManager.instance.screenTutorial.SetActive(true);
        //Invoke("setDatePopup",3f);
    }
    private void OnEnable()
    {
        if (PlayfabManager.instance != null && PlayfabManager.instance.loadingScreen != null)
        {
            LoadingPointsAnim loadingAnim = PlayfabManager.instance.loadingScreen.GetComponent<LoadingPointsAnim>();
            if (loadingAnim != null)
            {
                loadingAnim.hide();
            }
        }
    }
    //void setDatePopup()
    //{
    //    if (PlayfabManager.instance.dateCreatedAccount == "")
    //        return;
    //    dateCreated = DateTime.Parse(PlayfabManager.instance.dateCreatedAccount);
    //    dateCurrent = DateTime.Now;
    //    TimeSpan rest = dateCurrent - dateCreated;
    //    int resD = (int)rest.TotalHours;
    //    //int resH = rest.Hours;
    //    //int resM = rest.Minutes;
    //    if (resD > 73)
    //    {
    //        return;
    //    }
    //    //panelPromo.SetActive(true);
    //}
}
