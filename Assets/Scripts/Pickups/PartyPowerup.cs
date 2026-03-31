using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PartyPowerup : SpawnedPowerup
{

    public float enableDelay = 0.5f;
    public ParticleSystem fx;
    public AudioSource sfx;
    public GameObject objeto;
    public bool explosion=false;
    public Collider collider;
    [Networked] public TickTimer CollideTimer { get; set; }

    public override void Spawned()
    {
        base.Spawned();
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        setPosition();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (!fx.isPlaying && explosion) destroy(null);
    }
    public override bool Collide(KartEntity kart)
    {
        if(!explosion)
        {
            explosion = true;
            fx.Play();
            sfx.Play();
            objeto.SetActive(false);
            collider.enabled = false;
            return true;
        }
        return false;
    }
    private void setPosition()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        transform.Translate(-4, 0, 0);
        explosion = false;
        fx.Stop();
        objeto.SetActive(true);
        collider.enabled = true;
    }
    private void destroy(KartEntity kart)
    {
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        if (kart != null) kart.ImpactoKart(ClassPart.PARTY);
        Runner.Despawn(Object, true);
        setPosition();
    }
}