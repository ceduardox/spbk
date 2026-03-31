using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ObjectListText : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI sessionName;
    public TextMeshProUGUI players;
    public TextMeshProUGUI maxPlayers;
    public TextMeshProUGUI status;
    public bool isOpen;
    //public ObjectListRoom script;
    public Image[] playerPoint;
    public Image playerBar;
    float player, maxPlayer = 8;

    // Start is called before the first frame update
    // Start is called before the first frame update
    void Start()
    {
        player = maxPlayer;
        PlayerBarFiller();
        this.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (player > maxPlayer) player = maxPlayer;
        //PlayerBarFiller();
    }
    void PlayerBarFiller()
    {
        int total = int.Parse((players.text).ToString());
        //playerBar.fillAmount = player / maxPlayer;
        for (int i = 0; i < maxPlayer; i++)
        {
            bool xyz = !DisplayPlayerPoint(total, i);
            if (i < total)
            {
                playerPoint[i].enabled = true;
            }
            else
            {
                playerPoint[i].enabled = false;
            }
        }
    }
    bool DisplayPlayerPoint(float _player, int pointNumer)
    {
        return ((pointNumer * 10) >= _player);
    }
    public void RemovePoint(float point)
    {
        if (player > 0)
        {
            player -= point;
        }
    }
    public void AddPoint(float point)
    {
        if (player < maxPlayer)
        {
            player += point;
        }
    }


    Coroutine popUpCoroutine;
    public void OnPointerClick(PointerEventData pointerEventData)
    {

        //Use this to tell when the user right-clicks on the Button
        if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
            CLog.Log(sessionName + " Game Object Right Clicked!");
        }

        //Use this to tell when the user left-clicks on the Button
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            CLog.Log(sessionName + " Game Object Left Clicked! ");

        }

        CLog.Log(sessionName + " Is Open: "+isOpen);

        //if (popUpCoroutine==null)
        //  popUpCoroutine=StartCoroutine(consultarSaldo(this));
        if (isOpen) 
            ObjectListRoom.instance.SetCurrentNameRoom(sessionName.text);
        else
            ObjectListRoom.instance.SetCurrentNameRoom("");

        //        StartCoroutine(GameLauncher.instance.consultarSaldo(this,false));

    }






    /*
    StartCoroutine(saldo((myReturnValue) => {
        if (myReturnValue)
        {

        }
        else
        {

        }

    }));

    IEnumerator saldo(System.Action<bool> callback)
    {
        yield return null;
        callback(true);
    }

    */
}
