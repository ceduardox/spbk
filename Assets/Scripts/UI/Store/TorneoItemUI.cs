using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TorneoItemUI : MonoBehaviour
{
    public string id;//ID del torneo
    public TextMeshProUGUI title;
    public TextMeshProUGUI date;
    public TextMeshProUGUI details;
    public UnityEngine.UI.RawImage img;
    public UnityEngine.UI.Button btnInscribirse;
    public TextMeshProUGUI btnInscribirseText;
    public UnityEngine.UI.Image fondo;
    public TextMeshProUGUI telValue;
    public TextMeshProUGUI tlnValue;
    public TextMeshProUGUI level;
    public GameObject inscripcion;
    public Torneo torneo;

    private void Start()
    {
        StartCoroutine(timer());
    }

    public void inscribir()
    {
        if (PlayfabManager.instance.getTEL() < int.Parse(telValue.text) || PlayfabManager.instance.getTEL() < int.Parse(telValue.text))
        {
            GameLauncher.instance.sinFondos();
        }
        else
        {
            // PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_OK), Ya estas participando en este torneo ", IconosPopUp.questioin, false);
            if (torneo.Level>PlayfabManager.instance.getLevel())
            {
                 PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_ERROR),
                                                ("Necesitas ser nivel XXX para poder participar").Replace("XXX", torneo.Level.ToString()), IconosPopUp.error, false);
                                                //xxxt-
            }
            else
            { LittlePopUpManager.instance.setSmallPopUp(TranslateUI.getStringUI(UI_CODE.TRN_MSJ_03));
                //PlayfabTournament.registerTournament(int.Parse(telValue.text), int.Parse(tlnValue.text), this);

            }

        } 
    }
    //player
    //id=true


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
            fondo.GetComponent<Animator>().enabled = false;
            fondo.color = Color.green;
            setButton();

        }
        else
        {
            btnInscribirseText.GetComponent<TranslateUI_ITEM>()._code = UI_CODE.TRN_UI_SUBS_0;
            btnInscribirseText.text = TranslateUI.getStringUI(UI_CODE.TRN_UI_SUBS_0);/*"Suscribirse";//xxxt-*/
            btnInscribirse.interactable = true;
            fondo.color = Color.white;
        }
    }

    public void setButton()
    {
        //CLog.Log("ESTOOO: " + PlayfabManager.instance.isTorneo +" - "+ id+" - "+VersionNv.torneoActual.Id);
        if (TorneosScreen.instance&&
            PlayfabManager.instance.isTorneo&&
            id.Equals(VersionNv.torneoActual.Id))
        {
            TorneosScreen.instance.btnInscribirseText.GetComponent<TranslateUI_ITEM>()._code = UI_CODE.TRN_UI_SUBS_1;//.text = TranslateUI.getStringUI(UI_CODE.TRN_UI_SUBS_1);/*"Suscripto";//xxxt-*/
            TorneosScreen.instance.btnInscribirseText.text = TranslateUI.getStringUI(UI_CODE.TRN_UI_SUBS_1);
            TorneosScreen.instance.btnInscribirse.interactable = false;
            TorneosScreen.instance.fondo.GetComponent<Animator>().enabled = false;
            TorneosScreen.instance.fondo.color = Color.green;

        }
    }

    public void setButton(UnityEngine.UI.Button _btn)
    {
        _btn.interactable = false;
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
                    date.text = status + " " + //xxxt-
                                (
                                timeTMP!= System.TimeSpan.Zero
                                ?
                                timeTMP.Days + " "+ TranslateUI.getStringUI(UI_CODE.TRN_UI_DAYS) + ", " +
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


