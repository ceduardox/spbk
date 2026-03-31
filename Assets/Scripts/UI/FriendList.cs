using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System.IO;
using UnityEngine.UI;

public class FriendList : MonoBehaviour
{
    public static FriendList instance;

    [SerializeField] public GameObject objFriend;
    [SerializeField] public int friendCoubt;
    [SerializeField] public Transform Container;
    [SerializeField] public GameObject objFriendChat;
    [SerializeField] public Transform ContainerChat;
    [SerializeField] public GameObject objRequest;
    [SerializeField] public Transform ContainerReq;
    [SerializeField] public TMP_InputField idFriend;
    [SerializeField] public TextMeshProUGUI displayName;
    [SerializeField] public Image imgAvatar;
    private string imageName;
    [SerializeField] public TextMeshProUGUI displayLvl;
    [SerializeField] public TextMeshProUGUI MessageText;
    public string currentDisplayName;
    //[Header("SECCION IMAGE PROFILES")]
    //[SerializeField] public Transform ContainerImageProfiles;
    //[SerializeField] public GameObject objItemImageProfile;
    //public Dictionary<string, Texture> textureListImages = new Dictionary<string, Texture>();
    //[SerializeField] public Button btnProfileAvatar;
    //[SerializeField] public string currentUrlImage;
    [Header("POP UPS")]
    public GameObject PopUpAddFriend;
    public GameObject PopUpAvatarPanel;


    [Header("BUTTONS MENU CURRENT")]
    public ButtonMenu[] MenuNavegation;

    [System.Serializable]
    public struct ButtonMenu
    {
        public Button BtnMenu;
        public GameObject PanelMenu;
    }
    //public Dictionary<string, Texture> ListaImagenes = new Dictionary<string, Texture>();

    private void Awake()
    {
        CLog.Log("NAME : " + name);
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
        cargarDatos();
        onGetListFriends();
        //idFriend.onValueChanged.AddListener(delegate { findFriend(); });
    }
    public void findFriend(TMP_InputField input)
    {
        clearList(Container);
        if (input.text != "")
        {
            foreach (KeyValuePair<string, FriendBase> friend in FriendSystem.friends)
            {
                if (friend.Value.DisplayName.Contains(input.text) && friend.Value.Tag == "isfriend")
                {
                    FriendItemUI itemTMP;
                    //FriendItemUI itemTMP2;
                    GameObject oblist = Instantiate(objFriend, this.transform.position, Quaternion.identity);
                    oblist.transform.SetParent(Container);
                    itemTMP = oblist.GetComponent<FriendItemUI>();
                    itemTMP.DisplayName.text = friend.Value.DisplayName;
                    //itemTMP.DisplayLvl.text = friend.Value.DisplayName;
                    itemTMP.DisplayLastLogin.text = friend.Value.LastLogin;
                    itemTMP.chargeImage(friend.Value.AvatarUrl);
                }
            }
        }
        else
        {
            onGetListFriends();
        }


    }
    private void OnEnable()
    {
        FriendSystem.getFriends();
    }
    public void currentButton(Button btn)
    {
        for (int i = 0; i < MenuNavegation.Length; i++)
        {
            if (MenuNavegation[i].BtnMenu.name == btn.name)
            {
                MenuNavegation[i].BtnMenu.interactable = false;
                MenuNavegation[i].PanelMenu.SetActive(true);
            }
            else
            {
                MenuNavegation[i].BtnMenu.interactable = true;
                MenuNavegation[i].PanelMenu.SetActive(false);
            }
        }

    }

    void cargarDatos()
    {
        displayName.text = PlayfabManager.instance.displayName;
        displayLvl.text = "Nivel " + PlayfabManager.instance.getLevel().ToString();
        setAvatarImage(PlayfabManager.instance.urlAvatar);

    }
    public void setAvatarImage(string avatarImg)
    {
        //StartCoroutine(loadImage(avatarImg));
        InterfaceManager.Instance.setImage(avatarImg, imgAvatar);
    }

    //IEnumerator loadImage(string urlImage)
    //{
    //    WWW www = new WWW(urlImage);
    //    yield return www;
    //    if (!File.Exists(Application.persistentDataPath + imageName))
    //    {
    //        if (www.error == null)
    //        {
    //            Texture2D texture = www.texture;
    //            imgAvatar.GetComponent<RawImage>().texture = texture;
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
    //        imgAvatar.GetComponent<RawImage>().texture = texture;
    //    }

    //}
    public void clearList(Transform c)
    {
        foreach (Transform child in c)
        {
            Destroy(child.gameObject);
        }
        //foreach (Transform child in ContainerReq)
        //{
        //    Destroy(child.gameObject);
        //}
    }
    public void AgregarAmigo()
    {
        addFriends();

        //getFriendList();
    }
    internal void getFriendList()
    {
        //var req = new GetFriendsListRequest();
        //PlayFabClientAPI.GetFriendsList(req, onGetListFriends, onFriendError);
    }

    public void onGetListFriends()
    {
        //if (friendCount != FriendSystem.friends.Count)
        //{
        clearList(Container);
        Dictionary<string, FriendBase> listFriends = FriendSystem.friends;
        foreach (KeyValuePair<string, FriendBase> friend in listFriends)
        {
            if (friend.Value.Tag == "isfriend")
            {
                FriendItemUI itemTMP;
                GameObject oblist = Instantiate(objFriend, this.transform.position, Quaternion.identity);
                oblist.transform.SetParent(Container);
                itemTMP = oblist.GetComponent<FriendItemUI>();
                itemTMP.DisplayName.text = friend.Value.DisplayName;
                //itemTMP.DisplayLvl.text = friend.Value.;
                itemTMP.DisplayLastLogin.text = friend.Value.LastLogin;
                itemTMP.chargeImage(friend.Value.AvatarUrl);

            }

        }
        //}

    }
    public void onGetChatFriends()
    {
        //if (friendCount != FriendSystem.friends.Count)
        //{
        clearList(ContainerChat);
        //FriendSystem.friends;
        Dictionary<string, FriendBase> listFriends = FriendSystem.friends;
        foreach (KeyValuePair<string, FriendBase> friend in listFriends)
        {
            if (friend.Value.Tag == "isfriend")
            {
                FriendItemUI itemTMP2;
                GameObject obchat = Instantiate(objFriendChat, this.transform.position, Quaternion.identity);
                obchat.transform.SetParent(ContainerChat);
                itemTMP2 = obchat.GetComponent<FriendItemUI>();
                itemTMP2.DisplayName.text = friend.Value.DisplayName;
                //itemTMP.DisplayLvl.text = friend.Value.DisplayName;
                itemTMP2.DisplayLastLogin.text = friend.Value.LastLogin;
                itemTMP2.chargeImage(friend.Value.AvatarUrl);
                obchat.gameObject.GetComponent<SetChatPrivate>().chatInputPrivate = Chat._instance.chatInputPrivate;
                }
        }
        //}
    }

    public void onGetRequestFriends()
    {
        //if (friendCount != FriendSystem.friends.Count)
        //{
        clearList(ContainerReq);
        //FriendSystem.friends;
        Dictionary<string, FriendBase> listFriends = FriendSystem.friends;
        foreach (KeyValuePair<string, FriendBase> friend in listFriends)
        {
            if (friend.Value.Tag == "request")
            {
                FriendItemUI itemTMP;
                GameObject ob = Instantiate(objRequest, this.transform.position, Quaternion.identity);
                ob.transform.SetParent(ContainerReq);
                itemTMP = ob.GetComponent<FriendItemUI>();
                itemTMP.DisplayName.text = friend.Value.DisplayName;
                itemTMP.chargeImage(friend.Value.AvatarUrl);
            }
        }
        //}
    }



    public void acceptFriend(string displayName)
    {
        var req = new GetAccountInfoRequest { TitleDisplayName = displayName };
        PlayFabClientAPI.GetAccountInfo(req, onGetAccountAcceptSucces, onGetAccountError);
    }
    private void onGetAccountAcceptSucces(GetAccountInfoResult obj)
    {
        var req = new ExecuteCloudScriptRequest()
        {
            FunctionName = "AcceptFriendRequest",
            FunctionParameter = new { FriendPlayFabId = obj.AccountInfo.PlayFabId.ToString() }
        };

        PlayFabClientAPI.ExecuteCloudScript(req, onExecutesuccesAccept, onExecuteError);
    }
    private void onExecutesuccesAccept(ExecuteCloudScriptResult obj)
    {
        //LittlePopUpManager.instance.setSmallPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_FND_01));
        FriendSystem.getFriends();

    }
    public void declineFriend(string displayName)
    {
        var req = new GetAccountInfoRequest { TitleDisplayName = displayName };
        PlayFabClientAPI.GetAccountInfo(req, onGetAccountDeclineSucces, onGetAccountError);
    }
    private void onGetAccountDeclineSucces(GetAccountInfoResult obj)
    {
        var req = new ExecuteCloudScriptRequest()
        {
            FunctionName = "DenyFriendRequest",
            FunctionParameter = new { FriendPlayFabId = obj.AccountInfo.PlayFabId.ToString() }
        };

        PlayFabClientAPI.ExecuteCloudScript(req, onExecutesuccesAccept2, onExecuteError);
    }
    private void onExecutesuccesAccept2(ExecuteCloudScriptResult obj)
    {
        FriendSystem.getFriends();
    }
    private void addFriends()
    {
        var req = new GetAccountInfoRequest { TitleDisplayName = idFriend.text };
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
        idFriend.text = "";
        PopUpAddFriend.SetActive(false);
        FriendSystem.getFriends();
    }

    private void onExecuteError(PlayFabError obj)
    {
        LittlePopUpManager.instance.setSmallPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_FND_02));
    }
    private void onGetAccountError(PlayFabError obj)
    {
        LittlePopUpManager.instance.setSmallPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_FND_02));
    }
    internal void onFriendError(PlayFabError obj)
    {
        CLog.Log(obj.Error);
    }

    public class List<T1, T2>
    {
    }
}
