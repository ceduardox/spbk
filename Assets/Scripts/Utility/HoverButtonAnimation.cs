using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HoverButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public float scaleModifier = 0.1f;
    private Vector3 defaultScale;
    public Animator am;
    [HideInInspector] public bool isHovered = false;
    private Button button;

    private void Awake()
    {
        SetDefaultScale();
        button = GetComponent<Button>();
        am.enabled = false;
    }

    public void SetDefaultScale()
    {
        defaultScale = transform.localScale;
    }

    public Vector3 GetHoveredScale()
    {
        return defaultScale + new Vector3(scaleModifier, scaleModifier, scaleModifier);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.localScale = defaultScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.IsInteractable())
        {
            isHovered = true;
            transform.localScale = GetHoveredScale();
            am.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        transform.localScale = defaultScale;
        am.enabled = false;
    }

    private void OnDisable()
    {
        isHovered = false;
    }
}
