using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BallCambioPowerup : SpawnedPowerup
{
    public new Collider collider;
    public float collidetime;
    public bool AddForce = false;
    public ParticleSystem fxin;
    public ParticleSystem fxout;
    public GameObject objeto;
    public bool explosion = false;
    public Transform enemyPosition;
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
        setPosition();
        base.Spawned();
        CollideTimer = TickTimer.CreateFromSeconds(Runner, collidetime);
        DeadTimer = TickTimer.CreateFromSeconds(Runner, deadTime);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        collider.enabled = CollideTimer.ExpiredOrNotRunning(Runner);
        if(!AddForce && isServer)
        {
            AddForce = true;
            GetComponent<Rigidbody>().AddRelativeForce(500f+ kartParent.Rigidbody.Rigidbody.velocity.magnitude*15, 0f, 0f);
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
        CLog.Log("soy kart " + kart);
        if (kart != kartParent)
        {
            if (Object.IsValid && !HasInit) return false;
            objeto.SetActive(false);
            collider.enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
            sfx.Play();
            timeToDie = true;
            enemyPosition.position = new Vector3(kart.transform.position.x, kart.transform.position.y+1f, kart.transform.position.z);
            fxout.Play();
            kart.transform.position = new Vector3(kartParent.transform.position.x, kartParent.transform.position.y+1f, kartParent.transform.position.z);
            fxin.transform.position = kartParent.transform.position;
            fxin.Play();
            kartParent.transform.position = enemyPosition.position;
            return true;
        }
        else return false;
    }
    private void setPosition()
    {
        transform.Translate(1f, 2.5f, 0f);
        GetComponent<Rigidbody>().isKinematic = false;
        objeto.SetActive(true);
        collider.enabled = false;
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
