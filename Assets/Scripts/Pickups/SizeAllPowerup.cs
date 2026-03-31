using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SizeAllPowerup : SpawnedPowerup {

    public float timeToDie = 3f;
    [Networked] public TickTimer sizeTimer { get; set; }
   
    public override void Spawned() {
        Tamano(true); 
        
    }
    public void Tamano(bool sumar)
    {
        List<KartEntity> karts = KartEntity.Karts;

        foreach(var kart in KartEntity.Karts)
        {
            if(sumar)
            {
                if (kart!=kartParent)
                {
                    destroy(null);
                    kart.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                }
            }
            else
                if (kart!= kartParent)
                {
                    destroy(null);
                    kart.transform.localScale = new Vector3(1f, 1f, 1f);
                }
        }
        if(sumar)sizeTimer = TickTimer.CreateFromSeconds(Runner, timeToDie);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (sizeTimer.ExpiredOrNotRunning(Runner))
        {
            Tamano(false);
            Runner.Despawn(Object, true);
        }
    }

    private void destroy(KartEntity kart)
    {
        if (kart != null) kart.ImpactoKart(ClassPart.SIZEALL);
    }
}
