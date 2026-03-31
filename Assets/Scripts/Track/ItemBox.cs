using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Random = UnityEngine.Random;

public class ItemBox : NetworkBehaviour, ICollidable
{
    private static readonly ClassPart[] SafeBoxPowerupPool =
    {
        ClassPart.MINA,
        ClassPart.MISSILE,
        ClassPart.BANANA,
        ClassPart.BOMB,
        ClassPart.TURBO,
        ClassPart.OIL,
        ClassPart.FAKEITEM,
        ClassPart.SKULL,
        ClassPart.BUBBLEGUM,
        ClassPart.SOAP,
        ClassPart.WALL
    };

    public GameObject model;
    public ParticleSystem breakParticle;
    public float cooldown = 5f;
    public Transform visuals;
    public bool isAerea;

    [Networked(OnChanged = nameof(OnKartChanged))] public KartEntity Kart { get; set; }
    [Networked] public TickTimer DisabledTimer { get; set; }

    public bool Collide(KartEntity kart)
    {
        //CLog.LogError("kart: " + kart + " " + DisabledTimer.ExpiredOrNotRunning(Runner));
        if (kart != null && DisabledTimer.ExpiredOrNotRunning(Runner))
        {
            enabled = true;

            Kart = kart;
            DisabledTimer = TickTimer.CreateFromSeconds(Runner, cooldown);
            var powerUp = GetRandomPowerup();//sincroniza la semilla y genera el mismo index random

            if (Kart.HeldItemIndex== -1)
            {
                Kart.SetHeldItem(powerUp);
            }/*
            else if (Kart.HeldItemIndex2 == -1)//ADDED ITEM 2
            {
                Kart.SetHeldItem2(powerUp);
            }
            else if (Kart.HeldItemIndex3 == -1)//ADDED ITEM 3
            {
                Kart.SetHeldItem3(powerUp);
            }*/
            //Kart.SetHeldItem(powerUp);
            //Kart.SetHeldItem2(powerUp);

            //CLog.LogError("RANDOM ES: " + kart + " " +powerUp);
        }

        return true;
    }

    private static void OnKartChanged(Changed<ItemBox> changed) { changed.Behaviour.OnKartChanged(); }
    private void OnKartChanged()
    {
        bool hasKart = Kart != null;

        if (visuals != null)
            visuals.gameObject.SetActive(!hasKart);

        AudioManager.PlayAndFollow(
            hasKart && Kart.HeldItem != null ? "itemCollectSFX" : "itemWasteSFX",
            transform,
            AudioManager.MixerTarget.SFX
        );
        if (breakParticle != null)
            breakParticle.Play();

        if (!hasKart)
            return;
        if (!enabled) return;
        /*AudioManager.PlayAndFollow(
            Kart.HeldItem2 != null ? "itemCollectSFX" : "itemWasteSFX",
            transform,
            AudioManager.MixerTarget.SFX
        );
        AudioManager.PlayAndFollow(
            Kart.HeldItem3 != null ? "itemCollectSFX" : "itemWasteSFX",
            transform,
            AudioManager.MixerTarget.SFX
        );*/

    }
    bool bajar = true;
    public override void FixedUpdateNetwork()
    {


        base.FixedUpdateNetwork();


        if(bajar&& !isAerea)
        {
            Vector3 fwd = transform.TransformDirection(Vector3.down);

            if (Physics.Raycast(transform.position, fwd, 0.5f))
                bajar = false;
            //print("Bajando caja: "+);
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.05f, transform.position.z);
        }
        else if (visuals.gameObject.activeSelf) 
            enabled = false;

        //CLog.Log("TESTEANDO ALTURA: "+name);

        /*
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, 8))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            CLog.Log("Did Hit "+ hit.distance);
        }*/





        if (DisabledTimer.ExpiredOrNotRunning(Runner) && Kart != null)
        {
            Kart = null;
            enabled = false;
        }
    }

    private int GetRandomPowerup()
    {
        var seed = Runner.Simulation.Tick;

        Random.InitState(seed);

        var candidates = new List<int>(SafeBoxPowerupPool.Length);
        for (int i = 0; i < SafeBoxPowerupPool.Length; i++)
        {
            int index = ResourceManager.Instance.getPowerupIndex(SafeBoxPowerupPool[i]);
            if (index >= 0 && !candidates.Contains(index))
                candidates.Add(index);
        }

        if (candidates.Count == 0)
            return ResourceManager.Instance.getPowerupIndex(ClassPart.MISSILE);

        return candidates[Random.Range(0, candidates.Count)];
    }
}
