using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AddressableAssets;
//using UnityEngine.ResourceManagement.AsyncOperations;
//AsyncOperationHandle<GameObject> loadOp = Addressables.LoadAssetAsync<GameObject>(key);
//yield return loadOp;
public class SpotlightGroup : MonoBehaviour
{
    private static readonly Dictionary<string, SpotlightGroup> spotlights = new Dictionary<string, SpotlightGroup>();
	public bool selectKarts;
	public string searchName = "";
	public int defaultIndex = -1;
	public List<GameObject> objects;

    private GameObject focused = null;
	public Kart_Store kart_Store = null;
	public Char_Store char_Store = null;
	public Char_Store char_Store2 = null;

	int kartContadorUpload = 0;
	bool kartUpdatePlayerData = false;
	public static SpotlightGroup _instance;
	GameObject gameObjectTMP = null;
	Coroutine changeAllPartRoutine = null;
	IEnumerator loadKarts()
    {
		
		

		while(Catalogo.catalogoComplette!=0)
        {
			yield return new WaitForSeconds(.1f);
        }
		
		foreach (ItemBase _kart in Catalogo.getCatalogo(Catalogos.Karts))
		{

			//foreach (KartDefinition kd in ResourceManager.Instance.kartDefinitions)
			KartDefinition kd = ResourceManager.Instance.getKart(int.Parse(_kart.Id));
			if (kd)
			{
				//Cargo las definiciones y creo los autos prefab para la exibicion

				//kd = Resources.Load("Scriptable Objects/Kart Definitions/Kart_" + i++) as KartDefinition;

				if (kd.crearConfigInicial())
				{

					kartUpdatePlayerData = true;
					kartContadorUpload++;
					yield return new WaitWhile(() => PlayerDataTitle.contadorUpdates > 0 || DataEconomy.updateDataStatus == UpdateDataStatus.UPLOADING);


				}

				if(kd.Id==ClientInfo.KartId)
				{
					loadKart(kd);
				}

			}

		}
		

		foreach (DriverDefinition dd in ResourceManager.Instance.driverDefinitions)
		{
			if (dd.crearConfigInicial())
			{
				kartUpdatePlayerData = true;
				kartContadorUpload++;
				yield return new WaitWhile(() => PlayerDataTitle.contadorUpdates > 0 || DataEconomy.updateDataStatus == UpdateDataStatus.UPLOADING);
			}
			if(dd.Id==ClientInfo.CharId)
			{
				loadChar(dd);
			}
		}

		defaultIndex = ClientInfo.KartId;
		//de


	}


	public void loadKart(KartDefinition kd)
    {
		if (kd == null || kd.prefab == null)
		{
			CLog.LogError("No se puede cargar kart: definicion o prefab nulo.");
			return;
		}

		Debug.Log($"Log0: {kd}");
		Debug.Log($"Log1: {kd.prefab}");
        (gameObjectTMP = Instantiate(kd.prefab.gameObject, Vector3.zero, Quaternion.identity, transform)).SetActive(false);


		gameObjectTMP.transform.localPosition = new Vector3(-0.2f, 0, 0);//Vector3.zero;
		gameObjectTMP.transform.localRotation = new Quaternion(0, -.1f, 0, 1);//Quaternion.identity;
		gameObjectTMP.name = kd.Id + "";
		objects.Add(gameObjectTMP);

		foreach (var comp in gameObjectTMP.GetComponents<Component>())
		{
			if (!(comp is Transform || comp is Rigidbody || comp is Kart_Store)) 
			{
				Destroy(comp);
			}
		}

		setLayer(gameObjectTMP, 3);


		//CLog.Log("SOY: " + gameObjectTMP);				
		//gameObjectTMP.GetComponent<Kart_Store>().KartModel.transform.Find("Axle.B").parent = gameObjectTMP.GetComponent<Kart_Store>().KartModel.transform.Find("Axle.B").parent.parent.parent;
		//gameObjectTMP.GetComponent<Kart_Store>().KartModel.transform.Find("Axle.F").parent = gameObjectTMP.GetComponent<Kart_Store>().KartModel.transform.Find("Axle.F").parent.parent.parent;
		Kart_Store store = gameObjectTMP.GetComponent<Kart_Store>();
		if (store != null && store.driver != null && store.driver.childCount > 0)
			Destroy(store.driver.GetChild(0).gameObject);

		Destroy(gameObjectTMP.GetComponent<Fusion.NetworkRigidbody>());//.enabled = false;
		Destroy(gameObjectTMP.GetComponent<Rigidbody>());//.useGravity = false;
	}

	public void loadChar(DriverDefinition dd)
	{
		(gameObjectTMP = Instantiate(dd.prefab.gameObject, Vector3.zero, Quaternion.identity, transform)).SetActive(false);
		gameObjectTMP.transform.localPosition = Vector3.zero;
		gameObjectTMP.transform.localRotation = Quaternion.identity;
		gameObjectTMP.name = dd.Id + "";
		objects.Add(gameObjectTMP);

		/*
		foreach (var comp in gameObjectTMP.GetComponents<Component>())
		{
			if (!(comp is Transform || comp is Rigidbody || comp is Kart_Store))
			{
				Destroy(comp);
			}
		}*/

		setLayer(gameObjectTMP, 3);


		//Destroy(gameObjectTMP.GetComponent<Fusion.NetworkRigidbody>());//.enabled = false;
		//Destroy(gameObjectTMP.GetComponent<Rigidbody>());//.useGravity = false;

	}
	void setLayer(GameObject _gameObject, int _layer)
	{
		_gameObject.layer = _layer;
		foreach (Transform child in _gameObject.transform)
		{

			setLayer(child.gameObject, _layer);
		}

	}
	public static bool Search(string spotlightName, out SpotlightGroup spotlight)
	{
		return spotlights.TryGetValue(spotlightName, out spotlight);
	}

	private void OnEnable()
	{
		
		if (string.IsNullOrEmpty(searchName) == false)
		{
			spotlights.Add(searchName, this);
		}
		
	}

	private void OnDisable()
	{
		if (string.IsNullOrEmpty(searchName) == false)
		{
			spotlights.Remove(searchName);
		}
	}
    private void OnDestroy()
    {
		if (GameLauncher.instance)
			GameLauncher.instance.background.SetActive(false);

	}
	private void Awake()
    {
		if (selectKarts)
			_instance = this;
		if (GameLauncher.instance)
			GameLauncher.instance.background.SetActive(true);

	}

    public IEnumerator Start()//starFocus() 
	{

		//Screen.SetResolution(1280, 720, true);
		if (GameLauncher.instance.modeServerDedicado)
			yield break;
		

		if (selectKarts)
			yield return StartCoroutine(loadKarts());

		objects.ForEach((obj) => obj.SetActive(false));
		if (defaultIndex != -1)
		{
			FocusIndex(defaultIndex,true);
		}
		if(SpinStore.instance) SpinStore.instance.enabled = true;//activo la plataforma rotatoria
	}
	 
	public void FocusIndex(int index_ID, bool _kart)
	{
		
		if (focused) focused.SetActive(false);

		if (!(focused = getFocusedObject(index_ID)))
		{
			if (_kart)
			{
				Debug.Log("ID TRY LOAD: " + index_ID);
				KartDefinition kd = ResourceManager.Instance.getKart(index_ID);
				if (kd != null && kd.prefab != null)
					loadKart(kd);
				else
					CLog.LogError("Kart sin prefab o no encontrado para id: " + index_ID);
			}
			else loadChar(ResourceManager.Instance.getChar(index_ID));
		}
		//focused = objects[index];
		if (!(focused = getFocusedObject(index_ID)))
		{
			if (objects == null || objects.Count == 0)
			{
				CLog.LogError("No hay objetos en Spotlight para enfocar.");
				return;
			}
			focused = objects[0];
			index_ID = int.TryParse(focused.name, out int parsed) ? parsed : index_ID;
		}
				focused.SetActive(true);

		CLog.Log("ACTIVAR ID: " + index_ID + " " + focused);
		if (selectKarts)
		{
			if (changeAllPartRoutine != null)
				StopCoroutine(changeAllPartRoutine);
			changeAllPartRoutine = StartCoroutine(changeAllPart(index_ID, null));
		}
		//CLog.Log("PRENDI A " + focused + " "+ focused.GetComponent<Kart_Store>());


		//ResourceManager.Instance.getKart(ClientInfo.KartId);////////////////////////////Me devuelve la definicion del Kart
	}
	public void cleanPool()
    {
		if (!this || !isActiveAndEnabled)
			return;

		CancelInvoke("cleanPoolDelay");
		Invoke("cleanPoolDelay", 1);
	}
	public void cleanPoolDelay()
	{

		/*foreach (GameObject kart in objects)
		{
			if (!kart.activeSelf)
			{
				Destroy(kart);
			}
		}

		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i] == null)
			{
				objects.RemoveAt(i);
				i = 0;
			}
		}*/
	}
	Kart_Store lastKart_store;
	public bool viewChar=true;
	IEnumerator changeAllPart(int _index, Kart_Store _ks)
    {
				if (_ks)
					kart_Store = _ks;
				else
					kart_Store = focused ? focused.GetComponent<Kart_Store>() : null;

		if (kart_Store)
		{
			lastKart_store = kart_Store;
			ClientInfo.KartId = _index;
			if (kart_Store)// && DataEconomy.ECONOMYSTATUS == EconomyStatus.OK)
			{

				if (kartUpdatePlayerData)
				{
					kartUpdatePlayerData = false;
					int contador = 0;
					while (contador++ < 10)
					{

						CLog.Log("ESPERANDO UPDATE DE PLAYERDATATITTLE " + DataEconomy.updateDataStatus + " " + PlayerDataTitle.contadorUpdates);

						yield return new WaitWhile(() => PlayerDataTitle.contadorUpdates > 0);
						if (PlayerDataTitle.contadorUpdates == 0) break;
						yield return new WaitForSeconds(.5f);


					}

					//PlayerDataTitle.getPlayerData();
					contador = 0;
					while (contador++ < 10)
					{
						yield return new WaitWhile(() => DataEconomy.ECONOMYSTATUS != EconomyStatus.OK);

						yield return new WaitForSeconds(.5f);
						if (DataEconomy.ECONOMYSTATUS == EconomyStatus.OK)
							break;

						CLog.Log("ESPERANDO DESCARGA " + DataEconomy.ECONOMYSTATUS);


					}

				}
				yield return new WaitForSeconds(.5f);

				CLog.Log("TERMINADA LA DESCARGA " + DataEconomy.ECONOMYSTATUS);

					PlayerDataTitle.PlayerDataTi.TryGetValue(ClientInfo.KartId.ToString(), out List<PlayerD> playerData);



					//try
				{

						if (playerData != null)
						{
							foreach (PlayerD _part in playerData)
							{
								kart_Store.changePart(ResourceManager.Instance.getKart(ClientInfo.KartId), _part, false);
								//foreach (PlayerD pd in kart_Store.listUpgrade)
									//CLog.Log("Las partes son: " + pd.Id);
							}
						}




					if (viewChar)
					{
						if (char_Store2)
							DestroyImmediate(char_Store2.gameObject);

                        Debug.Log($"Try Get Char WITH ID:: {ClientInfo.CharId}");

	                        var obj = ResourceManager.Instance.getChar(ClientInfo.CharId);
							Debug.Log($"OBJ:: {obj}");
								if (obj == null || obj.prefab == null)
								{
									CLog.LogError("No se encontro prefab de personaje para id: " + ClientInfo.CharId);
									if (char_Store2) char_Store2.gameObject.SetActive(false);
									changeAllPartRoutine = null;
									yield break;
								}
							Debug.Log($"OBJ PREFAB:: {obj.prefab}");

                        char_Store2 = Instantiate(obj.prefab.gameObject).GetComponent<Char_Store>();// Resources.Load("Prefabs/Drivers/" + ResourceManager.Instance.getChar(int.Parse(_partID.Id)).name + "_Parts/" + _partID.Id.Replace(_kd.Id + "-", "")) as GameObject;

						if (char_Store2)
						{
							lastKart_store.setDriver(char_Store2.transform);
							loadChar(ClientInfo.CharId,char_Store2);
						}
					}
					if(char_Store2) char_Store2.gameObject.SetActive(viewChar);


				}
				//catch (System.Exception e)
				{

			//		CLog.Log("ERROR Index: " + _index + " - name: " + ClientInfo.CharId + " - Descripcion" + e);
				}

			}
			yield return new WaitForSeconds(.5f);
		}
			else
			{
				if (!focused)
				{
					CLog.LogError("Focus nulo al cambiar partes/char.");
					changeAllPartRoutine = null;
					yield break;
				}
				CLog.Log("ESTOY ACA: "+ focused.name);
				if ((char_Store = focused.GetComponent<Char_Store>()))//
				{
					loadChar(_index, char_Store);
				} 
			}

		DataEconomy.progreso++;
		changeAllPartRoutine = null;

	}

	private void loadChar(int _index, Char_Store char_Store)
    {
		PlayerDataTitle.PlayerDataTi.TryGetValue(_index.ToString(), out List<PlayerD> playerData2);

		if (char_Store == null)
			return;
		if (playerData2 == null)
			return;

		////.parent=kart_Store.driver
		GameObject focusedObj = getFocusedObject(_index);
		if (!focusedObj)
			return;
		Char_Store cs = focusedObj.GetComponent<Char_Store>();
		if (!cs)
			return;
		foreach (PlayerD _part in playerData2)
		{
			//CLog.Log("ESTOY APLICANDO E LCONTENDIDO " + _part);

			char_Store.changeCharPart(ResourceManager.Instance.getChar(_index), _part, false);
			cs.changeCharPart(ResourceManager.Instance.getChar(_index), _part, false);
		}
	}

	public void converConfig()
    {
		//return;
		string cadenaKart = "";
		string cadenaChar = "";
		string cadenaPU = "";

		PlayerDataTitle.PlayerDataTi.TryGetValue(ClientInfo.KartId.ToString(), out List<PlayerD> playerData);
		PlayerDataTitle.PlayerDataTi.TryGetValue(ClientInfo.CharId.ToString(), out List<PlayerD> playerData2);
		PlayerDataTitle.PlayerDataTi.TryGetValue(ClassPart.POWERUPS.ToString(),out List<PlayerD> playerDataPU);
		//PlayerDataTitle.PlayerDataTi.TryGetValue(ClientInfo.CharId.ToString(), out List<PlayerD> playerData2);

		/*
		  foreach (PlayerD pd in playerData)//kart_Store.listUpgrade)
			cadenaKart += pd.ClassPart + ":" + pd.Id + ":";//
		foreach (PlayerD pd in playerData2)//kart_Store.listUpgrade)
			cadenaChar+= pd.ClassPart + ":" + pd.Id + ":";//
		*/
		if(playerDataPU!=null)
		{

			foreach (PlayerD pd in playerDataPU)
			{
				//ItemBase item = Catalogo.getItem(Catalogos.PowersUps.ToString(), pd.Id);
				PItemBase item = Inventory.getItem(pd.Id);// Catalogo.getItem(Catalogos.PowersUps.ToString(), pd.Id);

				if (item != null)
				{
					int amount = PlayfabManager.instance.MaxPowerUps <= item.Amount ? PlayfabManager.instance.MaxPowerUps : item.Amount;
					cadenaPU += item.ItemClass.ToString() + ":" + item.InstanceId + ":" + amount+ ":";//+ item.;
				}
				else cadenaPU += ClassPart.NONE + ":0:0:";
			}
			
		}
		


		if (RoomPlayer.Local != null)
		{
			RoomPlayer.Local.RPC_sendCONFIG(cadenaKart, cadenaChar, cadenaPU);
		}

	}

	public GameObject getFocusedObject(int _index)
    {
		foreach(GameObject kart in objects)
        {			
				if (selectKarts)
				{
					int parsedId;
					if (int.TryParse(kart.name, out parsedId) && parsedId == _index)
						return kart;
				}
			else
            {
				return objects[_index];
            }
        }
		CLog.Log("ERROR ID NOT FOUND "+ _index+ " "+name);
		//return ResourceManager.Instance.kartDefinitions[0].Id;
		return null;
    }

	public Object getFocused(string _id)
    {
		foreach(Object kart in objects)
        {
			if (kart.name.Equals(_id))
				return kart;
        }
		return null;
    }

	public void Defocus()
	{
		if (focused) focused.SetActive(false);
		focused = null;
	}

	
}
