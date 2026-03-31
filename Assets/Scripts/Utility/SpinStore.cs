using UnityEngine;



public enum ScreenFocus
{
	HOME,
	POWERUPS,
	LOBBYLIST,
	LOBBY,
	STORE,
	NONE,
	STORE_KARTS,
	STORE_CHARS,
	CLAN,
	TORNEO,


}
public class SpinStore : MonoBehaviour
{
	public Vector3 axis = Vector3.up;
	public float rate;
	public float fv=30;
	public AnimationCurve curve;
	public float[] angles;
	float timeToRotate;
	public Camera cameraKart;
	public Camera cameraHome;
	public Transform plataforma;
	public static SpinStore instance;
	Vector3 originalPositionCameraKart;
	Vector4 originalPositionCameraHome;

	Vector4 powerUpsScreen;
	Vector4 lobbyListScreen;
	Vector4 lobbyScreen;
	Vector4 lastScreen;
	Vector4 storeScreen;
	Vector4 storeScreen2;
	ScreenFocus _actual;


	private void Start()
	{
		instance = this;
		plataforma = GameObject.Find("Plataforma").transform;
		originalPositionCameraKart = cameraKart.transform.localPosition;
		resetSpin();
		originalPositionCameraHome = new Vector4(1.23f,0.85f,2.44f,30);
		powerUpsScreen = new Vector4(2f, 0.72f,	1.95f,29);
		lobbyListScreen = new Vector4(.85f, 0.9f, 2.9f,34);
		lobbyScreen = new Vector4(1.4f, 0.62f, 2.4f,35);
		storeScreen = new Vector4(0.85f,.82f,2.68f,fv);
		storeScreen2 = new Vector4(1.13f,1.3f,3.5f,30);
		setCamera(ScreenFocus.HOME);
		//transform.localRotation = new Quaternion(0, 2.5f, 0, 1);

	}
	bool saltar;
	public void turnOffCameras()
    {
		cameraHome.gameObject.SetActive(false);
		cameraKart.gameObject.SetActive(false);
    }
	public void setCamera(ScreenFocus _screen)
    {
		if(_actual != _screen) saltar = false;
		value = 0;
		_actual = _screen;

		//CLog.Log("ESTO VALE: " + _screen+" "+ GameLauncher.ConnectionStatus);
		switch (_screen)
        {
			case ScreenFocus.HOME:
				if (GameLauncher.ConnectionStatus == ConnectionStatus.Connected|| GameLauncher.ConnectionStatus == ConnectionStatus.Connecting)
					setCamera(ScreenFocus.LOBBY); 
				else
				lastScreen = (originalPositionCameraHome);
				fv = 30;
				break;
			case ScreenFocus.POWERUPS:
				lastScreen=(powerUpsScreen);
				break;
			case ScreenFocus.LOBBYLIST:
				lastScreen = (lobbyListScreen);
				break;
			case ScreenFocus.LOBBY:
				lastScreen = (lobbyScreen);
				break;
			case ScreenFocus.STORE:
				lastScreen = (storeScreen);
				break;
		}
    }
	float value;
	float module;
	float module2;
	public void setCamera()
    {
		//CLog.Log("SALTARA: " + saltar);
		
		cameraHome.fieldOfView = Mathf.Lerp(cameraHome.fieldOfView, fv+ value, Time.deltaTime);
		
		/*if ((module=(cameraHome.transform.position - (Vector3)lastScreen).magnitude )< 0.01f && _actual == ScreenCamera.STORE || saltar) 
		{
			saltar = true;
			return; 
		}*/

		
		cameraHome.transform.localPosition = Vector3.Lerp(cameraHome.transform.localPosition, lastScreen, Time.deltaTime);
		


	}

	public void setFreeRotate(bool _free)
	{
		if (_free)
		{
			values = 0;
		}
		else
        {

        }
	} 
	public void setAngles(ClassPart _class)
    {
		//enabled = true;
		timeToRotate = 3;
		switch (_class)
		{
			case ClassPart.ALL:
				values = 160;
				cameraKart.fieldOfView = 40;
				cameraKart.transform.position = new Vector3(originalPositionCameraKart.x-0.05f, originalPositionCameraKart.y, originalPositionCameraKart.z);
				break; 
		case ClassPart.NONE:
				values = 160;
				break;
			case ClassPart.MOTOR:
				values = 250;
				break;
			case ClassPart.SPOILER_M:
				values = 190;
				break;
			case ClassPart.TIRES:
				values = 210;
				break;
			case ClassPart.FACE:
			case ClassPart.BODY:
			case ClassPart.GLASSES:
			case ClassPart.HAND:
			case ClassPart.GLOVES:
			case ClassPart.HAT:
			case ClassPart.HELMET:
				values =150;
				break;
			case ClassPart.LIGHT_F:
			case ClassPart.SPOILER_F:
				values = 170;
				break;
			case ClassPart.EXHAUST:
				values = 270;
				break;
			case ClassPart.ANTENNA:
				values = 300;
				lastScreen = storeScreen2;
				break;
			case ClassPart.SPOILER_B:
			values = 260;
				break;
			default:
				timeToRotate = -1;
			break;
		}

		if(ClassPart.ANTENNA==_class)
			lastScreen = storeScreen2;
		else
			lastScreen = storeScreen;

	}

	public void setCamera(bool _kart)
    {
		cameraKart.transform.position = originalPositionCameraKart;
		if (_kart)
		{
			cameraKart.fieldOfView = 40;
			//cameraHome.fieldOfView = 32;
			fv = 30;
			value = 3;

			plataforma.localScale = Vector3.one;
		}
		else
		{
			cameraKart.fieldOfView = 40;
			//cameraHome.fieldOfView = 25;
			fv = 24;
			value = -4;

			plataforma.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		}
	}
	public void resetSpin()
	{
		//timeToRotate = -1;
		setAngles(ClassPart.ALL);
		//transform.eulerAngles = new Vector3(transform.eulerAngles.x, values, transform.eulerAngles.z);

	}
	//public float value = 20;
	public float values=0;


    private void LateUpdate()
	{
		setCamera();
		//CLog.Log("CAMERA: " + cameraHome.transform.localPosition);

		/*
		//transform.localRotation = Quaternion.AngleAxis(curve.Evaluate(Time.time * rate) * 360, axis);
		//spine.localEulerAngles = LerpAxis(Axis.Y, spine.localEulerAngles, (inverseSpine ? 1 : -1) * TireYaw, 2 * Time.deltaTime);//DABROS316
		if (timeToRotate < 0)
		{
			//enabled = false;//	
		//transform.Rotate(Vector3.up * 15 * Time.deltaTime); // transform.localRotation = Quaternion.AngleAxis(transform.localRotation.eulerAngles.y, new Vector3(0,1,0));// ()//Quaternion.AngleAxis(curve.Evaluate(Time.deltaTime* rate) * 360, axis);

		}
		else*/
		if (values != 0)//&& saltar)
		{
			//timeToRotate -= Time.deltaTime;
			//transform.localRotation = Quaternion.Lerp(transform.localRotation, new Quaternion(0, values, 0, 1), Time.deltaTime * value);//         LerpAxis(axis.y, transform.localEulerAngles, 10, 2 * Time.deltaTime);//DABROS316
			//transform.localRotation = new Quaternion(0, 0, value,0);
			transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, new Vector3(transform.eulerAngles.x, values, transform.eulerAngles.z), Time.deltaTime*3);

			module2 = (transform.eulerAngles - new Vector3(transform.eulerAngles.x, values, transform.eulerAngles.z)).magnitude;
			//tr.rotation = Quaternion.Slerp(myObject.rotation, newRotation, Time.time * 1);
		}

	}
}
