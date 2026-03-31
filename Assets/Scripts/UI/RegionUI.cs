using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionUI : MonoBehaviour
{

	public UnityEngine.UI.Button connectBTN;
	public string curRegion = "";

    private void Awake()
    {
		if (TryGetComponent(out Dropdown dropdown))
		{

			// TODO: update options once we can request a list of regions
			string[] options = new string[] { "sa", "us", "eu", "asia" };

			dropdown.AddOptions(new List<string>(options));
			dropdown.onValueChanged.AddListener((index) =>
			{
				region(dropdown, index);




			});
            curRegion = Fusion.Photon.Realtime.PhotonAppSettings.Instance.AppSettings.FixedRegion;
            Debug.Log($"Initial region is {curRegion}");
            int curIndex = dropdown.options.FindIndex((op) => op.text == curRegion);
            if (curRegion != "" || curRegion != null)
            {


                connectBTN.interactable = true;

                dropdown.value = curIndex != -1 ? curIndex : 0; //dropdown.value = -1;
                                                                //StartCoroutine(checkRegion());
            }


        }
    }
	void OnEnable()
	{

		if (TryGetComponent(out Dropdown dropdown))
		{
			curRegion = Fusion.Photon.Realtime.PhotonAppSettings.Instance.AppSettings.FixedRegion;

			Debug.Log($"Initial region is {curRegion}");
			int curIndex = dropdown.options.FindIndex((op) => op.text == curRegion);
			if (curRegion != "" || curRegion != null)
			{


				connectBTN.interactable = true;

				dropdown.value = curIndex != -1 ? curIndex : 0; //dropdown.value = -1;
																//StartCoroutine(checkRegion());
			}
			else
			{
				LittlePopUpManager.instance.setSmallPopUpConfirm(TranslateUI.getStringUI(UI_CODE.POP_UP_SELECT_REGION));
			}
		}

	}


	void region(Dropdown dropdown, int index)
    {
		string region = dropdown.options[index].text;
		Fusion.Photon.Realtime.PhotonAppSettings.Instance.AppSettings.FixedRegion = region;
		Debug.Log($"+++Setting region to {region}");
		connectBTN.interactable = (curRegion != "" || curRegion != null);
	}


	System.Collections.IEnumerator checkRegion()
    {
		while (curRegion == "" || curRegion == null)
		{
			yield return new WaitForSeconds(.1f);

        }
		connectBTN.interactable = true;
    }




	public void ShutdownSession()
	{
		var nRunner = FindObjectOfType<NetworkRunner>();

        nRunner?.Shutdown();
	}
}