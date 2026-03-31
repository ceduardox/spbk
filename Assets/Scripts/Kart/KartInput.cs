using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

public class KartInput : KartComponent, INetworkRunnerCallbacks
{
	private bool _inputInitialized;

	public struct NetworkInputData : INetworkInput
	{
		public const uint ButtonAccelerate = 1 << 0;
		public const uint ButtonReverse = 1 << 1;
		public const uint ButtonDrift = 1 << 2;
		public const uint ButtonLookbehind = 1 << 3;
		public const uint UseItem = 1 << 4;
		public const uint UseItem2 = 1 << 5;
		public const uint UseItem3 = 1 << 6;
		public const uint UseItem4 = 1 << 7;
		/*
		public const uint UseItem5 = 1 << 8;
		public const uint UseItem6 = 1 << 9;
		public const uint UseItem7 = 1 << 10;
		public const uint UseItem8 = 1 << 11;
		public const uint UseItem9 = 1 << 12;*/





		public uint Buttons;
		public uint OneShots;

		private int _steer;
		public float Steer
		{
			get => _steer * .001f;
			set => _steer = (int)(value * 1000);
		}

		public bool IsUp(uint button) => IsDown(button) == false;
		public bool IsDown(uint button) => (Buttons & button) == button;

		public bool IsDownThisFrame(uint button) => (OneShots & button) == button;

		public bool IsAccelerate => IsDown(ButtonAccelerate);
		public bool IsReverse => IsDown(ButtonReverse);
		public bool IsDriftPressed => IsDown(ButtonDrift);
		public bool IsDriftPressedThisFrame => IsDownThisFrame(ButtonDrift);
	}

	public Gamepad gamepad;

	[SerializeField] private InputAction accelerate;
	[SerializeField] private InputAction reverse;
	[SerializeField] private InputAction drift;
	[SerializeField] private InputAction steer;
	[SerializeField] private InputAction lookBehind;
	[SerializeField] private InputAction useItem;
	[SerializeField] private InputAction useItem2;
	[SerializeField] private InputAction useItem3;
	[SerializeField] private InputAction useItem4;
	/*[SerializeField] private InputAction useItem5;
	[SerializeField] private InputAction useItem6;
	[SerializeField] private InputAction useItem7;
	[SerializeField] private InputAction useItem8;
	[SerializeField] private InputAction useItem9;
	*/
	[SerializeField] private InputAction pause;
	

	private bool _useItemPressed;
	private bool _useItemPressed2;
	private bool _useItemPressed3;
	private bool _useItemPressed4;
	/*private bool _useItemPressed5;
	private bool _useItemPressed6;
	private bool _useItemPressed7;
	private bool _useItemPressed8;
	private bool _useItemPressed9;
	*/
	private bool _driftPressed;
	
	public override void Spawned()
	{
		base.Spawned();

		if (!Object.HasInputAuthority)
			return;

		Runner.AddCallbacks(this);

		accelerate = accelerate.Clone();
		reverse = reverse.Clone();
		drift = drift.Clone();
		steer = steer.Clone();
		lookBehind = lookBehind.Clone();
		useItem = useItem.Clone();
		useItem2 = useItem2.Clone();
		useItem3 = useItem3.Clone();
		useItem4 = useItem4.Clone();
		/*useItem5 = useItem5.Clone();
		useItem6 = useItem6.Clone();
		useItem7 = useItem7.Clone();
		useItem8 = useItem8.Clone();
		useItem8 = useItem9.Clone();*/
		pause = pause.Clone();

		accelerate.Enable();
		reverse.Enable();
		drift.Enable();
		steer.Enable();
		lookBehind.Enable();
		useItem.Enable();
		useItem2.Enable();
		useItem3.Enable();
		useItem4.Enable();
		/*useItem5.Enable();
		useItem6.Enable();
		useItem7.Enable();
		useItem8.Enable();
		useItem9.Enable();*/
		pause.Enable();

		useItem.started += UseItemPressed;
		useItem2.started += UseItemPressed2;
		useItem3.started += UseItemPressed3;
		useItem4.started += UseItemPressed4;
		/*
		useItem5.started += UseItemPressed5;
		useItem6.started += UseItemPressed6;
		useItem7.started += UseItemPressed7;
		useItem8.started += UseItemPressed8;
		useItem9.started += UseItemPressed9;
		*/
		drift.started += DriftPressed;
		pause.started += PausePressed;
		_inputInitialized = true;
	}

	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		base.Despawned(runner, hasState);

		if (_inputInitialized)
		{
			DisposeInputs();
			Runner.RemoveCallbacks(this);
			_inputInitialized = false;
		}
	}

	private void OnDestroy()
	{
		if (_inputInitialized)
		{
			DisposeInputs();
			_inputInitialized = false;
		}
	}

	private void DisposeInputs()
	{
		accelerate.Dispose();
		reverse.Dispose();
		drift.Dispose();
		steer.Dispose();
		lookBehind.Dispose();
		useItem.Dispose();
		useItem2.Dispose();
		useItem3.Dispose();
		useItem4.Dispose();
		/*
		useItem5.Dispose();
		useItem6.Dispose();
		useItem7.Dispose();
		useItem8.Dispose();
		useItem9.Dispose();
		*/
		pause.Dispose();
		// disposal should handle these
		//useItem.started -= UseItemPressed;
		//drift.started -= DriftPressed;
		//pause.started -= PausePressed;
	}

	private void UseItemPressed(InputAction.CallbackContext ctx) => _useItemPressed = true;
	private void UseItemPressed2(InputAction.CallbackContext ctx) => _useItemPressed2 = true;
	private void UseItemPressed3(InputAction.CallbackContext ctx) => _useItemPressed3 = true;
	private void UseItemPressed4(InputAction.CallbackContext ctx) => _useItemPressed4 = true;
	/*private void UseItemPressed5(InputAction.CallbackContext ctx) => _useItemPressed5 = true;
	private void UseItemPressed6(InputAction.CallbackContext ctx) => _useItemPressed6 = true;
	private void UseItemPressed7(InputAction.CallbackContext ctx) => _useItemPressed7 = true;
	private void UseItemPressed8(InputAction.CallbackContext ctx) => _useItemPressed8 = true;
	private void UseItemPressed9(InputAction.CallbackContext ctx) => _useItemPressed9 = true;
	*/
	private void DriftPressed(InputAction.CallbackContext ctx) => _driftPressed = true;

	private void PausePressed(InputAction.CallbackContext ctx)
	{
		if (Kart.Controller.CanDrive) InterfaceManager.Instance.OpenPauseMenu();
	}

	/// This isn't networked, so is not inside the <see cref="NetworkInputData"/> struct
	public bool IsLookBehindPressed => ReadBool(lookBehind);

	private static bool ReadBool(InputAction action) => action.ReadValue<float>() != 0;
	private static float ReadFloat(InputAction action) => action.ReadValue<float>();

	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
		if (!_inputInitialized || !Object || !Object.HasInputAuthority)
			return;

		gamepad = Gamepad.current;

		var userInput = new NetworkInputData();

		if (ReadBool(accelerate)) userInput.Buttons |= NetworkInputData.ButtonAccelerate;
		if (ReadBool(reverse)) userInput.Buttons |= NetworkInputData.ButtonReverse;
		if (ReadBool(drift)) userInput.Buttons |= NetworkInputData.ButtonDrift;
		if (ReadBool(lookBehind)) userInput.Buttons |= NetworkInputData.ButtonLookbehind;

		if (_driftPressed) userInput.OneShots |= NetworkInputData.ButtonDrift;
		if (_useItemPressed) userInput.OneShots |= NetworkInputData.UseItem;
		if (_useItemPressed2) userInput.OneShots |= NetworkInputData.UseItem2;
		if (_useItemPressed3) userInput.OneShots |= NetworkInputData.UseItem3;
		if (_useItemPressed4) userInput.OneShots |= NetworkInputData.UseItem4;
		/*
		if (_useItemPressed5) userInput.OneShots |= NetworkInputData.UseItem5;
		if (_useItemPressed6) userInput.OneShots |= NetworkInputData.UseItem6;
		if (_useItemPressed7) userInput.OneShots |= NetworkInputData.UseItem7;
		if (_useItemPressed8) userInput.OneShots |= NetworkInputData.UseItem8;
		if (_useItemPressed9) userInput.OneShots |= NetworkInputData.UseItem9;
		*/

		userInput.Steer = ReadFloat(steer);

		input.Set(userInput);

		_driftPressed = false;
		_useItemPressed = false; 
		_useItemPressed2 = false;
		_useItemPressed3 = false;
		_useItemPressed4 = false;
		/*
		 _useItemPressed5 = false;
		_useItemPressed6 = false;
		_useItemPressed7 = false;
		_useItemPressed8 = false;
		_useItemPressed9 = false;
		*/
	}
	
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
	public void OnConnectedToServer(NetworkRunner runner) { }
	public void OnDisconnectedFromServer(NetworkRunner runner) { }
	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
	public void OnSceneLoadDone(NetworkRunner runner) { }
	public void OnSceneLoadStart(NetworkRunner runner) { }
	
}
