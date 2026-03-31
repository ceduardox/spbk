using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class ScreenTutorial : MonoBehaviour
{
    public enum Screen
    {
        MainMenu,
        KartStore,
        PowerUpStore
    }
    [Header("MAIN MENU BUTTONS")]
    public int stepNumber = 0;
    public Button nextBtn;
    public TextMeshProUGUI messageTxt;
    private string currentText;
    public float delayText;
    public Image exampleImg;
    public GameObject currentMenu;
    [Space]
    public Steps[] Paso;

    private void Awake()
    {
        nextBtn.onClick.AddListener(() => setObjects());
    }

    void Start()
    {
        setObjects();

    }
    void setObjects()
    {
        foreach (var step in Paso)
        {
            if (stepNumber == step.position)
            {
                if (stepNumber == 0)
                {
                    StartCoroutine(showText(TranslateUI.getStringUI(step.message)));
                    //gameObject.transform.GetChild(0).gameObject.SetActive(false);
                }
                else
                {

                    Image img = gameObject.transform.GetChild(1).gameObject.GetComponent<Image>();
                    var tempColor = img.color;
                    tempColor.a = 0.8f;
                    img.color = tempColor;
                    gameObject.transform.GetChild(0).gameObject.SetActive(false);
                    if (currentMenu) 
                        Destroy(currentMenu);
                    if(step.itemObject!=null)
                        currentMenu = Instantiate(step.itemObject, gameObject.transform);
                    StopCoroutine(showText(TranslateUI.getStringUI(step.message)));
                    StartCoroutine(showText(TranslateUI.getStringUI(step.message)));
                    if (step.exmpleImg != null) exampleImg = step.exmpleImg;
                    Button[] buttons = currentMenu.GetComponentsInChildren<Button>();
                    offButton(buttons);
                }
               
            }
        }
        stepNumber++;
        if (stepNumber >= Paso.Length)
            StartCoroutine(endMessage());
    }
    void offButton(Button[] btn)
    {
        foreach (Button b in btn)
        {
            b.interactable = false;
        }
    }
    IEnumerator showText(string text)
    {
        for (int i = 0; i <= text.Length; i++)
        {
            currentText = text.Substring(0, i);
            messageTxt.text = currentText;
            yield return new WaitForSeconds(delayText);
        }
    }
    IEnumerator endMessage()
    {
        
            yield return new WaitForSeconds(5);
        
        gameObject.SetActive(false);
    }
    [System.Serializable]
    public class Steps
    {
        [Header("Variables and Objects")]
        public int position;
        public GameObject itemObject;
        public UI_CODE message;
        public Image exmpleImg;
        public Screen Screen;
    }

}
