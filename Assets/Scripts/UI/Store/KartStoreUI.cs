using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KartStoreUI : MasterScreen
{
    /// <summary>
    /// Panel que contiene los botones de los Karts
    /// </summary>
    public Transform panelBotones;
    /// <summary>
    /// Panel que contiene las partes de los autos
    /// </summary>
    public Transform panelBotonesPartes;
    private int idKart;
    private int idChar;
    public UnityEngine.UI.Button btnBuy;
    public TextMeshProUGUI txtBtnBuy;
    public TextMeshProUGUI txtBtnSwitch;

    public TextMeshProUGUI txtTittleKarts;
    public TextMeshProUGUI txtTittleParts;

    [SerializeField] public bool KartStore;

    public StatsBars statsBar;

    public GameObject panelKarts;
    public GameObject panelParts;
    public Transform panelClassParts;

    Button    btnAccesorios;
    public TextMeshProUGUI txtBtnAccesorios;


    ItemStore idPart, idL2astPart;
    Dictionary<ClassPart, ItemStore> idLastPart = new Dictionary<ClassPart, ItemStore>();
    List<ItemStore> partsTmpAdds = new List<ItemStore>();
    ItemStore kartItemSelect, kartItemStoreSelectTMP;
    ItemStore charItemSelect, charItemStoreSelectTMP;
    SpotlightGroup spotlight;
    ItemStore itTMP;
    GameObject lastClassBtn;
    AudioSource demoExhasut;

    //Prueba Animacion Compra
    Animator Panda;
    Animator childAnim;

    // Start is called before the first frame update
    //private void OnEnable()
    private void Awake()
    {
        demoExhasut = GetComponent<AudioSource>();
        btnAccesorios = txtBtnAccesorios.transform.parent.GetComponent<Button>();
    }

    public void Update()
    {
        if (TimerCounterScreen._instance)
        {
            if (TimerCounterScreen._instance.gameObject.activeSelf && !TimerCounterScreen._instance.permitirCambios)
            {

                back();
                GetComponent<MasterScreen>().Back();  
                gameObject.SetActive(false);

            }
        }
    }

    bool loadIcons = true;
    public void setPanel()
    {
        if (loadIcons)
        {
            //ResourceManager.instance.loadAllIcons();  
            loadIcons = false;
        }
        asignarID();
        refreshParts();
        KartStore = !KartStore;

        ResetPanel();
        OnEnable();



    }
    public void setPanel(bool _kartStore)
    {
        if (loadIcons)
        {
            //ResourceManager.instance.loadAllIcons();
            loadIcons = false;
        }
        KartStore = _kartStore;
    }
    private void OnEnable()
    {
        //SpotlightGroup.Search("Kart Display", out SpotlightGroup spotlight2);
        //spotlight = spotlight2;
        spotlight = SpotlightGroup._instance; ;

        StartKartList();
        spotlight.viewChar = !KartStore;
        if (spotlight.char_Store2) spotlight.char_Store2.gameObject.SetActive(spotlight.viewChar);
        statsBar.gameObject.SetActive(KartStore);
        setButonAccesorios();
    }
    void setButonAccesorios()
    {
        if (KartStore) txtBtnSwitch.text = TranslateUI.getStringUI(UI_CODE.BTN_CHARS);// "Personajes";
        else txtBtnSwitch.text = TranslateUI.getStringUI(UI_CODE.BTN_KART); //"Karts";

        if (KartStore)
            txtBtnAccesorios.text = panelKarts.activeSelf ? TranslateUI.getStringUI(UI_CODE.BTN_UPGRADE_KART) : TranslateUI.getStringUI(UI_CODE.BTN_KART);// "Mejoras" : "Karts";
        else
            txtBtnAccesorios.text = panelKarts.activeSelf ? TranslateUI.getStringUI(UI_CODE.BTN_UPGRADE_CHAR) : TranslateUI.getStringUI(UI_CODE.BTN_CHARS);//"Accesorios" : "Personajes";

    }
    private bool ShouldHideKart(ItemBase item)
    {
        if (item == null) return false;
        string id = item.Id != null ? item.Id.ToLowerInvariant() : string.Empty;
        string name = item.DisplayName != null ? item.DisplayName.ToLowerInvariant() : string.Empty;
        string tag = item.Tag != null ? item.Tag.ToLowerInvariant() : string.Empty;
        return id.Contains("blokyair") || name.Contains("blokyair") || tag.Contains("blokyair");
    }

    public void StartKartList()
    {


        if (KartStore)
            idKart = ClientInfo.KartId;
        else
            idChar = ClientInfo.CharId;
        SpinStore.instance.setCamera(KartStore);


        if (panelParts.activeSelf) switchPanel();

        for (int i = 1; i < panelBotones.childCount; i++)
        {
            Destroy(panelBotones.GetChild(i).gameObject);

        }

        int contador = 0;
        if (true || KartStore)//GESTIONA LA STORE DE KARTS
        {

            GameObject buttonTMP;
            bool firstItem = true;
            //Esta en el catalogo
            foreach (ItemBase _itemCatalogo in Catalogo.getCatalogo(KartStore ? Catalogos.Karts : Catalogos.Characters))
            {
                if (KartStore && ShouldHideKart(_itemCatalogo))
                    continue;

                bool mostrarItem = false;
                bool inInventario = false;
                string price = "0";

                //Esta en el Store
                foreach (ProductBase itemStore in Store.getStore(KartStore ? Stores.Karts : Stores.Mamiferos))
                {
                    if (itemStore.Id.Equals(_itemCatalogo.Id))
                    {
                        mostrarItem = true;
                        price = itemStore.PriceTE;
                        break; ;
                    }
                }

                //esta en el Inventario
                foreach (var item in Inventory.Inventario)//busco el ID en el inventario
                {//
                    if (item.Id.Equals(_itemCatalogo.Id))
                    {
                        //itTMP.locked(false);
                        inInventario = true;
                        mostrarItem = true;
                        break;
                    }
                }


                if (mostrarItem)
                {
                    contador++;
                    KartDefinition _kd = null;
                    DriverDefinition _df = null;
                    if (KartStore)
                        _kd = ResourceManager.Instance.getKart(int.Parse(_itemCatalogo.Id));
                    else
                        _df = ResourceManager.Instance.getChar(int.Parse(_itemCatalogo.Id));
                    //else 
                    //DriverDefinition _kd = ResourceManager.Instance.getKart(int.Parse(_itemCatalogo.Id));

                    if (KartStore ? _kd : _df)
                    {
                        if (firstItem) buttonTMP = panelBotones.GetChild(0).gameObject; ////////////////////////////////////////////////////////Al primer item lo salto y para los siguientes los clono
                        else buttonTMP = Instantiate(panelBotones.GetChild(0).gameObject, panelBotones);
                        (itTMP = buttonTMP.GetComponent<ItemStore>()).setIcon(KartStore ? _kd.itemIcon : _df.itemIcon);

                        if (_itemCatalogo.Tag != null)
                            itTMP.setPos(int.Parse(_itemCatalogo.Tag));
                        else itTMP.setPos(0);

                        if (KartStore)
                        {
                            buttonTMP.GetComponent<ItemStore>().icon.enabled = true;/// cambiado a true para mostrar icono 
                            buttonTMP.GetComponent<Image>().sprite = _kd.itemIcon;
                            //SpriteState ss = new SpriteState();
                            //ss.pressedSprite = _kd.itemIconOnClick;
                            //ss.selectedSprite = _kd.itemIconSelected;             // comentado, sprite aniciones manejadas desde itemUI
                            //buttonTMP.GetComponent<Button>().spriteState = ss;
                            itTMP.setItemsDefaultPosition(KartStore);
                        }
                        else
                        {
                            buttonTMP.GetComponent<ItemStore>().icon.enabled = true;/// cambiado a true para mostrar icono
                            buttonTMP.GetComponent<Image>().sprite = _df.itemIcon;
                            //SpriteState ss = new SpriteState();
                            //ss.pressedSprite = _df.itemIconOnClick;
                            //ss.selectedSprite = _df.itemIconSelected;
                            //buttonTMP.GetComponent<Button>().spriteState = ss;
                            itTMP.setBackgroundImg(KartStore, _df.backgroundImg);
                            itTMP.setLogoImg(KartStore, _df.characterLogo);
                            itTMP.setItemsDefaultPosition(KartStore);
                        }


                        buttonTMP.name = KartStore ? _kd.Id.ToString() : _df.Id.ToString();
                        itTMP.idkart = KartStore ? _kd.Id : _df.Id;
                        itTMP.locked(true);
                        itTMP.setNameDesc(_itemCatalogo.DisplayName, _itemCatalogo.Description, _itemCatalogo.getCustomData(CustomDataItem.Level), KartStore, _itemCatalogo);
                        

                        itTMP.setNameItem(_itemCatalogo.DisplayName, KartStore);
                        if (inInventario) itTMP.locked(false);
                        itTMP.setPrice(price);

                        if (int.Parse(itTMP.priceValue.text) == 0)
                            itTMP.locked(false);

                        itTMP.isAvailable(_itemCatalogo.getCustomData(CustomDataItem.Look) == 0);

                        //if (!itTMP.available)
                        //    buttonTMP.transform.SetAsLastSibling();



                        firstItem = false;
                    }

                }
            }
  
            //ORDENAR POR TAG PLAYFAB
            for (int k = 0; k < panelBotones.transform.childCount; k++)
            {
                GameObject child = panelBotones.transform.GetChild(k).gameObject;
                int index = child.GetComponent<ItemStore>().posChild;
                if (index != 0)
                {
                    child.transform.SetAsFirstSibling();
                }
            }

        }
        /*else
        {
            GameObject buttonTMP;
            bool f+-irstItem = true;
            foreach (DriverDefinition _kd in ResourceManager.Instance.driverDefinitions)
            {
                if (firstItem) buttonTMP = panelBotones.GetChild(0).gameObject; ////////////////////////////////////////////////////////Al primer item lo salto y para los siguientes los clono
                else buttonTMP = Instantiate(panelBotones.GetChild(0).gameObject, panelBotones);
                (itTMP = buttonTMP.GetComponent<ItemStore>()).setIcon(_kd.itemIcon);
                itTMP.name = _kd.Id.ToString();
                itTMP.locked(true);
                itTMP.idkart = _kd.Id;

                foreach (var item in Inventory.Inventario)//busco el ID en el inventario
                {//
                    if (item.Id.Equals(_kd.Id.ToString()))
                    {
                        CLog.Log("IMPRIMO: " + item.Id);
                        itTMP.locked(false);
                        break;
                    }
                }
                if (itTMP.isLocked)
                {
                    foreach (ProductBase itemStore in Store.getStore(Stores.Mamiferos))
                    {
                        if (itemStore.Id.Equals(itTMP.idkart.ToString()))
                        {
                            itTMP.setPrice(itemStore.PriceTE);
                            break;
                        }
                    }
                }

                if (int.Parse(itTMP.priceValue.text) == 0)
                    itTMP.locked(false);

                buttonTMP.name = _kd.Id.ToString();
                firstItem = false;


            }
        }

        */

        if (KartStore)
            selectDirectKart(ClientInfo.KartId.ToString());
        else
            selectDirectKart(ClientInfo.CharId.ToString());
        setLengPanelBotones(panelBotones, contador);


    }
    public void selectDirectKart(string _id)
    {
        CLog.Log("EL ID QUE CARGO ES: " + _id);

        foreach (Transform _kart in panelBotones)
        {
            if (_kart.name.Equals(_id))
            {
                selectKart(_kart.gameObject);
                break;
            }
        }
    }

    //Click Store Kart
    public void selectKart(GameObject kartIndex)
    {

        if (KartStore)
        {
            if (!kartIndex.GetComponent<ItemStore>().available)
            {
                PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_WARNING), TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_09)/*"//xxxt-Este Kart se encuentra bloqueado"*/, IconosPopUp.block, false);
                return;
            }
            btnBuyActive((kartItemStoreSelectTMP = kartIndex.GetComponent<ItemStore>()).isLocked);

            //txtBtnBuy.text = "Comprar";
            txtBtnBuy.text = TranslateUI.getStringUI(UI_CODE.KRT_STS_BUY); 

            if (!kartItemStoreSelectTMP.isLocked)//si el auto esta comprado queda como seleccionado temporal hasta aceptar
            {
                kartItemSelect = kartIndex.GetComponent<ItemStore>();
                CLog.Log("ASIGNO IN: KART" + kartItemSelect.idkart);

            }

            updateBtn(kartIndex, kartsLow, kartsHight);
            spotlight.FocusIndex(idKart = int.Parse(kartIndex.name),true);
            loadPartsKarts(true);
            statsPreviewKart(null);// idKart, null);
            txtTittleKarts.text = kartItemStoreSelectTMP.nameItemStore;
        }
        else
        {
            //refreshParts();
            if (!kartIndex.GetComponent<ItemStore>().available)
            {
                PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_WARNING), TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_20) /*"//xxxt-Este Personaje se encuentra bloqueado"*/, IconosPopUp.block, false);
                return;
            }
            btnBuyActive((charItemStoreSelectTMP = kartIndex.GetComponent<ItemStore>()).isLocked);
            txtBtnBuy.text = TranslateUI.getStringUI(UI_CODE.KRT_STS_BUY);

            if (!charItemStoreSelectTMP.isLocked)//si el char esta comprado queda como seleccionado temporal hasta aceptar
            {
                charItemSelect = kartIndex.GetComponent<ItemStore>();
                CLog.Log("ASIGNO IN: CHAR" + charItemSelect.idkart);
            }

            updateBtn(kartIndex, kartsLow, kartsHight);
            spotlight.FocusIndex(idChar = int.Parse(kartIndex.name),false);
            loadPartsChars(true);
            txtTittleKarts.text = charItemStoreSelectTMP.nameItemStore;
            //statsKart(idKart); /////////////////////REMPLAZAR POR LA HISTORIA DEL PERSONAJE
        }


        SpinStore.instance.setAngles(ClassPart.NONE);
        //refreshParts();
    }

    public void statsPreviewKart(ItemStore _id)
    {
        if (KartStore)
        {
            KartDefinition kd = ResourceManager.Instance.getKart(idKart);
            ItemBase ib = null;//
            ItemBase _ib = null;
            if (_id != null)
                _ib = Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), _id.name);

            float speed = 0, acc = 0, turn = 0;
            float speedReal = 0, accReal = 0, turnReal = 0;
            PlayerDataTitle.PlayerDataTi.TryGetValue(idKart.ToString(), out List<PlayerD> playerData);//traigo el dataPlayerTitle

            if (playerData != null)
            {

                foreach (PlayerD _part in playerData)
                {
                    ib = Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), _part.Id);
                    if (ib != null)
                    {
                        speedReal += ib.getCustomData(CustomDataItem.Speed);
                        accReal += ib.getCustomData(CustomDataItem.Acceleration);
                        turnReal += ib.getCustomData(CustomDataItem.Turn);
                    }
                }

                foreach (PlayerD _part in playerData)
                {
                    ib = Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), _part.Id);
                    if (_ib != null && ib != null)
                    {
                        if (ib.ItemClass == _ib.ItemClass)
                            ib = _ib;
                    }
                    if (ib != null)
                    {
                        speed += ib.getCustomData(CustomDataItem.Speed);
                        acc += ib.getCustomData(CustomDataItem.Acceleration);
                        turn += ib.getCustomData(CustomDataItem.Turn);
                    }
                }
            }

            if (ib != null)
            {
                ib = Catalogo.getItem(Catalogos.Karts.ToString(), idKart.ToString());

                statsBar.setStats(ib.getCustomData(CustomDataItem.Speed), ib.getCustomData(CustomDataItem.Acceleration), ib.getCustomData(CustomDataItem.Turn));
                statsBar.setRealStats(speedReal, accReal, turnReal);
                statsBar.setNewStats(speed, acc, turn);
            }
        }
        //ItemBase ib = Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), itemStore.Id.ToString());

    }
    /* public void statsPreviewKart(ItemStore _id)
     {
         //if()

         //CLog.Log("MANDO A VER: " + _id + " " + _id.isEquipped);

         int mul = 1;
         if (_id == null)
         {
             statsBar.setPreviewStats(0, 0, 0);
             return;
         }
         else if (_id.isEquipped)
             mul = 0;

         ItemBase ib = Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), _id.name);

        // statsBar.setPreviewStats(ib.getStats(CustomDataItem.Speed) * mul, ib.getStats(CustomDataItem.Acceleration) * mul, ib.getStats(CustomDataItem.Turn) * mul);

         statsKart(idKart, ib);


     }
    */
    //Click Store Drivers
    public void selectDriver(GameObject kartIndex)
    {
        CLog.Log("CARGO DRIVER: " + kartIndex);
        spotlight.FocusIndex(idKart = int.Parse(kartIndex.name), true);
        refreshParts();

        //statsBar.setStats()

        //loadPartsKarts();
        //apo
    }


    public void loadPartsKarts(bool _eqquiped)
    {
        if (kartPartCR != null)
            StopCoroutine(kartPartCR);

            kartPartCR=StartCoroutine(loadPartsKartsCR(_eqquiped));
    }
    //Carga todas las partes del auto seleccionado 
    IEnumerator loadPartsKartsCR(bool _eqquiped)
    {
        for (int i = 1; i < panelBotonesPartes.childCount; i++)
        {
            Destroy(panelBotonesPartes.GetChild(i).gameObject);
        }

        ResourceRequest request2;
        //yield return new WaitForSeconds(0);
        GameObject buttonTMP;
        bool firstItem = true;
        int contador = 0;
        LittlePopUpManager.instance.SetSmallPopupWithoutTime("Cargando Recursos");
        Debug.Log("Abro el popUP C");
        KartDefinition selectedKartDefinition = ResourceManager.Instance != null ? ResourceManager.Instance.getKart(idKart) : null;
        if (!selectedKartDefinition || !selectedKartDefinition.prefab)
        {
            CLog.LogError("No se puede cargar partes. KartDefinition nulo para id: " + idKart);
            LittlePopUpManager.instance.DesactivePopupWithoutTime();
            kartPartCR = null;
            yield break;
        }


        foreach (ItemBase _itemCatalogo in Catalogo.getCatalogo(Catalogos.Parts_Upgrade))
        {
            //CLog.Log("ENCONTRE LA SIGUIENTE PARTE: " + _itemCatalogo.Id);
            if (_itemCatalogo.Id.Contains(idKart.ToString()) ||   //Sí es un objeto esclusivo de este auto
                _itemCatalogo.Id.Contains("ALL_KARTS"))                 //Sí es un objeto golbal
            {
                foreach (ProductBase itemStoreTMP in Store.getStore(Stores.KartsParts))
                {

                    if (_itemCatalogo.Id.Equals(itemStoreTMP.Id))
                    {
                        _itemCatalogo.priceNL = itemStoreTMP.PriceNL;
                        _itemCatalogo.priceTE = itemStoreTMP.PriceTE;
                        _itemCatalogo.isStore = true;
                        break;
                    }

                }

                foreach (PItemBase itemInventory in Inventory.Inventario)
                {
                    if (itemInventory.Id.Equals(_itemCatalogo.Id))
                    {
                        _itemCatalogo.isInventory = true;
                        _itemCatalogo.isStore = true;
                        break;
                    }
                }

                if (_itemCatalogo.isStore)
                {

                    if (firstItem) buttonTMP = panelBotonesPartes.GetChild(0).gameObject; ////////////////////////////////////////////////////////Al primer item lo salto y para los siguientes los clono
                    else buttonTMP = Instantiate(panelBotonesPartes.GetChild(0).gameObject, panelBotonesPartes);
                    firstItem = false;

                    buttonTMP.name = _itemCatalogo.Id.ToString();

                    //CLog.Log(Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), pb.Id).ItemClass);
                    // ItemBase ib = Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), itemStore.Id.ToString());

                    (itTMP = buttonTMP.GetComponent<ItemStore>()).classItem = _itemCatalogo.ItemClass;
                    itTMP.setSelect(false);

                    if (itTMP.classItem == ClassPart.KART)
                        itTMP.setPrice(_itemCatalogo.priceTE);
                    else itTMP.setPrice(_itemCatalogo.priceNL);


                    //Sprite sp = ResourceManager.Instance.getIconByCategory(_itemCatalogo.ItemClass, _itemCatalogo.Id);


                    Sprite sp;



                    
                    if (_itemCatalogo.ItemClass == ClassPart.ANTENNA)
                    {

                        // AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<AudioClip>("Prefabs/IconKarts/Accesorios/" + _itemCatalogo.Id.Replace("ALL_KARTS-", ""));

                        request2 = Resources.LoadAsync<Sprite>("Prefabs/IconKarts/Accesorios/" + _itemCatalogo.Id.Replace("ALL_KARTS-", ""));

                        //sp = Resources.Load<Sprite>("Prefabs/IconKarts/Accesorios/" + _itemCatalogo.Id.Replace("ALL_KARTS-", ""));

                        yield return request2;
                        sp = request2.asset as Sprite;

                        if (!sp)
                        {
                            request2 = Resources.LoadAsync<Sprite>("Prefabs/IconKarts/Accesorios/Flags/" + _itemCatalogo.Id.Replace("ALL_KARTS-Flag_", ""));
                            yield return request2;
                            sp = request2.asset as Sprite;
                            //sp = Resources.Load<Sprite>("Prefabs/IconKarts/Accesorios/Flags/" + _itemCatalogo.Id.Replace("ALL_KARTS-Flag_", ""));
                        }

                        CLog.Log("BUSCO: " + "Prefabs/IconKarts/Accesorios/Flags/" + _itemCatalogo.Id.Replace("ALL_KARTS-Flag_", "") + "--" + sp);
                    }
                    else
                    {

                        request2 = Resources.LoadAsync<Sprite>("Prefabs/IconKarts/" + selectedKartDefinition.prefab.name + "_Parts/" + _itemCatalogo.Id.Replace(idKart + "-", ""));
                        //sp = Resources.Load<Sprite>("Prefabs/IconKarts/" + ResourceManager.instance.getKart(idKart).prefab.name + "_Parts/" + _itemCatalogo.Id.Replace(idKart + "-", ""));
                        yield return request2;
                        //while (!request2.isDone) yield return null;
                       // Debug.Log("+ entontre Sprite " + request2.isDone+" "+request2.asset);

                        sp = request2.asset as Sprite;
                        //Debug.Log("+ entontre Sprite " + sp);
                    }

                    /* if (_itemCatalogo.ItemClass == ClassPart.ANTENNA)
                     {
                         sp = Resources.Load<Sprite>("Prefabs/IconKarts/Accesorios/" + _itemCatalogo.Id.Replace("ALL_KARTS-", ""));
                         if (!sp)
                         sp = Resources.Load<Sprite>("Prefabs/IconKarts/Accesorios/Flags/" + _itemCatalogo.Id.Replace("ALL_KARTS-Flag_", ""));

                         CLog.Log("BUSCO: " + "Prefabs/IconKarts/Accesorios/Flags/" + _itemCatalogo.Id.Replace("ALL_KARTS-Flag_", "")+"--"+ sp);
                     }
                     else
                         sp = Resources.Load<Sprite>("Prefabs/IconKarts/" + ResourceManager.instance.getKart(idKart).prefab.name + "_Parts/" + _itemCatalogo.Id.Replace(idKart + "-", ""));
                     */

                    if (sp) itTMP.setIcon(sp);
                    else itTMP.setIcon(ResourceManager.Instance.getIcon(ClassPart.ACCESORIES_KART));

                    itTMP.idkart = idKart;

                    itTMP.setNameDesc(_itemCatalogo.DisplayName, _itemCatalogo.Description, _itemCatalogo.getCustomData(CustomDataItem.Level), KartStore, null);// Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), itemStore.Id).DisplayName, Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), itemStore.Id).Description);
                    itTMP.setTag(_itemCatalogo.Tag);
                    if (_eqquiped)
                    {
                        PlayerDataTitle.PlayerDataTi.TryGetValue(idKart.ToString(),
                                                                    out List<PlayerD> playerData);//traigo el dataPlayerTitle
                        buttonTMP.SetActive(false);
                        if (playerData != null)
                        {
                            foreach (PlayerD _part in playerData)
                            {

                                if (_part.Id.Equals(itTMP.name))
                                {
                                    //CLog.Log("COMPARO: " + _part.Id + " con: " + itTMP.name);
                                    itTMP.GetComponent<ItemStore>().setEquipped(true);
                                    buttonTMP.SetActive(true);
                                    /* if (buttonTMP.activeSelf)
                                         itTMP.setIcon2();*/
                                    contador++;
                                    break;
                                }
                            }
                        }
                    }
                    if (int.Parse(itTMP.priceValue.text) > 0)
                    {
                        itTMP.locked(!_itemCatalogo.isInventory);
                    }
                    else itTMP.locked(false);
                }
            }
            //filterParts(panelBotones.GetChild(0).name);// ClassPart.MOTOR.ToString());
        }
        //panelBotonesPartes.GetComponent<RectTransform>().

        LittlePopUpManager.instance.DesactivePopupWithoutTime();



        setLengPanelBotones(panelBotonesPartes, contador);
        Invoke("noOffPartsmenu", 0.01f);
        kartPartCR = null;
    }


    void noOffPartsmenu()
    {
        
        foreach (Transform t in panelClassParts)
        {
            int partsCount = 0;
            t.gameObject.SetActive(false);

            foreach (Transform p in panelBotonesPartes)
            {

                if (p.GetComponent<ItemStore>().classItem == (KartStore ? t.GetComponent<ClassMenuItem>().kartStore : t.GetComponent<ClassMenuItem>().charStore))
                {
                    partsCount++;
                    //CLog.Log("ACTIVO: " + p.gameObject + " " + t.name + " - " + ClientInfo.KartId);
                    if (partsCount == 2)
                    {
                        t.gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }



        float space = panelClassParts.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>().spacing;
        float height = panelClassParts.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
        int contador = 0;
        foreach (Transform t in panelClassParts)
            if (t.gameObject.activeSelf) contador++;


        panelClassParts.GetComponent<RectTransform>().sizeDelta = new Vector2((height + space) * (contador), panelClassParts.GetComponent<RectTransform>().sizeDelta.y);// = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, content.GetChild(0).GetComponent<RectTransform>().sizeDelta.y*10);

        //panelClassParts.transform.localPosition = new Vector3(00, panelClassParts.transform.localPosition.y, panelClassParts.transform.localPosition.z);

    }

    Coroutine kartPartCR, charPartCR;
    public void loadPartsChars(bool _eqquiped)
    {
        if (charPartCR != null)
            StopCoroutine(charPartCR);

        charPartCR = StartCoroutine(loadPartsCharsCR(_eqquiped));
    }
    IEnumerator loadPartsCharsCR(bool _eqquiped)
    {
        LittlePopUpManager.instance.SetSmallPopupWithoutTime("Cargando Recursos");
        for (int i = 1; i < panelBotonesPartes.childCount; i++)
        {
            Destroy(panelBotonesPartes.GetChild(i).gameObject);
        }
        ResourceRequest request2;
        GameObject buttonTMP;
        bool firstItem = true;
        ItemStore itTMP;
        int contador = 0;
        //ProductBase itemStore;

        foreach (ItemBase _itemCatalogo in Catalogo.getCatalogo(Catalogos.Character_Upgrade))
        {
            if (_itemCatalogo.Id.Contains(idChar.ToString()) ||   //Sí es un objeto esclusivo de este auto
                _itemCatalogo.Id.Contains("ALL_CHAR"))                 //Sí es un objeto golbal
            {
                foreach (ProductBase itemStoreTMP in Store.getStore(Stores.CharParts))
                {
                    if (_itemCatalogo.Id.Equals(itemStoreTMP.Id))
                    {
                        _itemCatalogo.priceNL = itemStoreTMP.PriceNL;
                        _itemCatalogo.priceTE = itemStoreTMP.PriceTE;
                        _itemCatalogo.isStore = true;

                        break;
                    }

                }

                foreach (PItemBase itemInventory in Inventory.Inventario)
                {
                    if (itemInventory.Id.Equals(_itemCatalogo.Id))
                    {
                        _itemCatalogo.isInventory = true;
                        _itemCatalogo.isStore = true;
                        break;
                    }
                }

                if (_itemCatalogo.isStore)
                {
                    //CLog.Log("ENCONTRE ESTOS ACCESO: " + itemStore.Id);

                    if (firstItem) buttonTMP = panelBotonesPartes.GetChild(0).gameObject; ////////////////////////////////////////////////////////Al primer item lo salto y para los siguientes los clono
                    else buttonTMP = Instantiate(panelBotonesPartes.GetChild(0).gameObject, panelBotonesPartes);
                    firstItem = false;

                    buttonTMP.name = _itemCatalogo.Id.ToString();

                    //CLog.Log(Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), pb.Id).ItemClass);
                    //ItemBase ib = _itemCatalogo;// Catalogo.getItem(Catalogos.Character_Upgrade.ToString(), itemStore.Id.ToString());

                    (itTMP = buttonTMP.GetComponent<ItemStore>()).classItem = _itemCatalogo.ItemClass;
                    itTMP.setSelect(false);

                    if (itTMP.classItem == ClassPart.CHAR)
                        itTMP.setPrice(_itemCatalogo.priceTE);
                    else itTMP.setPrice(_itemCatalogo.priceNL);

                    //Sprite sp = ResourceManager.Instance.getIcon(_itemCatalogo.ItemClass);
                    // = ResourceManager.Instance.getIconByCategory(_itemCatalogo.ItemClass,_itemCatalogo.Id);

                    //CLog.Log("ESTOY BUSCANO: " + "Prefabs/Drivers/IconAccesories/BODY/" + _itemCatalogo.Id);

                    /*
                    if (_itemCatalogo.ItemClass == ClassPart.BODY)
                        sp = Resources.Load<Sprite>("Prefabs/Drivers/IconAccesories/BODY/" + _itemCatalogo.Id);
                    else
                        
                        */ 

                    //Sprite sp = Resources.Load<Sprite>("Prefabs/Drivers/IconAccesories/" + _itemCatalogo.ItemClass.ToString() + "/" + _itemCatalogo.Id);//Resources.Load<Sprite>("Prefabs/IconKarts/" + ResourceManager.instance.getKart(idKart).prefab.name + "_Parts/" + _itemCatalogo.Id.Replace(idKart + "-", ""));
                    request2 = Resources.LoadAsync<Sprite>("Prefabs/Drivers/IconAccesories/" + _itemCatalogo.ItemClass.ToString() + "/" + _itemCatalogo.Id);
                    yield return request2;
                    Sprite sp = request2.asset as Sprite;
                    



                    if (sp)
                    {
                        itTMP.setIcon(sp);
                    }
                    else itTMP.setIcon(ResourceManager.Instance.getIcon(ClassPart.ACCESORIES_CHAR));

                    itTMP.idkart = idChar;

                    itTMP.setNameDesc(_itemCatalogo.DisplayName, _itemCatalogo.Description, _itemCatalogo.getCustomData(CustomDataItem.Level),KartStore, null);// Catalogo.getItem(Catalogos.Character_Upgrade.ToString(), itemStore.Id).DisplayName, Catalogo.getItem(Catalogos.Character_Upgrade.ToString(), itemStore.Id).Description);

                    if (_eqquiped)
                    {
                        //CLog.LogError("EL ID QUE COMPARO VALE; " + idChar.ToString());
                        PlayerDataTitle.PlayerDataTi.TryGetValue(idChar.ToString(), out List<PlayerD> playerData);//traigo el dataPlayerTitle
                        buttonTMP.SetActive(false);
                        foreach (PlayerD _part in playerData)
                        {
                            //  CLog.Log("COMPARO: " + _part.Id + " con: " + itTMP.name);
                            if (_part.Id.Equals(itTMP.name))
                            {

                                itTMP.GetComponent<ItemStore>().setEquipped(true);
                                buttonTMP.SetActive(true);
                                contador++;
                                break;
                            }
                        }
                    }
                    //else
                    {
                        if (int.Parse(itTMP.priceValue.text) > 0)
                        {
                            itTMP.locked(!_itemCatalogo.isInventory);
                        }
                        else itTMP.locked(false);

                    }
                }
            }

        }
        LittlePopUpManager.instance.DesactivePopupWithoutTime();



        Invoke("noOffPartsmenu", 0.1f);
        /*
        foreach (Transform t in panelClassParts)
        {
            t.gameObject.SetActive(false);
            foreach (Transform p in panelBotonesPartes)
            {
                if (p.GetComponent<ItemStore>().classItem == t.GetComponent<ClassMenuItem>().charStore)
                {
                    t.gameObject.SetActive(true);
                    break;
                }
            }
        }
        */
        setLengPanelBotones(panelBotonesPartes, contador);
        charPartCR = null;
        //filterParts(panelBotones.GetChild(0).name);// ClassPart.MOTOR.ToString());
    }

    //Muestra solo los objeto de la Clase seleccioanda
    public void filterParts(GameObject _btn)//LLamada desde la UI
    {
        refreshParts();
        lastClassBtn = _btn;
        if (KartStore)
            filterParts(_btn.GetComponent<ClassMenuItem>().kartStore.ToString());
        else
            filterParts(_btn.GetComponent<ClassMenuItem>().charStore.ToString());




        updateBtn(_btn, partsLow, partsHight);


        if (KartStore) statsPreviewKart(null);
    }

    public void filterParts(ClassPart _class)
    {
        foreach (Transform btn in panelClassParts)
        {
            if ((KartStore ? btn.GetComponent<ClassMenuItem>().kartStore : btn.GetComponent<ClassMenuItem>().charStore) == _class)
            {
                filterParts(btn.gameObject);
            }
        }
    }

    public void filterParts(string _class)
    {
        SpinStore.instance.setAngles(ItemBase.asignarClass(_class)); //Angulos de la Camara

        btnBuyActive(false);
        PlayerDataTitle.PlayerDataTi.TryGetValue(KartStore ? idKart.ToString() : idChar.ToString(),
                                                    out List<PlayerD> playerData);//traigo el dataPlayerTitle
        int contador = 0;
        foreach (Transform t in panelBotonesPartes)
        {

            if (!t.GetComponent<ItemStore>()) continue;

            t.gameObject.SetActive(t.GetComponent<ItemStore>().classItem.ToString().Contains(_class) || _class.Equals(ClassPart.ALL.ToString()));//Activo los items de acuerdo a la clase
            if (t.gameObject.activeSelf) contador++;
            t.GetComponent<ItemStore>().setSelect(false);
            t.GetComponent<ItemStore>().setEquipped(false);


            //Marco como seleccionada la pieza que esta equipada
            foreach (PlayerD _part in playerData)
            {
                // CLog.Log("COMPARO: " + _part.Id + " con: " + t.name);
                if (_part.Id.Equals(t.name))
                {
                    t.GetComponent<ItemStore>().setEquipped(true);
                    if (t.gameObject.activeSelf) txtTittleParts.text = t.GetComponent<ItemStore>().nameItemStore;

                    //CLog.Log("EL PANEL ESTA: " + panelParts.activeSelf);
                    if (panelParts.activeSelf) t.GetComponent<ItemStore>().setSelect(true);
                    t.SetAsFirstSibling();
                }
            }
        }
        setLengPanelBotones(panelBotonesPartes, contador);

    }


    public void setLengPanelBotones(Transform panel, int contador)
    {

        float space = panel.GetComponent<UnityEngine.UI.GridLayoutGroup>().spacing.y;
        float height = panel.GetComponent<UnityEngine.UI.GridLayoutGroup>().cellSize.y;

        //CLog.Log("Pannel vale: " + panel.name + " - " + contador + " - " + space+" - "+( (contador % 3) == 0 ? contador / 3 : (contador / 3) + 1)+" - "+ panel.GetChild(0).GetComponent<RectTransform>().sizeDelta.y+" - "+ panel.GetChild(0).name);
        if (panel.childCount > 0)
        {
            panel.GetComponent<RectTransform>().sizeDelta = new Vector2(panel.GetComponent<RectTransform>().sizeDelta.x, (height + space) * ((contador % 2) == 0 ? contador / 2 : (contador / 2) + 1) + 2);// = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, content.GetChild(0).GetComponent<RectTransform>().sizeDelta.y*10);
            //CLog.Log("El contador me dio: " + contador);
        }

    }

    public void refreshParts()
    {
        if (partsTmpAdds.Count == 0) return;
        PlayerDataTitle.PlayerDataTi.TryGetValue(KartStore ? idKart.ToString() : idChar.ToString(),
                                               out List<PlayerD> playerData);//traigo el dataPlayerTitle

        foreach (ItemStore _parTMP in partsTmpAdds)
        {
            bool remove = true;
            bool cambiar = false;
            PlayerD _partChange = null;
            foreach (PlayerD _part in playerData)
            {
                //CLog.Log("Comparo: "+ _part.ClassPart+" "+ _parTMP.classItem);
                if (_part.ClassPart == _parTMP.classItem)
                {

                    //CLog.Log("Comparo2 : " + _part.Id + " " + _parTMP.name);
                    if (_parTMP)
                    {
                        remove = false;
                        if (_part.Id == _parTMP.name)
                        {
                            cambiar = false;
                        }
                        else
                        {
                            cambiar = true;
                            _partChange = _part;
                        }
                    }
                    break;
                }

            }
            //if (modificar)
            {

                {
                    if (KartStore)
                    {
                        //   if (cambiar)
                        //   spotlight.kart_Store.changePart(ResourceManager.Instance.getKart(idKart), new PlayerD(_partChange.Id, _partChange.ClassPart),false);
                        CLog.Log("ERROR: " + idKart + " " + _parTMP + " " + spotlight.kart_Store + " - " + ResourceManager.Instance.getKart(idKart));
                        if (remove)
                            if (_parTMP && spotlight.kart_Store) spotlight.kart_Store.changePart(ResourceManager.Instance.getKart(idKart), new PlayerD(_parTMP.name, _parTMP.classItem), true);
                    }
                    else
                    {
                        //  if (cambiar)
                        //   spotlight.char_Store.changeCharPart(ResourceManager.Instance.getChar(idChar), new PlayerD(_partChange.Id, _partChange.ClassPart), false);
                        if (remove)
                            if (_parTMP && spotlight.char_Store) spotlight.char_Store.changeCharPart(ResourceManager.Instance.getChar(idChar), new PlayerD(_parTMP.name, _parTMP.classItem), true);
                    }

                }
            }
        }


        foreach (PlayerD _part in playerData)
        {

            if (KartStore && spotlight.kart_Store)
                spotlight.kart_Store.changePart(ResourceManager.Instance.getKart(idKart), _part, false);
            else
                if (spotlight.char_Store) spotlight.char_Store.changeCharPart(ResourceManager.Instance.getChar(idChar), _part, false);

        }


        partsTmpAdds.Clear();
    }
    public void previewPart(GameObject _id)
    {
        //CLog.Log("LLAMO AL OBJETO: " + _id);
        bool remove = false;

        {
            SpinStore.instance.setAngles((idPart = _id.GetComponent<ItemStore>()).classItem); //Angulos de la Camara
            txtTittleParts.text = idPart.nameItemStore;



            PlayerDataTitle.PlayerDataTi.TryGetValue(KartStore ? idKart.ToString() : idChar.ToString(), out List<PlayerD> playerData);
            //if (charItemStoreSelectTMP)

            /*foreach (PlayerD _pd in playerData)//Verifico si ya está instalda
            {
                if (_pd.Id.Equals(_id.name))
                {
                    if (KartStore)
                    {
                        return;
                    }
                    else
                    {
                     remove = true;
                    }
                }
            }*/

            if (KartStore) ;//return;
            else
            {

                btnBuyActive(false);
                if (idPart.isSelect)
                {
                    remove = true;
                    if (idPart.isEquipped)
                    {
                        updateConfig(idPart, false);
                        idPart.setEquipped(false);
                    }
                }
            }


            /*if (idLastPart.Count > 0)
            {
                if (idLastPart[idPart.classItem])
                {
                    if (idLastPart[idPart.classItem] == idPart)
                    {
                        CLog.Log("COMPARO: " + idLastPart[idPart.classItem]+" "+idPart);
                        if (KartStore) return;
                        else
                        {
                            remove = true;
                            btnBuyActive(false);
                            if (idPart.isEquipped)
                            {
                                updateConfig(idPart, false);
                                idPart.setEquipped(false);
                            }

                        }
                    }
                }
            }*/
            PlayerD playerDataTmp = new PlayerD(_id.name, idPart.classItem);


            //CLog.Log("REMOVE VALE: " + remove + " " + (idLastPart!=null?idLastPart[idPart.classItem]:" ") + " " + idPart);

            if (KartStore)
            {
                spotlight.kart_Store.changePart(ResourceManager.Instance.getKart(idKart), playerDataTmp, false);// Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), _id.name).ItemClass, _id.name);
            }
            else
            {
                spotlight.char_Store.changeCharPart(ResourceManager.Instance.getChar(idChar), playerDataTmp, remove);// Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), _id.name).ItemClass, _id.name);
                if (remove) refreshParts();
            }
            partsTmpAdds.Add(idPart);

            //CLog.Log("MOSTRAR LA PIEZA APLICADA");
        }

        if (idPart.classItem == ClassPart.EXHAUST)
        {
            if (idPart.tagItem != null)
            {
                AudioClip ac = Resources.Load<AudioClip>("Prefabs/KartPrefabs/Accesorios/Exhaust_Sfx/" + idPart.tagItem + "/Demo");
                if (ac)
                {
                    demoExhasut.clip = ac;
                    demoExhasut.Play();
                }
            }
        }


        CLog.Log("ESTA EQUIPADO " + idPart.nameItem + " " + idPart.name + " " + idPart.isEquipped);
        if (panelParts.activeSelf)
        {
            foreach (Transform t in _id.transform.parent)
            {
                t.GetComponent<ItemStore>().setSelect(t.gameObject == _id && (!remove || lastClassBtn.GetComponent<ClassMenuItem>().isCharForce));
                if (remove) t.GetComponent<ItemStore>().setSelect(t.GetComponent<ItemStore>().isEquipped);
            }

            if (idPart.isLocked)
            {
                if (!remove) btnBuyActive(true);
                txtBtnBuy.text = TranslateUI.getStringUI(UI_CODE.KRT_STS_BUY);//COMPRAR
            }
            else if (!idPart.isEquipped)
            {
                btnBuyActive(idPart.isSelect);
                txtBtnBuy.text = TranslateUI.getStringUI(UI_CODE.KRT_STS_EQUIP);//EQUIPAR
            }
            else
            {
                btnBuyActive(false);
                txtBtnBuy.text = TranslateUI.getStringUI(UI_CODE.KRT_STS_EQUIP); //EQUIPAR
            }
            //btnBuyActive();

            if (KartStore)
                statsPreviewKart(idPart);
            else CLog.Log("Actualizar el texto Stats statsPreviewKart");
        }
        else
        {
            switchPanel(false);
            filterParts(idPart.classItem);
        }

        if (idPart.isEquipped) idLastPart[idPart.classItem] = idPart;
        if (remove) idLastPart.Remove(idPart.classItem);

    }

    void btnBuyActive(bool _state)
    {
        CLog.Log("ESTADO " + _state);
        btnBuy.interactable = _state;
        btnAccesorios.interactable = !btnBuy.interactable|| !panelKarts.activeSelf;
        //btnBuy.gameObject.SetActive(btnBuy.interactable);
    }

    //Vector2 partsLow = new Vector2(140, 160), partsHight = new Vector2(200, 200);
    //Vector2 kartsLow = new Vector2(200, 200), kartsHight = new Vector2(230, 230);
    Vector2 partsLow = new Vector2(100, 100), partsHight = new Vector2(100, 100);
    Vector2 kartsLow = new Vector2(100, 100), kartsHight = new Vector2(100, 100);
    /// <summary>
    /// Actualiza el tamaño de las listas de botones seleccionados de Karts y de Clases de Accesorios
    /// </summary>
    /// <param name="_btn"></param>
    /// <param name="_low"></param>
    /// <param name="_hight"></param>
    public void updateBtn(GameObject _btn, Vector2 _low, Vector2 _hight)
    {
        if (_btn.transform.parent.GetComponent<UnityEngine.UI.GridLayoutGroup>() != null)
        {
            _btn.transform.parent.GetComponent<UnityEngine.UI.GridLayoutGroup>().enabled = false;
        }
        else
        {
            _btn.transform.parent.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>().enabled = false;
        }

        //_btn.transform.parent.GetComponent<UnityEngine.UI.GridLayoutGroup>().enabled = true;

        foreach (Transform t in _btn.transform.parent)
        {

            if (_btn.transform == t)
            {
                t.GetComponent<RectTransform>().sizeDelta = _hight;
                //if(t.GetComponent<ItemStore>()) t.GetComponent<ItemStore>().selectImg.SetActive(true);
            }
            else
            {
                t.GetComponent<RectTransform>().sizeDelta = _low;
                //if (t.GetComponent<ItemStore>()) t.GetComponent<ItemStore>().selectImg.SetActive(false);
            }
        }
        if (_btn.transform.parent.GetComponent<UnityEngine.UI.GridLayoutGroup>() != null)
        {
            _btn.transform.parent.GetComponent<UnityEngine.UI.GridLayoutGroup>().enabled = true;
        }
        else
        {
            _btn.transform.parent.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>().enabled = true;
        }
        //_btn.transform.parent.GetComponent<UnityEngine.UI.GridLayoutGroup>().enabled = true;
        //_btn.transform.parent.GetComponent<UnityEngine.UI.GridLayoutGroup>().enabled = true;
    }
        

    public void switchPanel()
    {
        switchPanel(true);
        refreshParts();
    }
    public void switchPanel(bool _loadFirstClass)
    {
        SpinStore.instance.setAngles(ClassPart.NONE);

        if (panelKarts.activeSelf)
        {

            //            if(btnBuyActive()
            {
                //CLog.Log("No ")
            }
        }
        //ACTUALIZO LOS BOTONES DE CLASES
        foreach (Transform _classBtn in panelClassParts)
        {
            _classBtn.GetComponent<ClassMenuItem>().setIcon(ResourceManager.Instance.getIcon((KartStore ? _classBtn.GetComponent<ClassMenuItem>().kartStore : _classBtn.GetComponent<ClassMenuItem>().charStore)));        //    if (_kart.name.Equals(_id))

        }
        //panelKarts.SetActive(!panelKarts.activeSelf);
        panelParts.SetActive(!panelParts.activeSelf);
        setButonAccesorios();
        if (panelParts.activeSelf)
        {
            if (_loadFirstClass)
                filterParts(KartStore ? ClassPart.MOTOR : ClassPart.BODY);// ClassPart.MOTOR.ToString());            
        }
        else
        {
            if (KartStore) loadPartsKarts(true);
            else loadPartsChars(true);
        }
        btnBuyActive(false);



        //if (panelBotones.gameObject.activeSelf) ;

    }

    //Actualiza la lista de configuracion del Auto
    public bool updateConfig(ItemStore _idPart, bool _add)
    {
        try
        {
            int idTMP = idChar;
            if (KartStore)
                idTMP = idKart;


            for (int i = 0; i < PlayerDataTitle.PlayerDataTi[idTMP.ToString()].Count; i++)
            {
                //CLog.Log("COMPARO: " + _part.Id + " con: " + idPart.name);
                if (PlayerDataTitle.PlayerDataTi[idTMP.ToString()][i].ClassPart.Equals(_idPart.classItem))
                {

                    //CLog.Log("Encontre para reemplazar: " + PlayerDataTitle.PlayerDataTi[idTMP.ToString()][i].Id + " - " + _idPart.name);

                    PlayerDataTitle.PlayerDataTi[idTMP.ToString()].Remove(PlayerDataTitle.PlayerDataTi[idTMP.ToString()][i]);
                    break;
                }

            }
            if (_add) PlayerDataTitle.PlayerDataTi[idTMP.ToString()].Add(new PlayerD(_idPart.name, _idPart.classItem));
            return true;
        }
        catch (System.Exception e)
        {
            CLog.LogError("EXCEPTION: " + e);
            return false;
        }

    }

    //Apretar el boton Back confirmo el auto elegido
    public void back()
    {
        asignarID();

        
        if (ClientInfo.KartId != idKart)//Si el Kart seleccionado no esta comparado regresa al ultimo seleccionado comprado
        {
            selectDirectKart(ClientInfo.KartId.ToString());
        }
        if (RoomPlayer.Local != null)
        {
            RoomPlayer.Local.RPC_SetKartId(ClientInfo.KartId, ClientInfo.CharId); //Envio el ID a la instancia de este player en el sevidor, debo enviar el ID de Kart y Bajar su config desde el PlayerData
        }

        if (!KartStore)
        {
            SpinStore.instance.setCamera(true);
            //        spotlight.FocusIndex(ClientInfo.KartId);
        }
        else
        {
            spotlight.viewChar = true;
            if (spotlight.char_Store2) spotlight.char_Store2.gameObject.SetActive(true);

        }
        spotlight.FocusIndex(ClientInfo.KartId,true);
         
        resetCamera();//Invoke("resetCamera", 1);

        //refreshParts();
    }
    private void OnDisable()
    {
        if (spotlight)
            spotlight.cleanPool();
        if (LittlePopUpManager.instance)
            LittlePopUpManager.instance.DesactivePopupWithoutTime();
    }
    public void resetCamera()
    {
        SpinStore.instance.setAngles(ClassPart.ALL);
    }

    Coroutine buyItemCoroutine;
    public void buyItem()
    {
        if (buyItemCoroutine == null)
            buyItemCoroutine = StartCoroutine(buyItemStore());
    }
    IEnumerator buyItemStore()
    {
        btnBuyActive(false);
        if (panelKarts.activeSelf)//Compra de Karts usando TEL
        {
            if (KartStore)
            {
                if (int.Parse(kartItemStoreSelectTMP.priceValue.text) > PlayfabManager.instance.getTEL())
                {
                    GameLauncher.instance.sinFondos();
                    buyItemCoroutine = null;
                    yield break;
                }
                //PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_WARNING),"Comprar el " + kartItemStoreSelectTMP.displayname + " por " + kartItemStoreSelectTMP.priceValue.text, IconosPopUp.questioin, true);
                PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_WARNING), TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_BUYITEM).Replace("XXX", kartItemStoreSelectTMP.displayname).Replace("YYY", kartItemStoreSelectTMP.priceValue.text)+" TEL", IconosPopUp.questioin, true);
                yield return new WaitWhile(() => PopUpManager._instance.popUpState == PopUpStates.Wait);

                if (PopUpManager._instance.popUpState == PopUpStates.Ok)
                {
                    //LittlePopUpManager.instance.setSmallPopUp("Realizando operacion");//xxxt-
                    LittlePopUpManager.instance.setSmallPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_BUY));

                    //idKart //Kart a comprar
                    Busines.buyItem(Catalogos.Karts.ToString(), idKart.ToString(), int.Parse(kartItemStoreSelectTMP.priceValue.text), Currencys.TE.ToString(), Stores.Karts.ToString(), idKart.ToString(), Catalogo.getItem(Catalogos.Karts.ToString(), kartItemStoreSelectTMP.idkart.ToString()));
                    yield return new WaitForSeconds(.1f);
                    yield return new WaitWhile(() => DataEconomy.BuyStatus == BuyStatus.BUYING);
                    if (DataEconomy.BuyStatus == BuyStatus.ERROR)
                    {
                        CLog.Log("ERROR EN LA COMPRA DEL KART " + kartItemStoreSelectTMP.priceValue.text);
                        //PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_ERROR), TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_ERRORBUY).Replace("XXX", kartItemStoreSelectTMP.displayname).Replace("YYY", kartItemStoreSelectTMP.priceValue.text), IconosPopUp.error, false);
                        LittlePopUpManager.instance.setSmallPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_ERRORBUY).Replace("XXX", kartItemStoreSelectTMP.displayname).Replace("YYY", kartItemStoreSelectTMP.priceValue.text)+ " TEL");
                        buyItemCoroutine = null;
                        yield break;
                    }
                    else //Compra exitosa
                    {
                        Busines.AddExpertice(new List<string> { PlayfabManager.instance.IdPlayFab },
                                                new List<int> { (int)Catalogo.getItem(Catalogos.Karts.ToString(), kartItemStoreSelectTMP.idkart.ToString()).getCustomData(CustomDataItem.Xp) }, 0);
                        //Actualizao el Inventario con la nueva compra
                        kartItemStoreSelectTMP.locked(false);
                        Inventory.importInventory();
                        yield return new WaitForSeconds(.1f);
                        yield return new WaitWhile(() => DataEconomy.ECONOMYSTATUS == EconomyStatus.DOWNLOADING);

                        selectDirectKart(kartItemStoreSelectTMP.name);

                        PurshaseItem.instance.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                        PurshaseItem.instance.setPrefab(int.Parse(kartItemStoreSelectTMP.name), kartItemStoreSelectTMP.displayname, false);
                    }
                }

            }
            else
            {                //idChar //Char a comprar

                if (int.Parse(charItemStoreSelectTMP.priceValue.text) > PlayfabManager.instance.getTEL())
                {
                    GameLauncher.instance.sinFondos();
                    buyItemCoroutine = null;
                    yield break;
                }

                PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_WARNING), TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_BUYITEM).Replace("XXX", charItemStoreSelectTMP.displayname).Replace("YYY", charItemStoreSelectTMP.priceValue.text)+" TEL", IconosPopUp.questioin, true);
                yield return new WaitWhile(() => PopUpManager._instance.popUpState == PopUpStates.Wait);

                if (PopUpManager._instance.popUpState == PopUpStates.Ok)
                {
                    //LittlePopUpManager.instance.setSmallPopUp("Realizando operacion");//xxxt-
                    LittlePopUpManager.instance.setSmallPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_BUY));//xxxt-

                    //Prueba anim
                    /*
                    Transform lista2 = GameObject.Find("KartHolder").transform;
                    Char_Store character2 = lista2.Find(idChar.ToString()).GetComponent<Char_Store>();
                    character2.animator.CrossFade("Buy",.3f);
                    btnBuyActive(true);
                    buyItemCoroutine = null;
                    yield break;
                    */

                    Busines.buyItem(Catalogos.Characters.ToString(), idChar.ToString(), int.Parse(charItemStoreSelectTMP.priceValue.text), Currencys.TE.ToString(), Stores.Mamiferos.ToString(), idChar.ToString(), Catalogo.getItem(Catalogos.Characters.ToString(), charItemStoreSelectTMP.name.ToString()));
                    
                    yield return new WaitForSeconds(.1f);
                    yield return new WaitWhile(() => DataEconomy.BuyStatus == BuyStatus.BUYING);
                    if (DataEconomy.BuyStatus == BuyStatus.ERROR)
                    {
                        //PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_ERROR), TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_ERRORBUY).Replace("XXX", charItemStoreSelectTMP.displayname).Replace("YYY", charItemStoreSelectTMP.priceValue.text), IconosPopUp.error, false);
                        LittlePopUpManager.instance.setSmallPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_ERRORBUY).Replace("XXX", charItemStoreSelectTMP.displayname).Replace("YYY", charItemStoreSelectTMP.priceValue.text)+ " TEL");
                        CLog.Log("ERROR EN LA COMPRA DEL CHAR " + charItemStoreSelectTMP.priceValue.text);
                        buyItemCoroutine = null;
                        yield break;
                    }
                    else //Compra exitosa
                    {
                        Busines.AddExpertice(new List<string> { PlayfabManager.instance.IdPlayFab },
                                                new List<int> { (int)Catalogo.getItem(Catalogos.Characters.ToString(), charItemStoreSelectTMP.name.ToString()).getCustomData(CustomDataItem.Xp) }, 0);
                        //Actualizao el Inventario con la nueva compra
                        charItemStoreSelectTMP.locked(false);
                        Inventory.importInventory();
                        yield return new WaitForSeconds(.1f);
                        yield return new WaitWhile(() => DataEconomy.ECONOMYSTATUS == EconomyStatus.DOWNLOADING);

                        selectDirectKart(charItemStoreSelectTMP.name);
                        //Aqui Va la animacion
                        Transform lista = GameObject.Find("KartHolder").transform;
                        Char_Store character = lista.Find(idChar.ToString()).GetComponent<Char_Store>();
                        character.animator.CrossFade("Buy", .3f);
                        PurshaseItem.instance.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                        PurshaseItem.instance.setPrefab(int.Parse(charItemStoreSelectTMP.name),charItemStoreSelectTMP.displayname,true);
                    }

                }
            }
        }
        else  //Compra o equipar Upgrades usando TNL
        {

            if (idPart.isLocked)//Rutina de compra de Parte
            {
                //Comprar Producto

                //---- if()
                if (int.Parse(idPart.priceValue.text) > PlayfabManager.instance.getTLN())
                {
                    GameLauncher.instance.sinFondos();
                }
                else if (idPart.level <= PlayfabManager.instance.getLevel())
                {

                    //LittlePopUpManager.instance.setSmallPopUp("Realizando operacion");//xxxt-
                    LittlePopUpManager.instance.setSmallPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_BUY));//xxxt-

                    if (KartStore)
                        Busines.buyItem(Catalogos.Parts_Upgrade.ToString(), idPart.name, int.Parse(idPart.priceValue.text), Currencys.NL.ToString(), Stores.KartsParts.ToString(), idKart.ToString(), Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), idPart.name));
                    else
                        Busines.buyItem(Catalogos.Character_Upgrade.ToString(), idPart.name, int.Parse(idPart.priceValue.text), Currencys.NL.ToString(), Stores.CharParts.ToString(), idChar.ToString(), Catalogo.getItem(Catalogos.Character_Upgrade.ToString(), idPart.name));

                    yield return new WaitForSeconds(.1f);
                    yield return new WaitWhile(() => DataEconomy.BuyStatus == BuyStatus.BUYING);

                    //Resultado de la compra

                    if (DataEconomy.BuyStatus == BuyStatus.ERROR)
                    {
                        //PopUpManager._instance.setPopUp("ERROR", "Error en la compra de: " + idPart.name + " por " + idPart.priceValue.text+"\n"+ PlayfabClientCurrency.Error, idPart.icon, false);
                        CLog.Log("ERROR EN LA COMPRA");
                        LittlePopUpManager.instance.setSmallPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_ERRORBUY).Replace("XXX", idPart.displayname).Replace("YYY", idPart.priceValue.text) + " TNL");

                        LittlePopUpManager.instance.setSmallPopUp("ERROR EN LA COMPRA");//xxxt-

                        buyItemCoroutine = null;
                        yield break;
                    }
                    else //Compra exitosa
                    {
                        //Actualizao el Inventario con la nueva compra

                        //Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), idPart.name).getCustomData(CustomDataItem.Xp);

                        CLog.Log("AD XP: " + idPart.name);

                        CLog.Log("AD XP: " + Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), idPart.name));

                        Busines.AddExpertice(new List<string> { PlayfabManager.instance.IdPlayFab },
                                            new List<int> { (int)Catalogo.getItem(KartStore ? Catalogos.Parts_Upgrade.ToString() : Catalogos.Character_Upgrade.ToString(), idPart.name).getCustomData(CustomDataItem.Xp) }, 0);



                        Inventory.importInventory();
                        yield return new WaitForSeconds(.1f);
                        yield return new WaitWhile(() => DataEconomy.ECONOMYSTATUS == EconomyStatus.DOWNLOADING);

                        //Guardo la nueva config con la mejora comparada equipada
                        idPart.locked(!updateConfig(idPart, true));

                        if (KartStore) statsPreviewKart(null);
                        filterParts(lastClassBtn);

                        yield return StartCoroutine(updateConfigPlayFab());
                        CLog.Log("FINALIZO LA COROUTINA");
                    }

                }
                else
                {
                    PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_ERROR), TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_NEEDLEVEL).Replace("XXX", idPart.level.ToString()), IconosPopUp.error, false);

                }
            }//Si la parte esta comprada y se quiere equipar
            else
            {
                if (!idPart.isEquipped)
                {

                    idPart.setEquipped(updateConfig(idPart, true));


                    if (!KartStore)
                    {
                        if (idPart.classItem == ClassPart.HELMET)
                        {
                            ItemStore t = new ItemStore();
                            t.classItem = ClassPart.FACE;
                            updateConfig(t, false);
                            t.classItem = ClassPart.GLASSES;
                            updateConfig(t, false);
                            t.classItem = ClassPart.HAT;
                            updateConfig(t, false);
                        }
                        else if (idPart.classItem == ClassPart.HAT || idPart.classItem == ClassPart.GLASSES || idPart.classItem == ClassPart.FACE)
                        {
                            ItemStore t = new ItemStore();
                            t.classItem = ClassPart.HELMET;
                            updateConfig(t, false);
                        }


                    }
                    else
                    {
                        statsPreviewKart(idPart);
                    }

                    filterParts(lastClassBtn);

                    yield return StartCoroutine(updateConfigPlayFab());

                }
            }
        }
        buyItemCoroutine = null;
    }

    public void ResetPanel()
    {
        panelKarts.SetActive(true);
    }
    public void changePanel()
    {
        panelKarts.SetActive(!panelKarts.activeSelf);
        panelParts.GetComponent<UnityEngine.UI.LayoutElement>().ignoreLayout = false;
    }
    public void restorePanel()
    {
        panelKarts.SetActive(true);
        panelParts.GetComponent<UnityEngine.UI.LayoutElement>().ignoreLayout = true;
    }
    IEnumerator updateConfigPlayFab()
    {

        yield return StartCoroutine(PlayerDataTitle.updateConfigPlayFab(KartStore ? idKart.ToString() : idChar.ToString()));

        /* PlayerDataTitle.PlayerDataTi.TryGetValue(KartStore ? idKart.ToString() : idChar.ToString(), out List<PlayerD> playerData);

         for (int i = 0; i < 2; i++)//en caso de fallar el guardado de config del auto intento de nuevo 
         {
             PlayerDataTitle.updateData(KartStore ? idKart.ToString() : idChar.ToString(), playerData);
             yield return new WaitForSeconds(.1f);
             yield return new WaitWhile(() => DataEconomy.updateDataStatus == UpdateDataStatus.UPLOADING);
             if (DataEconomy.updateDataStatus == UpdateDataStatus.OK) break;
         }*/

    }

    public void asignarID()
    {
        if (KartStore)
        {
            //CLog.Log("ASIGNO IN: " + ClientInfo.CharId + " " + ClientInfo.KartId+" "+ kartItemSelect);
            if (kartItemSelect) ClientInfo.KartId = kartItemSelect.idkart;
        }
        else
        {
            if (charItemSelect) ClientInfo.CharId = charItemSelect.idkart;
            //CLog.Log("ASIGNO OUT: " + ClientInfo.CharId + " " + ClientInfo.KartId+" "+ kartItemSelect);

        }
    }

    private void OnDestroy()
    {

        asignarID();
    }



}
