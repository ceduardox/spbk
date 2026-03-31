using UnityEngine;
using Fusion;
using System.Collections;

public class BarrilesFusion : NetworkBehaviour
{
	//public Vector3 axis = Vector3.up;
	public float rate;
	//public AnimationCurve curve;
	//public bool rotate;
	//*Transform axe;
	public Vector3 position;
	public Quaternion rotation;
	public NetworkRigidbody _nrb;
	public float respawnTime;
	public void Awake()
    {
		position = transform.position;
		rotation = transform.rotation;
		_nrb = GetComponent<NetworkRigidbody>();
		StartCoroutine(respawn());
	}

	IEnumerator respawn()
    {
		yield return new WaitForSeconds(respawnTime);

		while (Object.HasStateAuthority)
        {
			_nrb.TeleportToPositionRotation(position, rotation);
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			yield return new WaitForSeconds(respawnTime);
			


		}
	}


}
