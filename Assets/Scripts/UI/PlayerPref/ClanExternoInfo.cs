using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.GroupsModels;
using System;
using PlayFab.ClientModels;
using Newtonsoft.Json;
public class ClanExternoInfo : MonoBehaviour
{

    public static ClanExternoInfo instance;

    [Header("TRADUCCIONES")]
    [SerializeField] TextMeshProUGUI clanCopasText;
    [SerializeField] TextMeshProUGUI miembrosText;
    [SerializeField] TextMeshProUGUI playerText;
    [SerializeField] TextMeshProUGUI rangeText;
    [SerializeField] TextMeshProUGUI copasJugadorText;
    [SerializeField] TextMeshProUGUI copasTeamText;

    [Header("INFO DATA CLAN")]
    [SerializeField] TextMeshProUGUI infoNameClanText;
    [SerializeField] TextMeshProUGUI infoClanCopasText;
    [SerializeField] TextMeshProUGUI infoMiembrosText;
    [SerializeField] TextMeshProUGUI infoDescriptionText;
    [SerializeField] TextMeshProUGUI infoRulesText;

    [Header("RESOURCES")]
    [SerializeField] GameObject prefabPlayerData;
    [SerializeField] Transform playersContainer;
    [SerializeField] string idClan;
    [SerializeField] int totalMembers;
    [SerializeField] string ClanName;
     private Dictionary<string,string> membersRoles=new Dictionary<string, string>();
    public void startPoup()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }



    public void setDataClan(string groupName){
        gameObject.SetActive(true);
        ClanName=groupName;
        PlayFabGroupsAPI.GetGroup(
            new GetGroupRequest{GroupName=groupName},
            onGetGroupSucces,
            onGetGroupError
        );


    }

    private void onGetGroupSucces(GetGroupResponse obj)
    {
        var req=new ExecuteCloudScriptRequest{FunctionName="getListMembersStruct",FunctionParameter=new {idc=obj.Group.Id}};
        PlayFabClientAPI.ExecuteCloudScript(req,onGetListMembersSucces,onGetListMembersError);
    }


    private void onGetListMembersSucces(ExecuteCloudScriptResult obj)
    {
        membersRoles.Clear();
        List<string> ids=new List<string>();
        ListGroupMembersResponse ob=JsonConvert.DeserializeObject<ListGroupMembersResponse>(obj.FunctionResult.ToString());
        foreach (var item in ob.Members)
        {
            CLog.Log(item.RoleId);
            foreach (var it in item.Members)
            {
                
                foreach (var i in it.Lineage)
                {
                    ids.Add(i.Value.Id);
                    membersRoles.Add(i.Value.Id,item.RoleId);
                }
            }
        }

        PlayFabClientAPI.ExecuteCloudScript(
            new ExecuteCloudScriptRequest{
                FunctionName="getMembersGroupProfile",
                FunctionParameter=new {ides=ids}
            },onExecuteCloudSucces,onExecuteCloudError
        );
        
     

    }
    public static Miembros dataMiembros;
    private void onExecuteCloudSucces(ExecuteCloudScriptResult obj)
    {
        dataMiembros=JsonConvert.DeserializeObject<Miembros>(obj.FunctionResult.ToString());
        setDataClan(dataMiembros,membersRoles,ClanName);
        
    }




    private void onExecuteCloudError(PlayFabError obj)
    {
        CLog.Log(obj.ErrorMessage);
    }
    private void onGetListMembersError(PlayFabError obj)
    {
        CLog.Log("ERROR AL TRAER LOS MIEMBROS DEL CLOUD");
    }





    //ESTABLECE EL CLAN CUANDO UNO VISITA EL CLAN EN EL PERFIL DE UN JUGADOR
    public void setDataClan(Miembros miembros,Dictionary<string,string> roles,string clanName){

        if (playersContainer.childCount>2)
        {
            for (int i = 2; i < playersContainer.childCount; i++)
            {
                Destroy( playersContainer.GetChild(i).gameObject);
            }
        }
        
        infoNameClanText.text=clanName;
        infoMiembrosText.text=roles.Count.ToString()+"/"+20;
        int copasTotales=0;
        int copasTotalesAcumuladas=0;
        foreach (var item in miembros.miembors)
        {   copasTotalesAcumuladas+=item.stats.Find(x=>x.StatisticName==data.Copas.ToString()).Value;
            if (roles[item.playid]=="admins")
            {
                copasTotales=item.stats.Find(x=>x.StatisticName==data.CopasClanes.ToString()).Value;
                
            }
        }
        CLog.Log(copasTotalesAcumuladas);
        CLog.Log(copasTotales);
        
        for (int i = 0; i < miembros.miembors.Count; i++)
        {
            int experiencia = miembros.miembors[i].stats.Find(x => x.StatisticName == data.Experiencia.ToString()).Value;
            int level = PlayfabManager.instance.getLevel2(experiencia);

            GameObject obj=Instantiate(prefabPlayerData,playersContainer);
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text=(i+1).ToString();
            obj.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text=miembros.miembors[i].name;
            obj.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text=roles[miembros.miembors[i].playid]=="admins"?ROL.Admin.ToString():ROL.Member.ToString();
            obj.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text=miembros.miembors[i].stats.Find(x=>x.StatisticName==data.Copas.ToString()).Value.ToString();

            //obj.transform.GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>().text=copasTotalesAcumuladas.ToString();
            obj.transform.GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>().text = level.ToString();

        }
        
        infoClanCopasText.text=copasTotalesAcumuladas.ToString();

    }

  
 
    private void onGetMembersError(PlayFabError obj)
    {
       CLog.Log(obj.ErrorMessage);
    }

    private void onGetGroupError(PlayFabError obj)
    {
        CLog.Log(obj.ErrorMessage);    
    }

}
