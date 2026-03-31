using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MinaPowerup : SpawnedPowerup
{

    public new Collider collider;
    public float enableDelay = 0.5f;
    public ClassPart classPowerUp;
    //public AutoDestroy fx;
    [Networked] public TickTimer CollideTimer { get; set; }

    private void Awake()
    {
        collider.enabled = false;
    }

    public override void Spawned()
    {
        base.Spawned();
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
    }

    public override void FixedUpdateNetwork()
    {


        base.FixedUpdateNetwork();
        collider.enabled = CollideTimer.ExpiredOrNotRunning(Runner);

        if (kartParent == null)
        {
            if(transform.parent&&
                transform.parent.GetComponent<MinaVIPPowerup>())
                kartParent = transform.parent.GetComponent<MinaVIPPowerup>().kartParent;
        }
    }

    public override bool Collide(KartEntity kart)
    {
        if (Object.IsValid && !HasInit) return false;

       /* if (fx)
        {
            Instantiate(fx, kart.transform.position, kart.transform.rotation);
        }*/

        destroy(kart);

        return true;
    }

    private void destroy(KartEntity kart)
    {

        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        collider.enabled = false;
        if (kart != null) kart.ImpactoKart(classPowerUp);
        if(Object.HasStateAuthority)GameLauncher.expancionFX(expancion,kart.transform,kartParent.transform);
        Runner.Despawn(Object, true);
    }





}