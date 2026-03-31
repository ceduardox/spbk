using System.Collections;
using System.Collections.Generic;
using Fusion;
using FusionExamples.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
	public class LevelManager : NetworkSceneManagerBase
	{
		public const int LAUNCH_SCENE = 1;
		public const int LOBBY_SCENE = 2;
		public const int TRACK_SCENE = 3;
		
		[SerializeField] private UIScreen _dummyScreen;
		[SerializeField] private UIScreen _lobbyScreen;
		[SerializeField] public CanvasFader fader;

		public static LevelManager Instance => Singleton<LevelManager>.Instance;
		
		public static void LoadMenu()
		{
			if(GameManager.Instance!=null)
            {
				GameManager.Instance.setLat(-1);
			}
			Debug.Log("BORRANDO Regreso al MENU");
			Instance.Runner.SetActiveScene(LOBBY_SCENE);
			GameLauncher.instance.setOpenNet(true);
			GameLauncher.instance.resetSesion();

			//RecentPlayerLobby.instance.addListPlayerPref(); //para posterior uso
		}

		public static void LoadTrack(string sceneIndex)
		{
			Instance.Runner.SetActiveScene(sceneIndex);
			Debug.Log("SE ACTIVO LA ESCENA: " + sceneIndex);
			//Instance.Runner.CurrentScene = null;


		}

		protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
		{
			Debug.Log($"Loading scene {newScene}");

			PreLoadScene(newScene);

			List<NetworkObject> sceneObjects = new List<NetworkObject>();

			if (newScene >= LOBBY_SCENE)
			{
				yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Single);
				Scene loadedScene = SceneManager.GetSceneByBuildIndex(newScene);
				CLog.Log($"Loaded scene {newScene}: {loadedScene}");
				sceneObjects = FindNetworkObjects(loadedScene, disable: false);
			}

			finished(sceneObjects);

			// Delay one frame, so we're sure level objects has spawned locally
			yield return null;


			// Now we can safely spawn karts
			if (GameManager.CurrentTrack != null && newScene > LOBBY_SCENE)
			{
					if ((Runner.GameMode == GameMode.Server && RoomPlayer.Local == null)|| (Runner.GameMode == GameMode.Host))//----if (Runner.GameMode == GameMode.Host)
					{   //Recorremos todos los Kart
						foreach (var player in RoomPlayer.Players)
						{
						try
						{
							player.GameState = RoomPlayer.EGameState.GameCutscene;
							GameManager.CurrentTrack.SpawnPlayer(Runner, player);
						}
						catch (System.Exception e)
						{
							CLog.LogError("Error spawneando jugador en pista: " + e.Message);
						}
					}
				}
			}

			PostLoadScene();
		}

		private void PreLoadScene(int scene)
		{
			if (scene > LOBBY_SCENE)
			{
				// Show an empty dummy UI screen - this will stay on during the game so that the game has a place in the navigation stack. Without this, Back() will break
				CLog.Log("Showing Dummy");
				if (_lobbyScreen != null && _lobbyScreen.gameObject.activeSelf)
					_lobbyScreen.gameObject.SetActive(false);
				if (LobbyUI._instance != null)
					LobbyUI._instance.pararContador();
				UIScreen.Focus(_dummyScreen);
			}
			else if(scene==LOBBY_SCENE)
			{
				foreach (RoomPlayer player in RoomPlayer.Players)
				{
					player.IsReady = false;
				}
				UIScreen.activeScreen.BackTo(_lobbyScreen);
			}
			else
			{
				UIScreen.BackToInitial();
				//GameLauncher.instance.mainScreen.BackTo(GameLauncher.instance.mainScreen);

			}
			fader.gameObject.SetActive(true);
			fader.FadeIn();
		}
	
		private void PostLoadScene()
		{
			fader.FadeOut();
		}
	}
}
