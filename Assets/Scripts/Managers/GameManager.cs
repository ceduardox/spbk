using System;
using Fusion;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
public class GameManager : NetworkBehaviour
{
	public static event Action<GameManager> OnLobbyDetailsUpdated;
	[SerializeField, Layer] private int groundLayer;
	public static int GroundLayer => Instance.groundLayer;
	[SerializeField, Layer] private int kartLayer;
	public static int KartLayer => Instance.kartLayer;


	public new Camera camera;
	private ICameraController cameraController;

	public GameType GameType => ResourceManager.Instance.gameTypes[GameTypeId];

	public static Track CurrentTrack { get; private set; }
	public static bool IsPlaying => CurrentTrack != null;

	public static GameManager Instance { get; private set; }

	public string TrackName => TranslateUI.getStringUI(ResourceManager.Instance.tracksDefinitions[TrackId].trackName);
	public string TrackDesc => TranslateUI.getStringUI(ResourceManager.Instance.tracksDefinitions[TrackId].trackDesc);
	public string ModeName => ResourceManager.Instance.gameTypes[GameTypeId].modeName.ToString();

	[Networked(OnChanged = nameof(OnLobbyDetailsChangedCallback))] public string LobbyName { get; set; }
	[Networked(OnChanged = nameof(OnLobbyDetailsChangedCallback))] public int TrackId { get; set; }
	[Networked(OnChanged = nameof(OnLobbyDetailsChangedCallback))] public int GameTypeId { get; set; }
	[Networked(OnChanged = nameof(OnLobbyDetailsChangedCallback))] public int MaxUsers { get; set; }
	[Networked(OnChanged = nameof(OnLobbyDetailsChangedCallback))] public int Bet { get; set; }


	//----Restablecer restart
	public int maxTimerReturn;
	public int maxTimerVoteTrak;
	public bool modeVote = true;
	internal int playersInLastRace;//numero de jugadores que comenzaron la carrera
	internal GameUI HUD;

	float timer;
	int oldTimer;
	internal bool restart;
	public bool vote;
	List<KartEntity> karts;
	public Dictionary<string, int> voteDate = new Dictionary<string, int>();
	private bool botVotesInjected;
	static bool changeDataLobby;

	private static void OnLobbyDetailsChangedCallback(Changed<GameManager> changed)

	{
		OnLobbyDetailsUpdated?.Invoke(changed.Behaviour);

		changeDataLobby = true;
		//CLog.Log("Change de datos");
	}






	private void Awake()
	{
		if (Instance)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

		DontDestroyOnLoad(gameObject);
		restartTimer();
	}

	public override void Spawned()
	{
		base.Spawned();

		if (Object.HasStateAuthority)
		{
			//GameTypeId = ServerInfo.GameMode; 
			GameTypeId = ServerInfo.GameMode;
			LobbyName = ServerInfo.LobbyName;
			TrackId = ServerInfo.TrackId;

			TrackId = ServerInfo.TrackId;// = ResourceManager.instance.getTrackID(GameType.modeName);
			CLog.Log("ESTO ES: " + GameType.modeName + " - " + ServerInfo.TrackId);



			MaxUsers = ServerInfo.MaxUsers;
			if (GameLauncher.instance.serverBet)
				Bet = GameLauncher.instance.minBetTEL;

			//GameLauncher.instance.setSessionProperties(SessionPropertyKey.TrackId, TrackId);

			//GameLauncher.instance.setSessionProperties(SessionPropertyKey.TrackId, ServerInfo.TrackId);
			//GameLauncher.instance.setSessionProperties(SessionPropertyKey.Mode, ServerInfo.GameMode);
			//GameLauncher.instance.setSessionProperties(SessionPropertyKey.Laps, 0);
			//GameLauncher.instance.setSessionProperties(SessionPropertyKey.MaxLaps, GameType.lapCount);
			//setDataLooby(); //changeDataLobby = true;
			//Invoke("setDataLooby", 5); 
		}

	}

	private void FixedUpdate()
	{
		//CLog.Log(GameLauncher.instance.modeServerDedicado + " " + restart+" "+ timer+" "+ oldTimer);
		if (GameLauncher.instance.modeServerDedicado && restart)
		{
			//verificar usuarios conectados

			if (GameLauncher.instance.isServer)
			{

				if (modeVote)//Para le modo votacipon de la pista al finalizar la carrera
				{
					if ((timer -= Time.deltaTime) < 0 && !vote)//Mando a votar
					{
						//restartTimer();

						if (allFinishKartsOk)
						{
							var voteTracks = crearListeTracks();
							RPC_countDown(0, 2, voteTracks, rewards);
							vote = true;
							timer = maxTimerVoteTrak;
						}
						else allFinishForce();
						return;
						//Funcion Transaccion					
					}
					else if (vote && timer < 0)//Termino el timpo para votar
					{
						selectNewVoteTrack();
						restartTimer();
						returnLobby();

					}

					if ((int)timer != oldTimer)
					{
						//if(oldTimer==)
						CLog.Log("MANDO TIMER");
						RPC_countDown((int)timer, vote ? 1 : 0, new int[2], "");
						//if(oldTimer==5)

					}
					oldTimer = (int)timer;


				}
				else //MODO NORMAL
				{
					if ((timer -= Time.deltaTime) < 0)
					{
						restartTimer();
						returnLobby();
						//Funcion Transaccion					
					}
					else
					{
						if ((int)timer != oldTimer)
							RPC_countDown((int)timer, 0, listIDTracksVote, rewards);
						oldTimer = (int)timer;
					}
				}
			}
		}

		if (changeDataLobby)
		{
			changeDataLobby = false;
			if ((GameLauncher.instance.modeServerDedicado && GameLauncher.instance.isServer) ||
				(RoomPlayer.Local && (RoomPlayer.Local.IsLeader)))
			{
				setDataLooby();


			}
		}

		// this shouldn't really be an interface due to how Unity handle's interface lifecycles (null checks dont work).
		if (cameraController == null) return;

		if (cameraController.Equals(null))
		{
			CLog.LogWarning("Phantom object detected");
			cameraController = null;
			return;
		}


		if (cameraController.ControlCamera(camera) == false)
			cameraController = null;


	}

	public void restartTimer()
	{
		vote = restart = false;
		timer = maxTimerReturn;
		karts = null;
		voteDate.Clear();
		votesCount = null;
		listIDTracksVote = null;
		botVotesInjected = false;

	}

	public void returnLobby()
	{
		Managers.LevelManager.LoadMenu();
		//	Invoke("testLobbyRoom",1);
	}
	public void testLobbyRoom()
	{
		if (LobbyUI._instance != null && !LobbyUI._instance.gameObject.activeSelf)
			LobbyUI._instance.gameObject.SetActive(true);


	}

	public static void GetCameraControl(ICameraController controller)
	{
		Instance.cameraController = controller;
	}

	public static bool IsCameraControlled => Instance.cameraController != null;

	public static void SetTrack(Track track)
	{
		CurrentTrack = track;
	}

	public void setDataLooby()
	{
		GameLauncher.instance.setSessionProperties(SessionPropertyKey.Bet, Bet);
		GameLauncher.instance.setSessionProperties(SessionPropertyKey.TrackId, TrackId);
		GameLauncher.instance.setSessionProperties(SessionPropertyKey.MaxLaps, GameType.lapCount);
	}

	/*
		[Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
		public void RPC_setDataLooby(SessionPropertyKey _key, int value)
		{
			if(firstTime) GameLauncher.instance.setSessionProperties(_key, Bet=value);
			firstTime = false;
		}
		*/

	//bool firstTime=true;
	public void resetServerBet()
	{
		Bet = GameLauncher.instance.minBetTEL;
		//firstTime = true;
		if (transactionPendiente)
		{
			StartCoroutine(GameLauncher.instance.devolver(true));
		}

	}

	int actualLap;
	public void setLat(int _lap)
	{
		if (_lap == -1)
		{
			actualLap = _lap;
		}
		else if (actualLap < _lap)
		{
			actualLap = _lap;
			GameLauncher.instance.setSessionProperties(SessionPropertyKey.Laps, actualLap - 1);
		}
	}

	bool allFinishKarts;
	bool allFinishKartsOk;
	bool setExperiencieOk;
	public void allFinishForce()
	{
		if (!allFinishKarts)
		{
			foreach (RoomPlayer player in RoomPlayer.Players)
			{
				if (!player.Kart.Kart.LapController.HasFinished)
					player.Kart.Kart.LapController.Lap = GameType.lapCount + 1;

			}
			allFinishKarts = true;
		}
	}

	public void allFinish(List<KartEntity> _karts)
	{
		allFinishKarts = true;
		//if (rewards == "")
		{
			if (GameLauncher.instance.isServer)
			{
				karts = _karts;
				setExperiencia();
				if (timer > 5)
				{
					timer = 5;
				}

			}
		}
	}
	public void setTransactionPendiente(bool _value)
	{
		transactionPendiente = _value;

	}

	private bool transactionPendiente;
	public void setWinner(RoomPlayer _idWinner)
	{

		//if (transactionPendiente)
		//{
			//StartCoroutine(transaction(_idWinner.Username));
			rewards += _idWinner.playFabID + "+" + (Bet * playersInLastRace - ((Bet * playersInLastRace) * PlayfabManager.instance.Comision[0])) + "+*";
			//CLog.Log("Envie: "+ rewards);
		//}
	}

	private System.Collections.IEnumerator transaction(string _idWinner)
	{
		{
			int contador = 0;
			while (contador < 10)
			{
				transactionPendiente = false;

				Busines.Transaction(new List<string>() { _idWinner });
				yield return new WaitWhile(() => DataEconomy.ECONOMYSTATUS == EconomyStatus.DOWNLOADING);
				if (DataEconomy.ECONOMYSTATUS == EconomyStatus.OK)
				{

					yield break;
				}
				yield return new WaitForSeconds(1);
				//else
			}
			CLog.LogError("FALLO EN LA ASIGNACION ");
		}
	}
	string rewards;
	public void setExperiencia()
	{
		StartCoroutine(setExperienciaAllPlayers());
	}
	System.Collections.IEnumerator setExperienciaAllPlayers()
	{
		if (setExperiencieOk) yield break;
		setExperiencieOk = true;
		List<string> jugadores = new List<string>();
		List<int> puntuaciones = new List<int>();
		int multiplicador = GameLauncher.instance.serverBet ? 4 : 1;
		if (rewards == "")
			rewards = "NOID+0+*";
		////////////////////////////////////////////////////////////////////////////////////////ADD XP
		for (int i = 0; i < karts.Count; i++)
		{
			if (isValidKart(i))
			{
				jugadores.Add(karts[i].Controller.RoomUser.playFabID);
				puntuaciones.Add(PlayfabManager.instance.WinnerXP * (playersInLastRace - i) * multiplicador);
			}

		}

		CLog.Log("Envio XP: " + jugadores.Count + " - " + puntuaciones.Count);
		yield return new WaitForSeconds(.2f);
		Busines.AddExpertice(jugadores, puntuaciones, 0);

		for (int i = 0; i < puntuaciones.Count; i++)
		{
			rewards += jugadores[i] + "+" + puntuaciones[i] + "+";

		}
		rewards += "*";



		////////////////////////////////////////////////////////////////////////////////////////ADD CUPS
		puntuaciones.Clear();
		//puntuaciones=new List<int>()
		for (int i = 0; i < (karts.Count >= 3 ? 3 : karts.Count); i++)
		{
			if (isValidKart(i))
			{
				//jugadores.Add(karts[i].Controller.RoomUser.playFabID);
				puntuaciones.Add((PlayfabManager.instance.WinnerCup - i) * (multiplicador > 2 ? multiplicador / 2 : multiplicador));
			}

		}
		CLog.Log("Envio CUPS: " + jugadores.Count + " - " + puntuaciones.Count);

		for (int i = 0; i < puntuaciones.Count; i++)
		{
			rewards += jugadores[i] + "+" + puntuaciones[i] + "+";

		}
		rewards += "*";

		yield return new WaitForSeconds(.2f);

		for (int i = puntuaciones.Count; i < jugadores.Count; i++)
			puntuaciones.Add(0);


		Busines.AddExpertice(jugadores, puntuaciones, 1);

		////////////////////////////////////////////////////////////////////////////////////////ADD TE Blok
		puntuaciones.Clear();
		for (int i = 0; i < karts.Count; i++)
		{
            //jugadores.Add(karts[i].Controller.RoomUser.playFabID);

            var apuestaTotal = Bet * jugadores.Count;
            if (jugadores.Count <4)
			{
				apuestaTotal = Bet * 4;
            }
			
			float Porcentaje = PlayfabManager.instance.Comision[i];

			float Recompensa = apuestaTotal * Porcentaje;
			int RecompensaInt = (int)MathF.Round(Recompensa);

                puntuaciones.Add(RecompensaInt);
				CLog.Log("Envio SPBK: " + jugadores[i] + " - " + puntuaciones[i]);
			

		}
		CLog.Log($"Envio SPBK: " + jugadores.Count + " - " + $"repartiran! { Bet* jugadores.Count}");
		//Busines.AddTnl(jugadores, puntuaciones);
		Busines.GetSPBKAutomatic(jugadores, Bet* jugadores.Count);

		for (int i = 0; i < puntuaciones.Count; i++)
		{
			rewards += jugadores[i] + "+" + puntuaciones[i] + "+";

		}


		//////////////////////////////////////////////////////////////////////////////////////ADD TORNEO
		if (VersionNv.torneoActual != null)
		{
			VersionNv.verificarStatusTorneos(VersionNv.torneoActual);
			if (VersionNv.torneoActual.Active)
			{
				puntuaciones.Clear();
				for (int i = 0; i < (karts.Count >= 3 ? 3 : karts.Count); i++)
				{
					if (isValidKart(i))
					{
						//jugadores.Add(karts[i].Controller.RoomUser.playFabID);
						//puntuaciones.Add((PlayfabManager.instance.WinnerRaceTorneo - i) * (multiplicador / 2));
						puntuaciones.Add(((PlayfabManager.instance.WinnerRaceTorneo) * (playersInLastRace - i) * (1)) * (karts[i].Controller.RoomUser.isTorneo ? 1 : 0));
						CLog.Log("Envio TORNEO: " + jugadores[i] + " - " + puntuaciones[i] + " - " + karts[i].Controller.RoomUser.isTorneo + " - " + VersionNv.torneoActual.Id);

					}

				}
				CLog.Log("Envio TORNEO: " + jugadores.Count + " - " + puntuaciones.Count);
				yield return new WaitForSeconds(.2f);

				for (int i = puntuaciones.Count; i < jugadores.Count; i++)
					puntuaciones.Add(0);

				Busines.AddExpertice(jugadores, puntuaciones, 2, VersionNv.torneoActual.Id);
			}
			else 
				CLog.Log("Torneo Finalizado: " + VersionNv.torneoActual.Id);
		}
		else 
			CLog.Log("Torneo Nulo: ");
		////////////////////////////////////////////////////////////////////////////////////////ADD WinRace
		puntuaciones.Clear();
		for (int i = 0; i < (karts.Count >= 3 ? 3 : karts.Count); i++)
		{
			if (isValidKart(i))
			{
				//jugadores.Add(karts[i].Controller.RoomUser.playFabID);
				//puntuaciones.Add((PlayfabManager.instance.WinnerRaceTorneo - i) * (multiplicador / 2));
				puntuaciones.Add(1);				

			}

		}
		CLog.Log("Envio Win Race: " + jugadores.Count + " - " + puntuaciones.Count);
		yield return new WaitForSeconds(.2f);

		for (int i = puntuaciones.Count; i < jugadores.Count; i++)
			puntuaciones.Add(0);
		
		Busines.AddExpertice(jugadores, puntuaciones, 3);




		allFinishKartsOk = true;
	}
	public bool isValidKart(int i)
    {
		if (karts[i] != null &&
				karts[i].Controller != null)
			return true;
		else return false;
	}
	public void completarVectores()
    {

    }

	public System.Collections.IEnumerator records(string _id, int _time, bool _guardar)
	{

		yield return new WaitWhile(() => RoomPlayer.Local==null);

		if (RoomPlayer.Local &&
			RoomPlayer.Local.Username != null &&
			LobbyUI._instance != null &&
			LobbyUI._instance.lobbyPlayerNameRecord != null)
			LobbyUI._instance.lobbyPlayerNameRecord.text = RoomPlayer.Local.Username;
		

		if (_guardar)
		{
			LeaderBoardSystem.SetRecord(_id, _time);
		}
		else
		{
			LeaderBoardSystem.GetRecord(_id);

		}
		yield return new WaitWhile(() => LeaderBoardSystem.status == EconomyStatus.DOWNLOADING);

		CLog.Log("Leido: " + _id + " - " + LeaderBoardSystem.recordTrack + " " + LeaderBoardSystem.parseTime(LeaderBoardSystem.recordTrack));


		if (LeaderBoardSystem.status == EconomyStatus.OK)
		{
			if (!_guardar)
			{
				 if (LobbyUI._instance != null && LobbyUI._instance.lobbyPlayerRecord != null)
					 LobbyUI._instance.lobbyPlayerRecord.text = LeaderBoardSystem.parseTime(LeaderBoardSystem.recordTrack);
			}
		}

	}

	int position;
	public void setHud(GameUI _hud)
	{
		HUD = _hud;
		position = -1;
	}

	public void setPosition(int _pos)
    {
		if(position!=_pos)
        {
			position = _pos;
			for (int i=0;i<HUD.playerPosition.childCount ;i++)
            {
				HUD.playerPosition.GetChild(i).gameObject.SetActive(i == position);
			}
        }
    }
	public void selectNewVoteTrack()
    {
		if (listIDTracksVote == null || listIDTracksVote.Length == 0)
		{
			CLog.LogWarning("selectNewVoteTrack sin opciones de pista; se mantiene la pista actual.");
			return;
		}

		int maxIndex = 0;
		int optionCount = listIDTracksVote.Length;
		if (votesCount != null && votesCount.Length >= optionCount)
		{
			int bestVotes = int.MinValue;
			List<int> winners = new List<int>();
			for (int i = 0; i < optionCount; i++)
			{
				int currentVotes = Mathf.Max(0, votesCount[i]);
				if (currentVotes > bestVotes)
				{
					bestVotes = currentVotes;
					winners.Clear();
					winners.Add(i);
				}
				else if (currentVotes == bestVotes)
				{
					winners.Add(i);
				}
			}

			if (winners.Count > 0)
				maxIndex = winners[UnityEngine.Random.Range(0, winners.Count)];
		}
		else
		{
			maxIndex = UnityEngine.Random.Range(0, optionCount);
		}

		CLog.Log("El track ganador fue: " + maxIndex);
		TrackId = listIDTracksVote[maxIndex];


	}

	int[] votesCount;
	int[] listIDTracksVote;

	[Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
	public void RPC_changeTrack(string _player, int _track)
	{
		if (_track < 0 || _track >= 3)
		{
			CLog.LogWarning("RPC_changeTrack recibio voto invalido: " + _track);
			return;
		}

		if (!voteDate.ContainsKey(_player))
			voteDate.Add(_player, _track);
		else
		{
			voteDate[_player] = _track;
		}
		CLog.Log("Recibi: " + _player + " " + _track + " " + voteDate.Count);
		votesCount = new int[3];

		foreach (var vote in voteDate)
		{
			votesCount[vote.Value]++;
		}

		//Envio a todos las votaciones
		if (HUD)
		{
			HUD.voteTrack.countVotes(votesCount);
		}
	}


	[Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
	public void RPC_syncTimer(float _timer, int _completeServer)
	{
		TimerCounterScreen._instance.syncTimer(_timer, _completeServer);
		
	}


	public int[] crearListeTracks()
    {
		listIDTracksVote = new int[3];
		for (int i=0;i<listIDTracksVote.Length;i++)
			listIDTracksVote[i]= -1;
		int randomIndex;
		bool repetir;

		listIDTracksVote[0] = TrackId;
		int repetciones = 0;
		for(int i=1;i<listIDTracksVote.Length;i++)
        {
			
			do
			{
				repetir = false;
				while (true)
				{
					randomIndex = UnityEngine.Random.Range(0, ResourceManager.Instance.tracksDefinitions.Count);
					if (ResourceManager.Instance.tracksDefinitions[randomIndex].mode == GameManager.Instance.GameType.modeName)
						break;
				}
				for (int j = 0; j < listIDTracksVote.Length; j++)
				{
					if (listIDTracksVote[j] == randomIndex)
					{
						repetir = true;
						repetciones++;
						break;
					}
				}
			} while (repetir&&repetciones<500);
			listIDTracksVote[i] = randomIndex;

		}
		
		return listIDTracksVote;
	}


	/// <summary>
	/// 0: Cuenta regresiva,
	/// 1: cuenta regresiva Votar pista 
	/// 2: muestra la pantalla de seleccion de pistas y establece las pistas para votar , 
	/// </summary>
	/// <param name="_timer"></param>
	/// <param name="_vote"></param>
	/// <param name="listIDTracksVote"></param>
	[Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
	public void RPC_countDown(int _timer, int _vote, int[] listIDTracksVote, string _rewards)
	{
		CLog.Log("Envio a todos: " + _vote + " " + _timer+ " "+_rewards);
		

		if (!GameLauncher.instance.isServer)
		{
			if (_vote==1)
			{
				HUD.showVoteTrack();
				HUD.voteTrack.setCount(_timer);
			}
			else if (_vote == 0)
			{
				//GameObject g = GameObject.Find("HUD(Clone)");
				//HUD.waitingPlayers.text = "La carrera finalizara en : " + _timer + " segundos";
				if (HUD.waitingPlayers.text!=null)
				{
					HUD.waitingPlayers.text = TranslateUI.getStringUI(UI_CODE.HUD_TIMER).Replace("XXX", _timer.ToString());
					HUD.waitingPlayers.gameObject.SetActive(true); //g.GetComponent<GameUI>().waitingPlayers.gameObject.SetActive(true);
				}
			}
			else if (_vote == 2)
            {
				CLog.Log("Envio: " + rewards);


				HUD.endRaceScreen.outPanel();
				HUD.voteTrack.setTracks(listIDTracksVote, _rewards);
				if (votesCount != null && votesCount.Length >= 3)
					HUD.voteTrack.countVotes(votesCount);
				HUD.waitingPlayers.gameObject.SetActive(false);

			}

		}
	}
	/// Modificar las listas de update cuando se sale del juego



	public void setTransaccion(List<string> _players)
    {
		rewards = "";
		allFinishKarts = false;
		allFinishKartsOk = false;
		setExperiencieOk = false;
		StartCoroutine(starRace(_players));
    }

	System.Collections.IEnumerator starRace(List<string> _players)
	{
		if (GameLauncher.instance.serverBet)
		{
			
			Busines.SetTransaction(_players, GameLauncher.instance.idServer.ToString(), Bet);
			yield return new WaitWhile(() => DataEconomy.ECONOMYSTATUS == EconomyStatus.DOWNLOADING);


			if (DataEconomy.ECONOMYSTATUS == EconomyStatus.OK)
			{
				Debug.Log("Transaccion realizada con exito");
				setTransactionPendiente(true);
			}
			else if (DataEconomy.ECONOMYSTATUS == EconomyStatus.ERROR)
			{
				PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_ERROR), TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_FAILSTART), IconosPopUp.error, false);
				Managers.LevelManager.LoadMenu();
			}
		}
		else Debug.Log("Servidor Free Transaccion realizada con exito");
	}






}
