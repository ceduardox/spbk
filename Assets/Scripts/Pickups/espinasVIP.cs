using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class espinasVIP : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(gameObject.name=="spin1")this.GetComponentInParent<DFEXVIPPowerud>().spina=1;
        if(gameObject.name == "spin2")this.GetComponentInParent<DFEXVIPPowerud>().spina = 2;
        if (gameObject.name == "spin3") this.GetComponentInParent<DFEXVIPPowerud>().spina = 3;
        if (gameObject.name == "spin4") this.GetComponentInParent<DFEXVIPPowerud>().spina = 4;
        CLog.Log(gameObject.name);
    }
}
