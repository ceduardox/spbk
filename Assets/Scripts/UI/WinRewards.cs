using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinRewards : MonoBehaviour
{

    public UnityEngine.UI.Text TEL;
    public UnityEngine.UI.Text TNL;
    public UnityEngine.UI.Text XP;
    public UnityEngine.UI.Text CUPS;
    // Start is called before the first frame update
    public void setValue(string _TEL, string _TLN, string _XP, string _Cups)
    {
        gameObject.SetActive(true);
        TEL.text = "x"+_TEL;
        TNL.text = "x" + _TLN;
        XP.text = "x" + _XP;
        CUPS.text = "x" + _Cups;
        TEL.gameObject.SetActive(false);
        TNL.gameObject.SetActive(false);
        XP.gameObject.SetActive(false);
        CUPS.gameObject.SetActive(false);
    }
    public void hide()
    {
        gameObject.SetActive(false);
    }

}
