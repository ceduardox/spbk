using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyItemUI : MonoBehaviour {

    public TextMeshProUGUI username;
    //public Text username;
    public Image ready;
    public Image leader;
    public Image kartSelect;
    public Image charSelect;

    private RoomPlayer _player;
    int lastKartID=0;
    int lastCharID=0;
    public GameObject addFriend;
    public void SetPlayer(RoomPlayer player) {
        _player = player;
        //ListPlayersPref.instance.newPlayer(player.Username);
        // kartSelect.sprite = ResourceManager.Instance.getKart(player.KartId).itemIcon;
        //charSelect.sprite = ResourceManager.Instance.getChar(player.CharId).itemIcon;
        kartSelect.sprite = null;
        //CLog.Log("")
        }
    
	private void Update() {
        if (_player == null)
            return;
		if (_player.Object != null && _player.Object.IsValid) 
		{
            username.text = _player.Username;
            ready.gameObject.SetActive(_player.IsReady);
            

            if (_player.KartId!=0 && _player.CharId!=0)
            {
                //CLog.Log("PlayfabManager.instance.displayName" + PlayfabManager.instance.displayName + "- " + _player.Username);
                addFriend.SetActive(!_player.Username.Equals(PlayfabManager.instance.displayName));

                if (_player.KartId != lastKartID || _player.CharId != lastCharID)
                {
                    
                    // CLog.Log("DATA PLAYERROOM: "+ _player.Username + " - " + _player.Kart + " - " + _player.CharId+" - ");
                    kartSelect.sprite = ResourceManager.Instance.getKart(_player.KartId).iconLobby;
                    charSelect.sprite = ResourceManager.Instance.getChar(_player.CharId).iconLobby;
                    lastKartID = _player.KartId;
                    lastCharID = _player.CharId;
                    kartSelect.gameObject.SetActive(true);
                    charSelect.gameObject.SetActive(true);
                }
            }

        }
    }
    public void addUser()
    {
        PlayfabManager.instance.addFriends(username.text);
        addFriend.SetActive(false);
        //FriendSystem.AddFriend(PlayfabManager.instance.displayName);
    }

    public void showPopUp()
    {

    }
}
