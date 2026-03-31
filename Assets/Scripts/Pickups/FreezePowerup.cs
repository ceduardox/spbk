using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FreezePowerup : SpawnedPowerup
{

    public new Collider collider;
    public float enableDelay = 0.5f;
    public KartEntity kartenemy;
    public AudioSource sfx;
    public bool reversa = false;

    public float DeadTime=5;
    public bool dead=false;
    [Networked] public TickTimer CollideTimer { get; set; }
    [Networked] public TickTimer DeadTimer { get; set; }

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
        if (DeadTimer.ExpiredOrNotRunning(Runner) && dead)
        {
            kartenemy.Rigidbody.Rigidbody.isKinematic = false;
            dead = false;
            Runner.Despawn(Object, true);
        }
    }

    public override bool Collide(KartEntity kart)
    {
        if (!dead)
        {
            collider.enabled = false;
            kartenemy = kart;
            sfx.Play();
            kartenemy.Rigidbody.Rigidbody.isKinematic = true;
            DeadTimer = TickTimer.CreateFromSeconds(Runner, DeadTime);
            dead = true;
        }
        return true;
    }
}