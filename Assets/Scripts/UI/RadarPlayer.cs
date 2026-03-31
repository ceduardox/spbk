using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarPlayer : MonoBehaviour
{
    public KartEntity target;
    public UnityEngine.UI.Image icon;
    bool move = true;
    public void position()
    {
        if (!move) return;

        if (target)
        {


            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, -target.transform.eulerAngles.y);
            transform.localPosition = new Vector3(target.transform.localPosition.x, target.transform.localPosition.z, transform.localPosition.z) * 2.5f;
            if (target.LapController.HasFinished)
            {
                move = false;
                setColorBlack();
            }

        }
        else gameObject.SetActive(false);

        //CLog.Log(target.rotation.y + " " + target.eulerAngles.y);
    }

    public void setColorRed()
    {
        icon.color = Color.red;
    }
    public void setColorBlack()
    {
        icon.color = Color.black;
    }
}
