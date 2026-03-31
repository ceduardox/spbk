using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpStore : MasterScreen
{
    public Transform panelPowerUpsStore;
    public Transform panelPowerUpsBag;
    //public Transform panelEquipados;
    public itemPowerUpStore slot1;
    public itemPowerUpStore slot2;
    public UnityEngine.UI.Button btn_Inventory;
    public UnityEngine.UI.Button btn_Store;
    //public int maxItemLoad;
    bool slotsChange;
    bool isInvetario = true;

    public GameObject panelStore;
    public GameObject panelBag;
    /*enum CLassPowerUps
    {
        POWERUPS,
        SLOT1,
        SLOT2
    }
    */

    public void Update()
    {
        if (TimerCounterScreen._instance)
        {
            if (!TimerCounterScreen._instance.permitirCambios)
            {

                //back();
                GetComponent<UIScreen>().Back();

            }
        }
    }


    private void OnEnable()
    {
        //isInvetario = true;
        //StartPowerUps();
        switchPanel(true);
        
    }
    // Start is called before the first frame update
    void StartPowerUps()
    {
        Transform panelTemporal;

        if (isInvetario)
            panelTemporal = panelPowerUpsBag;
        else
            panelTemporal = panelPowerUpsStore;

        for (int i = 1; i < panelTemporal.childCount; i++)
            Destroy(panelTemporal.GetChild(i).gameObject);
        int contador=0;
        bool firstItem = true;
        bool crearConfig=true;
        GameObject buttonTMP;
        itemPowerUpStore itemPowerUp;

        PlayerDataTitle.PlayerDataTi.TryGetValue(ClassPart.POWERUPS.ToString(),
                               out List<PlayerD> playerData);//traigo el dataPlayerTitle
        bool vacio = true;

        foreach (ItemBase _itempCatalogo in Catalogo.getCatalogo(Catalogos.PowersUps))
        {

            if (firstItem) buttonTMP = panelTemporal.GetChild(0).gameObject; ////////////////////////////////////////////////////////Al primer item lo salto y para los siguientes los clono
            else buttonTMP = Instantiate(panelTemporal.GetChild(0).gameObject, panelTemporal);
            buttonTMP.name = _itempCatalogo.DisplayName;
            itemPowerUp = buttonTMP.GetComponent<itemPowerUpStore>();
            itemPowerUp.reset();
            //if (isInvetario)
            buttonTMP.SetActive(false);
            if(itemPowerUp.setData(_itempCatalogo.DisplayName,
                                _itempCatalogo.Id,
                                _itempCatalogo.Description,
                                _itempCatalogo.getCustomData(CustomDataItem.Level)))
            {
                 buttonTMP.transform.SetAsLastSibling();

            }
            else buttonTMP.transform.SetAsFirstSibling();
            

            foreach (ProductBase itemStoreTMP in Store.getStore(Stores.PowerUps))
            {
                if (itemPowerUp.id.Equals(itemStoreTMP.Id))
                    itemPowerUp.setPrice(itemStoreTMP.PriceNL);
            }
            itemPowerUp.setClass(_itempCatalogo.ItemClass);
            //Esta en el Store
            if (!isInvetario)
            {
                vacio = false;
                foreach (ProductBase itemStore in Store.getStore(Stores.PowerUps))
                {
                    if (itemStore.Id.Equals(_itempCatalogo.Id))
                    {
                        CLog.Log("Encontre: " + itemPowerUp.nameItem + " " + _itempCatalogo.ItemClass);

                        _itempCatalogo.isStore = true;
                        buttonTMP.SetActive(true);
                        contador++;
                        itemPowerUp.setCount(10);
                        break;
                    }
                }
               
            }
            
            foreach (PItemBase itemInventory in Inventory.Inventario)
            {
                //CLog.Log("ITEMS SON: " + itemInventory.Id);
                if (itemInventory.Id.Equals(_itempCatalogo.Id))
                {
                    _itempCatalogo.isInventory = true;
                    vacio = false;
                    if (isInvetario)
                    {
                        contador++;
                        buttonTMP.SetActive(_itempCatalogo.isInventory);
                        itemPowerUp.setCount(itemInventory.Amount);
                        
                    }
                    //_itempCatalogo.isStore = true;


                    //esta en el inventario puede estar equipado
                    //CLog.Log("ESTO VALE PLAYERDATA: " + playerData + " " + firstItem);              
                    if (playerData != null)
                    {
                        crearConfig = false;
                        foreach (PlayerD _playerD in playerData)
                        {
                            if (itemInventory.Id.Equals(_playerD.Id))
                            {
                                if (itemInventory.Amount > 0)
                                {
                                    itemPowerUpStore  tmp=slot2;

                                    if (_playerD.ClassPart == ClassPart.SLOT1)
                                        tmp = slot1;
                                    tmp.gameObject.SetActive(false);

                                    tmp.setEquipped(itemPowerUp.nameItem.text,
                                                        itemPowerUp.id,
                                                        (itemInventory.Amount >=PlayfabManager.instance.MaxPowerUps? PlayfabManager.instance.MaxPowerUps: itemInventory.Amount).ToString(),
                                                        itemPowerUp.icon.sprite,itemPowerUp.backIcon.sprite); 


                                }
                            }
                        }
                        break;
                    }
                    else if(crearConfig)
                    {
                        CLog.Log("ESTO VALE PLAYERDATA: " + playerData + " " + firstItem);
                        List<PlayerD> list = new List<PlayerD>();
                        list.Add(new PlayerD("", ClassPart.SLOT1));
                        list.Add(new PlayerD("", ClassPart.SLOT2));
                        PlayerDataTitle.updateData(ClassPart.POWERUPS.ToString(), list);
                        crearConfig = false;


                    }
                }

            }

            firstItem = false;
        }
        if (vacio)
        {
            CLog.Log("VACIO VALE; " + vacio);
            switchPanel(false);
            btn_Inventory.interactable = false;

        }

        setLengPanelBotones(panelTemporal, contador);
    }

    public void setLengPanelBotones(Transform panelBotonesPartes, int contador)
    {
        if (panelBotonesPartes.childCount > 0)
        {
            panelBotonesPartes.GetComponent<RectTransform>().sizeDelta = new Vector2(panelBotonesPartes.GetComponent<RectTransform>().sizeDelta.x,
                                                                                     16+(panelBotonesPartes.GetChild(0).GetComponent<RectTransform>().sizeDelta.y) * ((contador+1)/2) );// = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, content.GetChild(0).GetComponent<RectTransform>().sizeDelta.y*10);
            CLog.Log("El contador me dio: " + contador);
        }

    }

    public void switchPanel(bool _inventario)
    {
        isInvetario = _inventario;
        btn_Store.interactable = !(btn_Inventory.interactable=!_inventario);
        panelBag.SetActive(isInvetario);
        panelStore.SetActive(!isInvetario);

        StartPowerUps();
    }
    public void loadConfig()
    {
        PlayerDataTitle.PlayerDataTi.TryGetValue(ClassPart.POWERUPS.ToString(),
                                       out List<PlayerD> playerData);//traigo el dataPlayerTitle
        if(playerData!=null)
        {
            foreach(PlayerD _playerD in playerData)
            {
                foreach (PItemBase itemInventory in Inventory.Inventario)
                {
                    if (itemInventory.Id.Equals(_playerD.Id))
                    {
                        if(itemInventory.Amount>0)
                        {
                            if (itemInventory.Amount >= PlayfabManager.instance.MaxPowerUps)
                            {
                                //slot1.setEquipped()
                            }
                        }
                    }
                }
                
            }

        }

    }


    itemPowerUpStore itemBuy;

    public void setItem(GameObject _btn)
    {
        itemBuy = _btn.GetComponent<itemPowerUpStore>();
        if (isInvetario) equippedItem();
    }

    Coroutine buyCoroutine;
    public void buyItem(PopUps _popUp)
    {
        if(buyCoroutine==null)
        buyCoroutine=StartCoroutine(buyItemCoroutine());
    }

    public void equippedItem()
    {
        //int v = RoomPlayer.Local.Kart.OnLapCompleted
        //GameManager.Instance.GameType.lapCount
            
        int count = int.Parse(itemBuy.count.text);

        if (count > 0)
        {
            if (count > PlayfabManager.instance.MaxPowerUps)
                count = PlayfabManager.instance.MaxPowerUps;

            itemPowerUpStore tmp=null;
            
            
 

            if (itemBuy.id.Equals(slot1.id)|| slot1.id=="")
                tmp = slot1;
            else if (itemBuy.id.Equals(slot2.id) || slot2.id == "")
                tmp = slot2;
            else
            {
                slotsChange = !slotsChange;
                if (slotsChange)
                    tmp = slot1;
                else
                    tmp = slot2;

            }

                tmp.setEquipped(itemBuy.nameItem.text, itemBuy.id, count.ToString(), itemBuy.icon.sprite, itemBuy.backIcon.sprite);
            
            if (updateConfig(tmp, true))
                StartCoroutine(updateConfigPlayFab());
        }
        //slot1.isActiveAndEnabled();
        //panelEquipados
    }

    public void removeEquipped(itemPowerUpStore _btn)
    {
        CLog.Log("MANDE A REMOVER: " + _btn);
        
        if (updateConfig(_btn, false))
            StartCoroutine(updateConfigPlayFab());
        _btn.gameObject.SetActive(false);
    }

    public bool updateConfig(itemPowerUpStore _idPart, bool _add)
    {
   //     try
        {

            for (int i = 0; i < PlayerDataTitle.PlayerDataTi[ClassPart.POWERUPS.ToString()].Count; i++)
            {
              //  CLog.Log("COMPARO: " + _part.Id + " con: " + idPart.name);
                if (PlayerDataTitle.PlayerDataTi[ClassPart.POWERUPS.ToString()][i].ClassPart.Equals(_idPart.classPu))
                {

               //     CLog.Log("Encontre para reemplazar: " + PlayerDataTitle.PlayerDataTi[idTMP.ToString()][i].Id + " - " + _idPart.name);

                    PlayerDataTitle.PlayerDataTi[ClassPart.POWERUPS.ToString()].Remove(PlayerDataTitle.PlayerDataTi[ClassPart.POWERUPS.ToString()][i]);
                    break;
                }

            }
            if (_add) PlayerDataTitle.PlayerDataTi[ClassPart.POWERUPS.ToString()].Add(new PlayerD(_idPart.id, _idPart.classPu));
            else PlayerDataTitle.PlayerDataTi[ClassPart.POWERUPS.ToString()].Add(new PlayerD("", _idPart.classPu));
            return true;
        }
     //   catch (System.Exception e)
        {
       //     CLog.LogError("EXCEPTION: " + e);
            return false;
        }

    }


    IEnumerator updateConfigPlayFab()
    {

        yield return StartCoroutine(PlayerDataTitle.updateConfigPlayFab("POWERUPS"));


    }



    IEnumerator buyItemCoroutine()
    {
        /*
        if (int.Parse(itemBuy.price.text) > PlayfabManager.instance.getTLN())
        {
            GameLauncher.instance.sinFondos();
            buyItemCoroustine = null;
            yield break;
        }
        */

        //idKart //Kart a comprar
        //LittlePopUpManager.instance.setSmallPopUp("Realizando operacion");//xxxt-
        LittlePopUpManager.instance.setSmallPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_BUY));//xxxt-

        Busines.buyItem(Catalogos.PowersUps.ToString(),
                            itemBuy.id,
                            itemBuy.price,
                            Currencys.NL.ToString(),
                            Stores.PowerUps.ToString(),
                            itemBuy.id,
                            Catalogo.getItem(Catalogos.PowersUps.ToString(),itemBuy.id));

        yield return null;

        yield return new WaitWhile(() => DataEconomy.BuyStatus == BuyStatus.BUYING);

        if (DataEconomy.BuyStatus == BuyStatus.ERROR)
        {
            CLog.Log("ERROR EN LA COMPRA DEL POWER UP" + itemBuy.id + " " + itemBuy.price);
            //PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_ERROR), "Error en la compra de: " + itemBuy.id + " " + itemBuy.price.text, IconosPopUp.error, false);
            //PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_ERROR), TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_ERRORBUY).Replace("XXX", itemBuy.id).Replace("YYY", itemBuy.price.text), IconosPopUp.error, false);
            LittlePopUpManager.instance.setSmallPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_ERRORBUY).Replace("XXX", itemBuy.id).Replace("YYY", itemBuy.price.ToString()));

            buyCoroutine = null;
            yield break;
        }
        else //Compra exitosa
        {
            CLog.Log("COMPRA EXITOSA DEL POWER UP " + itemBuy.id + " " + itemBuy.price);
            Busines.AddExpertice(new List<string> { PlayfabManager.instance.IdPlayFab },
                        new List<int> { (int)Catalogo.getItem(Catalogos.PowersUps.ToString(), itemBuy.id).getCustomData(CustomDataItem.Xp) }, 0);
            //itemBuy.addCount(Busines.consumibleCountBuy);
            GetComponent<UIScreen>().CloseModalUI(itemBuy.popUp.GetComponent<UIScreen>());
            //equippedItem();
            btn_Inventory.interactable = true;

            //StartPowerUps();
            Inventory.importInventory();

            yield return new WaitForSeconds(.1f);
            yield return new WaitWhile(() => DataEconomy.ECONOMYSTATUS == EconomyStatus.DOWNLOADING);
            

        }
        buyCoroutine = null;
    }

    public void showPanelInventory()
    {
        panelBag.SetActive(false);
        panelStore.SetActive(true);
    }
    public void showPanelStore()
    {
        panelStore.SetActive(false);
        panelBag.SetActive(true);
    }

}
