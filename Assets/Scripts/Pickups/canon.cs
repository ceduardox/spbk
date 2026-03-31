using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canon : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        this.GetComponentInParent<CanonPowerud>().disparo = true;
        CLog.Log(gameObject.name);
    }
}
