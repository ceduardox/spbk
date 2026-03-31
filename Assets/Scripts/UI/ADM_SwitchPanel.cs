using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ADM_SwitchPanel : MonoBehaviour
{
    //public TypeEvent eventInteraction=TypeEvent.OnClick;
    public GroupButtons_BP[] MenuGroupButtons;
    //public enum TypeEvent
    //{
    //    OnClick, OnHover
    //}
    void Start()
    {
        instantiateEvents();
    }
    void instantiateEvents()
    {
        foreach (var item in MenuGroupButtons)
        {
            foreach (var CurrentItem in item.Buttons)
            {
                //if (eventInteraction == TypeEvent.OnClick)
                //{
                CurrentItem.Button.onClick.AddListener(() => setPanel(item.nameGroup, CurrentItem.Button, CurrentItem.Panel, CurrentItem.HoldActived));
                //    return;
                //}else if (eventInteraction == TypeEvent.OnHover)
                //{
                //    //CurrentItem.Button.OnPointerEnter.AddListener(() => setPanel(item.nameGroup, CurrentItem.Button, CurrentItem.Panel));
                //}

            }
        }
    }
    void setPanel(string nameGroup, Button btn, GameObject go, bool holdActive)
    {
        foreach (var group in MenuGroupButtons)
        {
            if (group.nameGroup == nameGroup)
            {
                DeactivedButtons(group.nameGroup);
            }
        }
        if (go != null)
            if (holdActive && SystemInfo.deviceType != DeviceType.Handheld)
            {
                CLog.Log("corutina del switch activada" + holdActive);
                StartCoroutine(buyTELCoroutine());
                return;
            } 
            go.SetActive(true);
        btn.interactable = true;
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

    }

    public void DeactivedButtons(string groupMenu)
    {
        for (int j = 0; j < MenuGroupButtons.Length; j++)
        {
            if (MenuGroupButtons[j].nameGroup == groupMenu)
            {
                for (int i = 0; i < MenuGroupButtons[j].Buttons.Length; i++)
                {
                    MenuGroupButtons[j].Buttons[i].Button.interactable = true;
                    if(!MenuGroupButtons[j].Buttons[i].HoldActived && MenuGroupButtons[j].Buttons[i].Panel!=null)
                        MenuGroupButtons[j].Buttons[i].Panel.SetActive(false);
                }
            }
        }
    }
}