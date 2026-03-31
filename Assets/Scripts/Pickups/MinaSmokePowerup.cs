using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MinaSmokePowerup : SpawnedPowerup
{

    public float enableDelay = 0.5f;
    public ParticleSystem fx;

    [Networked] public TickTimer CollideTimer { get; set; }

    public override void Spawned()
    {
        base.Spawned();

        //AudioManager.PlayAndFollow("bananaDropSFX", transform, AudioManager.MixerTarget.SFX);

        //
        // We create a timer to count down so that the kart who spawned this object has time to drive away before the 
        // collider enables again. Without this, the person who drops the banana will spin themselves out!
        //
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
    }

    private void Update()
    {
        if (!fx.isPlaying) destroy(null);
    }
    private void destroy(KartEntity kart)
    {
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        if (kart != null) kart.ImpactoKart(ClassPart.MINA);
        Runner.Despawn(Object, true);
    }





}