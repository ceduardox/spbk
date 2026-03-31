using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class ProfileImageAvatar : MonoBehaviour
{
    public static ProfileImageAvatar instance;
    [Header("SECCION IMAGE PROFILES")]
    [SerializeField] public Transform ContainerImageProfiles;
    [SerializeField] public GameObject objItemImageProfile;
    //public Dictionary<string, Texture> textureListImages = new Dictionary<string, Texture>();
    public Dictionary<string, Sprite> textureListImages = new Dictionary<string, Sprite>();
    [SerializeField] public Button btnProfileAvatar;
    [SerializeField] public string currentUrlImage;
    //[SerializeField] public RawImage imgAvatar;
    [SerializeField] public string[] ImageAvatar;
    [SerializeField] public Texture2D[] ImgAvatar;
    private string imageName;
    bool uploadedAvatars = false;
    void Start()
    {
        chargeItemsImages();
    }
    private void Awake()
    {
        //CLog.Log("NAME : " + name);
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
    public void changePlayfavAvatar()
    {
        if (currentUrlImage != "")
        {
            PlayfabManager.instance.UpdateAvatarUrl(currentUrlImage);
            //setAvatarImage(currentUrlImage);
            btnProfileAvatar.GetComponent<Button>().interactable = false;
        }
    }
    public void loadImageProfiles()
    {
        ImgAvatar = Resources.LoadAll("Images/avatar/", typeof(Texture2D)).Cast<Texture2D>().ToArray();
        //var textures = Resources.LoadAll("Images/avatar/", typeof(Texture2D)).Cast<Texture2D>().ToArray();

        foreach(var data in ResourceManager.instance.ImgAvatar)
        {
            if(!textureListImages.ContainsKey(data.name)) textureListImages.Add(data.name, data);
        }
        //for (int i = 0; i<=ImgAvatar.Length; i++)
        //{
        //    //textureListImages.Add(ImgAvatar[i].name, ImageAvatar[i]);
        //}

    }
    //public void setAvatarImage(string avatarImg)
    //{
    //    StartCoroutine(loadImage(avatarImg));
    //}

    //IEnumerator loadImage(string urlImage)
    //{
    //    WWW www = new WWW(urlImage);
    //    yield return www;
    //    if (!File.Exists(Application.persistentDataPath + imageName))
    //    {
    //        if (www.error == null)
    //        {
    //            Texture2D texture = www.texture;
    //            imgAvatar.GetComponent<RawImage>().texture = texture;
    //            byte[] dataByte = texture.EncodeToPNG();
    //            File.WriteAllBytes(Application.persistentDataPath + imageName + "png", dataByte);

    //        }
    //    }
    //    else
    //    if (File.Exists(Application.persistentDataPath + imageName))
    //    {
    //        byte[] uploadByte = File.ReadAllBytes(Application.persistentDataPath + imageName);
    //        Texture2D texture = new Texture2D(10, 10);
    //        texture.LoadImage(uploadByte);
    //        imgAvatar.GetComponent<RawImage>().texture = texture;
    //    }

    //}
    public void cleanUrlImageChoiced()
    {
        currentUrlImage = "";
        btnProfileAvatar.interactable = false;
    }
    public void chargeItemsImages()
    {
        if (!uploadedAvatars)
        {
            foreach (var sp in textureListImages)
            {
                ItemImageProfileUI itemTMP;
                GameObject ob = Instantiate(objItemImageProfile, this.transform.position, Quaternion.identity);
                ob.transform.SetParent(ContainerImageProfiles);
                itemTMP = ob.GetComponent<ItemImageProfileUI>();
                itemTMP.setImageAvatar(sp.Key, sp.Value);
            }
            uploadedAvatars = true;
        }
    }
    //public void loadImagesAvatars()
    //{
    //    //StopAllCoroutines();
    //    StartCoroutine(loadImages());
    //}
    //IEnumerator loadImages()
    //{
    //    foreach (var imgurl in ImageAvatar)
    //    {
    //        WWW www = new WWW(imgurl);
    //        yield return www;
    //        if (!File.Exists(Application.persistentDataPath + imageName))
    //        {
    //            if (www.error == null)
    //            {
    //                Texture2D texture = www.texture;
    //                textureListImages.Add(imgurl, texture);
    //                byte[] dataByte = texture.EncodeToPNG();
    //                File.WriteAllBytes(Application.persistentDataPath + imageName + "png", dataByte);

    //            }
    //        }
    //        else
    //        if (File.Exists(Application.persistentDataPath + imageName))
    //        {
    //            byte[] uploadByte = File.ReadAllBytes(Application.persistentDataPath + imageName);
    //            Texture2D texture = new Texture2D(10, 10);
    //            texture.LoadImage(uploadByte);
    //            textureListImages.Add(imgurl, texture);
    //        }
    //    }
    //}
}
