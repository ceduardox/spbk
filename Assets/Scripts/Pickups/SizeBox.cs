using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SizeBox : SpawnedPowerup
{
    public new Collider collider;
    public float collideTime = 1f;
    public float enableDelay = 0.2f;
    public Rigidbody missil;
    public GameObject objeto;
    public ParticleSystem fx;
    public bool explosion = false;

    public float speed=0.25f;

    public Vector3 targetScale;

    bool isServer;
    [Networked] public TickTimer timerBomb { get; set; }
    [Networked] public TickTimer ColliderTimer { get; set; }

    public float timerToBomb;
    public void Start()
    {
        targetScale = transform.localScale * 12;
    }
    public override void Spawned()
    {
        if (GameLauncher.instance.modeServerDedicado)
            isServer = GameLauncher.instance.isServer;
        else
            isServer = RoomPlayer.Local.IsLeader;
        timerBomb = TickTimer.CreateFromSeconds(Runner, timerToBomb);
        ColliderTimer = TickTimer.CreateFromSeconds(Runner, collideTime);
        setPosition();
        base.Spawned();
    }
    public override void FixedUpdateNetwork()
    {
        collider.enabled = ColliderTimer.ExpiredOrNotRunning(Runner);
        gameObject.transform.localScale = Vector3.Lerp(transform.localScale,targetScale, speed * Time.deltaTime);
        if (timerBomb.ExpiredOrNotRunning(Runner) && !explosion)
        {
            objeto.SetActive(false);
            fx.Play();
            explosion = true;
            collider.enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }
        if (explosion && !fx.isPlaying)
        {
            Runner.Despawn(Object, true);
        }
    }
    public override bool Collide(KartEntity kart)
    {
        if(!explosion)
        {
            objeto.SetActive(false);
            fx.Play();
            explosion = true;
            collider.enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
            objeto.SetActive(false);
            destroy(kart);
            return true;
        }
        return false;


    }

    private void setPosition()
    {
        objeto.SetActive(true);
        fx.Stop();
        explosion = false;
        collider.enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        transform.Translate(-6, 0, 0);
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);

    }
    private void destroy(KartEntity kart)
    {
        isServer = false;
        if (kart != null) kart.ImpactoKart(ClassPart.MISSILE);
    }
}