using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class DFEXVIPPowerud : SpawnedPowerup
{
    public GameObject collider;
    public Collider spin1;
    public Collider spin2;
    public Collider spin3;
    public Collider spin4;
    public GameObject objeto1;
    public GameObject objeto2;
    public GameObject objeto3;
    public GameObject objeto4;
    public ParticleSystem fx1;
    public ParticleSystem fx2;
    public ParticleSystem fx3;
    public ParticleSystem fx4;
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
        transform.Rotate(Vector3.up, 180 * Time.deltaTime);
        transform.position = new Vector3(kartParent.transform.position.x, kartParent.transform.position.y + 0.5f, kartParent.transform.position.z);
        if (!spin1.enabled && !spin2.enabled && !spin3.enabled && !spin4.enabled && !fx1.isPlaying && !fx2.isPlaying && !fx3.isPlaying && !fx4.isPlaying)
        {
            Runner.Despawn(Object, true);
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
                    fx1.Play();
                    break;
                case 2:
                    objeto2.SetActive(false);
                    spin2.enabled = false;
                    fx2.Play();
                    break;
                case 3:
                    objeto3.SetActive(false);
                    spin3.enabled = false;
                    fx3.Play();
                    break;
                case 4:
                    objeto4.SetActive(false);
                    spin4.enabled = false;
                    fx4.Play();
                    break;
            }
            kart.Rigidbody.Rigidbody.AddRelativeForce(0,200f, -500f + kartParent.Rigidbody.Rigidbody.velocity.magnitude * 25);
            if (Object.IsValid && !HasInit) return false;
            explosion.Play();
            destroy(kart);
            return true;
        }
     else return false;


    }

    private void setPosition()
    {
        transform.Translate(0, 0.5f, 0);
        collider.SetActive(false);
        objeto1.SetActive(true);
        spin1.enabled = true;
        objeto2.SetActive(true);
        spin2.enabled = true;
        objeto3.SetActive(true);
        spin3.enabled = true;
        objeto4.SetActive(true);
        spin4.enabled = true;

    }
    private void destroy(KartEntity kart)
    {
        if (kart != null)kart.ImpactoKart(ClassPart.SPINSVIP);
    }
}