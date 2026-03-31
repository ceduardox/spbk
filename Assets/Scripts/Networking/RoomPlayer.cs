using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class RoomPlayer : NetworkBehaviour
{
    public enum EGameState
    {
        Lobby,
        GameCutscene,
        GameReady
    }

    public static readonly List<RoomPlayer> Players = new List<RoomPlayer>();

    public static Action<RoomPlayer> PlayerJoined;
    public static Action<RoomPlayer> PlayerLeft;
    public static Action<RoomPlayer> PlayerChanged;

    public static RoomPlayer Local;

    [Networked(OnChanged = nameof(OnStateChanged))] public NetworkBool IsReady { get; set; }
    [Networked(OnChanged = nameof(OnStateChanged))] public string Username { get; set; }
    [Networked] public string playFabID { get; set; }
    [Networked] public NetworkBool HasFinished { get; set; }
    [Networked(OnChanged = nameof(OnKartChanged))] public KartController Kart { get; set; }//ERROR
    [Networked] public EGameState GameState { get; set; }
    [Networked(OnChanged = nameof(OnStateChanged))] public int KartId { get; set; }
    [Networked(OnChanged = nameof(OnStateChanged))] public int CharId { get; set; }

    public bool isTorneo;

    public bool IsLeader => Object != null && Object.IsValid && Object.HasStateAuthority;

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasInputAuthority)
        {
            Local = this;

            PlayerChanged?.Invoke(this);
            RPC_SetPlayerStats(ClientInfo.Username, ClientInfo.KartId, ClientInfo.CharId, PlayfabManager.instance.IdPlayFab, PlayfabManager.instance.isTorneo?1:0);
        }

        //A�ado el player a la lista de Players General
        Players.Add(this);
        PlayerJoined?.Invoke(this);

        DontDestroyOnLoad(gameObject);
        if (demmon != null)
            StopCoroutine(demmon);
        demmon = null;
        startConfig(true);
        QueueKartVisualRefresh();
        if (RoomChat.Instance)
            RoomChat.Instance.resetChat();
        //StartCoroutine(test());
    }

    public void setController(KartController _controller)
    {
        Kart = _controller;
        QueueKartVisualRefresh();
    }


    //Establesco los datos del player en el servidor
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority, InvokeResim = true)]
    private void RPC_SetPlayerStats(string username, int kartId, int charId, string _playFabID, int _isTorneo)
    {
        Username = username;
        KartId = kartId;
        CharId = charId;
        playFabID = _playFabID;
        isTorneo = _isTorneo==1;
        //isTorneo = true;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_SetKartId(int kartId, int _charId)
    {
        KartId = kartId;
        CharId = _charId;
    }


    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_ChangeReadyState(NetworkBool state)
    {
        Debug.Log($"Setting {Object.Name} ready state to {state}");
        IsReady = state;
    }

    private void OnDisable()
    {
        // OnDestroy does not get called for pooled objects
        if (demmon != null)
            StopCoroutine(demmon);
        demmon = null;
        if (kartVisualRefreshRoutine != null)
            StopCoroutine(kartVisualRefreshRoutine);
        kartVisualRefreshRoutine = null;
        PlayerLeft?.Invoke(this);
        Players.Remove(this);
    }

    private static void OnStateChanged(Changed<RoomPlayer> changed) => PlayerChanged?.Invoke(changed.Behaviour);

    private static void OnKartChanged(Changed<RoomPlayer> changed)
    {
        changed.Behaviour.QueueKartVisualRefresh();
        PlayerChanged?.Invoke(changed.Behaviour);
    }

    public static int RemovePlayer(NetworkRunner runner, PlayerRef p)
    {
        var roomPlayer = Players.FirstOrDefault(x => x.Object.InputAuthority == p);

        if (roomPlayer != null)
        {
            if (roomPlayer.Kart != null)
                runner.Despawn(roomPlayer.Kart.Object);

            Players.Remove(roomPlayer);
            runner.Despawn(roomPlayer.Object);
        }
        Debug.Log("Players counts: " + Players.Count);
        return Players.Count;
    }


    ///////////////////////////////////////////////////////////////////////////CHAT


    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    public void RPC_sendMSJ(string _userName, string _msj, string _color)
    {
        RoomChat.Instance.addTextText("<b><color="+ _color + ">" + _userName + ": </b></color>" + _msj);
    }
    

    ///////////////////////////////////////////////////////////////////////////CONFIG
    List<PlayerD> listaConfigKart = new List<PlayerD>();
    List<PlayerD> listaConfigChar = new List<PlayerD>();
    List<PowerUpPlayerRace> listaConfigPU = new List<PowerUpPlayerRace>();

    public bool IsBotPlayer()
    {
        if (string.IsNullOrEmpty(playFabID))
            return false;
        return playFabID.StartsWith("BOT_", StringComparison.OrdinalIgnoreCase);
    }


    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    public void RPC_sendCONFIG(string _confgKart, string _configChar, string _configPU)
    {

        Debug.Log("LISTA MEJORAS: " + KartId + " - " + _confgKart + " - " + _configChar);


        listaConfigKart = new List<PlayerD>();
        listaConfigChar = new List<PlayerD>();
        listaConfigPU = new List<PowerUpPlayerRace>();

        string[] cadena = _confgKart.Split(":");
        for (int i = 0; i < cadena.Length - 1; i += 2)
            listaConfigKart.Add(new PlayerD(cadena[i + 1], cadena[i]));

        cadena = _configChar.Split(":");
        for (int i = 0; i < cadena.Length - 1; i += 2)
            listaConfigChar.Add(new PlayerD(cadena[i + 1], cadena[i]));



        listaConfigPU.Add(new PowerUpPlayerRace("0", ClassPart.NONE.ToString(), 0));
        listaConfigPU.Add(new PowerUpPlayerRace("0", ClassPart.NONE.ToString(), 0));



        if (!string.IsNullOrEmpty(_configPU))
        {
            cadena = _configPU.Split(":");
            for (int i = 0; i < cadena.Length - 1; i += 3)
                listaConfigPU[i / 3] = (new PowerUpPlayerRace(cadena[i + 1], cadena[i], int.Parse(cadena[i + 2])));

            if (listaConfigPU[0].classPart == ClassPart.NONE)
            {
                listaConfigPU[0] = listaConfigPU[1];
                listaConfigPU[1] = new PowerUpPlayerRace("0", ClassPart.NONE.ToString(), 0);
            }
        }


        foreach (PowerUpPlayerRace classs in listaConfigPU)
        {
            Debug.Log("LISTA POWER UPS: " + classs.classPart + " - " + classs.id + " - " + classs.amount + " " + Username);
        }

    }

    public List<PlayerD> getlistConfig(bool _kart)
    {
        if (_kart)
            return listaConfigKart;
        else
            return listaConfigChar;
    }

    public List<PowerUpPlayerRace> getPu()
    {
        return listaConfigPU;

    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    public void RPC_sssyncTimer(float _timer)
    {
        TimerCounterScreen._instance.syncTimer(_timer, 0);
    }



    Coroutine demmon;
    Coroutine kartVisualRefreshRoutine;

    private void QueueKartVisualRefresh()
    {
        if (!isActiveAndEnabled)
            return;

        if (kartVisualRefreshRoutine != null)
            StopCoroutine(kartVisualRefreshRoutine);

        kartVisualRefreshRoutine = StartCoroutine(RefreshKartVisualAfterSpawn());
    }

    private IEnumerator RefreshKartVisualAfterSpawn()
    {
        bool IsNetworkReady()
        {
            return Object != null && Object.IsValid;
        }

        KartController TryGetKartSafe()
        {
            if (!IsNetworkReady())
                return null;
            try
            {
                return Kart;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        int waitFrames = 240;
        while (waitFrames-- > 0 && IsNetworkReady())
        {
            if (ResourceManager.Instance == null || KartId <= 0 || CharId <= 0)
            {
                yield return null;
                continue;
            }

            var kartRef = TryGetKartSafe();
            if (kartRef == null)
            {
                yield return null;
                continue;
            }

            var ks = kartRef.gameObject.GetComponent<Kart_Store>();
            var kd = ResourceManager.Instance.getKart(KartId);
            var dd = ResourceManager.Instance.getChar(CharId);
            if (ks == null || kd == null || dd == null)
            {
                yield return null;
                continue;
            }

            var cs = ks.changePart(kd, new PlayerD(CharId.ToString(), ClassPart.DRIVER), false);
            if (cs != null && !IsBotPlayer())
            {
                foreach (PlayerD playerD in getlistConfig(false))
                    cs.changeCharPart(dd, playerD, false);
            }

            kartVisualRefreshRoutine = null;
            yield break;
        }

        kartVisualRefreshRoutine = null;
    }

    public void startConfig(bool _firsTime)
    {
        if (demmon == null)
            demmon = StartCoroutine(setKartConfig(_firsTime));
        /*		if(demmon!=null)
				{
					StopCoroutine(demmon);
					demmon = null;
				}
				demmon = StartCoroutine(setKartConfig(_firsTime));
		*/
    }    //[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    IEnumerator setKartConfig(bool _firsTime)
    {
        Debug.Log("EN EL DEMMON ESPERANDO PARA CONTINUAR:" + Username + " " + GameManager.Instance.Bet + " " + PlayfabManager.instance.getTEL());// + " " + ks + " " + ks.gameObject.name);

        bool IsNetworkReady()
        {
            return Object != null && Object.IsValid;
        }

        KartController TryGetKartSafe()
        {
            if (!IsNetworkReady())
                return null;
            try
            {
                return Kart;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        if (!IsNetworkReady())
        {
            demmon = null;
            yield break;
        }

        int botProbe = 5;
        while (botProbe-- > 0 && string.IsNullOrEmpty(playFabID))
            yield return null;

        if (IsBotPlayer())
        {
            yield return ConfigureBotVisualSafe(IsNetworkReady, TryGetKartSafe);
            demmon = null;
            yield break;
        }

        if (GameManager.Instance.Bet == 0 && !GameLauncher.instance.isServer)
        {
            PlayfabLifesControl.GetVidasIntentos(PlayfabManager.instance);
        }

        Inventory.importInventory();

        while (IsNetworkReady() && TryGetKartSafe() != null)
            yield return null;

        yield return null;

        if (Local)
        {
            if (!_firsTime && GameManager.Instance.Bet > 0)
            {
                PlayfabClientCurrency.GetCurrencyTel();
                yield return new WaitWhile(() => DataEconomy.ECONOMYSTATUS == EconomyStatus.DOWNLOADING);

                if (DataEconomy.ECONOMYSTATUS == EconomyStatus.OK)
                {
                    if (PlayfabManager.instance.getTEL() >= GameManager.Instance.Bet)
                    {
                        PopUpManager._instance.setPopUp(
                            TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_WARNING),
                            TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_CONFIRMNEXTBETRACE).Replace("XXX", GameManager.Instance.Bet.ToString()),
                            IconosPopUp.questioin,
                            true,
                            10);

                        while (IsNetworkReady() && PopUpManager._instance.popUpState == PopUpStates.Wait && TryGetKartSafe() == null)
                            yield return null;

                        if (PopUpManager._instance.popUpState == PopUpStates.Cancel)
                        {
                            GameLauncher.instance.LeaveSession();
                            demmon = null;
                            yield break;
                        }

                        PopUpManager._instance.hidePopUp();
                    }
                    else
                    {
                        GameLauncher.instance.sinFondos(GameManager.Instance.Bet);
                        GameLauncher.instance.LeaveSession();
                    }
                }
            }
        }

        yield return new WaitForSeconds(.2f);

        while (IsNetworkReady() && TryGetKartSafe() == null)
            yield return null;

        KartController kartRef = TryGetKartSafe();
        if (!IsNetworkReady() || kartRef == null)
        {
            demmon = null;
            yield break;
        }

        try
        {
            Kart_Store ks = kartRef.gameObject.GetComponent<Kart_Store>();
            Char_Store cs = null;
            Debug.Log("EN EL DEMMON, BUSCO :" + Username + " " + ks + " " + ks.gameObject.name + " - La lista vale: " + getlistConfig(true));

            if (getlistConfig(true) != null)
            {
                foreach (PlayerD _playerD in getlistConfig(true))
                {
                    ks.changePart(ResourceManager.Instance.getKart(KartId), _playerD, false);

                    if (_playerD.ClassPart == ClassPart.EXHAUST)
                    {
                        ItemBase ibTMP = Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), _playerD.Id);
                        if (ibTMP == null)
                        {
                            Debug.Log("DEBUG ERROR: " + _playerD.Id);
                            continue;
                        }

                        Debug.Log("ESTO VALE TAGS: " + ibTMP.Tag + " " + ibTMP.Id);

                        if (ibTMP.Tag != null)
                        {
                            ks.changeSfx(ibTMP.Tag);
                        }
                    }
                }
            }

            cs = ks.changePart(ResourceManager.Instance.getKart(KartId), new PlayerD(CharId.ToString(), ClassPart.DRIVER), false);
            if (cs != null)
                Debug.Log("+++ Cargue el Driver: " + cs.name + " - " + cs.model + " ID: " + CharId.ToString());
            else
                Debug.LogWarning("No se pudo cargar driver para " + Username + " CharId: " + CharId);
            if (cs)
            {
                foreach (PlayerD _playerD in getlistConfig(false))
                {
                    cs.changeCharPart(ResourceManager.Instance.getChar(CharId), _playerD, false);
                }
            }

            if (GameLauncher.instance.modeServerDedicado && GameLauncher.instance.isServer)
            {
                float speed = 0;
                float acc = 0;
                float turn = 0;
                ItemBase ibTMP = Catalogo.getItem(Catalogos.Karts.ToString(), KartId.ToString());

                speed += ibTMP.getCustomData(CustomDataItem.Speed);
                acc += ibTMP.getCustomData(CustomDataItem.Acceleration);
                turn += ibTMP.getCustomData(CustomDataItem.Turn);

                Debug.Log("ESTO VALE SPEED: " + speed + " - " + acc + " - " + turn + " - " + ibTMP.Tag);
                foreach (PlayerD _playerD in getlistConfig(true))
                {
                    Debug.Log("ESTO VALE PARTE" + _playerD.Id);

                    ibTMP = Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), _playerD.Id);
                    if (ibTMP == null)
                    {
                        Debug.Log("DEBUG ERROR: " + _playerD.Id);
                        continue;
                    }
                    speed += ibTMP.getCustomData(CustomDataItem.Speed);
                    acc += ibTMP.getCustomData(CustomDataItem.Acceleration);
                    turn += ibTMP.getCustomData(CustomDataItem.Turn);
                }
                Debug.Log("ESTO VALE SPEED: " + speed + " - " + acc + " - " + turn);

                kartRef.setStats(GameLauncher.MAX_SPEED * .5f + GameLauncher.MAX_SPEED * .5f * speed,
                                 GameLauncher.MAX_ACC * .5f + GameLauncher.MAX_ACC * .5f * acc,
                                 GameLauncher.MAX_TURN * .5f + GameLauncher.MAX_TURN * .5f * turn);
            }

            demmon = null;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error: " + e);
            demmon = null;
        }
    }

    private IEnumerator ConfigureBotVisualSafe(Func<bool> IsNetworkReady, Func<KartController> TryGetKartSafe)
    {
        int waitIdsFrames = 180;
        while (IsNetworkReady() && waitIdsFrames-- > 0)
        {
            bool hasDefinitions = false;
            try
            {
                if (KartId > 0 && CharId > 0 && ResourceManager.Instance != null)
                {
                    hasDefinitions = ResourceManager.Instance.getKart(KartId) != null &&
                                     ResourceManager.Instance.getChar(CharId) != null;
                }
            }
            catch (InvalidOperationException)
            {
                hasDefinitions = false;
            }

            if (hasDefinitions)
                break;

            yield return null;
        }

        while (IsNetworkReady() && TryGetKartSafe() == null)
            yield return null;

        var kartRef = TryGetKartSafe();
        if (!IsNetworkReady() || kartRef == null)
            yield break;

        try
        {
            var ks = kartRef.gameObject.GetComponent<Kart_Store>();
            if (ks == null || ResourceManager.Instance == null)
                yield break;

            var kd = ResourceManager.Instance.getKart(KartId);
            var dd = ResourceManager.Instance.getChar(CharId);
            if (kd == null || dd == null)
            {
                Debug.LogWarning("Bot preset invalido para " + Username + " KartId: " + KartId + " CharId: " + CharId);
                yield break;
            }

            var cs = ks.changePart(kd, new PlayerD(CharId.ToString(), ClassPart.DRIVER), false);
            if (cs == null)
                Debug.LogWarning("Bot sin driver para " + Username + " CharId: " + CharId);
        }
        catch (Exception e)
        {
            Debug.LogWarning("ConfigureBotVisualSafe error: " + e.Message);
        }
    }

    public void checkSession()
    {
        StartCoroutine(checkSessionCoroutine());
    }

    IEnumerator checkSessionCoroutine()
    {

        Busines.ConfirmKey();
        yield return new WaitWhile(() => DataEconomy.ECONOMYSTATUS == EconomyStatus.DOWNLOADING);
        if (DataEconomy.ECONOMYSTATUS != EconomyStatus.OK)
        {
            PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_WARNING), TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_MULTILOG), IconosPopUp.error, false);
            GameLauncher.instance.LeaveSession();
        }
        else
        {
            Debug.Log("Verificacion de sesion Valida");
        }

    }


}









