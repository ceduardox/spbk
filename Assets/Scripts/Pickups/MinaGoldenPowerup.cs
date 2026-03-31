using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MinaGoldenPowerup : SpawnedPowerup
{
    public ClassPart _class=ClassPart.NONE;
    public new Collider collider;
    public float enableDelay = 2f;
    public ParticleSystem fx;
    public bool dead;

    [Networked] public TickTimer CollideTimer { get; set; }

    private void Awake()
    {
        collider.enabled = false;
        dead = false;
        fx.Stop();
    }

    public override void Spawned()
    {
        base.Spawned();
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (dead && !fx.isPlaying) destroy(null);
        collider.enabled = CollideTimer.ExpiredOrNotRunning(Runner);
    }
    public override bool Collide(KartEntity kart)
    {
        if(!dead)
        {
            dead = true;
            fx.Play();
            destroy(kart);
            collider.enabled = false;
            return true;
        }
        return false;
    }
    private void destroy(KartEntity kart)
    {
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        collider.enabled = false;
        if (kart != null) kart.ImpactoKart(_class);
        Runner.Despawn(Object, true);
    }
}