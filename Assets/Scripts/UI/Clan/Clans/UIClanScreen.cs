using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class UIClanScreen : MonoBehaviour
{
    public static UIClanScreen instance;
    [Header("Data My Clan")]
    public Transform ContentUIMiClan;
    public GameObject itemPlayerObject;
    public TextMeshProUGUI TextNameClan;
    public TextMeshProUGUI TextTotalMembers;
    public TextMeshProUGUI TextTotalCups;
    public GameObject loaderImg;
    [Header("Controls only Admins")]
    public bool isAdmin = false;
    public Button BtnRequest;
    public Button BtnSendRequest;
    public Button BtnLeave;
    [Space]
    [Header("Data Explorer Clan")]
    public Transform ContentUIExplorerClan;
    public GameObject itemPlayerObjectExplorer;
    public string TextSelectedClan;
    [Space]
    [Header("Data Top Clan")]
    public Transform ContentUITopClan;
    public GameObject itemPlayerObjectTop;
    [Space]
    [Header("Data PopUps")]
    public TMP_InputField inputDisplayPlayer;
    public TextMeshProUGUI messageText;
    public GameObject popupInviteClan;
    //public Vector2 _screenSize;
    [Space]
    [Header("Data PopUps Options")]
    public menuAddFriend popupOptions;
    public string currentDisplayName;

    public Button btnViewProfile;
    public Button btnAddFriend;
    public Button btnKickPlayer;
    //public GameObject uiMainCanvas;
    //GraphicRaycaster ui_raycaster;

    //PointerEventData click_data;
    //List<RaycastResult> click_results;
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

        //_screenSize = new Vector2(Screen.width, Screen.height);
        //ClanSystem.GetMyGroup();
        //ListaDeClanesSystem.getClans();
        //ui_raycaster = uiMainCanvas.GetComponent<GraphicRaycaster>();
        //click_data = new PointerEventData(EventSystem.current);
        //click_results = new List<RaycastResult>();
    }

    public void showAdminControls(bool _isAdmin)
    {
        isAdmin = _isAdmin;
        if (isAdmin)
        {
            BtnRequest.interactable = true;
            BtnSendRequest.interactable = true;
            BtnLeave.interactable = false;
        }
    }
    private void OnEnable()
    {
        loaderImg.SetActive(true);
        ClanSystem.GetMyGroup();
        ListaDeClanesSystem.getClans();
        //TopClanSystem.getClans();
        //ListaDeClanesSystem.getClans();
    }

    private void OnDisable()
    {
        //gameObject.GetComponent<PopUpCreateClan>().noClanScreen.SetActive(false);
    }
    void cleanContainer(Transform parent)
    {
        for (int i = 2; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
    public void getMyClanMembers()
    {
        for (int i = 2; i < ContentUIMiClan.childCount; i++)
        {
            Destroy(ContentUIMiClan.GetChild(i).gameObject);
        }
        GameObject itemMember;
        int memberNumer = 0;
        int totalCups = 0;
        foreach (var miembros in ClanSystem.miembros)
        {
            int level = PlayfabManager.instance.getLevel2(miembros.exp);
            itemMember = Instantiate(itemPlayerObject, ContentUIMiClan);
            memberNumer++;
            itemMember.GetComponent<UIClanItemMiClan>().TextPositionMember.text = memberNumer.ToString();
            itemMember.GetComponent<UIClanItemMiClan>().TextNamePlayer.text = miembros.nombe.ToString();
            itemMember.GetComponent<UIClanItemMiClan>().TextRangePlayer.text = miembros.Rol.ToString();
            itemMember.GetComponent<UIClanItemMiClan>().TextCupsPlayer.text = miembros.copas.ToString();
            itemMember.GetComponent<UIClanItemMiClan>().TextCupsTeamPlayer.text = level.ToString();
            totalCups = totalCups + miembros.copas;
        }
        TextTotalMembers.text = memberNumer.ToString() + "/20";
        TextTotalCups.text = totalCups.ToString();
    }
    public void showPopUpOptions()
    {

        popupOptions.gameObject.SetActive(true);
        popupOptions.transform.position = Mouse.current.position.ReadValue() + new Vector2(60f, -30f);

    }
    public void showPopUpOptions(string displayName)
    {
        popupOptions.gameObject.SetActive(true);
        popupOptions.displayName.text = displayName;
        popupOptions.transform.position = Mouse.current.position.ReadValue() + new Vector2(60f, -30f);

    }
    public void showProfilePlayerClan()
    {
        ProfileInfo.instance.GetInfoProfile(popupOptions.displayName.text);
    }
    public void addFriendPlayerClan()
    {
        addFriends();
    }
    public void getClansInvitations(List<LosClane> listaClan)
    {
        //CLog.Log(listaClan.Count+"CAAAAAAAAAAAAAAAAAAAAAAAAANS");


        GameObject itemMember;


        foreach (var clan in listaClan)
        {

            itemMember = Instantiate(itemPlayerObjectExplorer, ContentUIExplorerClan);
            itemMember.GetComponent<UIClanItemExplorar>().TextNameClan.text = clan.GroupName.ToString();
            itemMember.GetComponent<UIClanItemExplorar>().TextMiembros.text = clan.TotalMembers + "/20";
            itemMember.GetComponent<UIClanItemExplorar>().TextCopas.text = clan.CopasTotal.ToString();
            itemMember.GetComponent<UIClanItemExplorar>().btnSendInvitation.interactable = !ClanSystem.hasclan;

            //itemMember = Instantiate(ContentUIExplorerClan.GetChild(0).gameObject, ContentUIExplorerClan);
            //itemMember.GetComponent<UIClanItemExplorar>().TextNameClan.text = clan.GroupName.ToString();
            //itemMember.GetComponent<UIClanItemExplorar>().TextMiembros.text = clan.TotalMembers + "/20";
            //itemMember.GetComponent<UIClanItemExplorar>().TextCopas.text = clan.CopasTotal.ToString();
            //itemMember.GetComponent<UIClanItemExplorar>().btnSendInvitation.interactable = !ClanSystem.hasclan;

        }

    }

    public void getTopClans(List<LosClane> listaClan)
    {
        //CLog.Log(listaClan.Count+"CAAAAAAAAAAAAAAAAAAAAAAAAANS");
        GameObject itemMember;
        int numberPosition = 1;
        foreach (var topclan in listaClan)
        {

            itemMember = Instantiate(itemPlayerObjectTop, ContentUITopClan);
            itemMember.GetComponent<UIClanItemTop>().NumberPosition.text = numberPosition.ToString();
            itemMember.GetComponent<UIClanItemTop>().TextNameClan.text = topclan.GroupName.ToString();
            itemMember.GetComponent<UIClanItemTop>().TextMiembros.text = topclan.TotalMembers + "/20";
            itemMember.GetComponent<UIClanItemTop>().TextCopas.text = topclan.CopasTotal.ToString();

            //itemMember = Instantiate(ContentUITopClan.GetChild(0).gameObject, ContentUITopClan);
            //itemMember.GetComponent<UIClanItemTop>().NumberPosition.text = numberPosition.ToString();
            //itemMember.GetComponent<UIClanItemTop>().TextNameClan.text = topclan.GroupName.ToString();
            //itemMember.GetComponent<UIClanItemTop>().TextMiembros.text = topclan.TotalMembers + "/20";
            //itemMember.GetComponent<UIClanItemTop>().TextCopas.text = topclan.CopasTotal.ToString();

            numberPosition++;
        }

    }
    //public void getInfoClan(string groupName)
    //{
    //    ClanExternoInfo.instance.setDataClan(groupName);
    //}
    public void setNameClan(string nameclan)
    {
        TextNameClan.text = nameclan;
    }
    public void inviteMember()
    {
        ClanSystem.InviteToGroup(inputDisplayPlayer.text);
    }
    public void leaveGroup()
    {
        LittlePopUpManager.instance.setSmallPopUpConfirm(TranslateUI.getStringUI(UI_CODE.POPUP_CLN_06));

    }
    public void RequestJoinClan(string groupName)
    {
        OtherClanSystem.GetClan(groupName);
        //OtherClanSystem.Join();

    }



    private void addFriends()
    {
        //var req = new GetAccountInfoRequest { TitleDisplayName = DisplayTittle.text };
        PlayfabManager.instance.addFriends(popupOptions.displayName.text);
    }
}
