using System.Collections;
using UnityEngine;
using ArrayUtility;

public class ItemDisplayBridge : MonoBehaviour
{
	[SerializeField] private GameUI hud;

	public void PlaySpinTickSound()
	{
		AudioManager.Play("tickItemUI", AudioManager.MixerTarget.UI);
	}

	public void GetRandomIcon()//Muestra el icono 
	{
		//CLog.Log(ResourceManager.Instance.powerups.Length+" "+ ResourceManager.Instance.powerups[0].itemName+" "+ ResourceManager.Instance.powerups[1].itemName);
		hud.SetPickupDisplay(ResourceManager.Instance.powerups.RandomElement());
		//CLog.Log("PULSOOOOOOOOOOOOOO");
	}

}