using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Start is called before the first frame update

    public Vector3 axies;
    // Update is called once per frame
    void Update()
    {
        transform.transform.Rotate(axies * 100*Time.deltaTime, Space.Self) ;
    }
}
