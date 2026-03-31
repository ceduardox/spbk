using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClanUIScreenPopUp : MonoBehaviour
{
    public static ClanUIScreenPopUp instance;
    public Transform ContentItem;
    public GameObject itemListContent;
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
    private void Start()
    {
        checkRolPlayer();
    }
    private void OnEnable()
    {
        checkRolPlayer();
    }
    void checkRolPlayer()
    {
        if (UIClanScreen.instance.isAdmin)
        {
            ClanSystem.ListSolicitudes();
        }
        else
        {
            ClanSystem.GetInvitations();
        }
    }
    public void cargarListaInvitacines(List<Invitation> listaInv)
    {
        clearList(ContentItem);
        foreach (var NombreClan in listaInv)
        {
            GameObject obj = Instantiate(itemListContent, ContentItem);
            obj.GetComponent<ClanItemUI>().setNameClanInvitations(NombreClan.gnombre);
            obj.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(()=>{

                ClanSystem.AcceptGroupInvitation(NombreClan.gnombre);
            });
            //CLog.Log(NombreClan.gnombre);
        }
    }
    public void cargarListaSolicitudes(List<Solicitud> listaSol)
    {
        clearList(ContentItem);
        foreach (var Player in listaSol)
        {
            GameObject obj = Instantiate(itemListContent, ContentItem);
            obj.GetComponent<ClanItemUI>().setNameClanInvitations(Player.nombe);
            obj.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(() => {

                ClanSystem.AcceptSolicitude(Player.nombe);
            });
           
            //CLog.Log(NombreClan.gnombre);
        }
    }
    public void clearList(Transform container)
    {
        foreach(Transform t in container)
        {
            Destroy(t.gameObject);
        }
    }
    public void closeScreen()
    {
        gameObject.SetActive(false);
    }
}
