using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIEffects : MonoBehaviour
{
    public enum estado { unfolded, folded }
    [Header("TRANSITION SCREEN")]
    public GameObject background;
    public float duration;
    public bool showDisplay = false;
    public GameObject DisplayInfo;
    [Header("MOVE EFFECTS")]
    public DOMoveEffect[] DOMoveItem;
    bool firstPosition = true;
    private void Awake()
    {
        //for (int i = 0; i < DOMoveItem.Length; i++)
        //{
        //    DOMoveItem[i].originalPosition = DOMoveItem[i].itemObject.transform.localPosition;
        //}
        //for (int i = 0; i < DOMoveItem.Length; i++)
        //{
        //    DOMoveItem[i].initialPosition = DOMoveItem[i].originalPosition + DOMoveItem[i].moveToPosition;
        //}

    }

    public void nextScreenInGame(UIScreen screen)
    {
        if (GameLauncher.ConnectionStatus == ConnectionStatus.Connected)
        {
            if (LobbyUI._instance != null)
                nextScreen(LobbyUI._instance.GetComponent<UIScreen>());
            else
                nextScreen(screen);
        }
        else nextScreen(screen);
    }
    public void nextScreen(UIScreen screen)
    {
        background.SetActive(true);
        startMoveEffects(estado.folded);
        background.GetComponent<RawImage>().DOFade(0f, 0);
        background.GetComponent<RawImage>().DOFade(1f, 0.5f).OnComplete(() => changeScreen(screen));
        //DisplayDataUI.instance.nextScreen();
    }
    public void backScreen()
    {
        background.SetActive(true);
        startMoveEffects(estado.folded);
        background.GetComponent<RawImage>().DOFade(0f, 0f);
        background.GetComponent<RawImage>().DOFade(1f, 0.5f).OnComplete(() => gameObject.GetComponent<UIScreen>().Back());
        background.SetActive(false);
    }
    void changeScreen(UIScreen screen)
    {
        background.SetActive(false);
        UIScreen.Focus(screen);
    }
    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(enableScreen());
    }
    IEnumerator enableScreen()
    {
        if (background)
        {
            background.SetActive(true);
            background.GetComponent<RawImage>().DOFade(1f, 0);
        }
        yield return new WaitForSeconds(0.1f);
        itemsStartPosition();
        startMoveEffects(estado.unfolded);
        if(background) background.GetComponent<RawImage>().DOFade(0f, duration).OnComplete(() => background.SetActive(false));
        InterfaceManager.Instance.enableDisplayData(showDisplay);
        //if(PlayfabManager.instance!=null) PlayfabManager.instance.updateDisplayMenu();
    }
    public void startMoveEffects(estado estadoActual)
    {
        if (DOMoveItem == null || DOMoveItem.Length == 0)
        {
            return;
        }

        if (estadoActual == estado.unfolded)
        {
            for (int i = 0; i < DOMoveItem.Length; i++)
            {
                if (DOMoveItem[i].itemObject == null) continue;
                RectTransform r2 = DOMoveItem[i].itemObject.GetComponent<RectTransform>();
                if (r2 == null) continue;
                r2.DOAnchorPos(DOMoveItem[i].originalPosition, DOMoveItem[i].expandDuration).SetEase(DOMoveItem[i].expandeEffect).SetDelay(DOMoveItem[i].delayIn);
            }
        }
        else if (estadoActual == estado.folded)
        {
            for (int i = 0; i < DOMoveItem.Length; i++)
            {
                if (DOMoveItem[i].itemObject == null) continue;
                RectTransform r2 = DOMoveItem[i].itemObject.GetComponent<RectTransform>();
                if (r2 == null) continue;
                r2.DOAnchorPos(DOMoveItem[i].initialPosition, DOMoveItem[i].expandDuration).SetEase(DOMoveItem[i].expandeEffect).SetDelay(DOMoveItem[i].delayOut);

                //DOMoveItem[i].itemObject.transform.DOMove(DOMoveItem[i].initialPosition, DOMoveItem[i].expandDuration).SetEase(DOMoveItem[i].expandeEffect).SetDelay(DOMoveItem[i].delay);
            }
        }
    }
    public void itemsStartPosition()
    {
        if (DOMoveItem == null || DOMoveItem.Length == 0)
        {
            firstPosition = false;
            return;
        }

        if (!firstPosition)
        {
            for (int i = 0; i < DOMoveItem.Length; i++)
            {
                //DOMoveItem[i].itemObject.transform.localPosition = DOMoveItem[i].initialPosition;
                if (DOMoveItem[i].itemObject == null) continue;
                RectTransform r = DOMoveItem[i].itemObject.GetComponent<RectTransform>();
                if (r == null) continue;
                Vector2 anch = r.anchoredPosition;
                anch = DOMoveItem[i].initialPosition;
                r.anchoredPosition = anch;
            }
            
        }
        firstPosition = false;
    }

}
