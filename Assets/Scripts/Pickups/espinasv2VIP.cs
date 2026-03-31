using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class espinasv2VIP : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(gameObject.name=="spin1")this.GetComponentInParent<espinasv2VIPPowerud>().spina=1;
        if(gameObject.name == "spin2")this.GetComponentInParent<espinasv2VIPPowerud>().spina = 2;
        if (gameObject.name == "spin3") this.GetComponentInParent<espinasv2VIPPowerud>().spina = 3;
        if (gameObject.name == "spin4") this.GetComponentInParent<espinasv2VIPPowerud>().spina = 4;
        CLog.Log(gameObject.name);
    }
}
