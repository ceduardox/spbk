using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecentPlayerLobby : MonoBehaviour, IPointerClickHandler
{
    public static RecentPlayerLobby instance;

    public Transform lobbyListPlayerContainer;
    public List<string> playerNames = new List<string>();
    private int cantPlayers;

    public GameObject popUPprofile;

    public GameObject uiMainCanvas;
    GraphicRaycaster ui_raycaster;

    PointerEventData click_data;
    List<RaycastResult> click_results;

    //public Camera mainCamera;
    //DefaultInputActions leftClick;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        //mainCamera = Camera.main;
        //leftClick.UI.Point.started += ctx => LeftMouseClicked(ctx);
    }
    private void Start()
    {
        ui_raycaster = uiMainCanvas.GetComponent<GraphicRaycaster>();
        click_data = new PointerEventData(EventSystem.current);
        click_results = new List<RaycastResult>();

        
    }
    /*private void Update()//revisar
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            //getuielement();
        }
    }*/
    public void OnPointerClick(PointerEventData e)
    {
        getuielement();
    }
    void getuielement()
    {
        click_data.position = Mouse.current.position.ReadValue();
        click_results.Clear();
        ui_raycaster.Raycast(click_data, click_results);

        foreach(RaycastResult result in click_results)
        {
            GameObject ui_element = result.gameObject;
            if (ui_element.tag == "ProfileUI")
            {
                Vector3 worldPoint = Mouse.current.position.ReadValue();
                worldPoint.z = Mathf.Abs(Camera.main.transform.position.z);
                //worldPoint.z = 11f;
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(worldPoint);
                mouseWorldPosition.z = 0f;
                //Instantiate(go, mouseWorldPosition, Quaternion.identity);

                //string displayName = ui_element.GetComponent<itemProfileUI>().displayName.text;
                //ClanSystem.GetUserProfile(displayName);
                popUPprofile.SetActive(true);
                popUPprofile.transform.position = Mouse.current.position.ReadValue();
            }
        }

    }
    
    public void CurrentClickedGameObject(GameObject gameObject)
    {
        CLog.Log(gameObject.name);
        if (gameObject.tag == "something")
        {
        }
    }
    public void OnPointerEnter(PointerEventData eventdata)
    {
        //CLog.Log("click enter");
    }
    public void OnPointerExit(PointerEventData eventdata)
    {
        //CLog.Log("click exit");
    }
    
    
    public void saveList()
    {
        for (int i = 0; i < playerNames.Count; i++)
        {
            PlayerPrefs.SetString("Players" + i, playerNames[i]);
        }
        PlayerPrefs.SetInt("Cant", playerNames.Count);
    }
    public void LoadList()
    {
        playerNames.Clear();
        cantPlayers = PlayerPrefs.GetInt("Cant");
        for (int i = 0; i < cantPlayers; i++)
        {
            string Player = PlayerPrefs.GetString("Players" + i);
            playerNames.Add(Player);
        }
    }
    public void addListPlayerPref(/*string displayName*/)
    {
        foreach (Transform t in lobbyListPlayerContainer)
        {
            string currentPlayer = t.GetComponent<LobbyItemUI>().username.text;
            if (!playerNames.Contains(PlayfabManager.instance.displayName) && PlayfabManager.instance.displayName != currentPlayer)
            {
                playerNames.Add(currentPlayer);
                //playerNames.Add(objTMP.GetComponent<LobbyItemUI>().playerDisplayName);
            }

        }
        saveList();
        //if (!playerNames.Contains(displayName) && PlayfabManager.instance.displayName!=displayName)
        //{
        //    playerNames.Add(displayName);
        //}
    }
    

}
