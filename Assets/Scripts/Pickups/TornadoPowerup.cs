using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TornadoPowerup : SpawnedPowerup
{

    public new Collider collider;
    //public Collider colliderCollision;
    public float enableDelay = 0.5f;
    public ParticleSystem particulas;
    public ParticleSystem particulasUp;
    //public Animator animator;
    float contadorDespawnMax = 7f;
    public ClassPart classPowerUp;

    
    [Networked] public TickTimer CollideTimer { get; set; }

    private void Awake()
    {
        //
        // We start the collider off as disabled, because the object may be predicted, so it takes time for FUN methods
        // to be called on this object. When the object has Spawned(), then the collider will be enabled.
        //
        //colliderCollision.enabled = 
            collider.enabled = false;
        //contadorDespawn = contadorDespawnMax;
    }
    private void OnEnable()
    {

        //colliderCollision.enabled = 
            collider.enabled = false;
        enabledCollider = enableDelay;

        //CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        //animator.Play("Ghost_Idle", 0, 0);
    }
    public override void Spawned()
    {
        base.Spawned();

        //AudioManager.PlayAndFollow("bananaDropSFX", transform, AudioManager.MixerTarget.SFX);

        //
        // We create a timer to count down so that the kart who spawned this object has time to drive away before the 
        // collider enables again. Without this, the person who drops the banana will spin themselves out!
        //
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        //
        // We want to set this every frame because we dont want to accidentally enable this somewhere in code, because
        // that will mess up prediction somewhere.
        //
        //collider.enabled = CollideTimer.ExpiredOrNotRunning(Runner);

    }

    float enabledCollider = 0;
    private void Update()
    {
        

        if (contadorDespawn > 0)
        {
            
            contadorDespawn -= Time.deltaTime;
            
            if (contadorDespawn<0)
            {
                readyDespawn();
            }
        }
        if(enabledCollider>0)
        {
            enabledCollider -= Time.deltaTime;
            if(enabledCollider<0)
            {
                //colliderCollision.enabled = 
                    collider.enabled = true; ;
            }
        }
    }

    public override bool Collide(KartEntity kart)
    {
       
       if (Object.IsValid && !HasInit&&collider.enabled) return false;
        
        //particulas.Play();
        //animator.Play("Ghost_Attack", 0, 0);
        //colliderCollision.enabled = 
            collider.enabled = false;
        particulas.Stop(true);
        particulasUp.Play();
        //kart.Controller.isTornado = true;


        if (GameLauncher.instance.modeServerDedicado && GameLauncher.instance.isServer)
            destroy(kart);
        else if (RoomPlayer.Local.IsLeader)
            destroy(kart);

        return true;
    }




    float contadorDespawn=0;

    private void destroy(KartEntity kart)
    {
        /*
        ok = false;
        speedLocal = speed;
        timeEnabled = enableDelay;
        comenzarSeguimiento = timeComenzarSeguimiento;
        finalizarSeguimiento = timeFinalizarSeguimiento;
        setPosition();
        if (teledirigido)
        {
            sensor.SetActive(false);
            target = null;
            posMiraMissil = Vector3.zero;
        }
        isServer = false;
        */
        
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        //colliderCollision.enabled = 
            collider.enabled = false;
        if (kart != null) kart.ImpactoKart(classPowerUp);
        contadorDespawn = contadorDespawnMax;

        //Runner.Despawn(Object, true);

    }


    void readyDespawn()
    {
        CLog.Log("MANDE A DESTRUIE");

        contadorDespawn = 0;
        Runner.Despawn(Object, true);
        enabledCollider = enableDelay;
        //CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay); //Invalida
    }





}