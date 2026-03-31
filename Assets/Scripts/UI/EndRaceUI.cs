using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class EndRaceUI : MonoBehaviour, GameUI.IGameUIComponent, IDisabledUI
{
  public PlayerResultItem resultItemPrefab;
	public Button continueEndButton;
	public GameObject resultsContainer;
	public bool serverInstance;
	private KartEntity _kart;

	private const float DELAY = 4;
	public void Init(KartEntity entity)
	{
		_kart = entity;
		if(continueEndButton) 
			continueEndButton.onClick.AddListener(() => LevelManager.LoadMenu());
	
	}

	public void Setup()
	{
		KartLapController.OnRaceCompleted += RedrawResultsList;
		KartEntity.OnKartSpawned += RedrawResultsList;
		KartEntity.OnKartDespawned += RedrawResultsList;
	}

	public void OnDestruction()
	{
		KartLapController.OnRaceCompleted -= RedrawResultsList;
		KartEntity.OnKartSpawned -= RedrawResultsList;
		KartEntity.OnKartDespawned -= RedrawResultsList;
	}

	public void RedrawResultsList(KartComponent updated)
	{
		
		if (serverInstance && !GameLauncher.instance.isServer)
			return;

		CLog.Log(serverInstance + " " + GameLauncher.instance.isServer);

		if (GameLauncher.ConnectionStatus != ConnectionStatus.Connected) return;

		List<KartEntity> karts;
		
		karts = GetFinishedKarts();

		if (karts != null)
		{
			if (GameLauncher.instance.isServer)
			{

				for (var i = 0; i < karts.Count; i++)
				{
					var kart = karts[i];
					if (kart != null &&
						kart.Controller.RoomUser != null)
					{
						//if(kart.LapController.GetTotalRaceTime()>30)

						GameManager.Instance.setWinner(kart.Controller.RoomUser);
					}
					CLog.Log("Position " + kart.Controller.RoomUser.Username + " " + kart.gameObject.name + " " + kart.LapController.GetTotalRaceTime() + " - " + kart.LapController.Lap);
					break;

				}

			}
			else
			{
				CLog.Log("soy: " + name);
				if (!resultsContainer) return;
				var parent = resultsContainer.transform;
				ClearParent(parent);

				for (var i = 0; i < karts.Count; i++)
				{
					var kart = karts[i];

					Instantiate(resultItemPrefab, parent)
						.SetResult(kart.Controller.RoomUser, kart.LapController.GetTotalRaceTime(), 
						GameManager.Instance.GameType.modeName==GameModes.DeathMatch? KartEntity.Karts.Count -(karts.Count-(i+1)) : i + 1);
				}

			}
			EnsureContinueButton(karts);
		}
	}
	public void outPanel()
    {
		
		CLog.Log("MANDE A OCULTAR");
		GetComponent<Animator>().Play("ResultsScreenAnimOut",0,.5f);
	}
	//ERROR--InvalidOperationException: Error when accessing GameManager.GameTypeId. Networked properties can only be accessed when Spawned() has been called.
		
		private static List<KartEntity> GetFinishedKarts() => KartEntity.Karts == null ? null :(!GameManager.Instance.Object.IsValid?null:
																GameManager.Instance.GameType.modeName==GameModes.DeathMatch ?

		KartEntity.Karts
			.OrderByDescending(x => x.LapController.GetTotalRaceTime())
			.Where(kart => kart.LapController.HasFinished)
			.ToList() 
			:
		KartEntity.Karts
			.OrderBy(x => x.LapController.GetTotalRaceTime())
			.Where(kart => kart.LapController.HasFinished)
			.ToList());


	private void EnsureContinueButton(List<KartEntity> karts)
	{
		var allFinished = karts.Count == KartEntity.Karts.Count;

		CLog.Log("Leader Value: " +name +" - "+ karts.Count + " - " + KartEntity.Karts.Count + " - " + allFinished);
		//if (allFinished&& KartEntity.Karts.Count==0)return;

			if (GameManager.Instance.GameType.modeName == GameModes.DeathMatch && (karts.Count == KartEntity.Karts.Count - 1) && KartEntity.Karts.Count > 1)
		{
			Invoke("addFinishPlayer", 1);
		}

		if (allFinished&&GameLauncher.instance.isServer)
        {
			GameManager.Instance.allFinish(karts);
        }
        else
        {
			if (RoomPlayer.Local&&
				RoomPlayer.Local.IsLeader)
			{
				continueEndButton.gameObject.SetActive(allFinished);
			}
		} 
			

    }


	public void addFinishPlayer()
    {
		foreach (var kart in KartEntity.Karts)
		{
			if (!kart.LapController.HasFinished)
			{

				kart.LapController.Lap = GameManager.Instance.GameType.lifes + 1;// EndRaceTick = kart.LapController.Runner.Simulation.Tick;
				if (!GameManager.Instance.restart)
					GameManager.Instance.restart = true;                                                                    
				//return;
			}


		}
	}

	private static void ClearParent(Transform parent)
	{
		var len = parent.childCount;
		for (var i = 0; i < len; i++)
		{
			Destroy(parent.GetChild(i).gameObject);
		}
	}
}