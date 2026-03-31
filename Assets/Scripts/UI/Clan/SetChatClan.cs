using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SetChatClan : MonoBehaviour
{
    public Transform chatContainerGroup;
    public ScrollRect scrollRectGroup;
    public TMP_InputField chatInputGroup;
    public TextMeshProUGUI nameClan;
    public TextMeshProUGUI membresClan;
    // Start is called before the first frame update
    private void Start() {
        
        Chat._instance.getRegisterChatGroup(chatContainerGroup,scrollRectGroup,chatInputGroup);
        //nameClan.text = ClanSystem.nombreClan;
        //membresClan.text = ClanSystem.membersCount.ToString()+" Miembros";
    }
    private void OnEnable() {

        Chat._instance.chatContainerGroup=chatContainerGroup;
        Chat._instance.scrollRectGroup=scrollRectGroup;
        Chat._instance.chatInputGroup=chatInputGroup;
    }
    public void SendChatMessageGroup(){
        CLog.Log("ENVIANDO MENSAJE DEL CHAT");
        Chat._instance.SendChatMessageGroup();
    }
    void OnGUI()
    {
        if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter && chatInputGroup.text != "")
        {
            SendChatMessageGroup();
            chatInputGroup.Select();
            chatInputGroup.ActivateInputField();
        }
    }

}
