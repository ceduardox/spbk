using Fusion;
using UnityEngine;
using System.Collections.Generic;
public class Track : NetworkBehaviour, ICameraController
{
	public static Track Current { get; private set; }

	[Networked] public TickTimer StartRaceTimer { get; set; }
	[Networked] public int lightIndex { get; set; } = -1;
	
	public CameraTrack[] introTracks;
	public Checkpoint[] checkpoints;
	public Transform checkpointsList;
	public Transform[] spawnpoints;
	public FinishLine finishLine;
	public GameObject itemContainer;
	public GameObject coinContainer;

	public TrackDefinition definition;
	public TrackStartSequence sequence;
	public Transform mapaRadar;

	public string music = "";
	public float introSpeed = 0.5f;

	private int _currentIntroTrack;
	private float _introIntervalProgress;
	public bool isNight;
	public Color fondo;
	public Material skyboxTrack;
	[SerializeField]public Texture2D[] bake;
	[SerializeField] int quantityTextures;
	[SerializeField] string folderBake;
	LightmapData[] data;
	private void Awake()
	{
		
		// data=new LightmapData[quantityTextures];
		// for (var i = 0; i < quantityTextures; i++)
		// {
		// 	data[i]=new LightmapData();
		// 	CLog.Log(data[0]==null);
			
		// 	data[i].lightmapColor=Resources.Load<Texture2D>($"Bakes/{folderBake}/Lightmap-{i.ToString()}_comp_light");
		// 	data[i].lightmapDir=Resources.Load<Texture2D>($"Bakes/{folderBake}/Lightmap-{i.ToString()}_comp_dir");
			
		// }
		// LightmapSettings.lightmaps=data;
		
		
		//RenderSettings.skybox = mat2;
		if (checkpointsList != null)
			checkpointsList.gameObject.SetActive(true);
		RenderSettings.ambientLight = fondo;
		RenderSettings.skybox = skyboxTrack;
		Current = this;
		InitCheckpoints();

		var gm = GameManager.Instance;
		if (gm != null && gm.GameType != null && gm.GameType.hasPickups == false)
		{
			if (itemContainer != null) itemContainer.SetActive(false);
			if (coinContainer != null) coinContainer.SetActive(false);
			
		}

		//GameLauncher.instance.stats.text = Screen.currentResolution.ToString();

		// Initialize cutscene
		AudioManager.StopMusic();

		GameManager.SetTrack(this);
		if (gm != null)
			gm.camera = Camera.main;

		var launcher = GameLauncher.instance;
		if (launcher != null && launcher.modeServerDedicado && launcher.isServer)
		{
			//GameManager.Instance.camera.enabled = false;


		}
		//CLog.Log("**ESTADO DE CONEXIO: " + GameLauncher.ConnectionStatus);
		if (launcher != null && !launcher.isServer && gm != null && gm.Bet == 0 && GameLauncher.ConnectionStatus==ConnectionStatus.Connected)
		{
			PlayfabLifesControl.ConsumeVidasIntentos();
			music = "track_"+Random.Range(0, 11);
		}

		//transform.Find("Lights");


		if (gm != null)
			StartIntro();
	}

	//GameObject light;
	public override void Spawned()
	{
		base.Spawned();
		lightIndex = -1;
		//----if (RoomPlayer.Local == null)
		//----		if (RoomPlayer.Local.IsLeader)


		//light = Resources.Load<GameObject>("Prefabs/Track/Lights/Lights");
		//light = Instantiate(light, transform);

		if (RoomPlayer.Local == null ||//soy dedicado
			RoomPlayer.Local.IsLeader)
		{//server sincroniza las pistas
			
			StartRaceTimer = TickTimer.CreateFromSeconds(Runner, sequence.duration + 4f);

			if (!isNight)
			{
				//lightIndex = Random.Range(0, light.transform.childCount-1);
				lightIndex = 0;
			}
			else
				lightIndex = 2;

			

		}



		if (!isNight)
		{
			StartCoroutine(setLight());
			
		}
		else 
		{

			//loadSkyBox(false);
		}

		
		sequence.StartSequence();
		if (GameManager.Instance.GameType.modeName == GameModes.Race)
			StartCoroutine(calcularVelocidad());
		else
			StartCoroutine(radar());
	} 

	public void loadSkyBox(bool _day)
    {
		return;
		if(_day)
			skyboxTrack = Resources.Load<Material>("Prefabs/Track/SkyBox/mat_Day");
		else
			skyboxTrack = Resources.Load<Material>("Prefabs/Track/SkyBox/mat_Night");

		if (skyboxTrack) RenderSettings.skybox = skyboxTrack;
	}

	System.Collections.IEnumerator setLight()
    {
		while (true)
		{
			//CLog.Log("EN EL SETLIGHT el vlaor es: " + lightIndex);
			if (lightIndex != -1)
			{
				//light.transform.GetChild(lightIndex).gameObject.SetActive(true);
				//if (lightIndex == light.transform.childCount - 1)
				//	isNight = true;
				loadSkyBox(!isNight );
				yield break;
			}
			yield return new WaitForSeconds(.1f);
		}
    }

	private void OnDestroy()
	{
		GameManager.SetTrack(null);
	}


	//Carga los Kart
	public void SpawnPlayer(NetworkRunner runner, RoomPlayer player)
	{
		if (runner == null || player == null || ResourceManager.Instance == null)
			return;

		Transform point = transform;
			if (spawnpoints != null && spawnpoints.Length > 0)
			{
				var index = RoomPlayer.Players.IndexOf(player);
				if (index < 0)
					index = 0;

				if (index >= spawnpoints.Length)
				{
					CLog.LogWarning($"Spawn index fuera de rango ({index}/{spawnpoints.Length}) para {player.Username}. Reutilizando slot.");
					index %= spawnpoints.Length;
			}

			if (spawnpoints[index] != null)
				point = spawnpoints[index];
		}
		else
		{
			CLog.LogWarning("Track sin spawnpoints configurados. Usando transform del Track.");
		}

		var kd = ResourceManager.Instance.getKart(player.KartId);
		if (kd == null || kd.prefab == null)
		{
			CLog.LogWarning($"No se puede spawnear kart para {player.Username}. KartId invalido: {player.KartId}");
			return;
		}

		var prefab = kd.prefab;
		prefab.id = player.KartId;

		PlayerRef owner = PlayerRef.None;
		try
		{
			if (player.Object != null && player.Object.IsValid)
				owner = player.Object.InputAuthority;
		}
		catch (System.InvalidOperationException)
		{
			owner = PlayerRef.None;
		}

		var entity = runner.Spawn(
			prefab,
			point.position,
			point.rotation,
			owner
		);

		if (entity == null || entity.Controller == null)
		{
			CLog.LogWarning($"Fallo al spawnear kart para {player.Username}");
			return;
		}

		entity.Controller.RoomUser = player;
		player.GameState = RoomPlayer.EGameState.GameCutscene;
		player.setController(entity.Controller);

		CLog.Log($"Spawning kart for {player.Username} as {entity.name}");
		entity.transform.name = $"Kart ({player.Username})";

	}

	//[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
	public void loadParts(Kart_Store kart_Store, RoomPlayer _player)
	{
		try
		{
			CLog.Log("CARGO LA CONFIG DE : " + _player.Username + " " + _player.getlistConfig(true).Count);
			//PlayerDataTitle.PlayerDataTi.TryGetValue(_id.ToString(), out List<PlayerD> playerData);

			//if (playerData!=null)
			{
				foreach (PlayerD _playerD in _player.getlistConfig(true))
				{
					kart_Store.changePart(ResourceManager.Instance.getKart(_player.KartId), _playerD, false);
				}
			}
		}
		catch (System.Exception ex)
		{
			CLog.Log("ERROR: " + ex);
		}

	}

	System.Collections.IEnumerator calcularVelocidad()
	{
		Transform checkPoint = null;
		Transform checkPointPlayer = null;
		Transform lastCheckPoint = null;

		List<KartEntity> kartsTMP = KartEntity.Karts;
		int totalCheckPoints = GameManager.Instance.GameType.lapCount * checkpoints.Length;
		int contador = -50;

		while (true)
		{
			GameLauncher.instance.stats.text = " fps: " + 1 / Time.deltaTime;
			foreach (var kart in KartEntity.Karts)
			{
				//CLog.Log("ESTO VALE EL CHECK POINT: " + kart.gameObject.name+ " - "+ ((kart.LapController.Lap) * (kart.LapController.totalCheckPoint + 1)) + " - "+(kart.LapController.totalCheckPoint + 1) + " - " + (kart.LapController.Lap - 1)+" - "+
				//(checkpoints.Length* (kart.LapController.Lap - 1)));
				
					checkPoint = this.checkPoint(kart.LapController.totalCheckPoint + 1);//>= checkpoints.Length+((kart.LapController.Lap-1) * checkpoints.Length))
					{

						//CLog.Log("La distancia es: " + (kart.distance=(1000*(totalCheckPoints - (((kart.LapController.Lap - 1)* checkpoints.Length) + kart.LapController.totalCheckPoint)))+((t.position - kart.transform.position).magnitude)));
						kart.distance = (1000 * (totalCheckPoints - (((kart.LapController.Lap - 1) * checkpoints.Length) + kart.LapController.totalCheckPoint))) + ((checkPoint.position - kart.transform.position).magnitude);
					}
				if (kart.Object.HasInputAuthority)
					checkPointPlayer = checkPoint;


				//kart.LapController.CheckpointIndex;
				//kart.LapController.Lap;
			}

			if (kartsTMP.Count != KartEntity.Karts.Count)
				kartsTMP = KartEntity.Karts;

			kartsTMP.Sort((p1, p2) => p1.distance.CompareTo(p2.distance));
			for (int i = 0; i < kartsTMP.Count; i++)
			{
				kartsTMP[i].position = i + 1;
				if (RoomPlayer.Local)
				{
					if (kartsTMP[i].Controller == RoomPlayer.Local.Kart)
					{
						//CLog.Log("MI POSICION : " + kartsTMP[i].position + " " + kartsTMP[i].gameObject.name);
						GameManager.Instance.setPosition(kartsTMP[i].position-1);
						//ac� va codigo Mandar a imprimir la posicion del player
						//if (Object.HasInputAuthority)
						//{ 
						if (lastCheckPoint == checkPointPlayer)
						{
							if (contador++ > 60) kartsTMP[i].flecha.gameObject.SetActive(true);
							if (checkPointPlayer != null && kartsTMP[i].flecha)
								kartsTMP[i].flecha.LookAt(checkPointPlayer);
						}
						else
						{
							contador = 0;
							kartsTMP[i].flecha.gameObject.SetActive(false);
                        }
						//}

					}
					//else
						//CLog.Log("OTRAS POSICION : " + kartsTMP[i].position + " " + kartsTMP[i].gameObject.name);
				}
				//kart.distance
			}
			lastCheckPoint = checkPointPlayer;

			yield return new WaitForSeconds(.1f);
		}
	}


	System.Collections.IEnumerator radar()
	{
		List<RadarPlayer> listRadarPlayer = new List<RadarPlayer>();

		yield return new WaitForSeconds(5);
		mapaRadar = GameManager.Instance.HUD.mapaRadar;
		Sprite sp = Resources.Load<Sprite>("Prefabs/Track/Radar/" + ResourceManager.instance.tracksDefinitions[GameManager.Instance.TrackId].trackSceneName);
		mapaRadar.GetChild(0).GetComponent<UnityEngine.UI.Image>().overrideSprite = sp;
		mapaRadar.gameObject.SetActive(sp);
		
		foreach (var kart in KartEntity.Karts)
		{
			GameObject t = Instantiate(Resources.Load("Prefabs/Accesories/FlechaRadar") as GameObject);
			t.transform.parent = mapaRadar;
			t.transform.localPosition = Vector3.zero;
			t.transform.localRotation = Quaternion.identity;
			t.GetComponent<RadarPlayer>().target = kart;
			if (RoomPlayer.Local)
			{
				if (kart.Controller == RoomPlayer.Local.Kart)
					t.GetComponent<RadarPlayer>().setColorRed();
			}
			listRadarPlayer.Add(t.GetComponent<RadarPlayer>());
		}
		while (true)
		{
			foreach (var kartRadar in listRadarPlayer)
			{
				kartRadar.position();
			}

			yield return new WaitForSeconds(.1f);

		}



	}

	Transform checkPoint(int _nodo)
	{
		if (_nodo >= checkpoints.Length)
			return checkpoints[0].transform;
		else
			return checkpoints[_nodo].transform;

	}
	private void InitCheckpoints()
	{
		if (checkpointsList != null && checkpointsList.childCount > 0)
		{
			checkpoints = new Checkpoint[checkpointsList.childCount];
			for (int i = 0; i < checkpointsList.childCount; i++)
			{
				checkpoints[i] = checkpointsList.GetChild(i).GetComponent<Checkpoint>();
				if (checkpoints[i] == null)
				{
					CLog.LogWarning("Checkpoint faltante en hijo " + i + " de checkpointsList en pista: " + name);
				}
			}
		}

		if (checkpoints == null || checkpoints.Length == 0)
		{
			CLog.LogError("Track sin checkpoints validos: " + name);
			checkpoints = System.Array.Empty<Checkpoint>();
			return;
		}

		for (int i = 0; i < checkpoints.Length; i++)
		{
			if (checkpoints[i] == null)
				continue;
			checkpoints[i].index = i;
		}
	}

	public bool ControlCamera(Camera cam)
	{
		CLog.Log("++" + "RECCORRIENDO CAMERA "+ cam+" "+cam.name+" "+cam.transform.parent);
		//if (Object.HasStateAuthority) return false;
		if (GameLauncher.instance.isServer) return false;


		cam.transform.position = Vector3.Lerp(
			introTracks[_currentIntroTrack].startPoint.position,
			introTracks[_currentIntroTrack].endPoint.position,
			_introIntervalProgress);

		cam.transform.rotation = Quaternion.Slerp(
			introTracks[_currentIntroTrack].startPoint.rotation,
			introTracks[_currentIntroTrack].endPoint.rotation,
			_introIntervalProgress);

		_introIntervalProgress += Time.deltaTime * introSpeed; 
		if (_introIntervalProgress > 1)
		{
			_introIntervalProgress -= 1;
			_currentIntroTrack++;
			if (_currentIntroTrack == introTracks.Length)
			{
				_currentIntroTrack = 0;
				_introIntervalProgress = 0;
				return false;
			}
		}

		return true;
	}

	public void StartIntro()
	{
		_currentIntroTrack = 0;
		_introIntervalProgress = 0;
		AudioManager.PlayMusic("intro");
		GameManager.GetCameraControl(this);
	}
}
