using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectActivatorByPlatform : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsForDesktop;
    private void Awake()
    {
        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            DisableObjectsOnMovile();
        }
    }
    private void DisableObjectsOnMovile()
    {
        foreach (GameObject go in objectsForDesktop)
        {
            go.SetActive(false);
        }
    }
}
