using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TournamentEffect : MonoBehaviour
{
    public GameObject logonera;
    public GameObject wallLeft;
    public GameObject wallRight;
    public GameObject logoneraleft;
    public GameObject logoneraright;
    public GameObject backDinamic;
    void Start()
    {
        
    }
    
    private void OnEnable()
    {
        wallLeft.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0f, 0f, 0f), 1f).SetEase(Ease.InBack).OnComplete(()=>logoneraleft.SetActive(false));
        wallRight.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0f, 0f, 0f), 1f).SetEase(Ease.InBack).OnComplete(() => logoneraright.SetActive(false));
        StartCoroutine(startLogonera());
       
    }
    IEnumerator startLogonera()
    {
        yield return new WaitForSeconds(1f);
        logonera.SetActive(true);
        logonera.transform.DORotate(new Vector3(0f, 0f, -90f), 0.5f).OnComplete(
            () => logonera.transform.DOScale(new Vector3(0f, 0f, 0f), 0.3f).OnComplete(
                () => backDinamic.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0f, 0f, 0f), 1f)));
    }
    //()=>logonera.transform.DOScale(new Vector3(0f,0f,0f),0.3f)
    //    ()=>backDinamic.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0f,0f,0f),1f)
    private void OnDisable()
    {
        logonera.transform.DORotate(new Vector3(0f, 0f,0f), 0f);
        logonera.transform.DOScale(new Vector3(1f, 1f, 1f), 0f);
        logonera.SetActive(false);
        wallLeft.GetComponent<RectTransform>().DOAnchorPos(new Vector3(-1080, 0f, 0f), 0f);
        wallRight.GetComponent<RectTransform>().DOAnchorPos(new Vector3(1080, 0f, 0f), 0f);
        backDinamic.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0, -1280f, 0f), 0f);
        logoneraleft.SetActive(true);
        logoneraright.SetActive(true);
        StopAllCoroutines();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
