using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUps : MonoBehaviour
{
    public TextMeshProUGUI nameItem;
    public TextMeshProUGUI desc;
    public TextMeshProUGUI price;
    public TextMeshProUGUI amount;
    public UnityEngine.UI.Image icon;
    public UnityEngine.UI.Button btnAdd;

    public GameObject PrecioCobra;
    public GameObject PrecioGratis;


    public void setPopUp(string _name, string _desc,int _price, string _amount, UnityEngine.UI.Image _icon, string _count)
    {
        nameItem.text = _name;
        //nameItem.text = TranslateUI.getStringStore(_id);
        desc.text = _desc; //TKARLOZ

        int _intprice = _price;
        if(_intprice != 0)
        {
            price.text = _intprice.ToString();
            if (PrecioCobra)
            {
                PrecioCobra.SetActive(true);
                PrecioGratis.SetActive(false);
            }

        }
        else
        {
            if (PrecioCobra)
            {
                PrecioCobra.SetActive(false);
                PrecioGratis.SetActive(true);
            }

        }
        amount.text = "x "+_amount;//monto a comprar
        icon.sprite = _icon.sprite;
        setEnabledAddButton(int.Parse(_count) > 0);
    }
    public void show()
    {
        gameObject.SetActive(true);
    }
    public void setEnabledAddButton(bool _value)
    {
     //   btnAdd.interactable=_value;        
    }
    
}
