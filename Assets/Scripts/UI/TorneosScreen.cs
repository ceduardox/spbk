using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class TorneosScreen : MasterScreen
{
    public static TorneosScreen instance;
    [Header("CURRENT TOURNAMENT")]
    public Transform ListContainer;
    private string imageName;
    public string id;
    public TextMeshProUGUI daysLeft;
    public TextMeshProUGUI hoursLeft;
    public TextMeshProUGUI minutesLeft;
    public TextMeshProUGUI secondsLeft;
    public Image postImage;
    public UnityEngine.UI.Button btnInscribirse;
    public TextMeshProUGUI btnInscribirseText;
    public UnityEngine.UI.Image fondo;
    public Torneo t;
    public TextMeshProUGUI rewardTel;
    public TextMeshProUGUI rewardTnl;
    [Space]
    [Header("RECORD TOURNAMENT")]
    public Transform RecordContainer;
    public TextMeshProUGUI titleEvent;
    public GameObject parentPosition;
    public Button btnSwitchPanel;
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
    void Start()
    {
        LoadRecordTournament();
        //PlayfabTournament.chekStatus(null);
        //VersionNv.torneoActual.torneo_UI.setButton();
        
    }
    private void OnEnable()
    {
        if (VersionNv.torneoActual != null)
        {
            //VersionNv.torneoActual.torneo_UI.setButton();
            PlayfabTournament.LeaderboardTournament(VersionNv.torneoActual.Id);
            StartCoroutine(updateTime());

            InterfaceManager.Instance.setImageUrl(VersionNv.torneoActual.Img, postImage);
            //StartCoroutine(setImageAvatar(VersionNv.torneoActual.Img, postImage));

            //RewardPopUp r = gameObject.GetComponent<RewardPopUp>();
            //r.id = VersionNv.torneoActual.Id;
            //r.t = VersionNv.torneoActual;
            PlayfabTournament.chekStatus(VersionNv.torneoActual.Id);
        }
        else
        {
            ListContainer.GetChild(0).gameObject.SetActive(false);
            gameObject.transform.GetChild(4).transform.GetChild(1).gameObject.SetActive(false);
            gameObject.transform.GetChild(4).transform.GetChild(2).gameObject.SetActive(false);
            gameObject.transform.GetChild(4).transform.GetChild(6).gameObject.SetActive(true);
        }
    }
    public void verificarInscripcion(bool _state)
    {
        if (_state)
        {
            btnInscribirseText.GetComponent<TranslateUI_ITEM>()._code = UI_CODE.TRN_UI_SUBS_1;//.text = TranslateUI.getStringUI(UI_CODE.TRN_UI_SUBS_1);/*"Suscripto";//xxxt-*/
            btnInscribirseText.text = TranslateUI.getStringUI(UI_CODE.TRN_UI_SUBS_1);
            btnInscribirse.interactable = false;
            //setButton();

        }
        else
        {
            if (VersionNv.torneoActual!=null)
            {
                btnInscribirseText.GetComponent<TranslateUI_ITEM>()._code = VersionNv.torneoActual.State != TorneoSTATE.Finalizado ? UI_CODE.TRN_UI_SUBS_0 : UI_CODE.TRN_UI_SUBS_0;//AGREGAR CODIGO DE "FINALIZADO" //xxxt-
                btnInscribirseText.text = VersionNv.torneoActual.State != TorneoSTATE.Finalizado ? TranslateUI.getStringUI(UI_CODE.TRN_UI_SUBS_0) : "FINALIZADO";/*//AGREGAR CODIGO DE "FINALIZADO"//xxxt-*/
                btnInscribirse.interactable = true && VersionNv.torneoActual.State != TorneoSTATE.Finalizado;
            }
            //setButton();
        }
    }
    public void RegistrarPlayer()
    {
        //if (VersionNv.torneoActual != null) VersionNv.torneoActual.torneo_UI.inscribir();
        if (VersionNv.torneoActual != null)
        {
            if (PlayfabManager.instance.getTEL() < int.Parse(VersionNv.torneoActual.PriceTEL.ToString()) || PlayfabManager.instance.getTLN() < int.Parse(VersionNv.torneoActual.PriceTLN.ToString()))
            {
                LittlePopUpManager.instance.setSmallPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_TELOUT2));
            }
            else
            {
                if (VersionNv.torneoActual.Level > PlayfabManager.instance.getLevel())
                {
                    LittlePopUpManager.instance.setSmallPopUp(("Necesitas ser nivel XXX para poder participar").Replace("XXX", VersionNv.torneoActual.Level.ToString()));
                }
                else
                {
                    LittlePopUpManager.instance.setSmallPopUp(TranslateUI.getStringUI(UI_CODE.TRN_MSJ_03));
                    PlayfabTournament.registerTournament(int.Parse(VersionNv.torneoActual.PriceTEL.ToString()), int.Parse(VersionNv.torneoActual.PriceTLN.ToString()), id);

                }

            }
        }
        waitRegister();
    }
    public void ShowPlayers()
    {
        for (int i = 1; i < ListContainer.childCount; i++)
        {
            Destroy(ListContainer.GetChild(i).gameObject);
        }
        TorneoPlayerList itemPlayer = ListContainer.GetChild(0).GetComponent<TorneoPlayerList>();
        bool firsItem = true;
        itemPlayer.gameObject.SetActive(PlayfabTournament.participantes.Count > 0);


        foreach (PlayerTorneo player in PlayfabTournament.participantes)
        {
            if (!firsItem)
                itemPlayer = Instantiate(ListContainer.GetChild(0).gameObject, ListContainer).GetComponent<TorneoPlayerList>();
            itemPlayer.posicion.text = player.pos.ToString();
            itemPlayer.crow.gameObject.SetActive(true);
            //StartCoroutine(setImageAvatar(player.avatarurl, itemPlayer.avatar));
            InterfaceManager.Instance.setImageUrl(player.avatarurl, itemPlayer.avatar);
            itemPlayer.namePlayer.text = player.displayname.ToString();
            itemPlayer.transform.GetChild(4).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Flags/" + player.location);
            itemPlayer.puntos.text = player.statvalue.ToString();
            if (player.statvalue > VersionNv.torneoActual.Premios[0])//Mayor al minimo de puntos
            {
                if (player.pos == 1)
                {
                    itemPlayer.GetComponent<UnityEngine.UI.Image>().color = new Color(.1f, .45f, .0f, .5f);
                    itemPlayer.crow.color = new Color(1, .8f, 0, 1);

                }
                else if (player.pos == 2) //if (player.statvalue > VersionNv.torneoActual.Premios[1])
                {
                    itemPlayer.GetComponent<UnityEngine.UI.Image>().color = new Color(.5f, .5f, .0f, .5f);
                    itemPlayer.crow.color = Color.white;
                }
                else if (player.pos == 3)//if (player.statvalue > VersionNv.torneoActual.Premios[2])
                {
                    itemPlayer.GetComponent<UnityEngine.UI.Image>().color = new Color(.45f, .25f, .0f, .5f);
                    itemPlayer.crow.color = new Color(.75f, .5f, .0f, .5f);
                }
            }
            else
            {
                itemPlayer.GetComponent<UnityEngine.UI.Image>().color = new Color(.27f, .27f, .27f, .5f);
                itemPlayer.crow.gameObject.SetActive(false);
            }

            firsItem = false;
        }
        rewardTel.text = VersionNv.torneoActual.PremioTEL.ToString();
        rewardTnl.text = VersionNv.torneoActual.PremioTNL.ToString();
        id = VersionNv.torneoActual.Id;


    }

    IEnumerator callPlayers()
    {
        yield return new WaitForSeconds(2f);
        if (VersionNv.torneoActual != null) PlayfabTournament.LeaderboardTournament(VersionNv.torneoActual.Id);
    }
    public void clearContainer()
    {
        for (int i = 1; i < ListContainer.childCount; i++)
        {
            Destroy(ListContainer.GetChild(i).gameObject);
        }
    }
    public void waitRegister()
    {
        StartCoroutine(callPlayers());
    }
    //IEnumerator setImageAvatar(string url, RawImage img)
    //{
    //    WWW www = new WWW(url);
    //    yield return www;
    //    if (!File.Exists(Application.persistentDataPath + imageName))
    //    {
    //        if (www.error == null)
    //        {
    //            Texture2D texture = www.texture;
    //            img.GetComponent<RawImage>().texture = texture;
    //            byte[] dataByte = texture.EncodeToPNG();
    //            File.WriteAllBytes(Application.persistentDataPath + imageName + "png", dataByte);

    //        }
    //    }
    //    else
    //    if (File.Exists(Application.persistentDataPath + imageName))
    //    {
    //        byte[] uploadByte = File.ReadAllBytes(Application.persistentDataPath + imageName);
    //        Texture2D texture = new Texture2D(10, 10);
    //        texture.LoadImage(uploadByte);
    //        img.GetComponent<RawImage>().texture = texture;
    //    }
    //}

    IEnumerator updateTime()
    {

        while (true)
        {
            yield return new WaitForSeconds(1);
            (string info, System.TimeSpan time) = VersionNv.remanenteTorneo(VersionNv.torneoActual.Id.ToString());
            if (time != System.TimeSpan.Zero)
            {
                daysLeft.text = time.Days.ToString();
                hoursLeft.text = time.Hours.ToString();
                minutesLeft.text = time.Minutes.ToString();
                secondsLeft.text = time.Seconds.ToString();
            }
        }
    }
    bool sw = false;
    public void tempBtn()
    {
        sw = !sw;
        if (sw)
        {
            gameObject.transform.GetChild(4).transform.GetChild(0).gameObject.SetActive(false);
            gameObject.transform.GetChild(4).transform.GetChild(1).gameObject.SetActive(false);
            gameObject.transform.GetChild(4).transform.GetChild(2).gameObject.SetActive(false);
            gameObject.transform.GetChild(4).transform.GetChild(3).gameObject.SetActive(true);
            gameObject.transform.GetChild(4).transform.GetChild(4).gameObject.SetActive(true);
            gameObject.transform.GetChild(4).transform.GetChild(5).gameObject.SetActive(true);
            gameObject.transform.GetChild(4).transform.GetChild(6).gameObject.SetActive(false);
        }
        else
        {

            if (VersionNv.torneoActual != null)
            {
                gameObject.transform.GetChild(4).transform.GetChild(1).gameObject.SetActive(true);
                gameObject.transform.GetChild(4).transform.GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                gameObject.transform.GetChild(4).transform.GetChild(6).gameObject.SetActive(true);
            }
            gameObject.transform.GetChild(4).transform.GetChild(0).gameObject.SetActive(true);
            gameObject.transform.GetChild(4).transform.GetChild(3).gameObject.SetActive(false);
            gameObject.transform.GetChild(4).transform.GetChild(4).gameObject.SetActive(false);
            gameObject.transform.GetChild(4).transform.GetChild(5).gameObject.SetActive(false);
            
        }
    }
    private void OnDisable()
    {
        gameObject.transform.GetChild(4).transform.GetChild(0).gameObject.SetActive(true);
        gameObject.transform.GetChild(4).transform.GetChild(1).gameObject.SetActive(true);
        gameObject.transform.GetChild(4).transform.GetChild(2).gameObject.SetActive(true);
        gameObject.transform.GetChild(4).transform.GetChild(3).gameObject.SetActive(false);
        gameObject.transform.GetChild(4).transform.GetChild(4).gameObject.SetActive(false);
        gameObject.transform.GetChild(4).transform.GetChild(5).gameObject.SetActive(false);
        gameObject.transform.GetChild(4).transform.GetChild(6).gameObject.SetActive(false);
        gameObject.transform.GetChild(4).transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.transform.GetChild(4).transform.GetChild(5).gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }
    void LoadRecordTournament()
    {
        bool firsItem = true;
        //TorneoRecordList ob;
        foreach (var item in VersionNv.TorneosPasados)
        {
            if (firsItem)
            {
                RecordContainer.GetChild(1).gameObject.GetComponent<TorneoRecordList>().idEvent = item.Value.Id;
                RecordContainer.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = item.Value.Detalles[1].Split("-")[1];
                RecordContainer.GetChild(1).gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = item.Value.Date_End;
                firsItem = false;
            }
            else
            {
                GameObject go = Instantiate(RecordContainer.transform.GetChild(1).gameObject, RecordContainer.transform.position, Quaternion.identity, RecordContainer.transform);
                go.GetComponent<TorneoRecordList>().idEvent = item.Value.Id;
                go.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = item.Value.Detalles[1].Split("-")[1];
                go.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = item.Value.Date_End;
            }
        }

    }
    public void setDataRecordEvent(string nameEvent, string idEvent)
    {
        StopAllCoroutines();
        titleEvent.text = nameEvent;
        PlayfabTournament.RecordLeaderboardTournament(idEvent);
        gameObject.transform.GetChild(4).transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(4).transform.GetChild(5).gameObject.transform.GetChild(1).gameObject.SetActive(true);

    }
    int countImg = 0;
    int countPlayers = 0;
    public void setWinnersPosition()
    {
        countImg = 0;
        countPlayers = 0;
        GameObject pos1 = parentPosition.transform.GetChild(1).gameObject;
        GameObject pos2 = parentPosition.transform.GetChild(0).gameObject;
        GameObject pos3 = parentPosition.transform.GetChild(2).gameObject;
        pos1.SetActive(false);
        pos2.SetActive(false);
        pos3.SetActive(false);
        foreach (var player in PlayfabTournament.currentparticipantes)
        {
            if (player.pos == 1)
            {
                countPlayers++;
                pos1.SetActive(true);
                //StartCoroutine(loadPlayerAvatar(player.avatarurl, pos1.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>()));
                InterfaceManager.Instance.setImage(player.avatarurl, pos1.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>());
                //pos1.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = player.pos.ToString(); //POSICION DE JUGADOR
                pos1.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = player.displayname; //NOMBRE DE JUGADOR
                pos1.transform.GetChild(3).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Flags/" + player.location); //LUGAR DEL JUGADOR
                pos1.transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>().text = player.statvalue.ToString(); //PUNTAJE DEL JUGADOR
            }
            else if (player.pos == 2)
            {
                countPlayers++;
                pos2.SetActive(true);
                //StartCoroutine(loadPlayerAvatar(player.avatarurl, pos2.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>()));
                InterfaceManager.Instance.setImage(player.avatarurl, pos2.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>());
                //pos2.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = player.pos.ToString(); //POSICION DE JUGADOR
                pos2.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = player.displayname; //NOMBRE DE JUGADOR
                pos2.transform.GetChild(3).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Flags/" + player.location); //LUGAR DEL JUGADOR
                pos2.transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>().text = player.statvalue.ToString(); //PUNTAJE DEL JUGADOR
            }
            else if (player.pos == 3)
            {
                countPlayers++;
                pos3.SetActive(true);
                //StartCoroutine(loadPlayerAvatar(player.avatarurl, pos3.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>()));
                InterfaceManager.Instance.setImage(player.avatarurl, pos3.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>());
                //pos3.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = player.pos.ToString(); //POSICION DE JUGADOR
                pos3.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = player.displayname; //NOMBRE DE JUGADOR
                pos3.transform.GetChild(3).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Flags/" + player.location); //LUGAR DEL JUGADOR
                pos3.transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>().text = player.statvalue.ToString(); //PUNTAJE DEL JUGADOR
            }

        }
        gameObject.transform.GetChild(4).transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.transform.GetChild(4).transform.GetChild(5).gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }
    //IEnumerator loadPlayerAvatar(string url, RawImage img)
    //{
    //    WWW www = new WWW(url);
    //    yield return www;
    //    if (!File.Exists(Application.persistentDataPath + imageName))
    //    {
    //        if (www.error == null)
    //        {
    //            Texture2D texture = www.texture;
    //            img.GetComponent<RawImage>().texture = texture;
    //            byte[] dataByte = texture.EncodeToPNG();
    //            File.WriteAllBytes(Application.persistentDataPath + imageName + "png", dataByte);

    //        }
    //    }
    //    else
    //    if (File.Exists(Application.persistentDataPath + imageName))
    //    {
    //        byte[] uploadByte = File.ReadAllBytes(Application.persistentDataPath + imageName);
    //        Texture2D texture = new Texture2D(10, 10);
    //        texture.LoadImage(uploadByte);
    //        img.GetComponent<RawImage>().texture = texture;
    //    }
    //    countImg++;
    //    if (countImg == countPlayers)
    //    {
    //        gameObject.transform.GetChild(4).transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.SetActive(true);
    //        gameObject.transform.GetChild(4).transform.GetChild(5).gameObject.transform.GetChild(1).gameObject.SetActive(false);
    //    }
    //}

}
