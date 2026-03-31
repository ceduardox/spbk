using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class LoginFromGuest : MonoBehaviour
{
    public TMP_InputField LoginEmailInput;
    public TMP_InputField LoginPasswordInput;

    
    public GameObject backgroundScene;

    public void DoLoginFromGuest()
    {
        //HiddenButtons();
        
        LoginPref.sabePref();
        PlayFabClientAPI.ForgetAllCredentials();
        PlayfabManager.instance.Login(LoginEmailInput.text, LoginPasswordInput.text);

    }

    IEnumerator ManageChangingAccount()
    {

        yield return new WaitForSeconds(1f);
    }
}
