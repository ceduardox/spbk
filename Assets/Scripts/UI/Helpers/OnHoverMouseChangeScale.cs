using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnHoverMouseChangeScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    float Scale = 0;
   

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.transform.localScale += new Vector3(Scale,Scale,0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.localScale -= new Vector3(Scale, Scale, 0);
    }

  
}
