using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Child_Checkpoint : MonoBehaviour
{
    Checkpoint parent;
    private void Start()
    {
        parent = transform.parent.GetComponent<Checkpoint>();
        GetComponent<Renderer>().enabled = false;
    }
    // Start is called before the first frame update
    private void OnTriggerStay(Collider other)
    {
        parent.OnTriggerStay(other);
    }
}
