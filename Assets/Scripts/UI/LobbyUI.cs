using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUI : MasterScreen, IDisabledUI
{
	public GameObject textPrefab;
	public Transform parent;
	public Button readyUp;
	public Button customizeButton;
	public TextMeshProUGUI betText;
	public TextMeshProUGUI trackNameText;
	public TextMeshProUGUI descTrackText;
	public TextMeshProUGUI modeNameText;
	public TextMeshProUGUI lobbyNameText;
	public TextMeshProUGUI lobbyLapsText;

	public TextMeshProUGUI lobbyPlayerNameRecord;
	public TextMeshProUGUI lobbyPlayerRecord;
	//public Text trackNameText;
	//public Text modeNameText;
	//public Text lobbyNameText;
	public Dropdown trackNameDropdown;
	public Dropdown gameTypeDropdown;
	public Image trackIconImage;
	public Button btnCustomizeKart;
	public Button btnCustomizeChar;
	public Button btnCustomizePowerUp;
	public Button btnOptions;
	public Button btnReady;
	public GameObject btnBack;
     

	public Text chat;
	public Button buttonSend;
	public GameObject buttonChangeTrack;

	public GameObject counterTimeObj;
	public static LobbyUI _instance;
	public static readonly Dictionary<RoomPlayer, LobbyItemUI> ListItems = new Dictionary<RoomPlayer, LobbyItemUI>();
	private static bool IsSubscribed;
	private bool _suppressDropdownCallbacks;
	
	private void Awake()
	{
		CLog.Log("**Lobby " + GameLauncher.ConnectionStatus + "-"+ _instance+"-"+(_instance==null));
		if (_instance!=null)
		{
			Destroy(gameObject);
			return;
		}
		_instance = this;
		trackNameDropdown.onValueChanged.AddListener(x =>
		{
			if (_suppressDropdownCallbacks)
				return;
			var gm = GameManager.Instance;
			if (gm != null) gm.TrackId = x;
		});
		gameTypeDropdown.onValueChanged.AddListener(x =>
		{
			if (_suppressDropdownCallbacks)
				return;
			var gm = GameManager.Instance;
			if (gm != null) gm.GameTypeId = x;
		});

		GameManager.OnLobbyDetailsUpdated += UpdateDetails;

		RoomPlayer.PlayerChanged -= HandlePlayerChanged;
		RoomPlayer.PlayerChanged += HandlePlayerChanged;


		btnCustomizeChar.gameObject.SetActive(!GameLauncher.instance.modeServerDedicado);
		btnCustomizeKart.gameObject.SetActive(!GameLauncher.instance.modeServerDedicado);
		btnCustomizePowerUp.gameObject.SetActive(!GameLauncher.instance.modeServerDedicado);
		btnOptions.gameObject.SetActive(!GameLauncher.instance.modeServerDedicado);
		buttonChangeTrack.SetActive(GameLauncher.instance.modeServerDedicado);
		btnReady.gameObject.SetActive(!GameLauncher.instance.modeServerDedicado);
		btnBack.SetActive(!GameLauncher.instance.modeServerDedicado);


	}

	bool limitOpenNet = true;
	private void Update()
	{
		if (TimerCounterScreen._instance)
		{
			if (limitOpenNet)
			{
				limitOpenNet = btnOptions.interactable=btnCustomizeChar.interactable = btnCustomizePowerUp.interactable = btnCustomizeKart.interactable = TimerCounterScreen._instance.isCustomize();

				if (!limitOpenNet)
				{

					//----
					if (GameLauncher.instance.isServer ||
						(RoomPlayer.Local != null && RoomPlayer.Local.IsLeader))
					{

						GameLauncher.instance.setOpenNet(false);
					}


					//SpotlightGroup.Search("Kart Display", out SpotlightGroup spotlight2);
					//spotlight2.converConfig();
					if (RoomPlayer.Local)
						RoomPlayer.Local.checkSession();

					SpotlightGroup._instance.converConfig();

				}
			}

			btnReady.interactable = TimerCounterScreen._instance.isReadyEnabled();
		}

			if (GameLauncher.instance &&
				GameLauncher.ConnectionStatus == ConnectionStatus.Disconnected)
			{
				if (verificarConnection)
				{
					CLog.Log("REGRESAR AL HOME " + GameLauncher.ConnectionStatus+" "+ GameLauncher.ConnectionStatus);
					GameLauncher.instance.LeaveSession();
				}
			}

		}
	bool verificarConnection = true;
	int verificarIdTrack = -1;
	private void OnEnable()
	{
		resetButtons();

		verificarIdTrack = -1;

		if (TimerCounterScreen._instance)
		{
			TimerCounterScreen._instance.reset();
			TimerCounterScreen._instance.active();
		}

		if (GameLauncher.ConnectionStatus == ConnectionStatus.Disconnected||
			GameLauncher.ConnectionStatus == ConnectionStatus.Failed)
        {
			FocusScreen(GameLauncher.instance.mainScreen.GetComponent<MasterScreen>());
			return;

		}



		if (GameLauncher.instance.isServer ||
			(RoomPlayer.Local &&
			RoomPlayer.Local.IsLeader))
		{
			

			GameLauncher.instance.setSessionProperties(SessionPropertyKey.Laps, GameManager.Instance.TrackId);
			GameLauncher.instance.setSessionProperties(SessionPropertyKey.MaxLaps, GameManager.Instance.GameType.lapCount);
			GameLauncher.instance.setSessionProperties(SessionPropertyKey.Laps, 0);
			GameLauncher.instance.setOpenNet(true);
		}

		//if (!GameLauncher.instance.isServer)
		{
			foreach (RoomPlayer _player in RoomPlayer.Players)
				_player.startConfig(false);
		}
		iniciarContador();
		CLog.Log("Copunter Inizialized on Enable screen");
		if (!GameLauncher.instance.isServer)
			actualizarBestLap();
	}

	public void resetButtons()
	{
		verificarConnection = limitOpenNet = btnReady.interactable = btnOptions.interactable = btnCustomizeKart.interactable = btnCustomizeChar.interactable = btnCustomizePowerUp.interactable = true;
	}

	void actualizarBestLap()
	{
		if (GameManager.Instance == null ||
			!GameManager.Instance.Object.IsValid)
		{
			Invoke("actualizarBestLap", .5f);
		}
		else
		{
			ScreenManager.lobbyUI.gameObject.SetActive(true);
			ScreenManager.mainUIScreen.gameObject.SetActive(false);
			StartCoroutine(GameManager.Instance.records(ResourceManager.Instance.tracksDefinitions[GameManager.Instance.TrackId].trackSceneName, 0, false));
		}
	}

	void UpdateDetails(GameManager manager)
	{
		if (trackNameDropdown == null || gameTypeDropdown == null)
			return;
		if (ResourceManager.Instance == null || GameManager.Instance == null)
			return;

		//CLog.Log(manager.LobbyName) ;
		lobbyNameText.text = manager.LobbyName;
		/*
		if (GameLauncher.instance.isServer)
		{ lobbyNameText.text = "Room Code: " + manager.LobbyName;
				}
		else
		{
			//ContentListRooms.instance.sessionSelect
		}*/
		trackNameText.text = manager.TrackName;
		descTrackText.text = manager.TrackDesc;
		lobbyLapsText.text = manager.GameType.lapCount.ToString();
		modeNameText.text = GameManager.Instance.Bet > 0 ? "Competitivo " : "Aventura";
		betText.text = GameManager.Instance.Bet.ToString();

		var tracks = ResourceManager.Instance.tracksDefinitions;
		var trackOptions = tracks.Select(x => TranslateUI.getStringUI(x.trackName)).ToList();

		var gameTypes = ResourceManager.Instance.gameTypes;
		var gameTypeOptions = gameTypes.Select(x => x.modeName.ToString()).ToList();

		_suppressDropdownCallbacks = true;
		try
		{
			trackNameDropdown.ClearOptions();
			trackNameDropdown.AddOptions(trackOptions);
			trackNameDropdown.value = GameManager.Instance.TrackId;

			if (trackIconImage)
			{ 
			trackIconImage.sprite = ResourceManager.Instance.tracksDefinitions[GameManager.Instance.TrackId].trackIcon;
			}

			gameTypeDropdown.ClearOptions();
			gameTypeDropdown.AddOptions(gameTypeOptions);
			gameTypeDropdown.value = GameManager.Instance.GameTypeId;
		}
		finally
		{
			_suppressDropdownCallbacks = false;
		}




	}

	public void changeTrack()
    {
		GameManager.Instance.TrackId = ResourceManager.instance.getTrackID(GameManager.Instance.GameType.modeName);
		GameLauncher.instance.setSessionProperties(SessionPropertyKey.TrackId, GameManager.Instance.TrackId);

	}

	public void Setup()
	{
		if (IsSubscribed) return;

		RoomPlayer.PlayerJoined += AddPlayer;
		RoomPlayer.PlayerLeft += RemovePlayer;

		RoomPlayer.PlayerChanged += EnsureAllPlayersReady;

		readyUp.onClick.AddListener(ReadyUpListener);

		IsSubscribed = true;
	}

	private void OnDestroy()
	{
		GameManager.OnLobbyDetailsUpdated -= UpdateDetails;
		RoomPlayer.PlayerChanged -= HandlePlayerChanged;
		if (_instance == this)
			_instance = null;

		if (!IsSubscribed) return;

		RoomPlayer.PlayerJoined -= AddPlayer;
		RoomPlayer.PlayerLeft -= RemovePlayer;
		RoomPlayer.PlayerChanged -= EnsureAllPlayersReady;

		readyUp.onClick.RemoveListener(ReadyUpListener);

		IsSubscribed = false;
	}

	private void HandlePlayerChanged(RoomPlayer player)
	{
		if (!this || !gameObject)
			return;

		var local = RoomPlayer.Local;
		var isLeader = local ? local.IsLeader : false;

		if (trackNameDropdown != null)
			trackNameDropdown.interactable = isLeader;
		if (gameTypeDropdown != null)
			gameTypeDropdown.interactable = isLeader;

		CLog.Log("**Lobby " + GameLauncher.ConnectionStatus + " ");

		bool canCustomize;
		if (TimerCounterScreen._instance == null)
			canCustomize = local ? !local.IsReady : false;
		else
			canCustomize = TimerCounterScreen._instance.isCustomize();

		if (btnOptions != null) btnOptions.interactable = canCustomize;
		if (btnCustomizePowerUp != null) btnCustomizePowerUp.interactable = canCustomize;
		if (btnCustomizeKart != null) btnCustomizeKart.interactable = canCustomize;
		if (btnCustomizeChar != null) btnCustomizeChar.interactable = canCustomize;
	}

	/// <summary>
	/// Agrego el Item a la lista de players
	/// </summary>
	/// <param name="player"></param>
	private void AddPlayer(RoomPlayer player)
	{
		if (ListItems.ContainsKey(player))
		{
			var toRemove = ListItems[player];
			Destroy(toRemove.gameObject);

			ListItems.Remove(player);
		}

		var obj = Instantiate(textPrefab, parent).GetComponent<LobbyItemUI>();
		obj.SetPlayer(player);
		
		ListItems.Add(player, obj);

		UpdateDetails(GameManager.Instance);

		//obj
		/*for (int i=0; i< ListItems.Count;i++)
        {
			CLog.Log("ESTO CONTIENE: "+ ListItems[player].);
        }*/
		
		
	}

	private void RemovePlayer(RoomPlayer player)
	{
		if (!ListItems.ContainsKey(player))
			return;

		var obj = ListItems[player];
		if (obj != null)
		{
			Destroy(obj.gameObject);
			ListItems.Remove(player);
		}
	}

	public void OnDestruction()
	{
	}

	private void ReadyUpListener()
	{
		var local = RoomPlayer.Local;
        if (local && local.Object && local.Object.IsValid)
        {
            local.RPC_ChangeReadyState(!local.IsReady);
        }
	}

	private void EnsureAllPlayersReady(RoomPlayer lobbyPlayer)
	{

		if (RoomPlayer.Local&&
			!RoomPlayer.Local.IsLeader) 
			return;

		//Todo los player son ready
		if (IsAllReady())
		{
			forceStart();
		}
	}

    public void disconnected()
    {
		if(TimerCounterScreen._instance)
			TimerCounterScreen._instance.reset();
		CLog.Log("CERRANDO SESION");
		GameLauncher.instance.LeaveSession();
		pararContador();
    }

    private static bool IsAllReady() => RoomPlayer.Players.Count > 0 && RoomPlayer.Players.All(player => player.IsReady);

	public void iniciarContador()
    {
		counterTimeObj.SetActive(true);
    }
	public void forceStart()
    {
		counterTimeObj.GetComponent<TimerCounterScreen>().forceTime();
	}
	public void pararContador()
	{
		counterTimeObj.SetActive(false);
	} 

	public void iniciarCarrera()
    {
		Debug.Log("CARGANDO LA NUEVA ESCENA " + ResourceManager.Instance.tracksDefinitions[GameManager.Instance.TrackId].trackSceneName);
		///---int scene = ResourceManager.Instance.tracksDefinitions[GameManager.Instance.TrackId].buildIndex;
		///---LevelManager.LoadTrack(scene);
		GameManager.Instance.playersInLastRace = RoomPlayer.Players.Count;
		List<string> namePlayers = new List<string>();
		foreach (RoomPlayer rp in RoomPlayer.Players)
			namePlayers.Add(rp.Username);
		
		pararContador();

		//LevelManager.LoadTrack(LevelManager.TRACK_SCENE);
		LevelManager.LoadTrack(ResourceManager.Instance.tracksDefinitions[GameManager.Instance.TrackId].trackSceneName);
        


        
		Busines.UpdateEstadisticas(	ResourceManager.Instance.tracksDefinitions[GameManager.Instance.TrackId].trackSceneName, 
									GameManager.Instance.GameType.modeName.ToString() + "_" + (GameLauncher.instance.serverBet?"Apuesta-":"Free-"));
        

        if (GameLauncher.instance.modeServerDedicado)
		{
			if (GameLauncher.instance.serverBet)
			GameManager.Instance.setTransaccion(namePlayers);
			//gameObject.SetActive(false);
		}


	}




}
