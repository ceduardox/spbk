using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharStoreUI : MonoBehaviour
{
  /*  public Transform panelBotones;
    /// <summary>
    /// Panel que contiene las partes de los autos
    /// </summary>
    public Transform panelBotonesPartes;
    private int idKart;
    public UnityEngine.UI.Button btnBuy;
    public TextMeshProUGUI txtBtnBuy;

    [SerializeField] bool KartStore;



    public GameObject panelChars;
    public GameObject panelParts;

    //public UnityEngine.UI.Button    btnBuy;
    public TextMeshProUGUI txtBtnAccesorios;

    ItemStore itTMP;
    // Start is called before the first frame update
    //private void OnEnable()
    private void OnEnable()
    {
        StartKartList();
    }
    public void StartKartList()
    {
        if (panelParts.activeSelf) switchPanel();

        for (int i = 1; i < panelBotones.childCount; i++)
        {
            Destroy(panelBotones.GetChild(i).gameObject);

        }


        if (KartStore)//GESTIONA LA STORE DE KARTS
        {
            GameObject buttonTMP;
            bool firstItem = true;
            foreach (KartDefinition _kd in ResourceManager.Instance.kartDefinitions)
            {
                if (firstItem) buttonTMP = panelBotones.GetChild(0).gameObject; ////////////////////////////////////////////////////////Al primer item lo salto y para los siguientes los clono
                else buttonTMP = Instantiate(panelBotones.GetChild(0).gameObject, panelBotones);
                (itTMP = buttonTMP.GetComponent<ItemStore>()).setIcon(_kd.itemIcon);
                buttonTMP.name = _kd.Id.ToString();
                itTMP.locked(true);
                foreach (var item in Inventory.Inventario)//busco el ID en el inventario
                {//
                    if (item.Id.Equals(_kd.Id.ToString()))
                    {
                        //CLog.Log("IMPRIMO: " + item.Id);
                        itTMP.locked(false);
                        break;
                    }
                }
                if (itTMP.isLocked)
                {
                    foreach (ProductBase itemStore in Store.getStore(Stores.Karts))
                    {
                        itTMP.setPrice(itemStore.PriceTE);
                    }
                }


                firstItem = false;


            }
        }
        else
        {
            GameObject buttonTMP;
            bool firstItem = true;
            foreach (DriverDefinition _kd in ResourceManager.Instance.driverDefinitions)
            {
                if (firstItem) buttonTMP = panelBotones.GetChild(0).gameObject; ////////////////////////////////////////////////////////Al primer item lo salto y para los siguientes los clono
                else buttonTMP = Instantiate(panelBotones.GetChild(0).gameObject, panelBotones);
                (itTMP = buttonTMP.GetComponent<ItemStore>()).setIcon(_kd.itemIcon);
                itTMP.locked(true);

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
                        itTMP.setPrice(itemStore.PriceTE);
                    }
                }

                buttonTMP.name = _kd.Id.ToString();
                firstItem = false;


            }
        }
        selectDirectKart(ClientInfo.KartId.ToString()); ;
        //selectKart()
    }



    public void selectDirectKart(string _id)
    {
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
        btnBuy.interactable = kartIndex.GetComponent<ItemStore>().isLocked;
        updateBtn(kartIndex, kartsLow, kartsHight);
        if (SpotlightGroup.Search("Kart Display", out SpotlightGroup spotlight)) spotlight.FocusIndex(idKart = int.Parse(kartIndex.name));
        loadPartsKarts(true);
        //statsKart(idKart);

        //apo
    }




    //public void statsKart(int _idKart)
    //{
    //    KartDefinition kd = ResourceManager.Instance.getKart(_idKart);
    //    ItemBase ib;//
    //    float speed = 0, acc = 0, turn = 0;
    //    PlayerDataTitle.PlayerDataTi.TryGetValue(idKart.ToString(), out List<PlayerD> playerData);//traigo el dataPlayerTitle

    //    if (playerData != null)
    //    {
    //        foreach (PlayerD _part in playerData)
    //        {
    //            ib = Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), _part.Id);
    //            speed += ib.getStats(CustomDataItem.Speed);
    //            acc += ib.getStats(CustomDataItem.Acceleration);
    //            turn += ib.getStats(CustomDataItem.Turn);

    //        }
    //    }


    //    CLog.Log("MANDO A RESTAURAR");
    //    ib = Catalogo.getItem(Catalogos.Karts.ToString(), _idKart.ToString());
    //    statsBar.setStats(ib.getStats(CustomDataItem.Speed), ib.getStats(CustomDataItem.Acceleration), ib.getStats(CustomDataItem.Turn));
    //    statsBar.setNewStats(speed, acc, turn);

    //    //ItemBase ib = Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), itemStore.Id.ToString());

    //}


    //public void statsPreviewKart(ItemStore _id)
    //{
    //    //if()

    //    //CLog.Log("MANDO A VER: " + _id + " " + _id.isEquipped);

    //    int mul = 1;
    //    if (_id == null)
    //    {
    //        statsBar.setPreviewStats(0, 0, 0);
    //        return;
    //    }
    //    else if (_id.isEquipped)
    //        mul = 0;

    //    ItemBase ib = Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), _id.name);
    //    statsBar.setPreviewStats(ib.getStats(CustomDataItem.Speed) * mul, ib.getStats(CustomDataItem.Acceleration) * mul, ib.getStats(CustomDataItem.Turn) * mul);

    //    statsKart(idKart);


    //}



    //Click Store Drivers
    public void selectDriver(GameObject kartIndex)
    {
        CLog.Log("CARGO DRIVER: " + kartIndex);
        if (SpotlightGroup.Search("Kart Display", out SpotlightGroup spotlight)) spotlight.FocusIndex(idKart = int.Parse(kartIndex.name));


        //statsBar.setStats()

        //loadPartsKarts();
        //apo
    }

    //Carga todas las partes del auto seleccionado
    public void loadPartsKarts(bool _eqquiped)
    {

        //int idKart;

        for (int i = 1; i < panelBotonesPartes.childCount; i++)
        {
            Destroy(panelBotonesPartes.GetChild(i).gameObject);
        }
        GameObject buttonTMP;
        bool firstItem = true;

        foreach (ProductBase itemStore in Store.getStore(Stores.KartsParts))
        {
            if (itemStore.Id.Contains(idKart.ToString()) ||   //Sí es un objeto esclusivo de este auto
                itemStore.Id.Contains("ALL_KARTS"))                 //Sí es un objeto golbal
            {

                if (firstItem) buttonTMP = panelBotonesPartes.GetChild(0).gameObject; ////////////////////////////////////////////////////////Al primer item lo salto y para los siguientes los clono
                else buttonTMP = Instantiate(panelBotonesPartes.GetChild(0).gameObject, panelBotonesPartes);
                firstItem = false;

                buttonTMP.name = itemStore.Id.ToString();

                //CLog.Log(Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), pb.Id).ItemClass);
                ItemBase ib = Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), itemStore.Id.ToString());

                (itTMP = buttonTMP.GetComponent<ItemStore>()).classItem = ib.ItemClass;
                itTMP.setSelect(false);

                if (itTMP.classItem == ClassPart.KART)
                    itTMP.setPrice(itemStore.PriceTE);
                else itTMP.setPrice(itemStore.PriceNL);

                Sprite sp = ResourceManager.Instance.getIcon(ib.ItemClass);
                //CLog.Log("ICONOVALE: " + sp.name+" "+ib.ItemClass);
                if (sp) itTMP.setIcon(sp);
                else itTMP.setIcon(ResourceManager.Instance.getIcon(ClassPart.ACCESORIES_KART));

                itTMP.idkart = idKart;

                itTMP.setNameDesc(Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), itemStore.Id).DisplayName, Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), itemStore.Id).Description);





                if (_eqquiped)
                {
                    PlayerDataTitle.PlayerDataTi.TryGetValue(idKart.ToString(), out List<PlayerD> playerData);//traigo el dataPlayerTitle
                    buttonTMP.SetActive(false);
                    foreach (PlayerD _part in playerData)
                    {

                        if (_part.Id.Equals(itTMP.name))
                        {
                            //CLog.Log("COMPARO: " + _part.Id + " con: " + itTMP.name);
                            itTMP.GetComponent<ItemStore>().setEquipped(true);
                            buttonTMP.SetActive(true);
                            break;
                        }
                    }
                }
                //else
                {
                    itTMP.locked(true);
                    foreach (PItemBase itemInventory in Inventory.Inventario)
                    {
                        if (itemInventory.Id.Equals(itemStore.Id))
                        {
                            itTMP.locked(false);
                            break;
                        }
                    }

                }


            }
        }
        //filterParts(panelBotones.GetChild(0).name);// ClassPart.MOTOR.ToString());
    }
    //quedan todos de azul

    //Muestra solo los objeto de la Clase seleccioanda
    public void filterParts(GameObject _btn)
    {
        lastClass = _btn;
        filterParts(_btn.name);
        updateBtn(_btn, partsLow, partsHight);
        //statsPreviewKart(null);
    }

    GameObject lastClass;
    public void filterParts(string _class)
    {

        btnBuy.interactable = false;
        PlayerDataTitle.PlayerDataTi.TryGetValue(idKart.ToString(), out List<PlayerD> playerData);//traigo el dataPlayerTitle

        foreach (Transform t in panelBotonesPartes)
        {

            if (!t.GetComponent<ItemStore>()) continue;

            t.gameObject.SetActive(t.GetComponent<ItemStore>().classItem.ToString().Contains(_class) || _class.Equals(ClassPart.ALL.ToString()));//Activo los items de acuerdo a la clase
            t.GetComponent<ItemStore>().setSelect(false);
            t.GetComponent<ItemStore>().setEquipped(false);
            // CLog.Log("Esto vale player data: " + playerData+" ID: "+ idKart);

            //CLog.Log("EL ID KART VALE: " +idKart);

            foreach (PlayerD _part in playerData)
            {
                //CLog.Log("COMPARO: " + _part.Id + " con: " + t.name);
                if (_part.Id.Equals(t.name))
                {
                    t.GetComponent<ItemStore>().setEquipped(true);
                    //CLog.Log("EL PANEL ESTA: " + panelParts.activeSelf);
                    if (panelParts.activeSelf) t.GetComponent<ItemStore>().setSelect(true);
                    t.SetAsFirstSibling();
                }
            }
        }
    }

    ItemStore idPart;
    public void previewPart(GameObject _id)
    {
        if (SpotlightGroup.Search("Kart Display", out SpotlightGroup spotlight))
        {
            SpinStore.instance.setAngles((idPart = _id.GetComponent<ItemStore>()).classItem);
            PlayerD playerData = new PlayerD(_id.name, idPart.classItem);
            spotlight.kart_Store.changePart(ResourceManager.Instance.getKart(idKart), playerData,false);// Catalogo.getItem(Catalogos.Parts_Upgrade.ToString(), _id.name).ItemClass, _id.name);
        }


        foreach (Transform t in _id.transform.parent)
        {
            t.GetComponent<ItemStore>().setSelect(t.gameObject == _id);
        }

        if (idPart.isLocked)
        {
            btnBuy.interactable = true;
            txtBtnBuy.text = "BUY";
        }
        else if (!idPart.isEquipped)
        {
            btnBuy.interactable = true;
            txtBtnBuy.text = "EQUIPPED";
        }
        else
        {
            idPart.isEquipped = false;
            txtBtnBuy.text = "EQUIPPED";
        }

        //statsPreviewKart(idPart);
    }



    Vector2 partsLow = new Vector2(140, 160), partsHight = new Vector2(200, 200);
    Vector2 kartsLow = new Vector2(200, 200), kartsHight = new Vector2(230, 230);
    /// <summary>
    /// Actualiza el tamaño de las listas de botones seleccionados de Karts y de Clases de Accesorios
    /// </summary>
    /// <param name="_btn"></param>
    /// <param name="_low"></param>
    /// <param name="_hight"></param>
    public void updateBtn(GameObject _btn, Vector2 _low, Vector2 _hight)
    {

        _btn.transform.parent.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>().enabled = false;

        foreach (Transform t in _btn.transform.parent)
        {

            if (_btn.transform == t)
            {
                t.GetComponent<RectTransform>().sizeDelta = _hight;
                if (t.GetComponent<ItemStore>()) t.GetComponent<ItemStore>().selectImg.SetActive(true);
            }
            else
            {
                t.GetComponent<RectTransform>().sizeDelta = _low;
                if (t.GetComponent<ItemStore>()) t.GetComponent<ItemStore>().selectImg.SetActive(false);
            }
        }
        _btn.transform.parent.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>().enabled = true;
    }

    public void switchPanel()
    {
        panelChars.SetActive(!panelChars.activeSelf);
        panelParts.SetActive(!panelParts.activeSelf);
        txtBtnAccesorios.text = panelChars.activeSelf ? "MEJORAS" : "KARTS";
     


        //if (panelBotones.gameObject.activeSelf) ;

    }


    //Actualiza la lista de configuracion del Auto
    public void updateConfig()
    {

        for (int i = 0; i < PlayerDataTitle.PlayerDataTi[idKart.ToString()].Count; i++)
        {
            //CLog.Log("COMPARO: " + _part.Id + " con: " + idPart.name);
            if (PlayerDataTitle.PlayerDataTi[idKart.ToString()][i].ClassPart.Equals(idPart.classItem))
            {
                CLog.Log("Encontre para reemplazar: " + PlayerDataTitle.PlayerDataTi[idKart.ToString()][i].Id + " - " + idPart.name);
                PlayerDataTitle.PlayerDataTi[idKart.ToString()].Remove(PlayerDataTitle.PlayerDataTi[idKart.ToString()][i]);
                PlayerDataTitle.PlayerDataTi[idKart.ToString()].Add(new PlayerD(idPart.name, idPart.classItem));
                break;


            }
        }

    }

    //Apretar el boton Back confirmo el auto elegido
    public void back()
    {
        ClientInfo.KartId = idKart;
        if (RoomPlayer.Local != null)
        {
            RoomPlayer.Local.RPC_SetKartId(ClientInfo.KartId); //Envio el ID a la instancia de este player en el sevidor, debo enviar el ID de Kart y Bajar su config desde el PlayerData
        }
    }




    public void buyItem()
    {
        StartCoroutine(buyItemStore());
    }




    IEnumerator buyItemStore()
    {
        btnBuy.interactable = false;

        if (panelChars.activeSelf)//Compra de Karts usando TEL
        {

            //idKart //Kart a comprar

        }
        else  //Compra o equipar Upgrades usando TNL
        {

            if (idPart.isLocked)//Rutina de compra de Parte
            {

                yield return new WaitForSeconds(2);//Despues de efectuar la compra y obtener compra valida:

            }
            //Actualizo la config dl auto y establezco los stats
            updateConfig();
            //statsKart(idKart);
            filterParts(lastClass);

            PlayerDataTitle.PlayerDataTi.TryGetValue(idKart.ToString(), out List<PlayerD> playerData);
            PlayerDataTitle.updateData(idKart.ToString(), playerData);

        }
    }

*/
}
