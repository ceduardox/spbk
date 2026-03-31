using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[OrderAfter(typeof(NetworkRigidbody))]
public class KartController : KartComponent
{
	public new CapsuleCollider collider;
	public DriftTier[] driftTiers;
	[SerializeField] private Axis tireYawAxis = Axis.Y;

	public Transform model;
	public Transform tireFL, tireFR, tireYawFL, tireYawFR, tireBL, tireBR;
	[SerializeField] private Axis spineYawAxis = Axis.X;
	public Transform spine;
	public bool inverseSpine;
	public float maxSpeedNormal;
	public float maxSpeedBoosting;
	public float reverseSpeed;
	public float acceleration;
	public float deceleration;
	internal float decelerationPowerUps=1; 

	[Tooltip("X-Axis: steering\nY-Axis: velocity\nCoordinate space is normalized")]
	public AnimationCurve steeringCurve = AnimationCurve.Linear(0, 0, 1, 1);

	public float maxSteerStrength = 25;
	public float steerAcceleration;
	public float steerDeceleration;
	public Vector2 driftInputRemap = new Vector2(0.5f, 1f);
	public float hopSteerStrength;
	public float speedToDrift=7;
	public float driftRotationLerpFactor = 10f;

	public Rigidbody Rigidbody;

	public bool IsBumped => !BumpTimer.ExpiredOrNotRunning(Runner);
	public bool IsBackfire => !BackfireTimer.ExpiredOrNotRunning(Runner);
	public bool IsHopping => !HopTimer.ExpiredOrNotRunning(Runner);
	public bool CanDrive => HasStartedRace && !HasFinishedRace && !IsBumped && !IsBackfire && IsImpact == -1;//!IsSpinout && !IsMissil && !IsRayo && !IsGhost;
	public bool HasFinishedRace => Kart.LapController.EndRaceTick != 0;
	public bool HasStartedRace => Kart.LapController.StartRaceTick != 0;
	public float BoostTime => BoostEndTick == -1 ? 0f : (BoostEndTick - Runner.Simulation.Tick) * Runner.DeltaTime;
	public float RealSpeed => transform.InverseTransformDirection(Rigidbody.velocity).z;
	public bool IsDrifting => IsDriftingLeft || IsDriftingRight;
	public bool IsBoosting => BoostTierIndex != 0;
	public bool IsOffroad => IsGrounded && GroundResistance >= 0.2f;
	public float DriftTime => (Runner.Simulation.Tick - DriftStartTick) * Runner.DeltaTime;

	[Networked] public float MaxSpeed { get; set; }

	[Networked(OnChanged = nameof(OnBoostTierIndexChangedCallback))]
	public int BoostTierIndex { get; set; }

	[Networked] public TickTimer BoostpadCooldown { get; set; }

	[Networked(OnChanged = nameof(OnDriftTierIndexChangedCallback))]
	public int DriftTierIndex { get; set; } = -1;

	[Networked] public NetworkBool IsGrounded { get; set; }
	[Networked] public float GroundResistance { get; set; }
	[Networked] public int BoostEndTick { get; set; } = -1;


	[Networked(OnChanged = nameof(OnImpactChangedCallback))]
	public short IsImpact { get; set; }

	/*
	[Networked(OnChanged = nameof(OnSpinoutChangedCallback))]
	public NetworkBool IsSpinout { get; set; }

	public bool isTornado;

	[Networked(OnChanged = nameof(OnMissilChangedCallback))]
	public NetworkBool IsMissil { get; set; }

	[Networked(OnChanged = nameof(OnRayoChangedCallback))]
	public NetworkBool IsRayo { get; set; }

	[Networked(OnChanged = nameof(OnGhostChangedCallback))]
	public NetworkBool IsGhost { get; set; }
	[Networked(OnChanged = nameof(OnBombChangedCallback))]
	
	 */
	 /*
	public NetworkBool IsBomb { get; set; }

	[Networked(OnChanged = nameof(OnTroncoChangedCallback))]
	public NetworkBool IsTronco { get; set; }
	[Networked(OnChanged = nameof(OnSkullChangedCallback))]
	public NetworkBool IsSkull { get; set; }
	[Networked(OnChanged = nameof(OnOilChangedCallback))]
	public NetworkBool IsOil { get; set; }*/

	[Networked] public float TireYaw { get; set; }
	[Networked] public RoomPlayer RoomUser { get; set; }
	[Networked] public NetworkBool IsDriftingLeft { get; set; }
	[Networked] public NetworkBool IsDriftingRight { get; set; }
	[Networked] public int DriftStartTick { get; set; }

	[Networked(OnChanged = nameof(OnIsBackfireChangedCallback))]
	public TickTimer BackfireTimer { get; set; }

	[Networked(OnChanged = nameof(OnIsBumpedChangedCallback))]
	public TickTimer BumpTimer { get; set; }

	[Networked(OnChanged = nameof(OnIsHopChangedCallback))]
	public TickTimer HopTimer { get; set; }

	[Networked] public float AppliedSpeed { get; set; }

	[Networked] public KartInput.NetworkInputData Inputs { get; set; }

	public event Action<int> OnDriftTierIndexChanged;
	public event Action<int> OnBoostTierIndexChanged;
	public event Action<bool> OnImpactChanged;///////////////////////////////////////////////////
	//public event Action<bool> OnSpinoutChanged;
	//public event Action<bool> OnMissilChanged;
	//public event Action<bool> OnRayoChanged;
	//public event Action<bool> OnGhostChanged;

	/*public event Action<bool> OnBombChanged;
	public event Action<bool> OnTroncoChanged;
	public event Action<bool> OnSkullChanged;
	public event Action<bool> OnOilChanged;*/
	public event Action<bool> OnBumpedChanged;
	public event Action<bool> OnHopChanged;
	public event Action<bool> OnBackfiredChanged;

	[Networked] private float SteerAmount { get; set; }
	[Networked] private int AcceleratePressedTick { get; set; }
	[Networked] private bool IsAccelerateThisFrame { get; set; }

	public Kart_Store kart_Parts;
	[Header("Bot AI (Safe)")]
		[SerializeField] private bool enableSimpleBotDrive = true;
		[SerializeField, Range(0.5f, 4f)] private float botSteerMultiplier = 2.1f;
		[SerializeField, Range(0f, 0.4f)] private float botSteerWobble = 0.02f;
		[SerializeField] private bool enableBotSteerWobble = false;
		[SerializeField] private bool enableBotLaneRouting = true;
		[SerializeField, Range(0.5f, 3f)] private float botLaneHalfWidth = 1.2f;
		[SerializeField, Range(2, 12)] private int botLaneSubdivisionsPerSegment = 7;
		[SerializeField, Range(0f, 3f)] private float botPathOffsetRadius = 0.95f;
		[SerializeField] private int botLookAheadCheckpoints = 1;
		[SerializeField] private float botPathLookAheadMin = 9f;
		[SerializeField] private float botPathLookAheadMax = 18f;
		[SerializeField, Range(0f, 1f)] private float botCurveBrakeStart = 0.42f;
		[SerializeField, Range(0f, 1f)] private float botCurveBrakeSpeed01 = 0.62f;
		[SerializeField, Range(0f, 1f)] private float botCurveHardBrakeSpeed01 = 0.76f;
		[SerializeField, Range(1f, 2.5f)] private float botCurveSteerBoost = 1.95f;
		[SerializeField] private bool enableBotAutoDrift = true;
		[SerializeField, Range(0f, 1f)] private float botDriftCurveStart = 0.44f;
		[SerializeField, Range(0f, 1f)] private float botDriftMinSpeed01 = 0.3f;
		[SerializeField, Range(0f, 1f)] private float botDriftSteerThreshold = 0.26f;
		[SerializeField] private int botDriftStartGuardTicks = 10;
		[SerializeField] private float botFrontRayDistance = 8f;
		[SerializeField] private float botSideRayDistance = 6f;
		[SerializeField, Range(0.5f, 2.5f)] private float botAvoidSteerStrength = 1.35f;
		[SerializeField] private int botStuckTicksThreshold = 45;
		[SerializeField] private int botReverseTicks = 16;
		[SerializeField] private int botReverseReentryDelayTicks = 90;
		[SerializeField] private int botReverseFromBlockMinStillTicks = 14;
		[SerializeField] private float botReverseDotThreshold = -0.12f;
		[SerializeField] private float botMinMoveSqrPerTick = 0.0008f;
		private bool _safeLaneModeEnabled;
		private float _safeLaneMaxSpeed = 11f;
		private float _safeLaneSteerMultiplier = 1.8f;
		private bool _safeLaneDisableDrift = true;
		private bool _safeLaneDisableSteerWobble = true;
		private float _safeLaneAvoidMultiplier = 1.2f;
		private float _safeLaneThrottleStart01 = 0.82f;
		private float _safeLaneThrottleStop01 = 0.97f;
		private bool _autoComputeSafeLaneFromTrack = true;
		private float _autoComputeInfluence = 0.85f;
		private Vector3 _botLastPos;
		private int _botStillTicks;
		private int _botReverseTicksLeft;
		private int _botReverseCooldownTicks;
		private int _botRecoveryForwardTicksLeft;
		private float _botRecoverySteerSign = 1f;
		private int _botSeed;
	private bool _botSeedInitialized;
		private int _botLaneIndex = -1;
		private int _botLaneProgressIndex = -1;
			private int _botDriftStartGuardLeft = 0;
			private int _botCurveSetupTicksLeft = 0;
			private float _botCurveSetupSteerValue = 0f;
			private int _botCurveManeuverTicksLeft = 0;
			private float _botCurveManeuverSteerValue = 0f;
			private float _botCurveManeuverSharpness = 0f;
			private bool _botCurveModeActive = false;
			private float _botCurveModeSteerSign = 0f;
			private int _botCurveModeExitTicks = 0;
			private int _botCurveDriftIntentTicksLeft = 0;
			private int _botLastDriftDebugTick = -10000;
			private static int _cachedTrackForBotLanes = int.MinValue;
			private static List<Vector3>[] _cachedBotLanes;
			private BotAITrackSettings _cachedBotTrackSettings;
		private int _cachedBotTrackSettingsTrackId = int.MinValue;
		private int _botRuntimeConfigRevision = -1;
		private int _botAutoTuneTrackId = int.MinValue;
		private int _botManualDriftPathHoldTicks = 0;
		private BotDriftPathTool _cachedBotDriftPathTool;
		private int _cachedBotDriftPathToolTrackId = int.MinValue;
		private static void OnIsBackfireChangedCallback(Changed<KartController> changed) =>
			changed.Behaviour.OnBackfiredChanged?.Invoke(changed.Behaviour.IsBackfire);

	private static void OnIsBumpedChangedCallback(Changed<KartController> changed) =>
		changed.Behaviour.OnBumpedChanged?.Invoke(changed.Behaviour.IsBumped);

	private static void OnIsHopChangedCallback(Changed<KartController> changed) =>
		changed.Behaviour.OnHopChanged?.Invoke(changed.Behaviour.IsHopping);

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private static void OnImpactChangedCallback(Changed<KartController> changed) =>
		changed.Behaviour.OnImpactChanged?.Invoke(changed.Behaviour.IsImpact != -1); 
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	/*
	private static void OnSpinoutChangedCallback(Changed<KartController> changed) =>
		changed.Behaviour.OnSpinoutChanged?.Invoke(changed.Behaviour.IsSpinout);

	private static void OnMissilChangedCallback(Changed<KartController> changed) =>
	changed.Behaviour.OnMissilChanged?.Invoke(changed.Behaviour.IsMissil);

	private static void OnRayoChangedCallback(Changed<KartController> changed) =>
	changed.Behaviour.OnRayoChanged?.Invoke(changed.Behaviour.IsRayo);

	private static void OnGhostChangedCallback(Changed<KartController> changed) =>
	changed.Behaviour.OnGhostChanged?.Invoke(changed.Behaviour.IsGhost);
	*/
	/*
	private static void OnBombChangedCallback(Changed<KartController> changed) =>
	changed.Behaviour.OnBombChanged?.Invoke(changed.Behaviour.IsBomb);
	private static void OnTroncoChangedCallback(Changed<KartController> changed) =>
	changed.Behaviour.OnTroncoChanged?.Invoke(changed.Behaviour.IsTronco);
	private static void OnSkullChangedCallback(Changed<KartController> changed) =>
	changed.Behaviour.OnSkullChanged?.Invoke(changed.Behaviour.IsSkull);
	private static void OnOilChangedCallback(Changed<KartController> changed) =>
	changed.Behaviour.OnOilChanged?.Invoke(changed.Behaviour.IsOil);
	*/

	private static void OnDriftTierIndexChangedCallback(Changed<KartController> changed) =>
		changed.Behaviour.OnDriftTierIndexChanged?.Invoke(changed.Behaviour.DriftTierIndex);

	private static void OnBoostTierIndexChangedCallback(Changed<KartController> changed) =>
		changed.Behaviour.OnBoostTierIndexChanged?.Invoke(changed.Behaviour.BoostTierIndex);


		public bool forceBrake;
		[Header("Collision Bump Tuning")]
		// Legacy kart-vs-kart bump lock was 0.4f seconds.
		[Tooltip("Duracion de bloqueo por choque entre karts. Legacy: 0.4f")]
		[SerializeField, Range(0.05f, 1.5f)] private float kartCollisionBumpSeconds = 0.16f;
		[Tooltip("Diferencia minima de velocidad para activar bump entre karts. Evita stun por roce leve.")]
		[SerializeField, Range(0f, 10f)] private float kartCollisionMinRelativeSpeed = 2.2f;
		// Legacy wall impact tuning:
		// minDot=0.25f, forceScale=0.25f, timeScale=0.8f.
		[Tooltip("Dot minimo para impacto contra pared. Legacy: 0.25f")]
		[SerializeField, Range(0.05f, 1f)] private float wallBumpMinDot = 0.14f;
		[Tooltip("Escala de fuerza por impacto contra pared. Legacy: 0.25f")]
		[SerializeField, Range(0.05f, 0.6f)] private float wallBumpForceScale = 0.11f;
		[Tooltip("Escala de tiempo de bump al chocar pared. Legacy: 0.8f")]
		[SerializeField, Range(0.1f, 1.5f)] private float wallBumpTimeScale = 0.38f;

		private void Awake()
		{
			collider = GetComponent<CapsuleCollider>();
			kart_Parts = GetBehaviour<Kart_Store>(); 
		
	}

	public override void Spawned()
	{
		base.Spawned();

		MaxSpeed = maxSpeedNormal;
		IsImpact = -1;
			_botLastPos = transform.position;
			_botStillTicks = 0;
			_botReverseTicksLeft = 0;
			_botReverseCooldownTicks = 0;
			_botRecoveryForwardTicksLeft = 0;
			_botRecoverySteerSign = 1f;
			_botSeedInitialized = false;
			_botSeed = 0;
			_botLaneIndex = -1;
			_botLaneProgressIndex = -1;
			_botDriftStartGuardLeft = 0;
			_botCurveSetupTicksLeft = 0;
			_botCurveSetupSteerValue = 0f;
			_botCurveManeuverTicksLeft = 0;
			_botCurveManeuverSteerValue = 0f;
			_botCurveManeuverSharpness = 0f;
			_botCurveModeActive = false;
			_botCurveModeSteerSign = 0f;
			_botCurveModeExitTicks = 0;
			_botCurveDriftIntentTicksLeft = 0;
			_botLastDriftDebugTick = -10000;
			_botAutoTuneTrackId = int.MinValue;
		}

	public void setStats(float _speed, float _acc, float _turn)
    {
		MaxSpeed = maxSpeedNormal = _speed;
		acceleration = deceleration = _acc;
		steerAcceleration = steerDeceleration = _turn;

	}
	bool ciclos = false;
	bool ciclos2 = false;
	private void Update()
	{
		if (ciclos = !ciclos) return;
		

		GroundNormalRotation();
		UpdateTireRotation();

		if (Object.HasInputAuthority && CanDrive)
		{
			if (Kart.Input.gamepad != null)
			{
				Kart.Input.gamepad.SetMotorSpeeds(IsOffroad ? AppliedSpeed / MaxSpeed : 0, 0);
			}
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		//
		// OnCollisionEnter and OnCollisionExit are not reliable when trying to predict collisions, however we can
		// use OnCollisionStay reliably. This means we have to make sure not to run code every frame
		//

		var layer = collision.gameObject.layer;

		// We don't want to run any of this code if we're already in the process of bumping
		if (IsBumped) return;
		if (damage) return;
			//CLog.Log("TAGO ES: " + collision.gameObject.tag);
			if (layer == GameManager.GroundLayer&& collision.gameObject.tag.Contains("Unt")) return;

		if (layer == GameManager.KartLayer && collision.gameObject.TryGetComponent(out KartEntity otherKart))
		{
			//
			// Collision with another kart - if we are going slower than them, then we should bump!  
			//

				float relativeSpeed = otherKart.Controller.AppliedSpeed - AppliedSpeed;
				if (relativeSpeed > kartCollisionMinRelativeSpeed)
				{
					BumpTimer = TickTimer.CreateFromSeconds(Runner, kartCollisionBumpSeconds);
				}
			}
			else 
			{
			//
			// Collision with a wall of some sort - We should get the angle impact and apply a force backwards, only if 
			// we are going above 'speedToDrift' speed.
			//
			if (RealSpeed > speedToDrift) 
			{

				//#if UNITY_EDITOR


				ContactPoint t;
				Debug.Log("+:" + collision.gameObject.name + " " + (t = collision.contacts[0]) + " " + damage);
				calculateDmage(t.point, collision.gameObject);

	//#endif
					var contact = collision.GetContact(0);
					var dot = Mathf.Max(wallBumpMinDot, Mathf.Abs(Vector3.Dot(contact.normal, Rigidbody.transform.forward))) * wallBumpForceScale;
					Rigidbody.AddForceAtPosition(contact.normal * AppliedSpeed * dot, contact.point, ForceMode.VelocityChange);

					BumpTimer = TickTimer.CreateFromSeconds(Runner, wallBumpTimeScale * dot);
					//damage = true;
					//Invoke("setDamage", 0.8f * dot);
				}
			}
	}

	bool damage = false;
	void setDamage()
	{
		damage = false;
		objectLast = null;
	}
	GameObject objectLast;
	Transform part;
	public void calculateDmage(Vector3 _colision, GameObject _object)
    {
		return;
		if (_object == objectLast) return;
		objectLast = _object;
		foreach (Kart_Parts p in kart_Parts.parts)
		{


			if (p.active)
			{


				if ((p.damage += (1 - (p.render() - _colision).magnitude)) > 1) 
				{
					CLog.Log("+ Analizando: " + p);
					p.returnPart().transform.parent = null;
					p.active = false;
					p.returnPart().gameObject.layer = 7;//""World collider
					p.returnPart().gameObject.AddComponent<BoxCollider>();
					if (p.returnPart().gameObject.GetComponent<Rigidbody>())
						p.returnPart().gameObject.GetComponent<Rigidbody>().drag = 20;
						else 
						p.returnPart().gameObject.AddComponent<Rigidbody>().drag=20;  

				}
			}
			CLog.Log("+" + p + ": " + p.damage);
		}
		damage = true;

		Invoke("setDamage", 0.1f);

	}
	public override void FixedUpdateNetwork()
	{
		base.FixedUpdateNetwork();

		if (GetInput(out KartInput.NetworkInputData input))
		{
			//
			// Copy our inputs that we have received, to a [Networked] property, so other clients can predict using our
			// tick-aligned inputs. This is the core of the Client Prediction system.
			//
			Inputs = input;
			
		}
		else
		{
			if (ShouldDriveBot())
			{
				Inputs = BuildBotInput();
			}
			else if (ciclos2 = !ciclos2) return;
		}
		//CLog.Log("SOY: " +Kart.gameObject);
		if (CanDrive)
			Move(Inputs);
		else
			RefreshAppliedSpeed();

		HandleStartRace();
		SpinOut(Inputs);
		Boost(Inputs);
		Drift(Inputs);
		Steer(Inputs);
		UpdateTireYaw(Inputs);
		UseItems(Inputs);


		//CLog.Log("ESTOY " + Kart.HeldItemIndex2 + " " + Kart.powerUp1.ExpiredOrNotRunning(Runner)+" "+RoomUser.Username);

		if(Kart.HeldItemIndex2==-1)
        {
			//CLog.Log("Time"+Kart.powerUp1.RemainingTime(Runner));
			if (Kart.powerUp1.ExpiredOrNotRunning(Runner))
			{
				Kart.SetHeldItem2(0);
				userItem2 = true;
			}
		}
		if (Kart.HeldItemIndex3 == -1)
		{
			if (Kart.powerUp2.ExpiredOrNotRunning(Runner))
			{
				Kart.SetHeldItem3(0);
				userItem3 = true;
			}
		}


	}
	internal bool userItem2 = true;
	internal bool userItem3 = true;

	//USE ITEM
	private void UseItems(KartInput.NetworkInputData inputs)
	{
		if (inputs.IsDownThisFrame(KartInput.NetworkInputData.UseItem))
		{
			CLog.Log("ITEM");
			Kart.Items.UseItem();
			//GetComponent<KartLapController>().Lap++;
		}
		if (inputs.IsDownThisFrame(KartInput.NetworkInputData.UseItem2))//ADDED ITEM 2
		{
			CLog.Log("ITEM2");
			Kart.Items.UseItem2();



		}
		if (inputs.IsDownThisFrame(KartInput.NetworkInputData.UseItem3))//ADDED ITEM 3
		{
			CLog.Log("ITEM3");
			Kart.Items.UseItem3();

		}	
		if (inputs.IsDownThisFrame(KartInput.NetworkInputData.UseItem4))//ADDED ITEM 3
		{
			CLog.Log("ITEM4");
			GetComponent<Kart_Store>().driver.GetChild(0).GetComponent<Char_Store>().playMele();

		}
	}

		private void HandleStartRace()
		{
			if (HasStartedRace || Track.Current == null)
				return;

			bool raceStarted = false;
			try
			{
				if (Track.Current.Object != null && Track.Current.Object.IsValid)
					raceStarted = Track.Current.StartRaceTimer.Expired(Runner);
			}
			catch (System.InvalidOperationException)
			{
				// Track exists but its networked state is not spawned yet.
				return;
			}

			if (!raceStarted)
				return;

			var components = GetComponentsInChildren<KartComponent>();
			foreach (var component in components) component.OnRaceStart();
		}

	/// <summary>
	/// Handling spinout at the start of the race. We record the tick that we last pressed the Accelerate button down,
	/// and then calculate how long we have been pressing that button elsewhere.
	/// </summary>
	/// <param name="input"></param>
	private void SpinOut(KartInput.NetworkInputData input)
	{
		var isAccelerate = input.IsDown(KartInput.NetworkInputData.ButtonAccelerate);

		if (isAccelerate && !IsAccelerateThisFrame)
		{
			AcceleratePressedTick = Runner.Simulation.Tick;
		}

		if (AcceleratePressedTick != -1 && !isAccelerate)
		{
			AcceleratePressedTick = -1;
		}

		IsAccelerateThisFrame = isAccelerate;
	}

	public override void OnRaceStart()
	{
		base.OnRaceStart();

		if (Object.HasInputAuthority)
		{
			AudioManager.PlayMusic(Track.Current.music);
		}

		//
		// If the acceleration button is held down OnRaceStart, then we can apply either a boost (if they were quick
		// enough), or stall them (if they were too slow!)
		//
		if (AcceleratePressedTick != -1)
		{
			var tickDiff = Runner.Simulation.Tick - AcceleratePressedTick;
			var time = tickDiff * Runner.DeltaTime;

			if (time < 0.15f)
				GiveBoost(false);
			else if (time < 0.3f)
			{
				BackfireTimer = TickTimer.CreateFromSeconds(Runner, 0.8f);
			}
		}
	}

		private void Move(KartInput.NetworkInputData input)
		{
			if (input.IsAccelerate)
			{
				// Bots need stronger forward acceleration so they can recover speed on ramps.
				if (ShouldDriveBot())
					AppliedSpeed = AppliedSpeed + (MaxSpeed - AppliedSpeed) * acceleration * Runner.DeltaTime;
				else
					AppliedSpeed = AppliedSpeed + (MaxSpeed - AppliedSpeed) * Runner.DeltaTime;
			}
			else if (input.IsReverse)
			{
			//AppliedSpeed = Mathf.Lerp(AppliedSpeed, -reverseSpeed, acceleration * Runner.DeltaTime);
			AppliedSpeed = AppliedSpeed+(-reverseSpeed - AppliedSpeed)* acceleration * Runner.DeltaTime;
		}
		else
		{
			//AppliedSpeed = Mathf.Lerp(AppliedSpeed, 0, deceleration * Runner.DeltaTime);
			AppliedSpeed = AppliedSpeed + (0 - AppliedSpeed) * deceleration * Runner.DeltaTime;
		}

		var resistance = 1 - (IsGrounded ? GroundResistance : 0);
		if (resistance < 1)
		{
			//AppliedSpeed = Mathf.Lerp(AppliedSpeed, AppliedSpeed * resistance, Runner.DeltaTime * (IsDrifting ? 8 : 2));
			AppliedSpeed = AppliedSpeed + (AppliedSpeed * resistance - AppliedSpeed) * Runner.DeltaTime * (IsDrifting ? 8 : 2);
		}

		// transform.forward is not reliable when using NetworkedRigidbody - instead use: NetworkRigidbody.Rigidbody.rotation * Vector3.forward
		var vel = (Rigidbody.rotation * Vector3.forward) * AppliedSpeed;
		vel.y = Rigidbody.velocity.y;
		Rigidbody.velocity = vel * decelerationPowerUps;
	}

	public void lerp(float a, float b, float time)
    {
		float lerp = a + (b - a) * time;
    }

	private void Steer(KartInput.NetworkInputData input)
	{
		var steerTarget = GetSteerTarget(input);

		if (SteerAmount != steerTarget)
		{
			var steerLerp = Mathf.Abs(SteerAmount) < Mathf.Abs(steerTarget) ? steerAcceleration : steerDeceleration;
			SteerAmount = Mathf.Lerp(SteerAmount, steerTarget, Runner.DeltaTime * steerLerp);
		}

		if (IsDrifting)
		{
			model.localEulerAngles = LerpAxis(Axis.Y, model.localEulerAngles, SteerAmount * 2,
				driftRotationLerpFactor * Runner.DeltaTime);
		}
		else
		{
			model.localEulerAngles = LerpAxis(Axis.Y, model.localEulerAngles, 0, 6 * Runner.DeltaTime);
		}

		if (CanDrive)
		{
			var rot = Quaternion.Euler(
				Vector3.Lerp(
					Rigidbody.rotation.eulerAngles,
					Rigidbody.rotation.eulerAngles + Vector3.up * SteerAmount,
					3 * Runner.DeltaTime)
			);

			Rigidbody.MoveRotation(rot);
		}
	}

	private float GetSteerTarget(KartInput.NetworkInputData input)
	{
		var steerFactor = steeringCurve.Evaluate(Mathf.Abs(RealSpeed) / maxSpeedNormal) * maxSteerStrength *
						  Mathf.Sign(RealSpeed);

		if (IsHopping && RealSpeed < speedToDrift)
			return input.Steer * hopSteerStrength;

		if (IsDriftingLeft)
			return Remap(input.Steer, -1, 1, -driftInputRemap.y, -driftInputRemap.x) * maxSteerStrength;
		if (IsDriftingRight)
			return Remap(input.Steer, -1, 1, driftInputRemap.x, driftInputRemap.y) * maxSteerStrength;

		return input.Steer * steerFactor;
	}

	private void Drift(KartInput.NetworkInputData input)
	{
		var startDrift = input.IsDriftPressedThisFrame && CanDrive && !IsDrifting;
		if (startDrift && IsGrounded)
		{
			StartDrifting(input);
			DriftStartTick = Runner.Simulation.Tick;
			HopTimer = TickTimer.CreateFromSeconds(Runner, 0.367f);
		}

		if (IsDrifting)
		{
			if (!input.IsDriftPressed || RealSpeed < speedToDrift)
			{
				StopDrifting();
			}
			else if (IsGrounded)
			{
				EvaluateDrift(DriftTime, out var index);
				if (DriftTierIndex != index) DriftTierIndex = index;
			}
		}
	}

	/// <summary>
	/// Handles when a boost is applied.
	/// </summary>
	/// <param name="input"></param>
	private void Boost(KartInput.NetworkInputData input)
	{
		if (BoostTime > 0)
		{
			MaxSpeed = maxSpeedBoosting*0.7f;
			//AppliedSpeed = Mathf.Lerp(AppliedSpeed, MaxSpeed, Runner.DeltaTime);
			AppliedSpeed = AppliedSpeed+(MaxSpeed-AppliedSpeed)* Runner.DeltaTime;

		}
		else if (BoostEndTick != -1)
		{
			StopBoosting();
		}
	}

	/// <summary>
	/// This corrects the kart visuals to the ground normal so the edges of the kart dont clip into the floor
	/// </summary>
	private void GroundNormalRotation()
	{
		var wasOffroad = IsOffroad;

		/*IsGrounded = Physics.SphereCast(collider.transform.TransformPoint(collider.center), collider.radius - 0.1f,
										Vector3.down, out var hit, 0.3f, ~LayerMask.GetMask("Kart"));		*/
		IsGrounded = Physics.SphereCast(collider.transform.TransformPoint(collider.center),  collider.radius - 0.1f,
										Vector3.down, out var hit, 0.3f, ~LayerMask.GetMask("Kart"));

		if (IsGrounded)
		{
			Debug.DrawRay(hit.point, hit.normal, Color.magenta);
			GroundResistance = hit.collider.material.dynamicFriction;

			model.transform.rotation = Quaternion.Lerp(
				model.transform.rotation,
				Quaternion.FromToRotation(model.transform.up * 2, hit.normal) * model.transform.rotation,
				7.5f * Time.deltaTime);
		}

		if (wasOffroad != IsOffroad)
		{
			if (IsOffroad)
				Kart.Animator.PlayOffroad();
			else
				Kart.Animator.StopOffroad();
		}
	}

	private void UpdateTireYaw(KartInput.NetworkInputData input)
	{
		TireYaw = input.Steer * maxSteerStrength;
	}

	private void UpdateTireRotation()
	{
		tireYawFL.localEulerAngles = LerpAxis(tireYawAxis, tireYawFL.localEulerAngles, TireYaw, 5 * Time.deltaTime);
		tireYawFR.localEulerAngles = LerpAxis(tireYawAxis, tireYawFR.localEulerAngles, TireYaw, 5 * Time.deltaTime);

		if (spine)
		{
			//Rotacion del Personaje
			spine.localEulerAngles = LerpAxis(Axis.Z, spine.localEulerAngles, (inverseSpine ? 1 : -1) * TireYaw, 2 * Time.deltaTime);//DABROS316
			//Rotacion del Kart
			Kart.Animator.transform.parent.localEulerAngles = LerpAxis(Axis.Z, Kart.Animator.transform.parent.localEulerAngles, (inverseSpine ? 1 : -1) * TireYaw*3, 1.5f * Time.deltaTime);//DABROS316
		}


		if (CanDrive)
		{
			tireFL.Rotate(90 * Time.deltaTime * AppliedSpeed * 0.75f, 0, 0);
			tireFR.Rotate(90 * Time.deltaTime * AppliedSpeed * 0.75f, 0, 0);
			tireBL.Rotate(90 * Time.deltaTime * AppliedSpeed * 0.75f, 0, 0);
			tireBR.Rotate(90 * Time.deltaTime * AppliedSpeed * 0.75f, 0, 0);
		}
	}

	// One-Shot Functions

		private void StartDrifting(KartInput.NetworkInputData input)
		{
			float driftStartSpeed = speedToDrift;
			if (ShouldDriveBot())
				driftStartSpeed = 0f;

			if (AppliedSpeed < driftStartSpeed || input.Steer == 0)
			{
				if (ShouldDriveBot() && input.IsDriftPressedThisFrame)
				{
					BotDriftDebug($"StartDrifting rechazado | grounded={IsGrounded} speed={AppliedSpeed:F2} need={driftStartSpeed:F2} steer={input.Steer:F2}");
				}
				StopDrifting();
				return;
			}

		IsDriftingRight = input.Steer > 0f;
		IsDriftingLeft = input.Steer < 0f;
	}

	private void StopDrifting()
	{
		BoostTierIndex = DriftTierIndex == -1 ? 0 : DriftTierIndex;
		BoostEndTick = BoostTierIndex == 0
			? -1
			: Runner.Simulation.Tick +
			  (int)(driftTiers[BoostTierIndex].boostDuration / Runner.Simulation.DeltaTime);

		if (BoostTime <= 0) StopBoosting();

		DriftStartTick = -1;
		DriftTierIndex = -1;
		IsDriftingLeft = false;
		IsDriftingRight = false;
	}

	private void StopBoosting()
	{
		BoostTierIndex = 0;
		BoostEndTick = -1;
		MaxSpeed = maxSpeedNormal;
	}

	public void GiveBoost(bool isBoostpad, int tier = 1)
	{
		if (isBoostpad)
		{
			//
			// If we are given a boost from a boostpad, we need to add a cooldown to ensure that we dont get a boost
			// every frame we are in contact with the boost pad.
			// 
			if (!BoostpadCooldown.ExpiredOrNotRunning(Runner))
				return;

			BoostpadCooldown = TickTimer.CreateFromSeconds(Runner, 4f);
		}

		// set the boost tier to 'tier' only if it's a higher tier than current
		BoostTierIndex = BoostTierIndex > tier ? BoostTierIndex : tier;

		if (BoostEndTick == -1) BoostEndTick = Runner.Simulation.Tick;
		BoostEndTick += (int)(driftTiers[tier].boostDuration / Runner.DeltaTime);
	}

	public void RefreshAppliedSpeed()
	{
		AppliedSpeed = transform.InverseTransformDirection(Rigidbody.velocity).z;
		//if (isTornado || IsGhost) Rigidbody.velocity = Vector3.zero;
		if (forceBrake) Rigidbody.velocity = Vector3.zero;
	}

		private bool ShouldDriveBot()
		{
		if (!enableSimpleBotDrive)
			return false;
		if (Object == null || !Object.IsValid || !Object.HasStateAuthority)
			return false;
		if (RoomUser == null)
			return false;
		try
		{
			return RoomUser.IsBotPlayer();
		}
		catch (System.InvalidOperationException)
		{
			return false;
			}
		}

			private void SyncBotRuntimeSettingsFromTrack()
			{
				_safeLaneModeEnabled = false;
				var settings = GetTrackBotSettings();
				if (settings == null)
					return;

				if (_botRuntimeConfigRevision != settings.runtimeConfigRevision)
				{
					_botRuntimeConfigRevision = settings.runtimeConfigRevision;
					ResetBotTransientDriveState();
				}

				if (!settings.useOverrides)
					return;

				_safeLaneModeEnabled = settings.enableSafeLaneMode;
				_safeLaneMaxSpeed = Mathf.Clamp(settings.safeLaneMaxSpeed, 6f, 30f);
				_safeLaneSteerMultiplier = Mathf.Clamp(settings.safeLaneSteerMultiplier, 0.8f, 3f);
				_safeLaneDisableDrift = settings.safeLaneDisableDrift;
				_safeLaneDisableSteerWobble = settings.safeLaneDisableSteerWobble;
				_safeLaneAvoidMultiplier = Mathf.Clamp(settings.safeLaneAvoidMultiplier, 0.5f, 2f);
				_safeLaneThrottleStart01 = Mathf.Clamp(settings.safeLaneThrottleStart01, 0.5f, 0.98f);
				_safeLaneThrottleStop01 = Mathf.Clamp(settings.safeLaneThrottleStop01, _safeLaneThrottleStart01 + 0.05f, 1f);
				_autoComputeSafeLaneFromTrack = settings.autoComputeSafeLaneFromTrack;
				_autoComputeInfluence = Mathf.Clamp01(settings.autoComputeInfluence);

				enableBotSteerWobble = settings.enableBotSteerWobble;
				botSteerWobble = Mathf.Clamp(settings.botSteerWobble, 0f, 0.4f);
			botSteerMultiplier = Mathf.Clamp(settings.botSteerMultiplier, 0.5f, 4f);

			enableBotLaneRouting = settings.enableBotLaneRouting;
			botLaneHalfWidth = Mathf.Clamp(settings.botLaneHalfWidth, 0.5f, 3f);
			botLaneSubdivisionsPerSegment = Mathf.Clamp(settings.botLaneSubdivisionsPerSegment, 2, 12);
			botPathOffsetRadius = Mathf.Clamp(settings.botPathOffsetRadius, 0f, 3f);
			botLookAheadCheckpoints = Mathf.Clamp(settings.botLookAheadCheckpoints, 0, 4);
			botPathLookAheadMin = Mathf.Clamp(settings.botPathLookAheadMin, 4f, 30f);
			botPathLookAheadMax = Mathf.Clamp(settings.botPathLookAheadMax, botPathLookAheadMin + 2f, 45f);

			botCurveBrakeStart = Mathf.Clamp01(settings.botCurveBrakeStart);
			botCurveBrakeSpeed01 = Mathf.Clamp01(settings.botCurveBrakeSpeed01);
			botCurveHardBrakeSpeed01 = Mathf.Clamp(settings.botCurveHardBrakeSpeed01, botCurveBrakeSpeed01, 1f);
			botCurveSteerBoost = Mathf.Clamp(settings.botCurveSteerBoost, 1f, 2.5f);

			enableBotAutoDrift = settings.enableBotAutoDrift;
			botDriftCurveStart = Mathf.Clamp01(settings.botDriftCurveStart);
			botDriftMinSpeed01 = Mathf.Clamp01(settings.botDriftMinSpeed01);
			botDriftSteerThreshold = Mathf.Clamp01(settings.botDriftSteerThreshold);
			botDriftStartGuardTicks = Mathf.Max(1, settings.botDriftStartGuardTicks);

			botFrontRayDistance = Mathf.Clamp(settings.botFrontRayDistance, 1f, 18f);
			botSideRayDistance = Mathf.Clamp(settings.botSideRayDistance, 1f, 14f);
			botAvoidSteerStrength = Mathf.Clamp(settings.botAvoidSteerStrength, 0.5f, 2.5f);

				botStuckTicksThreshold = Mathf.Max(10, settings.botStuckTicksThreshold);
				botReverseTicks = Mathf.Max(4, settings.botReverseTicks);
				botReverseReentryDelayTicks = Mathf.Max(10, settings.botReverseReentryDelayTicks);
				botReverseFromBlockMinStillTicks = Mathf.Max(4, settings.botReverseFromBlockMinStillTicks);
				botReverseDotThreshold = Mathf.Clamp(settings.botReverseDotThreshold, -1f, 0.2f);
				botMinMoveSqrPerTick = Mathf.Max(0.00001f, settings.botMinMoveSqrPerTick);

				if (_safeLaneModeEnabled)
				{
					botLookAheadCheckpoints = Mathf.Clamp(botLookAheadCheckpoints, 0, 1);
					botPathLookAheadMin = Mathf.Clamp(botPathLookAheadMin, 4f, 14f);
					botPathLookAheadMax = Mathf.Clamp(botPathLookAheadMax, botPathLookAheadMin + 1f, 20f);
					botCurveSteerBoost = Mathf.Clamp(botCurveSteerBoost, 1f, 2.1f);
					botStuckTicksThreshold = Mathf.Min(botStuckTicksThreshold, 36);
					botReverseTicks = Mathf.Clamp(botReverseTicks, 6, 12);
					botReverseReentryDelayTicks = Mathf.Max(botReverseReentryDelayTicks, 110);
				}

				if (_safeLaneModeEnabled && _autoComputeSafeLaneFromTrack)
				{
					ApplyAutoSafeLaneFromTrack(settings);
				}

				if (settings.liveRebuildLanesInPlay && Track.Current != null)
				{
					_cachedTrackForBotLanes = int.MinValue;
					_cachedBotLanes = null;
				}
			}

			private void ResetBotTransientDriveState()
			{
				_botStillTicks = 0;
				_botReverseTicksLeft = 0;
				_botReverseCooldownTicks = 0;
				_botRecoveryForwardTicksLeft = 0;
				_botRecoverySteerSign = 1f;
				_botDriftStartGuardLeft = 0;
				_botCurveSetupTicksLeft = 0;
				_botCurveSetupSteerValue = 0f;
				_botCurveManeuverTicksLeft = 0;
				_botCurveManeuverSteerValue = 0f;
				_botCurveManeuverSharpness = 0f;
				_botCurveModeActive = false;
				_botCurveModeSteerSign = 0f;
				_botCurveModeExitTicks = 0;
				_botCurveDriftIntentTicksLeft = 0;
			_botLastDriftDebugTick = -10000;
			_botLaneProgressIndex = -1;
			_botLastPos = transform.position;
			_botAutoTuneTrackId = int.MinValue;
			_botManualDriftPathHoldTicks = 0;
		}

			private void ClearBotCurveManeuver()
			{
				_botCurveManeuverTicksLeft = 0;
				_botCurveManeuverSteerValue = 0f;
				_botCurveManeuverSharpness = 0f;
			}

			private void ClearBotCurveSetup()
			{
				_botCurveSetupTicksLeft = 0;
				_botCurveSetupSteerValue = 0f;
			}

			private void ClearBotCurvePlanning()
			{
				ClearBotCurveSetup();
				ClearBotCurveManeuver();
				ClearBotSimpleCurveState();
			}

			private void ClearBotSimpleCurveState()
			{
				_botCurveModeActive = false;
				_botCurveModeSteerSign = 0f;
				_botCurveModeExitTicks = 0;
				_botCurveDriftIntentTicksLeft = 0;
			}

			private void BotDriftDebug(string message)
			{
				if (!ShouldDriveBot() || Runner == null)
					return;

				int tick = Runner.Simulation.Tick;
				if (tick - _botLastDriftDebugTick < 45)
					return;

				_botLastDriftDebugTick = tick;
				string botName = RoomUser != null ? RoomUser.name : name;
				Debug.Log($"[BOT-DRIFT] {botName} tick={tick} {message}");
			}

			private void ArmBotCurveSetup(float curveSharpness01, float steer, float speed01)
			{
				if (!enableSimpleBotDrive || !CanDrive || IsDrifting)
					return;

				if (curveSharpness01 < Mathf.Max(0.34f, botDriftCurveStart * 0.72f))
					return;

				float steerAbs = Mathf.Abs(steer);
				if (steerAbs < Mathf.Max(0.16f, botDriftSteerThreshold * 0.55f))
					return;

				if (speed01 < Mathf.Max(0.2f, botDriftMinSpeed01 * 0.8f))
					return;

				float steerSign = Mathf.Sign(steer);
				if (Mathf.Approximately(steerSign, 0f))
					return;

				int setupTicks = Mathf.RoundToInt(Mathf.Lerp(4f, 10f, Mathf.Clamp01(curveSharpness01 * 0.8f + speed01 * 0.2f)));
				setupTicks = Mathf.Clamp(setupTicks, 3, 12);
				float setupSteer = Mathf.Clamp(Mathf.Max(steerAbs * 0.72f, Mathf.Lerp(0.28f, 0.56f, curveSharpness01)), 0.24f, 0.62f) * steerSign;

				if (_botCurveSetupTicksLeft > 0 &&
				    Mathf.Sign(_botCurveSetupSteerValue) == steerSign &&
				    _botCurveSetupTicksLeft >= setupTicks - 2)
				{
					return;
				}

				_botCurveSetupTicksLeft = setupTicks;
				_botCurveSetupSteerValue = setupSteer;
			}

			private void ArmBotCurveManeuver(float curveSharpness01, float steer, float speed01)
			{
				if (!enableSimpleBotDrive || !CanDrive)
					return;

				if (curveSharpness01 < Mathf.Max(0.32f, botDriftCurveStart * 0.82f))
					return;

				float steerAbs = Mathf.Abs(steer);
				if (steerAbs < Mathf.Max(0.22f, botDriftSteerThreshold * 0.82f))
					return;

				if (speed01 < Mathf.Max(0.18f, botDriftMinSpeed01 * 0.8f))
					return;

				float steerSign = Mathf.Sign(steer);
				if (Mathf.Approximately(steerSign, 0f))
					return;

				int holdTicks = Mathf.RoundToInt(Mathf.Lerp(12f, 42f, Mathf.Clamp01(curveSharpness01 * 0.85f + speed01 * 0.15f)));
				holdTicks = Mathf.Clamp(holdTicks, 10, 48);
				float holdSteer = Mathf.Clamp(Mathf.Max(steerAbs, Mathf.Lerp(0.58f, 0.9f, curveSharpness01)), 0.58f, 1f) * steerSign;

				if (_botCurveManeuverTicksLeft > 0 &&
				    Mathf.Sign(_botCurveManeuverSteerValue) == steerSign &&
				    _botCurveManeuverSharpness >= curveSharpness01 &&
				    _botCurveManeuverTicksLeft >= holdTicks - 4)
				{
					return;
				}

				_botCurveManeuverTicksLeft = holdTicks;
				_botCurveManeuverSteerValue = holdSteer;
				_botCurveManeuverSharpness = curveSharpness01;
			}

			private void ApplyAutoSafeLaneFromTrack(BotAITrackSettings settings)
			{
				if (settings == null || Track.Current == null)
					return;

				int trackId = Track.Current.GetInstanceID();
				if (_botAutoTuneTrackId == trackId && !settings.liveRebuildLanesInPlay)
					return;

				if (!TryGetOrBuildBotLanes(out List<Vector3>[] lanes) || lanes == null || lanes.Length == 0)
					return;

				var centerLane = lanes[Mathf.Clamp(lanes.Length / 2, 0, lanes.Length - 1)];
				if (centerLane == null || centerLane.Count < 6)
					return;

				float curve01 = ComputeLaneCurvature01(centerLane);
				// Autotune conservador: solo la curvatura ajusta velocidad/giro.
				// La pendiente y el ancho no deben bajar velocidad; eso lo resuelve la fisica real del kart.
				float complexity01 = curve01;

				float suggestedSpeed = Mathf.Lerp(12.5f, 8f, complexity01);
				float suggestedSteer = Mathf.Lerp(1.75f, 2.45f, curve01);
				float suggestedAvoid = Mathf.Lerp(1.2f, 1.7f, curve01);
				float suggestedStart = Mathf.Lerp(0.84f, 0.9f, complexity01);
				float suggestedStop = Mathf.Lerp(0.97f, 0.93f, complexity01);
				if (suggestedStop <= suggestedStart + 0.03f)
					suggestedStop = suggestedStart + 0.03f;

				float w = Mathf.Clamp01(_autoComputeInfluence);
				_safeLaneMaxSpeed = Mathf.Lerp(_safeLaneMaxSpeed, suggestedSpeed, w);
				_safeLaneSteerMultiplier = Mathf.Lerp(_safeLaneSteerMultiplier, suggestedSteer, w);
				_safeLaneAvoidMultiplier = Mathf.Lerp(_safeLaneAvoidMultiplier, suggestedAvoid, w);
				_safeLaneThrottleStart01 = Mathf.Lerp(_safeLaneThrottleStart01, suggestedStart, w);
				_safeLaneThrottleStop01 = Mathf.Lerp(_safeLaneThrottleStop01, suggestedStop, w);

				botPathLookAheadMin = Mathf.Lerp(botPathLookAheadMin, Mathf.Lerp(8f, 10.5f, 1f - complexity01), w);
				botPathLookAheadMax = Mathf.Lerp(botPathLookAheadMax, Mathf.Lerp(12f, 17f, 1f - complexity01), w);

				_safeLaneMaxSpeed = Mathf.Clamp(_safeLaneMaxSpeed, 6f, 30f);
				_safeLaneSteerMultiplier = Mathf.Clamp(_safeLaneSteerMultiplier, 0.8f, 3f);
				_safeLaneAvoidMultiplier = Mathf.Clamp(_safeLaneAvoidMultiplier, 0.5f, 2f);
				_safeLaneThrottleStart01 = Mathf.Clamp(_safeLaneThrottleStart01, 0.5f, 0.98f);
				_safeLaneThrottleStop01 = Mathf.Clamp(_safeLaneThrottleStop01, _safeLaneThrottleStart01 + 0.03f, 1f);
				botPathLookAheadMin = Mathf.Clamp(botPathLookAheadMin, 4f, 30f);
				botPathLookAheadMax = Mathf.Clamp(botPathLookAheadMax, botPathLookAheadMin + 2f, 45f);

				_botAutoTuneTrackId = trackId;
			}

			private static float ComputeLaneCurvature01(List<Vector3> lane)
			{
				if (lane == null || lane.Count < 6)
					return 0f;

				float sum = 0f;
				int samples = 0;
				int count = lane.Count;
				for (int i = 0; i < count; i++)
				{
					Vector3 a = lane[(i - 1 + count) % count];
					Vector3 b = lane[i];
					Vector3 c = lane[(i + 1) % count];
					Vector3 d1 = (b - a);
					Vector3 d2 = (c - b);
					d1.y = 0f;
					d2.y = 0f;
					if (d1.sqrMagnitude < 0.0001f || d2.sqrMagnitude < 0.0001f)
						continue;
					float angle = Vector3.Angle(d1, d2);
					sum += Mathf.Clamp01((angle - 5f) / 70f);
					samples++;
				}

				return samples <= 0 ? 0f : Mathf.Clamp01(sum / samples);
			}

			private static float ComputeLaneSlope01(List<Vector3> lane)
			{
				if (lane == null || lane.Count < 3)
					return 0f;

				float sum = 0f;
				int samples = 0;
				int count = lane.Count;
				for (int i = 0; i < count; i++)
				{
					Vector3 a = lane[i];
					Vector3 b = lane[(i + 1) % count];
					Vector3 flat = b - a;
					float dy = Mathf.Abs(flat.y);
					flat.y = 0f;
					float horizontal = flat.magnitude;
					if (horizontal < 0.01f)
						continue;
					float slope = dy / horizontal;
					sum += Mathf.Clamp01(slope / 0.35f);
					samples++;
				}

				return samples <= 0 ? 0f : Mathf.Clamp01(sum / samples);
			}

			private static float ComputeLaneCrowd01(List<Vector3>[] lanes)
			{
				if (lanes == null || lanes.Length < 2)
					return 0.35f;

				var left = lanes[0];
				var right = lanes[lanes.Length - 1];
				if (left == null || right == null || left.Count < 2 || right.Count < 2)
					return 0.35f;

				int sampleCount = Mathf.Min(left.Count, right.Count);
				if (sampleCount < 2)
					return 0.35f;

				float avgWidth = 0f;
				int n = 0;
				for (int i = 0; i < sampleCount; i++)
				{
					avgWidth += Vector3.Distance(left[i], right[i]);
					n++;
				}
				if (n <= 0)
					return 0.35f;

				avgWidth /= n;
				return Mathf.Clamp01(1f - Mathf.InverseLerp(2.8f, 8.5f, avgWidth));
			}

		private KartInput.NetworkInputData BuildBotInput()
		{
			SyncBotRuntimeSettingsFromTrack();

			var input = new KartInput.NetworkInputData();

				bool accelerate = true;
				bool reverse = false;
				float steer = 0f;
				float curveSharpness01 = 0f;
				bool pathBrakeRequested = false;
				bool triggerDriftThisFrame = false;

		if (TryGetBotPathTarget(out Vector3 targetPoint, out curveSharpness01))
		{
			Vector3 toTarget = targetPoint - transform.position;
			if (toTarget.sqrMagnitude > 0.0001f)
			{
				Vector3 local = transform.InverseTransformDirection(toTarget.normalized);
					float steerMultiplier = _safeLaneModeEnabled ? _safeLaneSteerMultiplier : botSteerMultiplier;
					steer = Mathf.Clamp(local.x * steerMultiplier, -1f, 1f);

					bool allowWobble = enableBotSteerWobble && !(_safeLaneModeEnabled && _safeLaneDisableSteerWobble);
					if (allowWobble && botSteerWobble > 0f)
					{
						float phase = (Runner.Simulation.Tick + GetBotSeed()) * 0.05f;
						steer = Mathf.Clamp(steer + Mathf.Sin(phase) * botSteerWobble, -1f, 1f);
				}

				// Do not reverse just because the target is behind.
				// Reversing is handled only by stuck/block logic below to avoid forward/backward loops.
			}
		}

					float speed01 = Mathf.Clamp01(Mathf.Abs(RealSpeed) / Mathf.Max(1f, maxSpeedNormal));
					bool manualDriftPathActive = false;
					var driftPathTool = GetTrackBotDriftPathTool();
					if (driftPathTool != null && driftPathTool.TryGetActiveDriftAt(transform.position, out int driftHoldTicks))
					{
						manualDriftPathActive = true;
						_botManualDriftPathHoldTicks = Mathf.Max(_botManualDriftPathHoldTicks, driftHoldTicks);
					}
					else if (_botManualDriftPathHoldTicks > 0)
					{
						_botManualDriftPathHoldTicks--;
						manualDriftPathActive = true;
					}

					bool curveRecoveryActive = _botReverseTicksLeft > 0 || _botRecoveryForwardTicksLeft > 0;
					float curveEnterThreshold = Mathf.Max(0.32f, botDriftCurveStart * 0.72f);
					float curveExitThreshold = curveEnterThreshold * 0.55f;
					bool simpleCurveCanRun = !reverse && !curveRecoveryActive && CanDrive;

					if (!simpleCurveCanRun)
					{
						ClearBotCurvePlanning();
					}
					else
					{
						bool curveCandidate =
							curveSharpness01 >= curveEnterThreshold &&
							Mathf.Abs(steer) >= 0.12f &&
							speed01 >= Mathf.Max(0.18f, botDriftMinSpeed01 * 0.7f);

						if (curveCandidate)
						{
							float steerSign = Mathf.Sign(steer);
							if (!Mathf.Approximately(steerSign, 0f))
							{
								_botCurveModeActive = true;
								_botCurveModeSteerSign = steerSign;
								_botCurveModeExitTicks = 0;
								_botCurveDriftIntentTicksLeft = Mathf.Max(_botCurveDriftIntentTicksLeft, 18);
							}
						}
						else if (_botCurveModeActive)
						{
							if (curveSharpness01 <= curveExitThreshold)
							{
								_botCurveModeExitTicks++;
								if (_botCurveModeExitTicks >= 6)
									ClearBotCurvePlanning();
							}
							else
							{
								_botCurveModeExitTicks = 0;
							}
						}
					}

					// In sharp curves, increase steering authority so bots can close the turn line.
					if (curveSharpness01 > 0.2f)
					{
						float steerBoost = Mathf.Lerp(1f, botCurveSteerBoost, Mathf.Clamp01((curveSharpness01 - 0.2f) / 0.8f));
						steer = Mathf.Clamp(steer * steerBoost, -1f, 1f);
					}

					bool curveHoldActive = (_botCurveModeActive || manualDriftPathActive) && simpleCurveCanRun;
					if (curveHoldActive)
					{
						float effectiveSharpness = Mathf.Max(curveSharpness01, curveEnterThreshold);
						float holdSign = Mathf.Approximately(_botCurveModeSteerSign, 0f) ? Mathf.Sign(steer) : Mathf.Sign(_botCurveModeSteerSign);
						if (manualDriftPathActive && Mathf.Approximately(holdSign, 0f))
							holdSign = Mathf.Sign(steer);
						if (manualDriftPathActive && Mathf.Approximately(holdSign, 0f))
							holdSign = 1f;
						float holdMinAbs = manualDriftPathActive ? 0.58f : Mathf.Lerp(0.42f, 0.92f, effectiveSharpness);
						steer = holdSign * Mathf.Max(Mathf.Abs(steer), holdMinAbs);
						_botCurveDriftIntentTicksLeft = Mathf.Max(_botCurveDriftIntentTicksLeft, manualDriftPathActive ? 20 : Mathf.RoundToInt(Mathf.Lerp(14f, 26f, effectiveSharpness)));

						if (!IsDrifting &&
						    IsGrounded &&
						    (manualDriftPathActive || speed01 >= Mathf.Max(0.14f, botDriftMinSpeed01 * 0.55f)))
						{
							triggerDriftThisFrame = true;
						}
					}

			float avoidSteer = ComputeBotObstacleAvoidSteer(out bool hardFrontBlocked);
			if (_safeLaneModeEnabled)
				avoidSteer *= _safeLaneAvoidMultiplier;
			if (Mathf.Abs(avoidSteer) > 0.001f)
				steer = Mathf.Clamp(steer + avoidSteer, -1f, 1f);

		// If there is a near frontal obstacle, drop throttle before we touch wall.
			if (hardFrontBlocked)
			{
				accelerate = false;
				pathBrakeRequested = true;
			}

				if (_safeLaneModeEnabled && !reverse && pathBrakeRequested)
				{
					float speedAbs = Mathf.Abs(RealSpeed);
					float safeTop = Mathf.Max(4f, _safeLaneMaxSpeed);
					float throttleStartSpeed = safeTop * _safeLaneThrottleStart01;
					float throttleStopSpeed = safeTop * _safeLaneThrottleStop01;

				if (speedAbs >= throttleStopSpeed)
				{
					accelerate = false;
					pathBrakeRequested = true;
					}
					else if (speedAbs <= throttleStartSpeed)
					{
						accelerate = true;
					}
				}

			UpdateBotStuckState();
			if (_botReverseCooldownTicks > 0)
				_botReverseCooldownTicks--;

				if (_botReverseTicksLeft > 0)
				{
					reverse = true;
					accelerate = false;
					bool stopReverseNow = false;
					Vector3 toTargetNow = targetPoint - transform.position;
					if (toTargetNow.sqrMagnitude > 0.001f)
					{
						float facingDot = Vector3.Dot(transform.forward, toTargetNow.normalized);
						if (facingDot > botReverseDotThreshold)
						{
							stopReverseNow = true;
						}
					}

					if (reverse)
						steer = _botRecoverySteerSign * 0.9f;

					if (stopReverseNow)
					{
						_botReverseTicksLeft = 0;
						reverse = false;
						accelerate = true;
					}
					else
					{
						_botReverseTicksLeft--;
					}

						if (_botReverseTicksLeft <= 0)
						{
							_botRecoveryForwardTicksLeft = Mathf.Max(10, botReverseTicks);
							_botReverseCooldownTicks = Mathf.Max(_botReverseCooldownTicks, Mathf.Max(20, botReverseReentryDelayTicks));
							ClearBotCurvePlanning();
						}
					}
				else if (_botRecoveryForwardTicksLeft > 0)
			{
				reverse = false;
				accelerate = true;
				steer = Mathf.Clamp(steer + _botRecoverySteerSign * 0.35f, -1f, 1f);
				_botStillTicks = 0;
				_botRecoveryForwardTicksLeft--;
			}
				else if (CanDrive && _botStillTicks > botStuckTicksThreshold * 2)
				{
					// Emergency release: if we are clearly stuck for too long, force one reverse episode.
					_botStillTicks = 0;
					_botReverseCooldownTicks = 0;
					_botRecoverySteerSign = GetBotRecoverySteerSign(steer, avoidSteer);
					ClearBotCurvePlanning();
					_botReverseTicksLeft = Mathf.Max(4, botReverseTicks);
				}
				else if (CanDrive && _botStillTicks > botStuckTicksThreshold && _botReverseCooldownTicks <= 0)
				{
					_botStillTicks = 0;
					_botRecoverySteerSign = GetBotRecoverySteerSign(steer, avoidSteer);
					ClearBotCurvePlanning();
					_botReverseTicksLeft = Mathf.Max(4, botReverseTicks);
				}
				else if (CanDrive &&
				         hardFrontBlocked &&
				         Mathf.Abs(RealSpeed) < 1.8f &&
			         _botStillTicks >= Mathf.Max(4, botReverseFromBlockMinStillTicks) &&
				         _botReverseCooldownTicks <= 0)
				{
					_botRecoverySteerSign = GetBotRecoverySteerSign(steer, avoidSteer);
					ClearBotCurvePlanning();
					_botReverseTicksLeft = Mathf.Max(4, botReverseTicks / 2);
				}

				if (_botDriftStartGuardLeft > 0)
					_botDriftStartGuardLeft--;

				bool driftIntentActive = _botCurveDriftIntentTicksLeft > 0 && curveHoldActive && !reverse;
				if (!curveHoldActive && _botCurveDriftIntentTicksLeft > 0)
				{
					_botCurveDriftIntentTicksLeft--;
					driftIntentActive = _botCurveDriftIntentTicksLeft > 0 && !reverse;
				}

					bool allowAutoDrift = enableBotAutoDrift && !(_safeLaneModeEnabled && _safeLaneDisableDrift);
					bool wantsDrift = allowAutoDrift &&
					                  CanDrive &&
					                  !reverse &&
					                  (manualDriftPathActive ||
					                   curveHoldActive ||
					                   driftIntentActive ||
					                   triggerDriftThisFrame ||
					                   (IsDrifting && curveSharpness01 >= curveExitThreshold) ||
					                   (curveSharpness01 >= botDriftCurveStart &&
					                    speed01 >= botDriftMinSpeed01 &&
					                    Mathf.Abs(steer) >= botDriftSteerThreshold));

				if (wantsDrift)
				{
				input.Buttons |= KartInput.NetworkInputData.ButtonDrift;
				if (!IsDrifting &&
				    _botDriftStartGuardLeft <= 0 &&
				    IsGrounded &&
				    !Mathf.Approximately(steer, 0f))
				{
					input.OneShots |= KartInput.NetworkInputData.ButtonDrift;
					_botDriftStartGuardLeft = Mathf.Max(4, botDriftStartGuardTicks);
					_botCurveDriftIntentTicksLeft = Mathf.Max(_botCurveDriftIntentTicksLeft, 8);
					}
					else if (!IsDrifting)
					{
						BotDriftDebug($"OneShot bloqueado | curveMode={curveHoldActive} wants={wantsDrift} grounded={IsGrounded} steer={steer:F2} guard={_botDriftStartGuardLeft} intent={_botCurveDriftIntentTicksLeft} speed={AppliedSpeed:F2} sharp={curveSharpness01:F2}");
					}
				}

					if (accelerate)
						input.Buttons |= KartInput.NetworkInputData.ButtonAccelerate;
		if (reverse)
			input.Buttons |= KartInput.NetworkInputData.ButtonReverse;

		// Box items are currently equipped only into the primary slot.
		// For bots, use them as soon as the existing item cooldown allows it.
		if (Kart != null &&
		    Kart.Items != null &&
		    Kart.HeldItemIndex != -1 &&
		    Kart.Items.CanUseItem &&
		    CanDrive)
		{
			input.OneShots |= KartInput.NetworkInputData.UseItem;
		}

		input.Steer = steer;
		return input;
	}

		private void UpdateBotStuckState()
		{
			if (!CanDrive)
		{
			_botStillTicks = 0;
			_botLastPos = transform.position;
			return;
		}

		float moved = (transform.position - _botLastPos).sqrMagnitude;
		if (moved <= botMinMoveSqrPerTick && Mathf.Abs(RealSpeed) < 2f)
			_botStillTicks++;
		else
			_botStillTicks = 0;

			_botLastPos = transform.position;
		}

		private float GetBotRecoverySteerSign(float steer, float avoidSteer)
		{
			if (Mathf.Abs(avoidSteer) > 0.05f)
				return -Mathf.Sign(avoidSteer);
			if (Mathf.Abs(steer) > 0.05f)
				return -Mathf.Sign(steer);
			return (GetBotSeed() & 1) == 0 ? 1f : -1f;
		}

		private bool TryGetBotPathTarget(out Vector3 targetPoint, out float curveSharpness01)
		{
		targetPoint = transform.position + transform.forward * 12f;
		curveSharpness01 = 0f;

		if (enableBotLaneRouting && TryGetLanePathTarget(out targetPoint, out curveSharpness01))
			return true;

		return TryGetCheckpointPathTarget(out targetPoint, out curveSharpness01);
	}

		private bool TryGetLanePathTarget(out Vector3 targetPoint, out float curveSharpness01)
		{
			targetPoint = transform.position + transform.forward * 12f;
			curveSharpness01 = 0f;

		if (!TryGetOrBuildBotLanes(out List<Vector3>[] lanes))
			return false;
		if (lanes == null || lanes.Length == 0)
			return false;

		int forcedLaneIndex = GetForcedLaneIndex(lanes.Length);
		if (forcedLaneIndex >= 0)
		{
			if (_botLaneIndex != forcedLaneIndex)
				_botLaneProgressIndex = -1;
			_botLaneIndex = forcedLaneIndex;
		}
		else if (_botLaneIndex < 0 || _botLaneIndex >= lanes.Length)
			_botLaneIndex = Mathf.Abs(GetBotSeed()) % lanes.Length;

		var lane = lanes[_botLaneIndex];
		if (lane == null || lane.Count < 2)
			return false;

		Vector3 projected;
		int segIndex = GetClosestLaneSegment(lane, transform.position, _botLaneProgressIndex, out projected);
		if (segIndex < 0)
			return false;
		_botLaneProgressIndex = segIndex;

		float speed01 = Mathf.Clamp01(Mathf.Abs(RealSpeed) / Mathf.Max(1f, maxSpeedNormal));
		float lookAheadDist = Mathf.Lerp(botPathLookAheadMin, botPathLookAheadMax, speed01);
		lookAheadDist += Mathf.Clamp(botLookAheadCheckpoints, 0, 3) * 2.5f;

			targetPoint = AdvanceAlongPolyline(lane, segIndex, projected, lookAheadDist, out Vector3 tangent, out int outSegIndex);
			_botLaneProgressIndex = outSegIndex;
			curveSharpness01 = ComputeUpcomingCurveSharpness(lane, outSegIndex);

			return true;
		}

	private bool TryGetCheckpointPathTarget(out Vector3 targetPoint, out float curveSharpness01)
	{
		targetPoint = transform.position + transform.forward * 12f;
		curveSharpness01 = 0f;

		if (Track.Current == null || Track.Current.checkpoints == null || Track.Current.checkpoints.Length == 0)
			return false;

		var checkpoints = Track.Current.checkpoints;
		int length = checkpoints.Length;
		if (length == 1)
		{
			if (checkpoints[0] == null)
				return false;
			targetPoint = checkpoints[0].transform.position;
			return true;
		}

		int segEnd = GetBotRawNextCheckpointIndex(length);
		int segStart = WrapCheckpointIndex(segEnd - 1, length);
		if (checkpoints[segStart] == null || checkpoints[segEnd] == null)
			return false;

		Vector3 a = checkpoints[segStart].transform.position;
		Vector3 b = checkpoints[segEnd].transform.position;
		Vector3 ab = b - a;
		float abLen = ab.magnitude;
		if (abLen < 0.05f)
		{
			targetPoint = b;
			return true;
		}

		Vector3 abDir = ab / abLen;
		float along = Mathf.Clamp(Vector3.Dot(transform.position - a, abDir), 0f, abLen);
		Vector3 projected = a + abDir * along;

		float speed01 = Mathf.Clamp01(Mathf.Abs(RealSpeed) / Mathf.Max(1f, maxSpeedNormal));
		float lookAheadDist = Mathf.Lerp(botPathLookAheadMin, botPathLookAheadMax, speed01);
		lookAheadDist += Mathf.Clamp(botLookAheadCheckpoints, 0, 3) * 2.5f;

		var tmpLane = new List<Vector3>(length);
		for (int i = 0; i < length; i++)
		{
			if (checkpoints[i] != null)
				tmpLane.Add(checkpoints[i].transform.position);
			}
			if (tmpLane.Count < 2)
				return false;

			targetPoint = AdvanceAlongPolyline(tmpLane, segStart, projected, lookAheadDist, out _, out int outSegIndex);
			curveSharpness01 = ComputeUpcomingCurveSharpness(tmpLane, outSegIndex);

			return true;
		}

		private static float ComputeUpcomingCurveSharpness(List<Vector3> lane, int fromSegIndex)
		{
			if (lane == null || lane.Count < 4)
				return 0f;

			Vector3 baseDir = GetLaneSegmentDir(lane, fromSegIndex);
			if (baseDir.sqrMagnitude <= 0.001f)
				return 0f;

			float sharpness = 0f;
			int[] lookOffsets = { 2, 4, 6, 8 };
			for (int i = 0; i < lookOffsets.Length; i++)
			{
				Vector3 futureDir = GetLaneSegmentDir(lane, fromSegIndex + lookOffsets[i]);
				if (futureDir.sqrMagnitude <= 0.001f)
					continue;

				float angle = Vector3.Angle(baseDir, futureDir);
				float normalized = Mathf.Clamp01((angle - 8f) / 82f);
				if (normalized > sharpness)
					sharpness = normalized;
			}

			return sharpness;
		}

	private bool TryGetOrBuildBotLanes(out List<Vector3>[] lanes)
	{
		lanes = null;
		if (Track.Current == null)
			return false;

		int trackId = Track.Current.GetInstanceID();
		if (_cachedBotLanes != null && _cachedTrackForBotLanes == trackId)
		{
			lanes = _cachedBotLanes;
			return true;
		}

		if (TryBuildManualBotLanes(Track.Current.transform, out lanes) || TryBuildGeneratedBotLanes(out lanes))
		{
			_cachedTrackForBotLanes = trackId;
			_cachedBotLanes = lanes;
			return true;
		}

		return false;
	}

	private bool TryBuildManualBotLanes(Transform trackRoot, out List<Vector3>[] lanes)
	{
		lanes = null;
		if (trackRoot == null)
			return false;

		Transform botLanesRoot = null;
		var settings = GetTrackBotSettings();
		if (settings != null && settings.useOverrides && settings.botLanesRoot != null)
			botLanesRoot = settings.botLanesRoot;
		if (botLanesRoot == null)
			botLanesRoot = FindChildByNameRecursive(trackRoot, "BotLanes");
		if (botLanesRoot == null)
			return false;

		var built = new List<List<Vector3>>();
		for (int i = 0; i < botLanesRoot.childCount; i++)
		{
			var laneRoot = botLanesRoot.GetChild(i);
			var points = new List<Vector3>();
			for (int p = 0; p < laneRoot.childCount; p++)
				points.Add(laneRoot.GetChild(p).position);
			if (points.Count >= 2)
				built.Add(points);
		}

		if (built.Count == 0)
			return false;

		int laneCount = Mathf.Max(1, built.Count);
		lanes = new List<Vector3>[laneCount];
		for (int i = 0; i < laneCount; i++)
			lanes[i] = built[i];
		return true;
	}

	private bool TryBuildGeneratedBotLanes(out List<Vector3>[] lanes)
	{
		lanes = null;
		if (Track.Current == null || Track.Current.checkpoints == null || Track.Current.checkpoints.Length < 2)
			return false;

		var cps = Track.Current.checkpoints;
		var center = BuildSmoothCenterline(cps, Mathf.Max(2, botLaneSubdivisionsPerSegment));
		if (center == null || center.Count < 8)
			return false;

		int laneCount = 5;
		float[] laneFactors = { -1f, -0.5f, 0f, 0.5f, 1f };
		float laneHalfWidth = Mathf.Max(0.2f, botLaneHalfWidth + botPathOffsetRadius * 0.15f);
		lanes = new List<Vector3>[laneCount];
		for (int l = 0; l < laneCount; l++)
			lanes[l] = new List<Vector3>(center.Count);

		for (int i = 0; i < center.Count; i++)
		{
			Vector3 prev = center[(i - 1 + center.Count) % center.Count];
			Vector3 curr = center[i];
			Vector3 next = center[(i + 1) % center.Count];
			Vector3 tan1 = (curr - prev).normalized;
			Vector3 tan2 = (next - curr).normalized;
			Vector3 tangent = (tan1 + tan2);
			if (tangent.sqrMagnitude < 0.0001f)
				tangent = (next - prev).normalized;
			if (tangent.sqrMagnitude < 0.0001f)
				tangent = Vector3.forward;

			float angle = Vector3.Angle(tan1, tan2);
			float curve = Mathf.Clamp01((angle - 5f) / 85f);
			float widthScale = Mathf.Lerp(1f, 0.45f, curve);
			Vector3 right = Vector3.Cross(Vector3.up, tangent).normalized;

			for (int l = 0; l < laneCount; l++)
			{
				float offset = laneFactors[l] * laneHalfWidth * widthScale;
				lanes[l].Add(curr + right * offset);
			}
		}

		return true;
	}

	private List<Vector3> BuildSmoothCenterline(Checkpoint[] checkpoints, int subdivisions)
	{
		var anchors = new List<Vector3>();
		for (int i = 0; i < checkpoints.Length; i++)
		{
			if (checkpoints[i] != null)
				anchors.Add(checkpoints[i].transform.position);
		}

		if (anchors.Count < 2)
			return null;

		var result = new List<Vector3>(anchors.Count * (subdivisions + 1));
		int n = anchors.Count;
		for (int i = 0; i < n; i++)
		{
			Vector3 p0 = anchors[(i - 1 + n) % n];
			Vector3 p1 = anchors[i];
			Vector3 p2 = anchors[(i + 1) % n];
			Vector3 p3 = anchors[(i + 2) % n];

			for (int s = 0; s < subdivisions; s++)
			{
				float t = (float)s / subdivisions;
				result.Add(CatmullRom(p0, p1, p2, p3, t));
			}
		}

		return result;
	}

	private static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		float t2 = t * t;
		float t3 = t2 * t;
		return 0.5f * (
			(2f * p1) +
			(-p0 + p2) * t +
			(2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
			(-p0 + 3f * p1 - 3f * p2 + p3) * t3
		);
	}

	private int GetClosestLaneSegment(List<Vector3> lane, Vector3 position, int preferred, out Vector3 projected)
	{
		projected = position;
		if (lane == null || lane.Count < 2)
			return -1;

		int best = -1;
		float bestDist = float.MaxValue;
		int count = lane.Count;

		int start = 0;
		int end = count - 1;
		if (preferred >= 0 && preferred < count)
		{
			start = preferred - 12;
			end = preferred + 12;
		}

		for (int k = start; k <= end; k++)
		{
			int i = WrapCheckpointIndex(k, count);
			int j = WrapCheckpointIndex(i + 1, count);
			Vector3 a = lane[i];
			Vector3 b = lane[j];
			Vector3 closest = ClosestPointOnSegment(a, b, position);
			float d = (closest - position).sqrMagnitude;
			if (d < bestDist)
			{
				bestDist = d;
				best = i;
				projected = closest;
			}
		}

		if (best != -1)
			return best;

		for (int i = 0; i < count; i++)
		{
			int j = WrapCheckpointIndex(i + 1, count);
			Vector3 a = lane[i];
			Vector3 b = lane[j];
			Vector3 closest = ClosestPointOnSegment(a, b, position);
			float d = (closest - position).sqrMagnitude;
			if (d < bestDist)
			{
				bestDist = d;
				best = i;
				projected = closest;
			}
		}

		return best;
	}

	private static Vector3 ClosestPointOnSegment(Vector3 a, Vector3 b, Vector3 p)
	{
		Vector3 ab = b - a;
		float lenSq = ab.sqrMagnitude;
		if (lenSq <= 0.000001f)
			return a;
		float t = Mathf.Clamp01(Vector3.Dot(p - a, ab) / lenSq);
		return a + ab * t;
	}

	private Vector3 AdvanceAlongPolyline(List<Vector3> lane, int startSeg, Vector3 projected, float distance, out Vector3 tangent, out int outSegIndex)
	{
		tangent = transform.forward;
		outSegIndex = startSeg;
		if (lane == null || lane.Count < 2)
			return projected;

		Vector3 from = projected;
		int seg = WrapCheckpointIndex(startSeg, lane.Count);
		float remaining = Mathf.Max(0f, distance);
		int guard = lane.Count + 8;

		while (guard-- > 0)
		{
			int endIdx = WrapCheckpointIndex(seg + 1, lane.Count);
			Vector3 end = lane[endIdx];
			Vector3 s = end - from;
			float len = s.magnitude;
			if (len > 0.0001f)
			{
				Vector3 dir = s / len;
				tangent = dir;
				if (remaining <= len)
				{
					outSegIndex = seg;
					return from + dir * remaining;
				}
				remaining -= len;
			}

			from = end;
			seg = endIdx;
		}

		outSegIndex = seg;
		return from;
	}

	private static Vector3 GetLaneSegmentDir(List<Vector3> lane, int seg)
	{
		if (lane == null || lane.Count < 2)
			return Vector3.forward;
		int i = WrapCheckpointIndex(seg, lane.Count);
		int j = WrapCheckpointIndex(i + 1, lane.Count);
		Vector3 d = lane[j] - lane[i];
		return d.sqrMagnitude > 0.0001f ? d.normalized : Vector3.forward;
	}

	private static Transform FindChildByNameRecursive(Transform root, string name)
	{
		if (root == null || string.IsNullOrEmpty(name))
			return null;
		if (string.Equals(root.name, name, StringComparison.OrdinalIgnoreCase))
			return root;
		for (int i = 0; i < root.childCount; i++)
		{
			var found = FindChildByNameRecursive(root.GetChild(i), name);
			if (found != null)
				return found;
		}
		return null;
	}

	private BotAITrackSettings GetTrackBotSettings()
	{
		if (Track.Current == null)
			return null;

		int trackId = Track.Current.GetInstanceID();
		if (_cachedBotTrackSettings != null && _cachedBotTrackSettingsTrackId == trackId)
			return _cachedBotTrackSettings;

		_cachedBotTrackSettingsTrackId = trackId;
		_cachedBotTrackSettings = Track.Current.GetComponent<BotAITrackSettings>();
		return _cachedBotTrackSettings;
	}


	private BotDriftPathTool GetTrackBotDriftPathTool()
	{
		if (Track.Current == null)
			return null;

		int trackId = Track.Current.GetInstanceID();
		if (_cachedBotDriftPathTool != null && _cachedBotDriftPathToolTrackId == trackId)
			return _cachedBotDriftPathTool;

		_cachedBotDriftPathToolTrackId = trackId;
		_cachedBotDriftPathTool = Track.Current.GetComponent<BotDriftPathTool>();
		return _cachedBotDriftPathTool;
	}

	private int GetForcedLaneIndex(int laneCount)
	{
		if (laneCount <= 0)
			return -1;

		var settings = GetTrackBotSettings();
		if (settings == null || !settings.useOverrides || !settings.enableLaneTestMode)
			return -1;

		return Mathf.Clamp(settings.forcedLaneNumber - 1, 0, laneCount - 1);
	}

	private int GetBotRawNextCheckpointIndex(int length)
	{
		int nextIndex = 0;
		if (Kart != null && Kart.LapController != null)
			nextIndex = Kart.LapController.totalCheckPoint + 1;

		return WrapCheckpointIndex(nextIndex, length);
	}

	private static int WrapCheckpointIndex(int index, int length)
	{
		if (length <= 0)
			return 0;
		int wrapped = index % length;
		return wrapped < 0 ? wrapped + length : wrapped;
	}

	private float ComputeBotObstacleAvoidSteer(out bool hardFrontBlocked)
	{
		hardFrontBlocked = false;
		Vector3 origin = transform.position + Vector3.up * 0.35f;
		float avoid = 0f;
		int avoidContrib = 0;

		if (Physics.Raycast(origin, transform.forward, out RaycastHit frontHit, botFrontRayDistance, ~0, QueryTriggerInteraction.Ignore) &&
			IsValidBotObstacleHit(frontHit))
		{
			float normDist = Mathf.Clamp01(frontHit.distance / Mathf.Max(0.01f, botFrontRayDistance));
			float side = Vector3.Dot(frontHit.normal, transform.right);
			if (Mathf.Abs(side) < 0.05f)
				side = Mathf.Sign(Mathf.Sin((Runner.Simulation.Tick + GetBotSeed()) * 0.17f));

			avoid += -Mathf.Sign(side) * Mathf.Lerp(1f, 0.4f, normDist);
			avoidContrib++;
			if (frontHit.distance < botFrontRayDistance * 0.45f)
				hardFrontBlocked = true;
		}

		Vector3 leftDir = Quaternion.AngleAxis(-35f, Vector3.up) * transform.forward;
		if (Physics.Raycast(origin, leftDir, out RaycastHit leftHit, botSideRayDistance, ~0, QueryTriggerInteraction.Ignore) &&
			IsValidBotObstacleHit(leftHit))
		{
			float w = 1f - Mathf.Clamp01(leftHit.distance / Mathf.Max(0.01f, botSideRayDistance));
			avoid += 0.75f * w;
			avoidContrib++;
		}

		Vector3 rightDir = Quaternion.AngleAxis(35f, Vector3.up) * transform.forward;
		if (Physics.Raycast(origin, rightDir, out RaycastHit rightHit, botSideRayDistance, ~0, QueryTriggerInteraction.Ignore) &&
			IsValidBotObstacleHit(rightHit))
		{
			float w = 1f - Mathf.Clamp01(rightHit.distance / Mathf.Max(0.01f, botSideRayDistance));
			avoid -= 0.75f * w;
			avoidContrib++;
		}

		if (avoidContrib == 0)
			return 0f;

		return Mathf.Clamp((avoid / avoidContrib) * botAvoidSteerStrength, -1f, 1f);
	}

		private bool IsValidBotObstacleHit(RaycastHit hit)
		{
			if (hit.collider == null || hit.collider.isTrigger)
				return false;

			var otherRoot = hit.collider.transform.root;
			if (otherRoot == transform.root)
				return false;

			return true;
		}

	private int GetBotSeed()
	{
		if (_botSeedInitialized)
			return _botSeed;

		string source = gameObject.name;
		try
		{
			if (RoomUser != null && !string.IsNullOrEmpty(RoomUser.Username))
				source = RoomUser.Username;
		}
		catch (System.InvalidOperationException) { }

		unchecked
		{
			int hash = 17;
			for (int i = 0; i < source.Length; i++)
				hash = hash * 31 + source[i];
			_botSeed = Mathf.Abs(hash % 997) + 1;
		}

		_botSeedInitialized = true;
		return _botSeed;
	}


	// Utility functions

	private static Vector3 LerpAxis(Axis axis, Vector3 euler, float tgtVal, float t)
	{
		if (axis == Axis.X) return new Vector3(Mathf.LerpAngle(euler.x, tgtVal, t), euler.y, euler.z);
		if (axis == Axis.Y) return new Vector3(euler.x, Mathf.LerpAngle(euler.y, tgtVal, t), euler.z);
		return new Vector3(euler.x, euler.y, Mathf.LerpAngle(euler.z, tgtVal, t));
	}

	private static float Remap(float value, float srcMin, float srcMax, float destMin, float destMax, bool clamp = false)
	{
		if (clamp) value = Mathf.Clamp(value, srcMin, srcMax);
		return (value - srcMin) / (srcMax - srcMin) * (destMax - destMin) + destMin;
	}

	public DriftTier EvaluateDrift(float driftDuration, out int index)
	{
		var i = 0;
		var tier = driftTiers[0];
		while (i < driftTiers.Length)
		{
			if (driftDuration < tier.startTime)
			{
				tier = driftTiers[--i];
				break;
			}

			if (i < driftTiers.Length - 1)
				tier = driftTiers[++i];
			else
				break;
		}

		index = i;
		return tier;
	}

	public void ResetState(Quaternion _look)
	{
		Rigidbody.velocity = Vector3.zero;
		AppliedSpeed = 0;
		BoostEndTick = -1;
		BoostTierIndex = 0;
		transform.up = Vector3.up;
		model.transform.up = Vector3.up;
		transform.rotation = _look;
	}

	// type definitions

	public enum Axis
	{
		X,
		Y,
		Z
	}

	[Serializable]
	public struct DriftTier
	{
		public Color color;
		public float boostDuration;
		public float startTime;
	}
}


