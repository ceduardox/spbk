using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisconnectUI : MonoBehaviour
{
	public Transform parent;
	public TextMeshProUGUI disconnectStatus;
	public TextMeshProUGUI disconnectMessage;

	//public Text disconnectStatus;
	//public Text disconnectMessage;

	public bool ShowMessage( string status, string message)
	{
		if (status == null || message == null)
			return false;

		disconnectStatus.text = status;
		disconnectMessage.text = message;

		CLog.Log($"Showing message({status},{message})");
		parent.gameObject.SetActive(true);
		return true;
	}
}