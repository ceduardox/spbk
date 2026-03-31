using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoteTrackItem : MonoBehaviour
{
    public UnityEngine.UI.Text nameTrack;
    public UnityEngine.UI.Text voteNumber;
    public UnityEngine.UI.Image imageTrack;
    public UnityEngine.UI.Image imageTrackSelect;
    
    public void setItem(string _name, Sprite _img)
    {
        nameTrack.text =_name;
        imageTrackSelect.sprite=imageTrack.sprite = _img;

    }
}
