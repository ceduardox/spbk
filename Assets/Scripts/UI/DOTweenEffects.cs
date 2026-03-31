using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

[System.Serializable]
public class DOMoveEffect
{
    [Header("Variables and Objects")]
    public GameObject itemObject;
    public Vector2 originalPosition;
    public Vector2 initialPosition;
    public Vector2 moveToPosition;
    public float delayIn;
    public Ease expandeEffect;
    public float expandDuration;
    [Space]
    public float delayOut;
    public Ease collapseEffect;
    public float collapseDuration;

}

/// <summary>
/// 
/// </summary>
/// 
public enum typehorver { horizontal, vertical };
[System.Serializable]
public class GroupPanelsTransition
{
    public GameObject panel;
    public Vector2 initialPos;
    [HideInInspector] public Vector2 originalPos;
    [Header("PANEL IN")]
    public Ease EffectIn;
    [Range(0f,3f)]
    public float delayIn;
    [Range(0f, 3f)]
    public float TimeEffectIn=0.5f;
    [Space]
    [Header("PANEL OUT")]
    public Ease EffectOut;
    [Range(0f, 3f)]
    public float delayOut;
    [Range(0f, 3f)]
    public float TimeEffectOut = 0.5f;
}
[System.Serializable]
public class GroupPanelInterno
{
    public bool InitialPanel;
    public Button buttonPanel;
    public GameObject panel;
    public Vector2 initialPos;
    public Vector2 originalPos;
    public Ease Effect;
    [Range(0f, 3f)]
    public float durationEffect = 0.5f;
}
//[System.Serializable]
//public class DOFadeEffect
//{
//    public RawImage itemImage;
//    public Ease effect;
//    [Range(0,1)]
//    public float Fuerza;
//    public float duration;
//}
//[System.Serializable]
//public class DOScaleEffect
//{
//    public GameObject itemObject;
//    public Ease effect;
//    public float duration;
//}
//[System.Serializable]
//public class DORotate
//{
//    public GameObject itemObject;
//    public Vector3 Angulo;
//    public Ease effect;
//    public float duration;
//