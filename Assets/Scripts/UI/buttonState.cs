using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonState : MonoBehaviour
{
    public bool isLocked;
    public GameObject imgLock;
    public UnityEngine.UI.Image icon;


    /// <summary>
    /// true: significa desbloqueado, que el auto está comprado
    /// </summary>
    /// <param name="_state"></param>
    public void setState(bool _state)
    {
        imgLock.SetActive(isLocked = !_state);
    }

    public void setIcon(Sprite _icon)
    {
        icon.sprite = _icon;
    }
    // Start is called before the first frame update

}
