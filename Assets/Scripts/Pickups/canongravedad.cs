using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canongravedad : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        this.GetComponentInParent<CanonGravedadPowerud>().disparo = true;
        CLog.Log(gameObject.name);
    }
}
