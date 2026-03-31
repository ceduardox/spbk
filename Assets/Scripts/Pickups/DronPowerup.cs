using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class DronPowerup : SpawnedPowerup
{

    public float enableDelay = 0.5f;
    public ParticleSystem fx;
    public AudioSource sfx;
    public GameObject objeto;
    public Rigidbody rb;
    public Collider collider;
    public Collider box;
    public float deatTimer = 10;
    public KartEntity kartenemy;

    public bool kart=false;
    public bool seguir = false;

    [Networked] public TickTimer timer { get; set; }
    [Networked] public TickTimer ColliderTimer { get; set; }
    public override void Spawned()
    {
        base.Spawned();
        setPosition();
        timer = TickTimer.CreateFromSeconds(Runner, deatTimer);
        ColliderTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        collider.enabled = ColliderTimer.ExpiredOrNotRunning(Runner);
        box.enabled = ColliderTimer.ExpiredOrNotRunning(Runner);
        if (timer.ExpiredOrNotRunning(Runner)) Runner.Despawn(Object, true);
        if(kart && seguir && !fx.isPlaying) Runner.Despawn(Object, true);

    }
    public override bool Collide(KartEntity kart)
    {
        if(kart !=kartParent)
        {
            if(kart)
            {
                if (Object.IsValid && !HasInit) return false;
                fx.Play();
                sfx.Play();
                objeto.SetActive(false);
                box.enabled = false;
                destroy(kart);
                return true;
            }
            else
            {
                collider.enabled = false;
                kartenemy = kart;
                seguir = true;
            }
        }
        return false;
    }
    private void setPosition()
    {
        kart = false;
        seguir = false;
        transform.Translate(-3f, 2f, 0f);
        objeto.SetActive(true);
        collider.enabled = false;
        box.enabled = false;
    }
    private void destroy(KartEntity kart)
    {
        if (kart != null) kart.ImpactoKart(ClassPart.DRON);
        setPosition();
    }
}