using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HoverTipManager : MonoBehaviour
{
    public TextMeshProUGUI tipText;
    public RectTransform windowTip;
    //public GameObject windowTip;

    public static System.Action<string, Vector2> onMouseHover;
    public static System.Action onMouseOut;
    private void OnEnable()
    {
        onMouseHover += showTip;
        onMouseOut += HideTip;
    }
    private void OnDisable()
    {
        onMouseHover -= showTip;
        onMouseOut -= HideTip;
    }
    private void Start()
    {
        HideTip();
    }
    // Update is called once per frame
    private void showTip(string tip, Vector2 mousePos)
    {
        tipText.text = tip;
        windowTip.sizeDelta = new Vector2(tipText.preferredWidth > 350 ? 350 : tipText.preferredWidth, tipText.preferredHeight);

        windowTip.gameObject.SetActive(true);
        //windowTip.transform.position = new Vector2(mousePos.x + windowTip.GetComponent<RectTransform>().sizeDelta.x * 2, mousePos.y);
        windowTip.transform.position = mousePos + new Vector2(-20f,-10f);
    }
    private void HideTip()
    {
        tipText.text = default;
        windowTip.gameObject.SetActive(false);
    }
}
