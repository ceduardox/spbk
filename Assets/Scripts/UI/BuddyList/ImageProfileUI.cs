using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.IO;

public class ImageProfileUI : MonoBehaviour
{
    public static ImageProfileUI instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
        //FriendList.instance.chargeItemsImages();
    }
   
   
}

