using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class TranslateScript : MonoBehaviour
{
    public static TranslateScript instance;
    public TextAsset popUpTraslate;
    public TextAsset[] textTranslate;
    public List<KeyValuePair<string, List<StructurePlayfab>>> DicLanguages = new List<KeyValuePair<string, List<StructurePlayfab>>>();
    public List<StructurePopUps> PopUpLanguages = new List<StructurePopUps>();
    private void Awake()
    {
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
    void Start()
    {
        cargarInterfaceTxt();
        cargarPopUpsTxt();
    }
    void cargarInterfaceTxt()
    {
        for (int j = 0; j < textTranslate.Length; j++)
        {
            List<StructurePlayfab> lt1 = JsonConvert.DeserializeObject<List<StructurePlayfab>>(textTranslate[j].ToString());
            DicLanguages.Add(new KeyValuePair<string, List<StructurePlayfab>>(textTranslate[j].name, lt1));
        }
    }
    void cargarPopUpsTxt()
    {
        PopUpLanguages = JsonConvert.DeserializeObject<List<StructurePopUps>>(popUpTraslate.ToString());
        //PopUpLanguages.Add(new KeyValuePair<string, List<StructurePopUps>>(popUpTraslate.name, lt2));
    }
    public List<string> replacePopupLanguage(int _idmsg)
    {
        List<string> words = new List<string>();
        int language = GamePlayerPrefs.instance.loadLanguageInt();
        if (language == 0)//INGLES
        {
            for (int i = 0; i < PopUpLanguages.Count; i++)
            {
                if (PopUpLanguages[i].idmsg == _idmsg)
                {
                    words.Add(PopUpLanguages[i].tittleEN);
                    words.Add(PopUpLanguages[i].descEN);
                    return words;
                }
            }
        }
        else if (language == 1)//ESPANIOL
        {
            for (int i = 0; i < PopUpLanguages.Count; i++)
            {
                if (PopUpLanguages[i].idmsg == _idmsg)
                {
                    words.Add(PopUpLanguages[i].tittleES);
                    words.Add(PopUpLanguages[i].descES);
                    return words;
                }
            }
        }
        return null;
    }
    public List<string> replaceWordsLanguage(string cat, string _itemId, int language)
    {
        List<string> words = new List<string>();
     
        
        if (language == 0) //INGLES
        {
            for (int i = 0; i < textTranslate.Length; i++)
            {
                if(DicLanguages[i].Key.ToString()== cat)
                {
                    for (int j=0; j<DicLanguages[i].Value.Count;j++)
                    {
                        if (DicLanguages[i].Value[j].key.ToString()==_itemId)
                        {
                            words.Add(DicLanguages[i].Value[j].DisplaynameEN);
                            words.Add(DicLanguages[i].Value[j].DescriptionEN);
                            return words;
                        }
                    }
                }
            }
                
        }else if (language == 1) //ESPAÑOL
        {
            for (int i = 0; i < textTranslate.Length; i++)
            {
                if (DicLanguages[i].Key.ToString() == cat)
                {
                    for (int j = 0; j < DicLanguages[i].Value.Count; j++)
                    {
                        if (DicLanguages[i].Value[j].key.ToString() == _itemId)
                        {
                            words.Add(DicLanguages[i].Value[j].DisplaynameES);
                            words.Add(DicLanguages[i].Value[j].DescriptionES);
                            return words;
                        }
                    }
                }
            }
        }
        
        return null;
    }
    public bool ExistFileJson(string cat)
    {
        for(int i = 0; i<textTranslate.Length; i++)
        {
            if(textTranslate[i].name == cat)
            {
                return true;
            }
            
        }
        return false;
    }

    public class StructurePlayfab
    {
        public string key { set; get; }
        public string DisplaynameES { set; get; }
        public string DisplaynameEN { set; get; }
        public string DescriptionES { set; get; }
        public string DescriptionEN { set; get; }
    }
    public class StructurePopUps
    {
        public int idmsg { get; set; }
        public string tittleES { get; set; }
        public string tittleEN { get; set; }
        public string descES { get; set; }
        public string descEN { get; set; }
    }
}
