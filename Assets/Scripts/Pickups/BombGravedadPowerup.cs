using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BombGravedadPowerup : SpawnedPowerup
{
    public Collider collider;
    public Collider ball;
    public bool AddForce = false;
    public GameObject objeto;
    public Rigidbody bala;
    public bool gravedad=false;
    public KartEntity kartenemy;
    public ParticleSystem fx;

    float timelife = 10f;

    public float timeToEnable=0.5f;
    [Networked] public TickTimer CollideTimer { get; set; }
    [Networked] public TickTimer DeadTimer { get; set; }
    public override void Spawned()
    {
        CollideTimer = TickTimer.CreateFromSeconds(Runner, timeToEnable);
        DeadTimer = TickTimer.CreateFromSeconds(Runner, timelife);
        setPosition();
        base.Spawned();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        ball.enabled = CollideTimer.ExpiredOrNotRunning(Runner);
        collider.enabled = CollideTimer.ExpiredOrNotRunning(Runner);
        if (DeadTimer.ExpiredOrNotRunning(Runner)) Runner.Despawn(Object, true);
        if (!AddForce)
        {
            AddForce = true;
            GetComponent<Rigidbody>().AddRelativeForce(300f+ kartParent.Rigidbody.Rigidbody.velocity.magnitude*20, 300f, 0f);
        }
        if (gravedad)
        {
            kartenemy.transform.RotateAround(gameObject.transform.position,Vector3.up, Time.deltaTime * 180);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        ball.enabled = false;
        bala.isKinematic = true;
    }
    public override bool Collide(KartEntity kart)
    {
        if (!gravedad)
        {
            gravedad = true;
            kartenemy = kart;
            collider.enabled = false;
            objeto.SetActive(false);
            fx.Play();
            DeadTimer = TickTimer.CreateFromSeconds(Runner, 5f);
            destroy(kart);
            return true;
        }
        else return false;
    }
    private void setPosition()
    {
        fx.Stop();
        bala.isKinematic = false;
        objeto.SetActive(true);
        collider.enabled = false;
        ball.enabled = false;
        transform.Translate(3f, 1.25f, 0f);
        AddForce = false;
        gravedad = false;
    }
    private void destroy(KartEntity kart)
    {
        if (kart != null) kart.ImpactoKart(ClassPart.BOMBGRAVEDAD);
    }
}
