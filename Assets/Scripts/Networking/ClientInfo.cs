using UnityEngine;

public static class ClientInfo {
    public static string Username {
        get => PlayerPrefs.GetString("C_Username_"+PlayfabManager.instance.IdPlayFab, string.Empty);
        set => PlayerPrefs.SetString("C_Username_" + PlayfabManager.instance.IdPlayFab, value);
    }

    public static int KartId {
        get => PlayerPrefs.GetInt("C_KartId_" + PlayfabManager.instance.IdPlayFab, 1000);
        set => PlayerPrefs.SetInt("C_KartId_" + PlayfabManager.instance.IdPlayFab, value);
    }
    public static int CharId
    {
        get => PlayerPrefs.GetInt("C_DriverId_" + PlayfabManager.instance.IdPlayFab, 100);
        set => PlayerPrefs.SetInt("C_DriverId_" + PlayfabManager.instance.IdPlayFab, value);
    }

    public static string LobbyName {
        get => PlayerPrefs.GetString("C_LastLobbyName_" + PlayfabManager.instance.IdPlayFab, "");
        set => PlayerPrefs.SetString("C_LastLobbyName_" + PlayfabManager.instance.IdPlayFab, value);
    }
}