using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ClanItemUI : MonoBehaviour
{

    public TextMeshProUGUI NameClan;
    public void setNameClanInvitations(string NameClanInvitations)
    {
        NameClan.text = NameClanInvitations;
    }

    public void acceptInvitations()
    {
        ClanSystem.AcceptGroupInvitation(NameClan.text);
    }
    public void deniedInvitations()
    {
        //ClanSystem.(NameClan.text);
    }
}
