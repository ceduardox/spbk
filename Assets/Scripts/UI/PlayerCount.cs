using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCount : MonoBehaviour
{
    //public Text playerText;
    public Image playerBar;
    public Image[] playerPoint;
    float player, maxPlayer = 8;
    void Start()
    {
        player = maxPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        //playerText.text = "Player: " + player + "%";
        if (player > maxPlayer) player = maxPlayer;
        PlayerBarFiller();
    }
    bool DisplayPlayerPoint(float _player, int pointNumer)
    {
        return ((pointNumer * 10) >= _player);
    }
    void PlayerBarFiller()
    {
        playerBar.fillAmount = player / maxPlayer;
        for(int i=0; i<playerPoint.Length; i++)
        {
            playerPoint[i].enabled = !DisplayPlayerPoint(player, i);
        }
    }
    public void RemovePoint(float point)
    {
        if(player > 0)
        {
            player -= point;
        }
    }
    public void AddPoint(float point)
    {
        if(player < maxPlayer)
        {
            player += point;
        }
    }
}
