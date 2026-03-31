using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperBoostPowerup : SpawnedPowerup {
    public override void Init(KartEntity spawner) {
        base.Init(spawner);

        spawner.Controller.GiveBoost(false, 4);
        //kartParent.Rigidbody.Rigidbody.AddRelativeForce(0, 200f, 1000f + kartParent.Rigidbody.Rigidbody.velocity.magnitude * 25);

        // Runner.Despawn(Object, true);
        // Destroy(gameObject);
    }

    public override void Spawned() {
        base.Spawned();

        Runner.Despawn(Object, true);
    }

    public override void PredictedSpawnSpawned() { }
    public override void PredictedSpawnUpdate() { }
    public override void PredictedSpawnRender() { }
    public override void PredictedSpawnFailed() { }
    public override void PredictedSpawnSuccess() { }
}
