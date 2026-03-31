using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using FusionExamples.Utility;
public class LoadTrack : NetworkBehaviour
{
    

    // Start is called before the first frame update

    public static Managers.LevelManager Instance => Singleton<Managers.LevelManager>.Instance;

    void Awake()
    {
        return;
       // CLog.Log("ESTOY CARGANDO LA PISTA " + GameLauncher.instance.isServer + " " + RoomPlayer.Local.IsLeader);

        NetworkObject t = GetComponent<NetworkObject>();
        if (GameLauncher.instance.isServer||
            RoomPlayer.Local.IsLeader)
        {
            var test = ResourceManager.Instance.tracksDefinitions[GameManager.Instance.TrackId].preFab.GetComponent<Track>();
            //CLog.Log("vale: " + GameLauncher.instance._runner + " " + RoomPlayer.Local.Object.InputAuthority);

            var entity = GameLauncher.instance.getRunner().Spawn(
        test,
        Vector3.zero,
        Quaternion.identity,
        t.InputAuthority
    );

        }

}

}
