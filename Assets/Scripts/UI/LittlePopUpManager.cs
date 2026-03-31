using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
//public enum Iconos
//{
//    done,error
//}
public class LittlePopUpManager : MonoBehaviour
{
    public static LittlePopUpManager instance;

    public TextMeshProUGUI messagePopUp;
    public GameObject ButtonClose;
    public GameObject ButtonAccept;
    //public Sprite[] iconos;
    public GameObject panelPopUp;
    public enum icons { imgFailed, imgSucces };


    public void starPopUp()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    private void OnEnable()
    {
        panelPopUp.GetComponent<Transform>().DOScale(Vector3.zero, 0f);
        
    }

    public void SetSmallPopupWithoutTime(string _message)
    {
        messagePopUp.text = _message;
        gameObject.SetActive(true);
        ButtonAccept.SetActive(false);
        ButtonClose.SetActive(false);
        panelPopUp.GetComponent<Transform>().DOScale(new Vector3(1f, 1f, 1f), 0.025f);
    }
    public void DesactivePopupWithoutTime()
    {
        gameObject.SetActive(false);
    }
    public void setSmallPopUp(string _message/*, IconosPopUp _icon*/)
    {
        gameObject.SetActive(true);
        ButtonAccept.SetActive(false);
        ButtonClose.SetActive(false);
        panelPopUp.GetComponent<Transform>().DOScale(new Vector3(1f, 1f, 1f), 0.25f).OnComplete(() => expandPopUp(_message));
    }
    void expandPopUp(string _message)
    {
        StopAllCoroutines();
        StartCoroutine(showMessage(_message/*, _icon*/));
    }
    public void setSmallPopUpConfirm(string _message)
    {
        gameObject.SetActive(true);
        ButtonClose.SetActive(true);
        ButtonAccept.SetActive(true);
        messagePopUp.text = _message;
        panelPopUp.GetComponent<Transform>().DOScale(new Vector3(1f, 1f, 1f), 0.25f);
        ButtonAccept.GetComponent<Button>().onClick.AddListener(() => {
            ClanSystem.LeaveGroup();
            gameObject.SetActive(false);
            //PopUpCreateClan.instance.clanScreen.SetActive(false);
            //PopUpCreateClan.instance.noClanScreen.SetActive(true);
        });
    }
    IEnumerator showMessage(string message/*, IconosPopUp icon*/)
    {
        messagePopUp.text = message;
        yield return new WaitForSeconds(2f);
        panelPopUp.GetComponent<Transform>().DOScale(new Vector3(0f, 0f, 0f), 0.25f).OnComplete(() =>
        gameObject.SetActive(false));

    }



}
