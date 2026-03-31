using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CanonGravedadPowerud : SpawnedPowerup
{
    public GameObject collider;
    public Collider esfera;
    public Collider area;
    public GameObject ball;
    public Rigidbody bala;
    public float enableDelay;
    public AudioSource explosion;
    public bool disparo;
    public bool gravedad = false;
    public bool seguir = true;
    public KartEntity kartenemy;
    public ParticleSystem fx;

    float contadorDespawnMax = 2f;

    [Networked] public TickTimer CollideTimer { get; set; }


    public override void Spawned()
    {
        if (GameLauncher.instance.modeServerDedicado)
            isServer = GameLauncher.instance.isServer;
        else
            isServer = RoomPlayer.Local.IsLeader;
        enabled = true;
        base.Spawned();
        setPosition();
        
        CollideTimer = TickTimer.CreateFromSeconds(Runner, enableDelay);
    }

    bool isServer;

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if(CollideTimer.ExpiredOrNotRunning(Runner))
        {
            collider.SetActive(true);
        }
        transform.position = new Vector3(kartParent.transform.position.x, kartParent.transform.position.y + 1.25f, kartParent.transform.position.z);
        transform.rotation = kartParent.transform.rotation;
        if (!disparo && !bala.isKinematic)
        {
            bala.AddRelativeForce(0f, kartParent.Rigidbody.Rigidbody.velocity.magnitude * 10, 0f);
            esfera.transform.position = bala.transform.position;
        }

        if (contadorDespawn > 0)
        {

            contadorDespawn -= Time.deltaTime;

            if (contadorDespawn < 0)
            {
                readyDespawn();

            }
        }
        if (gravedad)
        {
            kartenemy.transform.Rotate(Vector3.up,Time.deltaTime*180);
        }
    }
    public override bool Collide(KartEntity kart)
    {
        if (kart != kartParent)
        {
            if (Object.IsValid && !HasInit) return false;
            if (disparo)
            {
                collider.SetActive(false);
                bala.isKinematic = true;
                explosion.Play();
                gravedad = true;
                kartenemy = kart;
                esfera.enabled = false;
                ball.SetActive(false);
                seguir = false;
                fx.Play();
                destroy(kart);
            }
            else
            {
                bala.isKinematic = false;
                area.enabled = false;
            }
            return true;
        }
        else return false;
    }

    private void setPosition()
    {
        seguir = true;
        gravedad = false;
        bala.isKinematic = true;
        transform.Translate(0f, 0f, 0f);
        collider.SetActive(false);
        ball.SetActive(true);
        esfera.enabled = true;
        area.enabled = true;

    }

    float contadorDespawn = 0;
    private void destroy(KartEntity kart)
    {
        area.enabled = false;
        if (kart != null)kart.ImpactoKart(ClassPart.CANONGRAVEDAD);
        contadorDespawn = contadorDespawnMax;
    }

    void readyDespawn()
    {
        contadorDespawn = 0;
        Runner.Despawn(Object, true);
        setPosition();
    }
}