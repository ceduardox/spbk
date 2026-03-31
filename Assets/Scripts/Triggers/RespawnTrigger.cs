using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
	public GameObject effect;
    private void OnEnable()
    {
        GetComponent<Renderer>().enabled = false;
    }
    private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out KartEntity kart))
		{
            if ( kart.Object.HasInputAuthority ) {
                if (effect != null)
                {
                    Instantiate(effect, kart.transform.position, kart.transform.rotation);
                }
            }
            
            if ( kart.Object.HasStateAuthority ) kart.LapController.ResetToCheckpoint();
        }
	}
}