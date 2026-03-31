using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class itemClanRolUI : MonoBehaviour
{
    public TextMeshProUGUI RolPlayer;
    void Start()
    {
        
    }
    public void setRol(string rol)
    {
        RolPlayer.text = rol;
    }
    
}
