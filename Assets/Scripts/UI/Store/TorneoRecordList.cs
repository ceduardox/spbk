using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TorneoRecordList : MonoBehaviour
{
    public string idEvent;
    public void setDataEvent()
    {
        TorneosScreen.instance.setDataRecordEvent(gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text, idEvent);
    }
}
