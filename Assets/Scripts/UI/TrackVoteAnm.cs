using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class TrackVoteAnm : MonoBehaviour
{
    public Transform track1;
    public Transform track2;
    public Transform track3;
    public Transform statBar;
    public Transform stat1;
    public Transform stat2;
    public Transform stat3;
    public Transform stat4;
    float delayScale = 0.5f;
    void Start()
    {
        
        
    }
    void setAnimated()
    {
        track1.DOScale(new Vector3(1f, 1f, 1f), delayScale);
        track2.DOScale(new Vector3(1, 1f, 1f), delayScale);
        track3.DOScale(new Vector3(1f, 1f, 1f), delayScale);
        track1.gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector3(-500f, 0f, 0f), 0.3f).SetDelay(delayScale);
        //track2.gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0f, 0f, 0f), 0.3f).SetDelay(delayScale);
        track3.gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector3(500f, 0f, 0f), 0.3f).SetDelay(delayScale);
        statBar.gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0f, -450f, 0f), 0.5f).SetDelay(0.5f).OnComplete(()=>setTextAnimation());
    }
    void setTextAnimation()
    {
        stat1.DOScale(new Vector3(1f, 1f, 1f), delayScale).OnComplete(()=> numberAnim(stat1.GetComponent<Text>(), stat1.GetComponent<Text>().text.ToString(),0.5f));
        stat2.DOScale(new Vector3(1f, 1f, 1f), delayScale).OnComplete(() => numberAnim(stat2.GetComponent<Text>(), stat2.GetComponent<Text>().text.ToString(), 0.5f));
        stat3.DOScale(new Vector3(1f, 1f, 1f), delayScale).OnComplete(() => numberAnim(stat3.GetComponent<Text>(), stat3.GetComponent<Text>().text.ToString(), 0.5f));
        stat4.DOScale(new Vector3(1f, 1f, 1f), delayScale).OnComplete(() => numberAnim(stat4.GetComponent<Text>(), stat4.GetComponent<Text>().text.ToString(), 0.5f));
    }
    void numberAnim(Text ob,string realN,float delay)
    {
        //ob.text = "";
        StartCoroutine(numberAnimation(ob,realN,delay));
    }
    
    void setStartPosition()
    {
        track1.DOScale(new Vector3(0f, 0f, 0f), 0f);
        track2.DOScale(new Vector3(0f, 0f, 0f), 0f);
        track3.DOScale(new Vector3(0f, 0f, 0f), 0f);
        track1.gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0f, 0f, 0f), 0f);
        track2.gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0f, 0f, 0f), 0f);
        track3.gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0f, 0f, 0f), 0f);
        statBar.gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector3(0f, -750f, 0f), 0f);
    }
    
    IEnumerator numberAnimation(Text ob, string realNumber, float delay)
    {
        //Debug.Log("++ realnumber " + realNumber);
        ob.text = "";
        ob.gameObject.SetActive(true);
        string finalText = realNumber ?? string.Empty;
        string cleanedText = finalText.Replace("x", "").Trim();
        if (!float.TryParse(cleanedText, NumberStyles.Float, CultureInfo.InvariantCulture, out float finalValue) &&
            !float.TryParse(cleanedText, out finalValue))
        {
            ob.text = finalText;
            yield break;
        }
        float contador = 1.5f;
        float valorParcial = 00;
        yield return new WaitForSeconds(delay);
        while (contador > 0)
        {
            ob.text = valorParcial.ToString();
            valorParcial = valorParcial + Random.Range(1, 6);
            if (valorParcial > 99) { valorParcial = 0; }
            contador -= Time.deltaTime;
            if (contador <= 0f) { ob.text = finalText; };
            yield return new WaitForEndOfFrame();
        }
    }
    private void OnEnable()
    {
        setAnimated();
    }
    private void OnDisable()
    {
        setStartPosition();
    }
}
