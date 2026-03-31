using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
       
    private void OnEnable()
    {
        GameManager.Instance.HUD.FadeIn(false, 0f);
        GameManager.Instance.HUD.GetComponent<Canvas>().sortingOrder = 0;
    }
    
    public void enableHUD()
    {
        GameManager.Instance.HUD.FadeIn(true, 0f);
        GameManager.Instance.HUD.GetComponent<Canvas>().sortingOrder = 5;
    }
}
