using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContentListRooms : MonoBehaviour
{
    public static ContentListRooms instance;
    //[SerializeField] private TextMeshProUGUI textoLista;
    [SerializeField] private GameObject object_item;
    [SerializeField] private GameObject object_panel;
    [SerializeField] private GameObject LoadingCircle;
    public string RoomSelectedObjet = null;
    public string RoomSelectedObjetButton = null;
    private List<itemList> listaSalas = new List<itemList>();
    public ObjectItemRoom sessionSelect;

    public Color sessionActive;
    public Color sessionDeactive;
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
        DontDestroyOnLoad(gameObject);
    }
    public void cargarLista(List<itemList> _listaTotal)
    {
        listaSalas = _listaTotal;
        ObjectItemRoom itemTMP=null;
        //Me encargo de limpiar la lista cada vez que se actualiza
        foreach (Transform items in transform)
        {
            bool borrar = true;
            for (int i = 0; i < _listaTotal.Count; i++)
            {
                if (items.GetComponent<ObjectItemRoom>().sessionName.text.Equals(_listaTotal[i].sessionName)&&
                    !_listaTotal[i].borrar)
                {
                    borrar = false;
                    break;
                }
                    
            }
            if(borrar)
                Destroy(items.gameObject);
        }
        GameObject ob;
        if (_listaTotal != null)
        {
            for (int i = 0; i < _listaTotal.Count; i++)
            {
                //Instantiate(objetoLista, Vector2.zero, Quaternion.identity);
                if (!_listaTotal[i].borrar)
                {
                    foreach (Transform items in transform)
                    {
                        if (items.GetComponent<ObjectItemRoom>().sessionName.text.Equals(_listaTotal[i].sessionName))
                        {
                            itemTMP = items.GetComponent<ObjectItemRoom>();
                            break;
                        }
                    }
                    
                    if (itemTMP==null)
                    {

                        ob = Instantiate(object_item, this.transform.position, Quaternion.identity);
                        ob.transform.SetParent(this.transform);
                        ob.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        itemTMP = ob.GetComponent<ObjectItemRoom>();
                    }

                    itemTMP.sessionName.text = _listaTotal[i].sessionName;
                    itemTMP.players.text = _listaTotal[i].players.ToString() + " / "+ _listaTotal[i].maxPlayers;
                    //itemTMP.maxPlayers.text = _listaTotal[i].maxPlayers.ToString();
                    itemTMP.status.text = _listaTotal[i].isOpen ? "En Lobby" : "En Carrera";
                    itemTMP.modeGame.text = ResourceManager.instance.gameTypes[_listaTotal[i].modeGame>0? _listaTotal[i].modeGame:0].name;
                    itemTMP.isOpen = _listaTotal[i].isOpen;
                    itemTMP.PlayerBarFiller(_listaTotal[i].players);
                    itemTMP.session = _listaTotal[i].session;
                    CLog.Log("BET ES: " + _listaTotal[i].bet);
                    itemTMP.bet = int.Parse(_listaTotal[i].bet);
                    itemTMP.belt.text = _listaTotal[i].bet;
                    itemTMP.borrar = _listaTotal[i].borrar;
                    itemTMP = null;
                }


                //var textName = GetComponent<TextMeshProUGUI>();
            }
        }






        updateListDetalsRooms();
        setLengPanelBotones();
    }

    public void setLengPanelBotones()
    {
        if (transform.childCount > 0)
        {
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(transform.GetComponent<RectTransform>().sizeDelta.x, (transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 3.5f) * (transform.childCount));// = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, content.GetChild(0).GetComponent<RectTransform>().sizeDelta.y*10);
            LoadingCircle.SetActive(false);
        }
        else
        {
            LoadingCircle.SetActive(true);
        }
    }
    private void OnDisable()
    {
        LoadingCircle.SetActive(true);
    }

    public void SetCurrentNameRoom(string text, bool isOpen, ObjectItemRoom _session)
    {
        //if (!btn || btn.interactable)
        //    AudioManager.Play("hoverUI", AudioManager.MixerTarget.UI);

        AudioManager.Play("clickUI", AudioManager.MixerTarget.UI);

        if (isOpen)
        {
            RoomSelectedObjetButton = text;
            sessionSelect = _session;
        }
        else
        {
            RoomSelectedObjetButton = text;
            sessionSelect = _session;

        }

        sessionSelect.ButtonSelect.color = sessionActive;

        /*  else
          {
              RoomSelectedObjetButton = "";
              sessionSelect = null;
          }*/

        RoomSelectedObjet = text;
        updateListDetalsRooms();
    }
    public void updateListDetalsRooms()
    {
        if (listaSalas != null)
        {
            bool isRoom = false;
            for (int i = 0; i < listaSalas.Count; i++)
            {
                if (listaSalas[i].sessionName == RoomSelectedObjet)
                {
                    isRoom = true;
                    object_panel.GetComponent<ObjectPanelRoom>().setDetails(listaSalas[i].trackid, listaSalas[i].laps, listaSalas[i].maxlaps, listaSalas[i].bet, ((listaSalas[i].players).ToString() + " / "+ listaSalas[i].maxPlayers), (listaSalas[i].isOpen ? "En Lobby" : "En Carrera"));
                    break;
                }
            }
            if(!isRoom)
                object_panel.GetComponent<ObjectPanelRoom>().setDetails("-", "-", "-", "-", "- / -", "-");
        }
    }
    public void removeItems()
    {
        foreach (Transform t in transform)
            Destroy(t.gameObject);
        RoomSelectedObjetButton = "";
    }
}
