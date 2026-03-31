using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

/*
public enum PowerUpType
{
    banana,
    turbo,
    misil,
    misilDirigido,
    mina,
    fantasma,
    tornado,
    bomba,
    rayo,
    aceite,
    tronco,
    flecha,
    dinamita,
    none,

}*/

/// <summary>
/// This is the object that physically sits in the world after being 'used' as an item.
/// </summary>
public abstract class SpawnedPowerup : NetworkBehaviour, ICollidable, IPredictedSpawnBehaviour
{
    [Networked] public NetworkBool HasInit { get; private set; }
    
    //public PowerUpType powerUpType;
    internal KartEntity kartParent;
    internal KartEntity kartTarget;
    [Range(0f, 5f)]
    public float expancion=0;
    //internal int idRandomPowerUp;
    //internal Vector3 RbPlayer;
    //internal Rigidbody rb;

    public virtual void Init(KartEntity spawner) 
    {
        kartParent = spawner;
    }

    public override void Spawned()
    {
        base.Spawned();

        HasInit = true;
    }

    public virtual bool Collide(KartEntity kart)
    {
        return false;
    }


    public virtual void PredictedSpawnSpawned() { }
    public virtual void PredictedSpawnUpdate() { }
    public virtual void PredictedSpawnRender() { }
    public virtual void PredictedSpawnFailed() { }
    public virtual void PredictedSpawnSuccess() { }
}