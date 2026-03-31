using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ClassMenuItem : MonoBehaviour
{
    public ClassPart kartStore;
    public ClassPart charStore;
    public bool isCharForce;
    public bool isKartForce;
    public UnityEngine.UI.RawImage icon;

    bool ButtonOn = false;

    public void setIcon(Sprite _sp)
    {
        if (_sp) icon.texture = _sp.texture;
    }
    public void toogleBtn()
    {
        cleanBtn();

        gameObject.transform.GetChild(0).gameObject.SetActive(true);

    }
    public void cleanBtn()
    {
        foreach (Transform go in gameObject.transform.parent.gameObject.transform)
        {
            go.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
