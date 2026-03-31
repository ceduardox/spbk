using UnityEngine;

[CreateAssetMenu(fileName = "New Kart Definition", menuName = "Scriptable Object/Kart Definiton")]
public class KartDefinition : ScriptableObject
{
	public const int	MAX_STAT = 20;
	
	public const string	PATH_PARTS_UPGRADE= "Prefabs/KartPrefabs";


	[SerializeField] private int	id;
	[SerializeField] public string		nameKart;
	public	KartEntity				prefab;
	public Sprite iconLobby;
	public	Sprite					itemIcon;
	public Sprite itemIconOnClick;
	public Sprite itemIconSelected;
	//public	Kart_Store				kartModel;
	//public	GameObject[]			parts;

	[SerializeField, Range(1, MAX_STAT)] private int speedStat;
	[SerializeField, Range(1, MAX_STAT)] private int accelStat;
	[SerializeField, Range(1, MAX_STAT)] private int turnStat;

	public System.Collections.Generic.List<Kart_Parts> startConfig;

	public bool crearConfigInicial()
	{
		if (PlayerDataTitle.PlayerDataTi == null)
		{
			CLog.LogWarning("PlayerDataTitle no inicializado al crear config inicial de kart " + id);
			return false;
		}

		PlayerDataTitle.PlayerDataTi.TryGetValue(id.ToString(), out System.Collections.Generic.List<PlayerD> playerData);

		if (playerData == null)
		{
			System.Collections.Generic.List<PlayerD> startConfigPlayerD = new System.Collections.Generic.List<PlayerD>();

			if (startConfig != null)
			{
				foreach (Kart_Parts _part in startConfig)
				{
					if (_part == null)
						continue;

					if (string.IsNullOrEmpty(_part.name))
					{
						CLog.LogWarning("Parte inicial sin nombre en kart " + id + ". Se omite.");
						continue;
					}

					startConfigPlayerD.Add(new PlayerD((_part.classPart == ClassPart.ANTENNA ? ClassPart.ALL_KARTS.ToString() : id.ToString()) + "-" + _part.name, _part.classPart));
				}
			}
			startConfigPlayerD.Add(new PlayerD(id+ "-Paint_00", ClassPart.PAINT));
			PlayerDataTitle.updateData(id.ToString(), startConfigPlayerD);
			return true;
		}
		return false;
	}

	public int		Id			=> id;
	
	//public string NameKart {  { return nameKart; } };

	public float	SpeedStat	=> (float)speedStat / MAX_STAT;
	public float	AccelStat	=> (float)accelStat / MAX_STAT;
	public float	TurnStat	=> (float)turnStat / MAX_STAT;
}
