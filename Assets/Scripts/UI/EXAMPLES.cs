using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXAMPLES : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
interface IKillable
{
    //int Daño { get; set; }

    void matar()
    {
        CLog.Log("testeando");
    }
}
public class example : MonoBehaviour, IKillable
{
    int _daño;
}
