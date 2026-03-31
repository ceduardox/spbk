using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KartSelectUI : MonoBehaviour
{

	public Transform contenedorKarts;
	
	public Image speedStatBar;
	public Image accelStatBar;
	public Image turnStatBar;	

	public GameObject footer, kartSelectionImage, kartSelectionStandart;

	public StatsBars statsBar;

	private void Start()
	{

		GameObject buttonTMP=null;
		bool primero = true;

		foreach (Transform t in contenedorKarts)
		{
			if (primero)
				primero = false;
			else
			{
				Destroy(t.gameObject);
			}
		}

		primero = true;
		foreach(KartDefinition kartDefinition in ResourceManager.Instance.kartDefinitions)
		{

			//Cargo los botones referenciados a las definiciones de los Karts

				if (primero) buttonTMP = contenedorKarts.GetChild(0).gameObject; ////////////////////////////////////////////////////////Al primer item lo salto y para los siguientes los clono
				else buttonTMP = Instantiate(contenedorKarts.GetChild(0).gameObject, contenedorKarts);
				primero = false;
				buttonTMP.name = kartDefinition.Id + "";

				////////////////////////////////////////////////////en esta parte va la funcion en la que llamamos al inventario del player para comparar el Id del auto, "kartDefinition.Id", con 
				///////////////////////////////////////////////////el inventario y si no está, se bloque al boton.

				buttonTMP.GetComponent<buttonState>().setState(true);
				buttonTMP.GetComponent<buttonState>().setIcon(kartDefinition.itemIcon);

		} 

		buttonTMP.GetComponent<buttonState>().setState(false);//////////////Bloque el ultimo boton
	}

    private void OnEnable() {		
		//SelectKart(ClientInfo.KartId);
		showButtonAndKart(false);
		kartSelectionStandart.SetActive(true);
		
	}

	/// <summary>
	/// Presionar el boton de selccionar un Kart, si esta bloquedo pedire confirmacion para comprarlo, si ya está comprado, lo selecciona
	/// </summary>
	/// <param name="kartIndex"></param>
	public void SelectKart(GameObject kartIndex)
	{
		
			showButtonAndKart(true);
			//ClientInfo.KartId = int.Parse(kartIndex.name);

			CLog.Log("Envio el ID: " + ClientInfo.KartId);
			//if (SpotlightGroup.Search("Kart Display", out SpotlightGroup spotlight)) spotlight.FocusIndex(ClientInfo.KartId);
			SpotlightGroup._instance.FocusIndex(ClientInfo.KartId,true);
			ApplyStats();

			if (RoomPlayer.Local != null) {
				RoomPlayer.Local.RPC_SetKartId(ClientInfo.KartId, ClientInfo.CharId); //Envio el ID a la instancia de este player en el sevidor, debo enviar el ID de Kart y Bajar su config desde el PlayerData
			}



		if (kartIndex.GetComponent<buttonState>().isLocked)
		{
			CLog.Log("No dispones este auto, quieres comprarlo?");

			////////////////////////////////////////////////////////////////////////////////////Rutina para comprar el auto
        }
	}




	private void ApplyStats()
	{
		



		//KartDefinition def = ResourceManager.Instance.kartDefinitions[ClientInfo.KartId];
		KartDefinition def = ResourceManager.Instance.getKart(ClientInfo.KartId);

		speedStatBar.fillAmount = def.SpeedStat;
		accelStatBar.fillAmount = def.AccelStat;
		turnStatBar.fillAmount = def.TurnStat;
	}

	void showButtonAndKart(bool show)
	{						
		footer.SetActive(show);
		kartSelectionStandart.SetActive(false);
		kartSelectionImage.SetActive(show);
	}



}
