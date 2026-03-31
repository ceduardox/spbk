using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CongratsPanel : MonoBehaviour
{
    public TextMeshProUGUI TextLvl;
    string clvl;
    public Transform wingL;
    public Transform wingR;
    public Transform shield;
    public Transform bright;
    public Transform glow;
    public Transform flag;
    public Transform btnAceptar;
    public Ease vfx;
    Material mat;
    public float Crecimiento = 0;
    private void Awake()
    {
        mat = GetComponent<Image>().material;
        mat.SetFloat("_ShineLocation", 0);
        setStartPosition();
        //TextLvl.text = PlayfabManager.instance.getLevel().ToString();
    }
    private void OnEnable()
    {
        TextLvl.text = DisplayDataUI.instance.currentLvlOnDisable.ToString();
        glowEffect();
        scaleMedal();
        setFlag();
        InvokeRepeating("Corrutina", 3f, 0.02f);
        time = 20;
        countDown();
    }

    int time;
    string descTMP;
    void countDown()
    {
        if (gameObject.activeSelf)
        {
            if (--time == 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                btnAceptar.GetChild(0).GetComponent<TextMeshProUGUI>().text = TranslateUI.getStringUI(UI_CODE._ACCEPT) + " (" + time + ")";
                Invoke("countDown", 1);

            }

        }
    }

    private void OnDisable()
    {
        setStartPosition();
    }
    void glowEffect()
    {
        glow.DORotate(new Vector3(0f, 0f, 360f), 3f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        bright.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 1f).OnComplete(() => bright.DOScale(new Vector3(1f, 1f, 1f), 1f)).SetLoops(-1, LoopType.Yoyo);
    }
    void scaleMedal()
    {
        shield.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetEase(Ease.Linear).OnComplete(()=> startEffects());
    }
    void startEffects()
    {
        wingL.GetComponent<RectTransform>().DOAnchorPos(new Vector3(-200f, 80f, 0f), 0.8f).SetEase(Ease.InSine);
        wingR.GetComponent<RectTransform>().DOAnchorPos(new Vector3(200f, 80f, 0f), 0.8f).SetEase(Ease.InSine);
        wingL.DOScale(new Vector3(1f, 1f, 1f), 0.3f).SetDelay(0.5f);
        wingR.DOScale(new Vector3(1f, 1f, 1f), 0.3f).SetDelay(0.5f);
        wingL.DORotate(new Vector3(0f, 0f, -20f), 1.2f, RotateMode.Fast).SetEase(Ease.OutElastic).SetDelay(0.5f); ;
        wingR.DORotate(new Vector3(0f, 0f, 20f), 1.2f, RotateMode.Fast).SetEase(Ease.OutElastic).SetDelay(0.5f);
        btnAceptar.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetDelay(3f);
        btnAceptar.GetChild(0).gameObject.GetComponent<Transform>().DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.6f).OnComplete(() => btnAceptar.GetChild(0).gameObject.GetComponent<Transform>().DOScale(new Vector3(1f, 1f, 1f), 1f)).SetLoops(-1, LoopType.Yoyo);
    }
    void setFlag()
    {
        flag.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0f, -254f, 0f), 3f).SetEase(vfx);
    }
    void setStartPosition()
    {
        wingL.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0f, 0f, 0f), 0f);
        wingR.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0f, 0f, 0f), 0f);
        wingL.DOScale(new Vector3(0.5f, 0.5f, 1f), 0f);
        wingR.DOScale(new Vector3(0.5f, 0.5f, 1f), 0f);
        wingL.DORotate(new Vector3(0f, 0f, 0f), 0f, RotateMode.Fast);
        wingR.DORotate(new Vector3(0f, 0f, 0f), 0f, RotateMode.Fast);
        shield.DOScale(new Vector3(0f, 0f, 0f), 0f);
        flag.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0f, -700f, 0f), 0f);
        btnAceptar.DOScale(new Vector3(0f, 0f, 0f), 0f);
    }
    void Corrutina()
    {
        Crecimiento = Crecimiento + 0.02f;
        mat.SetFloat("_ShineLocation", Crecimiento);

        if (Crecimiento >= 1)
        {
            Crecimiento = 0;
            CancelInvoke("Corrutina");
            InvokeRepeating("Corrutina", 3f, 0.02f);
        }
    }
}
