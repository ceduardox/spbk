using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class OilPowerup : SpawnedPowerup
{
    public new Collider collider;
    public float enableCollider=1f;
    public bool AddForce = false;
    public ParticleSystem fx2;
    public GameObject objeto;
    public GameObject objeto2;
    public bool dead = false;
    public AudioSource sfx;
    public Rigidbody rigidbody;

    public float deadTime=20f;
    [Networked] public TickTimer DeadTimer { get; set; }

    bool isServer;
    [Networked] public TickTimer CollideTimer { get; set; }

    public override void Spawned()
    {
        base.Spawned();
        if (GameLauncher.instance.modeServerDedicado)
            isServer = GameLauncher.instance.isServer;
        else
            isServer = RoomPlayer.Local.IsLeader;
        setPosition();
        CollideTimer = TickTimer.CreateFromSeconds(Runner,enableCollider);
        DeadTimer = TickTimer.CreateFromSeconds(Runner, deadTime);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        collider.enabled = CollideTimer.ExpiredOrNotRunning(Runner);
        if (!AddForce && isServer)
        {
            AddForce = true;
            rigidbody.AddRelativeForce(500f+ kartParent.Rigidbody.Rigidbody.velocity.magnitude*20, 200f, 0f);
        }
        if (dead && !fx2.isPlaying) readyDespawn();
        if (DeadTimer.ExpiredOrNotRunning(Runner) && !dead)
        {
            dead = true;
            fx2.Play();
            collider.enabled = false;
            objeto2.SetActive(false);
            objeto.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        sfx.Play();
        rigidbody.isKinematic=true;
        objeto.SetActive(false);
        objeto2.SetActive(true);
    }
    public override bool Collide(KartEntity kart)
    {
        if (Object.IsValid && !HasInit) return false;
            if (!dead)
            {
                collider.enabled = false;
                objeto2.SetActive(false);
                objeto.SetActive(false);
                dead = true;
                fx2.Play();
                destroy(kart);
                return true;
            }
        return false;
    }
    private void setPosition()
    {
        collider.enabled = false;
        transform.Translate(2f,1f,0f);
        dead = false;
        rigidbody.isKinematic = false;
        objeto.SetActive(true);
        sfx.Stop();
        objeto2.SetActive(false);
        fx2.Stop(); 
        AddForce = false;
    }

    private void destroy(KartEntity kart)
    {
        if (kart != null) kart.ImpactoKart(ClassPart.BANANA);
    }
    void readyDespawn()
    {
        Runner.Despawn(Object, true);
    }
}
