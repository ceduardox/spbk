using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SizePowerup : SpawnedPowerup
{

    public new Collider collider;
    public float enableDelay = 0.5f;
    public Transform kartScale;
    public KartEntity kartenemy;
    public GameObject objeto; 
    public AudioSource sfx;
    public AudioSource sfx2;
    public ParticleSystem fx;

    public float timeToDie = (float)(GameLauncher.timeToRespawn*0.9);

    public float timeToSize = 5f;

    public bool cancel = false;
    [Networked] public TickTimer CollideTimer { get; set; }

    [Networked] public TickTimer RespawnTimer { get; set; }

    [Networked] public TickTimer SizeTimer { get; set; }

    private void Awake()
    {
        collider.enabled = false;
    }

    public override void Spawned()
    {
        base.Spawned();
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        RespawnTimer = TickTimer.CreateFromSeconds(Runner, timeToDie);
        setPosition();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        collider.enabled = CollideTimer.ExpiredOrNotRunning(Runner);
        if (RespawnTimer.ExpiredOrNotRunning(Runner) && !cancel)
        {
            destroy(null);
        }
        if (SizeTimer.ExpiredOrNotRunning(Runner) && cancel)
        {
            cancel = false;
            kartenemy.transform.localScale = new Vector3(1f, 1f, 1f);
            destroy(null);
        }


    }

    public override bool Collide(KartEntity kart)
    {
        if(!cancel)
        {
            if (Object.IsValid && !HasInit) return false;
            RespawnTimer = TickTimer.CreateFromSeconds(Runner,timeToSize);
            collider.enabled = false;
            cancel = true;
            kartScale = kart.transform;
            kartenemy = kart;
            objeto.SetActive(false);
            sfx.Play();
            sfx2.Stop();
            fx.Play();
            kartenemy.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            SizeTimer = TickTimer.CreateFromSeconds(Runner, timeToSize);
        }
        return true;
    }



    private void setPosition()
    {
        cancel = false;
        objeto.SetActive(true);
        sfx2.Play();
    }

    private void destroy(KartEntity kart)
    {
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        RespawnTimer = TickTimer.CreateFromSeconds(Runner, timeToDie);
        SizeTimer = TickTimer.CreateFromSeconds(Runner, timeToSize);
        collider.enabled = false;
        if (kart != null) kart.ImpactoKart(ClassPart.MINA);
        Runner.Despawn(Object, true);
        setPosition();
    }
}