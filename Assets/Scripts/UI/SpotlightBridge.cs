using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightBridge : MonoBehaviour
{
	public string target = "";

	public void FocusIndex(int index)
	{
		if (string.IsNullOrEmpty(target))
		{			
			CLog.LogWarning("SpotlightBridge target field has not been set", this);
			return;
		}
		//CLog.Log("SOY GAME " + index);
		if (SpotlightGroup.Search(target, out SpotlightGroup spotlight))
			spotlight.FocusIndex(index,true);
	}
}
