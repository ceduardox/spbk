using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minas : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.name == "mina1") this.GetComponentInParent<minasvipPowerud>().mina = 1;
        if (gameObject.name == "mina2") this.GetComponentInParent<minasvipPowerud>().mina = 2;
        if (gameObject.name == "mina3") this.GetComponentInParent<minasvipPowerud>().mina = 3;
        CLog.Log(gameObject.name);
    }
}
