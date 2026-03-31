//using Facebook.Unity;
//using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLog : MonoBehaviour
{
    [field: SerializeField] public static bool WarningLogActived { get; private set; }
    [field: SerializeField] public static bool ErrorLogActived { get; private set; }
    [field: SerializeField] public static bool normalLogActived { get; private set; }
    public static CLog Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
            return;
        }
        DontDestroyOnLoad(this);
    }
   

    public static void Log(object log)
    {
        if (normalLogActived)
        {
            Debug.Log(log);
        }
    }
    public static void Log(object log, Object arg2)
    {
        if (normalLogActived)
        {
            Debug.Log(log, arg2);
        }
    }
    public static void LogWarning(object log)
    {
        if (normalLogActived)
        {
            Debug.LogWarning(log);
        }
    }
    public static void LogWarning(object log,Object arg2)
    {
        if (normalLogActived)
        {
            Debug.LogWarning(log,arg2);
        }
    }
    public static void LogError(object log)
    {
        if (normalLogActived)
        {
            Debug.LogError(log);
        }
    }
    public static void LogError(object log, Object arg2)
    {
        if (normalLogActived)
        {
            Debug.LogError(log,arg2);
        }
    }


    [ContextMenu("DisableFirebaseLogs")]
    private void DisableFirebaseLogs()
    {
        //FirebaseApp.LogLevel = LogLevel.Error;
    }
    [ContextMenu("EnableFirebaseLogs")]
    private void EnableFirebaseLogs()
    {
        //FirebaseApp.LogLevel = LogLevel.Debug;
    }

}

