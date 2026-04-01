using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using FusionExamples.FusionHelpers;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ConnectionStatus
{
    Disconnected,
    Connecting,
    Failed,
    Connected
}

public enum SessionPropertyKey
{
    Bet,
    TrackId,
    Laps,
    MaxLaps,
    Mode,
    ServerIP
}

public enum KeysPlayerPref
{
    accSensibilidad,
    deadZone,
    enableAcc
}



[RequireComponent(typeof(LevelManager))]
public class GameLauncher : MasterScreen, INetworkRunnerCallbacks
{
    public const float MAX_SPEED = 24;
    public const float MAX_ACC = .75f;
    public const float MAX_TURN = 7;
    private const string DisplayPlayersPropertyKey = "DisplayPlayers";



    [SerializeField] private GameManager _gameManagerPrefab;
    [SerializeField] private RoomPlayer _roomPlayerPrefab;
    [SerializeField] private DisconnectUI _disconnectUI;
    //[SerializeField] private ObjectListRoom _listRooms;
    [SerializeField] private ContentListRooms _listRooms;
    [SerializeField] private LobbyUI _lobbyUI;


    public static ConnectionStatus ConnectionStatus = ConnectionStatus.Disconnected;

    public static GameLauncher instance;

    private GameMode _gameMode;
    private NetworkRunner _runner;
    private FusionObjectPoolRoot _pool;
    private LevelManager _levelManager;
    public UIScreen mainScreen;
    public UIScreen gameModesScreen;
    public PopUpManager popUp;
    public LittlePopUpManager SmallPopUp;
    public ProfileInfo popUpProfileInfo;
    public ClanExternoInfo popUpClanInfo;
    public DisplayDataUI display;
    public string IP_server = "181.30.95.162";
    public bool modeServerDedicado;
    public bool serverBet;
    public int minPlayers;
    public int maxPlayers;
    public int mode;
    public GameObject background;

    public string idServer;
    public int minBetTEL = 100;
    public static float timeToRespawn = 5f;//tiempo para respawnear
    internal bool isServer;
    public TMPro.TextMeshProUGUI stats;
    [Header("Safe Free Bots")]
    [SerializeField] private bool enableSafeFreeLobbyBots = true;
    [SerializeField] private int maxSafeFreeLobbyBots = 5;
    [SerializeField] private string botPlayfabPrefix = "BOT_";
    [Header("Free Lobby Display Players")]
    [SerializeField] private bool enableFreeLobbyDisplayPlayers = true;
    [SerializeField] private int freeLobbyDisplayMin = 1;
    [SerializeField] private int freeLobbyDisplayMax = 3;
    [SerializeField] private float freeLobbyDisplayStepSeconds = 10f;
    [SerializeField] private int freeLobbyDisplaySoftCap = 4;
    private int _botSequence = 0;
    private string _lastSafeBotPresetKey = string.Empty;
    private int _botNameCursor = 0;
    private Coroutine _freeLobbyDisplayPlayersRoutine;
    private float _freeLobbyDisplayStartedAt = -1f;
    private int _freeLobbyInitialDisplay = 0;
    private int _lastPublishedDisplayPlayers = int.MinValue;
    private static readonly int[] BotNameRecycleStarts = { 0, 9, 19, 29, 39, 49, 59, 69, 79, 89, 99, 109, 119, 129, 139, 149 };
    private static readonly int[] BotNameRecycleSteps = { 3, 7, 11, 13, 17, 19, 21, 23, 27, 29, 31 };
    private static readonly string[] BotNamePool = BuildBotNamePool();
    private static readonly HashSet<string> BotAllowedKartNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "NFTURBO",
        "BLOCKCHAININVADER",
        "BITCRUISER",
        "TOKENHUNTER",
        "SPACEBLOKER",
        "DELOREANHASH",
    };
    private static readonly HashSet<string> BotBlockedDriverNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "KARLA",
        "EDU",
        "CHRIS",
    };
    private static readonly HashSet<int> BotBlockedDriverIds = new HashSet<int> { 700, 800, 900 };


    [SerializeField] private List<itemList> ListaSalas;

    Dictionary<string, SessionProperty> sessionProperties = new Dictionary<string, SessionProperty>();

    private bool CanUseFreeLobbyDisplayPlayers()
    {
        return enableFreeLobbyDisplayPlayers &&
               _runner != null &&
               _runner.IsRunning &&
               _runner.IsServer &&
               !serverBet;
    }

    private int GetVisibleMaxPlayersForList()
    {
        int rawMaxPlayers = maxPlayers > 0 ? maxPlayers : ServerInfo.MaxUsers;
        return Mathf.Max(0, rawMaxPlayers - 1);
    }

    private int GetCurrentLobbyVisiblePlayers()
    {
        return Mathf.Clamp(RoomPlayer.Players.Count, 0, GetVisibleMaxPlayersForList());
    }

    private void StartFreeLobbyDisplayPlayersRoutine()
    {
        StopFreeLobbyDisplayPlayersRoutine(true);

        if (!CanUseFreeLobbyDisplayPlayers())
            return;

        int min = Mathf.Max(0, freeLobbyDisplayMin);
        int max = Mathf.Max(min, freeLobbyDisplayMax);
        _freeLobbyInitialDisplay = UnityEngine.Random.Range(min, max + 1);
        _freeLobbyDisplayStartedAt = Time.realtimeSinceStartup;
        _lastPublishedDisplayPlayers = int.MinValue;

        PublishFreeLobbyDisplayPlayersNow();
        _freeLobbyDisplayPlayersRoutine = StartCoroutine(FreeLobbyDisplayPlayersRoutine());
    }

    private void StopFreeLobbyDisplayPlayersRoutine(bool clearProperty)
    {
        if (_freeLobbyDisplayPlayersRoutine != null)
        {
            StopCoroutine(_freeLobbyDisplayPlayersRoutine);
            _freeLobbyDisplayPlayersRoutine = null;
        }

        _freeLobbyDisplayStartedAt = -1f;
        _freeLobbyInitialDisplay = 0;
        _lastPublishedDisplayPlayers = int.MinValue;

        if (clearProperty)
            RemoveCustomSessionProperty(DisplayPlayersPropertyKey);
    }

    private System.Collections.IEnumerator FreeLobbyDisplayPlayersRoutine()
    {
        while (CanUseFreeLobbyDisplayPlayers())
        {
            PublishFreeLobbyDisplayPlayersNow();
            yield return new WaitForSeconds(1f);
        }

        _freeLobbyDisplayPlayersRoutine = null;
    }

    private int ComputeFreeLobbyDisplayPlayers()
    {
        int visibleMax = GetVisibleMaxPlayersForList();
        int actualVisible = GetCurrentLobbyVisiblePlayers();
        int softCap = Mathf.Clamp(freeLobbyDisplaySoftCap, 0, visibleMax);

        if (softCap <= 0)
            return actualVisible;

        int initial = Mathf.Clamp(_freeLobbyInitialDisplay, 0, softCap);
        int steps = 0;
        if (freeLobbyDisplayStepSeconds > 0f && _freeLobbyDisplayStartedAt >= 0f)
        {
            float elapsed = Mathf.Max(0f, Time.realtimeSinceStartup - _freeLobbyDisplayStartedAt);
            steps = Mathf.FloorToInt(elapsed / freeLobbyDisplayStepSeconds);
        }

        int warmupVisible = Mathf.Clamp(initial + steps, 0, softCap);
        return Mathf.Clamp(Mathf.Max(actualVisible, warmupVisible), 0, visibleMax);
    }

    private void PublishFreeLobbyDisplayPlayersNow()
    {
        if (!CanUseFreeLobbyDisplayPlayers())
        {
            RemoveCustomSessionProperty(DisplayPlayersPropertyKey);
            return;
        }

        int displayPlayers = ComputeFreeLobbyDisplayPlayers();
        if (_lastPublishedDisplayPlayers == displayPlayers)
            return;

        _lastPublishedDisplayPlayers = displayPlayers;
        SetCustomSessionProperty(DisplayPlayersPropertyKey, displayPlayers);
    }

    private void SetCustomSessionProperty(string key, SessionProperty value)
    {
        if (string.IsNullOrEmpty(key))
            return;

        if (sessionProperties.ContainsKey(key))
            sessionProperties[key] = value;
        else
            sessionProperties.Add(key, value);

        if (_runner != null &&
            _runner.SessionInfo.IsValid &&
            _runner.SessionInfo.IsVisible)
        {
            _runner.SessionInfo.UpdateCustomProperties(sessionProperties);
        }
    }

    private void RemoveCustomSessionProperty(string key)
    {
        if (string.IsNullOrEmpty(key) || !sessionProperties.ContainsKey(key))
            return;

        sessionProperties.Remove(key);

        if (_runner != null &&
            _runner.SessionInfo.IsValid &&
            _runner.SessionInfo.IsVisible)
        {
            _runner.SessionInfo.UpdateCustomProperties(sessionProperties);
        }
    }

    private static string[] BuildBotNamePool()
    {
        return new[]
        {
            "carlos_001","troyano","galaxor","andres_777","neon_404","lince_rojo","mila_909","kraken77","orion_x",
            "deltafox","rayoazul","pixelnegro","hadesrun","zenit_011","bruno_008","dante_420","luna_311","kora_019",
            "falco_303","noxfire","ultra_mate","ramiro_026","xenon_707","mauro_131","temporal8","fercho_212","boreal_52",
            "triton_01","sombra_9","vector_18","horizon_6","valen_001","mateo_014","tiago_019","nuria_808","agata_212",
            "julia_111","natalia7","santino5","amparo_27","fiero_013","drakon_88","leviatan3","ryzen_104","kartazo_77",
            "mecaniko","blitz_501","astrolux","sirio_73","vortex_16","cronos_91","marte_002","atlas_021","mercurio4",
            "nublado_7","aurora_65","iguana_55","pantera13","jaguar_27","cenit_011","fenix_100","obelisco9","quantum_9",
            "polaris_2","saturno8","artemis_3","helios_66","aegis_404","urbanox_9","darkmint7","turin_808","golem_018",
            "rubicon_7","obelix_31","nomada_16","ragnar_07","mithra_27","helix_19","sirius_81","medusa_29","gandor_41",
            "bastion_6","clank_005","gatsu_120","nexus_808","zafiro_99","pluton_41","argo_314","greko_019","vandal_37",
            "nilo_707","sultan_16","osiris_35","quijote9","vulcano4","xirax_22","chimera_1","calypso8","delta_902",
            "omega_117","rurik_004","nardo_300","dorian_44","legion_77","hazel_019","olimpo_5","selene_77","trance_20",
            "saber_909","rexor_311","cobra_502","titan_144","xantos_86","kaiser_12","faktor_17","narval_08","juno_600",
            "apollo_24","gaia_008","terran_90","cometa_12","lyra_300","zonda_77","vento_110","gizmo_41","bunker_66",
            "arkham_19","frosty_8","sumatra_3","ciclon_90","tercio_52","drifter_1","ramjet_44","turbina9","forastero5",
            "pelicano7","corsario8","tornado_7","pegaso_11","astilla_3","cobalto_9","ambar_051","lazaro_35","vasco_18",
            "brava_204","navarro_9","sergio_17","ruben_021","fabian_044","julian_15","ricardo_5","alvaro_90","ismael_12",
            "samuel_71","marcos_08","daniel_31","elias_014","hugo_117","jairo_73","josue_404","saul_220","ulises_21",
            "joaquin8","damian_34","nicolas_2","franco_19","lucas_777","adrian_18","emanuel_52","ignacio_7","benjamin_3",
            "martin_64","agustin_11","valeria_23","camila_17","isabel_52","romina_19","sofia_44","martina_8","paula_14",
            "brenda_31","lorena_06","daniela_15","karina_10","tatiana_5","azucena_7","adela_24","maite_16","irene_71",
            "carina_29","micaela_9","ximena_12","celeste_77","violeta_33","nayla_404","samira_90","fiorella_19","renata_52",
            "alondra_7","aryana_18","brisa_22","corina_17","dalila_8","estela_27","flavia_13","ginebra_4","helena_31",
            "ivanna_5","janet_66","kiara_20","ludmila_14","melina_11","noelia_6","olivia_88","priscila_9","raquel_12",
            "tamara_17","ursula_4","veronica_31","wanda_5","yolanda_19","zulema_8"
        };
    }


    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
            return;

        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(serverDedicado());





    }

    System.Collections.IEnumerator serverDedicado()
    {

        if (Application.platform == RuntimePlatform.Android)
        {
            Screen.SetResolution(1280, 720, true);
            //Application.targetFrameRate = 60; 
            Application.targetFrameRate = -1;
            yield break;
        }


        //1             2      3                4              5           6   7        8 9 10  11             12
        //test@test.com 123456 ServidorDedicado -dedicated_ON -apuesta_ON 100 Server_01 4 8	0	"181.30.95.162" sa
#if UNITY_EDITOR
        //string[] args = { "-","test@test.com", "12345678", "ServidorDedicado", "-dedicated_ON", "-apuesta_ON", "100", "Server_01","1","8","0","180.1.2.4","sa"};
        string[] args = System.Environment.GetCommandLineArgs();
#else
		string[] args = System.Environment.GetCommandLineArgs();   
#endif
        for (int i = 0; i < args.Length; i++) 
        {
            Debug.Log("+ ARGUMENTOS: " + i + ": " + args[i]);
            /*
			if (args[i] == "-folderInput")
			{
				input = args[i + 1];
			}*/
        }
        if (args.Length == 13)
        {
            Screen.SetResolution(800, 480, FullScreenMode.Windowed);
            modeServerDedicado = args[4].Contains("-dedicated_ON");
            _gameMode = modeServerDedicado ? GameMode.Server : GameMode.Host;
            serverBet = args[5].Contains("-apuesta_ON");
            minBetTEL = serverBet ? int.Parse(args[6]) : 0;
            idServer = args[7];
            ServerInfo.MaxUsers = int.Parse(args[9]);
            //IP_server = args[11];
            ServerInfo.LobbyName = args[7];// "neraverse" + Random.Range(1, 100000);
            minPlayers = int.Parse(args[8]);
            ServerInfo.MaxUsers = maxPlayers = int.Parse(args[9]);
            ServerInfo.GameMode = mode = int.Parse(args[10]);//0 - > Race,  1 -> Deathmatch 
            Fusion.Photon.Realtime.PhotonAppSettings.Instance.AppSettings.FixedRegion = args[12];//region
            yield return new WaitWhile(() => PlayfabManager.instance == null);

            PlayfabManager.instance.Login(args[1], args[2]);

            yield return new WaitWhile(() => !PlayFab.PlayFabClientAPI.IsClientLoggedIn());


            /////////////////////////////////////////////////////////////// CONSULTAR IP
            UnityEngine.Networking.UnityWebRequest consulta = UnityEngine.Networking.UnityWebRequest.Get("http://checkip.dyndns.org");
            yield return consulta.SendWebRequest();
            
            while (!consulta.isDone)
                yield return null;

            if (consulta.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError)
            {
                CLog.Log(consulta.error + " No se reconoce la IP");
                IP_server = "127.0.0.1";

            }
            else
            {
                // Show results as text
                CLog.Log(consulta.downloadHandler.text);

                // Or retrieve results as binary data
                var myExtIP = consulta.downloadHandler.text;
                myExtIP = myExtIP.Substring(myExtIP.IndexOf(":") + 1);
                myExtIP = myExtIP.Substring(0, myExtIP.IndexOf("<"));
                CLog.Log("Mi IP es: " + myExtIP);
                IP_server = myExtIP.Trim();

            }
            /////////////////////////////////////////////////////////////////////
            CLog.Log("+ Cargando server");

            yield return new WaitForSeconds(10);

            _lobbyUI.GetComponent<UIScreen>().FocusScreen(_lobbyUI.GetComponent<UIScreen>());
            JoinOrCreateLobby();
            yield return new WaitForSeconds(10);
            for (int i = 0; i < args.Length; i++)
            {
                CLog.Log("+ ARGUMENTOS: " + i + ": " + args[i]);
                /*
				if (args[i] == "-folderInput")
				{
					input = args[i + 1];
				}*/
            }


            while (true)
            {
                yield return new WaitForSeconds(1);

                if (GameManager.Instance == null && _runner &&
                    _runner.IsRunning && _runner.SessionInfo.IsValid)
                {
                    _runner.Spawn(_gameManagerPrefab, Vector3.zero, Quaternion.identity);
                    break;
                }
            }
            yield return new WaitForSeconds(10);

            while (true)
            {


                if (_runner != null &&
                    _runner.IsRunning && _runner.SessionInfo.IsValid)
                {
                    yield return new WaitForSeconds(10);

                }
                else
                {
                    CLog.Log("Log Status: " + ConnectionStatus);


                    if (_runner != null)
                        CLog.Log("Log: " + _runner.IsConnectedToServer + " - " + _runner.IsRunning + " - " + _runner.SessionInfo.IsValid);
                    else
                        CLog.Log("Log:  Runner Null");


                    if (Application.platform == RuntimePlatform.WindowsServer)
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                    else
                        Application.Quit(-1);






                    yield return new WaitForSeconds(1);
                }



            }


        }
        else
#if UNITY_ANDROID
            Screen.SetResolution(1280, 720, true);
        //Application.targetFrameRate = 60; 
        Application.targetFrameRate = -1;
        yield break;
#endif
        Screen.SetResolution(1920, 1080, true);


    }

    internal void loadLobbyScene()
    {
        SceneManager.LoadScene(LevelManager.LOBBY_SCENE);
    }

    //public
    private void Start()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        QualitySettings.vSyncCount = 1;
        _levelManager = GetComponent<LevelManager>();
        //SceneManager.LoadScene(LevelManager.LOBBY_SCENE);
        popUp.starPopUp();
        SmallPopUp.starPopUp();
        if (popUpProfileInfo) popUpProfileInfo.startPoup();
        popUpClanInfo.startPoup();
        display.startDisplay();
        CheckInternetConnection();
    }

    private void CheckInternetConnection()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            CLog.Log("Ops, parece que no tienes coneccion a internet !");

            PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_ERROR),
                                            TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_ERRORNET), IconosPopUp.error, false);
        }
    }


    public void resetSesion()
    {
        Debug.Log("BORRANDO: " + _runner);
        Debug.Log("BORRANDO: " + _runner.transform.childCount);
        if (_runner.transform.childCount > 0)
        {
            foreach (Transform t in _runner.transform)
            {
                Debug.Log("BORRANDO: " + t);
                Destroy(t.gameObject);
            }
        }
    }
    public NetworkRunner getRunner()
    {
        return _runner;
    }

    public bool TrySpawnSafeFreeLobbyBot()
    {
        if (!enableSafeFreeLobbyBots)
            return false;
        if (_runner == null || !_runner.IsRunning || !_runner.IsServer)
            return false;
        if (serverBet)
            return false;
        if (_roomPlayerPrefab == null || ResourceManager.Instance == null)
            return false;

        int maxSlots = maxPlayers > 0 ? maxPlayers : ServerInfo.MaxUsers;
        if (maxSlots <= 0 || RoomPlayer.Players.Count >= maxSlots)
            return false;

        int currentBots = CountSafeBots();
        int botLimit = Mathf.Max(2, maxSafeFreeLobbyBots);
        if (currentBots >= botLimit)
            return false;

        if (!TryGetHumanPreset(out int selectedKartId, out int selectedDriverId))
        {
            CLog.Log("[BOT-SAFE] Waiting human preset (kart/char) before spawning bots");
            return false;
        }

        var roomPlayer = _runner.Spawn(_roomPlayerPrefab, Vector3.zero, Quaternion.identity);
        if (roomPlayer == null || roomPlayer.Object == null || !roomPlayer.Object.IsValid)
            return false;

        _botSequence++;
        roomPlayer.GameState = RoomPlayer.EGameState.Lobby;
        roomPlayer.IsReady = true;
        roomPlayer.Username = GetNextBotDisplayName();
        roomPlayer.playFabID = botPlayfabPrefix + idServer + "_" + _botSequence.ToString("000");
        roomPlayer.KartId = selectedKartId;
        roomPlayer.CharId = selectedDriverId;

        CLog.Log("[BOT-SAFE] Spawn bot " + roomPlayer.Username);
        PublishFreeLobbyDisplayPlayersNow();
        return true;
    }

    private bool TryGetHumanPreset(out int kartId, out int charId)
    {
        kartId = 0;
        charId = 0;
        var approvedKartIds = GetApprovedKartsFromCatalog();
        var approvedCharIds = GetApprovedCharsFromCatalog();

        if (approvedKartIds.Count == 0 || approvedCharIds.Count == 0)
            AddApprovedFallbackFromHumans(approvedKartIds, approvedCharIds);

        if (approvedKartIds.Count == 0 || approvedCharIds.Count == 0)
            return false;

        var used = new HashSet<string>(StringComparer.Ordinal);
        for (int i = 0; i < RoomPlayer.Players.Count; i++)
        {
            var rp = RoomPlayer.Players[i];
            if (rp == null)
                continue;

            try
            {
                if (rp.KartId > 0 && rp.CharId > 0)
                    used.Add(rp.KartId + ":" + rp.CharId);
            }
            catch (InvalidOperationException) { }
        }

        var allCombos = new List<(int kart, int chr)>();
        for (int i = 0; i < approvedKartIds.Count; i++)
        {
            for (int j = 0; j < approvedCharIds.Count; j++)
            {
                allCombos.Add((approvedKartIds[i], approvedCharIds[j]));
            }
        }

        if (allCombos.Count == 0)
            return false;

        var availableUnique = new List<(int kart, int chr)>(allCombos.Count);
        for (int i = 0; i < allCombos.Count; i++)
        {
            string key = allCombos[i].kart + ":" + allCombos[i].chr;
            if (used.Contains(key))
                continue;
            availableUnique.Add(allCombos[i]);
        }

        // Prefer unique bot presets first; if exhausted, allow reuse so lobby can still fill with >1 bot.
        var source = availableUnique.Count > 0 ? availableUnique : allCombos;

        if (source.Count > 1 && !string.IsNullOrEmpty(_lastSafeBotPresetKey))
        {
            source.RemoveAll(x => (x.kart + ":" + x.chr) == _lastSafeBotPresetKey);
            if (source.Count == 0)
                source = availableUnique.Count > 0 ? availableUnique : allCombos;
        }

        if (source.Count == 0)
            return false;

        var pick = source[UnityEngine.Random.Range(0, source.Count)];
        kartId = pick.kart;
        charId = pick.chr;
        _lastSafeBotPresetKey = kartId + ":" + charId;
        return true;
    }

    private List<int> GetApprovedKartsFromCatalog()
    {
        var ids = new List<int>();
        if (ResourceManager.Instance == null)
            return ids;

        try
        {
            var catalog = Catalogo.getCatalogo(Catalogos.Karts);
            if (catalog == null)
                return ids;

            for (int i = 0; i < catalog.Count; i++)
            {
                var item = catalog[i];
                if (item == null || string.IsNullOrEmpty(item.Id))
                    continue;
                if (!int.TryParse(item.Id, out int id))
                    continue;
                if (ResourceManager.Instance.getKart(id) == null)
                    continue;
                if (!IsKartAllowedForBots(item, id))
                    continue;
                if (!ids.Contains(id))
                    ids.Add(id);
            }
        }
        catch (Exception) { }

        return ids;
    }

    private List<int> GetApprovedCharsFromCatalog()
    {
        var ids = new List<int>();
        if (ResourceManager.Instance == null)
            return ids;

        try
        {
            var catalog = Catalogo.getCatalogo(Catalogos.Characters);
            if (catalog == null)
                return ids;

            for (int i = 0; i < catalog.Count; i++)
            {
                var item = catalog[i];
                if (item == null || string.IsNullOrEmpty(item.Id))
                    continue;
                if (!int.TryParse(item.Id, out int id))
                    continue;
                if (ResourceManager.Instance.getChar(id) == null)
                    continue;
                if (!IsDriverAllowedForBots(item, id))
                    continue;
                if (!ids.Contains(id))
                    ids.Add(id);
            }
        }
        catch (Exception) { }

        return ids;
    }

    private void AddApprovedFallbackFromHumans(List<int> approvedKartIds, List<int> approvedCharIds)
    {
        if (approvedKartIds == null || approvedCharIds == null || ResourceManager.Instance == null)
            return;

        for (int i = 0; i < RoomPlayer.Players.Count; i++)
        {
            var rp = RoomPlayer.Players[i];
            if (rp == null || IsSafeBot(rp))
                continue;

            int k = 0;
            int c = 0;
            try
            {
                k = rp.KartId;
                c = rp.CharId;
            }
            catch (InvalidOperationException)
            {
                continue;
            }

            if (k > 0 && c > 0 &&
                IsKartAllowedForBotsById(k) &&
                IsDriverAllowedForBotsById(c) &&
                ResourceManager.Instance.getKart(k) != null &&
                ResourceManager.Instance.getChar(c) != null)
            {
                if (!approvedKartIds.Contains(k))
                    approvedKartIds.Add(k);
                if (!approvedCharIds.Contains(c))
                    approvedCharIds.Add(c);
            }
        }
    }

    private bool IsKartAllowedForBots(ItemBase item, int id)
    {
        if (!IsKartAllowedForBotsById(id))
            return false;

        string display = NormalizeBotLabel(item != null ? item.DisplayName : string.Empty);
        return BotAllowedKartNames.Contains(display);
    }

    private bool IsKartAllowedForBotsById(int id)
    {
        if (ResourceManager.Instance == null)
            return false;
        if (ResourceManager.Instance.getKart(id) == null)
            return false;

        try
        {
            var catalog = Catalogo.getCatalogo(Catalogos.Karts);
            if (catalog == null)
                return false;

            for (int i = 0; i < catalog.Count; i++)
            {
                var item = catalog[i];
                if (item == null || string.IsNullOrEmpty(item.Id))
                    continue;
                if (!int.TryParse(item.Id, out int itemId))
                    continue;
                if (itemId != id)
                    continue;

                return BotAllowedKartNames.Contains(NormalizeBotLabel(item.DisplayName));
            }
        }
        catch (Exception) { }

        return false;
    }

    private bool IsDriverAllowedForBots(ItemBase item, int id)
    {
        if (!IsDriverAllowedForBotsById(id))
            return false;

        string display = NormalizeBotLabel(item != null ? item.DisplayName : string.Empty);
        if (BotBlockedDriverNames.Contains(display))
            return false;

        return true;
    }

    private bool IsDriverAllowedForBotsById(int id)
    {
        if (BotBlockedDriverIds.Contains(id))
            return false;
        if (ResourceManager.Instance == null)
            return false;

        var dd = ResourceManager.Instance.getChar(id);
        if (dd == null)
            return false;

        string driverName = NormalizeBotLabel(dd.nameDriver);
        if (BotBlockedDriverNames.Contains(driverName))
            return false;

        return true;
    }

    private static string NormalizeBotLabel(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        char[] buffer = new char[value.Length];
        int p = 0;
        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            if (c == ' ' || c == '_' || c == '-')
                continue;
            buffer[p++] = char.ToUpperInvariant(c);
        }
        return new string(buffer, 0, p);
    }

    private string GetNextBotDisplayName()
    {
        if (BotNamePool == null || BotNamePool.Length == 0)
            return "BOT_" + _botSequence.ToString("00");

        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < RoomPlayer.Players.Count; i++)
        {
            var rp = RoomPlayer.Players[i];
            if (rp == null)
                continue;
            if (!string.IsNullOrEmpty(rp.Username))
                used.Add(rp.Username);
        }

        int baseSeq = _botNameCursor;
        int maxAttempts = BotNamePool.Length + 32;
        for (int i = 0; i < maxAttempts; i++)
        {
            int index = GetBotNameIndex(baseSeq + i);
            string candidate = BotNamePool[index];
            if (used.Contains(candidate))
                continue;

            _botNameCursor = baseSeq + i + 1;
            return candidate;
        }

        string fallback = BotNamePool[GetBotNameIndex(baseSeq)];
        _botNameCursor = baseSeq + 1;
        return fallback + "_" + ((_botSequence % 90) + 10).ToString();
    }

    private int GetBotNameIndex(int sequence)
    {
        int poolLen = BotNamePool.Length;
        if (poolLen <= 0)
            return 0;

        if (sequence < poolLen)
            return sequence;

        int recycle = sequence - poolLen;
        int round = recycle / poolLen;
        int pos = recycle % poolLen;

        int start = BotNameRecycleStarts[round % BotNameRecycleStarts.Length] % poolLen;
        int step = BotNameRecycleSteps[round % BotNameRecycleSteps.Length];
        return (start + (pos * step)) % poolLen;
    }

    private int CountSafeBots()
    {
        int bots = 0;
        for (int i = 0; i < RoomPlayer.Players.Count; i++)
        {
            var rp = RoomPlayer.Players[i];
            if (rp == null)
                continue;
            if (IsSafeBot(rp))
                bots++;
        }
        return bots;
    }

    private int CountHumans()
    {
        int humans = 0;
        for (int i = 0; i < RoomPlayer.Players.Count; i++)
        {
            var rp = RoomPlayer.Players[i];
            if (rp == null)
                continue;
            if (!IsSafeBot(rp))
                humans++;
        }
        return humans;
    }

    private bool IsSafeBot(RoomPlayer rp)
    {
        if (rp == null)
            return false;
        try
        {
            return !string.IsNullOrEmpty(rp.playFabID) &&
                   rp.playFabID.StartsWith(botPlayfabPrefix, StringComparison.OrdinalIgnoreCase);
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    private void DespawnSafeBots()
    {
        if (_runner == null || !_runner.IsRunning || !_runner.IsServer)
            return;

        var all = new List<RoomPlayer>(RoomPlayer.Players);
        for (int i = 0; i < all.Count; i++)
        {
            var rp = all[i];
            if (!IsSafeBot(rp))
                continue;

            try
            {
                if (rp.Kart != null && rp.Kart.Object != null && rp.Kart.Object.IsValid)
                    _runner.Despawn(rp.Kart.Object);
            }
            catch (InvalidOperationException) { }

            if (rp.Object != null && rp.Object.IsValid)
                _runner.Despawn(rp.Object);
        }

        PublishFreeLobbyDisplayPlayersNow();
    }
    public void setOpenNet(bool _value)
    {
        if (ConnectionStatus == ConnectionStatus.Connected)
        {
            //return;
            if (_runner != null)
            {
                _runner.SessionInfo.IsOpen = _value;
                //	if(_value)
                //_runner.CurrentScene
            }
        }
        //_runner.sce

    }
    public void SetCreateLobby() => _gameMode = modeServerDedicado ? GameMode.Server : GameMode.Host;//-GameMode.Host;
    public void SetJoinLobby() => _gameMode = GameMode.Client;

    //Se llama esta funcion desde el boton Join para comenzar la conexion y descarga de la lista
    public void SetJoinLobby2()
    {
        _ = JoinLobby();
    }

    public async void JoinOrCreateLobby()
    {
        
        SetConnectionStatus(ConnectionStatus.Connecting);
        
        if (_runner != null)
            LeaveSession();

        GameObject go = new GameObject("Session");
        DontDestroyOnLoad(go);

        _runner = go.AddComponent<NetworkRunner>();

        if (modeServerDedicado)
            _runner.ProvideInput = false;
        else _runner.ProvideInput = _gameMode != GameMode.Server;


        _runner.AddCallbacks(this);


        _pool = go.AddComponent<FusionObjectPoolRoot>();


        //GameType
        setSessionProperties(SessionPropertyKey.Bet, modeServerDedicado ? minBetTEL : 0);
        setSessionProperties(SessionPropertyKey.TrackId, ServerInfo.TrackId = ResourceManager.instance.getTrackID(ResourceManager.instance.gameTypes[ServerInfo.GameMode].modeName));
        setSessionProperties(SessionPropertyKey.Mode, ServerInfo.GameMode);
        setSessionProperties(SessionPropertyKey.Laps, 0);
        setSessionProperties(SessionPropertyKey.MaxLaps, 0);
        setSessionProperties(SessionPropertyKey.ServerIP, IP_server);




        Debug.Log($"Created gameobject {go.name} - starting game " + _pool + " " + _pool.name + " " + _gameMode);
        //return;
        await _runner.StartGame(new StartGameArgs
        {
            GameMode = _gameMode,
            //-SessionName = _gameMode == GameMode.Host ? ServerInfo.LobbyName : ClientInfo.LobbyName,
            SessionName = ServerInfo.LobbyName = "ServerDed_" + UnityEngine.Random.Range(1000, 10000),
            //SessionName = _gameMode == GameMode.Host ? ServerInfo.LobbyName : ObjectListRoom.instance.RoomSelectedObjet,
            ObjectPool = _pool,
            SceneManager = _levelManager,
            PlayerCount = ServerInfo.MaxUsers,
            CustomLobbyName = "MyCustomLobby",
            DisableClientSessionCreation = true,
            SessionProperties = sessionProperties,

        });

        if (serverBet)
        {
            StopFreeLobbyDisplayPlayersRoutine(true);
            StartCoroutine(devolver(false));
        }
        else
        {
            StartFreeLobbyDisplayPlayersRoutine();
        }
        //_runner.SessionInfo.pr
        
        
        //Debug.Log("Crear Server: " + _runner.IsServer + " " + _runner.IsClient + " " + _runner.IsPlayer);
        Debug.Log("Server: " + _runner.SessionInfo.Name +" Region: " + _runner.SessionInfo.Region);


        //_runner.SessionInfo.p = "PATITAS"; ;
        //_runner.SessionInfo. = "PATITAS"; ;
        // target as NetworkRunner;
        // _ = JoinLobby(_runner);
        //_runner.SessionInfo.IsOpen = false;
        //		StartCoroutine(time());

    }


    public void JoinOrCreateLobbyOk()
    {


    }

    public void setSessionProperties(SessionPropertyKey _key, SessionProperty value)
    {

        if (sessionProperties.ContainsKey(_key.ToString()))
        {
            CLog.Log("Modifico valor " + _key + " - " + value.ToString() + " - " + value.PropertyValue);
            sessionProperties[_key.ToString()] = value;
        }
        else
        {
            CLog.Log("Agrego valor " + _key + " - " + value.ToString() + " - " + value.PropertyValue);
            sessionProperties.Add(_key.ToString(), value);
        }


        if (_runner != null &&
            _runner.SessionInfo.IsValid &&
            _runner.SessionInfo.IsVisible)//ConnectionStatus == ConnectionStatus.Connected)
        {
            _runner.SessionInfo.UpdateCustomProperties(sessionProperties);

        }

        //prop["bet"] = (int)10;
    }

    private void SetConnectionStatus(ConnectionStatus status)
    {
        Debug.Log($"Setting connection status to {status}");

        ConnectionStatus = status;


        if (!Application.isPlaying)
            return;

        if (status == ConnectionStatus.Disconnected || status == ConnectionStatus.Failed)
        {

            CLog.Log(SceneManager.GetActiveScene().name + " SCENE ACTIVE");
            if (!SceneManager.GetActiveScene().name.Contains("Lobby"))
                SceneManager.LoadScene(LevelManager.LOBBY_SCENE);
            //----
            //UIScreen.BackToInitial();
            mainScreen.BackTo(mainScreen);
        }
    }

    void loading()
    {
        LevelManager.Instance.fader.FadeOut();
    }
    public void LeaveSession()
    {
        if (ConnectionStatus == ConnectionStatus.Connected)
        {
            if (LevelManager.Instance)
            {
                LevelManager.Instance.fader.gameObject.SetActive(true);
                LevelManager.Instance.fader.FadeIn();
                Invoke("loading", 1);

            }
        }
        CLog.Log("CONEXION VALE: " + ConnectionStatus + " " + _runner);
        if (_runner != null)
        {

            _runner.Shutdown();
            _lobbyUI.pararContador();
            //_runner = null;


        }
        else
        {
            if (ConnectionStatus == ConnectionStatus.Disconnected)
                mainScreen.BackTo(mainScreen);
            else
                SetConnectionStatus(ConnectionStatus.Disconnected);
        }

        //PlayfabClientCurrency.addCurrency();
        //TELPoints.instance.ChangeTel(+1); //revisar si aquí funciona de manera correcta y no se puede hacer la trampa de TELS infinitos
    }

    public void LeaveSessionGame(int time)
    {
        PlayfabManager.instance.loadingScreen.GetComponent<LoadingPointsAnim>().setLoading("Cargando", 2, time);

        SceneManager.LoadScene(LevelManager.LOBBY_SCENE); //SceneManager.LoadScene(0);

        Invoke("leave", 2);
        /*
		mainScreen.BackTo(gameModesScreen);
		SetConnectionStatus(ConnectionStatus.Disconnected);
		*/
        CLog.Log("CONEXION VALE: " + ConnectionStatus + " " + _runner);
        /*if (_runner != null)
		{
			_runner.Shutdown();
			_lobbyUI.pararContador();
			//_runner = null;


		}*//*
		else
		{
			if (ConnectionStatus == ConnectionStatus.Disconnected)
				mainScreen.BackTo(gameModesScreen);
			else
				SetConnectionStatus(ConnectionStatus.Disconnected);

		}*/

        //PlayfabClientCurrency.addCurrency();
        //TELPoints.instance.ChangeTel(+1); //revisar si aquí funciona de manera correcta y no se puede hacer la trampa de TELS infinitos
    }
    public void leave()
    {
        LeaveSession();
        //PlayfabManager.instance.loadingScreen.GetComponent<LoadingPointsAnim>().hide();

    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server");
        SetConnectionStatus(ConnectionStatus.Connected);


        Debug.Log($"Entro {ContentListRooms.instance.sessionSelect.bet}");

        /*if (master)
		{
			master = false;
			SessionProperty sp;
			ContentListRooms.instance.sessionSelect.session.Properties.TryGetValue(SessionPropertyKey.Bet.ToString(), out sp);
			int bet = 0;
			if (sp != null)
			{
				CLog.Log("Prop: " + SessionPropertyKey.Bet + ": " + sp.PropertyValue.ToString());
				bet = int.Parse(sp.PropertyValue.ToString());
			}

			//if (ContentListRooms.instance.sessionSelect.session.PlayerCount==1&&ContentListRooms.instance.sessionSelect.bet >= 100)
			if (ContentListRooms.instance.sessionSelect.session.PlayerCount==1 && bet== minBetTEL)			
			{
				//setSessionProperties(SessionPropertyKey.Bet, PopUpManager._instance.getApuesta());
				StartCoroutine(setBet(true, 0));
			}
			else
			{
				StartCoroutine(setBet(true, bet));
				//mainScreen.BackTo(gameModesScreen);
			}
		}
		*/
    }


    /*System.Collections.IEnumerator setBet(bool _ok, int bet)
    {
		if (_ok)
		{
			yield return new WaitWhile(() => GameManager.Instance == null);
			GameManager.Instance.RPC_setDataLooby(SessionPropertyKey.Bet, PopUpManager._instance.getApuesta());
			yield break;
		}

		PopUpManager._instance.setPopUp("Error", "Ya establecieron un monto a esta partida" + bet + " TEL", null, false);
		yield return new WaitWhile(() => PopUpManager._instance.popUpState == PopUpStates.None);
		LeaveSession();


	}*/
    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        CLog.Log("Disconnected from server");
        LeaveSession();
        SetConnectionStatus(ConnectionStatus.Disconnected);
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        if (runner.CurrentScene > 0 && false)
        {
            CLog.LogWarning($"Refused connection requested by {request.RemoteAddress}");
            request.Refuse();
        }
        else
            request.Accept();
    }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

        CLog.Log($"Connect failed {reason}");
        LeaveSession();
        SetConnectionStatus(ConnectionStatus.Failed);
        (string status, string message) = ConnectFailedReasonToHuman(reason);
        _disconnectUI.ShowMessage(status, message);
        CLog.Log($"Connect failed {status} - {message}");

    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player} Joined!" + _runner.IsServer + " " + runner.IsServer);
        if (runner.IsServer)
        {
            if (_gameMode == GameMode.Server || _gameMode == GameMode.Host)//----if(_gameMode==GameMode.HOST)
                runner.Spawn(_gameManagerPrefab, Vector3.zero, Quaternion.identity);

            var roomPlayer = runner.Spawn(_roomPlayerPrefab, Vector3.zero, Quaternion.identity, player);
            roomPlayer.GameState = RoomPlayer.EGameState.Lobby;
        }

        if (isServer = (runner.IsServer && _gameMode == GameMode.Server))
        {
            _lobbyUI.iniciarContador();

            Debug.Log("EL PING ES: " + runner.GetPlayerRtt(player));
            //Ping
            //if (gameObject.GetComponent<EndRaceUI>() == null)
            //gameObject.AddComponent<EndRaceUI>();
        }
        SetConnectionStatus(ConnectionStatus.Connected);

        if (RoomPlayer.Local)
        {
            RoomPlayer.Local.checkSession();
        }

        PublishFreeLobbyDisplayPlayersNow();


        //runner.p
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        CLog.Log($"{player.PlayerId} disconnected.");

        int playersLeft = RoomPlayer.RemovePlayer(runner, player);

        if (playersLeft > 0 && CountHumans() == 0)
        {
            DespawnSafeBots();
            playersLeft = RoomPlayer.Players.Count;
        }

        if (playersLeft == 0)
        {
            LevelManager.LoadMenu();
            if (GameManager.Instance.Bet != 0)
            {
                GameManager.Instance.resetServerBet();

            }
        }

        PublishFreeLobbyDisplayPlayersNow();

        SetConnectionStatus(ConnectionStatus);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        CLog.Log($"OnShutdown {shutdownReason}");
        StopFreeLobbyDisplayPlayersRoutine(true);
        SetConnectionStatus(ConnectionStatus.Disconnected);

        (string status, string message) = ShutdownReasonToHuman(shutdownReason);
        bool fail = _disconnectUI.ShowMessage(status, message);

        RoomPlayer.Players.Clear();

        if (_runner)
            Destroy(_runner.gameObject);

        // Reset the object pools
        if (_pool != null)
            _pool.ClearPools();
        _pool = null;

        _runner = null;
        if (PlayfabManager.instance.loadingScreen.GetComponent<LoadingPointsAnim>().isShow())
        {
            PlayfabManager.instance.loadingScreen.GetComponent<LoadingPointsAnim>().hide();

        }
        if (fail) LeaveSessionGame(10);

    }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    private static (string, string) ShutdownReasonToHuman(ShutdownReason reason)
    {
        switch (reason)
        {
            case ShutdownReason.Ok:
                return (null, null);
            case ShutdownReason.Error:
                return ("Error", "Shutdown was caused by some internal error");
            case ShutdownReason.IncompatibleConfiguration:
                return ("Incompatible Config", "Mismatching type between client Server Mode and Shared Mode");
            case ShutdownReason.ServerInRoom:
                return ("Room name in use", "There's a room with that name! Please try a different name or wait a while.");
            case ShutdownReason.DisconnectedByPluginLogic:
                return ("Disconnected By Plugin Logic", "You were kicked, the room may have been closed");
            case ShutdownReason.GameClosed:
                return ("Game Closed", "The session cannot be joined, the game is closed");
            case ShutdownReason.GameNotFound:
                return ("Game Not Found", "This room does not exist");
            case ShutdownReason.MaxCcuReached:
                return ("Max Players", "The Max CCU has been reached, please try again later");
            case ShutdownReason.InvalidRegion:
                return ("Invalid Region", "The currently selected region is invalid");
            case ShutdownReason.GameIdAlreadyExists:
                return ("ID already exists", "A room with this name has already been created");
            case ShutdownReason.GameIsFull:
                return ("Game is full", "This lobby is full!");
            case ShutdownReason.InvalidAuthentication:
                return ("Invalid Authentication", "The Authentication values are invalid");
            case ShutdownReason.CustomAuthenticationFailed:
                return ("Authentication Failed", "Custom authentication has failed");
            case ShutdownReason.AuthenticationTicketExpired:
                return ("Authentication Expired", "The authentication ticket has expired");
            case ShutdownReason.PhotonCloudTimeout:
                return ("Cloud Timeout", "Connection with the Photon Cloud has timed out");
            default:
                CLog.LogWarning($"Unknown ShutdownReason {reason}");
                return ("Unknown Shutdown Reason", $"{(int)reason}");

        }

    }

    private static (string, string) ConnectFailedReasonToHuman(NetConnectFailedReason reason)
    {
        switch (reason)
        {
            case NetConnectFailedReason.Timeout:
                return ("Timed Out", "");
            case NetConnectFailedReason.ServerRefused:
                return ("Connection Refused", "The lobby may be currently in-game");
            case NetConnectFailedReason.ServerFull:
                return ("Server Full", "");
            default:
                CLog.LogWarning($"Unknown NetConnectFailedReason {reason}");
                return ("Unknown Connection Failure", $"{(int)reason}");
        }
    }

    private void Connect()
    {
        if (_runner == null)
        {
            //SetConnectionStatus(ConnectionStatus.Connecting);
            GameObject go = new GameObject("Session");
            //go.transform.SetParent(transform);
            DontDestroyOnLoad(go);
            //_players.Clear();
            _runner = go.AddComponent<NetworkRunner>();
            _runner.AddCallbacks(this);

            //_runner.
        }
        //_runner.loadBalancingClient.ConnectToRegionMaster(regionString);
        //PING
        //_runner.GetPlayerRtt();
    }

    //Conecta con el lobby generico para poder acceder a la lista de usuarios
    public async Task JoinLobby()
    {

        Connect();
        CLog.Log("Estado conexion: " + _runner.CurrentScene);
        var result = await _runner.JoinSessionLobby(SessionLobby.Custom, "MyCustomLobby");
        if (result.Ok)
        {
            // all good
            CLog.Log($"Conexion Exitosa " + result + " - " + _runner);
        }
        else
        {
            CLog.LogError($"Failed to Start: {result.ShutdownReason}");
        }
        // _ = StartHost(runner);
    }

    GameModes gameFilter;
    public void setMode(int _value)
    {
        gameFilter = (GameModes)_value;
    }


    public async Task StartHost()//NetworkRunner runner)//CREO LA SESION
    {
        Connect();
        var result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            CustomLobbyName = "MyCustomLobby"
        });

        if (result.Ok)
        {
            CLog.LogError("CREE EL SERVER");
            // all good
        }
        else
        {
            CLog.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }


    //Actualiza la lista de Sesiones abierta en tiempo real
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        CLog.Log($"Session List Updated with {sessionList.Count} session(s)");
        if (ListaSalas == null) ListaSalas = new List<itemList>();
        bool resetearSesionSeleccionada = true;
        SessionProperty sp;

        //Borro las listas que ya no estan
        if (ListaSalas.Count > 0)
        {
            bool borrar = true;
            foreach (itemList item in ListaSalas)
            {
                item.borrar = true;
                foreach (var session in sessionList)
                {
                    if (session.Name.Equals(item.sessionName))
                    {
                        item.borrar = false;
                        break;
                    }
                }
                if (item.borrar) CLog.Log("Borrar: " + item.sessionName);
            }



        }

        foreach (var session in sessionList)
        {


            CLog.Log($"Listando: {session.Name} " + session.PlayerCount);
            string trackid = "----", laps = "----", maxlaps = "----", bet = "----", serverIP = "----", serverMode = "-1";
            int displayPlayersOverride = -1;
            session.Properties.TryGetValue(SessionPropertyKey.TrackId.ToString(), out sp);
            if (sp != null)
            {
                CLog.Log("Prop: " + SessionPropertyKey.TrackId + ": " + sp.PropertyValue.ToString());
                trackid = sp.PropertyValue.ToString();
            }
            session.Properties.TryGetValue(SessionPropertyKey.Mode.ToString(), out sp);

            if (sp != null)
            {
                CLog.Log("Prop: " + SessionPropertyKey.Mode + ": " + sp.PropertyValue.ToString());
                serverMode = sp.PropertyValue.ToString();
            }
            session.Properties.TryGetValue(SessionPropertyKey.Laps.ToString(), out sp);
            if (sp != null)
            {
                CLog.Log("Prop: " + SessionPropertyKey.Laps + ": " + sp.PropertyValue.ToString());
                laps = sp.PropertyValue.ToString();
            }
            session.Properties.TryGetValue(SessionPropertyKey.MaxLaps.ToString(), out sp);
            if (sp != null)
            {
                CLog.Log("Prop: " + SessionPropertyKey.MaxLaps + ": " + sp.PropertyValue.ToString());
                maxlaps = sp.PropertyValue.ToString();
            }
            session.Properties.TryGetValue(SessionPropertyKey.Bet.ToString(), out sp);
            if (sp != null)
            {
                CLog.Log("Prop: " + SessionPropertyKey.Bet + ": " + sp.PropertyValue.ToString());
                bet = sp.PropertyValue.ToString();
            }
            session.Properties.TryGetValue(SessionPropertyKey.ServerIP.ToString(), out sp);

            if (sp != null)
            {
                CLog.Log("Prop: " + SessionPropertyKey.ServerIP + ": " + sp.PropertyValue.ToString());
                serverIP = sp.PropertyValue.ToString();
            }
            session.Properties.TryGetValue(DisplayPlayersPropertyKey, out sp);
            if (sp != null)
            {
                int.TryParse(sp.PropertyValue.ToString(), out displayPlayersOverride);
            }
            if (serverMode != null)
            {
                if ((int)gameFilter == int.Parse(serverMode))
                {
                    int betValue = 0;
                    bool isFreeSession = int.TryParse(bet, out betValue) && betValue <= 0;
                    if (!session.IsOpen || !isFreeSession)
                        displayPlayersOverride = -1;

                    if (ListaSalas.Find((x) => x.sessionName == session.Name) == null)
                    {
                        ListaSalas.Add(new itemList(session, session.Name, serverMode, session.PlayerCount, session.MaxPlayers, session.IsOpen, trackid, laps, maxlaps, bet, serverIP, displayPlayersOverride));
                    }
                    else
                    {
                        ListaSalas.Find((x) => x.sessionName == session.Name).Refresh(session, session.Name, serverMode, session.PlayerCount, session.MaxPlayers, session.IsOpen, trackid, laps, maxlaps, bet, displayPlayersOverride);

                    }

                    if (ContentListRooms.instance.RoomSelectedObjet.Equals(session.Name))
                        resetearSesionSeleccionada = false;
                }
                else
                {
                    if (ListaSalas.Find((x) => x.sessionName == session.Name) != null)
                        ListaSalas.Find((x) => x.sessionName == session.Name).borrar = true;
                }
            }

        }
        if (resetearSesionSeleccionada) ContentListRooms.instance.RoomSelectedObjet = "";

        if (ListaSalas != null) _listRooms.cargarLista(ListaSalas);
    }


    /*public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
		CLog.Log($"Session List Updated with {sessionList.Count} session(s)");
		ListaSalas = new List<itemList>();
		bool resetearSesionSeleccionada = true;
		SessionProperty sp;

		foreach (var session in sessionList)
		{


			CLog.Log($"Listando: {session.Name} " + session.PlayerCount);
			string trackid = "----", laps = "----", maxlaps = "----", bet = "----";
			session.Properties.TryGetValue(SessionPropertyKey.TrackId.ToString(), out sp);
			if (sp != null)
			{
				CLog.Log("Prop: " + SessionPropertyKey.TrackId + ": " + sp.PropertyValue.ToString());
				trackid = SessionPropertyKey.TrackId + " " + sp.PropertyValue.ToString();
			}
			session.Properties.TryGetValue(SessionPropertyKey.Laps.ToString(), out sp);
			if (sp != null)
			{
				CLog.Log("Prop: " + SessionPropertyKey.Laps + ": " + sp.PropertyValue.ToString());
				laps = SessionPropertyKey.Laps + " " + sp.PropertyValue.ToString();
			}
			session.Properties.TryGetValue(SessionPropertyKey.MaxLaps.ToString(), out sp);
			if (sp != null)
			{
				CLog.Log("Prop: " + SessionPropertyKey.MaxLaps + ": " + sp.PropertyValue.ToString());
				maxlaps = SessionPropertyKey.MaxLaps + " " + sp.PropertyValue.ToString();
			}
			session.Properties.TryGetValue(SessionPropertyKey.Bet.ToString(), out sp);
			if (sp != null)
			{
				CLog.Log("Prop: " + SessionPropertyKey.Bet + ": " + sp.PropertyValue.ToString());
				bet = sp.PropertyValue.ToString();
			}
			//session.Properties.TryGetValue(SessionPropertyKey.Bet.ToString(), out sp);
			//foreach(var sp2 in session.Properties)
			//CLog.Log("Estado: "+ sp2.Key.ToString()+" - "+ sp2.Value.ToString());

			ListaSalas.Add(new itemList(session, session.Name, session.PlayerCount, session.MaxPlayers, session.IsOpen, trackid, laps, maxlaps, bet));

			if (ContentListRooms.instance.RoomSelectedObjet.Equals(session.Name))
				resetearSesionSeleccionada = false;
		}
		if (resetearSesionSeleccionada) ContentListRooms.instance.RoomSelectedObjet = "";

		if (ListaSalas != null) _listRooms.cargarLista(ListaSalas);
	}*/

    public void joinLobbyFromList()
    {
        CLog.Log("ERROR: " + (ContentListRooms.instance.sessionSelect == null));

        if (ContentListRooms.instance.sessionSelect != null)
        {
            if (ContentListRooms.instance.sessionSelect.bet >= minBetTEL)
                StartCoroutine(consultarSaldo());
            else joinLobbyFromListOk();
        }

    }

    //Se conecta a la sala seleccioanda	
    public void joinLobbyFromListOk(/*NetworkRunner runner,string sessionName*/)
    {
        SetConnectionStatus(ConnectionStatus.Connecting);
        CLog.Log("Entro al Join " + ContentListRooms.instance.RoomSelectedObjet + " Runer vale: " + _runner);

        //_pool = _runner.gameObject.AddComponent<FusionObjectPoolRoot>();
        //PlayfabManager.instance.loadingScreen.GetComponent<LoadingPointsAnim>().setLoading("Conectando", 1);
        LevelManager.Instance.fader.gameObject.SetActive(true);
        LevelManager.Instance.fader.FadeIn();


        Invoke("lobby", .5f);
        FocusScreen(_lobbyUI);
        InterfaceManager.Instance.Displayname.SetActive(false);


        if (_runner)
        {
            //ListaSalas.Add(session.Name);
            // Entrar a la Primera Session disponible
            _runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Client, // Client GameMode
                SessionName = ContentListRooms.instance.RoomSelectedObjet,//session.Name, // Session to Join
                SceneManager = _levelManager,
                //SceneObjectProvider = GetSceneProvider(runner), // Scene Provider
                DisableClientSessionCreation = true, // Make sure the client will never create a Session
            });
            CLog.Log("Realice la conexion" + ContentListRooms.instance.RoomSelectedObjet + " Runer vale: " + _runner);

            if (RoomPlayer.Local != null)
            {
                RoomPlayer.Local.RPC_SetKartId(ClientInfo.KartId, ClientInfo.CharId); //Envio el ID a la instancia de este player en el sevidor, debo enviar el ID de Kart y Bajar su config desde el PlayerData


            }

        }
    }

    void lobby()
    {
        _lobbyUI.GetComponent<UIScreen>().FocusScreen(_lobbyUI.GetComponent<UIScreen>());
    }

    bool inCoroutine;
    //bool master;
    //Rutina para consutar el saldo del player
    public System.Collections.IEnumerator consultarSaldo()
    {
        if (inCoroutine) yield break;
        inCoroutine = true;
        //master = false; 

        if (ContentListRooms.instance.sessionSelect != null)
        {/*
			if (ContentListRooms.instance.sessionSelect.session.PlayerCount == 1)//Soy el primer player en la sala vacia
			{
				PopUpManager._instance.setPopUp("Atencion", "Cambiar el monto de la apuesta: "+ ContentListRooms.instance.sessionSelect.bet + " TEL", null, true, true);
				master = true;
			}
			else
			{
				PopUpManager._instance.setPopUp("Atencion", "Entraras a una partida apostando " + ContentListRooms.instance.sessionSelect.bet + " TEL\n Continuar?", null, true);
			}*/

            //PopUpManager._instance.setPopUp("Atencion", "Entraras a una partida apostando " + ContentListRooms.instance.sessionSelect.bet + " TEL\n Continuar?", IconosPopUp.questioin, true);
            PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_WARNING), TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_BETCONFIRM).Replace("XXX", ContentListRooms.instance.sessionSelect.bet.ToString()), IconosPopUp.questioin, true, 20);

        }
        yield return new WaitWhile(() => PopUpManager._instance.popUpState == PopUpStates.Wait);

        if (PopUpManager._instance.popUpState == PopUpStates.Ok)
        {
            PlayfabClientCurrency.GetCurrencyTel();
            yield return new WaitWhile(() => DataEconomy.ECONOMYSTATUS == EconomyStatus.DOWNLOADING);

            if (DataEconomy.ECONOMYSTATUS == EconomyStatus.OK)
            {
                Debug.Log("Este es el saldo: " + PlayfabManager.instance.getTEL());
                if (PlayfabManager.instance.getTEL() >= minBetTEL &&
                    PlayfabManager.instance.getTEL() >= ContentListRooms.instance.sessionSelect.bet)//>=(master ? PopUpManager._instance.getApuesta() : ContentListRooms.instance.sessionSelect.bet))
                {
                    joinLobbyFromListOk();

                }
                else
                {
                    //sinFondos((master ? PopUpManager._instance.getApuesta() : ContentListRooms.instance.sessionSelect.bet));
                    sinFondos(ContentListRooms.instance.sessionSelect.bet);

                }



            }
            else if (DataEconomy.ECONOMYSTATUS == EconomyStatus.ERROR)
            {
                //PopUpManager._instance.setPopUp("Error", "Error en la conexion, intente de nuevo", IconosPopUp.error, false);
                PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_ERROR),
                                                TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_ERRORNET), IconosPopUp.error, false);
                mainScreen.BackTo(gameModesScreen);

            }

        }
        else if (PopUpManager._instance.popUpState == PopUpStates.Cancel)
        {

            CLog.Log("REGRESANDO ACA ");
            mainScreen.BackTo(gameModesScreen);

        }

        inCoroutine = false;
        //popUpCoroutine = null;
    }

    public void sinFondos(int _bet)
    {
        PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_ERROR), TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_NOTEL).Replace("XXX", PlayfabManager.instance.getTEL().ToString()), IconosPopUp.error, false);
        mainScreen.BackTo(gameModesScreen);
    }

    public System.Collections.IEnumerator devolver(bool quedarseMonto)
    {

        int contador = 0;
        while (contador < 10)
        {
            Busines.Devolver(idServer.ToString(), quedarseMonto);
            yield return new WaitWhile(() => DataEconomy.ECONOMYSTATUS == EconomyStatus.DOWNLOADING);
            if (DataEconomy.ECONOMYSTATUS == EconomyStatus.OK)
            {
                if (quedarseMonto)
                    GameManager.Instance.setTransactionPendiente(false);
                yield break;
            }


            yield return new WaitForSeconds(1);
            //else
        }
        CLog.LogError("FALLO EN LA OPERACION");
    }
    public void sinFondos()
    {
        PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_ERROR), TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_TELOUT2), IconosPopUp.error, false);
    }

    public void exitPopUp()
    {
        //popUpState = PopUpStates.Wait;

        PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_WARNING), TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_EXIT), IconosPopUp.questioin, true); //question enum = 1 (int) + msg 01 = idmsg: 101
        StartCoroutine(exit());
    }
    public System.Collections.IEnumerator exit()
    {


        yield return new WaitWhile(() => PopUpManager._instance.popUpState == PopUpStates.Wait);

        if (PopUpManager._instance.popUpState == PopUpStates.Ok)
        {

            Application.Quit();
        }



    }

    /// <summary>
    /// Forma 1: Kart Origen,	Kart Parent 
    /// Forma 2: PowerUp		null
    /// </summary>
    /// <param name="expancion"></param>
    /// <param name="_origen"></param>
    /// <param name="kartParent"></param>
    public static void expancionFX(float expancion, Transform _origen, Transform kartParent)
    {
        if (expancion > 0)
        {
            foreach (KartEntity k in KartEntity.Karts)
            {
                if (_origen != k.transform && (kartParent == null ? true : k.transform != kartParent))
                {
                    CLog.Log("LA DISTANCIA ES: " + k.name + " - " + (k.transform.position - _origen.position).magnitude + " - " + expancion);
                    if ((k.transform.position - _origen.position).magnitude < expancion)
                        k.ImpactoKart(ClassPart.BANANA);
                }
            }
        }
    }
}




public class itemList
{
    public string sessionName;
    public string serverIP;
    public int players;
    public int maxPlayers;
    public bool isOpen;
    public string trackid;
    public string laps;
    public string maxlaps;
    public string bet;
    public SessionInfo session;
    public bool borrar;
    public bool isAdd;
    public int modeGame;
    public itemList(SessionInfo _session, string _nameRoom, string _gameMode, int _players, int _maxPlayers, bool _status, string _trackid, string _laps, string _maxlaps, string _bet, string _serverIP, int _displayPlayers = -1)
    {
        CLog.Log("guarde: " + _nameRoom + " - " + _players + " - " + _status);
        session = _session;
        GameLauncher.instance.IP_server = serverIP = _serverIP;
        sessionName = _nameRoom;
        maxPlayers = Mathf.Max(0, _maxPlayers - 1);
        players = ResolveVisiblePlayers(_players, maxPlayers, _displayPlayers);
        isOpen = _status;
        trackid = _trackid;
        laps = _laps;
        maxlaps = _maxlaps;
        bet = _bet;
        isAdd = false;
        //CLog.Log("GAME ROM: "+_nam)
        modeGame = int.Parse(_gameMode);

        //return this;
    }
    public void Refresh(SessionInfo _session, string _nameRoom, string _gameMode, int _players, int _maxPlayers, bool _status, string _trackid, string _laps, string _maxlaps, string _bet, int _displayPlayers = -1)
    {
        CLog.Log("Refresh: " + _nameRoom + " - " + _players + " - " + _status);
        //session = _session;
        sessionName = _nameRoom;
        maxPlayers = Mathf.Max(0, _maxPlayers - 1);
        players = ResolveVisiblePlayers(_players, maxPlayers, _displayPlayers);
        isOpen = _status;
        trackid = _trackid;
        laps = _laps;
        maxlaps = _maxlaps;
        bet = _bet;
        borrar = false;
        isAdd = true;
        modeGame = int.Parse(_gameMode);

        //return this;
    }

    private static int ResolveVisiblePlayers(int sessionPlayers, int visibleMaxPlayers, int displayPlayersOverride)
    {
        int realVisiblePlayers = Mathf.Clamp(sessionPlayers - 1, 0, visibleMaxPlayers);
        if (displayPlayersOverride < 0)
            return realVisiblePlayers;

        return Mathf.Clamp(Mathf.Max(realVisiblePlayers, displayPlayersOverride), 0, visibleMaxPlayers);
    }
}
