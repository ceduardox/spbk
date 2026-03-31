using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlayers : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform player;
    public Transform cameraMP;
    public Material playerOne;
    Camera cameraMiniMap;
    KartEntity kart;
    //int minZoom = 20;
    public static float minZoom = 20;


    IEnumerator Start()
    {
        GetComponent<Renderer>().enabled = true; 

        // Wait until GameManager and minimap camera are initialized.
        yield return new WaitWhile(() => GameManager.Instance == null || GameManager.Instance.camera == null);

        Transform kartRoot = transform.parent != null ? transform.parent.parent : null;
        if (kartRoot == null)
        {
            yield break;
        }

        kart = kartRoot.GetComponent<KartEntity>();
        if (!kart || kart.Object == null)
        {
            yield break;
        }

        if (kart.Object.HasInputAuthority)
        {
            transform.localScale = Vector3.one * 8;
            GetComponent<Renderer>().material = playerOne;
            GameObject miniMapObj = GameObject.Find("CamerMiniMap");
            if (!miniMapObj)
            {
                yield break;
            }
            cameraMP = miniMapObj.transform;  
            cameraMP.parent = transform;
            cameraMP.localPosition = new Vector3(0, 5, 0);
            cameraMP.localEulerAngles = new Vector3(0, 0, 0);
            cameraMP.eulerAngles = new Vector3(90, cameraMP.eulerAngles.y, cameraMP.eulerAngles.z);
            cameraMiniMap = cameraMP.GetComponent<Camera>();
            if (cameraMiniMap != null)
            {
                cameraMiniMap.orthographicSize = 40;
            }
        }
        else 
        {
            transform.localScale = Vector3.one * 6;
            kart = null;
            //player = transform.parent.parent;
            //transform.parent = player.parent;
        }
        player = kartRoot;
        //transform.parent = player.parent;


    }

    public static void setZoom(float _value) 
        {
        if (_value == 0)
            minZoom = 20;
        else minZoom = 20 + 30 * _value;
    }
    // Update is called once per frame 
    void LateUpdate()
    {
        if (player)
        {
            transform.position = new Vector3(player.position.x, 160, player.position.z);
            //Debug.Log("-*- kart vale: " + kart + " " + cameraMiniMap.orthographicSize);   
            if (kart)
            {
                if (cameraMiniMap == null || kart.Controller == null)
                    return;

                cameraMiniMap.orthographicSize = Mathf.Lerp(cameraMiniMap.orthographicSize,
                                                            minZoom +
                                                            ((kart.Controller.RealSpeed < 0 ? kart.Controller.RealSpeed * -1 : kart.Controller.RealSpeed) / kart.Controller.maxSpeedBoosting) * (minZoom + 25), Time.deltaTime * 4);
            }
        }
    }
}
