using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ImanPowerup : SpawnedPowerup
{
    public ClassPart _class = ClassPart.NONE;
    public new Collider collider;
    public float enableDelay = 0.5f;
    public bool seguir = false;
    public KartEntity kartenemy;
    public GameObject objeto;
    public bool dead = false;

    public float LifeTime = 20f;

    public float deadTime=5f;

    public bool timeToDie = false;
    [Networked] public TickTimer CollideTimer { get; set; }
    [Networked] public TickTimer DeadTimer { get; set; }
    [Networked] public TickTimer LifeTimer { get; set; }

    private void Awake()
    {
        collider.enabled = false;
    }

    public override void Spawned()
    {
        setPosition();
        base.Spawned();
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        LifeTimer = TickTimer.CreateFromSeconds(Runner, LifeTime);
    }

    public override void FixedUpdateNetwork()
    {
        collider.enabled = CollideTimer.ExpiredOrNotRunning(Runner);
        if (seguir)
        {
            kartenemy.transform.position = new Vector3(gameObject.transform.position.x,gameObject.transform.position.y+0.5f, gameObject.transform.position.z);
            objeto.transform.position = new Vector3(kartenemy.transform.position.x, kartenemy.transform.position.y + 1f, kartenemy.transform.position.z);
        }
        if(seguir && timeToDie)
        {
            Runner.Despawn(Object, true);
        }

        if (DeadTimer.ExpiredOrNotRunning(Runner) && !timeToDie && dead)
        {
            timeToDie = true;
            kartenemy.Rigidbody.Rigidbody.isKinematic = false;
            kartenemy.Rigidbody.Rigidbody.useGravity = true;
        }
        if(LifeTimer.ExpiredOrNotRunning(Runner)) Runner.Despawn(Object, true);
    }

    public override bool Collide(KartEntity kart)
    {
        if(!seguir)
        {
            collider.enabled = false;
            seguir = true;
            kartenemy = kart;
            kart.Rigidbody.Rigidbody.isKinematic = true;
            kart.Rigidbody.Rigidbody.useGravity = false;
            DeadTimer = TickTimer.CreateFromSeconds(Runner, deadTime);
            dead = true;
            return true;
        }
        return false;
    }


    private void setPosition()
    {
        dead = false;
        transform.Translate(-3f, 1f, 0f);
        objeto.transform.position = gameObject.transform.position;
        objeto.SetActive(true);
        seguir = false;
        collider.enabled = false;
        timeToDie = false;
    }

    private void destroy(KartEntity kart)
    {
        if (kart != null) kart.ImpactoKart(_class);
    }
}