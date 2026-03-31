using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BalloonGravityPowerup : SpawnedPowerup
{
    public ClassPart _class = ClassPart.NONE;
    public new Collider collider;
    public float enableDelay = 0.5f;
    public bool seguir = false;
    public KartEntity kartenemy;
    public ParticleSystem fx;
    public GameObject objeto;
    public AudioSource sfx;
    public bool dead = false;

    public float deadTime=5f;

    public bool timeToDie = false;
    [Networked] public TickTimer CollideTimer { get; set; }
    [Networked] public TickTimer DeadTimer { get; set; }

    private void Awake()
    {
        collider.enabled = false;
    }

    public override void Spawned()
    {
        setPosition();
        base.Spawned();
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        
    }

    public override void FixedUpdateNetwork()
    {
        collider.enabled = CollideTimer.ExpiredOrNotRunning(Runner);
        if (seguir)
        {
            kartenemy.transform.position = new Vector3(gameObject.transform.position.x,gameObject.transform.position.y+0.5f, gameObject.transform.position.z);
            objeto.transform.position = new Vector3(kartenemy.transform.position.x, kartenemy.transform.position.y + 1f, kartenemy.transform.position.z);
        }
        if(seguir && !fx.isPlaying && timeToDie)
        {
            seguir = false;
            Runner.Despawn(Object, true);
            
        }

        if (DeadTimer.ExpiredOrNotRunning(Runner) && !timeToDie && dead)
        {
            timeToDie = true;
            objeto.SetActive(false);
            fx.Play();
            sfx.Play();
            kartenemy.Rigidbody.Rigidbody.isKinematic = false;
            kartenemy.Rigidbody.Rigidbody.useGravity = true;
        }
    }

    public override bool Collide(KartEntity kart)
    {
        if (Object.IsValid && !HasInit) return false;
        collider.enabled = false;
        seguir = true;
        kartenemy = kart;
        kart.Rigidbody.Rigidbody.isKinematic = true;
        kart.Rigidbody.Rigidbody.useGravity = false;
        DeadTimer = TickTimer.CreateFromSeconds(Runner, deadTime);
        dead = true;
        return true;
    }


    private void setPosition()
    {
        dead = false;
        transform.Translate(-3f, 1f, 0f);
        objeto.transform.position = gameObject.transform.position;
        fx.transform.position = gameObject.transform.position;
        objeto.SetActive(true);
        seguir = false;
        fx.Stop();
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        collider.enabled = false;
        timeToDie = false;
    }

    private void destroy(KartEntity kart)
    {
        if (kart != null) kart.ImpactoKart(_class);
    }
}