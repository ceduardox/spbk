using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class RayoPowerup : SpawnedPowerup
{

    public new Collider collider;
    public float life;
    public ParticleSystem rayo;

    public GameObject sensor;

    [Networked] public TickTimer CollideTimer { get; set; }

    [Networked] public bool rayoOn{ get; set; }

    private void Awake()
    {
        //
        // We start the collider off as disabled, because the object may be predicted, so it takes time for FUN methods
        // to be called on this object. When the object has Spawned(), then the collider will be enabled.
        //
        timeEnabled = life;
        collider.enabled = false;
    }

    float timeEnabled;
    private void Update()
    {
        
        timeEnabled -= Time.deltaTime;
        if (timeEnabled < 0 && isServer&& !rayoOn)
        {
            timeEnabled = 1000;
           // lanzarRayo();
            transform.Translate(kartParent.Rigidbody.Rigidbody.velocity.magnitude*.8f+5f, 0, 0);//5 atras
            destroy(null);
            rayoOn = true;


        }
        //CLog.Log("SOY MISIL Update");
    }

    public override void Spawned()
    {
        
        if (GameLauncher.instance.modeServerDedicado)
            isServer = GameLauncher.instance.isServer;
        else
            isServer = RoomPlayer.Local.IsLeader;
        rayo.gameObject.SetActive(false);
        timeEnabled = life;
        base.Spawned();
        rayoOn = false;


        setPosition();
        // AudioManager.PlayAndFollow("bananaDropSFX", transform, AudioManager.MixerTarget.SFX);

        //
        // We create a timer to count down so that the kart who spawned this object has time to drive away before the 
        // collider enables again. Without this, the person who drops the banana will spin themselves out!
        //
        CollideTimer = TickTimer.CreateFromSeconds(Runner, .1f);

        //	transform.rotation = new Quaternion(0,-90,0,0);
    }
    

    bool isServer;//=if (RoomPlayer.Local.IsLeader)

    /* private void Update()
     {
          CLog.Log("SOY MISILDIRIGIDO" + collider.enabled);


     }*/
    public void OnEnable()
    {
        enabled = true;
    }



    public override void FixedUpdateNetwork()
    {

        if (isServer)
        {
            if (!rayoOn)
            {
                collider.enabled = !collider.enabled;// CollideTimer.ExpiredOrNotRunning(Runner);
                ok = true;
               // transform.Translate(40 * Time.fixedDeltaTime, 0, 0);//DaBros316Avanza hacia adelante
            }

           // CLog.Log("ESTO VALE111: " + transform.position + " - " + target);

            if (target != null)
            {
                CLog.Log("ESTO VALE: " + transform.position + " - " + target.transform.position + " - " + target.name);
                transform.position = target.transform.position;
                isServer = false;

            }
        }

        if (Object.isActiveAndEnabled)
        
         if(!rayo.gameObject.activeSelf&&
                rayoOn)
            {
                lanzarRayo();
            }
        
    }
    public Vector3 position;

    private Vector3 posMiraMissil = Vector3.zero;
    void SmoothLookAt(Vector3 newDirection)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(newDirection), Time.deltaTime);
    }



    public GameObject target;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject!=kartParent.gameObject&&target == null && ok &&isServer&&// && finalizarSeguimiento > 0 && ok && teledirigido &&
            other.gameObject.GetComponent<KartEntity>())
        {
          //  if (RoomPlayer.Local.Kart != other.GetComponent<KartController>())
            {
                target = other.gameObject;
                //CLog.Log("ENTRE EN COLISION CON: " + target);

                sensor.SetActive(false);
                //rayoOn = true;
                //rayo.transform.position = new Vector3(target.transform.position.x, rayo.transform.position.y, target.transform.position.z);
                //rayo.gameObject.SetActive(true);
                destroy(other.GetComponent<KartEntity>());
            }
            //enabled = false;
        }
    }


    public void lanzarRayo()
    {
        AudioManager.PlayAndFollow("rayoElectricidadSFX", transform, AudioManager.MixerTarget.SFX);
        rayo.gameObject.SetActive(true);
        rayo.Play();
    }

    bool ok = true;
    public override bool Collide(KartEntity kart)
    {
        if (Object.IsValid && !HasInit || !ok) return false;

       // CLog.Log("ENTRO ACA: " + kart.name + " " + name + " " + collider.enabled + " " + ok);


      //  destroy(kart);


        return true;
    }

    private void setPosition()
    {
        //transform.position = RoomPlayer.Local.Kart.transform.position;//new Vector3(transform.position.x, transform.position.y, transform.position.z);
        //CLog.Log(RoomPlayer.Local.Kart.GetComponent<Rigidbody>().velocity.magnitude); Velocidades maximas del player 25 - 38
       // transform.Translate(1.3f, 0, 0);//5 atras

    }
    private void destroy(KartEntity kart)
    {
    if (!rayoOn)
        {
            rayoOn = true;
            ok = false;
            setPosition();
            sensor.SetActive(true);

            //rayo.gameObject.SetActive(false);
            CollideTimer = TickTimer.CreateFromSeconds(Runner, .1f);
            if (kart != null) kart.ImpactoKart(ClassPart.THUNDER);

            //enabled = true;
            Invoke("despawn", 2);
        }
    }

    private void despawn()
    {
        if (Object)
        {
            target = null;
            rayoOn = false;
            Runner.Despawn(Object, true);
            setPosition();
        }
    }
}