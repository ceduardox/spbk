using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ItemStore : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler
{
    public bool isKart;
    public int idkart;
    public int posChild;
    public UnityEngine.UI.Image icon;
    public Image logoImg;
    public int level;
    public TextMeshProUGUI nameItemKarChar;
    public TextMeshProUGUI nameItem;
    public TextMeshProUGUI descItem;
    public GameObject priceModule;
    public TextMeshProUGUI priceValue;
    public TextMeshProUGUI levelItem;
    public GameObject equipped;
    public GameObject selectImg;
    public GameObject frameEquiped;
    public GameObject blokItem;
    public bool isLocked;
    public bool isSelect;
    public Image backGround;
    public Sprite background_Karts;
    public ClassPart classItem;
    public bool isEquipped;
    public bool available = true;
    public TextMeshProUGUI limitedText;
    public TextMeshProUGUI limiteRemainig;
    internal string tagItem;
    // variables para posiciones de items
    public RectTransform itemNameHolder;


    internal string displayname;
    internal string displayDescription;
    internal string nameItemStore;
    internal bool isLimited;
    internal ItemBase itemBase;
    public void setNameDesc(string _name, string _desc, float _level, bool _showlevelPart, ItemBase _itemBase)
    {
        nameItemStore = _name;

        if (nameItem)
        {
            string cadenaCelda;

            if (classItem == ClassPart.BODY ||
                classItem == ClassPart.HELMET ||
                classItem == ClassPart.HAT ||
                classItem == ClassPart.GLASSES ||
                classItem == ClassPart.HAND ||
                classItem == ClassPart.FACE ||
                classItem == ClassPart.ANTENNA)
            {
                cadenaCelda = TranslateUI.getStringStore(name);
                string[] nombreDesctipcion = cadenaCelda.Split("-");
                if (nombreDesctipcion.Length > 1)
                {
                    nameItem.text = nombreDesctipcion[0];
                    descItem.text = nombreDesctipcion[1];
                }
                else
                {
                    nameItem.text = TranslateUI.getStringStore(classItem);
                    descItem.text = nombreDesctipcion[0];
                }

            }
            else
            {
                nameItem.text = TranslateUI.getStringStore(classItem);
                descItem.text = TranslateUI.getStringStore(name.Split("-")[1].Trim());
            }

        }
        level = (int)_level;

        if (blokItem)
            blokItem.SetActive((level > PlayfabManager.instance.getLevel()));

        displayname = _name;
        displayDescription = _desc;
        if (levelItem)
        {
            if (_showlevelPart) levelItem.text = "Nivel " + name.Substring(name.Length - 2, 2);
            else levelItem.text = "";
        }

        if (_itemBase != null)
        {
            itemBase = _itemBase;
            isLimited = _itemBase.isLimited;
            if (isLimited)
            {
                foreach (var item in PlayfabManager.instance.itemsLimitedEdition.itemLimEdi)
                {
                    if (item.idItem == _itemBase.Id)
                    {
                        limitedText.transform.parent.gameObject.SetActive(isLimited);
                        limiteRemainig.text = item.quantity.ToString();
                    }
                }
            }
            else
            {
                limitedText.transform.parent.gameObject.SetActive(isLimited);
                limiteRemainig.text = "-";
            }

            //if (limitedText)
            //{
            //    limitedText.transform.parent.gameObject.SetActive(isLimited);
            //    //limiteRemainig.text=
            //}
        }
    }



    public void setTag(string _tag)
    {
        tagItem = _tag;
    }
    public void setIcon(Sprite _icon)
    {
        icon.sprite = _icon;
    }
    public void setPos(int position)
    {
        posChild = position;
    }

    public void setIcon2()
    {

        //  icon.texture = totalItems = Resources.LoadAll("Prefabs/IconKarts/" + kartDefinitions[j].nameKart + "_Parts", typeof(Sprite)).Cast<Sprite>().ToArray();

    }
    public void locked(bool _state)
    {
        //if(loockIcon) loockIcon.SetActive(!_state);

        if (priceModule)
        {
            priceModule.SetActive(_state);
            if (limitedText && !_state)
                limitedText.transform.parent.gameObject.SetActive(false);
        }

        isLocked = _state;
        //CLog.Log("SOY: " + name + " STte: " + _state);
    }


    public void isAvailable(bool _value)
    {
        if (blokItem)
            blokItem.SetActive(!_value);
        available = _value;
    }

    //Color normal = new Color(0.5f, 0.12f, 0.2f, 1);
    //Color select = new Color(0.15f, 0.3f, 0.5f, 1);
    public void setEquipped(bool _select)
    {
        equipped.SetActive(_select);
        isEquipped = _select;
    }

    public void setSelect(bool _select)
    {
        if (_select)
        {
            if (backGround) backGround.color = Color.yellow;
        }
        else
        {
            if (backGround) backGround.color = Color.white;
        }
        isSelect = _select;
    }
    public void setPrice(string _value)
    {
        priceValue.text = _value;

    }
    public bool getLocked()
    {
        return isLocked;
    }

    public void setNameItem(string _value, bool kartstore)
    {
        nameItemKarChar.text = _value;
        //if (kartstore)  // comentado para nueva ui de item
        //{
        //    nameItemKarChar.alignment = TextAlignmentOptions.Right;
        //    //priceModule.transform.position = priceModule.transform.position * -1;
        //    //priceModule.GetComponent<RectTransform>(). = -90f;
        //    priceModule.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
        //    priceModule.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
        //}
        //else
        //{
        //    //priceModule.transform.position = priceModule.transform.position * -1;
        //    nameItemKarChar.alignment = TextAlignmentOptions.Left;
        //    //priceModule.GetComponent<RectTransform>().position = new Vector2(90f, 0f);
        //    priceModule.GetComponent<RectTransform>().anchorMin = new Vector2(0.4f, 0.5f);
        //    priceModule.GetComponent<RectTransform>().anchorMax = new Vector2(0.4f, 0.5f);
        //}

    }

    public void setItemsDefaultPosition(bool KartStore)
    {

        if (!KartStore)
        {
            isKart = false;
            logoImg.gameObject.SetActive(true);
            Vector2 offsetMin = itemNameHolder.offsetMin;
            offsetMin.x = 167.687f;
            itemNameHolder.offsetMin = offsetMin;
            icon.transform.localPosition = new Vector3(-60f, 2f, 0f);
            icon.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

        }
        else
        {
            isKart = true;
            logoImg.gameObject.SetActive(false);
            Vector2 offsetMin = itemNameHolder.offsetMin;
            offsetMin.x = -6.103508e-05f;
            itemNameHolder.offsetMin = offsetMin;
            icon.transform.localPosition = new Vector3(-20f, -16.527f, 0f);
            icon.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public void setLogoImg(bool kartStore, Sprite _logoImg)
    {
        if (kartStore) return;
        else logoImg.sprite = _logoImg;
    }

    public void setBackgroundImg(bool kartStore, Sprite _backgroundImg)
    {
        if (kartStore) backGround.sprite = background_Karts;
        backGround.sprite = _backgroundImg;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (isKart)
        {
            logoImg.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.15f);
            logoImg.gameObject.GetComponent<RectTransform>().DOAnchorPosY(-43.606f, 0.15f);
        }
        else
        {
            logoImg.transform.DOScale(new Vector3(1.4f, 1.4f, 1.4f), 0.25f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //CLog.Log("eventData:" + eventData.selectedObject);
        if (eventData.selectedObject == this.gameObject) return;

        if (isKart)
            icon.transform.DOScale(new Vector3(1.05f, 1.05f, 1.05f), 0.25f);
        else
            logoImg.transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.25f);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.selectedObject == this.gameObject) return;
        if (isKart)
        {
            logoImg.gameObject.GetComponent<RectTransform>().DOAnchorPosY(-16.527f, 0.25f);
            logoImg.transform.DOScale(new Vector3(1f, 1f, 1f), 0.25f);

        }
        else
        {
            logoImg.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.25f);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (isKart)
        {
            logoImg.transform.DOScale(new Vector3(1f, 1f, 1f), 0.15f);
            logoImg.gameObject.GetComponent<RectTransform>().DOAnchorPosY(-16.527f, 0.25f);

        }
        else
        {
            logoImg.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.15f);
        }
    }
}
