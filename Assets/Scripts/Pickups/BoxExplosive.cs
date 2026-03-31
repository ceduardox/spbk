using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BoxExplosive : SpawnedPowerup
{
    public new Collider collider;
    public float enableDelay;
    public AudioSource explosion;
    public AudioSource sfx;
    public ParticleSystem fx;
    public GameObject objeto;
    public bool dead;

    [Networked] public TickTimer CollideTimer { get; set; }

    public override void Spawned()
    {
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        setPosition();
        base.Spawned();
    }

    bool isServer;

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        collider.enabled = CollideTimer.ExpiredOrNotRunning(Runner);
        gameObject.transform.Rotate(Vector3.up, 180 * Time.deltaTime);
        if (dead && !fx.isPlaying) Runner.Despawn(Object, true);
    }
    public override bool Collide(KartEntity kart)
    {
        if(!dead)
        {
            dead = true;
            collider.enabled = false;
            explosion.Play();
            fx.Play();
            sfx.Stop();
            objeto.SetActive(false);
            destroy(kart);
            return true;
        }
        return false;
    }

    private void setPosition()
    {
        sfx.Play();
        transform.Translate(-1f, 0.75f, 0f);
        dead = false;
        collider.enabled = false;
    }
    private void destroy(KartEntity kart)
    {
        collider.enabled = false;
        if (kart != null)kart.ImpactoKart(ClassPart.MISSILE);
    }
}