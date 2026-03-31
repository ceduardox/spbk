using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateUI_ITEM : MonoBehaviour
{
    public UI_CODE _code;

    UnityEngine.UI.Text textUI;
    TMPro.TextMeshProUGUI textMPRO;
    private void Start()
    { 
        if (!(textUI = GetComponent<UnityEngine.UI.Text>()))
        {
            textMPRO = GetComponent<TMPro.TextMeshProUGUI>();
        }
        TranslateUI.addItem(this);
        setText();
    }

    private void OnEnable()
    {
    setText();
    }

    public void setText()
    {
        if (textUI != null) textUI.text = TranslateUI.getStringUI(_code);
        else if (textMPRO != null)
            textMPRO.text = TranslateUI.getStringUI(_code);
    }
}
