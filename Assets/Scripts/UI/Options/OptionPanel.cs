using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionPanel : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TimerCounterScreen._instance)
        {
            //CLog.Log("SSSSSSSSSSSS" + TimerCounterScreen._instance.permitirCambios);
            if (!TimerCounterScreen._instance.permitirCambios)
            {

                //back();
                GetComponent<UIScreen>().Back();
                gameObject.SetActive(false); 

            }
        }
    }
}
