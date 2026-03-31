using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class AceitePowerup : SpawnedPowerup
{
    public new Collider collider;
    //public float enableDelay = 0.1f;
    public Rigidbody rigidbody;
    public bool AddForce = false;
    public Vector3 RbKart;
    //public float height = 10f;
    //public double i = 0f;
    [Networked] public TickTimer CollideTimer { get; set; }

    /*private void Awake()
    {
        collider.enabled=false;
    }*/

    public override void Spawned()
    {
        base.Spawned();
        setPosition();
        //RbKart = base.RbPlayer;
        //CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        //collider.enabled = CollideTimer.ExpiredOrNotRunning(Runner);
        /*if (i <= height) i += 1;
        else if(i>height) i -= 1;
        transform.Translate(50f * Time.fixedDeltaTime, i*Time.deltaTime, 0);//DaBros316Avanza hacia adelante*/
        if (!AddForce)
        {
            AddForce = true;
            rigidbody.AddRelativeForce(500f + RbKart.x, 200f, 0f);
        }
    }

    /*public void OnTriggerEnter(Collider other)
    {
        CLog.Log(other.name);
        if(Object.IsValid && HasInit)
        {
            CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
            collider.enabled = false;
            Runner.Despawn(Object, true);
            setPosition();
        }
    }*/

   /* public override bool Collide(KartEntity kart)
    {
        if (kart.idRandom != idRandomPowerUp)
        {
            if (Object.IsValid && !HasInit) return false;

            // kart.SpinOut();
            destroy(kart);

            return true;
        }
        else return false;
    }*/
    private void setPosition()
    {
        AddForce = false;
        rigidbody.AddForce(Vector3.zero);
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z);
        transform.Translate(3, 0.5f, 0);
        CLog.Log(transform.position);
    }

    private void destroy(KartEntity kart)
    {
        //CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
        //collider.enabled = false;
        if (kart != null) kart.ImpactoKart(ClassPart.ACEITE);
        Runner.Despawn(Object, true);
        setPosition();
    }
}
