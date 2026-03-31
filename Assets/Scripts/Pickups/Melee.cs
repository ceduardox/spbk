using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Melee : SpawnedPowerup
{
    bool isServer;
    public Collider _collider;
    public ClassPart classPart;
    private void Start()
    {
        if (GameLauncher.instance.modeServerDedicado)
            isServer = GameLauncher.instance.isServer;
        else
            isServer = (RoomPlayer.Local)? RoomPlayer.Local.IsLeader:false;
        _collider = GetComponent<Collider>();
        _collider.enabled = false;
        //kart.GetComponent<Kart_Store>().driver.GetChild(0).GetComponent<Char_Store>().findMelee
        //kartParent=
    }
    
    public void enabledCollider(bool _value)
    {
        _collider.enabled = _value;
    }


    public override bool Collide(KartEntity _kart)
    {
        if (isServer)
        {
            if (kartParent != _kart) 
            {
                _kart.ImpactoKart(classPart);
                //CLog.Log("mande la mina a " + _kart);
            }
        }
        //else
          //  CLog.Log("Impaco con: " + _kart); 
            CLog.Log("Impaco con: " + _kart +" "+ isServer+" - "+ _kart+" "+ (kartParent != _kart)); 
        return true;
    }

    public void playSfx()
    {

    }
}