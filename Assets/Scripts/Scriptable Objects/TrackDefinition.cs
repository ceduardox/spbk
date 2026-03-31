using UnityEngine;

[CreateAssetMenu(fileName = "New Track", menuName = "Scriptable Object/Track Definition")]
public class TrackDefinition : ScriptableObject
{
	//public string trackName; 
	public UI_CODE trackName;
	//public string trackDesc;
	public UI_CODE trackDesc;
	public Sprite trackIcon;
	public int index;
	public GameObject preFab;
	public GameModes mode;
	public string trackSceneName;
}
