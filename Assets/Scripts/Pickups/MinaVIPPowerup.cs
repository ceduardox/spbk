using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MinaVIPPowerup : SpawnedPowerup
{


    public ClassPart _class = ClassPart.NONE;

    public override void Spawned()
    {
        if (_class == ClassPart.MINAVIP)
        {
            foreach (Transform t in transform)
                t.GetComponent<MinaPowerup>().kartParent = kartParent;
        }
        

    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if(transform.childCount==0)
            Runner.Despawn(Object, true);
        
    }

}