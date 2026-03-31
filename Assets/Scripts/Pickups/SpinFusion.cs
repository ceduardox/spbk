using UnityEngine;
using Fusion;

public class SpinFusion : NetworkBehaviour
{
	public Vector3 axis = Vector3.up;
	public float rate;
	public AnimationCurve curve;
	public bool rotate;
	//*Transform axe;
	Vector3 position;

	public void Awake()
    {
		position = transform.localPosition;

	}
	
	 public override void FixedUpdateNetwork()
	//private void Update()
    {
        
		base.FixedUpdateNetwork();
		if (rotate) transform.localRotation = Quaternion.AngleAxis(curve.Evaluate(Time.time * rate) * 360, axis);
		else transform.localPosition = (curve.Evaluate(Time.time * rate)*axis) + position;// new Vector3()//Quaternion.AngleAxis(curve.Evaluate(Time.time * rate) * 360, axis);
	}

}
