using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BombSizePowerup : SpawnedPowerup
{
    public new Collider collider;
    public float collideTime = 1f;
    public bool AddForce = false;
    public GameObject objeto;
    public AudioSource sfx;
    public ParticleSystem fx;
    public bool sizing = false;
    public bool ready = false;


    public float speed;

    public Vector3 targetScale;

    public float timeToDead = 10f;

    bool isServer;
    [Networked] public TickTimer CollideTimer { get; set; }

    [Networked] public TickTimer DeadTimer { get; set; }

    public override void Spawned()
    {
        if (GameLauncher.instance.modeServerDedicado)
            isServer = GameLauncher.instance.isServer;
        else
            isServer = RoomPlayer.Local.IsLeader;  
        CollideTimer = TickTimer.CreateFromSeconds(Runner, collideTime);
        DeadTimer = TickTimer.CreateFromSeconds(Runner, timeToDead);
        setPosition();
        base.Spawned();
    }

    public void Start()
    {
        targetScale = transform.localScale * 6;
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if(CollideTimer.ExpiredOrNotRunning(Runner) && !sizing)
        {
            collider.enabled = true;
            sizing = true;
            DeadTimer = TickTimer.CreateFromSeconds(Runner, timeToDead * 2f);
        }

      if (DeadTimer.ExpiredOrNotRunning(Runner) && !ready)
        {
            ready = true;
            fx.Play();
            objeto.SetActive(false);
            collider.enabled = false;
        }
        if (!fx.isPlaying && ready) destroy(null);
        if(!AddForce && isServer)
        {
            AddForce = true;
            GetComponent<Rigidbody>().AddRelativeForce(300f+ kartParent.Rigidbody.Rigidbody.velocity.magnitude*30, 300f, 0f);
        }
        if (sizing)
        {
            gameObject.transform.localScale = Vector3.Lerp(transform.localScale, targetScale, speed * Runner.DeltaTime);
        }
    }
    public override bool Collide(KartEntity kart)
    {
        if (!ready)
        {
            collider.enabled = false;
            fx.Play();
            objeto.SetActive(false);
            ready = true;
            destroy(kart);
            return true;
        }
        else return false;
    }
    private void setPosition()
    {
        ready = false;
        sizing = false;
        GetComponent<Rigidbody>().isKinematic = false;
        objeto.SetActive(true);
        collider.enabled = false;
        transform.Translate(1, 0.5f, 0);
        gameObject.transform.localScale=new Vector3(1f, 1f, 1f);
        AddForce = false;
        CLog.Log(transform.position);
    }

    private void destroy(KartEntity kart)
    {
        if (kart != null) kart.ImpactoKart(ClassPart.BOMBSIZE);
        Runner.Despawn(Object, true);
    }
}
