using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagLoad : MonoBehaviour
{
    public Material material;
    public bool loadFlag(string _name)
    {
        material.SetTexture("_MainTex", Resources.Load<Texture2D>("Prefabs/IconKarts/Accesorios/Flags/"+_name.Replace("ALL_KARTS-Flag_", "")));
        if (material.mainTexture != null)
            return true;
        CLog.Log("Cargo FALSE: " + "Prefabs / IconKarts / Accesorios / Flags / " + _name.Replace("ALL_KARTS-Flag_", ""));
        return false;
        
    }
}
