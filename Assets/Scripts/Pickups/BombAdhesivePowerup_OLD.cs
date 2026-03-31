using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BombAdhesivePowerup_OLD : SpawnedPowerup
{

    public new Collider collider;
    public Collider smoke;
    public float enableDelay = 0.5f;
    public bool seguir = false;
    public KartEntity kartenemy;
    public ParticleSystem fx;
    public Rigidbody rigidbody;
    public GameObject objeto;
    public AudioSource sfx;
    public AudioSource sfx2;

    public float deadTime = 5f;
    public bool timeToDie = false;
    [Networked] public TickTimer CollideTimer { get; set; }
    [Networked] public TickTimer deadTimer { get; set; }

    private void Awake()
    {
        collider.enabled = false;
    }
    bool isServer;
    public override void Spawned()
    {
        setPosition();
        base.Spawned();
        if (GameLauncher.instance.modeServerDedicado)
            isServer = GameLauncher.instance.isServer;
        else
            isServer = RoomPlayer.Local.IsLeader;


        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        deadTimer = TickTimer.CreateFromSeconds(Runner, deadTime);
        
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        collider.enabled = CollideTimer.ExpiredOrNotRunning(Runner);
        if (seguir)//BUG seguir con el objeto Arriba
        {
            gameObject.transform.position = new Vector3(kartenemy.transform.position.x,kartenemy.transform.position.y+1f,kartenemy.transform.position.z);
        }
        if(seguir && !fx.isPlaying && timeToDie)
        {
            Runner.Despawn(Object, true);
        }
        if(deadTimer.ExpiredOrNotRunning(Runner) && !timeToDie)
        {
            timeToDie = true;
            objeto.SetActive(false);
            fx.Play();
            sfx.Play();
            sfx2.Stop(); 
            destroy(kartenemy);
        }

    }

    public override bool Collide(KartEntity kart)
    {
        if (Object.IsValid && !HasInit) return false;//repetir en cada powerup


        collider.enabled = false;
        smoke.enabled = false;
        seguir = true;
        kartenemy = kart;
        rigidbody.isKinematic = true;
        gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        return true;
    }


    private void setPosition()
    {
        transform.Translate(-0.5f, 0.25f, 0f);
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        objeto.SetActive(true);
        seguir = false;
        smoke.enabled = true;
        fx.Stop();
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        deadTimer = TickTimer.CreateFromSeconds(Runner, deadTime);
        collider.enabled = false;
        timeToDie = false;
        rigidbody.isKinematic = false;
    }

    private void destroy(KartEntity kart)
    {
        if (kart != null) kart.ImpactoKart(ClassPart.BOMBADHESIVE);
    }
}