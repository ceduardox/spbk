using Fusion;
using UnityEngine;

public class KartAnimator : KartComponent
{
	public ParticleSystem[] backfireEmitters;
	public ParticleSystem[] boostEmitters;
	public ParticleSystem[] driftEmitters;
	public ParticleSystem[] driftTierEmitters;
	public ParticleSystem[] tireSmokeEmitters;
	public ParticleSystem[] offroadDustEmitters;
	public ParticleSystem	explosion;
	public ParticleSystem smoke;
	public ParticleSystem	rayo;
	public TrailRenderer[] skidEmitters;

	[SerializeField] private NetworkMecanimAnimator _nma;
	[SerializeField] private Animator _animator;

    private KartController Controller => Kart.Controller;
    
    /// <summary>
	/// Anim hook
	/// </summary>
	public void AllowDrive()
	{
		Controller.RefreshAppliedSpeed();
		Controller.IsImpact = -1;
		lastIndex = -2;
		Controller.decelerationPowerUps = 1;
		rayo.Stop();
		rayo.gameObject.SetActive(false);
		
	}

	short lastIndex;
	public override void Spawned()
	{
		base.Spawned();

		/*if (Kart || Kart.Controller)
			return;*/
		Kart.Controller.OnDriftTierIndexChanged += UpdateDriftState;
		Kart.Controller.OnBoostTierIndexChanged += UpdateBoostState;



		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



		Kart.Controller.OnImpactChanged += val =>
		{
			
			
			if (!val) return;
			
			CLog.Log("ESTE VALOR ME RETORNO: " + (ClassPart)Kart.Controller.IsImpact);

			if (Kart.Controller.IsImpact == lastIndex) return;

			lastIndex = Kart.Controller.IsImpact;
			switch ((ClassPart)Kart.Controller.IsImpact) 
			{
				case ClassPart.BANANA:
					SetTrigger("Spinout");

					break;
				case ClassPart.BOMB:
					SetTrigger("Explosion");
					FX_List.playEffect(FX_Type.explosionKarts, transform);

					//explosion.Play();
					break;
				case ClassPart.GHOST:
					SetTrigger("Ghost");//Fantasma
					Kart.Controller.forceBrake = true;
					break;
				case ClassPart.MINA:
					SetTrigger("Spinout");
					FX_List.playEffect(FX_Type.explosionKarts, transform); //explosion.Play();
					break;
				case ClassPart.FAKEITEM:
					SetTrigger("Spinout");
					FX_List.playEffect(FX_Type.explosionKarts, transform);
					//explosion.Play();
					break;
				case ClassPart.WALL:
					SetTrigger("Spinout");
					FX_List.playEffect(FX_Type.explosionKarts, transform);//explosion.Play();
					break;
				case ClassPart.MISSILE:
				case ClassPart.MISSILE_GUIDED:
					SetTrigger("Spinout");
					//explosion.Play();
					break;
				case ClassPart.SKULL:
					SetTrigger("Spinout");
					//explosion.Play(); 
					break;
				case ClassPart.OIL:
					SetTrigger("Spinout");
					//explosion.Play();
					FX_List.playEffect(FX_Type.explosionKarts, transform); //explosion.Play();
					break;
				case ClassPart.THUNDER:
					SetTrigger("Electricidad");//Rayo
					FX_List.playEffect(FX_Type.electricity, transform); //explosion.Play();

					//rayo.gameObject.SetActive(true);
					//rayo.Play();

					break;
				case ClassPart.TWISTED:
					SetTrigger("Twister");
					Kart.Controller.forceBrake = true;
					break;
				case ClassPart.TURBO:
					break;
					
				/*case ClassPart.IMAN:
					Kart.Controller.deceleration= .3f;
					break;*/
				case ClassPart.BOXEXPLOSIVE:
					SetTrigger("Spinout");
					explosion.Play();
					break;
				case ClassPart.SIZEBOX:
					SetTrigger("Spinout");
					explosion.Play();
					break;
				case ClassPart.BUBBLEGUM:
					SetTrigger("Spinout");
					explosion.Play();
					break;
				case ClassPart.SOAP:
					SetTrigger("Spinout");
					FX_List.playEffect(FX_Type.bubbles, transform); //explosion.Play();

					break;
				case ClassPart.MUROESPINAS:
					SetTrigger("Explosion");
					//explosion.Play();
					break;
				case ClassPart.REMOTEKART:
					SetTrigger("Spinout");
					explosion.Play();
					break;
				case ClassPart.PARTY:
					SetTrigger("Spinout");
					explosion.Play();
					break;
				case ClassPart.SPINS:
					SetTrigger("Explosion");
					explosion.Play();
					break;
				case ClassPart.SPINSVIP:
					SetTrigger("Explosion");
					explosion.Play();
					break;
				case ClassPart.SPINSV2:
					SetTrigger("Explosion4");
					explosion.Play();
					break;
				case ClassPart.SPINSV2VIP:
					SetTrigger("Explosion4");
					explosion.Play();
					break;
				case ClassPart.MINAVIP:
					SetTrigger("Explosion");
					explosion.Play();
					break;
				case ClassPart.SUPERBOOST:
					SetTrigger("Explosion");
					explosion.Play();
					break;
				case ClassPart.BUBBLEDF:
					SetTrigger("Explosion");
					explosion.Play();
					break;
				case ClassPart.CANON:
					SetTrigger("Explosion");
					explosion.Play();
					break;
				case ClassPart.CANONGRAVEDAD:
					SetTrigger("Explosion");
					explosion.Play();
					break;
				case ClassPart.BOMBGRAVEDAD:
					SetTrigger("Explosion");
					//explosion.Play();
					break;
				case ClassPart.IMAN:
					SetTrigger("Globos");//Iman
					Kart.Controller.forceBrake = true;

					//SetTrigger("Explosion");
					//explosion.Play();
					break;
				case ClassPart.BALL:
					SetTrigger("Explosion");
					//explosion.Play();
					break;
				case ClassPart.SMOKEADHESIVE:
					/*SetTrigger("Explosion");
					explosion.Play();*/
					break;
				case ClassPart.BOMBADHESIVE:
					SetTrigger("Explosion");
					FX_List.playEffect(FX_Type.explosionKarts, transform);
					/*explosion.Play();*/
					break;
				case ClassPart.BALLOONGRAVITY:
					SetTrigger("Globos");//Fantasma
					//FX_List.playEffect(FX_Type.gravity, transform); //explosion.Play();
					Kart.Controller.forceBrake = true;
					break;
				case ClassPart.BALLOONSPINAS:
					SetTrigger("Explosion");
					/*explosion.Play();*/
					break;
				case ClassPart.FREEZE:
					SetTrigger("Freeze");
					Kart.Controller.forceBrake = true;
					FX_List.playEffect(FX_Type.freeze, transform);


					/*SetTrigger("Explosion");
					explosion.Play();*/
					break;
				case ClassPart.BOMBTELEPORT:
					/*SetTrigger("Explosion");
					explosion.Play();*/
					break;
				case ClassPart.BOMBENEMY:
					/*SetTrigger("Explosion");
					explosion.Play();*/
					break;
				case ClassPart.SIZE:
					/*SetTrigger("Explosion");
					explosion.Play();*/
					break;
				case ClassPart.BOMBSIZE:
					SetTrigger("Explosion");
					explosion.Play();
					break;
				case ClassPart.BOMBCAMBIO:
					//SetTrigger("Explosion");
					//explosion.Play();
					break;
				case ClassPart.SIZEALL:
					//SetTrigger("Explosion");
					//explosion.Play();
					break;
				case ClassPart.SMOKEALL:
					SetTrigger("Explosion");
					smoke.Play();
					break;
				case ClassPart.SUPERBOMB:
					SetTrigger("Explosion");
					//explosion.Play();
					break;
				case ClassPart.BOMBPARTY:
					//SetTrigger("Explosion");
					//explosion.Play();
					break;
				case ClassPart.DRON:
					SetTrigger("Explosion");
					explosion.Play();
					break;
				case ClassPart.BOMBVIP:
					SetTrigger("Explosion2");
					//explosion.Play();
					break;
				case ClassPart.BANANAVIP:
					SetTrigger("Explosion3");
					//explosion.Play();
					break;
				case ClassPart.IMANGOLD:
					//SetTrigger("Explosion3");
					//explosion.Play();
					break;
				case ClassPart.MINAGOLD:
					SetTrigger("Explosion3");
					//explosion.Play();
					break;
				case ClassPart.MISILVIP:
					SetTrigger("Explosion3");
					//explosion.Play();
					break;
				case ClassPart.MINASMOKEVIP:
					//SetTrigger("Explosion3");
					//explosion.Play();
					break;


				//----------------------------------------------------------------------MELEE
				case ClassPart.MELEE_SABLELASER:
				case ClassPart.MELEE_ANTORCHA:
				case ClassPart.MELEE_BATE:
				case ClassPart.MELEE_CHIPOTE:
				case ClassPart.MELEE_HELADO:
				case ClassPart.MELEE_KATANA:
				case ClassPart.MELEE_PALETA:
				case ClassPart.MELEE_PARCA:
				SetTrigger("Explosion");
					break;
			}

		};


		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/*

		Kart.Controller.OnSpinoutChanged += val =>
		{
			if (!val) return;
			if (Kart.Controller.isTornado)
				SetTrigger("Twister");
			else
				SetTrigger("Spinout");
			
		};




		Kart.Controller.OnMissilChanged += val =>
		{

			if (!val) return;
			SetTrigger("Spinout");
			explosion.Play();

		};

		Kart.Controller.OnRayoChanged += val =>
		{
			if (!val) return;
			SetTrigger("Electricidad");//Rayo
			rayo.gameObject.SetActive(true);
			rayo.Play();

		};
		
		Kart.Controller.OnGhostChanged+= val =>
		{
			if (!val) return;
			SetTrigger("Ghost");//Fantasma
			//rayo.gameObject.SetActive(true);
			//rayo.Play();

		};
		*/


		Kart.Controller.OnBumpedChanged += val =>
		{
			if (val)
			{
				SetTrigger("Bump");
				AudioManager.Play("bumpSFX", AudioManager.MixerTarget.SFX, transform.position);
			}
			else
			{
				Kart.Controller.RefreshAppliedSpeed();
			}
		};

		Kart.Controller.OnBackfiredChanged += val =>
		{
			if (!val) return;
			PlayBackfire();
			AudioManager.Play("backfireSFX", AudioManager.MixerTarget.SFX, transform.position);
		};

        Kart.Controller.OnHopChanged += val => {
            if (!val) return;
            Kart.Animator.SetTrigger("Hop");
        };
    }

	private void OnDestroy()
	{
		//CLog.Log("name: " + name+" "+transform.parent);
		Kart.Controller.OnDriftTierIndexChanged -= UpdateDriftState;
		Kart.Controller.OnBoostTierIndexChanged -= UpdateBoostState;
	}

	private void UpdateDriftState(int index)
	{
		if (index == -1)
		{
			StopDrift();
			return;
		}

		var color = Controller.driftTiers[index].color;
		foreach (var emitter in driftEmitters)
		{
			var main = emitter.main;
			main.startColor = color;
			foreach (var subEmitter in emitter.GetComponentsInChildren<ParticleSystem>())
			{
				var sub = subEmitter.main;
				sub.startColor = color;
			}

			emitter.Play(true);
		}

		foreach (var emitter in tireSmokeEmitters)
		{
			emitter.Play(true);
		}
	}

	private void StopDrift()
	{
		foreach (var emitter in driftEmitters)
		{
			emitter.Stop(true);
		}

		foreach (var emitter in tireSmokeEmitters)
		{
			emitter.Stop(true);
		}

		StopSkidFX();
	}

	private void UpdateBoostState(int index)
	{
		if (index == 0)
		{
			StopBoost();
			return;
		}

		SetTrigger("Boost");

		Color color = Controller.driftTiers[index].color;
		foreach (var emitter in boostEmitters)
		{
			var main = emitter.main;
			main.startColor = color;
			foreach (var subEmitter in emitter.GetComponentsInChildren<ParticleSystem>())
			{
				var sub = subEmitter.main;
				sub.startColor = color;
			}

			emitter.Play(true);
		}
		
		if (Object.HasInputAuthority)
		{
			Kart.Camera.speedLines.Play();
		}
	}

	public void StopBoost()
	{
		foreach (var emitter in boostEmitters)
		{
			emitter.Stop(true);
		}

		if (Object.HasInputAuthority)
		{
			Kart.Camera.speedLines.Stop();
		}
	}

	public void PlayOffroad()
	{
		foreach (var emitter in offroadDustEmitters)
		{
			emitter.Play(true);
		}
	}

	public void StopOffroad()
	{
		foreach (var emitter in offroadDustEmitters)
		{
			emitter.Stop(true);
		}
	}

	public void PlaySkidFX()
	{
		if (Kart.Controller.IsDrifting)
		{
			foreach (var trailRend in skidEmitters)
			{
				trailRend.emitting = true;
			}
		}
	}

	public void StopSkidFX()
	{
		foreach (var trailRend in skidEmitters)
		{
			trailRend.emitting = false;
		}
	}

	private void PlayBackfire()
	{
		SetTrigger("Stall");
		foreach (var emitter in backfireEmitters)
		{
			emitter.Play(true);
		}
	}
	/*
	private void PlayBackExplosion()
	{
		explosion.Play();
	}
	*/
	// TODO: this should be replaced with NetworkMecanimAnimator's SetTrigger when Fusion implement animator prediction
	public void SetTrigger(string trigger)
	{
		if (Object.HasStateAuthority)
			_nma.SetTrigger(trigger);
		else if (Object.HasInputAuthority && Runner.IsForward)
			_animator.SetTrigger(trigger);
	}
}