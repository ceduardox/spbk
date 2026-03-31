using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OnEnableContentChatPrivate : MonoBehaviour
{
    public RectTransform content;
    private void Start() {
        Chat._instance.chatContainerPrivate=GetComponent<RectTransform>();
        Chat._instance.chatContainerPrivate=content;
    }
    private void Awake() {
        Chat._instance.chatContainerPrivate=GetComponent<RectTransform>();
        Chat._instance.chatContainerPrivate=content;
    }
    private void OnEnable() {
        Chat._instance.chatContainerPrivate=GetComponent<RectTransform>();
        Chat._instance.chatContainerPrivate=content;
    }
    private void Update() {//revisar
        if (Chat._instance.chatContainerPrivate==null)
        {
            CLog.Log("ESTABLECIENDO CONTENT EN CHAT PRIVATE");
            Chat._instance.chatContainerPrivate=content;
        }
    }
    private void OnDisable() {
        Chat._instance.targetPlayfabid="";
    }
}
