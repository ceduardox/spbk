using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accelerometro : MonoBehaviour
{
    // Start is called before the first frame update

    public static float sensibilidad;
    public static float deadZone;
    public static bool isAcelerometro;

    public UnityEngine.UI.Slider sensibilidadSlider;
    public UnityEngine.UI.Slider deadZoneSlider;
    public UnityEngine.UI.Toggle activeAccelerometro;
    public GameObject ContainerAndroid;

    private void Start()
    {
        ContainerAndroid.SetActive(!InterfaceManager.Instance.isPC);
    }

    private void OnEnable()
    {
        sensibilidadSlider.value = sensibilidad = PlayerPrefs.GetFloat(KeysPlayerPref.accSensibilidad.ToString(), .5f);
        deadZoneSlider.value = deadZone = PlayerPrefs.GetFloat(KeysPlayerPref.deadZone.ToString(), .5f);
        activeAccelerometro.isOn = isAcelerometro = PlayerPrefs.GetInt(KeysPlayerPref.enableAcc.ToString(), 0) == 1;
    }
    public void setValueSensibilidad(UnityEngine.UI.Slider _sl)
    {

        PlayerPrefs.SetFloat(KeysPlayerPref.accSensibilidad.ToString(), sensibilidad = _sl.value);
    }
    public void setValueDeadZone(UnityEngine.UI.Slider _sl)
    {

        PlayerPrefs.SetFloat(KeysPlayerPref.deadZone.ToString(), deadZone = _sl.value);
    }
    public void setValueActive(UnityEngine.UI.Toggle _value)
    {

        PlayerPrefs.SetInt(KeysPlayerPref.enableAcc.ToString(), (isAcelerometro = _value.isOn) ? 1 : 0);
    }
}
