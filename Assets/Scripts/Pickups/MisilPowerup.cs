using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MisilPowerup : SpawnedPowerup
{
    public ClassPart _class = ClassPart.NONE;
    public new Collider collider;
    public float enableDelay = 0.2f;
    public Rigidbody missil;
    public ParticleSystem explosion;
    public GameObject misilModel;
    public AudioSource audioPropulsor;
    public float speed;
    float speedLocal;

    //Teledirigido
    public bool teledirigido;
    public GameObject sensor;

    public float tiempodeActivacion = .1f, timeFinalizarSeguimiento = 50;
    float comenzarSeguimiento, finalizarSeguimiento;

    [Networked] public bool explotarNow { get; set; }


    Vector3 originalPositionSensor;
    [Networked] public TickTimer CollideTimer { get; set; }

    private void Awake()
    {
        if (teledirigido)
        {
            sensor.SetActive(false);
            originalPositionSensor = sensor.transform.position;
        }
        //
        // We start the collider off as disabled, because the object may be predicted, so it takes time for FUN methods
        // to be called on this object. When the object has Spawned(), then the collider will be enabled.
        //

        // collider.enabled = false;
    }

    float timeEnabled = .2f;
    private void Update()
    {
        timeEnabled -= Time.deltaTime;
        if (timeEnabled < 0)
        {
            ok = true;
        }
        comenzarSeguimiento -= Time.deltaTime;
        finalizarSeguimiento -= Time.deltaTime;

        if (!isServer&&isSpawn)
        {
            if (explotarNow)
            {
                explotar();
            }
        }
        //CLog.Log("SOY MISIL Update");
    }


    public override void Spawned()
    {
        if (GameLauncher.instance.modeServerDedicado)
            isServer = GameLauncher.instance.isServer;
        else
            isServer = RoomPlayer.Local.IsLeader;
        timeEnabled = enableDelay;
        comenzarSeguimiento = tiempodeActivacion;
        finalizarSeguimiento = timeFinalizarSeguimiento;
        speedLocal = speed;
        base.Spawned();
        setPosition();
        despawnOk = true;
        explotarNow = false;
        enabled = true;
        isSpawn = true;
        // CLog.Log("SOY " + name);

        // AudioManager.PlayAndFollow("bananaDropSFX", transform, AudioManager.MixerTarget.SFX);

        //
        // We create a timer to count down so that the kart who spawned this object has time to drive away before the 
        // collider enables again. Without this, the person who drops the banana will spin themselves out!
        //
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);

        //	transform.rotation = new Quaternion(0,-90,0,0);
    }



    bool isServer;//=if (RoomPlayer.Local.IsLeader)


    /* private void Update()
     {
          CLog.Log("SOY MISILDIRIGIDO" + collider.enabled);


     }*/
    private void OnEnable()
    {
        enabled = true;
        missil.isKinematic = false;
        if(sensor)sensor.transform.position= originalPositionSensor;
    }
    bool intercalar;
    public override void FixedUpdateNetwork()
    {
        //    base.FixedUpdateNetwork();

        //
        //
        // We want to set this every frame because we dont want to accidentally enable this somewhere in code, because
        // that will mess up prediction somewhere.
        //


        if (isServer&& despawnOk)
        {


           // ok = true;

            if (finalizarSeguimiento < 0)
            {
                if(despawnOk) destroy(null);
            }

            if (teledirigido && finalizarSeguimiento > 0)
            {
                //sensor.SetActive(true);
                if (teledirigido&& comenzarSeguimiento < 0)
                {
                    sensor.SetActive(true);// ; collider.enabled = !collider.enabled;// CollideTimer.ExpiredOrNotRunning(Runner);
                    comenzarSeguimiento = 100;
                }

                


                //  CLog.Log("SOY MISILDIRIGIDO" + collider.enabled);
                if (target != null)
                {
                    if (posMiraMissil == Vector3.zero)
                        posMiraMissil = sensor.transform.position;

                    transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));//"Kart (test)"


                    // transform.LookAt(posMiraMissil = Vector3.Lerp(posMiraMissil, new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z), Time.deltaTime * 1));
                    transform.transform.Rotate(new Vector3(0, -90, 0), Space.Self);
                }

            }

            transform.Translate((kartParent.Rigidbody.Rigidbody.velocity.magnitude+speedLocal )* Time.fixedDeltaTime, 0, 0);//DaBros316Avanza hacia adelante
            if (teledirigido) sensor.transform.Translate(15* Time.fixedDeltaTime, 0, 0);

            //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z+);// Translate(speedLocal * Time.fixedDeltaTime, 0, 0);//DaBros316Avanza hacia adelante
        }
        //else enabled = false;
        


        //transform.localPosition = new Vector3(transform.localPosition.x - Time.deltaTime, transform.localPosition.y , transform.localPosition.z );
        //if()
    }
    private Vector3 posMiraMissil = Vector3.zero;
    void SmoothLookAt(Vector3 newDirection)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(newDirection), Time.deltaTime);
    }

    GameObject target;
    public void OnTriggerEnter(Collider other)
    {
        if (target == null && finalizarSeguimiento > 0 && ok && teledirigido && isServer&&
            other.gameObject.GetComponent<KartEntity>())
        {
            target = other.gameObject;
            sensor.SetActive(false);
        }
         if(other.gameObject.layer == 7|| other.gameObject.layer == 8)
        {
            if (isServer && isSpawn&&
                explosion && !explotarNow)
            {
                explotarNow = true;
                explotar();

            }
            //destroy(null);
        }
    }
    bool isSpawn;
    public void explotar()
    {
        if(isSpawn)
        {
            isSpawn = false;
            explosion.Play();
            if (audioPropulsor) audioPropulsor.Stop();
            if (_class == ClassPart.MISSILE)
            {
                AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
            }
            else if (_class == ClassPart.SKULL)
                audioPropulsor.Play();
                

            if (misilModel) misilModel.SetActive(false);
            if(isServer)destroy(null);
        }
    }

    bool ok = false;
    public override bool Collide(KartEntity kart)
    {   
        if (!Object) return false;
        if (kart != kartParent)
        {
            if (Object.IsValid && !HasInit || !ok) return false;

//            CLog.Log("ENTRO ACA: " + kart.name + " " + name + " " + collider.enabled + " " + ok);


            destroy(kart);
            return true;
        }
        else return false;

       
    }

    private void setPosition()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z);
        if (misilModel) misilModel.SetActive(true);
        transform.Translate(1, 0, 0);      
    }
    private void destroy(KartEntity kart)
    {
        despawnOk = false;

        if (kart != null)
        {
            if (!explotarNow)
            {
                kart.ImpactoKart(ClassPart.MISSILE);
                if (Object.HasStateAuthority) GameLauncher.expancionFX(expancion, kart.transform, kartParent.transform);
                explotar();
                //despawn();//Runner.Despawn(Object, true);
                Invoke("despawn", 3);
            }
            explotarNow = true;
        }
        else
        {
            if (Object.HasStateAuthority) GameLauncher.expancionFX(expancion, transform, null);

            Invoke("despawn", 3);
            
        }     
    }
    bool despawnOk;
    public void ResetPU()
    {

        ok = false;
        speedLocal = speed;
        timeEnabled = tiempodeActivacion;
        comenzarSeguimiento = tiempodeActivacion;
        finalizarSeguimiento = timeFinalizarSeguimiento;
        //setPosition();
        if (teledirigido)
        {
            sensor.SetActive(false);
            target = null;
            posMiraMissil = Vector3.zero;
        }
        isServer = false;
        collider.enabled = true;
        ok = false;
        missil.isKinematic = true;
        ok = false;
        enabled = true;
    }

    public void despawn()
    {
        CLog.Log("ESTOY RESPAWNEANDO ");
        ResetPU();
       // if(despawnOk&&isServer)
        if(Object)Runner.Despawn(Object, true);


    }




}