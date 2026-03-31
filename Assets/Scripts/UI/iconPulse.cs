using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class iconPulse : MonoBehaviour
{
    Transform frame;
    Transform glow;
    Image frameI;
    void Start()
    {
        frame = gameObject.transform.GetChild(0).transform;
        glow = gameObject.transform.GetChild(1).transform;
        frameI = gameObject.transform.GetChild(0).transform.gameObject.GetComponent<Image>();
        setEffect();
    }
    void setEffect()
    {
        frameI.DOFade(0f, 1f).OnComplete(() => frameI.DOFade(1, 0f)).SetLoops(-1, LoopType.Yoyo);
        //frameI.DOFade(0f, 0f).OnComplete(()=> frameI.DOFade(1, 1f).OnStart(()=> frame.DOScale(new Vector3(1.2f, 1.2f, 0f), 1f)).SetEase(Ease.Linear)).SetLoops(-1, LoopType.Yoyo);
        //frame.DOScale(new Vector3(1.2f, 1.2f, 0f), 1f).OnComplete(()=> frameI.DOFade(0f, 1f)).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        frame.DOScale(new Vector3(1.2f, 1.2f, 0f), 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        glow.DOScale(new Vector3(1.5f, 1.5f, 0f), 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

}
