using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    Vector3 cameraMobile = new Vector3(0f, 1.5f, -1.4f);
    private void Awake()
    {
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            gameObject.transform.localPosition = cameraMobile;
    
        }
        else
        {
            gameObject.transform.localPosition = cameraMobile; 

        }
    }
}
