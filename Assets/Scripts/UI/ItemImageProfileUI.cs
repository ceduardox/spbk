using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemImageProfileUI : MonoBehaviour
{
    [SerializeField] public Image imgAvatar;
    //[SerializeField] public Image imgAvatar;
    [SerializeField] public string nameAvatar;
    
    void Start()
    {
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
    }
    public void setImageAvatar(string urlString, Sprite profileImg)
    {
        imgAvatar.GetComponent<Image>().sprite = profileImg;
        nameAvatar = urlString;
    }
    public void setUrlImageChoiced()
    {
        ProfileImageAvatar.instance.currentUrlImage = nameAvatar;
        ProfileImageAvatar.instance.btnProfileAvatar.GetComponent<Button>().interactable = true;   
    }
    
}
