using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectListRoom : MonoBehaviour
{
    public static ObjectListRoom instance;
    //[SerializeField] private TextMeshProUGUI textoLista;
    [SerializeField] private GameObject objetoLista;
    public string RoomSelectedObjet=null;
    public string RoomSelectedObjetButton=null;
    // Start is called before the first frame update
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
    void Start()
    {
        
    }
    
    public void cargarLista(List<itemList> _listaTotal)
    {
       
        ObjectListText itemTMP;
        //Me encargo de limpiar la lista cada vez que se actualiza
        foreach(Transform items in transform)
        {
            Destroy(items.gameObject);
        }
        //ListaTotal;
        if (_listaTotal != null)
        {
            for (int i = 0; i < _listaTotal.Count; i++)
            {
                //Instantiate(objetoLista, Vector2.zero, Quaternion.identity);
                GameObject ob = Instantiate(objetoLista, this.transform.position, Quaternion.identity);
                ob.transform.SetParent(this.transform);
                itemTMP =ob.GetComponent<ObjectListText>();
                itemTMP.sessionName.text = _listaTotal[i].sessionName;
                itemTMP.players.text = _listaTotal[i].players.ToString();
                itemTMP.maxPlayers.text = _listaTotal[i].maxPlayers.ToString();
                itemTMP.status.text = _listaTotal[i].isOpen?"En Lobby":"En Carrera";
                itemTMP.isOpen= _listaTotal[i].isOpen;
                //var textName = GetComponent<TextMeshProUGUI>();
            }
        }
    }
     
    public void SetCurrentNameRoom(string text)
    {
        RoomSelectedObjetButton = RoomSelectedObjet = text;
    }

    public void removeItems()
    {
    foreach(Transform t in transform)
            Destroy(t.gameObject);
    RoomSelectedObjetButton = "";
    }
}
