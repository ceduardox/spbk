using UnityEngine.UI;
using UnityEngine;
using System.Threading.Tasks;

public interface ICommand
{
    Task Execute();
}

[System.Serializable]
public struct GroupButtons_BP
{
    public string nameGroup;
    public bool isActive;
    public ButtonsPanels[] Buttons;
}
[System.Serializable]
public struct ButtonsPanels
{
    public Button Button;
    public GameObject Panel;
    public bool HoldActived;
}
[System.Serializable]
public struct GroupButtons_BF
{
    public string nameGroup;
    public bool stayActive;
    //public bool defaultValueYes;
    public ButtonsFunctions[] Buttons;
}
[System.Serializable]
public struct ButtonsFunctions
{
    public Button Button;
    public bool interactable;
    public Button.ButtonClickedEvent Function;
}

[System.Serializable]
public struct GroupSettings_CP
{
    public string nameGroup;
    public bool isActive;
    public ControlPanels[] ControlPanel;
}
[System.Serializable]
public class ControlPanels
{
    //public Button Button;
    //public delegate void changeImage();
    //public event changeImage setImage;
    public string nameSetting;
    //public string PlayerPref;
    public GameObject Control;
    public GameObject Panel;
    public int defaultValue;
}