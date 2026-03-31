using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateKart : MonoBehaviour
{
    Vector3 mouseReference;
    public void OnMouseDown()
    {
        mouseReference = Input.mousePosition;
        SpinStore.instance.setFreeRotate(true);
    }

    public void OnMouseDrag()
    {
        Vector3 offset = (Input.mousePosition - mouseReference);
        if((SpinStore.instance.transform.eulerAngles.y<301||offset.x>0)&&(SpinStore.instance.transform.eulerAngles.y>=149||offset.x<0))
        SpinStore.instance.transform.Rotate(new Vector3(0, -offset.x * 0.3f, 0));
        //CLog.Log(SpinStore.instance.transform.eulerAngles.y+" "+ offset);
        mouseReference = Input.mousePosition;
        
    }
}
