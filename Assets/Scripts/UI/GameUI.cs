using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameUI : MonoBehaviour
{
    public interface IGameUIComponent
    {
        void Init(KartEntity entity);
    }

    public CanvasGroup fader;
    public GameObject GameUIHolders;
    public Animator introAnimator;
    public Animator countdownAnimator;
    public Animator itemAnimator;
    public Animator itemAnimator2;
    public Animator itemAnimator3;
    public GameObject timesContainer;
    public GameObject coinCountContainer;
    public GameObject lapCountContainer;
    public GameObject pickupContainer;
    public GameObject pickupContainer2;
    public Text countPickupContainer2;
    public GameObject pickupContainer3;
    public Text countPickupContainer3;
    public EndRaceUI endRaceScreen;
    public VoteTrack voteTrack;
    public Image pickupDisplay;
    public Image pickupDisplay2;
    public Image pickupDisplay3;
    public Image boostBar;
    public Slider handleGlow;
    public GameObject vfxBar;
    public Text coinCount;
    public Text lapCount;
    public Text raceTimeText;
    public GameObject LapsCountContainer;
    public Text[] lapTimeTexts;
    public Text introGameModeText;
    public Text introTrackNameText;
    public Button continueEndButton;
    private bool _startedCountdown;
    public Text waitingPlayers;
    public Transform mapaRadar;
    public Transform playerPosition;

    public KartEntity Kart { get; private set; }
    private KartController KartController => Kart.Controller;

    public void Init(KartEntity kart)
    {
        if (GameLauncher.ConnectionStatus != ConnectionStatus.Connected)
        {
            gameObject.SetActive(false);
            return;
        }
        GameManager.Instance.setHud(this);
        setlapsTexts();
        Kart = kart;

        var uis = GetComponentsInChildren<IGameUIComponent>(true);
        foreach (var ui in uis) ui.Init(kart);

        kart.LapController.OnLapChanged += SetLapCount;

        var track = Track.Current;
        //CLog.Log("ESTO VALE CURRENT TRAK: " + track.definition.trackName);
        if (track == null)
            CLog.LogWarning($"You need to initialize the GameUI on a track for track-specific values to be updated!");
        else
        {
            introGameModeText.text = GameManager.Instance.GameType.modeName.ToString();
            introTrackNameText.text = TranslateUI.getStringUI(ResourceManager.instance.tracksDefinitions[GameManager.Instance.TrackId].trackName);//
                                                                                                                                                  //track.definition.trackName;
        }

        GameType gameType = GameManager.Instance.GameType;

        if (gameType.IsPracticeMode())
        {
            timesContainer.SetActive(false);
            lapCountContainer.SetActive(false);
        }

        if (gameType.hasPickups == false)
        {
            pickupContainer.SetActive(false);
            pickupContainer2.SetActive(false);//ADDED ITEM 2
            pickupContainer3.SetActive(false);//ADDED ITEM 3
        }
        else
        {
            ClearPickupDisplay();
            ClearPickupDisplay2();//ADDED ITEM 2
            ClearPickupDisplay3();//ADDED ITEM 3
        }

        if (gameType.hasCoins == false)
        {
            coinCountContainer.SetActive(false);
        }

        continueEndButton.gameObject.SetActive(kart.Object.HasStateAuthority);

        kart.OnHeldItemChanged += index =>
        {
            if (index == -1)
            {
                ClearPickupDisplay();
            }
            else
            {
                StartSpinItem();
            }
        };
        kart.OnHeldItem2Changed += index => //ADDED ITEM 2
        {
            if (index < 0)
            {
                ClearPickupDisplay2();
            }
            else
            {
                StartSpinItem2();
            }
        };
        kart.OnHeldItem3Changed += index => //ADDED ITEM 3
        {
            if (index < 0)
            {
                ClearPickupDisplay3();
            }
            else
            {
                StartSpinItem3();
            }
        };

        kart.OnCoinCountChanged += count =>
        {
            AudioManager.Play("coinSFX", AudioManager.MixerTarget.SFX);
            coinCount.text = $"{count:00}";
        };
        StartCoroutine(UpdateV2());
    }
    private void Start()
    {
        //setlapsTexts();
        if (transform.childCount == 0)
        {
            CLog.LogWarning("GameUI.Start: no child canvas root found.");
            return;
        }

        setCanvas gameui = gameObject.transform.GetChild(0).GetComponent<setCanvas>();
        if (gameui == null)
        {
            CLog.LogWarning("GameUI.Start: setCanvas component is missing.");
            return;
        }

        if (!gameui.desktop)
        {
            pickupContainer = gameui.IHM1;
            pickupContainer2 = gameui.IHM2;
            pickupContainer3 = gameui.IHM3;
            if (pickupContainer != null) itemAnimator = pickupContainer.GetComponent<Animator>();
            if (pickupContainer2 != null) itemAnimator2 = pickupContainer2.GetComponent<Animator>();
            if (pickupContainer3 != null) itemAnimator3 = pickupContainer3.GetComponent<Animator>();
            pickupDisplay = gameui.iconItem1;
            pickupDisplay2 = gameui.iconItem2;
            pickupDisplay3 = gameui.iconItem3;
            countPickupContainer2 = gameui.countItem2;
            countPickupContainer3 = gameui.countItem3;
        }
    }

    private void setlapsTexts()
    {
        GameObject obText = LapsCountContainer.transform.GetChild(0).gameObject;
        obText.SetActive(false);
        for (int i = 0; i < GameManager.Instance.GameType.lapCount; i++)
        {
            GameObject curretOb = Instantiate(obText, LapsCountContainer.transform.position, Quaternion.identity, LapsCountContainer.transform);
            System.Array.Resize(ref lapTimeTexts, lapTimeTexts.Length + 1);
            lapTimeTexts[i + 1] = curretOb.transform.GetChild(2).gameObject.GetComponent<Text>();
        }
    }

    private void OnDestroy()
    {
        if (Kart != null && Kart.LapController != null)
            Kart.LapController.OnLapChanged -= SetLapCount;
    }

    public void FinishCountdown()
    {
        // Kart.OnRaceStart();
    }

    public void HideIntro()
    {
        introAnimator.SetTrigger("Exit");
    }

    public void FadeIn(bool active, float time)
    {
        StartCoroutine(FadeInRoutine(active, time));
    }

    //private IEnumerator FadeInRoutine()
    //{
    //    float t = 1;
    //    while (t > 0)
    //    {
    //        fader.alpha = 1 - t;
    //        t -= Time.deltaTime;
    //        yield return null;
    //    }
    //}
    private IEnumerator FadeInRoutine(bool active, float time)
    {
        if (active)
        {
            while (time >= 0)
            {
                fader.alpha = 1 - time;
                time -= Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            while (time >= 0)
            {
                fader.alpha = 0 + time;
                time -= Time.deltaTime;
                yield return null;
            }
        }
    }
    WaitForSeconds time = new WaitForSeconds(.1f);
    private IEnumerator UpdateV2()
    {
        while (true)
        {
            yield return time;

            if (!Kart || !Kart.LapController.Object || !Kart.LapController.Object.IsValid)
                continue;

            if (!_startedCountdown && Track.Current != null)
            {
                bool isTimerRunning = false;
                float? remainingTime = null;
                try
                {
                    if (Track.Current.Object != null && Track.Current.Object.IsValid)
                    {
                        isTimerRunning = Track.Current.StartRaceTimer.IsRunning;
                        if (isTimerRunning)
                            remainingTime = Track.Current.StartRaceTimer.RemainingTime(Kart.Runner);
                    }
                }
                catch (System.InvalidOperationException)
                {
                    // Track exists but networked state is not ready yet.
                    isTimerRunning = false;
                }

                if (isTimerRunning && remainingTime != null && remainingTime <= 3.0f)
                {
                    _startedCountdown = true;
                    HideIntro();
                    FadeIn(true, 1f);
                    countdownAnimator.SetTrigger("StartCountdown");
                }
            }

            UpdateBoostBar();

            if (Kart.LapController.enabled) UpdateLapTimes();

            var controller = Kart.Controller;
            if (controller.BoostTime > 0f)
            {
                if (controller.BoostTierIndex == -1) continue;

                Color color = controller.driftTiers[controller.BoostTierIndex].color;
                SetBoostBarColor(color);
            }
            else
            {
                if (!controller.IsDrifting) continue;

                SetBoostBarColor(controller.DriftTierIndex < controller.driftTiers.Length - 1
                    ? controller.driftTiers[controller.DriftTierIndex + 1].color
                    : controller.driftTiers[controller.DriftTierIndex].color);
            }
        }
    }

    /*
    private void Update()
    {
        if (!Kart || !Kart.LapController.Object || !Kart.LapController.Object.IsValid)
            return;

        if (!_startedCountdown && Track.Current != null && Track.Current.StartRaceTimer.IsRunning)
        {
            var remainingTime = Track.Current.StartRaceTimer.RemainingTime(Kart.Runner);
            if (remainingTime != null && remainingTime <= 3.0f)
            {
                _startedCountdown = true;
                HideIntro();
                FadeIn(true,1f);
                countdownAnimator.SetTrigger("StartCountdown");
            }
        }

        UpdateBoostBar();

        if (Kart.LapController.enabled) UpdateLapTimes();

        var controller = Kart.Controller;
        if (controller.BoostTime > 0f)
        {
            if (controller.BoostTierIndex == -1) return;

            Color color = controller.driftTiers[controller.BoostTierIndex].color;
            SetBoostBarColor(color);
        }
        else
        {
            if (!controller.IsDrifting) return;

            SetBoostBarColor(controller.DriftTierIndex < controller.driftTiers.Length - 1
                ? controller.driftTiers[controller.DriftTierIndex + 1].color
                : controller.driftTiers[controller.DriftTierIndex].color);
        }
    }
*/
    private void UpdateBoostBar()
    {
        if (!KartController.Object || !KartController.Object.IsValid)
            return;

        var driftIndex = KartController.DriftTierIndex;
        var boostIndex = KartController.BoostTierIndex;

        if (KartController.IsDrifting)
        {
            if (driftIndex < KartController.driftTiers.Length - 1)
                SetBoostBar((KartController.DriftTime - KartController.driftTiers[driftIndex].startTime) /
                            (KartController.driftTiers[driftIndex + 1].startTime - KartController.driftTiers[driftIndex].startTime));
            else
                SetBoostBar(1);
        }
        else
        {
            SetBoostBar(boostIndex == -1
                ? 0f
                : KartController.BoostTime / KartController.driftTiers[boostIndex].boostDuration);
        }
    }

    private void UpdateLapTimes()
    {
        if (!Kart.LapController.Object || !Kart.LapController.Object.IsValid)
            return;

        var lapTimes = Kart.LapController.LapTicks;

        for (var i = 0; i < Mathf.Min(lapTimes.Length, lapTimeTexts.Length); i++)
        {
            var lapTicks = lapTimes.Get(i);

            if (lapTicks == 0)
            {
                lapTimeTexts[i].text = "";
            }
            else
            {
                var previousTicks = i == 0
                    ? Kart.LapController.StartRaceTick
                    : lapTimes.Get(i - 1);

                var deltaTicks = lapTicks - previousTicks;
                var time = TickHelper.TickToSeconds(Kart.Runner, deltaTicks);

                SetLapTimeText(time, i);
            }
        }

        SetRaceTimeText(Kart.LapController.GetTotalRaceTime());
    }

    public void SetBoostBar(float amount)
    {

        amount = boostBar.fillAmount = float.IsNaN(amount) ? 0 : amount;
        handleGlow.value = amount;
        if (amount == 1)
            vfxBar.SetActive(true);
        if (amount == 0)
            vfxBar.SetActive(false);
        //CLog.Log("amountsito " + amount);
    }

    public void SetBoostBarColor(Color color)
    {
        boostBar.color = color;
        CLog.Log("colorsito " + color);
    }

    public void SetCoinCount(int count)
    {
        coinCount.text = $"{count:00}";
    }

    private void SetLapCount(int lap, int maxLaps)
    {
        var text = $"{(lap > maxLaps ? maxLaps : lap)}/{maxLaps}";
        lapCount.text = text;
    }

    public void SetRaceTimeText(float time)
    {
        raceTimeText.text = $"{(int)(time / 60):00}:{time % 60:00.000}";
    }

    public void SetLapTimeText(float time, int index)
    {
        //Kart.LapController.enabled = false;
        lapTimeTexts[index].gameObject.transform.parent.gameObject.SetActive(true);
        lapTimeTexts[index].text = $"<color=#FFC600>L{index + 1}</color> {(int)(time / 60):00}:{time % 60:00.000}";
        //CLog.Log("TEST LAPS A: " + time);
    }

    public void StartSpinItem()
    {
        StartCoroutine(SpinItemRoutine());
    }
    public void StartSpinItem2()//ADDED ITEM 2
    {
        StartCoroutine(SpinItemRoutine2());
    }
    public void StartSpinItem3()//ADDED ITEM 3
    {
        StartCoroutine(SpinItemRoutine3());
    }

    private IEnumerator SpinItemRoutine()
    {
        itemAnimator.SetBool("Ticking", true);
        float dur = 3;
        float spd = Random.Range(9f, 11f); // variation, for flavor.
        float x = 0;
        while (x < dur)
        {
            x += Time.deltaTime;

            itemAnimator.speed = (spd - 1) / (dur * dur) * (x - dur) * (x - dur) + 1;
            yield return null;
        }

        itemAnimator.SetBool("Ticking", false);
        SetPickupDisplay(Kart.HeldItem);
        pickupContainer.GetComponent<Button>().interactable = true;
        // Kart.canUseItem = true;
    }
    bool firsTimeItem1 = true;
    bool firsTimeItem2 = true;
    private IEnumerator SpinItemRoutine2() //ADDED ITEM 
    {/*
		itemAnimator2.SetBool("Ticking", true);
		float dur = 3;
		float spd = Random.Range(9f, 11f); 
		float x = 0;
		while (x < dur)
		{
			x += Time.deltaTime;

			itemAnimator2.speed = (spd - 1) / (dur * dur) * (x - dur) * (x - dur) + 1;
			yield return null;
		}
		*/
        if (firsTimeItem1)
        {
            firsTimeItem1 = false;
            yield return new WaitForSeconds(1);
        }
        CLog.Log("ESTO ES LO VALE EL DIGITO: " + Kart.HeldItemCount2 + " " + Kart.Controller.RoomUser.Username);
        if (Kart.HeldItemCount2 >= 0)
        {
            countPickupContainer2.text = (Kart.HeldItemCount2).ToString();

            itemAnimator2.SetBool("Ticking", true);
            yield return null;
            yield return null;
            yield return null;
            itemAnimator2.SetBool("Ticking", false);
            SetPickupDisplay2(Kart.HeldItem2);
            pickupContainer2.GetComponent<Button>().interactable = true;
        }
        else SetPickupDisplay2(null);

    }
    private IEnumerator SpinItemRoutine3() //ADDED ITEM 
    {
        /*itemAnimator3.SetBool("Ticking", true);
		float dur = 3;
		float spd = Random.Range(9f, 11f); 
		float x = 0;
		while (x < dur)
		{
			x += Time.deltaTime;
			itemAnimator3.speed = (spd - 1) / (dur * dur) * (x - dur) * (x - dur) + 1;
			yield return null;
		}
		*/
        if (firsTimeItem2)
        {
            firsTimeItem2 = false;
            yield return new WaitForSeconds(1);
        }
        CLog.Log("ESTO ES LO VALE EL DIGITO 3: " + Kart.HeldItemCount3 + " " + Kart.Controller.RoomUser.Username);

        if (Kart.HeldItemCount3 >= 0)
        {
            countPickupContainer3.text = (Kart.HeldItemCount3).ToString();
            itemAnimator3.SetBool("Ticking", true);
            yield return null;
            yield return null;
            yield return null;
            itemAnimator3.SetBool("Ticking", false);
            SetPickupDisplay3(Kart.HeldItem3);
            pickupContainer3.GetComponent<Button>().interactable = true;
        }
        else SetPickupDisplay3(null);
        //Kart.Controller..CanUseItem = true;
    }

    public void SetPickupDisplay(Powerup item)
    {
        if (item)
        {
            pickupDisplay.sprite = item.itemIcon;
            pickupContainer.GetComponent<Button>().interactable = false;
        }
        else
        {
            pickupDisplay.sprite = ResourceManager.Instance.noPowerup.itemIcon; //null;
        }
        //if(pickupDisplay.sprite==null)
    }
    public void SetPickupDisplay2(Powerup item)//ADDED ITEM 2
    {
        if (item)
        {
            pickupDisplay2.sprite = item.itemIcon;
            pickupContainer2.GetComponent<Button>().interactable = false;
        }
        else
        {
            pickupDisplay2.sprite = ResourceManager.Instance.noPowerup.itemIcon;
            //pickupDisplay2.sprite = null;
        }
    }
    public void SetPickupDisplay3(Powerup item)//ADDED ITEM 3
    {
        //if (pickupDisplay3.sprite != null) return;
        if (item)
        {
            pickupDisplay3.sprite = item.itemIcon;
            pickupContainer3.GetComponent<Button>().interactable = false;
        }
        else
        {
            pickupDisplay3.sprite = ResourceManager.Instance.noPowerup.itemIcon;
        }
        //pickupDisplay3.sprite = null;
    }

    public void ClearPickupDisplay()
    {
        SetPickupDisplay(ResourceManager.Instance.noPowerup);
    }
    public void ClearPickupDisplay2()//ADDED ITEM 2
    {
        SetPickupDisplay2(ResourceManager.Instance.noPowerup);
        countPickupContainer2.text = "";
    }
    public void ClearPickupDisplay3()//ADDED ITEM 3
    {
        SetPickupDisplay3(ResourceManager.Instance.noPowerup);
        countPickupContainer3.text = "";
    }

    public void ShowEndRaceScreen()
    {
        endRaceScreen.gameObject.SetActive(true);
        FadeIn(false, 10);
    }

    public void OpenPauseMenu()
    {
        InterfaceManager.Instance.OpenPauseMenu();
    }

    public void showVoteTrack()
    {
        voteTrack.showVoteTrack();
    }
}
