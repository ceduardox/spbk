using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TorneoItemUIS : MonoBehaviour
{
    public string id;//ID del torneo
    public TextMeshProUGUI title;
    public TextMeshProUGUI date;
    public TextMeshProUGUI details;
    //public UnityEngine.UI.RawImage img;
    public GameObject imgPost;
    public GameObject DetailPost;
    public UnityEngine.UI.Button btnInscribirse;
    public TextMeshProUGUI btnInscribirseText;
    public UnityEngine.UI.Image fondo;
    public TextMeshProUGUI telValue;
    public TextMeshProUGUI tlnValue;
    public TextMeshProUGUI level;
    public string Reward1;
    public string Reward2;
    public string Reward3;
    public string PremioTEL;
    public string PremioTNL;
    //public GameObject inscripcion;
    //public RewardPopUp popUpRewards;
    public Torneo torneo;
    public RewardPopUp rewardPopUp;

    private void Start()
    {
        //StartCoroutine(timer());
    }

   

    public void showRewards()
    {
        RewardPopUp r = RewardPopUp.instance;
        r.id = id;
        r.TittleEvent.text = title.text;
        r.DateEvent.text = date.text;
        r.DetailEvent.text = details.text;
        r.ReqTel.text = telValue.text;
        r.ReqTnl.text = tlnValue.text;
        r.ReqLvl.text = level.text;
        r.t = torneo;
        calcularPremios(r);
        PlayfabTournament.chekStatus(id);
    }
    void calcularPremios(RewardPopUp r)
    {
        float currentTel = float.Parse(PremioTEL);
        float currentTnl = float.Parse(PremioTNL);
        r.Reward1TEL.text = currentTel.ToString();
        r.Reward1TNL.text = currentTnl.ToString();
        currentTel = currentTel * 0.5f;
        currentTnl = currentTnl * 0.9f;
        r.Reward2TEL.text = currentTel.ToString();
        r.Reward2TNL.text = currentTnl.ToString();
        currentTel = currentTel * 0.5f;
        currentTnl = currentTnl * 0.9f;
        r.Reward3TEL.text = currentTel.ToString();
        r.Reward4TNL.text = currentTnl.ToString();
    }
}
//r.Reward1TEL.text = Reward1;
//r.Reward1TNL.text = Reward2;
//r.Reward2TEL.text = Reward3;
//r.Reward2TNL.text = Reward1;
//r.Reward3TEL.text = Reward2;
//r.Reward4TNL.text = Reward3;


