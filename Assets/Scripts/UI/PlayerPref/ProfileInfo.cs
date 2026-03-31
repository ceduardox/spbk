using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.GroupsModels;
using System.IO;
using Newtonsoft.Json;
using DG.Tweening;


enum data
{
    Copas,
    CopasClanes,
    Experiencia,
    NL,
    TE,
    TotalRaces,
    WinRaces
}
public class ProfileInfo : MasterScreen
{
    public static ProfileInfo instance;
    [Header("TRANSLATE TEXTS")]
    [SerializeField] TextMeshProUGUI rolText;
    [SerializeField] TextMeshProUGUI mailTagText;
    [SerializeField] TextMeshProUGUI regionTagText;
    [SerializeField] TextMeshProUGUI editProfileText;
    [SerializeField] TextMeshProUGUI addFriendText;
    [SerializeField] TextMeshProUGUI sendMessageText;
    [SerializeField] TextMeshProUGUI levelTagText;
    [SerializeField] TextMeshProUGUI totalRacesTagText;
    [SerializeField] TextMeshProUGUI winRacesTagText;
    [SerializeField] TextMeshProUGUI BtnBackText;


    [Header("DATA PROFILE TEXT")]
    [SerializeField] TextMeshProUGUI nameClanText;
    [SerializeField] TextMeshProUGUI infoTagMail;
    public TextMeshProUGUI nameInfoText;
    [SerializeField] TextMeshProUGUI infoLevelTagText;
    [SerializeField] TextMeshProUGUI infoTnlText;
    [SerializeField] TextMeshProUGUI infoTelText;
    [SerializeField] TextMeshProUGUI infoCoupsText;
    [SerializeField] TextMeshProUGUI infoProgressText;
    [SerializeField] TextMeshProUGUI infoTotalRacesTagText;
    [SerializeField] TextMeshProUGUI infoWinRacesTagText;


    [Header("SOURCES")]
    [SerializeField] string currentIdClan;
    [SerializeField] string playfabid;
    [SerializeField] string ClanName;
    [SerializeField] Image profilePicture;
    [SerializeField] RawImage rolImage;
    [SerializeField] Sprite iconCorona;
    [SerializeField] Sprite iconEspadas;
    [SerializeField] Button btnShowClan;
    [SerializeField] GameObject btnEditProfile;
    [SerializeField] Button btnSendMessage;
    [SerializeField] GameObject closeSession;
    [SerializeField] GameObject rawTnl;
    [SerializeField] GameObject rawTel;
    [SerializeField] GameObject mail;
    [SerializeField] RectTransform progressBar;
    [SerializeField] RectTransform totalProgressBar;
    [SerializeField] GameObject profile_WarningLinkAccIcon;
    [Header("EFFECTS")]
    [SerializeField] Transform stat1;
    [SerializeField] Transform stat2;
    [SerializeField] Transform stat3;
    [SerializeField] Image leftImage;
    [SerializeField] Image rightImage;
    [Header("SOURCES")]
    [SerializeField] GameObject objectAddFriend;
    public GameObject objectChangeDisplayname;
    [SerializeField] public TMP_InputField DisplayName;
    [SerializeField] public TMP_InputField NewDisplayName;
    private string imageName;
    public string urlImg;

    private Dictionary<string, string> membersRoles = new Dictionary<string, string>();


    public void startPoup()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }
    private void Start()
    {
        //setImageAvatar(PlayfabManager.instance.urlAvatar);
        
        Invoke("startAnimation",0.15f);
        setEffects();
        Invoke("setEffectRewards", 0.2f);
    }
    void setEffects()
    {
        stat1.DOScale(new Vector3(0f, 0f, 0f), 0f);
        stat2.DOScale(new Vector3(0f, 0f, 0f), 0f);
        stat3.DOScale(new Vector3(0f, 0f, 0f), 0f);
    }
    void setEffectRewards()
    {
        stat1.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetEase(Ease.InElastic).OnComplete(() =>
        stat2.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetEase(Ease.InElastic).OnComplete(() =>
        stat3.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetEase(Ease.InElastic)));
        leftImage.DOFade(0f,1f).OnComplete(()=> leftImage.DOFade(1f, 1f)).SetEase(Ease.Linear).SetLoops(5,LoopType.Yoyo);
        rightImage.DOFade(0f, 1f).OnComplete(() => rightImage.DOFade(1f, 1f)).SetEase(Ease.Linear).SetLoops(5, LoopType.Yoyo);
    }
    void startAnimation()
    {
        //StartCoroutine(numberAnimation(infoWinRacesTagText, infoWinRacesTagText.text.ToString()));
        //StartCoroutine(numberAnimation(infoTotalRacesTagText, infoTotalRacesTagText.text.ToString()));
    }
    private void OnEnable()
    {
        setWarningIcons();
        GetInfoProfile();
    }

    //FUNCION PARA TRAER TU PROPIO PERFIL
    public void GetInfoProfile()
    {

        btnEditProfile.GetComponent<Button>().interactable = true;
        btnSendMessage.interactable = false;
        rawTel.SetActive(true);
        mail.SetActive(true);
        rawTnl.SetActive(true);
        var req = new GetPlayerCombinedInfoRequest
        {
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {


                GetPlayerProfile = true,
                GetPlayerStatistics = true,
                GetUserAccountInfo = true,
                GetUserVirtualCurrency = true,

                ProfileConstraints = new PlayerProfileViewConstraints()
                {
                    ShowLastLogin = true,
                    ShowLocations = true,
                    ShowDisplayName = true
                }

            }
        };

        PlayFabClientAPI.GetPlayerCombinedInfo(req, onGetProfileSucces, onerror);
    }
    public void setImageAvatar()
    {
        //StopAllCoroutines();
        //StartCoroutine(loadProfile());
        InterfaceManager.Instance.setImage(urlImg,profilePicture);
    }
   
    public void setWarningIcons()
    {
        CLog.Log("Setting Warning Icons");
        if(PlayfabManager.instance.isGuest()) profile_WarningLinkAccIcon.SetActive(true);
        else profile_WarningLinkAccIcon.SetActive(false);
    }
    //IEnumerator loadProfile()
    //{
    //    WWW www = new WWW(urlImg);
    //    yield return www;
    //    if (!File.Exists(Application.persistentDataPath + imageName))
    //    {
    //        if (www.error == null)
    //        {
    //            Texture2D texture = www.texture;
    //            profilePicture.GetComponent<RawImage>().texture = texture;
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
    //        profilePicture.GetComponent<RawImage>().texture = texture;
    //    }

    //}
   

    private void onGetProfileSucces(GetPlayerCombinedInfoResult obj)
    {
        int experiencia = obj.InfoResultPayload.PlayerStatistics.Find(x => x.StatisticName == data.Experiencia.ToString()).Value;
        int level = PlayfabManager.instance.getLevel2(experiencia);
        int max = PlayfabManager.levels[level];

        playfabid = obj.PlayFabId;
        //PLAYER PROFILE
        nameInfoText.text = obj.InfoResultPayload.PlayerProfile.DisplayName;
        infoLevelTagText.text = level.ToString();
        infoProgressText.text = experiencia.ToString() + "/" + max.ToString();
        mailTagText.text = obj.InfoResultPayload.AccountInfo.PrivateInfo.Email;
        closeSession.SetActive(true);
        urlImg = PlayfabManager.instance.urlAvatar;
        Invoke("setImageAvatar", 1f);

        //PLAYER ECONOMY
        infoTnlText.text = obj.InfoResultPayload.UserVirtualCurrency[data.NL.ToString()].ToString();
        infoTelText.text = obj.InfoResultPayload.UserVirtualCurrency[data.TE.ToString()].ToString();

        //PLAYER STATS
        infoCoupsText.text = obj.InfoResultPayload.PlayerStatistics.Find(x => x.StatisticName == data.Copas.ToString()).Value.ToString();
        string NewWinRace = obj.InfoResultPayload.PlayerStatistics.Find(x => x.StatisticName == data.WinRaces.ToString()).Value.ToString();
        string NewRacesPlayed = obj.InfoResultPayload.PlayerStatistics.Find(x => x.StatisticName == data.TotalRaces.ToString()).Value.ToString();
        
        StartCoroutine(numberAnimation(infoWinRacesTagText, NewWinRace));
        StartCoroutine(numberAnimation(infoTotalRacesTagText, NewRacesPlayed));

        setProgressBar(experiencia, max);

        //gameObject.SetActive(true);
        var req = new GetPlayerProfileRequest();
        PlayFabClientAPI.GetPlayerProfile(req, onSuccesGetProfile, onerror);
        //CLog.Log(obj.InfoResultPayload.PlayerStatistics.Find(x=>x.StatisticName==data.CopasClanes.ToString()).Value);
    }
    IEnumerator numberAnimation(TextMeshProUGUI ob , string realNumber)
    {
        float finalValue = float.Parse(realNumber);
        float contador = 1.5f;
        float valorParcial=00;
        //yield return new WaitForSeconds(contador);
        while (contador > 0)
        {
            ob.text = valorParcial.ToString();
            valorParcial = valorParcial + Random.Range(1, 6);
            if (valorParcial > 99) { valorParcial = 0; }
            contador -= Time.deltaTime;
            if (contador <= 0f) { ob.text = finalValue.ToString(); };
            yield return new WaitForEndOfFrame();
        }
    }
    private void onSuccesGetProfile(GetPlayerProfileResult obj)
    {

        var req = new GetUserDataRequest { Keys = new List<string> { STATS.CLAN } };

        PlayFabClientAPI.GetUserReadOnlyData(req, onGetUserReadOnlySucces, onerror);
    }

    private void onGetUserReadOnlySucces(GetUserDataResult obj)
    {
        if (obj.Data.Count != 0)
        {

            currentIdClan = obj.Data[STATS.CLAN].Value;
            PlayFabGroupsAPI.GetGroup(new GetGroupRequest
            {
                Group = new PlayFab.GroupsModels.EntityKey { Id = currentIdClan, Type = "group" }
            }, onGetGroupSucces,
                onGetGroupError
            );
        }
        else
        {
            noClan();

        }
    }

    private void onerror(PlayFabError obj)
    {
        CLog.Log(obj.ErrorMessage);
    }




    //FUNCION PARA TRAER EL PERFIL DE OTRO JUGADOR PASANDO SU DISPLAYNAME

    public void GetInfoProfile(string displayname)
    {

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { TitleDisplayName = displayname }, ongetAccountSucces, onGetAccountError);

    }

    private void ongetAccountSucces(GetAccountInfoResult obj)
    {
        btnEditProfile.GetComponent<Button>().interactable = false;
        btnSendMessage.interactable = true;
        rawTel.SetActive(false);
        mail.SetActive(false);
        rawTnl.SetActive(false);

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest { FunctionName = "getProfile", FunctionParameter = new { PlayFabId = obj.AccountInfo.PlayFabId } }, onGetProfileCloudSucces, onGetProfileCloudError);

    }

    private void onGetProfileCloudSucces(ExecuteCloudScriptResult obj)
    {

        CLog.Log(obj.FunctionResult);
        var otherProfile = JsonConvert.DeserializeObject<OtherProfile>(obj.FunctionResult.ToString());

        int experiencia = otherProfile.stats.Find(x => x.StatisticName == data.Experiencia.ToString()).Value;
        int level = PlayfabManager.instance.getLevel2(experiencia);
        int max = PlayfabManager.levels[level + 1];

        playfabid = otherProfile.playid;
        //PLAYER PROFILE
        nameInfoText.text = otherProfile.name;
        infoLevelTagText.text = level.ToString();
        infoProgressText.text = experiencia.ToString() + "/" + max.ToString();
        closeSession.SetActive(false);
        urlImg = otherProfile.avatarUrl.ToString();
        Invoke("setImageAvatar", 1f);

        //PLAYER STATS
        infoCoupsText.text = otherProfile.stats.Find(x => x.StatisticName == data.Copas.ToString()).Value.ToString();
        infoTotalRacesTagText.text = otherProfile.stats.Find(x => x.StatisticName == data.TotalRaces.ToString()).Value.ToString();
        infoWinRacesTagText.text = otherProfile.stats.Find(x => x.StatisticName == data.WinRaces.ToString()).Value.ToString();

        setProgressBar(experiencia, max);

        if (otherProfile.idgroup != null)
        {
            currentIdClan = otherProfile.idgroup.ToString();
            PlayFabGroupsAPI.GetGroup(
                new GetGroupRequest
                {
                    Group = new PlayFab.GroupsModels.EntityKey
                    {
                        Id = otherProfile.idgroup.ToString(),
                        Type = "group"
                    }
                },
                onGetGroupSucces,
                onGetGroupError
            );
        }
        else
        {
            noClan();
        }
        gameObject.SetActive(true);
        //currentIdClan=otherProfile.idgroup==null?noClan():otherProfile.idgroup;
    }

    private void onGetProfileCloudError(PlayFabError obj)
    {
        CLog.Log(obj.ErrorMessage);
    }
    private void onGetAccountError(PlayFabError obj)
    {
        CLog.Log("ERROR AL TRAER INFO DEL JUGADOR");
        LittlePopUpManager.instance.setSmallPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_FND_02));
    }

    private void setProgressBar(int exp, int totalExp)
    {

        progressBar.sizeDelta = new Vector2(((float)exp / (float)totalExp) * totalProgressBar.sizeDelta.x, progressBar.sizeDelta.y);
    }
    private string noClan()
    {

        CLog.Log("NO EXITE CLAN AL QUE ACCEDER");
        btnShowClan.interactable = false;
        nameClanText.text = "NoClan";
        rolText.text = "--";
        return "";
    }

    private void onGetGroupError(PlayFabError obj)
    {
        CLog.Log(obj.ErrorMessage);
        btnShowClan.interactable = false;
        nameClanText.text = "NoClan";
        rolText.text = "--";

    }

    private void onGetGroupSucces(GetGroupResponse obj)
    {
        ClanName = obj.GroupName;
        //nameClanText.text = "clan\n" + obj.GroupName;
        nameClanText.text = obj.GroupName;
        //rolText.text=obj.Roles[nameInfoText.text];
        var req = new ExecuteCloudScriptRequest { FunctionName = "getListMembersStruct", FunctionParameter = new { idc = obj.Group.Id } };
        PlayFabClientAPI.ExecuteCloudScript(req, onGetListMembersSucces, onGetListMembersError);
    }


    private void onGetListMembersError(PlayFabError obj)
    {
        CLog.Log(obj.ErrorMessage);
    }

    private void onGetListMembersSucces(ExecuteCloudScriptResult obj)
    {
        CLog.Log("TRAYENDO MIEMBROOOOOOOS");
        membersRoles.Clear();
        List<string> ids = new List<string>();
        ListGroupMembersResponse ob = JsonConvert.DeserializeObject<ListGroupMembersResponse>(obj.FunctionResult.ToString());
        CLog.Log(obj.FunctionResult);
        foreach (var item in ob.Members)
        {
            CLog.Log(item.RoleId);
            foreach (var it in item.Members)
            {

                foreach (var i in it.Lineage)
                {
                    ids.Add(i.Value.Id);
                    membersRoles.Add(i.Value.Id, item.RoleId);
                }
            }
        }


        if (membersRoles[playfabid] == "admins")
        {
            rolText.text = ROL.Admin.ToString();
            rolImage.texture = iconCorona.texture;
        }
        else
        {
            rolText.text = ROL.Member.ToString();
            rolImage.texture = iconEspadas.texture;
        }


        PlayFabClientAPI.ExecuteCloudScript(
            new ExecuteCloudScriptRequest
            {
                FunctionName = "getMembersGroupProfile",
                FunctionParameter = new { ides = ids }
            }, onExecuteCloudSucces, onExecuteCloudError
        );
        btnShowClan.interactable = true;
    }
    public static Miembros dataMiembros;
    private void onExecuteCloudSucces(ExecuteCloudScriptResult obj)
    {
        dataMiembros = JsonConvert.DeserializeObject<Miembros>(obj.FunctionResult.ToString());
    }

    private void onExecuteCloudError(PlayFabError obj)
    {
        CLog.Log("NO SE CONSIGUIO ENVIAR EL CLOUD");
    }

    public void openClanUi()
    {

        ClanExternoInfo.instance.setDataClan(dataMiembros, membersRoles, ClanName);
    }

    public void AgregarAmigo()
    {
        addFriends();
    }
    public void ActualizarDisplayname()
    {
        PlayfabManager.instance.UpdateNameUser2(NewDisplayName.text);
    }
    
    private void addFriends()
    {
        var req = new GetAccountInfoRequest { TitleDisplayName = DisplayName.text };
        PlayFabClientAPI.GetAccountInfo(req, onGetAccountSucces, onGetAccountError);
    }
    private void onGetAccountSucces(GetAccountInfoResult obj)
    {
        var req = new ExecuteCloudScriptRequest()
        {
            FunctionName = "SendFriendRequest",
            FunctionParameter = new { FriendPlayFabId = obj.AccountInfo.PlayFabId.ToString() }
        };

        PlayFabClientAPI.ExecuteCloudScript(req, onExecutesucces, onExecuteError);
    }
    private void onExecutesucces(ExecuteCloudScriptResult obj)
    {
        //PopUpAddFriend.SetActive(false);
        LittlePopUpManager.instance.setSmallPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_FND_01));
        DisplayName.text = "";
        objectAddFriend.SetActive(false);
        FriendSystem.getFriends();
    }
    private void onExecuteError(PlayFabError obj)
    {
        LittlePopUpManager.instance.setSmallPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_FND_02));
    }
}




// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class OtherProfile
{
    public string playid { get; set; }
    public string name { get; set; }
    public string avatarUrl { get; set; }
    public List<Stat> stats { get; set; }
    public object idgroup { get; set; }
}

public class Stat
{
    public string StatisticName { get; set; }
    public int Value { get; set; }
    public int Version { get; set; }
}

public class Miembor
{
    public string playid { get; set; }
    public string name { get; set; }
    public List<Stat> stats { get; set; }
}

public class Miembros
{
    public List<Miembor> miembors { get; set; }
}
