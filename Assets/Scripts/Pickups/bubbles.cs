using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bubbles : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(gameObject.name=="spin1")this.GetComponentInParent<BubblesPowerud>().spina=1;
        if(gameObject.name == "spin2")this.GetComponentInParent<BubblesPowerud>().spina = 2;
        CLog.Log(gameObject.name);
    }
}
