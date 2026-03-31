using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BallTeleportPowerup : SpawnedPowerup
{
    public new Collider collider;
    public bool AddForce = false;
    public GameObject objeto;
    public AudioSource sfx;
    public ParticleSystem fxin;
    public ParticleSystem fxout;
    public bool dead=false;
    bool isServer;
    public float time=0.4f;
    [Networked] public TickTimer CollideTimer { get; set; }
    
    public override void Spawned()
    {

        if (GameLauncher.instance.modeServerDedicado)
            isServer = GameLauncher.instance.isServer;
        else
            isServer = RoomPlayer.Local.IsLeader;
        CollideTimer = TickTimer.CreateFromSeconds(Runner,time);
        setPosition();
        base.Spawned();
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(!dead)
        {
            kartParent.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 2f, gameObject.transform.position.z);
            dead = true;
            collider.enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
            objeto.SetActive(false);
            fxout.transform.position = kartParent.transform.position;
            fxout.Play();
            fxin.transform.position = gameObject.transform.position;
            fxin.Play();
        }
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        collider.enabled = CollideTimer.ExpiredOrNotRunning(Runner);
        if (!AddForce && isServer)
        {
            AddForce = true;
            GetComponent<Rigidbody>().AddRelativeForce(500f+ kartParent.Rigidbody.Rigidbody.velocity.magnitude*20, 200f, 0f);
        }
        if(dead && !fxin.isPlaying && !fxout.isPlaying) Runner.Despawn(Object, true);
    }
    private void setPosition()
    {
        collider.enabled = false;
        dead = false;
        transform.Translate(2f, 3f, 0f);
        GetComponent<Rigidbody>().isKinematic = false;
        objeto.SetActive(true);
        AddForce = false;
    }
}
