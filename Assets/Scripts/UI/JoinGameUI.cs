using UnityEngine;
using UnityEngine.UI;

public class JoinGameUI : MasterScreen 
{
    
    public InputField lobbyName;
	public Button confirmButton;

	//testr()=> ObjectListRoom.instance.RoomSelectedObjet!=null;
	


	private void OnEnable()
	{
		if(lobbyName) SetLobbyName(lobbyName.text);///////////////////////////////////////////////////////lobbyName.text es NULL en los clientes
		//confirmButton.interactable = false;
	}

    private void LateUpdate()
    {
		//confirmButton.interactable = ObjectListRoom.instance.RoomSelectedObjetButton != "";
		confirmButton.interactable = ContentListRooms.instance.RoomSelectedObjetButton != ""&& ContentListRooms.instance.sessionSelect!=null&&
										ContentListRooms.instance.sessionSelect.isOpen;

	}
    private void Start() {
		if (lobbyName)
		{
			lobbyName.onValueChanged.AddListener(SetLobbyName);/////////////////////////////////////////////////Trae null para los clientes	
			lobbyName.text = ClientInfo.LobbyName;
		}
    }

    private void SetLobbyName(string lobby)
	{
		ClientInfo.LobbyName = lobby;
		//confirmButton.interactable = !string.IsNullOrEmpty(lobby);
	}
    private void OnDisable()
    {
		//ObjectListRoom.instance.removeItems();
		ContentListRooms.instance.removeItems();
    }
}
