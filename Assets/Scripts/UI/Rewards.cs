using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Rewards : MonoBehaviour
{
    [Header("POPUPS")]
    public GameObject WinnerPopUp;
    public GameObject LoserPopUp;
    [Header("WINNER")]
    public GameObject positionWinner;
    public TextMeshProUGUI telWinWinner;
    public TextMeshProUGUI tnlWinWinner;
    public TextMeshProUGUI tournamentWinner;
    public TextMeshProUGUI playernameWinner;
    public TextMeshProUGUI pointsWinner;
    [Header("LOSER")]
    public TextMeshProUGUI positionLoser;
    public TextMeshProUGUI tournamentLoser;
    public TextMeshProUGUI playernameLoser;
    public TextMeshProUGUI pointsLoser;
    public TextMeshProUGUI position;
    void Start()
    {
        if (PlayfabTournament.hasPrize)
        {
            checkPrizes();
        }
        
    }

    // Update is called once per frame
    void checkPrizes()
    {
        if(PlayfabTournament.premio.TelWin>0|| PlayfabTournament.premio.TnlWin> 0)//Position==1 || PlayfabTournament.premio.Position == 2 || PlayfabTournament.premio.Position == 3)
        {
            WinnerPopUp.SetActive(true);
            setDataWinner();
        }
        else
        {
            LoserPopUp.SetActive(true);
            setDataLoser();
        }
    }
    void setDataWinner()
    {
        switch (PlayfabTournament.premio.Position)
        {
            case 1:
                positionWinner.transform.GetChild(0).gameObject.SetActive(true);
                //position.color= new Color(.1f, .45f, .0f, .5f);
                break;
            case 2:
                positionWinner.transform.GetChild(1).gameObject.SetActive(true);
                //position.color = new Color(.5f, .5f, .0f, .5f);
                break;
            case 3:
                positionWinner.transform.GetChild(2).gameObject.SetActive(true);
                //position.color = new Color(.45f, .25f, .0f, .5f);   
                break;
            default:
                position.text = PlayfabTournament.premio.Position.ToString();
                position.gameObject.SetActive(true);
                break;
        }
        
        telWinWinner.text = PlayfabTournament.premio.TelWin.ToString();
        tnlWinWinner.text = PlayfabTournament.premio.TnlWin.ToString();
        tournamentWinner.text = VersionNv.getNameTorneo(PlayfabTournament.premio.id_Torneo.ToString());// PlayfabTournament.premio.id_Torneo.ToString();
        
        playernameWinner.text = PlayfabTournament.premio.NamePlayer.ToString();
        pointsWinner.text = PlayfabTournament.premio.StatVal.ToString();
    }
    void setDataLoser()
    {
        positionLoser.text = PlayfabTournament.premio.Position.ToString();
        tournamentLoser.text = PlayfabTournament.premio.id_Torneo.ToString();
        playernameLoser.text = PlayfabTournament.premio.NamePlayer.ToString();
        pointsLoser.text = PlayfabTournament.premio.StatVal.ToString();
    }
    public void claimReward()
    {
        PlayfabTournament.reclaimPrize();
        gameObject.SetActive(false);
    }
    public void closeWindows()
    {
        gameObject.SetActive(false);
        PlayfabManager.instance.BtnReward.SetActive(false);
    }
}
