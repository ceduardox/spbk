using UnityEngine;

public class KartCamera : KartComponent, ICameraController
{
	public ParticleSystem speedLines;

	public Transform rig;
	public Transform camNode;
	public Transform forwardVP;
	public Transform backwardVP;
	public Transform forwardBoostVP;
	public Transform backwardBoostVP;
	public Transform finishVP;
	public float lerpFactorVP = 3f;
	public float lerpFactorFOV = 1.5f; 
	public float normalFOV = 60;
	public float boostFOV = 70;
	public float finishFOV = 45;
	public bool useFinishVP;

	private float _currentFOV = 60;
	private Transform _viewpoint;
	private bool _shouldLerpCamera = true;
	private bool _lastFrameLookBehind;
	public float lerpRotation;
	public float angle;
	public float angleVertical = .5f;
	public override void OnLapCompleted(int lap, bool isFinish)
	{
		base.OnLapCompleted(lap, isFinish);

		if (isFinish)
		{
			useFinishVP = true;
		}
	}

	public override void Render()
	{
		base.Render();

		if (Object.HasInputAuthority && _shouldLerpCamera && !GameManager.IsCameraControlled)
		{
			rig.rotation = transform.rotation;
			GameManager.GetCameraControl(this);
		}
	}

	public bool ControlCamera(Camera cam)
	{
		if (this.Equals(null))  
		{
			CLog.LogWarning("Releasing camera from kart"); 
			return false;
		}

		_viewpoint = GetViewpoint();

		if (_shouldLerpCamera)
			ControlCameraLerp(cam);
		else
			ControlCameraDriving(cam); 

		return true;
	}
	float Steer;
	float amount;
	bool change = true; 
	private void ControlCameraDriving(Camera cam)
	{
		if (change)
		{
			rig.localPosition = new Vector3(0, 0.8f, -0.18f); 
			//rig.localRotation= new Vector3(0, 0.8f, -0.18f); 
			cam.transform.parent = camNode.parent;
			change = false;
		}
		var lookBehindThisFrame = _lastFrameLookBehind != Kart.Input.IsLookBehindPressed;
		if (Kart.Controller.Inputs.Steer < 0)
		{
			//if(Steer!=-1)
			//amount = 1;
			Steer = Mathf.Lerp(Steer,-angle, Time.deltaTime* lerpRotation);
		}
		else if (Kart.Controller.Inputs.Steer > 0)
		{
			//if(Steer != 1)
			//amount = 1;
			Steer = Mathf.Lerp(Steer, angle, Time.deltaTime * lerpRotation);

		}
		else Steer = Mathf.Lerp(Steer, 0, Time.deltaTime * lerpRotation*3);

		//CLog.Log("333"+Kart.Controller.Inputs.Steer+" "+ Steer);

		rig.localEulerAngles = _viewpoint.localEulerAngles;

		_lastFrameLookBehind = Kart.Input.IsLookBehindPressed;
		camNode.localPosition = _viewpoint.localPosition;

		camNode.localPosition = Vector3.Lerp(
			camNode.localPosition,
			_viewpoint.localPosition,
			Time.deltaTime * lerpFactorVP); 

		cam.transform.position = Vector3.Lerp(cam.transform.position ,camNode.position, Time.deltaTime*15);//DABROS316 camNode.position
		cam.transform.localPosition = new Vector3(cam.transform.localPosition.x+Steer, cam.transform.localPosition.y, cam.transform.localPosition.z);
		/*
		cam.transform.rotation= Quaternion.Lerp(cam.transform.rotation, 
								Quaternion.Euler(cam.transform.parent.rotation.x, cam.transform.parent.rotation.y+ Steer, cam.transform.parent.rotation.z),
								Time.deltaTime);
		*/
		cam.transform.LookAt(new Vector3(cam.transform.parent.position.x, cam.transform.parent.position.y+ angleVertical, cam.transform.parent.position.z));
		//cam.transform.localRotation = Quaternion.Euler(cam.transform.localRotation.x, cam.transform.localRotation.y - Steer, cam.transform.localRotation.z);
			//LookAt( (t=Vector3.Lerp( t,cam.transform.parent.position,Time.deltaTime* lerpRotation)));
			//cam.transform.rotation = Quaternion.Looka//Quaternion.LookRotation(camNode.forward, Vector3.up);
			//t = cam.transform.parent.position;
			//CLog.Log("333");
		SetFOV(cam);
	}
	Vector3 t;
	
	private void ControlCameraLerp(Camera cam)
	{
		//CLog.Log("111");
		cam.transform.position = Vector3.Lerp(cam.transform.position, camNode.position, Time.deltaTime * 2f);
		cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, camNode.rotation, Time.deltaTime * 2f);
		if (Vector3.Distance(cam.transform.position, camNode.position) < 0.05f &&
			Vector3.Dot(cam.transform.forward, camNode.forward) > 0.95f)
		{
			_shouldLerpCamera = false;
		}
	}

	private void SetFOV(Camera cam)
	{
		_currentFOV = useFinishVP ? finishFOV : Kart.Controller.BoostTime > 0 ? boostFOV-10 : normalFOV-5;//DABROS316
		cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, _currentFOV, Time.deltaTime * lerpFactorFOV);
	}

	private Transform GetViewpoint() 
	{
		if (Kart.Controller == null) return null;
		if (useFinishVP) return finishVP;

		if (Kart.Input.IsLookBehindPressed)
		{
			return Kart.Controller.BoostTime > 0 ? backwardBoostVP : backwardVP;
		}

		//return Kart.Controller.BoostTime > 0 ? forwardBoostVP : forwardVP;
		return forwardVP;
	}
}