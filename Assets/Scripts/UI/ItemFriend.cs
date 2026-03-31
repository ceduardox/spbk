using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

public class FriendItemUI : MonoBehaviour, IDeselectHandler, ISelectHandler
{
    [SerializeField] public TextMeshProUGUI DisplayName;
    [SerializeField] public TextMeshProUGUI DisplayLvl;
    [SerializeField] public TextMeshProUGUI DisplayLastLogin;
    [SerializeField] public Image imgAvatar;
    private string imageName;
    //public string currentDisplayName;
    // Start is called before the first frame update
    void Start()
    {
        this.transform.localScale = new Vector3(1f, 1f, 1f);
        //DisplayLastLogin.enabled = false;
    }
    public void AcceptFriend()
    {
        //CLog.Log(DisplayName.text);
        FriendList.instance.acceptFriend(DisplayName.text);
        gameObject.SetActive(false);
    }
    public void DeclineFriend()
    {
        FriendList.instance.declineFriend(DisplayName.text);
        gameObject.SetActive(false);
    }
    public void chargeImage(string urlimg)
    {
        //StartCoroutine(loadImage(urlimg));
        InterfaceManager.Instance.setImage(urlimg,imgAvatar);
    }
    public void OnDeselect(BaseEventData data)
    {
        //DisplayLvl.enabled = true;
        //DisplayLastLogin.enabled = false;
    }
    public void OnSelect(BaseEventData data)
    {
        //DisplayLvl.enabled = true;
        //DisplayLastLogin.enabled = false;
    }
    public void btnSelected()
    {
        DisplayLvl.enabled = false;
        DisplayLastLogin.enabled = true;
    }
    //public IEnumerator loadImage(string urlImage)
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
}
