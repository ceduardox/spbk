using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public enum PopUpStates
{
    Ok,
    Wait,
    Cancel,
    None
}
public enum IconosPopUp
{
    warning,
    questioin,
    block,
    error,
    soporte
}

public class PopUpManager : MonoBehaviour
{
    //public UnityEngine.UI.Text tittle;
    public TextMeshProUGUI tittle;
    //public UnityEngine.UI.Text desc;
    public TextMeshProUGUI desc;
    public UnityEngine.UI.Image icon;
    public UnityEngine.UI.Button btn_OK;
    public UnityEngine.UI.Button btn_Cancel;
    public GameObject apuestaModule;
    public TMP_InputField betValue;
    public PopUpStates popUpState;
    public static PopUpManager _instance;
    private int apuesta;
    public Sprite[] iconos;
    public GameObject panelPopUp;

    public GameObject ButtonSoport;
    //private void Awake()
    //{
    //    panelPopUp.GetComponent<RectTransform>().DOScale(new Vector3(0f, 0f, 0f), 0f);
    //}

    public void starPopUp()
    {
        if (_instance)
        {
            Destroy(this);
            return;
        }
            _instance = this;
    }
  
    public void setPopUp(int _idmsg, string _tittle, string _desc, IconosPopUp _icon, bool _confirm)
    {
        List<string> traslate = TranslateScript.instance.replacePopupLanguage(_idmsg);
        setPopUp(traslate[0], traslate[1], _icon, _confirm, false);
    }
    public void setPopUp(string _tittle, string _desc, IconosPopUp _icon, bool _confirm)
    {
        //panelPopUp.GetComponent<Transform>().DOScale(new Vector3(1f, 1f, 1f), 0.25f);
        //setPopUp(_tittle, _desc, _icon, _confirm, false);
        setPopUp(_tittle, _desc, _icon, _confirm, 0);
    }

    int time;
    string descTMP;
    public void setPopUp(string _tittle, string _desc, IconosPopUp _icon, bool _confirm, int _time)
    {
        panelPopUp.GetComponent<Transform>().DOScale(new Vector3(1f, 1f, 1f), 0.25f);

        //Si es soporte, aparecer Botón
            ButtonSoport.gameObject.SetActive(_icon == IconosPopUp.soporte);

        setPopUp(_tittle, _desc + " (" + _time + ")", _icon, _confirm, false);
        if (_time > 0)
        {
            descTMP = _desc;
            time = _time;
            countDown();
        }
    }

    public void OpenSuportPage()
    {
        StartCoroutine(_GoToSoport());
    }
    public IEnumerator _GoToSoport()
    {
        PopUpManager._instance.setPopUp("Ir a página de Soporte", "Serás llevado al sitio oficial, para solicitar el soporte de la cuenta", IconosPopUp.block, true, 20);


        yield return new WaitWhile(() => PopUpManager._instance.popUpState == PopUpStates.Wait);

        if (PopUpManager._instance.popUpState == PopUpStates.Ok)
        {
            if (VersionNv.version!=null)
            {
                Application.OpenURL(VersionNv.version.UrlSoporteAccount);

            }
            else
            {
                Application.OpenURL("https://superblok.co/pages/soporte.html");

            }
        }
    }

    void countDown()
    {
        if(gameObject.activeSelf)
        {
            if(--time==0)
            {
                pressButon(0);
            }
            else
            {
                desc.text = descTMP  +" (" + time + ")";

                Invoke("countDown",1);

            }

        }
    }




    public void setPopUp(string _tittle, string _desc, IconosPopUp _icon, bool _confirm, bool _bet)
    {
        if (_confirm)
            popUpState = PopUpStates.Wait;
        else
            popUpState = PopUpStates.None;

        apuestaModule.SetActive(_bet);
        if (_bet)
            betValue.text = GameLauncher.instance.minBetTEL.ToString();
        
        

        btn_Cancel.gameObject.SetActive(_confirm);
        btn_OK.interactable = true;
        CLog.Log("ESTO VALE: " + _tittle);
        CLog.Log("ESTO VALE: " + tittle.text);
        tittle.text = _tittle;
        desc.text = _desc
            ;
        if (icon) icon.overrideSprite = iconos[(int)_icon];// Sprite.Create((Texture2D)_icon.texture, new Rect(0.0f, 0.0f, _icon.texture.width, _icon.texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        gameObject.SetActive(true);
    }
    public void hidePopUp()
    {
        gameObject.SetActive(false);

    }
    public void pressButon(int _value)
    {
        panelPopUp.GetComponent<Transform>().DOScale(new Vector3(0f, 0f, 0f), 0.25f).OnComplete(()=> getBtnValue(_value));
    }
    void getBtnValue(int _value)
    {
        if (_value == 0)
        {
            popUpState = PopUpStates.Ok;
        }
        else if (_value == 1)
        {
            popUpState = PopUpStates.Cancel;
        }
        else
        {
            popUpState = PopUpStates.None;
        }
        StartCoroutine(ApagarObjeto());
    }
    public IEnumerator ApagarObjeto()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);

    }
    public void setBet(TMP_InputField _if)
    {
        if(!string.IsNullOrEmpty(_if.text))
            apuesta = int.Parse(_if.text);
        btn_OK.interactable = apuesta > GameLauncher.instance.minBetTEL;
        /*if (apuesta < 100)
            apuesta = 100;*/
    }

    public int getApuesta()
    {
        return apuesta;
    }






}
