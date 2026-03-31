using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FakeItemPowerup : SpawnedPowerup
{
    public new Collider collider;
    public float enableDelay = 0.5f;

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
        transform.Rotate(Vector3.up, 180 * Time.deltaTime*.5f);
    }

    public override bool Collide(KartEntity kart)
    {
        if (Object.IsValid && !HasInit) return false;
        destroy(kart);

        return true;
    }





    private void destroy(KartEntity kart)
    {
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        collider.enabled = false;
        if (kart != null) kart.ImpactoKart(ClassPart.FAKEITEM);
        Runner.Despawn(Object, true);
    }
}
