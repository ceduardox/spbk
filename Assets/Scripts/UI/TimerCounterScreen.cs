using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;


public class TimerCounterScreen : MonoBehaviour
{
    public float maxTimer = 60;
    public float timeNoConfig = 10;
    [Header("Bot Fill (Safe Skeleton)")]
    [SerializeField] private bool enableBotLobbyScheduler = true;
    [SerializeField] private float botJoinIntervalSeconds = 10f;
    [Header("Virtual Bots (Free Mode Only)")]
    [SerializeField] private bool enableVirtualLobbyBots = true;
    [SerializeField] private int maxVirtualBots = 6;
    float timer, timer2;
    float timeAllReady = 4;
    public LobbyUI _lobbyUI;
    public TextMeshProUGUI counterUItext;
    public TextMeshProUGUI firstTextUItext;
    public TextMeshProUGUI lastUItext;
    public bool permitirCambios;
    public static TimerCounterScreen _instance;
    private float nextBotTickAt = -1f;
    private int virtualBotCount = 0;


    private void Awake()
    {
        _instance = this;
        timer2 = timer = maxTimer;
    }

    private void OnEnable()
    {
        permitirCambios = true;
        timer2 = timer = maxTimer;
        nextBotTickAt = maxTimer - botJoinIntervalSeconds;
        virtualBotCount = 0;
        counterUItext.text = "";
        Invoke("setOff", 1);
    }
    void setOff()
    {
        if(GameLauncher.ConnectionStatus!=ConnectionStatus.Connected)
        {
            CLog.Log("Timer set off due to connection status is not connected");
            gameObject.SetActive(false); 
        }

    }
    public void active()
    {
        gameObject.SetActive(true);
    }

    public void show(bool _value)
    {
        gameObject.SetActive(_value);
    }
    public void forceTime()
    {
        if (timer > timeAllReady)
            timer = timeAllReady;
    }
    int lastTimer = 0;
    void Update()
    {
        //----if (RoomPlayer.Local == null) return;
        //----if (!RoomPlayer.Local.IsLeader) return;// gameObject.SetActive(false);
        //CLog.Log("ESTO SOY: " + Object + " " + Object.IsValid + " "+Object.HasStateAuthority);

        if (GameLauncher.instance.isServer ||
            RoomPlayer.Local &&
            RoomPlayer.Local.IsLeader)
        {
            
            timer -= Time.deltaTime;
            UpdateBotLobbyScheduler();
            int playersForCountdown = GetEffectiveLobbyPlayers();

            if ((int)timer != lastTimer)
            {
                if (playersForCountdown < GameLauncher.instance.minPlayers)
                {
                    reset();
                    if (!GameLauncher.instance.getRunner().SessionInfo.IsOpen)
                    {
                        GameLauncher.instance.setOpenNet(true);
                       
                    }
                }
                GameManager.Instance.RPC_syncTimer(timer, (playersForCountdown - GameLauncher.instance.minPlayers));
                
            }
            

            lastTimer = (int)timer;

            if (timer < 0)
            {
                timer = maxTimer;
                permitirCambios = true;
                _lobbyUI.iniciarCarrera();
                reset();
            }
        }
        if(GameLauncher.instance)
        {
            if(GameLauncher.ConnectionStatus!=ConnectionStatus.Connected)
            {
                gameObject.SetActive(false);
            }
        }
    }
    public bool isCustomize()
    {
        return timer2 > timeNoConfig;
    }

    public void reset()
    {
        permitirCambios = true;
        timer2 = timer = maxTimer;
        virtualBotCount = 0;
        nextBotTickAt = maxTimer - botJoinIntervalSeconds;
        counterUItext.text = "";
        firstTextUItext.text = "Comienza en ";
        lastUItext.text = "Seg";
    }
    public bool isReadyEnabled()
    {
        return timer2 > timeAllReady;
    }

    private void UpdateBotLobbyScheduler()
    {
        if (!enableBotLobbyScheduler)
            return;
        if (GameLauncher.instance == null || GameManager.Instance == null)
            return;
        if (botJoinIntervalSeconds <= 0f)
            return;

        // Free mode only: never schedule in bet sessions.
        bool isFreeMode = IsFreeModeSafe();
        if (!isFreeMode)
            return;

        int currentPlayers = RoomPlayer.Players.Count;
        int maxPlayers = GameLauncher.instance.maxPlayers;
        int maxByCups = Mathf.Max(0, maxPlayers - currentPlayers);
        int virtualBotsCap = Mathf.Min(maxVirtualBots, maxByCups);
        if (currentPlayers >= maxPlayers || virtualBotCount >= virtualBotsCap)
            return;

        if (nextBotTickAt < 0f)
            nextBotTickAt = maxTimer - botJoinIntervalSeconds;

        if (timer <= nextBotTickAt && timer > 0f)
        {
            bool spawned = GameLauncher.instance.TrySpawnSafeFreeLobbyBot();
            if (spawned)
                CLog.Log($"[BOT-SAFE] +1 bot lobby={Mathf.CeilToInt(timer)}s players={RoomPlayer.Players.Count}/{maxPlayers}");
            nextBotTickAt -= botJoinIntervalSeconds;
        }
    }

    private int GetEffectiveLobbyPlayers()
    {
        int realPlayers = RoomPlayer.Players.Count;
        if (!enableVirtualLobbyBots)
            return realPlayers;
        return realPlayers + Mathf.Max(0, virtualBotCount);
    }

    private bool IsFreeModeSafe()
    {
        if (GameLauncher.instance == null)
            return false;
        if (GameLauncher.instance.serverBet)
            return false;
        if (GameManager.Instance == null)
            return false;
        if (GameManager.Instance.Object == null || !GameManager.Instance.Object.IsValid)
            return false;

        try
        {
            return GameManager.Instance.Bet <= 0;
        }
        catch (System.InvalidOperationException)
        {
            return false;
        }
    }

    public void syncTimer(float _timer, int _complete)
    {
        timer2 = _timer;
        if (_complete >= 0)
        {
            counterUItext.text = ((int)_timer).ToString();
            firstTextUItext.text = "Comienza en ";
            lastUItext.text = "Seg";
        }
        else
        {
            counterUItext.text = (-_complete).ToString(); ;
            firstTextUItext.text = "Esperando  ";
            lastUItext.text = "Players";
            if (LobbyUI._instance != null)
                LobbyUI._instance.resetButtons();
        }
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        if (_timer < timeNoConfig)
        {
            permitirCambios = false;
        }
        else
        {
            permitirCambios = true;
        }
        if ((int)_timer == 0)
        {
            if (!GameLauncher.instance.isServer &&
                (RoomPlayer.Local != null &&
                !RoomPlayer.Local.IsLeader))
                gameObject.SetActive(false);
        }
    }


}
