using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LoadingPointsAnim : MonoBehaviour
{

    float total = 1750f;
    float carga = 0f;
    public int economytotal;
    public int economyprogress;
    public GameObject barra;
    public TextMeshProUGUI text;
    public int modoLoading;
    float timer = 20;
    public TextMeshProUGUI countdownText; // Nuevo componente TextMeshProUGUI para mostrar la cuenta regresiva
   public int countdownSeconds; // Variable para almacenar los segundos restantes en la cuenta regresiva

    // Nueva variable para almacenar la duración inicial de la cuenta regresiva
    public int countdownDuration;
    private bool timeoutTriggered;

    void OnEnable()
    {
        StartCountdown(timer);
    }
    // Nueva función para iniciar la cuenta regresiva
    void StartCountdown(float duration)
    {
        countdownDuration = Mathf.Max(1, Mathf.CeilToInt(duration));
        StopCoroutine(nameof(CountdownCoroutine));
        StartCoroutine(CountdownCoroutine());
    }

    // Corrutina para la cuenta regresiva
    IEnumerator CountdownCoroutine()
    {
        countdownSeconds = countdownDuration;
        while (countdownSeconds > 0)
        {
            // Actualiza el texto del contador regresivo
            countdownText.text = "Cerrar (" + countdownSeconds.ToString() + ")";
            yield return new WaitForSeconds(1);
            countdownSeconds--;
        }
        // Cuando la cuenta regresiva llega a 0, oculta el objeto
        if (LobbyUI._instance != null)
        {
            LobbyUI._instance.disconnected();
            if (InterfaceManager.Instance != null && InterfaceManager.Instance.mainMenu != null)
                LobbyUI._instance.FocusScreen(InterfaceManager.Instance.mainMenu.GetComponent<MasterScreen>());
        }
        else if (GameLauncher.instance != null && GameLauncher.instance.mainScreen != null)
        {
            UIScreen.Focus(GameLauncher.instance.mainScreen);
        }



        gameObject.SetActive(false);
    }

    public void setLoading(string _text, int _modoLoading)
    {
        setLoading(_text, _modoLoading, 20);

    }
    public void setLoading(string _text, int _modoLoading, int _timer)
    {
        timeoutTriggered = false;
        if (GameLauncher.instance.modeServerDedicado)
            timer = 10;
        else 
            timer = _timer;

        CLog.Log("LLAMO A CARGAR PANTALLA "+_modoLoading);
        Debug.Log($"{_text}, Aprendemos cosas nuevas");

        DataEconomy.progreso = -1;
        text.text = _text;
        carga = gameObject.GetComponent<RectTransform>().rect.width;
        modoLoading = _modoLoading;
        barra.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
        countdownText.gameObject.SetActive(true); // Activa el componente TextMeshProUGUI para mostrar la cuenta regresiva

        gameObject.SetActive(true);
        StartCountdown(timer);
    }
    public bool isShow()
    {
        return gameObject.activeSelf;
    }
    public void hide()
    {
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {

        if((timer-=Time.deltaTime) <0 && !timeoutTriggered)
        {
            timeoutTriggered = true;
            if(!GameLauncher.instance.modeServerDedicado)
                GameLauncher.instance.LeaveSessionGame(10);
            //hide();
        }
        if (modoLoading == 0 && DataEconomy.progreso != DataEconomy.total - 1)
        {
            economytotal = DataEconomy.total - 1;
            economyprogress = DataEconomy.progreso;
            float porcentaje = ((float)DataEconomy.progreso / (float)(DataEconomy.total - 2)) * total;
            barra.GetComponent<RectTransform>().sizeDelta = new Vector2(porcentaje, 0f);
        }
        else if (modoLoading == 1 && GameLauncher.ConnectionStatus != ConnectionStatus.Connected)
        {
            barra.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Lerp(barra.GetComponent<RectTransform>().sizeDelta.x, total, Time.deltaTime), 0f);
        }
        else if (modoLoading == 2)
        {
            barra.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Lerp(barra.GetComponent<RectTransform>().sizeDelta.x, total, Time.deltaTime * .4f), 0f);
        }
        else
        {
            if (modoLoading == 0 && DataEconomy.ECONOMYSTATUS == EconomyStatus.OK)
            {
                hide();
            }

        }
    }
}
