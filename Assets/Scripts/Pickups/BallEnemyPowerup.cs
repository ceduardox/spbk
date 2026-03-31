using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BallEnemyPowerup : SpawnedPowerup
{
    public new Collider collider;
    public float collideTime;
    public bool AddForce = false;
    public ParticleSystem fxin;
    public ParticleSystem fxout;
    public GameObject objeto;
    public bool explosion = false;
    public AudioSource sfx;
    bool isServer;

    public bool timeToDie = false;
    public float deadTime;

    [Networked] public TickTimer CollideTimer { get; set; }
    [Networked] public TickTimer DeadTimer { get; set; }

    public override void Spawned()
    {
        if (GameLauncher.instance.modeServerDedicado)
            isServer = GameLauncher.instance.isServer;
        else
            isServer = RoomPlayer.Local.IsLeader;
        
        CollideTimer = TickTimer.CreateFromSeconds(Runner, collideTime);
        DeadTimer = TickTimer.CreateFromSeconds(Runner, deadTime);
        setPosition();
        base.Spawned();
        
    }

    public override void FixedUpdateNetwork() 
    {
        base.FixedUpdateNetwork();
        collider.enabled = CollideTimer.ExpiredOrNotRunning(Runner);
        if(!AddForce && isServer)
        {
            AddForce = true;
            GetComponent<Rigidbody>().AddRelativeForce(300f+ kartParent.Rigidbody.Rigidbody.velocity.magnitude*15, 200f, 0f);
        }
        if (DeadTimer.ExpiredOrNotRunning(Runner)&&!timeToDie)
        {
            fxout.transform.position = gameObject.transform.position;
            fxout.Play();
            objeto.SetActive(false);
            collider.enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
            sfx.Play();
            timeToDie = true;
        }
        if(!fxin.isPlaying && !fxout.isPlaying && timeToDie)
        {
            readyDespawn();
        }
    }

    public override bool Collide(KartEntity kart)
    {
        if (!timeToDie)
        {
            fxin.Play();
            objeto.SetActive(false);
            collider.enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
            sfx.Play();
            timeToDie = true;
            kart.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 20, gameObject.transform.position.z);
            fxout.transform.position = kart.transform.position;
            fxout.Play();
            return true;
        }
        else return false;
    }
    private void setPosition()
    {
        collider.enabled = false;
        transform.Translate(2f, 2.5f, 0f);
        GetComponent<Rigidbody>().isKinematic = false;
        objeto.SetActive(true);
        explosion = false;
        fxin.Stop();
        fxout.Stop();
        AddForce = false;
        timeToDie = false;
    }

    void readyDespawn()
    {
        Runner.Despawn(Object, true);
        setPosition();
    }
}
