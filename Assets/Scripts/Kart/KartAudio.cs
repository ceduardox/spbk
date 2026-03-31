using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class KartAudio : KartComponent
{
	public AudioSource StartSound;
	public AudioSource IdleSound;
	public AudioSource RunningSound;
	public AudioSource ReverseSound;
	public AudioSource Drift;
	public AudioSource Boost; 
	public AudioSource Offroad;
	public AudioSource Crash;
	public AudioSource Horn;
    [Range(0.1f, 1.0f)] public float RunningSoundMaxVolume = 1.0f;
	[Range(0.1f, 2.0f)] public float RunningSoundMaxPitch = 1.0f;
	[Range(0.1f, 1.0f)] public float ReverseSoundMaxVolume = 0.5f;
	[Range(0.1f, 2.0f)] public float ReverseSoundMaxPitch = 0.6f;
	[Range(0.1f, 1.0f)] public float IdleSoundMaxVolume = 0.6f;

	[Range(0.1f, 1.0f)] public float DriftMaxVolume = 0.5f;

    private KartController Controller => Kart.Controller; 

    /*public override void Spawned()
    {
        base.Spawned();
        Invoke("auxLoad", .1f);//StartCoroutine(SpawnedDelay());
    }
    void auxLoad()
    {
        if (!Kart || !Kart.Controller)
        {
            Invoke("auxLoad", .1f);
            CLog.Log("+ "+ Kart+" - "+ Kart.Controller);  
        }
        else SpawnedDelay();
        }*/
    public override void Spawned()//void SpawnedDelay()
                                  {

        
         //   yield return null;

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///


            Kart.Controller.OnImpactChanged += val =>
        {
            if (!val) return;

            CLog.Log("Este valor retonro: " +Kart.Controller.IsImpact);

            switch ((ClassPart)Kart.Controller.IsImpact)
            {
                case ClassPart.BANANA:
                    AudioManager.PlayAndFollow("slipSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.BOMB:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.FAKEITEM:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.WALL:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.GHOST:
                    AudioManager.PlayAndFollow("ghostSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.MINA:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.MISSILE:
                case ClassPart.MISSILE_GUIDED:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.SKULL:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.OIL:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.THUNDER:
                    break;
                case ClassPart.TWISTED:
                    AudioManager.PlayAndFollow("tornadoSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.TURBO:
                    break;
                case ClassPart.BOXEXPLOSIVE:
                    AudioManager.PlayAndFollow("energyFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.SIZEBOX:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.BUBBLEGUM:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.SOAP:
                    AudioManager.PlayAndFollow("slipSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.SMOKE:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.BALL:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.MUROESPINAS:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.REMOTEKART:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.PARTY:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.SPINS:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.SPINSVIP:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.SPINSV2:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.SPINSV2VIP:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.MINAVIP:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.SUPERBOOST:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.BUBBLEDF:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.CANON:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.CANONGRAVEDAD:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.BOMBGRAVEDAD:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.IMAN:
                    AudioManager.PlayAndFollow("imanSFX", transform, AudioManager.MixerTarget.SFX);

                    break;
                case ClassPart.SMOKEADHESIVE:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.BOMBADHESIVE:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.BALLOONGRAVITY:
                    AudioManager.PlayAndFollow("globoSFX", transform, AudioManager.MixerTarget.SFX);//BUG Cambiar audio
                    break;
                case ClassPart.BALLOONSPINAS:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.FREEZE:
                    AudioManager.PlayAndFollow("freezeSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.BOMBTELEPORT:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.BOMBENEMY:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.SIZE:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.BOMBSIZE:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.BOMBCAMBIO:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.SIZEALL:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.SMOKEALL:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.SUPERBOMB:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.BOMBPARTY:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.DRON:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.BOMBVIP:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.BANANAVIP:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.IMANGOLD:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.MINAGOLD:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.MISILVIP:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.MINASMOKEVIP:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;

                //--------------------------------------------------------------------------------------------------------------Melee

                case ClassPart.MELEE_SABLELASER:
                    AudioManager.PlayAndFollow("rayoElectricidadSFX", transform, AudioManager.MixerTarget.SFX);
                    break;
                case ClassPart.MELEE_ANTORCHA:
                case ClassPart.MELEE_BATE:
                case ClassPart.MELEE_CHIPOTE:
                case ClassPart.MELEE_HELADO:
                case ClassPart.MELEE_KATANA:
                case ClassPart.MELEE_PALETA:
                case ClassPart.MELEE_PARCA:
                    AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
                    break;

                default:
                    AudioManager.PlayAndFollow("itemCollectSFX", transform, AudioManager.MixerTarget.SFX);

                    break;

            }
        };

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /*
        
        Kart.Controller.OnSpinoutChanged += val => 
        {
            if ( !val ) return;
            CLog.Log("ESTE VALOR ME RETORNO: " + Kart.Controller.IsImpact);


            
            if (Kart.Controller.isTornado)
                AudioManager.PlayAndFollow("tornadoSFX", transform, AudioManager.MixerTarget.SFX);
            else
                AudioManager.PlayAndFollow("slipSFX", transform, AudioManager.MixerTarget.SFX);
        };

        Kart.Controller.OnMissilChanged += val => {
            if (!val) return;
            AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
        };
        Kart.Controller.OnRayoChanged += val => {
            if (!val) return;
            //AudioManager.PlayAndFollow("missilExplosionSFX", transform, AudioManager.MixerTarget.SFX);
        };
        Kart.Controller.OnGhostChanged+= val => {
            if (!val) return;
            AudioManager.PlayAndFollow("ghostSFX", transform, AudioManager.MixerTarget.SFX);
        };
        */
        if(Kart)
            if(Kart.Controller)
                Kart.Controller.OnBoostTierIndexChanged += val => {
            if ( val == 0 ) return;

            Boost.Play();
        };
    }

    public override void Render() {
        base.Render();

        var rb = Controller.Rigidbody;
        var speed = rb.transform.InverseTransformVector(rb.velocity / Controller.maxSpeedBoosting).z;

        HandleDriftAudio(speed);
        HandleOffroadAudio(speed);
        HandleDriveAudio(speed);
		
        IdleSound.volume = Mathf.Lerp(IdleSoundMaxVolume, 0.0f, speed * 4);
    }

    private void HandleDriveAudio(float speed) {
        if ( speed < 0.0f ) {
            // In reverse
            RunningSound.volume = 0.0f;
            ReverseSound.volume = Mathf.Lerp(0.1f, ReverseSoundMaxVolume, -speed * 1.2f);
            ReverseSound.pitch = Mathf.Lerp(0.1f, ReverseSoundMaxPitch, -speed + (Mathf.Sin(Time.time) * .1f));
        } else {
            // Moving forward
            ReverseSound.volume = 0.0f;
            RunningSound.volume = Mathf.Lerp(0.1f, RunningSoundMaxVolume, speed * 1.2f);
            RunningSound.pitch = Mathf.Lerp(0.3f, RunningSoundMaxPitch, speed + (Mathf.Sin(Time.time) * .1f));
        }
    }

    private void HandleDriftAudio(float speed) {
        var b = Controller.IsDrifting && Controller.IsGrounded
            ? speed * DriftMaxVolume
            : 0.0f;
        Drift.volume = Mathf.Lerp(Drift.volume, b, Time.deltaTime * 20f);
    }
    
    private void HandleOffroadAudio(float speed) {
        Offroad.volume = Controller.IsOffroad
            ? Mathf.Lerp(0, 0.25f, Mathf.Abs(speed) * 1.2f)
            : Mathf.Lerp(Offroad.volume, 0, Time.deltaTime * 10f);
    }

    public void PlayHorn()
	{
		Horn.Play();
	}
}
