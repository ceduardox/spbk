using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HoverOverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Hover Button Scaler")]
    public bool useImageScaler;
    public float scaleModifier = 0.1f;
    private Vector3 defaultScale;
    public bool onChildren;
    public GameObject children;
    [Space]
    [Header("Hover Button Change Texture")]
    public bool useTextureChanger;
    [SerializeField]
    private Texture FrameGlow;
    [SerializeField]
    private Texture FrameInactiveGlow;


    [HideInInspector] public bool isHovered = false;
    private Button button;

    private void Awake()
    {
        //FrameInactiveGlow = this.transform.GetChild(0).gameObject.GetComponent<RawImage>().texture;

        //enabled = false;
        SetDefaultScale();
        button = GetComponent<Button>();
    }

    public void SetDefaultScale()
    {
        if (!onChildren)
        {
            defaultScale = transform.localScale;
        }
        else
        {
            defaultScale = children.transform.localScale;
        }
    }

    public Vector3 GetHoveredScale()
    {
        return defaultScale + new Vector3(scaleModifier, scaleModifier, scaleModifier);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!onChildren)
        {
            transform.localScale = defaultScale;
        }
        else
        {
            children.transform.localScale = defaultScale;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (useImageScaler)
        {
            if (button.IsInteractable() && !onChildren)
            {
                isHovered = true;
                transform.localScale = GetHoveredScale();
            }
            else if (button.IsInteractable() && onChildren)
            {
                isHovered = true;
                children.transform.localScale = GetHoveredScale();
            }
        }
        if (useTextureChanger)
        {
            this.transform.GetChild(0).gameObject.GetComponent<RawImage>().texture = FrameGlow;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (useImageScaler)
        {
            if (!onChildren)
            {
                transform.localScale = defaultScale;
            }
            else
            {
                children.transform.localScale = defaultScale;
            }
        }
        if (useTextureChanger)
        {
            this.transform.GetChild(0).gameObject.GetComponent<RawImage>().texture = FrameInactiveGlow;
        }
    }

    private void OnDisable()
    {
        isHovered = false;
    }
}
