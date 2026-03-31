using Fusion;
using UnityEngine;

public class KartItemController : KartComponent {
    public float equipItemTimeout = 3f;
    public float useItemTimeout = 2.5f;

    [Networked]
    public TickTimer EquipCooldown { get; set; }
    
    public bool CanUseItem => Kart.HeldItemIndex != -1 && EquipCooldown.ExpiredOrNotRunning(Runner);
    public bool CanUseItem2 => Kart.HeldItemIndex2 >=0;// && EquipCooldown.ExpiredOrNotRunning(Runner); //ADDED
    public bool CanUseItem3 => Kart.HeldItemIndex3 >=0;// && EquipCooldown.ExpiredOrNotRunning(Runner); //ADDED

    public override void OnEquipItem(Powerup powerup, float timeUntilCanUse) {
        base.OnEquipItem(powerup, timeUntilCanUse);

        EquipCooldown = TickTimer.CreateFromSeconds(Runner, equipItemTimeout);
    }

    //USAR ITEM
    public void UseItem() {
        if ( !CanUseItem || !Kart.Controller.CanDrive) {
            // We dont want to play the horn on re-simulations.
            if ( !Runner.IsForward ) return;
            
            Kart.Audio.PlayHorn();
        } else {
            CLog.Log("- usar Item -");
            Kart.HeldItem.Use(Runner, Kart);
            //Kart.HeldItem2.Use(Runner, Kart);
            Kart.HeldItemIndex = -1;
        }
    }
    public void UseItem2()//ADDED ITEM 2
    {
        if (!CanUseItem2|| !Kart.Controller.CanDrive)
        {
            // We dont want to play the horn on re-simulations.
            if (!Runner.IsForward) return;

            Kart.Audio.PlayHorn();
        }
        else
        {
            CLog.Log("- usar Item2 -");
            Kart.HeldItem2.Use(Runner, Kart);
            //Kart.HeldItem2.Use(Runner, Kart);
            
            if (Kart.Controller.userItem2)
            {
                Kart.Controller.userItem2 = false;
                if (Object.InputAuthority)
                {                    
                    CLog.Log("ESTOY MANDANDO ITEM 2 " + Kart.HeldItemIndex2);
                    Kart.HeldItemCount2--;
                    if (Kart.HeldItemCount2 > 0) Kart.HeldItemIndex2 = ResourceManager.Instance.getPowerupIndex(Kart.Controller.RoomUser.getPu()[0].classPart);
                    else Kart.HeldItemIndex2 = -2;

                    Busines.ConsumeItem(1, Kart.Controller.RoomUser.getPu()[0].id);
                }

            }
            Kart.HeldItemIndex2 = -1;
            Kart.powerUp1 = TickTimer.CreateFromSeconds(Runner, 5);

        }
    }
    public void UseItem3()//ADDED ITEM 3
    {
        if (!CanUseItem3 || !Kart.Controller.CanDrive)
        {
            // We dont want to play the horn on re-simulations.
            if (!Runner.IsForward) return;

            Kart.Audio.PlayHorn();
        }
        else
        {
            CLog.Log("- usar Item3 -");
            Kart.HeldItem3.Use(Runner, Kart);

            if (Kart.Controller.userItem3)
            {
                Kart.Controller.userItem3 = false;
                if (Object.InputAuthority)
                {
                    CLog.Log("ESTOY MANDANDO ITEM 2 " + Kart.HeldItemIndex3);
                    Kart.HeldItemCount3--;
                    if (Kart.HeldItemCount3> 0) Kart.HeldItemIndex3 = ResourceManager.Instance.getPowerupIndex(Kart.Controller.RoomUser.getPu()[1].classPart);
                    else Kart.HeldItemIndex3 = -2;

                    Busines.ConsumeItem(1, Kart.Controller.RoomUser.getPu()[1].id);
                }

            }
            Kart.HeldItemIndex3 = -1;
            Kart.powerUp2 = TickTimer.CreateFromSeconds(Runner, 5);
        }
    }
}