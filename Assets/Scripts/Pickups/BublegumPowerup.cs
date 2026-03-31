using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BublegumPowerup : SpawnedPowerup
{
    public new Collider collider;
    public float enableDelay = 2f;
    public ParticleSystem fx;
    public bool explosion;
    public GameObject objeto;
    [Networked] public TickTimer CollideTimer { get; set; }

    private void Awake()
    {
        collider.enabled = false;
    }

    public override void Spawned()
    {
        base.Spawned();
        setPosition();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (explosion && !fx.isPlaying)
        {
            destroy(null);
        }
    }

    private void setPosition()
    {
        transform.Translate(-2, 0, 0);
        explosion = false;
        fx.Stop();
        collider.enabled = true;
        objeto.SetActive(true);

    }
    public override bool Collide(KartEntity kart)
    {
        if(!explosion)
        {
            collider.enabled = false;
            kart.ImpactoKart(ClassPart.BANANA);
            explosion = true;
            fx.Play();
            collider.enabled = false;
            objeto.SetActive(false);
            return true;
        }
        return false;
    }
    private void destroy(KartEntity kart)
    {
        if (kart != null) kart.ImpactoKart(ClassPart.BUBBLEGUM);
        Runner.Despawn(Object, true);
        setPosition();
    }
}
