using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FantasmaPowerup : SpawnedPowerup
{

    public new Collider collider;
    public Collider colliderCollision;
    public float enableDelay = 0.5f;
    public ParticleSystem particulas;
    public Transform fantasma;
    public Animator animator;
    float contadorDespawnMax = 5f;
    public ClassPart classPowerUp;
    public AudioSource sfx;



    [Networked] public TickTimer CollideTimer { get; set; }
    Vector3 positionInicial;
    private void Awake()
    {
        colliderCollision.enabled = collider.enabled = false;
    }
    private void OnEnable()
    {
        positionInicial= fantasma.localPosition;
        colliderCollision.enabled = collider.enabled = false;
        animator.Play("Ghost_Idle", 0, 0);
    }
    public override void Spawned()
    {
        base.Spawned();
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        colliderCollision.enabled = collider.enabled = false;
        enabledCollider = true;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if(UpGhost)
        {
            if(classPowerUp==ClassPart.GHOST) fantasma.position = new Vector3(fantasma.position.x, fantasma.position.y+Time.deltaTime*(contadorDespawnMax-contadorDespawn)* (contadorDespawnMax - contadorDespawn), fantasma.position.z);
        }
        if (enabledCollider&& CollideTimer.ExpiredOrNotRunning(Runner))
        {
            colliderCollision.enabled = collider.enabled = true;
        }
    }

    bool UpGhost = false;
    bool enabledCollider = false;
    private void Update()
    {


        if (contadorDespawn > 0)
        {

            contadorDespawn -= Time.deltaTime;

            if (contadorDespawn < 0)
            {
                readyDespawn();
            }
            if (contadorDespawn < contadorDespawnMax*3/4)
            {
                UpGhost = true;
            }
        }
        else UpGhost = false;
      /*  if(enabledCollider>0)
        {
            enabledCollider -= Time.deltaTime;
            if(enabledCollider<0)
            {
                colliderCollision.enabled = collider.enabled = true; ;
            }
        }*/
    }

    public override bool Collide(KartEntity kart)
    {
       
       if (Object.IsValid && !HasInit) return false;

        particulas.Play();
        animator.Play("Ghost_Attack", 0, 0);
        enabledCollider = colliderCollision.enabled = collider.enabled = false;

        if (GameLauncher.instance.modeServerDedicado&&GameLauncher.instance.isServer)
                destroy(kart);
        else if(RoomPlayer.Local.IsLeader)
                destroy(kart);


        return true;
    }




    float contadorDespawn=0;

    private void destroy(KartEntity kart)
    {
        //CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        colliderCollision.enabled = collider.enabled = false;
        if (kart != null) kart.ImpactoKart(classPowerUp);
        contadorDespawn = contadorDespawnMax;
    }


    void readyDespawn()
    {
        CLog.Log("MANDE A DESTRUIE");
        UpGhost = false;
        fantasma.localPosition = positionInicial;
        contadorDespawn = 0;
        Runner.Despawn(Object, true);
        //enabledCollider = enableDelay;
    }
}