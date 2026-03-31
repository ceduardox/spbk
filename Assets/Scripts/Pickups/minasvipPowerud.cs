using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class minasvipPowerud : SpawnedPowerup
{
    public GameObject collider;
    public Collider mina1;
    public Collider mina2;
    public Collider mina3;
    public GameObject objeto1;
    public GameObject objeto2;
    public GameObject objeto3;
    public float enableDelay;
    public AudioSource explosion;
    public int mina = 0;

    [Networked] public TickTimer CollideTimer { get; set; }

    public override void Spawned()
    {
        if (GameLauncher.instance.modeServerDedicado)
            isServer = GameLauncher.instance.isServer;
        else
            isServer = RoomPlayer.Local.IsLeader;
        enabled = true;
        base.Spawned();
        setPosition();
        
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
    }

    bool isServer;

    public override void FixedUpdateNetwork()
    {
        //base.FixedUpdateNetwork();
        if(CollideTimer.ExpiredOrNotRunning(Runner))
        {
            collider.SetActive(true);
        }
        if(!mina1.enabled && !mina2.enabled && !mina3.enabled)
        {
            Runner.Despawn(Object, true);
            setPosition();
        }
    }
    public override bool Collide(KartEntity kart)
    {
        CLog.Log(mina);
        if (kart != kartParent)
        {
            if (Object.IsValid && !HasInit) return false;
            switch (mina)
            {
                case 1:
                    objeto1.SetActive(false);
                    mina1.enabled = false;
                    break;
                case 2:
                    objeto2.SetActive(false);
                    mina2.enabled = false;
                    break;
                case 3:
                    objeto3.SetActive(false);
                    mina3.enabled = false;
                    break;
            }

            explosion.Play();
            destroy(kart);
            return true;
        }
        else return false;
        
    }

    private void setPosition()
    {
        transform.Translate(-1f, 0.5f, 0f);
        collider.SetActive(false);
        objeto1.SetActive(true);
        mina1.enabled = true;
        objeto2.SetActive(true);
        mina2.enabled = true;
        objeto3.SetActive(true);
        mina3.enabled = true; 

    }
    private void destroy(KartEntity kart)
    {
        if (kart != null)kart.ImpactoKart(ClassPart.MINAVIP);
    }
}