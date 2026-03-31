using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ObjectItemRoom : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI sessionName;
    public TextMeshProUGUI modeGame;
    public TextMeshProUGUI players;
    public TextMeshProUGUI status;
    public TextMeshProUGUI belt;
    public TextMeshProUGUI ping;
    public Image ButtonSelect;

    public Slider currentPlayers;
    public Fusion.SessionInfo session;
    public int bet;
    public bool isOpen;
    public bool borrar;
    Ping pinger;
    //string direccion = "181.30.95.162";
    private void Start()
    {
        //CLog.Log("IONICIO EL OBJECT " + GameLauncher.instance.IP_server);
    }
    private void OnEnable()
    {
        pinger = new Ping(GameLauncher.instance.IP_server.Trim());
        contadorPing = 0; 
        StartCoroutine(pingControl());
        
    }

    public void PlayerBarFiller(int players)
    {
        currentPlayers.value = players;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //if (isOpen) 
        if (ContentListRooms.instance.sessionSelect != null)
        {
            ContentListRooms.instance.sessionSelect.ButtonSelect.color = ContentListRooms.instance.sessionDeactive;
        }

        ContentListRooms.instance.SetCurrentNameRoom(sessionName.text, isOpen, this);
        CLog.Log("CLICKEO EN EL ITEM ROOM ");
    }
    public void setCurrentNameRoom()
    {
        if (ContentListRooms.instance.sessionSelect != null)
        {
            ContentListRooms.instance.sessionSelect.ButtonSelect.color = ContentListRooms.instance.sessionDeactive;
        }

        ContentListRooms.instance.SetCurrentNameRoom(sessionName.text, isOpen, this);
        CLog.Log("CLICKEO EN EL ITEM ROOM 2");
    }

    int contadorPingMax = 10;
    int contadorPing = 0;
    int pingValue = 0;
    float timetoChech = 1;

    IEnumerator pingControl()
    {
        contadorPing = 0;

        while (true)
        {

            //CLog.Log("EL PING ES A: " + pinger.time + " a-" + pinger.ip + "-" + GameLauncher.instance.IP_server+" "+ pinger.isDone);
            if (contadorPing++ < contadorPingMax)
            {
                yield return new WaitForSeconds(.1f);
            }
            else
            {
                yield return new WaitForSeconds(2f);
            }
         //   yield return new WaitForSeconds(.2f);
            if (pinger.isDone)
            {
                pingValue = pinger.time;// / ++contadorPing;
                ping.text = pingValue.ToString();
                //CLog.Log("EL PING ES: " + pinger.time + " a " + pinger.ip + " " + GameLauncher.instance.IP_server); 
                pinger = new Ping(GameLauncher.instance.IP_server.Trim());
                
            }
        }

    }

}
