using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BombPowerup : SpawnedPowerup
{

    public ClassPart _class = ClassPart.NONE;
    public Collider _collider;
    public float enableDelay = 0.5f;
    public float deadtime;

    [Networked] public TickTimer timerBomb { get; set; }
    [Networked] public TickTimer CollideTimer { get; set; }

    public float force;
    public float magnitude;


    private void Awake()
    {
        //
        // We start the collider off as disabled, because the object may be predicted, so it takes time for FUN methods
        // to be called on this object. When the object has Spawned(), then the collider will be enabled.
        //
        _collider.enabled = false;
    }

    public override void Spawned()
    {
        base.Spawned();
        //AudioManager.PlayAndFollow("bananaDropSFX", transform, AudioManager.MixerTarget.SFX);
        _collider.enabled = false;

        //
        // We create a timer to count down so that the kart who spawned this object has time to drive away before the 
        // collider enables again. Without this, the person who drops the banana will spin themselves out!
        //
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        kartTarget = null;
        timerBomb = TickTimer.CreateFromSeconds(Runner, deadtime);
        transform.Translate(.4f, 1f, 0f);

        disparar = true;
    }
    bool disparar;
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        //
        // We want to set this every frame because we dont want to accidentally enable this somewhere in code, because
        // that will mess up prediction somewhere.
        //
        _collider.enabled = CollideTimer.ExpiredOrNotRunning(Runner)&&kartTarget==null;

        if(timerBomb.ExpiredOrNotRunning(Runner))
        {
            despawn(null);
        }
        if(disparar&&Object.HasStateAuthority)
        {
            disparar = false;
            CLog.Log("DISPARANDO");
            GetComponent<Rigidbody>().AddRelativeForce(force + kartParent.Rigidbody.Rigidbody.velocity.magnitude * magnitude, 100, 0f);
        }

    }

    public override bool Collide(KartEntity kart)
    {
        if (Object.IsValid && !HasInit) return false;
        kartTarget = kart;
        despawn(kart);
        
        return true;
    }


    void despawn(KartEntity kart)
    {
        _collider.enabled = false;
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay * 10);
        timerBomb = TickTimer.CreateFromSeconds(Runner, enableDelay * 10);

        if (kart != null)
        {
            kart.ImpactoKart(_class);
            if (Object.HasStateAuthority) GameLauncher.expancionFX(expancion, kart.transform, null);

        }
        else
        {
            FX_List.playEffect(FX_Type.explosion, transform);
            if (Object.HasStateAuthority) GameLauncher.expancionFX(expancion, transform, null);
        }
        Runner.Despawn(Object, true);
    }





























    /*



    public ClassPart _class = ClassPart.NONE;
    public new Collider collider;
    public float collideTime;
    public float force;
    public float magnitude;
    public float height;
    public float axisY;
    public bool AddForce = false;
    public ParticleSystem fx;
    public GameObject objeto;
    public bool explosion = false;
    public AudioSource sfx;

    

    public float deadtime;

    bool isServer;
    [Networked] public TickTimer timerBomb { get; set; }
    [Networked] public TickTimer CollideTimer { get; set; }
    
       public override void Spawned()
    {
        if (GameLauncher.instance.modeServerDedicado)
            isServer = GameLauncher.instance.isServer;
        else
            isServer = RoomPlayer.Local.IsLeader;
        timerBomb = TickTimer.CreateFromSeconds(Runner, deadtime);
        CollideTimer = TickTimer.CreateFromSeconds(Runner, collideTime);
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
            GetComponent<Rigidbody>().AddRelativeForce(force+ kartParent.Rigidbody.Rigidbody.velocity.magnitude*magnitude, height, 0f);
        }
        if (timerBomb.ExpiredOrNotRunning(Runner) && !explosion)
        {
            objeto.SetActive(false);
            fx.Play();
            sfx.Play();
            explosion = true;
            collider.enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }
        if (explosion && !fx.isPlaying)
        {
            destroy(null);
        }
    }
    public override bool Collide(KartEntity kart)
    {
        if(!explosion)
        {
            collider.enabled = false;
            explosion = true;
            GetComponent<Rigidbody>().isKinematic = true;
            objeto.SetActive(false);
            fx.Play();
            sfx.Play();
            destroy(kart);
            return true;
        }
        else return false;
    }
    private void setPosition()
    {
        collider.enabled = false;
        gameObject.transform.Translate(axisY, 1f, 0f);
        GetComponent<Rigidbody>().isKinematic = false;
        objeto.SetActive(true);
        explosion = false;
        fx.Stop();
        AddForce = false;
    }

    private void destroy(KartEntity kart)
    {
        if (kart != null) kart.ImpactoKart(_class);
        if (Object.HasStateAuthority) GameLauncher.expancionFX(expancion, transform , null);
        Runner.Despawn(Object, true);
        setPosition();
    }*/
}
