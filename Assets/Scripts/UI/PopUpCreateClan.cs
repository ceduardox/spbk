using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpCreateClan : MonoBehaviour
{

    public static PopUpCreateClan instance;
    public TMP_InputField clanName;
    public TextMeshProUGUI priceClan;
    //public GameObject clanScreen;
    //public GameObject noClanScreen;
    //public GameObject popUp;
    //public GameObject topClans;
    //public GameObject explore;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        priceClan.text = PlayfabManager.instance.PriceClan.ToString();
    }
    private void OnEnable()
    {
        //priceClan.text = PlayfabManager.instance.PriceClan.ToString();
        //if (topClans)
        //{
        //    topClans.SetActive(false);
        //}
        //if (explore)
        //{
        //    explore.SetActive(false);
        //}

    }
    public void HasClan()
    {

        //if (ClanSystem.hasclan)
        //{
        //    clanScreen.SetActive(true);
        //    noClanScreen.SetActive(false);


        //}
        //else
        //{
        //    clanScreen.SetActive(false);
        //    noClanScreen.SetActive(true);

        //}
    }

    public void createClan()
    {
        ClanSystem.CreateGroup(clanName.text);
    }

    public void openScreenNoClan()
    {
        //clanScreen.SetActive(false);
        //noClanScreen.SetActive(true);
        //gameObject.GetComponent<UIClanScreen>().loaderImg.SetActive(false);
    }
    public void openScreenClan()
    {
        //noClanScreen.SetActive(false);
        //clanScreen.SetActive(true);
        //gameObject.GetComponent<UIClanScreen>().loaderImg.SetActive(false);
    }
    public void closeScreenNoClan()
    {

        //noClanScreen.SetActive(false);
        //popUp.SetActive(false);
    }
}

