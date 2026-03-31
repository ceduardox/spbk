using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsShower : MonoBehaviour
{
   

    // Update is called once per frame
    void Update()
    {
        
        if (GameLauncher.instance == null) return;
        GameLauncher.instance.stats.text = " fps: " + 1 / Time.deltaTime;
        //CLog.Log("stat: " + GameLauncher.instance.stats.text);
    }
}
