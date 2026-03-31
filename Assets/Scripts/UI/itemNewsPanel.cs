using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class itemNewsPanel : MonoBehaviour, IDeselectHandler, ISelectHandler
{
    public TextMeshProUGUI day;
    public TextMeshProUGUI month;
    public TextMeshProUGUI tittle;
    public GameObject obTittle;
    public GameObject obDate;
    public Button btnItem;
    public string _idNews;
    void Start()
    {
        
    }

    // Update is called once per frame

    public void verificarInscripcion()
    {
        //PlayfabTournament.chekStatus(this);
    }
    public void verificarInscripcion(bool _state)
    {
        if (_state)
        {
            //btnInscribirseText.text = TranslateUI.getStringUI(UI_CODE.TRN_UI_SUBS_1);/*"Suscripto";//xxxt-*/
            //btnInscribirse.interactable = false;
        }
        else
        {
            //btnInscribirseText.text = TranslateUI.getStringUI(UI_CODE.TRN_UI_SUBS_0);/*"Suscribirse";//xxxt-*/
            //btnInscribirse.interactable = true;
        }
    }
    public void setData()
    {
        //newsPanel.instance.setBodyPanel(_idNews);
    }
    public void OnDeselect(BaseEventData data)
    {
        //obTittle.SetActive(true);
        //obDate.SetActive(false);
        //btnItem.interactable=true;
    }
    public void OnSelect(BaseEventData data)
    {
        //obTittle.SetActive(false);
        //obDate.SetActive(true);
        //btnItem.interactable = false;
    }
   
}
