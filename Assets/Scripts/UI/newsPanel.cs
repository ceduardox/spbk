using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using TMPro;
using System.IO;
using DG.Tweening;
using System.Globalization;
using System;

public class newsPanel : MonoBehaviour
{
    public static newsPanel instance;
    public Transform contentNews, contentMenu;
    //public RawImage imagePost;
    //public TextMeshProUGUI tittlePost;
    //public TextMeshProUGUI bodyPost;
    public GameObject objectPanel;
    public GameObject objectMenu;
    private string imageName;
    public string _idNews;
    //public TextMeshProUGUI priceTel;
    //public TextMeshProUGUI priceTnl;
    //public Button btnInscribe;
    //public GameObject objPaper;
    //public GameObject Loader;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector3(-1280f, 0f, 0f), 0f);
    }
    void Start()
    {
        //getEvent();
    }

 

    void getEvent()
    {
        GameObject itemMenu;
        bool firsItem = true;
        foreach (Transform evt in contentNews.transform)
            if (evt.GetComponent<TorneoItemUIS>()) Destroy(evt);

        foreach (var torneo in VersionNv.TorneosActuales)
        {
            TorneoItemUIS go = Instantiate(objectPanel, contentNews.transform.position, Quaternion.identity, contentNews.transform).GetComponent<TorneoItemUIS>();
            if (firsItem)
            {
                itemMenu = contentMenu.transform.GetChild(0).gameObject;
                itemMenu.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = torneo.Value.Detalles[(int)GamePlayerPrefs.instance._LanguageGame].Split("-")[1];
                firsItem = false;
            }
            else
            {
                itemMenu = Instantiate(contentMenu.transform.GetChild(0).gameObject, contentMenu.transform.position, Quaternion.identity, contentMenu.transform);
                itemMenu.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = torneo.Value.Detalles[(int)GamePlayerPrefs.instance._LanguageGame].Split("-")[1];
            }
            go.id = torneo.Value.Id;
            go.title.text = torneo.Value.Detalles[(int)GamePlayerPrefs.instance._LanguageGame].Split("-")[1];
            go.date.text = torneo.Value.Date_Start;
            go.details.text = torneo.Value.Detalles[(int)GamePlayerPrefs.instance._LanguageGame].Split("-")[2];
            StartCoroutine(loadPostImage(torneo.Value.Img, go.imgPost.GetComponent<RawImage>()));
            go.telValue.text = torneo.Value.PriceTEL.ToString();
            go.tlnValue.text = torneo.Value.PriceTLN.ToString();
            go.level.text = torneo.Value.Level.ToString();


            //go._idNews = torneo.Value.Id;
            //go.day.text = torneo.Value.DateTime_Start.Day.ToString();
            //go.month.text = torneo.Value.DateTime_Start.ToString();
            //go.month.text = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(torneo.Value.DateTime_Start.Month);
        }
    }
    IEnumerator loadPostImage(string url, RawImage img)
    {
        WWW www = new WWW(url);
        yield return www;
        if (!File.Exists(Application.persistentDataPath + imageName))
        {
            if (www.error == null)
            {
                Texture2D texture = www.texture;
                img.GetComponent<RawImage>().texture = texture;
                byte[] dataByte = texture.EncodeToPNG();
                File.WriteAllBytes(Application.persistentDataPath + imageName + "png", dataByte);

            }
        }
        else
        if (File.Exists(Application.persistentDataPath + imageName))
        {
            byte[] uploadByte = File.ReadAllBytes(Application.persistentDataPath + imageName);
            Texture2D texture = new Texture2D(10, 10);
            texture.LoadImage(uploadByte);
            img.GetComponent<RawImage>().texture = texture;
        }
        //objPaper.GetComponent<Image>().DOFade(0, 1f).OnComplete(() => objPaper.SetActive(false)); ;
        //Loader.SetActive(false);
    }
    //public void setBodyPanel(string key)
    //{
    //    foreach (var torneo in VersionNv.TorneosActuales)
    //    {
    //        if (torneo.Key == key)
    //        {
    //            //objPaper.SetActive(true);
    //            //Loader.SetActive(true);
    //            //objPaper.GetComponent<Image>().DOFade(1, 0f);
    //            //bodyPost.text = torneo.Value.Detalles[(int)GamePlayerPrefs.instance._LanguageGame].Split("-")[2];
    //            //tittlePost.text = torneo.Value.Detalles[(int)GamePlayerPrefs.instance._LanguageGame].Split("-")[1];
    //            //priceTel.text = torneo.Value.PriceTEL.ToString();
    //            //priceTnl.text = torneo.Value.PriceTLN.ToString();
    //            //StartCoroutine(loadPostImage(torneo.Value.Img, imagePost));
    //        }

    //    }

    //}
    //private void OnEnable()
    //{
    //    gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0f, 0f, 0f), 0.5f).SetEase(Ease.OutBack);
    //}
    //public void cerrarVentana()
    //{
    //    gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector3(-1280f, 0f, 0f), 0.5f).OnComplete(() => gameObject.SetActive(false));
    //}
}
