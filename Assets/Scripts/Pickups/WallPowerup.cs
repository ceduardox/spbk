using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class WallPowerup : SpawnedPowerup
{
    public bool AddForce = false;
    public GameObject objeto;
    public ParticleSystem fx;
    public ParticleSystem fx2;
    public bool explosion = false;
    public Collider collider;

    [Networked] public bool timerBomb { get; set; }

    public float timerToBomb;
    float realTimer;

    bool isServer;
    [Networked] public TickTimer CollideTimer { get; set; }


    /*private void Awake()
    {
        collider.enabled=false;
    }*/

    public override void Spawned()
    {
        if (GameLauncher.instance.modeServerDedicado)
            isServer = GameLauncher.instance.isServer;
        else
            isServer = RoomPlayer.Local.IsLeader;

        base.Spawned();
        setPosition();
        timerBomb = false;
        //RbKart = kartParent.Rigidbody.Rigidbody.velocity;
        //RbKart = base.RbPlayer; BRYAN
        //CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (!AddForce && isServer)
        {
            AddForce = true;
            //RbKart = kartParent.Rigidbody.Rigidbody.velocity.magnitude;
            GetComponent<Rigidbody>().AddRelativeForce(-1000f - kartParent.Rigidbody.Rigidbody.velocity.magnitude * 20, 600f, 0f);
        }
    }

    private void Update()
    {
        if (isServer)
        {
            if ((realTimer -= Time.deltaTime) < 0)
            {
                timerBomb = true;
            }
        }
        
        if (Object.IsValid&&
            timerBomb && !explosion)
        {
            CLog.Log("Exploto en todos lados, agregar los eventos en la Misma bomba");
            explosion = true;
            objeto.SetActive(false);
            fx.Play();
            fx2.Stop();
            collider.enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }
        if (Object.IsValid && 
            timerBomb && !fx.isPlaying)
        {
            destroy(null);
        }
    }
    private void setPosition()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        explosion = false;
        objeto.SetActive(true);
        fx.Stop();
        fx2.Play();
        collider.enabled = true;
        realTimer = timerToBomb;
        transform.Translate(-1, 0.5f, 0);
        AddForce = false;
        CLog.Log(transform.position);
    }

    private void destroy(KartEntity kart)
    {
        if (kart != null) kart.ImpactoKart(ClassPart.BOMB);
        Runner.Despawn(Object, true);
        setPosition();
    }
}