using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrackManger : MonoBehaviour
{
    private string music = "";
    private void OnEnable()
    {
        if (GameManager.Instance != null && GameManager.Instance.GameType != null)
            AudioEnviromentControl.TotalLaps = GameManager.Instance.GameType.lapCount;

        var track = GetComponent<Track>();
        if (track == null)
            return;

        music = track.music;
        if (!string.IsNullOrEmpty(music))
            AudioEnviromentControl.trackEnabled(music);

    }

    private void OnDisable()
    {
        if (!string.IsNullOrEmpty(music))
            AudioEnviromentControl.trackDisabled(music);
    }
}
