using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MuroEspinasPowerup : SpawnedPowerup
{
    public new Collider collider;
    public float enableDelay = 1f;
    public ParticleSystem fx;
    public AudioSource sfx;
    public GameObject objeto;
    public bool explosion=false;

    public bool dead = false;
    [Networked] public TickTimer CollideTimer { get; set; }

    public override void Spawned()
    {
        
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        setPosition();
        base.Spawned();
        
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        collider.enabled = CollideTimer.ExpiredOrNotRunning(Runner);
        if (!fx.isPlaying && dead) Runner.Despawn(Object, true);
    }

    public override bool Collide(KartEntity kart)
    {
        if(!dead)
        {
            collider.enabled = false;
            fx.Play();
            sfx.Play();
            objeto.SetActive(false);
            kart.Rigidbody.Rigidbody.AddRelativeForce(0, 200f, -1000f + kartParent.Rigidbody.Rigidbody.velocity.magnitude * 20);
            dead = true;
            destroy(kart);
            return true;
        }
        return false;
    }

    private void setPosition()
    {
        transform.Translate(-1f, 0.5f, 0);
        explosion = false;
        collider.enabled = false;
        fx.Stop();
        sfx.Stop();
        objeto.SetActive(true);
        dead = false;

    }
    private void destroy(KartEntity kart)
    {
        if (kart != null) kart.ImpactoKart(ClassPart.MUROESPINAS);
    }
}
