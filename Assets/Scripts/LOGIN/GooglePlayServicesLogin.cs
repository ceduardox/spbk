# if UNITY_ANDROID

using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;



public class GooglePlayServicesLogin : MonoBehaviour
{
    [SerializeField] public bool isGoogleAutoLogin;
    public bool isGoogleAuthenticated = false;

    public void Start()
    {
        if (SystemInfo.deviceType != DeviceType.Handheld) return;  // <-- CORRER SOLO EN MOBILE ?
        Initialize();
    }

    private void Initialize()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .AddOauthScope("profile")
            .RequestServerAuthCode(false)
            .Build();
        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        //
        PlayGamesPlatform.Activate();
        CLog.Log("Google Play Games Initialized");
        SignInWithGooglePlayGames();
    }

    public void SignInWithGooglePlayGames()
    {
        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity
            //.CanPromptOnce        <<-------  solo activa el popup si el usuario no cancelo anteriormente el login
            .CanPromptAlways  // <<----  prueba aver como funciona, deberia mostrar el popup en todos los casos al comienzo del juego
            , (success) =>
         {
             switch (success)
             {
                 case SignInStatus.Success:
                     CLog.Log("Signed to Google Games Successfull" + success);
                     isGoogleAuthenticated = true;
                    //  if (isGoogleAutoLogin)                    //<--- Logea automaticamente a PlayFab con Google si es "true"
                    //      PlayFab_LoginWithGoogle();
                     break;
                 default:
                     CLog.Log("Sign in to Google Play Services Failed" + success);
                     isGoogleAuthenticated = false;
                     break;
             }

         });
    }
public void SignInWithGooglePlayGames(Action callback)
{
    PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptAlways, (success) =>
    {
        switch (success)
        {
            case SignInStatus.Success:
                CLog.Log("Signed to Google Games Successfull" + success);
                isGoogleAuthenticated = true;
                callback?.Invoke();
                break;
            default:
                CLog.Log("Sign in to Google Play Services Failed" + success);
                isGoogleAuthenticated = false;
                break;
        }
    });
}
    // llama al login desde el PlayfabManager y envia el AuthCode de Google.
    public void PlayFab_LoginWithGoogle()
{
    if (PlayGamesPlatform.Instance.localUser.authenticated)
    {
        var serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
        var googleDisplayName = PlayGamesPlatform.Instance.GetUserDisplayName();
        CLog.Log("googleserverauthCode: " + serverAuthCode);
        CLog.Log("google displayName" + googleDisplayName);
        PlayfabManager.instance.Playfab_LoginWithGoogleServices(serverAuthCode, googleDisplayName);
    }
    else
    {
        SignInWithGooglePlayGames(() =>
        {
            var serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
            var googleDisplayName = PlayGamesPlatform.Instance.GetUserDisplayName();
            CLog.Log("googleserverauthCode: " + serverAuthCode);
            CLog.Log("google displayName" + googleDisplayName);
            PlayfabManager.instance.Playfab_LoginWithGoogleServices(serverAuthCode, googleDisplayName);
        });
    }
}

}
#endif
