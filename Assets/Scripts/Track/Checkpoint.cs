using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	public int index = -1;
	private void Start()
	{
		if(GetComponent<FinishLine>() == null) GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Renderer>().enabled = false;
	}
	public void OnTriggerStay(Collider other)
	{
		if (other.TryGetComponent(out KartLapController kart)) { 
            kart.ProcessCheckpoint(this);
		}
	}

    bool bajar=true;
    private void Update()
    {
        if (bajar&&GetComponent<FinishLine>()==null)
        {
            Vector3 fwd = transform.TransformDirection(Vector3.down);

            if (Physics.Raycast(transform.position, fwd, 0.5f))
                bajar = false;
            //print("There is something in front of the object!");
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.05f, transform.position.z);
        }
        else //if (visuals.gameObject.activeSelf)
            enabled = false;
    }



}
