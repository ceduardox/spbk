using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class superBombPowerup : SpawnedPowerup
{

    public new Collider collider;
    public float enableDelay = 0.5f;
    public float turnTime = 4f;
    public bool seguir = false;
    public bool player = false;
    KartEntity kartenemy;

    public Rigidbody rb;
    [Networked] public TickTimer CollideTimer { get; set; }
    [Networked] public TickTimer deadTiner { get; set; }
    [Networked] public TickTimer destroyTiner { get; set; }

    private void Awake()
    {
        collider.enabled = false;
    }

    public override void Spawned()
    {
        base.Spawned();
        Tamano();
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        deadTiner = TickTimer.CreateFromSeconds(Runner, turnTime);
        setPosition();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if(CollideTimer.ExpiredOrNotRunning(Runner))
        {
            collider.enabled = CollideTimer.ExpiredOrNotRunning(Runner);
            
        }
        if(deadTiner.ExpiredOrNotRunning(Runner))
        {
            if(!player)
            {
                rb.useGravity = true;
            }
            player = false;
            deadTiner = TickTimer.CreateFromSeconds(Runner, 3f);
        }
        //if (destroyTiner.ExpiredOrNotRunning(Runner)) destroy(null);
        if(seguir && !player)
        {
            gameObject.transform.position = new Vector3(kartenemy.transform.position.x, kartenemy.transform.position.y + 1f, kartenemy.transform.position.z);
        }
        if (seguir && player)
        {
            gameObject.transform.position = new Vector3(kartParent.transform.position.x, kartParent.transform.position.y + 1f, kartParent.transform.position.z);
        }
        gameObject.transform.Rotate(Vector3.up, 180 * Time.deltaTime);
    }

    public override bool Collide(KartEntity kart)
    {
        if(kartParent!=kart)
        {
            if (Object.IsValid && !HasInit) return false;
            destroy(kart);
            return true;
        }
        return false;
    }
    private void destroy(KartEntity kart)
    {
        deadTiner = TickTimer.CreateFromSeconds(Runner, turnTime);
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        collider.enabled = false;
        if (kart != null) kart.ImpactoKart(ClassPart.SUPERBOMB);
        Runner.Despawn(Object, true);
        setPosition();
    }

    public void setPosition()
    {
        seguir = false;
        player = false;
    }
    public void Tamano()
    {
        List<KartEntity> karts = KartEntity.Karts;

        foreach (var kart in KartEntity.Karts)
        {
            if(kart.Kart.position!=kartParent.Kart.position)
            {
                if(kart.Kart.position==1)
                {
                    //destroyTiner = TickTimer.CreateFromSeconds(Runner, 2f);
                    kartenemy = kart;
                    player = true;
                    seguir = true;
                }
            }
        }
    }
}