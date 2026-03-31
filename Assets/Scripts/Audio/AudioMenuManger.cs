using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMenuManger : MonoBehaviour
{
    private void OnEnable()
    {
        //if()
        AudioEnviromentControl.menuEnabled("menu");
        
    }
}
