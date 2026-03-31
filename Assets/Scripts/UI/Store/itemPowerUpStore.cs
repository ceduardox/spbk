using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

public class itemPowerUpStore : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler
{
    public int price; //TKARLOZ
    public TextMeshProUGUI Textprice; //TKARLOZ
    public TextMeshProUGUI nameItem;
    public TextMeshProUGUI descItem;
    public GameObject block;
    internal string desc;
    public int level;
    public TextMeshProUGUI count;
    public UnityEngine.UI.Image backIcon;
    public UnityEngine.UI.Image icon;
    public ClassPart classPu;
    public string id;
    private bool empty;
    public PopUps popUp;
    public bool isItemStore;
    public GameObject PrecioCobra;
    public GameObject PrecioGratis;


    private void Start()
    {
        if(!isItemStore)
        {
            //verificar Stock
        }
    }


    /// <summary>
    /// true si esta bloqueado
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_id"></param>
    /// <param name="_desc"></param>
    /// <param name="_level"></param>
    /// <returns></returns>
    public bool setData(string _name, string _id,string _desc, float _level)
    {
        //nameItem.text = _name;
        //TranslateUI.getStringStore(name.Split("-")[1].Trim());
        nameItem.text = TranslateUI.getStringStore(_id);
        descItem.text = TranslateUI.getStringStore(_id+"_desc");
        desc = _desc;
        id = _id;
        level = (int)_level;
        if(block) block.SetActive(_level > PlayfabManager.instance.getLevel());
        return _level > PlayfabManager.instance.getLevel();
    }
    public void setCount(int _count)
    {
        count.text = _count+"";
    }
    public void addCount(int _count)
    {
        count.text = int.Parse(count.text)+_count + "";
    }
    public void setPrice(string _price)
    {
        price = int.Parse( _price);
        if (price > 0)
        {
            Textprice.text = price.ToString();
            if (PrecioCobra)
            {
                PrecioGratis.SetActive(false);
                PrecioCobra.SetActive(true);
            }
        }
        else
        {
            if (PrecioCobra)
            {
                PrecioGratis.SetActive(true);
                PrecioCobra.SetActive(false);

            }
        }
    }

    public void setClass(ClassPart _class )
    {
        classPu = _class;
        icon.sprite = ResourceManager.Instance.getIconPowerUps(_class);
        var sprite = ResourceManager.instance.getBackIconPowerUps(_class);
        if (sprite == null)
        {
            Debug.LogError($"Not found Srprite{_class}", this);
        }
        else
        {
         //   Debug.Log($"Assing Srpite{sprite}", this);
            backIcon.sprite = sprite;
        }

    }

    public void reset()
    {
        nameItem.text = "";
        if (Textprice)
        {
            Textprice.text = "";
            price = 0;
        }
        if (PrecioCobra)
        {
            PrecioCobra.SetActive(true);
            PrecioGratis.SetActive(false);
        }
        id = "";
        count.text = "0";
        icon.sprite = null;
    }

    public void setPopUp()
    {

        if (price > PlayfabManager.instance.getTLN())
        {
            GameLauncher.instance.sinFondos();
            return;
        }

        if (level <= PlayfabManager.instance.getLevel()) //SETLANGUAGE
        {
            popUp.setPopUp(nameItem.text,
                        desc,
                        price,
                        Busines.consumibleCountBuy.ToString(),
                        icon,
                        count.text);
            popUp.show();
        }
        else
        {
            PopUpManager._instance.setPopUp(TranslateUI.getStringUI(UI_CODE.POPUP_TITLE_ERROR), TranslateUI.getStringUI(UI_CODE.POPUP_MSJ_NEEDLEVEL).Replace("XXX",level.ToString()), IconosPopUp.error, false);
        }
    }



    public void setEquipped(string _name,string _id,string _count,Sprite _icon,Sprite _backIcon)
    {
        nameItem.text = _name;
        id = _id;
        count.text = _count;
        icon.sprite = _icon;
        backIcon.sprite = _backIcon;
        gameObject.SetActive((int.Parse(count.text) > 0));
    }

    public bool isEmpty()
    {
        return empty;
    }

    public void OnSelect(BaseEventData eventData)
    {

        icon.transform.DOScale(new Vector3(1.4f, 1.4f, 1.4f), 0.15f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //CLog.Log("eventData:" + eventData.selectedObject);
        if (eventData.selectedObject == this.gameObject) return;

        icon.transform.DOScale(new Vector3(1.15f, 1.15f, 1.15f), 0.25f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.selectedObject == this.gameObject) return;
        icon.transform.DOScale(new Vector3(1f, 1f, 1f), 0.25f);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        icon.transform.DOScale(new Vector3(1f, 1f, 1f), 0.15f);
    }
}
