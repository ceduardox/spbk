using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "New Char Definition", menuName = "Scriptable Object/Drivers Definitons")]
public class DriverDefinition : ScriptableObject
{
	//public const int MAX_STAT = 10;

	[SerializeField] private int				id;
	public string								nameDriver;
	public Sprite								iconLobby;
	public Sprite								itemIcon;
	public Sprite								itemIconOnClick;
	public Sprite								itemIconSelected;
	// agregado para las imagenes personalizadas de los personajes
	public Sprite backgroundImg;  
	public Sprite characterLogo;

	public Char_Store				prefab;
	//public Char_Store				prefabSit;
	//[SerializeField] private GameObject[]	parts;
	//[SerializeField] private Dictionary<string, string>	partsKarts;
	public List<Kart_Parts> startConfig;


	public bool crearConfigInicial()
	{
		if (PlayerDataTitle.PlayerDataTi == null)
		{
			CLog.LogWarning("PlayerDataTitle no inicializado al crear config inicial de driver " + id);
			return false;
		}

		PlayerDataTitle.PlayerDataTi.TryGetValue(id.ToString(), out System.Collections.Generic.List<PlayerD> playerData);

		if (playerData == null)
		{
			List<PlayerD> startConfigPlayerD = new List<PlayerD>();

			/*foreach (Kart_Parts _part in startConfig)
			{
				startConfigPlayerD.Add(new PlayerD(id + "-" + _part.name, _part.classPart));
			}*/
			startConfigPlayerD.Add(new PlayerD(id + "-Body_00", ClassPart.BODY));
			PlayerDataTitle.updateData(id.ToString(), startConfigPlayerD);
			return true;
		}
		return false;
	}







	//private readonly Dictionary<string, AudioClip> dictionary = new Dictionary<string, AudioClip>();
	public int Id => id;

	/*
	public int		Id			=> id;
	//public string	NameKart	=> nameKart;
	public float	SpeedStat	=> (float)speedStat / MAX_STAT;
	public float	AccelStat	=> (float)accelStat / MAX_STAT;
	public float	TurnStat	=> (float)turnStat / MAX_STAT;*/
}
