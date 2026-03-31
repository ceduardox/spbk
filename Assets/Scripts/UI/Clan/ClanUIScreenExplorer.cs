using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClanUIScreenExplorer : MonoBehaviour
{
    public static ClanUIScreenExplorer instance;
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
        ListaDeClanesSystem.getClans();
    }
    private void OnEnable()
    {
        ListaDeClanesSystem.getClans();
    }
    private void clearList(Transform container)
    {
        foreach(Transform t in container)
        {
            Destroy(t.gameObject);
        }
    }
    public void cargarListaClanesAround(List<string> listaClan)
    {
        clearList(ContentItem);
        CLog.Log(listaClan);
        foreach (var NombreClan in listaClan)
        {
            GameObject obj = Instantiate(itemListContent, ContentItem);
            obj.GetComponent<ExplorerClanitemUI>().setNameClanInvitations(NombreClan.ToString());
        }
    }
}
