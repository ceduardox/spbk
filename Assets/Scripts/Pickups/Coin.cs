using Fusion;
using UnityEngine;

public class Coin : NetworkBehaviour, ICollidable {

    [Networked(OnChanged = nameof(OnIsEnabledChangedCallback))]
    public NetworkBool IsActive { get; set; } = true;

    public Transform visuals;
    public bool isAerea;
    bool bajar = true;
    bool isSpawn = true;
    private void Update()
    {
        if (bajar&&!isAerea)
        {
            Vector3 fwd = transform.TransformDirection(Vector3.down);

            if (Physics.Raycast(transform.position, fwd, 0.6f))
                bajar = false;
            //print("There is something in front of the object!");
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.05f, transform.position.z);
        }
        else if (visuals.gameObject.activeSelf)
            enabled = false;
    }
    public bool Collide(KartEntity kart) {
        if (!isSpawn) return false;
        isSpawn = false;

        if ( IsActive ) {
            kart.CoinCount++;

            IsActive = false;
            
            if ( kart.Object.HasStateAuthority ) {
                Runner.Despawn(Object);
            }
        }

        return true;
    }

    private static void OnIsEnabledChangedCallback(Changed<Coin> changed) {
        var behaviour = changed.Behaviour;
        behaviour.visuals.gameObject.SetActive(behaviour.IsActive);

        if ( !behaviour.IsActive )
            AudioManager.PlayAndFollow("coinSFX", behaviour.transform, AudioManager.MixerTarget.SFX);
    }
}
