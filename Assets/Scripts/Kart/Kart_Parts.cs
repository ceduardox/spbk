using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kart_Parts : MonoBehaviour
{
    internal Transform exhaust_L;
    internal Transform exhaust_R;
    public ClassPart  classPart;
    public float damage = 0;
    public bool active=true;
    public void StartExhaust()
    {
        if (transform.childCount > 0)
        {
            exhaust_L = transform.GetChild(0); 
            exhaust_R = transform.GetChild(1); 
        }
    
    }

    public Vector3 render()
    {
        if (classPart == ClassPart.ANTENNA)
            return transform.position;

        if(transform.childCount>0)
            return GetComponentInChildren<Renderer>().bounds.center;
        else
            return GetComponent<Renderer>().bounds.center;
    }

    public Transform returnPart()
    {
        if (classPart == ClassPart.ANTENNA)
            return transform;
        if (transform.childCount > 0)
            return transform.GetChild(0);
        else
            return transform;

    }

    // Start is called before the first frame update


    // Update is called once per frame 

}
