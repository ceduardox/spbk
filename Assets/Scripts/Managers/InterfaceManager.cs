using FusionExamples.Utility;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.IO;

public class InterfaceManager : MonoBehaviour
{
	//public static InterfaceManager instance;

	//[SerializeField] private ProfileSetupUI profileSetup;

	public UIScreen mainMenu;
	public UIScreen pauseMenu;
	public GameObject Displayname;
	public TMP_Dropdown QualityGraphics;
    public bool isPC=true;
	public static InterfaceManager Instance => Singleton<InterfaceManager>.Instance;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            isPC = false;
        }
    }
    public void enableDisplayData(bool showDisplayName)
    {
        if (showDisplayName)
        {
            if (GameLauncher.ConnectionStatus == ConnectionStatus.Connected) 
            {
                Displayname.SetActive(false);
            }
            else
            {
                Displayname.SetActive(true);
            }
        }
        else { Displayname.SetActive(false); }
    }
	public void OpenPauseMenu()
	{
		// open pause menu only if the kart can drive and the menu isn't open already
		if (UIScreen.activeScreen != pauseMenu)
		{
			UIScreen.Focus(pauseMenu);
		}
	}

	public void Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
		Application.Quit();
	}

	public void onlogin()
    {
		//DataEconomy.getEconomy();//trae catalogo store, inventario del player, playerdata


		//playerdatatitle
		//PlayerDataTitle.getPlayerData();//trae data player de playfab
		//PlayerDataTitle.PlayerDataTi["karts_01"];//trae la lista del id especificado
		//PlayerDataTitle.updateData("Karts_01",//lista)//actualiza o sube data del player


		//inventory
	/*	Inventory.importInventory();//trae el inventario del jugador y lo almacena en una lista

      /*  foreach (var item in Inventory.Inventario)
        {
			CLog.Log(item.CustomData);
        } ;//trae la lista de items*/


		

		//Store.Stores["Kart"].Store[0].;//trae el store pasando clave

		//catalogo
		//Catalogo.SelectCatalog[""].Catalog[0].//trae el catalogo pasado clave


	}

	public void setImage(string url, Image component)
    {
        //StartCoroutine(LoadImage(url, component));
        CLog.Log("ESTO TRAJE: " + url);
        if (string.IsNullOrEmpty(url)) return;
        url = url.Replace("http://linturismochina.com/wp-content/uploads", "");
        url = url.Replace("/", "");
        url = url.Replace(".png", "");
		url = url.Replace(".jpg", "");
		foreach (var img in ResourceManager.instance.ImgAvatar)
        {
            if (url == img.name)
            {
				component.sprite = img;
            }
        }
    }

	public void setImageUrl(string url, Image component)
	{
		StartCoroutine(LoadImage(url, component));
		//StartCoroutine(LoadImageFromURL(url, component));
	}
    //public IEnumerator LoadImageFromURL(string url, Image img)
    //{
    //	
    //	www = UnityWebRequestTexture.GetTexture(url);
    //	yield return www.SendWebRequest();
    //	Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
    //	Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());
    //	img.sprite = sprite;
    //	if (www.isNetworkError || www.isHttpError)
    //	{
    //		CLog.LogError(www.error);
    //	}
    //	else
    //	{
    //		//Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
    //		//Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());
    //		//img.sprite = sprite;
    //	}
    //}
    IEnumerator LoadImage(string MediaUrl, Image img)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isDone)
        {
            Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
            img.overrideSprite = sprite;
        }
        else
        {
            Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
            img.overrideSprite = sprite;
        }

    }
    //public void LoadRawImage(string url, RawImage component)
    //{
    //	StartCoroutine(LoadRawImage(url, component));
    //}
    //IEnumerator LoadRawImage(string MediaUrl, RawImage img)
    //{
    //	UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
    //	yield return request.SendWebRequest();

    //	Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
    //	Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
    //	img.GetComponent<RawImage>().overrideSprite = sprite;

    //}
    public void DropdownQualityValueChanged()
	{
		QualitySettings.SetQualityLevel(QualityGraphics.value);

		GamePlayerPrefs.instance.saveGraphicsEnum(QualityGraphics.value);
	}
	// Audio Hooks
	public void SetVolumeMaster(float value) => AudioManager.SetVolumeMaster(value);
	public void SetVolumeSFX(float value) => AudioManager.SetVolumeSFX(value);
	public void SetVolumeUI(float value) => AudioManager.SetVolumeUI(value);
	public void SetVolumeMusic(float value) => AudioManager.SetVolumeMusic(value);
}