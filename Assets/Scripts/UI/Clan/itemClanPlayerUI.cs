using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class itemClanPlayerUI : MonoBehaviour
{
    public TextMeshProUGUI NamePlayer;

    public void setName(string name)
    {
        NamePlayer.text = name;
    }
}
