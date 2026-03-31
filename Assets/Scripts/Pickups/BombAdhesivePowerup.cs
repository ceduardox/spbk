using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BombAdhesivePowerup : SpawnedPowerup
{

    public int timerBomb = 3;
    float timerBombLocal;
    
    public new Collider collider;
    public float enableDelay = 0.5f;
    public ClassPart classPowerUp;
    public AutoDestroy fx;
    Transform kartTarget2;
    public AudioSource readySfx;
    
    [Networked] public TickTimer CollideTimer { get; set; }
    //[Networked] public TickTimer timerBomb { get; set; }

    [Networked] public bool bomb_ON { get; set; }


    private void Awake()
    {
        //
        // We start the collider off as disabled, because the object may be predicted, so it takes time for FUN methods
        // to be called on this object. When the object has Spawned(), then the collider will be enabled.
        //
        collider.enabled = false;
    }

    public override void Spawned()
    {
        base.Spawned();
        kartTarget2 = null;
        GetComponent<Rigidbody>().isKinematic = false;
        timerBombLocal = timerBomb;
        bomb_ON = false;
        yaExploto = false;
        
        //AudioManager.PlayAndFollow("bananaDropSFX", transform, AudioManager.MixerTarget.SFX);

        //
        // We create a timer to count down so that the kart who spawned this object has time to drive away before the 
        // collider enables again. Without this, the person who drops the banana will spin themselves out!
        //
        //
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
    }

    bool yaExploto = false;
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (Object.IsValid && !HasInit) return;
        if (!yaExploto)
        {
            if (kartTarget2)
            {
                gameObject.transform.position = new Vector3(kartTarget2.position.x, kartTarget2.position.y + .75f, kartTarget2.position.z);

                if (bomb_ON)
                {
                    yaExploto = true;
                    CLog.Log("Estoy dentro " + bomb_ON);
                    //getFX();
                    readySfx.Stop();
                    destroy(kartTarget2.GetComponent<KartEntity>());
                    return;
                    //kartTarget = null;
                }

                if (Object
                    && Object.HasStateAuthority)
                {
                    if ((timerBombLocal -= Time.fixedDeltaTime) < 0 && !bomb_ON)
                        bomb_ON = true;
                }
            }


            //
            // We want to set this every frame because we dont want to accidentally enable this somewhere in code, because
            // that will mess up prediction somewhere.
            //
         collider.enabled = CollideTimer.ExpiredOrNotRunning(Runner) && !kartTarget2;
        }
    }

    public override bool Collide(KartEntity kart)
    {
        if (yaExploto) return false;
        if (Object.IsValid && !HasInit||kartTarget2!=null) return false;

        collider.enabled = false;
        //timerBomb = TickTimer.CreateFromSeconds(Runner, enableTimerBomb);
        GetComponent<Rigidbody>().isKinematic = true;
        kartTarget2 = kart.transform;
        readySfx.Play();
        //destroy(kart);

        return true;
    }



   /* public void getFX()
    {
        if (fx)
        {
            Instantiate(fx, kartTarget.transform.position, kartTarget.transform.rotation);
            fx.setTraget(kartTarget);
        }

    }*/

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
        //CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        collider.enabled = false;
        if (kart != null) kart.ImpactoKart(classPowerUp);
        Runner.Despawn(Object, true);


        //kart.ImpactoKart(ClassPart.SCALEREDUCE);

        //setPosition();
    }





}