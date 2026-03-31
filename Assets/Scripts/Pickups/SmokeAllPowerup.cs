using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SmokeAllPowerup : SpawnedPowerup {

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
            if (kart != kartParent)
            {
                CLog.Log(kart.id);
                destroy(kart);
            }
        }
        CLog.Log(kartParent.id);
        if(sumar)sizeTimer = TickTimer.CreateFromSeconds(Runner, timeToDie);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (sizeTimer.ExpiredOrNotRunning(Runner))
        {
            Runner.Despawn(Object, true);
        }
    }

    private void destroy(KartEntity kart)
    {
        if (kart != null) kart.ImpactoKart(ClassPart.SMOKEALL);
    }
}
