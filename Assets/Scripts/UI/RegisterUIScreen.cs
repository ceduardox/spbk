using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RegisterUIScreen : MonoBehaviour
{
    [Header("GUI Register Inputs Objects")]
    #region REGISTER
    public TMP_InputField RegisterEmailInput;
    public TMP_InputField RegisterPasswordInput;
    public TMP_InputField RegisterNicknameInput;
    public TMP_InputField RegisterConfirmPasswordInput;
    #endregion

    public void RegisterOnClick()
    {
        //LoginScreen.instance.HiddenButtons();
        ScreenManager.loginScreen.HiddenButtons(true);
        PlayfabManager.instance.Register(RegisterEmailInput.text, RegisterPasswordInput.text, RegisterNicknameInput.text, RegisterConfirmPasswordInput.text);
    }

    public void RegisterOnClickGuest()
    {
        //LoginScreen.instance.HiddenButtons();
        PlayfabManager.instance.RegisterAGuest(
           RegisterEmailInput.text,
           RegisterNicknameInput.text,
           RegisterPasswordInput.text,
           RegisterConfirmPasswordInput.text
           );
    }
}
