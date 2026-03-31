using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuego : MonoBehaviour
{
    private Light fuego;
    public float rangeMax=10;
    public float rangeMin=8;
    // Start is called before the first frame update
    

    void Start()
    {
        StartCoroutine(fuegoCR());
        fuego=GetComponent<Light>();
    }
    // Update is called once per frame
    IEnumerator fuegoCR()
    {
        while(true)
        {
            yield return new WaitForSeconds(.2f);
            fuego.range=Random.Range(rangeMin, rangeMax);

        }
    }
    
}
