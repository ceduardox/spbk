using UnityEngine;


public enum GameModes
{
	Race,
	DeathMatch,
	Practice,
	History,
	Tournament,
	all,
}
[CreateAssetMenu(fileName ="New Game Type", menuName = "Scriptable Object/Game Type")]
public class GameType : ScriptableObject
{
	public GameModes modeName;
	public int lapCount;
	public int lifes;
	public bool hasCoins;
	public bool hasPickups;

    public bool IsPracticeMode() => lapCount == 0;
}
