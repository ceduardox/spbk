using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectPanelRoom : MonoBehaviour
{
    public static ObjectPanelRoom instance;

    public TextMeshProUGUI trackid;
    public TextMeshProUGUI laps;
    public TextMeshProUGUI maxlaps;
    public TextMeshProUGUI bet;
    public TextMeshProUGUI players;
    public TextMeshProUGUI status;
    public UnityEngine.UI.Image trackIcon;
    public Sprite MapImageDefault;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public void setDetails(string _trackid, string _laps, string _maxlaps, string _bet, string _players, string _status)
    {
        CLog.Log("Set value: " + _trackid + " - " + _laps);
        trackid.text = _trackid;
        laps.text = _laps+" / "+_maxlaps;
        maxlaps.text = _maxlaps;
        bet.text = _bet;
        players.text = _players;
        status.text = _status;
        if (!_trackid.Contains("-"))
        {
            trackIcon.sprite = ResourceManager.Instance.tracksDefinitions[int.Parse(_trackid)].trackIcon;
        }
        else
        {
            trackIcon.sprite = MapImageDefault;
        }
        //else trackIcon.sprite = null;
    }

    [ContextMenu("Reinciar la Sala seccionada")]
    public void Reiniciar()
    {
        setDetails("-", "-", "-", "-", "- / -", "-");
        ContentListRooms.instance.RoomSelectedObjetButton = "";
        ContentListRooms.instance.RoomSelectedObjet = "";

        if (ContentListRooms.instance.sessionSelect != null)
        {
            ContentListRooms.instance.sessionSelect.ButtonSelect.color = ContentListRooms.instance.sessionDeactive;
        }

        ContentListRooms.instance.sessionSelect = null;

    }
}
