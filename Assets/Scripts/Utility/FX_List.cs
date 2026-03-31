using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FX_Type
    {
    explosionKarts,
    freeze,
    electricity,
    bubbles,
    gravity,
    explosion

}
public class FX_List : MonoBehaviour
{
    //public static FX_List instance;
    public List<effec> effects;
    public static Dictionary<FX_Type, AutoDestroy> effectsList;
    void Awake()
    {
        //instance = this;
        effectsList = new Dictionary<FX_Type, AutoDestroy>();
        foreach (effec fx in effects)
            effectsList.Add(fx.type, fx.fx);
    }

    public static bool playEffect(FX_Type _fx, Transform _target)
    {
        AutoDestroy fx=effectsList[_fx];
        if(fx)
        {
            Instantiate(fx, _target.transform.position, _target.transform.rotation).setTraget(_target);
            
            return true;
        }
        return false;
    }

}


[System.Serializable]
public class effec
{
    [Header("Effect")]
    public FX_Type type;
    public AutoDestroy fx;
}
