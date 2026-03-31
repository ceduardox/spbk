using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BombpartyPowerup : SpawnedPowerup
{
    public new Collider collider;
    public bool AddForce = false;
    public ParticleSystem fx;
    public GameObject objeto;
    public bool explosion = false;
    public AudioSource sfx;
    public float enableDelay;

    bool isServer;
    [Networked] public TickTimer CollideTimer { get; set; }
    
    public override void Spawned()
    {
        if (GameLauncher.instance.modeServerDedicado)
            isServer = GameLauncher.instance.isServer;
        else
            isServer = RoomPlayer.Local.IsLeader;

        base.Spawned();
        setPosition();
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        
        if(!AddForce && isServer)
        {
            AddForce = true;
            GetComponent<Rigidbody>().AddRelativeForce(500f+ kartParent.Rigidbody.Rigidbody.velocity.magnitude*30, 200f, 0f);
        }
    }

    private void Update()
    {
        if(explosion && !fx.isPlaying)
        {
            destroy(null);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        sfx.Play();
        CLog.Log("Exploto en todos lados, agregar los eventos en la Misma bomba");
        objeto.SetActive(false);
        fx.Play();
        explosion = true;
        collider.enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }
    private void setPosition()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        objeto.SetActive(true);
        collider.enabled = true;
        explosion = false;
        fx.Stop();

        transform.Translate(3, 0.5f, 0);
        AddForce = false;
        CLog.Log(transform.position);
    }

    private void destroy(KartEntity kart)
    {
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        collider.enabled = false;
        if (kart != null) kart.ImpactoKart(ClassPart.BOMBPARTY);
        Runner.Despawn(Object, true);
        setPosition();
    }
}
