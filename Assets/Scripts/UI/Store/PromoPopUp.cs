using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromoPopUp : MonoBehaviour
{

    public void ShowPromo()
    {
        CLog.Log("promo popup");
        PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_WARNING), TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_BETCONFIRM), IconosPopUp.questioin, true);

    }
}
