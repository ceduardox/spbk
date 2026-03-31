using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class KartEntity : KartComponent
{
	public static event Action<KartEntity> OnKartSpawned;
	public static event Action<KartEntity> OnKartDespawned;

    public event Action<int> OnHeldItemChanged;
	public event Action<int> OnHeldItem2Changed;
	public event Action<int> OnHeldItem3Changed;

	public event Action<int> OnCoinCountChanged;

	public int id;// { get;  set; }
	public KartAnimator Animator { get; private set; }
	public KartCamera Camera { get; private set; }
	public KartController Controller { get; private set; }
	public KartInput Input { get; private set; }
	public KartLapController LapController { get; private set; }
	public KartAudio Audio { get; private set; }
	public GameUI Hud { get; private set; }
	public KartItemController Items { get; private set; }
	public float distance;
	public int position;
	public NetworkRigidbody Rigidbody { get; private set; }

	public Powerup HeldItem =>
		HeldItemIndex == -1
			? null
			: ResourceManager.Instance.powerups[HeldItemIndex];

	public Powerup HeldItem2 =>
		HeldItemIndex2 < 0
			? null
			: ResourceManager.Instance.powerups[HeldItemIndex2];
	public int HeldItemCount2;
	public int HeldItemCount3;
	public Powerup HeldItem3 =>
	HeldItemIndex3 < 0
		? null
		: ResourceManager.Instance.powerups[HeldItemIndex3];
	

	[Networked(OnChanged = nameof(OnHeldItemIndexChangedCallback))]
	public int HeldItemIndex { get; set; } = -1;
	[Networked(OnChanged = nameof(OnHeldItemIndex2ChangedCallback))]
	public int HeldItemIndex2 { get; set; } = -1;
	[Networked(OnChanged = nameof(OnHeldItemIndex3ChangedCallback))]
	public int HeldItemIndex3 { get; set; } = -1;

	[Networked(OnChanged = nameof(OnCoinCountChangedCallback))]
	public int CoinCount { get; set; }

	public Transform itemDropNode;

    private bool _despawned;
	public Transform flecha;

	//POWERUPS RECARGADOS:
	[Networked] public TickTimer powerUp1 { get; set; }
	[Networked] public TickTimer powerUp2 { get; set; }

	//
	private static void OnHeldItemIndexChangedCallback(Changed<KartEntity> changed)
	{
		changed.Behaviour.OnHeldItemChanged?.Invoke(changed.Behaviour.HeldItemIndex);

		if (changed.Behaviour.HeldItemIndex != -1)
		{
			foreach (var behaviour in changed.Behaviour.GetComponentsInChildren<KartComponent>())
				behaviour.OnEquipItem(changed.Behaviour.HeldItem, 3f);
		}
	}


	private static void OnHeldItemIndex2ChangedCallback(Changed<KartEntity> changed)
	{
		changed.Behaviour.OnHeldItem2Changed?.Invoke(changed.Behaviour.HeldItemIndex2);

		if (changed.Behaviour.HeldItemIndex2 != -1)
		{
			foreach (var behaviour in changed.Behaviour.GetComponentsInChildren<KartComponent>())
				behaviour.OnEquipItem(changed.Behaviour.HeldItem2, 0.1f);

		}


	}
	private static void OnHeldItemIndex3ChangedCallback(Changed<KartEntity> changed)
	{
		changed.Behaviour.OnHeldItem3Changed?.Invoke(changed.Behaviour.HeldItemIndex3);

		if (changed.Behaviour.HeldItemIndex3 != -1)
		{
			foreach (var behaviour in changed.Behaviour.GetComponentsInChildren<KartComponent>())
				behaviour.OnEquipItem(changed.Behaviour.HeldItem3, 0.1f);


		}
	}

	private static void OnCoinCountChangedCallback(Changed<KartEntity> changed)
	{
		changed.Behaviour.OnCoinCountChanged?.Invoke(changed.Behaviour.CoinCount);
	}

	
	private void Awake()
	{
			itemDropNode.transform.Rotate(new Vector3(0, -90, 0), Space.Self);//DaBros316
																		  // Set references before initializing all components

			Animator = GetComponentInChildren<KartAnimator>();
			Camera = GetComponent<KartCamera>();
			Controller = GetComponent<KartController>();
			Input = GetComponent<KartInput>();
			LapController = GetComponent<KartLapController>();
			Audio = GetComponentInChildren<KartAudio>();
			Items = GetComponent<KartItemController>();
			Rigidbody = GetComponent<NetworkRigidbody>();

			// Initializes all KartComponents on or under the Kart prefab
			var components = GetComponentsInChildren<KartComponent>();
			foreach (var component in components) component.Init(this);

		Invoke("setPowerUps", 0.5f);



	}

	public void setPowerUps() 
    {
		var controller = GetComponent<KartController>();
		if (controller == null || controller.RoomUser == null)
		{
			HeldItemCount2 = 0;
			HeldItemCount3 = 0;
			HeldItemIndex2 = HeldItemIndex3 = -1;
			return;
		}

		var slot0 = GetPowerUpSlot(controller.RoomUser, 0);
		var slot1 = GetPowerUpSlot(controller.RoomUser, 1);

		HeldItemCount2 = slot0.amount;
		HeldItemCount3 = slot1.amount;
		HeldItemIndex2 = HeldItemIndex3 = -1;
		//Kart.powerUp1 = TickTimer.CreateFromSeconds(Runner, 1); 
		//Kart.powerUp2 = TickTimer.CreateFromSeconds(Runner, 1);
		//CLog.Log("ESTOY LLAMANDO EL METODO " + Controller.RoomUser.getPu()[0].classPart + " " + Controller.RoomUser.getPu()[0].amount + " " + Controller.RoomUser.Username +" "+ HeldItemIndex2+" "+ HeldItemCount2);

	}

	private PowerUpPlayerRace GetPowerUpSlot(RoomPlayer user, int index)
	{
		var fallback = new PowerUpPlayerRace("0", ClassPart.NONE.ToString(), 0);
		if (user == null)
			return fallback;

		var list = user.getPu();
		if (list == null || index < 0 || index >= list.Count)
			return fallback;

		return list[index];
	}

	public static readonly List<KartEntity> Karts = new List<KartEntity>();

	public override void Spawned()
	{
		base.Spawned();
		//setPowerUps();
		
		if (Object.HasInputAuthority)
		{
			
			// Create HUD
			Hud = Instantiate(ResourceManager.Instance.hudPrefab);
			Hud.Init(this);

			Instantiate(ResourceManager.Instance.nicknameCanvasPrefab);

			GameObject t = Instantiate(Resources.Load("Prefabs/Accesories/Flecha") as GameObject);
			//t.transform.parent = GetComponent<Kart_Store>().driver; 

			t.SetActive(false);
			flecha = t.transform; 

			CLog.Log("ESTO ES22: CARGA " + flecha);


			Invoke("addFlecha",5); 
			if (Object.IsValid&& 
				GameManager.Instance.camera.GetComponent<CameraServer>())	
			{
				GameManager.Instance.camera.GetComponent<CameraServer>().reflection.transform.parent = transform.GetChild(0);
				GameManager.Instance.camera.GetComponent<CameraServer>().reflection.transform.localPosition = Vector3.zero;
				
				GameManager.Instance.camera.GetComponent<CameraServer>().reflection.SetActive(true);// ((Application.platform != RuntimePlatform.Android)));
				
			}
		}

		Karts.Add(this);
		OnKartSpawned?.Invoke(this);

	}
	public void addFlecha()
	{
		flecha.parent = GetComponent<Kart_Store>().driver;
		flecha.localPosition = Vector3.zero;
		flecha.localRotation = Quaternion.identity;
	}
	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		base.Despawned(runner, hasState);
		Karts.Remove(this);
		_despawned = true;
		OnKartDespawned?.Invoke(this);
	}

	private void OnDestroy()
	{
		Karts.Remove(this);
		if (!_despawned)
		{
			OnKartDespawned?.Invoke(this);
		}
	}
	public Vector3 location;
    private void OnTriggerStay(Collider other) { 
        if (other.TryGetComponent(out ICollidable collidable))    
        {
			CLog.Log("+: " + other.gameObject + " " +other.ClosestPoint(location)+" "+ location+" " + other.ClosestPoint(transform.position));
			//Vector3 closestPoint = other.ClosestPoint(location);
			//Gizmos.DrawSphere(location, 0.1f);
			//Gizmos.DrawWireSphere(closestPoint, 0.1f);
			collidable.Collide(this);
			Controller.calculateDmage(other.ClosestPoint(transform.position), other.gameObject); 
        }
    }

	//seteo el PowerUp que encontro
    public bool SetHeldItem(int index)
	{
		if (HeldItem != null) return false;
        
		HeldItemIndex = index;
		return true;
	}
	public bool SetHeldItem2(int index)//ADDED ITEM 2
	{
		if (HeldItem2 != null) return false;
		if (HeldItemCount2 > 0)
		{
			var slot = GetPowerUpSlot(Controller.RoomUser, 0);
			HeldItemIndex2 = ResourceManager.Instance.getPowerupIndex(slot.classPart);
		}
		
		return true;
	}
	public bool SetHeldItem3(int index)//ADDED ITEM 3
	{
		if (HeldItem3 != null) return false;

		//HeldItem3= ResourceManager.Instance.getPowerup(Controller.RoomUser.getPu()[1].classPart);
		//HeldItemIndex3 = index;
		//if (HeldItemCount3-- > 0) HeldItemIndex3 = ResourceManager.Instance.getPowerupIndex(Controller.RoomUser.getPu()[1].classPart);
		//else HeldItemIndex3 = -2;
		if (HeldItemCount3 > 0)
		{
			var slot = GetPowerUpSlot(Controller.RoomUser, 1);
			HeldItemIndex3 = ResourceManager.Instance.getPowerupIndex(slot.classPart);
		}
		
		return true;
	}


	public void SpinOut()
	{
	//	Controller.IsSpinout = true;
	}

	public void ImpactoKart(ClassPart _value)
    {
		//Controller.IsSpinout = true;
		CLog.Log("ESTA ES LA CLASE QUE ENVIO: " + _value + " " + (ClassPart)_value);
		if (GameManager.Instance.GameType.modeName == GameModes.DeathMatch&& Controller.IsImpact<0)
			LapController.Lap++;

		if(Controller.IsImpact<0) Controller.IsImpact= (short)_value;

		//En modo DeathMatch usamos la variable Lap que ya esta sincronizada como el contador de Vidas 
		//En cada impacto, sumamos una vida 

	}
	/*
	public void SpinOutExplosion()
	{
		Controller.IsMissil = true;


	}

	public void SpinOutFantasma()
	{
		//Animator.explosion.Play();
		Controller.IsGhost = true;
	}
	public void SpinOutTornado()
	{
		//Animator.explosion.Play();
		Controller.isTornado = true;
		Controller.IsSpinout= true;
		
	}

	public void SpinOutRayo()
	{
		//Animator.explosion.Play();
		Controller.IsRayo= true;
	}*/

	private IEnumerable OnSpinOut()
	{
		yield return new WaitForSeconds(2f);
		CLog.Log("XXXXXXXXXXXX ENTRE EN EL ON SIN TEMP");
		//Controller.IsMissil = false;
		//Controller.IsSpinout = false;
		//Controller.IsRayo = false;
		//Controller.IsGhost = false;
		Controller.IsImpact = -1;
		Controller.decelerationPowerUps = 1;
		Controller.forceBrake = false;

		//Controller.IsBomb = false;

	}



}
