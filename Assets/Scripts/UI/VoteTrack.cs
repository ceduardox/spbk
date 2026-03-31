using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoteTrack : MonoBehaviour
{
    public VoteTrackItem item_0;
    public VoteTrackItem item_1;
    public VoteTrackItem item_2;
    public WinRewards panelRewards;
    public UnityEngine.UI.ToggleGroup group;
    public UnityEngine.UI.Text countText;
    string rewards;

    public void showVoteTrack()
    {
        //
        gameObject.SetActive(true);
    }
    public void selectTrack(int _value)
    {
        group.allowSwitchOff = false;
        GameManager.Instance.RPC_changeTrack(RoomPlayer.Local.Id.ToString(), _value);//----CAMBIAR ESTO
    }

    public void setTracks(int[] _tracks, string _rewards)
    {
        rewards = _rewards;
        var defs = ResourceManager.Instance != null ? ResourceManager.Instance.tracksDefinitions : null;
        if (_tracks == null || _tracks.Length < 3 || defs == null || defs.Count == 0)
        {
            CLog.LogWarning("VoteTrack.setTracks recibio datos invalidos.");
            return;
        }

        SetTrackSafe(item_0, _tracks[0], defs, 0);
        SetTrackSafe(item_1, _tracks[1], defs, 1);
        SetTrackSafe(item_2, _tracks[2], defs, 2);
        setRewards();
    }
    


    public void setCount(int _text)
    {
        countText.text = _text.ToString();
    }

    public void countVotes(int[] _values)
    {
        if (_values == null || _values.Length < 3)
            return;

        item_0.voteNumber.text = _values[0].ToString();
        item_1.voteNumber.text = _values[1].ToString();
        item_2.voteNumber.text = _values[2].ToString();
    }

    private void SetTrackSafe(VoteTrackItem item, int trackIndex, List<TrackDefinition> defs, int slot)
    {
        if (item == null)
            return;

        if (trackIndex < 0 || trackIndex >= defs.Count || defs[trackIndex] == null)
        {
            CLog.LogWarning("VoteTrack.setTracks indice invalido en slot " + slot + ": " + trackIndex);
            return;
        }

        item.setItem(TranslateUI.getStringUI(defs[trackIndex].trackName), defs[trackIndex].trackIcon);
    }
    
    public void setRewards()
    {
        string[] rewardsLocal=rewards.Split('*');
        string[] temp = rewardsLocal[0].Split('+');

        string TEL = "00";
        for (int i = 0; i < temp.Length - 2; i++)
        {
            if (RoomPlayer.Local.playFabID.Equals(temp[i]))
            {

                TEL = temp[i + 1];
                break;
            }
        }

        string XP = "";
        temp = rewardsLocal[1].Split('+');

        for (int i = 0; i < temp.Length-2; i++)
        {
            if (RoomPlayer.Local.playFabID.Equals(temp[i]))
            {

                XP = temp[i+1];
                break;
            }
        }
        string CUPS = "";
        temp = rewardsLocal[2].Split('+');
        for (int i = 0; i < temp.Length-2; i++)
        {
            CLog.Log("Envio: CUPS: " + RoomPlayer.Local.playFabID + " comparo con: "+temp[i]+" - "+ temp[i+1]);
            if (RoomPlayer.Local.playFabID.Equals(temp[i]))
            {
                CUPS = temp[i + 1];
                break;
            }
        }
        string TNL="";
        temp = rewardsLocal[3].Split('+');
        for (int i = 0; i < temp.Length-2; i++)
        {
            if (RoomPlayer.Local.playFabID.Equals(temp[i]))
            {
                TNL = temp[i + 1];
                break;
            }
        }

        panelRewards.setValue(TEL,TNL, XP,CUPS);
    }
}
