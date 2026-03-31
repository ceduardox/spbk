using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIClanItemMiClan : MonoBehaviour
{
    public TextMeshProUGUI TextPositionMember;
    public TextMeshProUGUI TextNamePlayer;
    public TextMeshProUGUI TextRangePlayer;
    public TextMeshProUGUI TextCupsPlayer;
    public TextMeshProUGUI TextCupsTeamPlayer;

    public void showPopUpOptions()
    {
        //UIClanScreen.instance.currentDisplayName = TextNamePlayer.text.ToString();
        //UIClanScreen.instance.DisplayTittle.text = TextNamePlayer.text.ToString();
        UIClanScreen.instance.showPopUpOptions(TextNamePlayer.text.ToString());
    }
}
