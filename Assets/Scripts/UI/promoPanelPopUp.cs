using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class promoPanelPopUp : MonoBehaviour
{
    [SerializeField] public Transform glowEffect;
    [SerializeField] public Transform btnBuy;
    [SerializeField] public Transform rewardTnl;
    [SerializeField] public Transform rewardTel;
    [SerializeField] public Transform noAds;
    public DateTime dateCreated;
    public DateTime dateCurrent;
    public TMPro.TextMeshProUGUI txtTimer;
    int totalDuration;
    DateTime fechaLimite;

    void Start()
    {
        setDatePopup();
        
    }
    private void OnEnable()
    {
        setEffects();
        Invoke("setEffectRewards", 1f);
    }
    void setEffects()
    {
        glowEffect.DORotate(new Vector3(0f, 0f, 360f), 3f,RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1,LoopType.Restart);
        btnBuy.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 1f).OnComplete(() => btnBuy.DOScale(new Vector3(1f, 1f, 1f), 1f)).SetLoops(-1, LoopType.Yoyo);
        rewardTnl.DOScale(new Vector3(0f, 0f, 0f), 0f);
        rewardTel.DOScale(new Vector3(0f, 0f, 0f), 0f);
        noAds.DOScale(new Vector3(0f, 0f, 0f), 0f);
    }
    void setEffectRewards()
    {
        rewardTnl.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetEase(Ease.InElastic).OnComplete(() =>
        rewardTel.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetEase(Ease.InElastic).OnComplete(() =>
        noAds.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetEase(Ease.InElastic)));
    }

    void setDatePopup()
    {
        if (PlayfabManager.instance.dateCreatedAccount == "")
            return;
        dateCreated = DateTime.Parse(PlayfabManager.instance.dateCreatedAccount);
        dateCurrent = DateTime.Now;
        fechaLimite = dateCreated.AddDays(3f);

        //TimeSpan rest = fechaLimite - dateCurrent;
        ////TimeSpan rest = dateCurrent - dateCreated;
        //int resD = (int)rest.TotalDays;
        //int resH = rest.Hours;
        //int resM = rest.Minutes;
        //int resS = rest.Seconds;
        //if (resD > 0) totalDuration = resD * resH * resM * resS;
        //else if
        //    (resH > 0) totalDuration = resH * resM * resS;
        //else if
        //    (resM > 0) totalDuration = resM * resS;
        //else if
        //    (resS > 0) totalDuration = resS;
        //else
        //{
        //    totalDuration = 0;
        //}

        //InvokeRepeating("contador", 0.0f, 1.0f);
        InvokeRepeating("contador2", 0.0f, 1.0f);

    }
    void contador1()
    {
        int days = totalDuration / 86400;
        int hours = (totalDuration % 86400) / 3600;
        int minutes = (totalDuration % 3600) / 60;
        int seconds = totalDuration % 60;
        CLog.Log($"Faltan {days} días, {hours} horas, {minutes} minutos y {seconds} segundos para finalizar el contador regresivo");
        txtTimer.text = (days > 0 ? days.ToString() + " D " : "") + (hours > 0 ? hours.ToString() + " H " : "") + (minutes > 0 ? minutes.ToString() + " M " : "") + (seconds.ToString() + " S");
        totalDuration--;
        if (totalDuration < 0)
        {

            CLog.Log("SEACABO!!!");
        }
    }
    void contador2()
    {
        TimeSpan timeremain = fechaLimite - DateTime.Now;
        int days = timeremain.Days;
        int hours = timeremain.Hours;
        int minutes = timeremain.Minutes;
        int seconds = timeremain.Seconds;
        txtTimer.text = (days > 0 ? days.ToString() + " D " : "") + (hours > 0 ? hours.ToString() + " H " : "") + (minutes > 0 ? minutes.ToString() + " M " : "") + (seconds.ToString() + " S");
        if (timeremain.TotalHours > 72) gameObject.SetActive(false);
    }
    
}
