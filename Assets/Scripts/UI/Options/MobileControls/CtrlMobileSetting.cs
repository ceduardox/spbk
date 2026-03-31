using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CtrlMobileSetting : MonoBehaviour
{

    public Toggle toggleDoubleTap;
    public Toggle toggleHeldButton;
    public Toggle toggleAddButton;
    public Slider sensibilityTouch;

    private void Start()
    {
        toggleDoubleTap.onValueChanged.AddListener(delegate {
            ToggleValueChangedDT(toggleDoubleTap);
        });
        toggleHeldButton.onValueChanged.AddListener(delegate {
            ToggleValueChangedH(toggleHeldButton);
        });
        toggleAddButton.onValueChanged.AddListener(delegate {
            ToggleValueChangedADD(toggleAddButton);
        });
        sensibilityTouch.onValueChanged.AddListener(delegate {
            sliderValueChange(sensibilityTouch);
        });
    }
    void ToggleValueChangedDT(Toggle change)
    {
        CLog.Log("dt " + change);
        if (change.enabled)
        {
            UnityEngine.InputSystem.OnScreen.DrifMode.modetap = true;
            UnityEngine.InputSystem.OnScreen.DrifMode.modeheld = false;
            UnityEngine.InputSystem.OnScreen.DrifMode.Lbtn.SetActive(false);
            UnityEngine.InputSystem.OnScreen.DrifMode.Rbtn.SetActive(false);
        }
    }
    void ToggleValueChangedH(Toggle change)
    {
        CLog.Log("h " + change);
        if (change.enabled)
        {
            UnityEngine.InputSystem.OnScreen.DrifMode.modeheld = true;
            UnityEngine.InputSystem.OnScreen.DrifMode.modetap = false;
            UnityEngine.InputSystem.OnScreen.DrifMode.Lbtn.SetActive(false);
            UnityEngine.InputSystem.OnScreen.DrifMode.Rbtn.SetActive(false);
        }
    }
    void ToggleValueChangedADD(Toggle change)
    {
        if (change.enabled)
        {
            UnityEngine.InputSystem.OnScreen.DrifMode.Lbtn.SetActive(true);
            UnityEngine.InputSystem.OnScreen.DrifMode.Rbtn.SetActive(true);
            UnityEngine.InputSystem.OnScreen.DrifMode.modeheld = false;
            UnityEngine.InputSystem.OnScreen.DrifMode.modetap = false;
        }
    }
    void sliderValueChange(Slider sl)
    {
        UnityEngine.InputSystem.OnScreen.DrifMode.timeMouseDown = sl.value;
        UnityEngine.InputSystem.OnScreen.DrifMode.interval = sl.value;
    }
}
