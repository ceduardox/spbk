//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Fusion;
//using Fusion.Sockets;
//using FusionExamples.FusionHelpers;
//using Managers;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public enum ConnectionStatus
//{
//	Disconnected,
//	Connecting,
//	Failed,
//	Connected
//}

//public enum SessionPropertyKey
//{
//	Bet,
//	TrackId,
//	Laps,
//	MaxLaps,
//	Mode,
//}



//[RequireComponent(typeof(LevelManager))]
//public class test : MonoBehaviour, INetworkRunnerCallbacks
//{


//	[SerializeField] private GameManager _gameManagerPrefab;
//	[SerializeField] private RoomPlayer _roomPlayerPrefab;
//	[SerializeField] private DisconnectUI _disconnectUI;
//	[SerializeField] private ObjectListRoom _listRooms;
//	//[SerializeField] private ContentListRooms _contentRooms;
//	[SerializeField] private LobbyUI _lobbyUI;


//	public static ConnectionStatus ConnectionStatus = ConnectionStatus.Disconnected;

//	public static test instance;

//	private GameMode _gameMode;
//	private NetworkRunner _runner;
//	private FusionObjectPoolRoot _pool;
//	private LevelManager _levelManager;
//	public UIScreen mainScreen;
//	public UIScreen gameModesScreen;
//	public PopUpManager popUp;
//	public bool modeServer;
//	internal bool isServer;

//	[SerializeField] private List<itemList> ListaSalas;

//	private void Awake()
//	{
//		if (instance)
//		{
//			Destroy(this);
//			return;
//		}
//		CLog.Log("CREE LA INSTANCE");
//		instance = this;
//		DontDestroyOnLoad(gameObject);
//	}

//	private void Start()
//	{
//		Application.runInBackground = true;
//		Application.targetFrameRate = Screen.currentResolution.refreshRate;
//		QualitySettings.vSyncCount = 1;
//		_levelManager = GetComponent<LevelManager>();
//		SceneManager.LoadScene(LevelManager.LOBBY_SCENE);
//		popUp.starPopUp();
//	}

//	public NetworkRunner getRunner()
//	{
//		return _runner;
//	}
//	public void setOpenNet(bool _value)
//	{
//		//return;
//		if (_runner != null)
//		{
//			_runner.SessionInfo.IsOpen = _value;
//			//	if(_value)
//			//_runner.CurrentScene
//		}

//		//_runner.sce

//	}
//	public void SetCreateLobby() => _gameMode = modeServer ? GameMode.Server : GameMode.Host;//-GameMode.Host;
//	public void SetJoinLobby() => _gameMode = GameMode.Client;

//	//Se llama esta funcion desde el boton Join para comenzar la conexion y descarga de la lista
//	public void SetJoinLobby2()
//	{
//		_ = JoinLobby();
//	}

//	public void JoinOrCreateLobby()
//	{
//		StartCoroutine(consultarSaldo(true));
//	}

//	public void JoinOrCreateLobbyOk()
//	{

//		SetConnectionStatus(ConnectionStatus.Connecting);

//		if (_runner != null)
//			LeaveSession();

//		GameObject go = new GameObject("Session");
//		DontDestroyOnLoad(go);

//		_runner = go.AddComponent<NetworkRunner>();

//		if (modeServer)
//			_runner.ProvideInput = false;
//		else _runner.ProvideInput = _gameMode != GameMode.Server;

//		_runner.AddCallbacks(this);


//		_pool = go.AddComponent<FusionObjectPoolRoot>();



//		setSessionProperties(SessionPropertyKey.Bet, 50);
//		setSessionProperties(SessionPropertyKey.TrackId, -1);
//		setSessionProperties(SessionPropertyKey.Laps, -1);
//		setSessionProperties(SessionPropertyKey.MaxLaps, -1);




//		CLog.Log($"Created gameobject {go.name} - starting game " + _pool + " " + _pool.name);
//		_runner.StartGame(new StartGameArgs
//		{
//			GameMode = _gameMode,
//			//-SessionName = _gameMode == GameMode.Host ? ServerInfo.LobbyName : ClientInfo.LobbyName,
//			SessionName = "ServerDedicated" + UnityEngine.Random.Range(1000, 10000),
//			//SessionName = _gameMode == GameMode.Host ? ServerInfo.LobbyName : ObjectListRoom.instance.RoomSelectedObjet,
//			ObjectPool = _pool,
//			SceneObjectProvider = _levelManager,
//			PlayerCount = ServerInfo.MaxUsers,
//			CustomLobbyName = "MyCustomLobby",
//			DisableClientSessionCreation = true,
//			SessionProperties = sessionProperties,

//		});
//		//_runner.SessionInfo.pr
//		//CLog.Log("Crear Server: " + _runner.IsServer + " " + _runner.IsClient + " " + _runner.IsPlayer);


//		//_runner.SessionInfo.p = "PATITAS"; ;
//		//_runner.SessionInfo. = "PATITAS"; ;
//		// target as NetworkRunner;
//		// _ = JoinLobby(_runner);
//		//_runner.SessionInfo.IsOpen = false;
//		//		StartCoroutine(time());
//	}

//	Dictionary<string, SessionProperty> sessionProperties = new Dictionary<string, SessionProperty>();
//	public void setSessionProperties(SessionPropertyKey _key, SessionProperty value)
//	{
//		if (sessionProperties.ContainsKey(_key.ToString()))
//		{
//			CLog.Log("Modifico valor " + _key + " - " + value.ToString());
//			sessionProperties[_key.ToString()] = value;
//		}
//		else
//		{
//			CLog.Log("Agrego valor " + _key + " - " + value.ToString());
//			sessionProperties.Add(_key.ToString(), value);
//		}

//		if (ConnectionStatus == ConnectionStatus.Connected)
//		{
//			_runner.SessionInfo.UpdateCustomProperties(sessionProperties);

//		}

//		//prop["bet"] = (int)10;
//	}

//	private void SetConnectionStatus(ConnectionStatus status)
//	{
//		CLog.Log($"Setting connection status to {status}");

//		ConnectionStatus = status;


//		if (!Application.isPlaying)
//			return;

//		if (status == ConnectionStatus.Disconnected || status == ConnectionStatus.Failed)
//		{
//			SceneManager.LoadScene(LevelManager.LOBBY_SCENE);
//			//----UIScreen.BackToInitial();
//			mainScreen.BackTo(mainScreen);
//		}
//	}

//	public void LeaveSession()
//	{
//		//CLog.Log("CONEXION VALE: " + ConnectionStatus);
//		if (_runner != null)
//		{
//			_runner.Shutdown();
//			_lobbyUI.pararContador();
//			_runner = null;
//		}
//		else
//		{
//			if (ConnectionStatus == ConnectionStatus.Disconnected)
//				mainScreen.BackTo(gameModesScreen);
//			else
//				SetConnectionStatus(ConnectionStatus.Disconnected);

//		}

//		PlayfabClientCurrency.addCurrency();
//		//TELPoints.instance.ChangeTel(+1); //revisar si aquí funciona de manera correcta y no se puede hacer la trampa de TELS infinitos
//	}

//	public void OnConnectedToServer(NetworkRunner runner)
//	{
//		CLog.Log("Connected to server");
//		SetConnectionStatus(ConnectionStatus.Connected);
//	}
//	public void OnDisconnectedFromServer(NetworkRunner runner)
//	{
//		CLog.Log("Disconnected from server");
//		LeaveSession();
//		SetConnectionStatus(ConnectionStatus.Disconnected);
//	}
//	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
//	{
//		if (runner.CurrentScene > 0 && false)
//		{
//			CLog.LogWarning($"Refused connection requested by {request.RemoteAddress}");
//			request.Refuse();
//		}
//		else
//			request.Accept();
//	}
//	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
//	{

//		CLog.Log($"Connect failed {reason}");
//		LeaveSession();
//		SetConnectionStatus(ConnectionStatus.Failed);
//		(string status, string message) = ConnectFailedReasonToHuman(reason);
//		_disconnectUI.ShowMessage(status, message);
//		CLog.Log($"Connect failed {status} - {message}");

//	}
//	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
//	{
//		CLog.Log($"Player {player} Joined!" + _runner.IsServer + " " + runner.IsServer);
//		if (runner.IsServer)
//		{
//			if (_gameMode == GameMode.Server || _gameMode == GameMode.Host)//----if(_gameMode==GameMode.HOST)
//				runner.Spawn(_gameManagerPrefab, Vector3.zero, Quaternion.identity);

//			var roomPlayer = runner.Spawn(_roomPlayerPrefab, Vector3.zero, Quaternion.identity, player);
//			roomPlayer.GameState = RoomPlayer.EGameState.Lobby;
//		}

//		if (isServer = (runner.IsServer && _gameMode == GameMode.Server))
//		{
//			_lobbyUI.iniciarContador();
//			//if (gameObject.GetComponent<EndRaceUI>() == null)
//			//gameObject.AddComponent<EndRaceUI>();
//		}
//		SetConnectionStatus(ConnectionStatus.Connected);

//		//runner.p
//	}

//	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
//	{
//		CLog.Log($"{player.PlayerId} disconnected.");

//		if (RoomPlayer.RemovePlayer(runner, player) == 0)
//			LevelManager.LoadMenu();


//		SetConnectionStatus(ConnectionStatus);
//	}
//	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
//	{
//		CLog.Log($"OnShutdown {shutdownReason}");
//		SetConnectionStatus(ConnectionStatus.Disconnected);

//		(string status, string message) = ShutdownReasonToHuman(shutdownReason);
//		_disconnectUI.ShowMessage(status, message);

//		RoomPlayer.Players.Clear();

//		if (_runner)
//			Destroy(_runner.gameObject);

//		// Reset the object pools
//		_pool.ClearPools();
//		_pool = null;

//		_runner = null;
//	}
//	public void OnInput(NetworkRunner runner, NetworkInput input) { }
//	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
//	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
//	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
//	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
//	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
//	public void OnSceneLoadDone(NetworkRunner runner) { }
//	public void OnSceneLoadStart(NetworkRunner runner) { }

//	private static (string, string) ShutdownReasonToHuman(ShutdownReason reason)
//	{
//		switch (reason)
//		{
//			case ShutdownReason.Ok:
//				return (null, null);
//			case ShutdownReason.Error:
//				return ("Error", "Shutdown was caused by some internal error");
//			case ShutdownReason.IncompatibleConfiguration:
//				return ("Incompatible Config", "Mismatching type between client Server Mode and Shared Mode");
//			case ShutdownReason.ServerInRoom:
//				return ("Room name in use", "There's a room with that name! Please try a different name or wait a while.");
//			case ShutdownReason.DisconnectedByPluginLogic:
//				return ("Disconnected By Plugin Logic", "You were kicked, the room may have been closed");
//			case ShutdownReason.GameClosed:
//				return ("Game Closed", "The session cannot be joined, the game is closed");
//			case ShutdownReason.GameNotFound:
//				return ("Game Not Found", "This room does not exist");
//			case ShutdownReason.MaxCcuReached:
//				return ("Max Players", "The Max CCU has been reached, please try again later");
//			case ShutdownReason.InvalidRegion:
//				return ("Invalid Region", "The currently selected region is invalid");
//			case ShutdownReason.GameIdAlreadyExists:
//				return ("ID already exists", "A room with this name has already been created");
//			case ShutdownReason.GameIsFull:
//				return ("Game is full", "This lobby is full!");
//			case ShutdownReason.InvalidAuthentication:
//				return ("Invalid Authentication", "The Authentication values are invalid");
//			case ShutdownReason.CustomAuthenticationFailed:
//				return ("Authentication Failed", "Custom authentication has failed");
//			case ShutdownReason.AuthenticationTicketExpired:
//				return ("Authentication Expired", "The authentication ticket has expired");
//			case ShutdownReason.PhotonCloudTimeout:
//				return ("Cloud Timeout", "Connection with the Photon Cloud has timed out");
//			default:
//				CLog.LogWarning($"Unknown ShutdownReason {reason}");
//				return ("Unknown Shutdown Reason", $"{(int)reason}");
//		}
//	}

//	private static (string, string) ConnectFailedReasonToHuman(NetConnectFailedReason reason)
//	{
//		switch (reason)
//		{
//			case NetConnectFailedReason.Timeout:
//				return ("Timed Out", "");
//			case NetConnectFailedReason.ServerRefused:
//				return ("Connection Refused", "The lobby may be currently in-game");
//			case NetConnectFailedReason.ServerFull:
//				return ("Server Full", "");
//			default:
//				CLog.LogWarning($"Unknown NetConnectFailedReason {reason}");
//				return ("Unknown Connection Failure", $"{(int)reason}");
//		}
//	}



//	private void Connect()
//	{
//		if (_runner == null)
//		{
//			//SetConnectionStatus(ConnectionStatus.Connecting);
//			GameObject go = new GameObject("Session");
//			//go.transform.SetParent(transform);
//			DontDestroyOnLoad(go);
//			//_players.Clear();
//			_runner = go.AddComponent<NetworkRunner>();
//			_runner.AddCallbacks(this);

//			//_runner.

//		}
//	}

//	//Conecta con el lobby generico para poder acceder a la lista de usuarios
//	public async Task JoinLobby()
//	{

//		Connect();
//		CLog.Log("Estado conexion: " + _runner.CurrentScene);
//		var result = await _runner.JoinSessionLobby(SessionLobby.Custom, "MyCustomLobby");
//		if (result.Ok)
//		{
//			// all good
//			CLog.Log($"Conexion Exitosa " + result + " - " + _runner);
//		}
//		else
//		{
//			CLog.LogError($"Failed to Start: {result.ShutdownReason}");
//		}
//		// _ = StartHost(runner);
//	}

//	public async Task StartHost()//NetworkRunner runner)//CREO LA SESION
//	{
//		Connect();
//		var result = await _runner.StartGame(new StartGameArgs()
//		{
//			GameMode = GameMode.Host,
//			CustomLobbyName = "MyCustomLobby"
//		});

//		if (result.Ok)
//		{
//			CLog.LogError("CREE EL SERVER");
//			// all good
//		}
//		else
//		{
//			CLog.LogError($"Failed to Start: {result.ShutdownReason}");
//		}
//	}


//	//Actualiza la lista de Sesiones abierta en tiempo real
//	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
//	{
//		CLog.Log($"Session List Updated with {sessionList.Count} session(s)");
//		ListaSalas = new List<itemList>();
//		bool resetearSesionSeleccionada = true;
//		SessionProperty sp;
//		foreach (var session in sessionList)
//		{


//			CLog.Log($"Listando: {session.Name}");

//			session.Properties.TryGetValue(SessionPropertyKey.TrackId.ToString(), out sp);
//			if (sp != null) CLog.Log("Prop: " + SessionPropertyKey.TrackId + ": " + sp.PropertyValue.ToString());
//			string trackid = SessionPropertyKey.TrackId.ToString() + " " + sp.PropertyValue.ToString();
//			session.Properties.TryGetValue(SessionPropertyKey.Laps.ToString(), out sp);
//			if (sp != null) CLog.Log("Prop: " + SessionPropertyKey.Laps + ": " + sp.PropertyValue.ToString());
//			string Laps = SessionPropertyKey.Laps.ToString() + " " + sp.PropertyValue.ToString();
//			session.Properties.TryGetValue(SessionPropertyKey.MaxLaps.ToString(), out sp);
//			if (sp != null) CLog.Log("Prop: " + SessionPropertyKey.MaxLaps + ": " + sp.PropertyValue.ToString());
//			string maxLpas = SessionPropertyKey.MaxLaps.ToString() + " " + sp.PropertyValue.ToString();
//			session.Properties.TryGetValue(SessionPropertyKey.Bet.ToString(), out sp);
//			if (sp != null) CLog.Log("Prop: " + SessionPropertyKey.Bet + ": " + sp.PropertyValue.ToString());
//			string Bet = SessionPropertyKey.Bet.ToString() + " " + sp.PropertyValue.ToString();
//			//session.Properties.TryGetValue(SessionPropertyKey.Bet.ToString(), out sp);
//			//foreach(var sp2 in session.Properties)
//			//CLog.Log("Estado: "+ sp2.Key.ToString()+" - "+ sp2.Value.ToString());
//			//session.Properties.


//			ListaSalas.Add(new itemList(session.Name, session.PlayerCount, session.MaxPlayers, session.IsOpen, trackid, Laps, maxLpas, Bet));

//			if (ObjectListRoom.instance.RoomSelectedObjet.Equals(session.Name))
//				resetearSesionSeleccionada = false;
//			//        if (ContentListRooms.instance.RoomSelectedObjet.Equals(session.Name))
//			//resetearSesionSeleccionada = false;

//			/*
//			// Entrar a la Primera Session disponible
//			runner.StartGame(new StartGameArgs()
//			{
//				GameMode = GameMode.Client, // Client GameMode
//				SessionName = session.Name, // Session to Join
//				//SceneObjectProvider = GetSceneProvider(runner), // Scene Provider
//				DisableClientSessionCreation = true, // Make sure the client will never create a Session
//			});

//			return;*/
//		}
//		if (resetearSesionSeleccionada) ObjectListRoom.instance.RoomSelectedObjet = "";
//		//if (resetearSesionSeleccionada) ContentListRooms.instance.RoomSelectedObjet = "";

//		if (ListaSalas != null) _listRooms.cargarLista(ListaSalas);
//		//if (ListaSalas != null) _contentRooms.cargarLista(ListaSalas);
//	}

//	public void joinLobbyFromList()
//	{
//		StartCoroutine(consultarSaldo(false));
//	}

//	//Se conecta a la sala seleccioanda	
//	public void joinLobbyFromListOk(/*NetworkRunner runner,string sessionName*/)
//	{
//		CLog.Log("Entro al Join " + ObjectListRoom.instance.RoomSelectedObjet + " Runer vale: " + _runner);
//		//CLog.Log("Entro al Join " + ContentListRooms.instance.RoomSelectedObjet + " Runer vale: " + _runner);


//		//_pool = _runner.gameObject.AddComponent<FusionObjectPoolRoot>();
//		if (_runner)
//		{
//			//ListaSalas.Add(session.Name);
//			// Entrar a la Primera Session disponible
//			_runner.StartGame(new StartGameArgs()
//			{
//				GameMode = GameMode.Client, // Client GameMode
//				SessionName = ObjectListRoom.instance.RoomSelectedObjet,//session.Name, // Session to Join
//																		//SessionName = ContentListRooms.instance.RoomSelectedObjet,//session.Name, // Session to Join
//				SceneObjectProvider = _levelManager,
//				//SceneObjectProvider = GetSceneProvider(runner), // Scene Provider
//				DisableClientSessionCreation = true, // Make sure the client will never create a Session
//			});
//			CLog.Log("Realice la conexion" + ObjectListRoom.instance.RoomSelectedObjet + " Runer vale: " + _runner);
//			//CLog.Log("Realice la conexion" + ContentListRooms.instance.RoomSelectedObjet + " Runer vale: " + _runner);

//			if (RoomPlayer.Local != null)
//			{
//				RoomPlayer.Local.RPC_SetKartId(ClientInfo.KartId, ClientInfo.CharId); //Envio el ID a la instancia de este player en el sevidor, debo enviar el ID de Kart y Bajar su config desde el PlayerData
//			}

//		}
//	}



//	//Coroutine createAndJoin;
//	bool inCoroutine;
//	//Rutina para consutar el saldo del player
//	public System.Collections.IEnumerator consultarSaldo(bool _create)
//	{
//		if (inCoroutine) yield break;
//		inCoroutine = true;
//		PlayfabClientCurrency.GetCurrencyTel();
//		yield return new WaitWhile(() => DataEconomy.ECONOMYSTATUS == EconomyStatus.DOWNLOADING);

//		if (DataEconomy.ECONOMYSTATUS == EconomyStatus.OK)
//		{
//			if (DisplayDataUI.instance.getTel() > 0)
//			{

//				if (_create)
//					PopUpManager._instance.setPopUp("Atencion", "Crearas una partida apostando " + 500 + " TEL\n Continuar?", null, true);
//				else
//					PopUpManager._instance.setPopUp("Atencion", "Entraras a una partida apostando " + 500 + " TEL\n Continuar?", null, true);

//				yield return new WaitWhile(() => PopUpManager._instance.popUpState == PopUpStates.Wait);

//				if (PopUpManager._instance.popUpState == PopUpStates.Ok)
//				{
//					if (_create)
//					{
//						JoinOrCreateLobbyOk();
//					}
//					else
//					{
//						joinLobbyFromListOk();
//					}
//				}
//				else if (PopUpManager._instance.popUpState == PopUpStates.Cancel)
//				{
//					if (_create)
//					{
//						CLog.Log("REGRESANDO ACA ");
//						mainScreen.BackTo(gameModesScreen);
//					}
//				}
//			}
//			else
//			{
//				PopUpManager._instance.setPopUp("Error", "No cuenta con fondos suficientes para continuar " + DisplayDataUI.instance.getTel(), null, false);
//			}
//		}
//		else if (DataEconomy.ECONOMYSTATUS == EconomyStatus.ERROR)
//		{
//			PopUpManager._instance.setPopUp("Error", "Error en la conexion, intente de nuevo", null, false);
//		}
//		inCoroutine = false;
//		//popUpCoroutine = null;
//	}



//}




//public class itemList
//{
//	public string sessionName;
//	public int players;
//	public int maxPlayers;
//	public bool isOpen;
//	public string track;
//	public string laps;
//	public string maxLaps;
//	public string bet;
//	public itemList(string _nameRoom, int _players, int _maxPlayers, bool _status, string _track, string _laps, string _maxlaps, string _bet)
//	{
//		//CLog.Log("guarde: " + _nameRoom + " - "+_players + " - " + _status);
//		sessionName = _nameRoom;
//		players = _players;
//		maxPlayers = _maxPlayers;
//		isOpen = _status;
//		track = _track;
//		laps = _laps;
//		maxLaps = _maxlaps;
//		bet = _bet;
//		//return this;
//	}
//}