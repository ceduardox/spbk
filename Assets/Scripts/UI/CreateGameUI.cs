using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;

public class CreateGameUI : MonoBehaviour
{
	public PlayfabManager playfabManagerScript;
	//public InputField lobbyName;
	public Text lobbyName;
	public Dropdown track;
	public Dropdown gameMode;
	public Slider playerCountSlider;
	public Image trackImage;
	public Text playerCountSliderText;
	public Image playerCountIcon;
	public Button confirmButton;
	public TextMeshProUGUI trackName;
    
	//resources
	public Sprite padlockSprite, publicLobbyIcon;

	internal int sessionNumber = 0;
	internal string loadedSessionNumber = "0";
	private int maxSessionNumber = 9;

	private void Start()
	{		
		playerCountSlider.SetValueWithoutNotify(8);
		SetPlayerCount();

		track.ClearOptions();
		track.AddOptions(ResourceManager.Instance.tracksDefinitions.Select(x => TranslateUI.getStringUI(x.trackName)).ToList());
		track.onValueChanged.AddListener(SetTrack);
		SetTrack(0);

		gameMode.ClearOptions();
		gameMode.AddOptions(ResourceManager.Instance.gameTypes.Select(x => x.modeName.ToString()).ToList());
		gameMode.onValueChanged.AddListener(SetGameType);
		SetGameType(0);

		playerCountSlider.wholeNumbers = true;
		playerCountSlider.minValue = 3;
		playerCountSlider.maxValue = 8;
		playerCountSlider.value = 3;		
		playerCountSlider.onValueChanged.AddListener(x => ServerInfo.MaxUsers = (int)x);

		/*
		lobbyName.onValueChanged.AddListener(x =>
		{
			ServerInfo.LobbyName = x;
			confirmButton.interactable = !string.IsNullOrEmpty(x);
		});
		*/
		lobbyName.text = ServerInfo.LobbyName = "neraverse" + Random.Range(1, 100000);

		//ServerInfo.TrackId = track.value;
		ServerInfo.GameMode = gameMode.value;
		ServerInfo.MaxUsers = (int)playerCountSlider.value;
	}

	public void SetGameType(int gameType)
	{
		ServerInfo.GameMode = gameType;
	}

	public void SetTrack(int trackId)
	{
		//ServerInfo.TrackId = trackId;
		trackImage.sprite = ResourceManager.Instance.tracksDefinitions[trackId].trackIcon;
        if (trackId==1) {
			trackName.text = "Sahara Desert";
		}
		else if (trackId == 0)
        {
			trackName.text = "Miami Beach";
		}
	}

	public void SetPlayerCount()
	{
        playerCountSlider.value = ServerInfo.MaxUsers;
        playerCountSliderText.text = $"{ServerInfo.MaxUsers}";
        //playerCountSlider.value = 8;
        //playerCountSliderText.text = "8";
        playerCountIcon.sprite = ServerInfo.MaxUsers > 1 ? publicLobbyIcon : padlockSprite;
	}

	// UI Hooks

    private bool _lobbyIsValid;

	public void ValidateLobby()
	{
		_lobbyIsValid = string.IsNullOrEmpty(ServerInfo.LobbyName) == false;
	}

	public void TryFocusScreen(UIScreen screen)
	{
		if (_lobbyIsValid)
		{
			UIScreen.Focus(screen);
		}
	}

	public void TryCreateLobby(GameLauncher launcher)
	{
		if (_lobbyIsValid)
		{
			launcher.JoinOrCreateLobby();
			_lobbyIsValid = false;
		}
	}

	public void GetSessionNumber()
	{
		//playfabManagerScript.GetPlayerData();
	}

	internal void ChangeSessionNumber(string loadedSessionNumberTemp)
	{		
		//Debug.Log
		sessionNumber = int.Parse(loadedSessionNumberTemp) + 1;
		if(sessionNumber > maxSessionNumber)
		{
			sessionNumber = 1;
		}
		//guardar sessionNumber
		loadedSessionNumber = sessionNumber.ToString();
		//playfabManagerScript.SavePlayerData();
		//ShowSessionNumber();
		lobbyName.text = ServerInfo.LobbyName = "neraverse" + sessionNumber;
	}

	internal void ShowSessionNumber()
	{
	}
}