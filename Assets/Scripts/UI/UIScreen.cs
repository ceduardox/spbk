using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class UIScreen : MonoBehaviour
{
    public bool isModal = false;
    public bool showDisplay = false;
    [SerializeField] private UIScreen previousScreen = null;
    public static UIScreen activeScreen;
    public ScreenFocus _screen;


    private void OnEnable()
    {
        if (showDisplay && InterfaceManager.Instance != null) { InterfaceManager.Instance.enableDisplayData(true); } 
        if (SpinStore.instance) SpinStore.instance.setCamera(_screen);
        setBack();
        if (GetComponent<UIEffects>()) 
        {
            GetComponent<UIEffects>().enabled = false; 
            GetComponent<UIEffects>().enabled = true; 
        }
        Invoke("setBack", 1);
        var masterScreen = GetComponent<MasterScreen>();
        if (masterScreen && HeaderScreen.instance)
        {
            HeaderScreen.instance.setPosition(masterScreen.DisplayPosition/*, true*/);
        } 
    }
    private void OnDisable()
    {
        //if (gameObject.GetComponent<MasterScreen>())
        //{
        //    HeaderScreen.instance.setPosition(HeaderScreen.instance.previousState/*, false*/);
        //}
    }
    void setBack()
    {
        if (SpinStore.instance && _screen != ScreenFocus.NONE)
            SpinStore.instance.setCamera(_screen);

    }

    public void setActiveObj(GameObject obj)
    {
        if (obj == activeScreen)
            return;
        obj.SetActive(true);
    }
    public void setDeactiveObj(GameObject obj)
    {
        if (obj == activeScreen)
            return;
        obj.SetActive(false);
    }
    public static void Focus(UIScreen screen)
    {
        CLog.Log("PRENDI A: " + screen);
        if (screen == activeScreen)
            return;

        if (activeScreen)
            activeScreen.Defocus();
        screen.previousScreen = activeScreen;
        activeScreen = screen;
        screen.Focus();
        CLog.Log("+ PRENDIB: " + screen);
        InterfaceManager.Instance.enableDisplayData(screen.showDisplay);
        CLog.Log("+ PRENDIC: " + screen);
    }
    public void OpenModalUI(UIScreen screen)
    {
        if (screen == activeScreen)
            return;
        screen.Focus();
    }
    public void CloseModalUI(UIScreen screen)
    {
        screen.Defocus();
    }
    public static void BackToInitial()
    {
        activeScreen?.BackTo(null);
    }


    public void FocusScreen(UIScreen screen)
    {
        Focus(screen);
    }

    private void Focus()
    {
        if (gameObject)
            gameObject.SetActive(true);
    }

    private void Defocus()
    {
        //CLog.Log("APAGUE: " + gameObject);
        if (gameObject)
            gameObject.SetActive(false);
    }
    public void UpdateDisplayName()
    {
        //PlayfabManager.instance.UpdateNameUser();
        Back();
    }
    public void Back()
    {
        if (previousScreen)
        {
            Defocus();
            activeScreen = previousScreen;
            activeScreen.Focus();
            previousScreen = null;
        }
    }

    public void BackTo(UIScreen screen)
    {
        CLog.Log("SCREEN TO INITITAL: " + screen + " " + activeScreen);
        while (activeScreen != null && activeScreen.previousScreen != null && activeScreen != screen)
        {
            activeScreen.Back();

        }
        if (screen) InterfaceManager.Instance.enableDisplayData(screen.showDisplay);
    }

    public void ConfirmKart()
    {
        if (previousScreen)
        {
            Defocus();
            activeScreen = previousScreen;
            activeScreen.Focus();
            previousScreen = null;
        }
        PlayfabClientCurrency.consumeCurrency();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void enableHeader()
    {
        InterfaceManager.Instance.enableDisplayData(true);
    }

    public static void ScrollToTop(ScrollRect scrollRect)
    {
        scrollRect.normalizedPosition = new Vector2(1, 0);
    }
    public static void ScrollToBottom(ScrollRect scrollRect)
    {
        scrollRect.normalizedPosition = new Vector2(-1, 0);
    }
}
