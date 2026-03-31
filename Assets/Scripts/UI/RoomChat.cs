using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class RoomChat : MonoBehaviour
{

    public static RoomChat Instance;


    //public UnityEngine.UI.Text tex_Chat;
    public TextMeshProUGUI tex_Chat;
    public string colorText;
    public UnityEngine.UI.Button button_Send;
    //public UnityEngine.UI.InputField text_Msj;
    public TMP_InputField text_Msj;


    // Start is called before the first frame update
    void Awake()
    {
        
        Instance = this;

        colorText = randonColor();
    }
    private string randonColor()
    {
        int c = UnityEngine.Random.Range(1, 9);
        switch (c)
        {
            case 1:
                return "red";
            case 2:
                return "blue";
            case 3:
                return "yellow";
            case 4:
                return "green";
            case 5:
                return "orange";
            case 6:
                return "purple";
            case 7:
                return "white";
            case 8:
                return "grey";
        }
        return null;
       
    }
    // Update is called once per frame
    public void changeText()
    {
        button_Send.interactable = text_Msj.text.Length > 0;
    }

    //public void sendMsj()
    //{
    //    if(RoomPlayer.Local)
    //        RoomPlayer.Local.RPC_sendMSJ((RoomPlayer.Local.IsLeader ? "<color=red>" : "") + RoomPlayer.Local.Username + (RoomPlayer.Local.IsLeader ? "</color>" : ""), text_Msj.text);
    //    text_Msj.text = "";
    //    text_Msj.ActivateInputField();
    //}
    public void addTextText(string _text)
    {
        tex_Chat.text += _text+"\n";
        tex_Chat.transform.localPosition = new Vector3(tex_Chat.transform.localPosition.x, tex_Chat.GetComponent<RectTransform>().sizeDelta.y, tex_Chat.transform.localPosition.z);
    }
    public void resetChat()
    {
        tex_Chat.text = "";
    }
    void OnGUI()
    {
        if (Event.current.keyCode == KeyCode.Return  && text_Msj.text!="")
        {
            sendMsj();
        }else if(Event.current.keyCode == KeyCode.KeypadEnter && text_Msj.text != "")
        {
            sendMsj();
        }
    }
    void sendMsj()
    {
        RoomPlayer.Local.RPC_sendMSJ(RoomPlayer.Local.Username, text_Msj.text, colorText);
        text_Msj.text = "";

    }
}
