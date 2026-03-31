using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ManagerError : MonoBehaviour
{

    string error;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        //CLog.Log("ERROR DETECTADO: " + error + " " + logString +" ---------"+ type);

        if (type == LogType.Error|| type == LogType.Exception)
        {
            error = error + "\n" + logString;
            CLog.Log("ERROR DETECTADO: " + error + " " + logString); 
        }
    }

    public void Dismiss()
    {
      
    }

}
