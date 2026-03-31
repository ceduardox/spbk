using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnEnableScrollViewChatPrivate : MonoBehaviour
{
    public ScrollRect scrollview;
    private void Start() {
        Chat._instance.scrollRectPrivate=GetComponent<ScrollRect>();
        Chat._instance.scrollRectPrivate=scrollview;
    }
    private void Awake() {
        Chat._instance.scrollRectPrivate=GetComponent<ScrollRect>();
        Chat._instance.scrollRectPrivate=scrollview;
    }
    private void OnEnable() {
        Chat._instance.scrollRectPrivate=GetComponent<ScrollRect>();
        Chat._instance.scrollRectPrivate=scrollview;
    }
    private void Update() {//revisar
        if (Chat._instance.scrollRectPrivate==null)
        {
            CLog.Log("ESTABLECIENDO SCROOL EN CHAT PRIVATE");
            Chat._instance.scrollRectPrivate=scrollview;
        }
    }
    private void OnDisable() {
        Chat._instance.targetPlayfabid="";
    }
}
