using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class OnEnableInputChatPrivate : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_InputField inputfield;
    private void Start() {
        Chat._instance.chatInputPrivate=GetComponent<TMP_InputField>();
        Chat._instance.chatInputPrivate=inputfield;
    }
    private void Awake() {
        Chat._instance.chatInputPrivate=GetComponent<TMP_InputField>();
        Chat._instance.chatInputPrivate=inputfield;
    }
    private void OnEnable() {
        Chat._instance.chatInputPrivate=GetComponent<TMP_InputField>();
        Chat._instance.chatInputPrivate=inputfield;
    }
    private void Update() {//revisar
        if (Chat._instance.chatInputPrivate==null)
        {
            CLog.Log("ESTABLECIENDO INPUTFIELD EN CHAT PRIVATE");
            Chat._instance.chatInputPrivate=inputfield;
        }
    }
    private void OnDisable() {
        Chat._instance.targetPlayfabid="";
    }
}
