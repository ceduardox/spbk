using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SmokeAdhesivePowerup : SpawnedPowerup
{

    public new Collider collider;
    public Collider smoke;
    public float enableDelay = 0.5f;
    public bool seguir = false;
    public ParticleSystem fx;
    public AudioSource sfx;
    public Rigidbody rigidbody;
    public GameObject objeto;
    [Networked] public TickTimer CollideTimer { get; set; }

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
        collider.enabled = CollideTimer.ExpiredOrNotRunning(Runner)&& !seguir;
        if (seguir)
        {
            CLog.Log("ESTO VALE: " + objeto.activeSelf);
            gameObject.transform.position = new Vector3(kartTarget.transform.position.x,kartTarget.transform.position.y+0.25f,kartTarget.transform.position.z);
            transform.rotation = kartParent.transform.rotation;
            fx.transform.Rotate(new Vector3(1,0,0), 45 * Time.deltaTime);
        }
        if(seguir && !fx.isPlaying)
        {
            Runner.Despawn(Object, true);
        }
    }

    public override bool Collide(KartEntity kart)
    {
        if(!seguir)
        { 
            collider.enabled = false;
            smoke.enabled = false;
            seguir = true;
            kartTarget = kart;
            rigidbody.isKinematic = true;
            objeto.SetActive(false);
            fx.Play();
            sfx.Play();
        }
        return true;
    }


    private void setPosition()
    {
        transform.Translate(-.5f, 0.25f, 0f);
        objeto.SetActive(true);
        seguir = false;
        collider.enabled = false;
        smoke.enabled = true;
        fx.Stop();
        sfx.Stop();
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        rigidbody.isKinematic = false;
    }
}