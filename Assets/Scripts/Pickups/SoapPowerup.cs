using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SoapPowerup : SpawnedPowerup
{

    public new Collider collider;
    public float enableDelay = 0.5f;
    //public ParticleSystem fx;
    public ParticleSystem soapfx;
    //public bool explosion;
    //public GameObject objeto;


    [Networked] public TickTimer CollideTimer { get; set; }

    private void Awake()
    {
        //
        // We start the collider off as disabled, because the object may be predicted, so it takes time for FUN methods
        // to be called on this object. When the object has Spawned(), then the collider will be enabled.
        //
        collider.enabled = false;
    }

    public override void Spawned()
    {
        base.Spawned();
        setPosition();
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);

        //AudioManager.PlayAndFollow("bananaDropSFX", transform, AudioManager.MixerTarget.SFX);

        //
        // We create a timer to count down so that the kart who spawned this object has time to drive away before the 
        // collider enables again. Without this, the person who drops the banana will spin themselves out!
        //
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        collider.enabled = CollideTimer.ExpiredOrNotRunning(Runner);

        //
        // We want to set this every frame because we dont want to accidentally enable this somewhere in code, because
        // that will mess up prediction somewhere.
        //
        /*if (explosion)// && !fx.isPlaying)
        {
            destroy(null);
        }*/
    }

    private void setPosition()
    {
        //transform.Translate(0 , 0, 0);
        //explosion =false;
        //fx.Stop();
        collider.enabled = false;
        //soapfx.Play();

    }
    public override bool Collide(KartEntity kart)
    {
        if (Object.IsValid && !HasInit) return false;

        collider.enabled = false;
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);

        kart.ImpactoKart(ClassPart.SOAP);
        soapfx.Stop();
        Runner.Despawn(Object, true);
        return true;
    }
    
}