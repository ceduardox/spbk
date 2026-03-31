using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BubblesPowerud : SpawnedPowerup
{
    public GameObject collider;
    public Collider spin1;
    public Collider spin2;
    public GameObject objeto1;
    public GameObject objeto2;
    public ParticleSystem fx1;
    public ParticleSystem fx2;
    public float enableDelay;
    public AudioSource explosion;
    public int spina = 0;


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
        if(CollideTimer.ExpiredOrNotRunning(Runner))
        {
            collider.SetActive(true);
        }
        transform.position = new Vector3(kartParent.transform.position.x, kartParent.transform.position.y + 0.65f, kartParent.transform.position.z);
        transform.rotation = kartParent.transform.rotation;
        if (!spin1.enabled && !spin2.enabled && !fx1.isPlaying && !fx2.isPlaying)
        {
            Runner.Despawn(Object, true);
            setPosition();
        }
    }
    public void Prueba()
    {
        CLog.Log(spina);
    }
    public override bool Collide(KartEntity kart)
    {
      if (kart != kartParent)
        {
            switch(spina)
            {
                case 1:
                    objeto1.SetActive(false);
                    spin1.enabled = false;
                    kart.Rigidbody.Rigidbody.AddRelativeForce(500f + kartParent.Rigidbody.Rigidbody.velocity.magnitude * 30, 400f,0);
                    explosion.Play();
                    fx1.Play();
                    break;
                case 2:
                    objeto2.SetActive(false);
                    spin2.enabled = false;
                    kart.Rigidbody.Rigidbody.AddRelativeForce(+500f + kartParent.Rigidbody.Rigidbody.velocity.magnitude * 30, 400f, 0);
                    explosion.Play();
                    fx2.Play();
                    break;
            }
            if (Object.IsValid && !HasInit) return false;
            explosion.Play();
            destroy(kart);
            return true;
        }
     else return false;
    }

    private void setPosition()
    {
        transform.Translate(0, 0.65f, 0);
        collider.SetActive(false);
        objeto1.SetActive(true);
        spin1.enabled = true;
        objeto2.SetActive(true);
        spin2.enabled = true;

    }
    private void destroy(KartEntity kart)
    {
        if (kart != null)kart.ImpactoKart(ClassPart.BUBBLEDF);
    }
}