using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerResultItem : MonoBehaviour
{
	//public Text placementText;
	//public Text nameText;
	//public Text timeText;

    public Image iconPlayer;
    public Image iconKartPlayer;
    public TextMeshProUGUI placeText;
    public TextMeshProUGUI displayNameText;
    public TextMeshProUGUI timerText;

    public void SetResult(RoomPlayer _player, float time, int place)
    {
        placeText.text = 
            place == 1 ? "1" :
            place == 2 ? "2" :
            place == 3 ? "3" :
            $"{place}";
        iconPlayer.sprite = ResourceManager.instance.getChar(_player.CharId).iconLobby;
        iconKartPlayer.sprite = ResourceManager.instance.getKart(_player.KartId).iconLobby;

        displayNameText.text = _player.Username;
        timerText.text = $"{(int)(time / 60):00}:{time % 60:00.000}";
    }
}
