using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardPopUp : MonoBehaviour
{
    public static RewardPopUp instance;
    public string id;
    public TextMeshProUGUI TittleEvent;
    public TextMeshProUGUI DateEvent;
    public TextMeshProUGUI DetailEvent;
    public TextMeshProUGUI ReqTel;
    public TextMeshProUGUI ReqTnl;
    public TextMeshProUGUI ReqLvl;
    public TextMeshProUGUI Reward1TEL;
    public TextMeshProUGUI Reward1TNL;
    public TextMeshProUGUI Reward2TEL;
    public TextMeshProUGUI Reward2TNL;
    public TextMeshProUGUI Reward3TEL;
    public TextMeshProUGUI Reward4TNL;
    public Torneo t;
    public Button btnInscribirse;
    public TextMeshProUGUI btnInscribirseText;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public void setButton()
    {
        //CLog.Log("ESTOOO: " + PlayfabManager.instance.isTorneo +" - "+ id+" - "+VersionNv.torneoActual.Id);
        if (TorneosScreen.instance &&
            PlayfabManager.instance.isTorneo &&
            id.Equals(VersionNv.torneoActual.Id))
        {
            TorneosScreen.instance.btnInscribirseText.GetComponent<TranslateUI_ITEM>()._code = UI_CODE.TRN_UI_SUBS_1;//.text = TranslateUI.getStringUI(UI_CODE.TRN_UI_SUBS_1);/*"Suscripto";//xxxt-*/
            TorneosScreen.instance.btnInscribirseText.text = TranslateUI.getStringUI(UI_CODE.TRN_UI_SUBS_1);
            TorneosScreen.instance.btnInscribirse.interactable = false;
            //TorneosScreen.instance.fondo.GetComponent<Animator>().enabled = false;
            //TorneosScreen.instance.fondo.color = Color.green;

        }
        else if (TorneosScreen.instance) TorneosScreen.instance.btnInscribirse.interactable = true && t.State != TorneoSTATE.Finalizado;
    }
    public void inscribir()
    {
        if (PlayfabManager.instance.getTEL() < int.Parse(ReqTel.text) || PlayfabManager.instance.getTEL() < int.Parse(ReqTel.text))
        {
            //GameLauncher.instance.sinFondos();
            LittlePopUpManager.instance.setSmallPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_TELOUT2));

        }
        else
        {
            // PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_OK), Ya estas participando en este torneo ", IconosPopUp.questioin, false);
            if (t.Level > PlayfabManager.instance.getLevel())
            {

                LittlePopUpManager.instance.setSmallPopUp(("Necesitas ser nivel XXX para poder participar").Replace("XXX", t.Level.ToString()));

                //PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_ERROR),
                //                              ("Necesitas ser nivel XXX para poder participar").Replace("XXX", torneo.Level.ToString()), IconosPopUp.error, false);
                //xxxt-
            }
            else
            {
                LittlePopUpManager.instance.setSmallPopUp(TranslateUI.getStringUI(UI_CODE.TRN_MSJ_03));
                PlayfabTournament.registerTournament(int.Parse(ReqTel.text), int.Parse(ReqTnl.text), id);

            }

        }
    }
    public void verificarInscripcion()
    {
        //PlayfabTournament.chekStatus(this);
    }
    public void verificarInscripcion(bool _state)
    {
        if (_state)
        {
            btnInscribirseText.GetComponent<TranslateUI_ITEM>()._code = UI_CODE.TRN_UI_SUBS_1;//.text = TranslateUI.getStringUI(UI_CODE.TRN_UI_SUBS_1);/*"Suscripto";//xxxt-*/
            btnInscribirseText.text = TranslateUI.getStringUI(UI_CODE.TRN_UI_SUBS_1);
            btnInscribirse.interactable = false;
            setButton();

        }
        else
        {
            btnInscribirseText.GetComponent<TranslateUI_ITEM>()._code = t.State != TorneoSTATE.Finalizado ? UI_CODE.TRN_UI_SUBS_0 : UI_CODE.TRN_UI_SUBS_0;//AGREGAR CODIGO DE "FINALIZADO" //xxxt-
            btnInscribirseText.text = t.State != TorneoSTATE.Finalizado ? TranslateUI.getStringUI(UI_CODE.TRN_UI_SUBS_0) : "FINALIZADO";/*//AGREGAR CODIGO DE "FINALIZADO"//xxxt-*/
            btnInscribirse.interactable = true && t.State != TorneoSTATE.Finalizado;
            setButton();

        }
    }
    public void Start()
    {
        //StartCoroutine(timer());
    }
    public void OnEnable()
    {
        StartCoroutine(timer());
        //PlayfabTournament.chekStatus(this);
        //verificarInscripcion();
        setDataRewards();
    }
    void setDataRewards()
    {
        
    }
    public void activarBoton()
    {

        TorneosScreen.instance.btnInscribirseText.text = TranslateUI.getStringUI(UI_CODE.TRN_UI_SUBS_0);
        btnInscribirse.interactable = true;
    }

    IEnumerator timer()
    {
        yield return new WaitForSeconds(1);
        //System.TimeSpan timeTMP;
        while (true)
        {
            yield return new WaitForSeconds(1);
            //if (VersionNv.TorneosActuales[id].Enabled)
            {
                //if (VersionNv.TorneosActuales[id].State == TorneoSTATE.Activo)
                {
                    (string status, System.TimeSpan timeTMP) = VersionNv.remanenteTorneo(id);
                    DateEvent.text = status + " " + //xxxt-
                                (
                                timeTMP != System.TimeSpan.Zero
                                ?
                                timeTMP.Days + " " + TranslateUI.getStringUI(UI_CODE.TRN_UI_DAYS) + ", " +
                                timeTMP.Hours + " " + TranslateUI.getStringUI(UI_CODE.TRN_UI_HRS) + ", " +
                                timeTMP.Minutes + " " + TranslateUI.getStringUI(UI_CODE.TRN_UI_MIN) + ", " +
                                timeTMP.Seconds + " " + TranslateUI.getStringUI(UI_CODE.TRN_UI_SEG)
                                :
                                ""
                                );
                }//12 Dias, 
            }


            //CLog.Log();
        }
    }
}
