using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIClanItemExplorar : MonoBehaviour
{
    public TextMeshProUGUI TextNameClan;
    public TextMeshProUGUI TextMiembros;
    public TextMeshProUGUI TextCopas;
    public Button btnSendInvitation;
    void Start()
    {
        
    }

    public void setClanSelected()
    {
        UIClanScreen.instance.TextSelectedClan = TextNameClan.text;
        UIClanScreen.instance.RequestJoinClan(TextNameClan.text);
    }
    public void getInfoClan()
    {
        ClanExternoInfo.instance.setDataClan(TextNameClan.text);
    }
}
