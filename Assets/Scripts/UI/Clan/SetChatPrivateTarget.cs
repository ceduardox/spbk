using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetChatPrivateTarget : MonoBehaviour
{
    public TextMeshProUGUI displayname;



    private void Start() {
        CLog.Log("PLAYER "+displayname.text +"IS CHILD: "+gameObject.transform.GetSiblingIndex());
    }
    public void setTarget(){
        CLog.Log($"{Chat._instance.targetPlayfabid} {FriendSystem.friends[GetComponent<ItemFriend>().DisplayName.text].PlayfabId}" );
        if (Chat._instance.targetPlayfabid==FriendSystem.friends[GetComponent<ItemFriend>().DisplayName.text].PlayfabId||Chat._instance.targetPlayfabid=="")
        {
        }else
        {
            Chat._instance.clearChat();
        }
        Chat._instance.targetPlayfabid=FriendSystem.friends[displayname.text].PlayfabId;
        
        if (Chat._instance.targetPlayfabid!="")
        {
            Chat._instance.getRegisterChatPrivate();
        }
    }



}
