using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SetChatPrivate : MonoBehaviour
{
    public Transform chatContainerPrivate;
    public ScrollRect scrollRectPrivate;
    public TMP_InputField chatInputPrivate;
    // Start is called before the first frame update
    private void OnEnable()
    {
        CLog.Log("HABILITANDO LA UI DE FIRENDS");
        Chat._instance.chatContainerPrivate = chatContainerPrivate;
        Chat._instance.scrollRectPrivate = scrollRectPrivate;
        Chat._instance.chatInputPrivate = chatInputPrivate;
    }

    public void SendChatMessagePrivate()
    {
        CLog.Log("ENVIANDO MENSAJE DEL CHAT PRIVADO");
        Chat._instance.SendChatMessagePrivate();

    }
    //private void Update()
    //{
    //    if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter/* && chatInputPrivate.text != ""*/)
    //    {
    //        SendChatMessagePrivate();
    //        //chatInputPrivate.Select();
    //        //chatInputPrivate.ActivateInputField();
    //    }
    //}
    void OnGUI()
    {
        //if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter && chatInputPrivate.text != "")
        //{
        //    SendChatMessagePrivate();
        //    chatInputPrivate.Select();
        //    chatInputPrivate.ActivateInputField();
        //}
        if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter/* && chatInputPrivate.text != ""*/)
        {
            SendChatMessagePrivate();
            //chatInputPrivate.Select();
            //chatInputPrivate.ActivateInputField();
        }
    }
}
