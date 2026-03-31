using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BalloonSpinasPowerup : SpawnedPowerup
{

    public float enableDelay = 0.5f;
    public ParticleSystem fx;
    public ParticleSystem fxBalloon;
    public AudioSource sfx;
    public GameObject objeto;
    public GameObject balloon;
    public Rigidbody rb;
    public Collider collider;
    public Collider box;
    public bool move=true;

    public override void Spawned()
    {
        base.Spawned();
        setPosition();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if(move)rb.AddForce(new Vector3(-5f * Time.deltaTime, 0f, 0f));
        if(!fx.isPlaying && !fxBalloon.isPlaying && !move) Runner.Despawn(Object, true);

    }
    public override bool Collide(KartEntity kart)
    {
        if (Object.IsValid && !HasInit) return false;
        move = false;
        fx.Play();
        fxBalloon.Play();
        sfx.Play();
        objeto.SetActive(false);
        balloon.SetActive(false);
        collider.enabled = false;
        box.enabled = false;
        destroy(kart);
        return true;
    }
    private void setPosition()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z);
        transform.Translate(-4f, 0.75f, 0f);
        transform.rotation = kartParent.transform.rotation;
        objeto.SetActive(true);
        balloon.SetActive(true);
        collider.enabled = true;
        box.enabled = true;
    }
    private void destroy(KartEntity kart)
    {
        if (kart != null) kart.ImpactoKart(ClassPart.BALLOONSPINAS);
    }
}