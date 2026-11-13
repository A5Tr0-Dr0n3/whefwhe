using System;
using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using GVR.Gfx;
using TButt;
using TMPro;
using UnityEngine;

public abstract class VVR_HUB : SceneBehaviorManager
{
	public enum HubStates
	{
		Intro,
		HubGiveJobs
	}

	public enum JobStates
	{
		Macy,
		Garden,
		Sunset,
		Tumbleweed1,
		Union2Vacuum,
		ButterSnack,
		Tina,
		GardenNight,
		Tumbleweed2,
		LOOP
	}

	[SerializeField]
	protected ItemDisplaySwapper flipperZone;

	public static VVR_HUB instance;

	[HideInInspector]
	public bool hasStartedJob;

	[HideInInspector]
	public bool hasCompletedJob;

	[HideInInspector]
	public bool wasInBackstage;

	[HideInInspector]
	public bool wasInJail;

	[HideInInspector]
	public bool jobTimerStarted;

	[HideInInspector]
	public bool wasInUnion;

	[HideInInspector]
	public bool postHernandez;

	public bool ignoreNextReentry;

	internal bool tinaGoodEnding;

	[Header("XXX(JULIAN): This is coupled with JobStates in a scary way; beware!!!")]
	[SerializeField]
	protected TCPlatformScene[] _jobScenes;

	protected HubStates hubState;

	protected JobStates jobState;

	[SerializeField]
	protected HubScriptableObject hubContent;

	[Header("References:")]
	[SerializeField]
	protected GameObject jobHeadset;

	[SerializeField]
	protected VVRDialogueSource chazDialogueSource;

	[SerializeField]
	protected ChazController chazControl;

	[SerializeField]
	protected GameObject nervousEyeShaker;

	[SerializeField]
	protected GameObject searchingUI;

	[SerializeField]
	protected Transform aestheticObjectParent;

	[SerializeField]
	protected GameObject plant;

	[SerializeField]
	protected GameObject fish;

	[SerializeField]
	protected GameObject chaz;

	[SerializeField]
	protected List<GameObject> balloons;

	[SerializeField]
	protected ObjectButton saveButton;

	[SerializeField]
	protected GvrAudioSource spawnObjSound;

	[SerializeField]
	protected AudioLoop introMusic;

	[SerializeField]
	protected GameObject HRHeadsetUnlockedParent;

	[SerializeField]
	protected GameObject HRHeadsetLockedParent;

	[SerializeField]
	protected GameObject HRHeadsetPet;

	[SerializeField]
	protected GameObject HRCage;

	[SerializeField]
	protected GvrAudioSource sceneAudio;

	[SerializeField]
	protected List<GameObject> resetObjects = new List<GameObject>();

	[SerializeField]
	protected ChazResetter cReset;

	[SerializeField]
	protected Vector3 chazResetPos = new Vector3(3.007098f, 2.2f, 3f);

	protected Dictionary<GameObject, Vector3> resetObjectsPos = new Dictionary<GameObject, Vector3>();

	protected Dictionary<GameObject, Quaternion> resetObjectsRot = new Dictionary<GameObject, Quaternion>();

	protected Dictionary<GameObject, Transform> resetObjectsParent = new Dictionary<GameObject, Transform>();

	[Header("Environments:")]
	public EnvironmentProperties lightEnvironment;

	public EnvironmentProperties darkEnvironment;

	public MaterialUpdateController lightMaterialProperties;

	public MaterialUpdateController darkMaterialProperties;

	public VVR_Point lightSun;

	public VVR_Point lightUnion;

	[Header("Activitude Main Panel:")]
	public TextMeshPro sessionTimerText;

	public TextSwapper nextJobText;

	public TextMeshPro ratingText;

	public TextMeshPro employeeNumberText;

	public TextMeshPro worldTimeText;

	public TextSwapper nextJobLabel;

	public AudioEvent clockTic;

	public AudioEvent chime;

	[Header("Review Panel:")]
	[SerializeField]
	protected JobReviewBrowsingInterface reviewScript;

	public JobReviewScriptableObject plantReview;

	public JobReviewScriptableObject fishReview;

	[Header("Stapler/Headset:")]
	public AudioClip unionMessage;

	[Header("Stapler/Headset:")]
	public AudioClip vacuumMessage;

	public HubPhone phoneSound;

	[SerializeField]
	protected UnionStapler u2Stapler;

	[SerializeField]
	protected UnionStapler u3Stapler;

	public AudioClip lightSwitch;

	public AudioClip vacuumWarning;

	public GameObject table;

	public UnionStapler unionStapler;

	public Vector3 staplerSpawnPoint;

	[Header("Spawn Props")]
	public Transform spawnPoint;

	public GameObject pot;

	public GameObject hat;

	public GameObject mug;

	public GameObject lunchbox;

	public GameObject croquet;

	public GameObject beachball;

	public List<GameObject> multiObjectSpawn = new List<GameObject>();

	public AudioLoop radio;

	[Header("Blocking")]
	public Transform behindAestheticObject;

	public Transform leftOfObjects;

	public Transform tooClose;

	public Transform backToOne;

	public Transform chazCorner;

	[Header("Subtitles")]
	public GameObject union2Subtitles;

	protected bool hasLoggedIntoHub;

	protected bool hasInterrupted;

	protected bool unionInterrupting;

	protected bool chazComments;

	protected SafeCoroutine unionThread;

	protected SafeCoroutine jobFollowupThread;

	protected SafeCoroutine jobInterruptThread;

	protected SafeCoroutine finishJobThread;

	protected SafeCoroutine jailFollowupThread;

	protected SafeCoroutine giveJobThread;

	protected SafeCoroutine chazRetrieveThread;

	protected SafeCoroutine commentThread;

	protected SafeCoroutine smallTalkThread;

	protected SafeCoroutine lightsThread;

	protected SafeCoroutine macyThread;

	protected bool hasDoneFlipTutorial;

	protected bool shouldDoFlipTutorial;

	protected float lastTime;

	protected List<IEnumerator> smallTalkPool;

	protected TutorialLevelSequence.AestheticObjects aestheticObject;

	protected float lastComment;

	private Transform commentTransformTarget;

	private bool unionThreeExit;

	internal bool lightsAreOn = true;

	private int lastJobInterruptIndex;

	private int jobSuspiciousInterruptIndex;

	private int jobGenericInterruptIndex;

	protected float ticker;

	protected float nextJobTimer;

	protected int year;

	protected int minutes;

	protected int seconds;

	protected abstract SafeCoroutine SetUpFlipper();

	public virtual void PlayFlipperFridgeDialogue()
	{
	}

	private void Awake()
	{
		if (instance != null)
		{
			throw new UnityException("Only one instance of VVR_Hub in a scene is allowed.");
		}
		instance = this;
	}

	private void OnEnable()
	{
		CharacterFlip.OnFlipEvent += CommentPlayerFlipped;
	}

	private void OnDisable()
	{
		CharacterFlip.OnFlipEvent -= CommentPlayerFlipped;
		for (int i = 0; i < ToolManager.ActiveToolGroups.Count; i++)
		{
			ToolManager.ActiveToolGroups[i].Manipulator.OnSwitchTool -= VacuumInterruptStapler;
		}
		PersistentDataManager.onLanguageChange -= SetUpStaticText;
		lastTime = Time.time;
	}

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		smallTalkPool = new List<IEnumerator>
		{
			SmallTalkBackpat(),
			SmallTalkBrainJuice(),
			SmallTalkClientQuery(),
			SmallTalkColor(),
			SmallTalkNap(),
			SmallTalkOverthink(),
			SmallTalkPingPong(),
			SmallTalkReadyBee(),
			SmallTalkRealLabor(),
			SmallTalkSeas(),
			SmallTalkWall(),
			SmallTalkYourAll()
		};
		hubState = PersistentDataManager.GetSavedHubState();
		jobState = PersistentDataManager.GetJobState();
		SetUpStaticText();
		if (hubState == HubStates.Intro)
		{
			HubIntro();
		}
		else
		{
			SafeDebug.Log("<color=blue>GiveNextJob on firstTimeSceneActivated non intro</color>");
			GiveNextJob();
		}
		chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		if (u2Stapler.isActiveAndEnabled)
		{
			u2Stapler.gameObject.SetActive(value: false);
		}
		if (u3Stapler.isActiveAndEnabled)
		{
			u3Stapler.gameObject.SetActive(value: false);
		}
		LightsOn(sound: false);
		for (int i = 0; i < resetObjects.Count; i++)
		{
			resetObjectsPos.Add(resetObjects[i], resetObjects[i].transform.position);
			resetObjectsRot.Add(resetObjects[i], resetObjects[i].transform.rotation);
			resetObjectsParent.Add(resetObjects[i], resetObjects[i].transform.parent);
		}
	}

	protected override void OnSceneActivated()
	{
		base.OnSceneActivated();
		PersistentDataManager.WriteSaveDelay(3f);
		jobSuspiciousInterruptIndex = PersistentDataManager.GetSuspiciousIndex();
		jobGenericInterruptIndex = PersistentDataManager.GetGenericIndex();
		if (!ignoreNextReentry)
		{
			EagerScenes.instance.LoadQueue(TCScenes.S.VVR_PAUSE);
			reviewScript.UpdateReviewData();
			for (int i = 0; i < ToolManager.ActiveToolGroups.Count; i++)
			{
				ToolManager.ActiveToolGroups[i].Manipulator.OnSwitchTool += VacuumInterruptStapler;
			}
			PersistentDataManager.onLanguageChange += SetUpStaticText;
			EagerHeadsetManager.SetCanRemoveHeadset(canRemove: true);
			if (PersistentDataManager.GetArchiveState() == VVR_Archive.ArchiveStates.ArchivesFinished)
			{
				SafeDebug.Log("<color=blue> give next job false on scene activated  , archives finished</color>");
				GiveNextJob();
			}
		}
	}

	protected void SetUpStaticText()
	{
		year = DateTime.Now.Year + 12;
		if (PersistentDataManager.GetSubtitleLanguage() == VVR_METASAVEDATA.SubtitlesState.ENGLISH || PersistentDataManager.GetSubtitleLanguage() == VVR_METASAVEDATA.SubtitlesState.NONE)
		{
			worldTimeText.text = DateTime.Now.ToString("MM\\/dd\\/" + year);
		}
		else
		{
			worldTimeText.text = DateTime.Now.ToString("dd\\/MM\\/" + year);
		}
		employeeNumberText.text = UsernameGenerator.GetPrefixedUsername(PersistentDataManager.GetUsername());
	}

	private void HubIntro()
	{
		GlobalCoroutineContainer.AddThread(hubIntroCoroutine(), this);
	}

	protected IEnumerator hubIntroCoroutine()
	{
		HRHeadsetUnlockedParent.SetActive(value: false);
		HRCage.SetActive(value: false);
		HRHeadsetLockedParent.SetActive(value: false);
		reviewScript.reviewInterfaceActive = false;
		unionStapler.gameObject.SetActive(value: true);
		yield return null;
		PersistentDataManager.OverwriteLastLoadedScene(EagerScenes.HUB_LEVEL);
		EagerScenes.instance.DestroyScene(EagerScenes.BACKSTAGE_LEVEL);
		if (EagerScenes.instance.lastHeadsetRemoved != null)
		{
			chazControl.GrabObject(EagerScenes.instance.lastHeadsetRemoved.gameObject, destroy: true);
		}
		chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Happy, immediately: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		chazDialogueSource.PlayClip(hubContent.dialogue.iIntro);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		chazControl.EyeLookAt(plant.transform, 3.5f, ChazController.eyeColor.Happy, immediately: false);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		aestheticObject = PersistentDataManager.GetAestheticObject();
		spawnObjSound.Play();
		if (aestheticObject == TutorialLevelSequence.AestheticObjects.Plant)
		{
			fish.SetActive(value: true);
			chazDialogueSource.PlayClip(hubContent.dialogue.iFishLiven);
			JobReviewUtility.PushJobReview(1, fishReview.mReview);
		}
		else
		{
			plant.SetActive(value: true);
			chazDialogueSource.PlayClip(hubContent.dialogue.iPlantTiedTogether);
			JobReviewUtility.PushJobReview(1, plantReview.mReview);
		}
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		chazDialogueSource.PlayClip(hubContent.dialogue.iReviewIntro);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2.4f, this);
		chazControl.EyeLookAt(reviewScript.transform, 2f, ChazController.eyeColor.Default, immediately: false);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2.6f, this);
		reviewScript.reviewInterfaceActive = true;
		reviewScript.SelectReviewTile(0);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		chazControl.EyeLookAt(reviewScript.transform, 0.6f, ChazController.eyeColor.Default, immediately: false);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		chazControl.EyeLookAt(reviewScript.transform, 2f, ChazController.eyeColor.Angry, immediately: false);
		chazDialogueSource.PlayClip(hubContent.dialogue.iReviewReaction);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
		reviewScript.HideReview();
		chazControl.EyeLookAt(nervousEyeShaker.transform, 0f, ChazController.eyeColor.Thinking, immediately: true);
		chazDialogueSource.PlayClip(hubContent.dialogue.iApologizing);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		chazControl.EyeLookAt(HRHeadsetLockedParent.transform, 2f, ChazController.eyeColor.Deactivated, immediately: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
		chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		chazDialogueSource.PlayClip(hubContent.dialogue.iHRIntro);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.25f, this);
		HRHeadsetLockedParent.SetActive(value: true);
		HRHeadsetPet.GetComponent<Rigidbody>().isKinematic = true;
		spawnObjSound.Play();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		chazDialogueSource.PlayClip(hubContent.dialogue.iHRCage);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		HRCage.SetActive(value: true);
		HRHeadsetPet.GetComponent<Rigidbody>().isKinematic = false;
		spawnObjSound.Play();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.5f, this);
		chazDialogueSource.PlayClip(hubContent.dialogue.iGetAcquainted);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		chazDialogueSource.PlayClip(hubContent.dialogue.iPartingWords);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.25f, this);
		introMusic.Play();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		chaz.SetActive(value: false);
		spawnObjSound.Play();
		hubState = HubStates.HubGiveJobs;
		PersistentDataManager.OverwriteHubState(hubState);
		hasStartedJob = false;
		hasLoggedIntoHub = true;
		SafeDebug.Log("GiveNextJob hub intro");
		GiveNextJob();
	}

	protected void GiveNextJob(bool smallTalk = false)
	{
		if (giveJobThread == null || !giveJobThread.IsRunning)
		{
			giveJobThread = GlobalCoroutineContainer.AddThread(GiveJobCoroutine(smallTalk), this);
		}
	}

	protected IEnumerator GiveJobCoroutine(bool smallTalk = false)
	{
		aestheticObject = PersistentDataManager.GetAestheticObject();
		if (PersistentDataManager.GetBackstageCaughtCounter() != 2)
		{
			if (aestheticObject == TutorialLevelSequence.AestheticObjects.Plant)
			{
				fish.SetActive(value: true);
			}
			else
			{
				plant.SetActive(value: true);
			}
		}
		if (PersistentDataManager.GetArchiveState() == VVR_Archive.ArchiveStates.ArchivesFinished)
		{
			ChazStopRetrievingHeadsets();
			HRHeadsetLockedParent.SetActive(value: false);
			HRHeadsetUnlockedParent.SetActive(value: true);
			EagerScenes.instance.LoadQueue(TCScenes.S.VVR_ACTIVITUDE_HR);
			chaz.SetActive(value: false);
			yield break;
		}
		HRHeadsetLockedParent.SetActive(value: true);
		HRHeadsetUnlockedParent.SetActive(value: false);
		if (hasStartedJob)
		{
			ChazRetrieveHeadsets();
			hasLoggedIntoHub = true;
			jobHeadset.SetActive(value: false);
			jobHeadset = EagerScenes.instance.lastHeadsetRemoved.gameObject;
			jobHeadset.SetActive(value: true);
			nextJobTimer = -1f;
			jobTimerStarted = true;
			nextJobText.SetNewKey("TEXT_328");
			nextJobText.UseAltString(alt: false);
			yield break;
		}
		if (jobState == JobStates.Union2Vacuum)
		{
			if (unionThread == null)
			{
				unionThread = GlobalCoroutineContainer.AddThread(UnionTwoCoroutine(), this);
			}
			hasLoggedIntoHub = true;
			yield break;
		}
		if (!hasLoggedIntoHub)
		{
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.5f, this);
			if (DateTime.Now.DayOfWeek == DayOfWeek.Monday && !PersistentDataManager.HasPlayedCoroutine("mondayWelcome"))
			{
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.mondayWelcome);
				PersistentDataManager.MarkCoroutinePlayed("mondayWelcome");
			}
			else if (DateTime.Now.DayOfWeek == DayOfWeek.Tuesday && !PersistentDataManager.HasPlayedCoroutine("tuesdayWelcome"))
			{
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.tuesdayWelcome);
				PersistentDataManager.MarkCoroutinePlayed("tuesdayWelcome");
			}
			else if (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday && !PersistentDataManager.HasPlayedCoroutine("wednesdayWelcome"))
			{
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.wednesdayWelcome);
				PersistentDataManager.MarkCoroutinePlayed("wednesdayWelcome");
			}
			else if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday && !PersistentDataManager.HasPlayedCoroutine("thursdayWelcome"))
			{
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.thursdayWelcome);
				PersistentDataManager.MarkCoroutinePlayed("thursdayWelcome");
			}
			else if ((DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday) && !PersistentDataManager.HasPlayedCoroutine("weekendWelcome"))
			{
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.weekendWelcome);
				PersistentDataManager.MarkCoroutinePlayed("weekendWelcome");
			}
			else
			{
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.loginWelcomes[UnityEngine.Random.Range(0, hubContent.dialogue.loginWelcomes.Length)]);
			}
			hasLoggedIntoHub = true;
		}
		if (PersistentDataManager.GetHernandezState() == VVR_Hernandez.HernandezLevelState.done && !PersistentDataManager.GetUnion3Completed())
		{
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(4.5f, this);
			unionThread = GlobalCoroutineContainer.AddThread(UnionThreeCoroutine(), this);
			hasLoggedIntoHub = true;
			yield break;
		}
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		EagerScenes.instance.LoadQueue(_jobScenes[(int)jobState]);
		searchingUI.SetActive(value: true);
		nextJobLabel.SetNewKey("TEXT_331");
		if (!chaz.activeInHierarchy)
		{
			searchingUI.SetActive(value: false);
			nextJobLabel.SetNewKey("TEXT_045");
			chime.Play();
			nextJobTimer = UnityEngine.Random.Range(3f, 10f);
			jobTimerStarted = true;
			while (nextJobTimer > 0f)
			{
				nextJobTimer -= Time.deltaTime;
				yield return null;
			}
			chaz.transform.position = new Vector3(backToOne.position.x, 2.2f, backToOne.position.z);
			chaz.SetActive(value: true);
			spawnObjSound.Play();
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		}
		else
		{
			if (shouldDoFlipTutorial && TBCore.GetActivePlatform() != VRPlatform.OculusPC)
			{
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.5f, this);
				chaz.SetActive(value: false);
				spawnObjSound.Play();
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(3.5f, this);
				chaz.SetActive(value: true);
				spawnObjSound.Play();
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.5f, this);
				chazDialogueSource.PlayClip(hubContent.dialogue.flipTutorial);
				hasDoneFlipTutorial = true;
				PersistentDataManager.OverwriteHasDoneFlipTutorial(1);
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
				shouldDoFlipTutorial = false;
			}
			chazComments = true;
			SafeDebug.Log("chazComments = true wandering in givejobcoroutine");
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.5f, this);
			if (smallTalk)
			{
				SmallTalk();
			}
			while (smallTalkThread != null && smallTalkThread.IsRunning)
			{
				if (commentThread != null && commentThread.IsRunning)
				{
					smallTalkThread.Stop();
					break;
				}
				yield return null;
			}
			while (commentThread != null && commentThread.IsRunning)
			{
				yield return null;
			}
			chazControl.EyeLookAt(nextJobText.transform, 0f, ChazController.eyeColor.Default, immediately: false);
			searchingUI.SetActive(value: false);
			nextJobLabel.SetNewKey("TEXT_045");
			chime.Play();
			nextJobTimer = 5.5f;
			for (jobTimerStarted = true; nextJobTimer > 0f; nextJobTimer -= Time.deltaTime)
			{
				yield return null;
			}
			chazComments = false;
		}
		while (chazDialogueSource.isPlaying)
		{
			lastComment = Time.time;
			yield return null;
		}
		while (Time.time - lastComment < 2f)
		{
			yield return null;
		}
		chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		if (Vector3.Distance(EagerMaster.instance.mainCamera.transform.position, chazControl.head.transform.position) > 2f)
		{
			chazControl.WalkTowards(EagerMaster.instance.mainCamera.transform.position, 1f);
		}
		else
		{
			chazControl.WalkTowards(EagerMaster.instance.mainCamera.transform.position, -0.5f);
		}
		switch (jobState)
		{
		case JobStates.Macy:
			chazDialogueSource.PlayClip(hubContent.dialogue.macyIntro);
			break;
		case JobStates.Garden:
			chazDialogueSource.PlayClip(hubContent.dialogue.gardenOneIntro);
			break;
		case JobStates.Sunset:
			chazDialogueSource.PlayClip(hubContent.dialogue.sunsetIntro);
			break;
		case JobStates.Tumbleweed1:
			chazDialogueSource.PlayClip(hubContent.dialogue.tumbleOneIntro);
			break;
		case JobStates.ButterSnack:
			chazDialogueSource.PlayClip(hubContent.dialogue.butterTwoIntroEnd);
			break;
		case JobStates.Tina:
			chazDialogueSource.PlayClip(hubContent.dialogue.tinaIntro);
			break;
		case JobStates.GardenNight:
			chazDialogueSource.PlayClip(hubContent.dialogue.gardenTwoIntro);
			break;
		case JobStates.Tumbleweed2:
			chazDialogueSource.PlayClip(hubContent.dialogue.tumbleTwoIntro);
			break;
		case JobStates.LOOP:
			chazDialogueSource.PlayClip(hubContent.dialogue.endlessDesireIntro[UnityEngine.Random.Range(0, hubContent.dialogue.endlessDesireIntro.Length)]);
			break;
		}
		SafeDebug.Log("<color=blue>Job Ready</color>");
		while (!EagerScenes.SceneIsLoaded(_jobScenes[(int)jobState]))
		{
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		jobHeadset.GetComponent<Headset>().Scene = _jobScenes[(int)jobState];
		jobHeadset.GetComponent<Headset>().isActivated = true;
		jobHeadset.GetComponent<ObjectBase>().ungrabbable = false;
		chazControl.PresentObject(jobHeadset);
		ChazRetrieveHeadsets();
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		chazComments = true;
		SafeDebug.Log("chazComments = trueGiveJobCoroutine end");
	}

	protected void SmallTalk()
	{
		if (smallTalkPool == null || smallTalkPool.Count <= 0)
		{
			PersistentDataManager.ClearCoroutines();
			smallTalkPool = new List<IEnumerator>
			{
				SmallTalkBackpat(),
				SmallTalkBrainJuice(),
				SmallTalkClientQuery(),
				SmallTalkColor(),
				SmallTalkNap(),
				SmallTalkOverthink(),
				SmallTalkPingPong(),
				SmallTalkReadyBee(),
				SmallTalkRealLabor(),
				SmallTalkSeas(),
				SmallTalkWall(),
				SmallTalkYourAll()
			};
		}
		IEnumerator enumerator = smallTalkPool[UnityEngine.Random.Range(0, smallTalkPool.Count)];
		if (PersistentDataManager.HasPlayedCoroutine(enumerator))
		{
			smallTalkPool.Remove(enumerator);
			SmallTalk();
		}
		else if (unionThreeExit && !PersistentDataManager.HasPlayedCoroutine(CommentStaplerCoroutine()))
		{
			smallTalkThread = GlobalCoroutineContainer.AddThread(CommentStaplerCoroutine(), this);
			PersistentDataManager.MarkCoroutinePlayed(CommentStaplerCoroutine());
			unionThreeExit = false;
		}
		else
		{
			smallTalkThread = GlobalCoroutineContainer.AddThread(enumerator, this);
			PersistentDataManager.MarkCoroutinePlayed(enumerator);
		}
	}

	private IEnumerator SmallTalkBasic()
	{
		chazControl.WalkTo(chazCorner.position);
		chazControl.EyeLookAt(chazCorner, 8f, ChazController.eyeColor.Thinking, immediately: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(10f, this);
	}

	private IEnumerator SmallTalkBackpat()
	{
		chazControl.WalkTo(backToOne.position);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
		chazDialogueSource.PlayClip(hubContent.dialogue.backpat);
		if (Vector3.Distance(EagerMaster.instance.mainCamera.transform.position, chazControl.head.transform.position) > 2f)
		{
			chazControl.WalkTowards(EagerMaster.instance.mainCamera.transform.position, 1f);
		}
		else
		{
			chazControl.WalkTowards(EagerMaster.instance.mainCamera.transform.position, -0.5f);
		}
		chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 5.5f, ChazController.eyeColor.Happy, immediately: true);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		smallTalkThread = null;
		yield return null;
	}

	private IEnumerator SmallTalkBrainJuice()
	{
		chazControl.WalkTo(backToOne.position);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
		chazDialogueSource.PlayClip(hubContent.dialogue.brainJuice);
		chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 3f, ChazController.eyeColor.Happy, immediately: true);
		chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 6f, ChazController.eyeColor.Happy, immediately: false);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		for (int i = 0; i < 20; i++)
		{
			chazControl.ReStraighten();
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.2f, this);
		}
		smallTalkThread = null;
		yield return null;
	}

	private IEnumerator SmallTalkClientQuery()
	{
		chazControl.WalkTo(behindAestheticObject.position);
		chazControl.EyeLookAt(nextJobText.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(5f, this);
		chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		chazDialogueSource.PlayClip(hubContent.dialogue.clientQuery);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		smallTalkThread = null;
		yield return null;
	}

	private IEnumerator SmallTalkColor()
	{
		chazControl.WalkTowards(chazCorner.position, 2f);
		chazControl.EyeLookAt(nervousEyeShaker.transform, 5f, ChazController.eyeColor.Thinking, immediately: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
		chazDialogueSource.PlayClip(hubContent.dialogue.color);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		smallTalkThread = null;
		yield return null;
	}

	private IEnumerator SmallTalkEyes()
	{
		chazDialogueSource.PlayClip(hubContent.dialogue.eyes);
		chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 2.2f, ChazController.eyeColor.Angry, immediately: true);
		chazControl.EyeLookAt(nervousEyeShaker.transform, 1f, ChazController.eyeColor.Thinking, immediately: false);
		chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 3f, ChazController.eyeColor.Angry, immediately: false);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		smallTalkThread = null;
		yield return null;
	}

	private IEnumerator SmallTalkNap()
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.5f, this);
		chazDialogueSource.PlayClip(hubContent.dialogue.nap);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		chazControl.PowerDown(pitch: false, 3f);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
		smallTalkThread = null;
		yield return null;
	}

	private IEnumerator SmallTalkOverthink()
	{
		chazControl.WalkTo(chazCorner.position);
		chazControl.EyeLookAt(nervousEyeShaker.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(9f, this);
		if (Vector3.Distance(EagerMaster.instance.mainCamera.transform.position, chazControl.head.transform.position) > 2f)
		{
			chazControl.WalkTowards(EagerMaster.instance.mainCamera.transform.position, 0.8f);
		}
		else
		{
			chazControl.WalkTowards(EagerMaster.instance.mainCamera.transform.position, -0.5f);
		}
		chazDialogueSource.PlayClip(hubContent.dialogue.overthink);
		smallTalkThread = null;
		yield return null;
	}

	private IEnumerator SmallTalkPingPong()
	{
		if (Vector3.Distance(EagerMaster.instance.mainCamera.transform.position, chazControl.head.transform.position) > 2f)
		{
			chazControl.WalkTowards(EagerMaster.instance.mainCamera.transform.position, 1f);
		}
		else
		{
			chazControl.WalkTowards(EagerMaster.instance.mainCamera.transform.position, -0.5f);
		}
		chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 3f, ChazController.eyeColor.Happy, immediately: true);
		chazDialogueSource.PlayClip(hubContent.dialogue.pingPong);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		smallTalkThread = null;
		yield return null;
	}

	private IEnumerator SmallTalkReadyBee()
	{
		chazControl.WalkTo(backToOne.position);
		chazControl.EyeLookAt(nervousEyeShaker.transform, 6f, ChazController.eyeColor.Default, immediately: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(6f, this);
		chazDialogueSource.PlayClip(hubContent.dialogue.readyBee);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		smallTalkThread = null;
		yield return null;
	}

	private IEnumerator SmallTalkRealLabor()
	{
		chazControl.WalkTo(chazCorner.position);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
		chazDialogueSource.PlayClip(hubContent.dialogue.realLabor);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.5f, this);
		smallTalkThread = null;
		yield return null;
	}

	private IEnumerator SmallTalkSeas()
	{
		chazControl.EyeLookAt(reviewScript.transform, 1f, ChazController.eyeColor.Default, immediately: true);
		chazControl.EyeLookAt(nervousEyeShaker.transform, 3f, ChazController.eyeColor.Default, immediately: false);
		chazControl.WalkTowards(reviewScript.transform.position, -1f);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(4f, this);
		chazDialogueSource.PlayClip(hubContent.dialogue.seas);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		smallTalkThread = null;
		yield return null;
	}

	private IEnumerator SmallTalkWall()
	{
		chazControl.EyeLookAt(nervousEyeShaker.transform, 3f, ChazController.eyeColor.Default, immediately: false);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
		chazDialogueSource.PlayClip(hubContent.dialogue.wall);
		chazControl.EyeLookAt(reviewScript.transform, 3f, ChazController.eyeColor.Default, immediately: true);
		chazControl.WalkTowards(reviewScript.transform.position, -1f);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		smallTalkThread = null;
		yield return null;
	}

	private IEnumerator SmallTalkYourAll()
	{
		chazControl.EyeLookAt(reviewScript.transform, 3f, ChazController.eyeColor.Default, immediately: true);
		chazControl.WalkTowards(reviewScript.transform.position, -1f);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
		chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		chazDialogueSource.PlayClip(hubContent.dialogue.yourCall);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		smallTalkThread = null;
		yield return null;
	}

	public void CommentPlayerFlipped()
	{
		if (chazComments && chazControl.isActiveAndEnabled && Time.time - lastComment > 5f && (commentThread == null || !commentThread.IsRunning) && !hasDoneFlipTutorial && TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
		{
			commentThread = GlobalCoroutineContainer.AddThread(CommentPlayerFlippedCoroutine(), this);
			hasDoneFlipTutorial = true;
			PersistentDataManager.OverwriteHasDoneFlipTutorial(1);
		}
	}

	private IEnumerator CommentPlayerFlippedCoroutine()
	{
		chazDialogueSource.PlayClip(hubContent.dialogue.playerFlipped);
		yield return null;
	}

	public void CommentHRCage()
	{
		if (chazComments && chazControl.isActiveAndEnabled && Time.time - lastComment > 5f && PersistentDataManager.PrejobGetHRCage() < hubContent.dialogue.HRHeadset.Length && (commentThread == null || !commentThread.IsRunning))
		{
			commentThread = GlobalCoroutineContainer.AddThread(CommentHRCageCoroutine(PersistentDataManager.PrejobGetHRCage()), this);
			PersistentDataManager.PrejobSetHRCage();
		}
	}

	private IEnumerator CommentHRCageCoroutine(int i)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.HRHeadset[i]);
		chazControl.EyeLookAt(HRCage.transform, 5f, ChazController.eyeColor.Default, immediately: true);
		chazControl.WalkTowards(HRCage.transform.position, 0.5f);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(hubContent.dialogue.HRHeadset[i].duration - 3f, this);
		lastComment = Time.time;
		commentThread = null;
	}

	public void CommentChazAbuse()
	{
		if (chazComments && chazControl.isActiveAndEnabled && Time.time - lastComment > 5f && PersistentDataManager.PrejobGetChazAbuse() < hubContent.dialogue.chazAbuse.Length && (commentThread == null || !commentThread.IsRunning))
		{
			commentThread = GlobalCoroutineContainer.AddThread(CommentChazAbuseCoroutine(PersistentDataManager.PrejobGetChazAbuse()), this);
			PersistentDataManager.PrejobSetChazAbuse();
		}
	}

	private IEnumerator CommentChazAbuseCoroutine(int i)
	{
		chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.chazAbuse[i]);
		chazControl.WalkTowards(EagerMaster.instance.mainCamera.transform.position, -0.5f);
		chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 5f, ChazController.eyeColor.Angry, immediately: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(hubContent.dialogue.chazAbuse[i].duration - 2f, this);
		lastComment = Time.time;
		commentThread = null;
	}

	public void CommentSaveButton()
	{
		if (chazComments && chazControl.isActiveAndEnabled && Time.time - lastComment > 5f && PersistentDataManager.PrejobGetSaveButton() < hubContent.dialogue.saveButton.Length && (commentThread == null || !commentThread.IsRunning))
		{
			commentThread = GlobalCoroutineContainer.AddThread(CommentSaveButtonCoroutine(PersistentDataManager.PrejobGetSaveButton()), this);
			PersistentDataManager.PrejobSetSaveButton();
		}
	}

	private IEnumerator CommentSaveButtonCoroutine(int i)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.5f, this);
		chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.saveButton[i]);
		if (Vector3.Distance(EagerMaster.instance.mainCamera.transform.position, chazControl.head.transform.position) > 2f)
		{
			chazControl.WalkTowards(EagerMaster.instance.mainCamera.transform.position, 1.5f);
		}
		else
		{
			chazControl.WalkTowards(EagerMaster.instance.mainCamera.transform.position, -0.5f);
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(hubContent.dialogue.saveButton[i].duration - 1.5f, this);
		lastComment = Time.time;
		commentThread = null;
	}

	public void CommentBalloons(Transform balloonTransform)
	{
		if (chazComments && chazControl.isActiveAndEnabled && Time.time - lastComment > 5f && !PersistentDataManager.HasPlayedCoroutine(CommentBalloonsCoroutine()) && (commentThread == null || !commentThread.IsRunning))
		{
			commentTransformTarget = balloonTransform;
			commentThread = GlobalCoroutineContainer.AddThread(CommentBalloonsCoroutine(), this);
			PersistentDataManager.MarkCoroutinePlayed(CommentBalloonsCoroutine());
		}
	}

	private IEnumerator CommentBalloonsCoroutine()
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.balloons);
		chazControl.WalkTowards(commentTransformTarget.position, 1f);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(hubContent.dialogue.balloons.duration - 2f, this);
		lastComment = Time.time;
		commentThread = null;
	}

	public void CommentFishbowl()
	{
		if (chazComments && chazControl.isActiveAndEnabled && Time.time - lastComment > 5f && !PersistentDataManager.HasPlayedCoroutine(CommentFishbowlCoroutine()) && (commentThread == null || !commentThread.IsRunning))
		{
			commentThread = GlobalCoroutineContainer.AddThread(CommentFishbowlCoroutine(), this);
			PersistentDataManager.MarkCoroutinePlayed(CommentFishbowlCoroutine());
		}
	}

	private IEnumerator CommentFishbowlCoroutine()
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.fishbowl);
		chazControl.WalkTowards(fish.transform.position, 0.5f);
		chazControl.EyeLookAt(fish.transform, 5f, ChazController.eyeColor.Default, immediately: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(hubContent.dialogue.fishbowl.duration - 2f, this);
		lastComment = Time.time;
		commentThread = null;
	}

	public void CommentPlant()
	{
		if (chazComments && chazControl.isActiveAndEnabled && Time.time - lastComment > 5f && !PersistentDataManager.HasPlayedCoroutine(CommentPlantCoroutine()) && (commentThread == null || !commentThread.IsRunning))
		{
			commentThread = GlobalCoroutineContainer.AddThread(CommentPlantCoroutine(), this);
			PersistentDataManager.MarkCoroutinePlayed(CommentPlantCoroutine());
		}
	}

	private IEnumerator CommentPlantCoroutine()
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.plant);
		chazControl.WalkTowards(plant.transform.position, 0.5f);
		chazControl.EyeLookAt(plant.transform, 5f, ChazController.eyeColor.Default, immediately: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(hubContent.dialogue.plant.duration - 2f, this);
		lastComment = Time.time;
		commentThread = null;
	}

	public void CommentStapler()
	{
		if (chazComments && chazControl.isActiveAndEnabled && Time.time - lastComment > 5f && !PersistentDataManager.HasPlayedCoroutine(CommentStaplerCoroutine()) && (commentThread == null || !commentThread.IsRunning))
		{
			commentThread = GlobalCoroutineContainer.AddThread(CommentStaplerCoroutine(), this);
			PersistentDataManager.MarkCoroutinePlayed(CommentStaplerCoroutine());
		}
	}

	private IEnumerator CommentStaplerCoroutine()
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.stapler);
		chazControl.WalkTowards(unionStapler.transform.position, 1f);
		chazControl.EyeLookAt(unionStapler.transform, 9f, ChazController.eyeColor.Default, immediately: true);
		chazControl.EyeLookAt(unionStapler.transform, 2f, ChazController.eyeColor.Angry, immediately: false);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(hubContent.dialogue.stapler.duration + 1f, this);
		lastComment = Time.time;
		commentThread = null;
		smallTalkThread = null;
	}

	private IEnumerator CommentWait()
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(5f, this);
		commentThread = null;
	}

	public void CommentPlushie(Transform plushieTransform)
	{
		if (chazComments && chazControl.isActiveAndEnabled && Time.time - lastComment > 5f && !PersistentDataManager.HasPlayedCoroutine(CommentPlushieCoroutine()) && (commentThread == null || !commentThread.IsRunning))
		{
			commentTransformTarget = plushieTransform;
			commentThread = GlobalCoroutineContainer.AddThread(CommentPlushieCoroutine(), this);
			PersistentDataManager.MarkCoroutinePlayed(CommentPlushieCoroutine());
		}
	}

	private IEnumerator CommentPlushieCoroutine()
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.plushie);
		chazControl.WalkTowards(commentTransformTarget.position, 1.5f);
		chazControl.EyeLookAt(commentTransformTarget, 5f, ChazController.eyeColor.Happy, immediately: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(hubContent.dialogue.plushie.duration + 2f, this);
		lastComment = Time.time;
		commentThread = null;
	}

	protected IEnumerator CommentWhistle()
	{
		introMusic.Play();
		chazDialogueSource.PlayClip(hubContent.dialogue.whistle);
		float t = 0f;
		while (chazDialogueSource.isPlaying)
		{
			t += Time.deltaTime;
			if ((double)t > 0.75)
			{
				chazControl.ReStraighten();
				t = 0f;
			}
			yield return null;
		}
		yield return null;
	}

	protected void ChazReset()
	{
		chazControl.StopWalking();
		chaz.transform.position = new Vector3(backToOne.position.x - chazControl.head.transform.position.x, chaz.transform.position.y, backToOne.position.z - chazControl.head.transform.position.z);
	}

	protected void ObjectReset()
	{
		for (int i = 0; i < resetObjects.Count; i++)
		{
			if (!resetObjects[i])
			{
				continue;
			}
			if (resetObjectsPos.ContainsKey(resetObjects[i]))
			{
				resetObjects[i].transform.position = resetObjectsPos[resetObjects[i]];
			}
			if (resetObjectsRot.ContainsKey(resetObjects[i]))
			{
				resetObjects[i].transform.rotation = resetObjectsRot[resetObjects[i]];
			}
			if (flipperZone.IsObjectInFlipper(resetObjects[i]))
			{
				flipperZone.RemoveObjectFromFlipper(resetObjects[i].GetComponentInChildren<ObjectBase>());
				resetObjects[i].SetActive(value: true);
				ObjectBase componentInChildren = resetObjects[i].GetComponentInChildren<ObjectBase>();
				if (componentInChildren != null && componentInChildren.rb != null)
				{
					componentInChildren.rb.isKinematic = false;
				}
			}
			if (resetObjectsParent.ContainsKey(resetObjects[i]))
			{
				resetObjects[i].transform.parent = resetObjectsParent[resetObjects[i]];
			}
			Rigidbody componentInChildren2 = resetObjects[i].GetComponentInChildren<Rigidbody>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.velocity = Vector3.zero;
				componentInChildren2.angularVelocity = Vector3.zero;
			}
		}
	}

	protected void ResetHubVariables()
	{
		hasCompletedJob = false;
		hasStartedJob = false;
		wasInBackstage = false;
		wasInUnion = false;
		hasInterrupted = false;
		jobTimerStarted = false;
		wasInJail = false;
		postHernandez = false;
		jobInterruptThread = null;
		giveJobThread = null;
	}

	protected IEnumerator JailFollowupCoroutine()
	{
		SafeDebug.Log("jailFollowupCoroutine post hernandez: " + postHernandez + " caughtCounter: " + PersistentDataManager.GetBackstageCaughtCounter());
		chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		chazComments = false;
		bool smallTalk = false;
		if (postHernandez)
		{
			if (!PersistentDataManager.GetUnion3Completed())
			{
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.irrelevant);
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(4.5f, this);
				unionThread = GlobalCoroutineContainer.AddThread(UnionThreeCoroutine(), this);
			}
			else
			{
				yield return null;
			}
		}
		else
		{
			switch (PersistentDataManager.GetBackstageCaughtCounter())
			{
			case 1:
				chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.afterBackstageCapture);
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(5f, this);
				chazControl.EyeLookAt(nervousEyeShaker.transform, 3f, ChazController.eyeColor.Default, immediately: true);
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(11f, this);
				chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 2f, ChazController.eyeColor.Angry, immediately: true);
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
				chaz.SetActive(value: false);
				spawnObjSound.Play();
				break;
			case 2:
			{
				aestheticObject = PersistentDataManager.GetAestheticObject();
				ObjectBase aestheticBase = ((aestheticObject != TutorialLevelSequence.AestheticObjects.Plant) ? plant.GetComponent<ObjectBase>() : fish.GetComponent<ObjectBase>());
				if (!aestheticBase.gameObject.activeInHierarchy)
				{
					aestheticBase.transform.parent = aestheticObjectParent;
					aestheticBase.transform.position = resetObjectsPos[aestheticBase.gameObject];
					aestheticBase.transform.rotation = resetObjectsRot[aestheticBase.gameObject];
					aestheticBase.gameObject.SetActive(value: true);
				}
				aestheticBase.ungrabbable = true;
				aestheticBase.rb.isKinematic = true;
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.iWarnedYou);
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
				chazControl.WalkTo(behindAestheticObject.position);
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.seeThis);
				float t = 0f;
				while (t < 1.5f)
				{
					aestheticBase.transform.Rotate(0f, 60f * Time.deltaTime, 0f, Space.World);
					aestheticBase.transform.position = new Vector3(aestheticBase.transform.position.x, aestheticBase.transform.position.y + 0.45f * Time.deltaTime, aestheticBase.transform.position.z);
					t += Time.deltaTime;
					yield return null;
				}
				while (chazDialogueSource.isPlaying)
				{
					aestheticBase.transform.Rotate(0f, 60f * Time.deltaTime, 0f, Space.World);
					yield return null;
				}
				chazDialogueSource.PlayClip(hubContent.dialogue.aestheticLuxury);
				while (chazDialogueSource.isPlaying)
				{
					aestheticBase.transform.Rotate(0f, 60f * Time.deltaTime, 0f, Space.World);
					yield return null;
				}
				chazControl.LazerObject(aestheticBase.gameObject);
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
				chazDialogueSource.PlayClip(hubContent.dialogue.youLikeThis);
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
				chazDialogueSource.PlayClip(hubContent.dialogue.cleanUpThatMess);
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
				chaz.SetActive(value: false);
				spawnObjSound.Play();
				aestheticBase.ungrabbable = false;
				aestheticBase.rb.isKinematic = false;
				break;
			}
			case 3:
			{
				if (aestheticObject == TutorialLevelSequence.AestheticObjects.Plant)
				{
					fish.SetActive(value: false);
				}
				else
				{
					plant.SetActive(value: false);
				}
				chazControl.WalkTo(behindAestheticObject.position);
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.signOfPeace);
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
				spawnObjSound.Play();
				if (aestheticObject == TutorialLevelSequence.AestheticObjects.Plant)
				{
					fish.SetActive(value: true);
				}
				else
				{
					plant.SetActive(value: true);
				}
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.5f, this);
				chazControl.WalkTo(leftOfObjects.position);
				chazDialogueSource.PlayClip(hubContent.dialogue.lifeWithoutBeauty);
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
				chazControl.WalkTo(tooClose.position);
				chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 2f, ChazController.eyeColor.Happy, immediately: true);
				chazDialogueSource.PlayClip(hubContent.dialogue.youreAGem);
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
				chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 2f, ChazController.eyeColor.Happy, immediately: true);
				chazDialogueSource.PlayClip(hubContent.dialogue.youreGoingToLoveIt);
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
				radio.gameObject.SetActive(value: true);
				chazControl.PresentObject(radio.gameObject, radio: true);
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
				radio.Play();
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.3f, this);
				for (int i = 0; i < 15; i++)
				{
					chazControl.ReStraighten();
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.3f, this);
				}
				chazDialogueSource.PlayClip(hubContent.dialogue.danceWithMe);
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
				for (int j = 0; j < 3; j++)
				{
					chazControl.ReStraighten();
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.3f, this);
				}
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
				radio.Stop();
				chazControl.CloseStorage();
				chazDialogueSource.PlayClip(hubContent.dialogue.tooLoud);
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
				hat.transform.position = spawnPoint.position;
				hat.SetActive(value: true);
				spawnObjSound.Play();
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
				chazDialogueSource.PlayClip(hubContent.dialogue.coffeeCup);
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
				mug.transform.position = spawnPoint.position;
				mug.SetActive(value: true);
				spawnObjSound.Play();
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
				chazDialogueSource.PlayClip(hubContent.dialogue.lunchBox);
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
				lunchbox.transform.position = spawnPoint.position;
				lunchbox.SetActive(value: true);
				spawnObjSound.Play();
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
				chazDialogueSource.PlayClip(hubContent.dialogue.croquetSet);
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
				croquet.transform.position = spawnPoint.position;
				croquet.SetActive(value: true);
				spawnObjSound.Play();
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
				chazDialogueSource.PlayClip(hubContent.dialogue.surprise);
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
				beachball.transform.position = spawnPoint.position;
				beachball.SetActive(value: true);
				spawnObjSound.Play();
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
				chazDialogueSource.PlayClip(hubContent.dialogue.beachballAndMore);
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
				for (int k = 0; k < multiObjectSpawn.Count; k++)
				{
					multiObjectSpawn[k].transform.position = spawnPoint.position;
					multiObjectSpawn[k].SetActive(value: true);
					spawnObjSound.Play();
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.25f, this);
				}
				chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Happy, immediately: true);
				chazDialogueSource.PlayClip(hubContent.dialogue.sameSide);
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
				chazDialogueSource.PlayClip(hubContent.dialogue.proudOfYou);
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
				chaz.SetActive(value: false);
				spawnObjSound.Play();
				break;
			}
			default:
				smallTalk = true;
				break;
			}
			EagerScenes.instance.LoadQueue(EagerScenes.JAIL_LEVEL);
			ResetHubVariables();
			SafeDebug.Log("<color=blue>GiveNextJobsmalltalkjailfollowup + smalltalk: </color>" + smallTalk);
			GiveNextJob(smallTalk);
		}
		chazComments = true;
		SafeDebug.Log("chazComments = true jailfollowup over");
	}

	protected IEnumerator UnionTwoCoroutine()
	{
		if (PersistentDataManager.GetHasAdmin())
		{
			PersistentDataManager.OverwriteHasVaccum(s: false);
			ToolManager.AllManipulators.HasVacuum = false;
		}
		ChazStopRetrievingHeadsets();
		unionInterrupting = true;
		EagerScenes.instance.LoadQueue(_jobScenes[(int)jobState]);
		chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.unionIntro);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(6.1f, this);
		chazControl.PowerDown(pitch: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.5f, this);
		while (!EagerScenes.SceneIsLoaded(_jobScenes[(int)jobState]))
		{
			yield return null;
		}
		LightsOff();
		PauseAllMainThreads();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
		unionStapler.gameObject.SetActive(value: false);
		u2Stapler.gameObject.SetActive(value: true);
		u2Stapler.RingStapler(unionMessage, UnionStapler.StaplerLocation.Backstage);
		if (u2Stapler.phoneWrangler != null)
		{
			u2Stapler.phoneWrangler.enabled = true;
		}
		while (!ToolManager.AllManipulators.HasVacuum)
		{
			yield return null;
		}
		jobHeadset.SetActive(value: false);
		PersistentDataManager.OverwriteHasVaccum(s: true);
		PersistentDataManager.WriteSaveDelay(1f);
		phoneSound.Stop();
		u2Stapler.gameObject.SetActive(value: false);
		union2Subtitles.SetActive(value: false);
		unionStapler.gameObject.SetActive(value: true);
		unionStapler.transform.position = EagerMaster.instance.mainCamera.transform.position + EagerMaster.instance.mainCamera.transform.forward * 0.3f;
		ToolManager.ActiveToolGroups[0].Manipulator.Grab(unionStapler.GetComponent<ObjectBase>(), unionStapler.transform.position, playSound: false);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
		LightsOn(sound: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.5f, this);
		ResumeAllMainThreads();
		chazControl.PowerUp(pitch: true);
		unionInterrupting = false;
		EagerScenes.instance.LoadQueue(EagerScenes.JAIL_LEVEL);
		ResetHubVariables();
		SafeDebug.Log("GiveNextJob union two");
		GiveNextJob();
	}

	protected IEnumerator UnionThreeCoroutine()
	{
		hasStartedJob = false;
		ChazStopRetrievingHeadsets();
		unionInterrupting = true;
		PauseAllMainThreads();
		chazControl.PowerDown(pitch: true);
		LightsOff();
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_LVL_UNION_THREE);
		while (!EagerScenes.SceneIsLoaded(TCScenes.S.VVR_LVL_UNION_THREE))
		{
			yield return null;
		}
		unionStapler.gameObject.gameObject.SetActive(value: false);
		u3Stapler.gameObject.SetActive(value: true);
		u3Stapler.RingStapler(unionMessage, UnionStapler.StaplerLocation.Backstage);
		if (u3Stapler.phoneWrangler != null)
		{
			u3Stapler.phoneWrangler.enabled = true;
		}
		postHernandez = true;
		while (!PersistentDataManager.GetUnion3Completed())
		{
			yield return null;
		}
		PersistentDataManager.WriteSaveDelay(1f);
		jobHeadset.SetActive(value: false);
		LightsOn(sound: true);
		chazControl.PowerUp(pitch: true);
		u3Stapler.gameObject.SetActive(value: false);
		unionStapler.gameObject.SetActive(value: true);
		unionStapler.transform.position = EagerMaster.instance.mainCamera.transform.position + EagerMaster.instance.mainCamera.transform.forward * 0.3f;
		ToolManager.ActiveToolGroups[0].Manipulator.Grab(unionStapler.GetComponent<ObjectBase>(), unionStapler.transform.position, playSound: false);
		unionThreeExit = true;
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
		ResumeAllMainThreads();
		unionInterrupting = false;
		EagerScenes.instance.LoadQueue(EagerScenes.JAIL_LEVEL);
		ResetHubVariables();
		SafeDebug.Log("GiveNextJobtrue union Three");
		GiveNextJob(smallTalk: true);
		yield return null;
	}

	protected void LightsOn(bool sound)
	{
		if (lightsThread != null && lightsThread.IsRunning)
		{
			lightsThread.Stop();
		}
		lightsThread = GlobalCoroutineContainer.AddThread(LightsOnCoroutine(sound), this);
	}

	private IEnumerator LightsOnCoroutine(bool sound)
	{
		if (sound)
		{
			sceneAudio.clip = lightSwitch;
			sceneAudio.Play();
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.4f, this);
		}
		lightEnvironment.gameObject.SetActive(value: true);
		darkEnvironment.gameObject.SetActive(value: false);
		lightEnvironment.pointLights = new List<VVR_Point> { lightSun };
		lightEnvironment.Activate();
		lightMaterialProperties.PushMaterials();
		GetComponent<EagerScene>().defaultMaterialProperties = lightMaterialProperties;
		GetComponent<EagerScene>().mEnvironmentProperties = lightEnvironment;
		lightsAreOn = true;
		yield return null;
	}

	public void LightsOff()
	{
		if (lightsThread != null && lightsThread.IsRunning)
		{
			lightsThread.Stop();
		}
		lightsThread = GlobalCoroutineContainer.AddThread(LightsOffCoroutine(), this);
	}

	private IEnumerator LightsOffCoroutine()
	{
		sceneAudio.clip = lightSwitch;
		sceneAudio.Play();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.4f, this);
		darkEnvironment.gameObject.SetActive(value: true);
		lightEnvironment.gameObject.SetActive(value: false);
		darkEnvironment.pointLights = new List<VVR_Point> { lightUnion };
		darkEnvironment.Activate();
		darkMaterialProperties.PushMaterials();
		GetComponent<EagerScene>().defaultMaterialProperties = darkMaterialProperties;
		GetComponent<EagerScene>().mEnvironmentProperties = darkEnvironment;
		lightsAreOn = false;
		yield return null;
	}

	public void KillAllMainThreads()
	{
		if (unionThread != null && unionThread.IsRunning)
		{
			unionThread.Stop();
			unionThread = null;
		}
		if (jobFollowupThread != null && jobFollowupThread.IsRunning)
		{
			jobFollowupThread.Stop();
			jobFollowupThread = null;
		}
		if (finishJobThread != null && finishJobThread.IsRunning)
		{
			finishJobThread.Stop();
			finishJobThread = null;
		}
		if (jailFollowupThread != null && jailFollowupThread.IsRunning)
		{
			jailFollowupThread.Stop();
			jailFollowupThread = null;
		}
		if (giveJobThread != null && giveJobThread.IsRunning)
		{
			giveJobThread.Stop();
			giveJobThread = null;
		}
		if (chazRetrieveThread != null && chazRetrieveThread.IsRunning)
		{
			chazRetrieveThread.Stop();
			chazRetrieveThread = null;
		}
	}

	public void PauseAllMainThreads()
	{
		if (jobFollowupThread != null && jobFollowupThread.IsRunning)
		{
			jobFollowupThread.Pause();
		}
		if (finishJobThread != null && finishJobThread.IsRunning)
		{
			finishJobThread.Pause();
		}
		if (jailFollowupThread != null && jailFollowupThread.IsRunning)
		{
			jailFollowupThread.Pause();
		}
		if (giveJobThread != null && giveJobThread.IsRunning)
		{
			giveJobThread.Pause();
		}
		if (chazRetrieveThread != null && chazRetrieveThread.IsRunning)
		{
			chazRetrieveThread.Pause();
		}
	}

	protected void ResumeAllMainThreads()
	{
		if (jobFollowupThread != null && jobFollowupThread.IsPaused)
		{
			jobFollowupThread.Resume();
		}
		if (finishJobThread != null && finishJobThread.IsPaused)
		{
			finishJobThread.Resume();
		}
		if (jailFollowupThread != null && jailFollowupThread.IsPaused)
		{
			jailFollowupThread.Resume();
		}
		if (giveJobThread != null && giveJobThread.IsPaused)
		{
			giveJobThread.Resume();
		}
		if (chazRetrieveThread != null && chazRetrieveThread.IsPaused)
		{
			chazRetrieveThread.Resume();
		}
	}

	protected void VacuumInterruptStapler()
	{
		if (!PersistentDataManager.GetVacuumMessageHeard() && !unionInterrupting && !PersistentDataManager.GetHasAdmin())
		{
			PauseAllMainThreads();
			GlobalCoroutineContainer.AddThread(VacuumInterruptStaplerCoroutine(), this);
		}
	}

	private IEnumerator VacuumInterruptStaplerCoroutine()
	{
		ChazStopRetrievingHeadsets();
		unionInterrupting = true;
		PauseAllMainThreads();
		if (chazControl.isActiveAndEnabled)
		{
			chazControl.PowerDown(pitch: true);
		}
		LightsOff();
		unionStapler.gameObject.SetActive(value: true);
		unionStapler.transform.position = staplerSpawnPoint;
		unionStapler.RingStapler(vacuumMessage, UnionStapler.StaplerLocation.Vacuum);
		if (unionStapler.phoneWrangler != null)
		{
			unionStapler.phoneWrangler.enabled = true;
		}
		while (!PersistentDataManager.GetVacuumMessageHeard() && !EagerHeadsetManager.instance.isAddingHeadset)
		{
			yield return null;
		}
		ToolManager.DropEverything(playSFX: false);
		if (unionStapler.phoneWrangler != null)
		{
			unionStapler.phoneWrangler.enabled = false;
		}
		LightsOn(sound: true);
		if (chazControl.isActiveAndEnabled && !chazControl.poweredUp)
		{
			chazControl.PowerUp(pitch: true);
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
		ResumeAllMainThreads();
		unionInterrupting = false;
		yield return null;
	}

	protected void FinishJob()
	{
		KillAllMainThreads();
		finishJobThread = GlobalCoroutineContainer.AddThread(FinishJobCoroutine(), this);
	}

	private IEnumerator FinishJobCoroutine()
	{
		ObjectReset();
		SafeDebug.Log("<color=blue>Job Completed</color>");
		EagerScenes.instance.DestroyAllScenesExcept(EagerScenes.HUB_LEVEL);
		if (jobHeadset != null && (bool)jobHeadset.GetComponent<Headset>() && (bool)jobHeadset.GetComponent<ObjectBase>())
		{
			jobHeadset.GetComponent<Headset>().ResetHeadset();
			jobHeadset.GetComponent<Headset>().ResetPosition();
			jobHeadset.GetComponent<Headset>().ResetScale();
			jobHeadset.GetComponent<Headset>().isActivated = false;
			jobHeadset.GetComponent<ObjectBase>().ungrabbable = true;
		}
		if (chazRetrieveThread != null && chazRetrieveThread.IsRunning)
		{
			chazRetrieveThread.Stop();
			chazRetrieveThread = null;
		}
		if (chazControl.poweredUp && jobHeadset != null && jobHeadset.activeInHierarchy)
		{
			chazControl.GrabObject(jobHeadset, destroy: true);
		}
		chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		PersistentDataManager.OverwriteLastLoadedScene(EagerScenes.HUB_LEVEL);
		chazComments = false;
		chazDialogueSource.PurgeAllDialogue();
		if (wasInJail)
		{
			JailFollowup();
		}
		else
		{
			JobFollowup();
		}
		yield return null;
		if (PersistentDataManager.GetJobState() + 1 <= JobStates.LOOP)
		{
			PersistentDataManager.OverwriteJobState(PersistentDataManager.GetJobState() + 1);
			jobState = PersistentDataManager.GetJobState();
		}
		reviewScript.UpdateReviewData();
	}

	protected void JobFollowup()
	{
		jobFollowupThread = GlobalCoroutineContainer.AddThread(JobFollowupCoroutine(), this);
	}

	private IEnumerator JobFollowupCoroutine()
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0f, this);
		bool smallTalk = false;
		switch (jobState)
		{
		case JobStates.Macy:
			EagerScenes.instance.headsets[EagerScenes.instance.headsets.Count - 1].spawnScale = 1f;
			PlayerScaler.instance.ScaleHead(Vector3.one);
			chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.macy);
			while (chazDialogueSource.isPlaying)
			{
				yield return null;
			}
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.7f, this);
			yield return SetUpFlipper();
			break;
		case JobStates.Garden:
			chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.gardenOne);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
			pot.transform.position = chazControl.head.transform.position + Vector3.up * 0.2f;
			pot.SetActive(value: true);
			spawnObjSound.Play();
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(7f, this);
			chazControl.WalkTo(behindAestheticObject.position);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.5f, this);
			if (!hasDoneFlipTutorial)
			{
				shouldDoFlipTutorial = true;
			}
			break;
		case JobStates.Sunset:
			chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.sunset);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(5f, this);
			reviewScript.SelectReviewTile(0);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.5f, this);
			reviewScript.HideReview();
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.25f, this);
			reviewScript.SelectReviewTile(2);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.5f, this);
			reviewScript.HideReview();
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.25f, this);
			reviewScript.SelectReviewTile(3);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
			reviewScript.HideReview();
			while (chazDialogueSource.isPlaying)
			{
				yield return null;
			}
			chazControl.WalkTo(chazCorner.position);
			chazControl.EyeLookAt(chazCorner, 0f, ChazController.eyeColor.Default, immediately: true);
			break;
		case JobStates.Tina:
			if (tinaGoodEnding)
			{
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.tinaGood);
			}
			else
			{
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.tinaBad);
			}
			break;
		case JobStates.Tumbleweed1:
			yield return null;
			break;
		case JobStates.Union2Vacuum:
			chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.unionOutro);
			break;
		default:
			smallTalk = true;
			break;
		}
		EagerScenes.instance.LoadQueue(EagerScenes.JAIL_LEVEL);
		ResetHubVariables();
		SafeDebug.Log("<color=blue>GiveNextJob smalltalk jobfollowup + smalltalk: </color>" + smallTalk);
		GiveNextJob(smallTalk);
		yield return null;
	}

	protected void JailFollowup()
	{
		jailFollowupThread = GlobalCoroutineContainer.AddThread(JailFollowupCoroutine(), this);
	}

	protected void InterruptJob()
	{
		if (jobInterruptThread == null || Time.time - lastTime > 15f)
		{
			KillAllMainThreads();
			jobInterruptThread = GlobalCoroutineContainer.AddThread(InterruptJobCoroutine(), this);
		}
	}

	private IEnumerator InterruptJobCoroutine()
	{
		SafeDebug.Log("<color=cyan>Get back to the job Bee.</color>");
		chazComments = false;
		if (jobHeadset != null && jobHeadset.activeInHierarchy)
		{
			chazControl.GrabObject(jobHeadset);
		}
		ChazRetrieveHeadsets();
		if (wasInBackstage)
		{
			if (jobSuspiciousInterruptIndex >= hubContent.dialogue.suspiciousInter.Length)
			{
				jobSuspiciousInterruptIndex = 0;
			}
			else
			{
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.suspiciousInter[jobSuspiciousInterruptIndex]);
			}
			jobSuspiciousInterruptIndex++;
			PersistentDataManager.OverwriteSuspiciousIndex(jobSuspiciousInterruptIndex);
		}
		else if (hasInterrupted)
		{
			if (jobGenericInterruptIndex >= hubContent.dialogue.genericInter.Length)
			{
				jobGenericInterruptIndex = 0;
			}
			if (UnityEngine.Random.Range(0, 8) == 0)
			{
				jobInterruptThread = GlobalCoroutineContainer.AddThread(CommentWhistle(), this);
				yield break;
			}
			int num = UnityEngine.Random.Range(0, hubContent.dialogue.genericInter.Length);
			while (num == lastJobInterruptIndex)
			{
				num = UnityEngine.Random.Range(0, hubContent.dialogue.genericInter.Length);
				if (num >= hubContent.dialogue.genericInter.Length - 3)
				{
					chazControl.EyeLookAt(nervousEyeShaker.transform, 0f, ChazController.eyeColor.Default, immediately: true);
				}
				else
				{
					chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
				}
			}
			chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.genericInter[jobGenericInterruptIndex]);
			jobGenericInterruptIndex++;
			PersistentDataManager.OverwriteGenericIndex(jobGenericInterruptIndex);
			lastJobInterruptIndex = jobGenericInterruptIndex;
		}
		else
		{
			switch (jobState)
			{
			case JobStates.ButterSnack:
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.butterTwoInter);
				break;
			case JobStates.Macy:
				if (!hasCompletedJob)
				{
					chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.macyInter);
				}
				break;
			case JobStates.Garden:
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.gardenOneInter);
				break;
			case JobStates.GardenNight:
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.gardenTwoInter);
				break;
			case JobStates.Sunset:
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.sunsetInter);
				break;
			case JobStates.Tina:
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.tinaInter);
				break;
			case JobStates.Tumbleweed1:
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.tumbleOneInter);
				break;
			case JobStates.Tumbleweed2:
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.tumbleTwoInter);
				break;
			default:
				chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.genericInter[UnityEngine.Random.Range(0, hubContent.dialogue.genericInter.Length)]);
				break;
			}
			hasInterrupted = true;
		}
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		chazComments = true;
		SafeDebug.Log("chazComments = true jailInterruptionCoroutine");
		jobInterruptThread = null;
	}

	protected void ChazRetrieveHeadsets()
	{
		chazRetrieveThread = GlobalCoroutineContainer.AddThread(ChazRetrieveCoroutine(), this);
	}

	protected void ChazStopRetrievingHeadsets()
	{
		if (chazRetrieveThread != null && chazRetrieveThread.IsRunning)
		{
			chazRetrieveThread.Stop();
			chazRetrieveThread = null;
		}
	}

	private IEnumerator ChazRetrieveCoroutine()
	{
		float t = 0f;
		yield return null;
		while (t < 5f)
		{
			if (chazControl.poweredUp && !chazControl.isPresenting && !ToolManager.PlayerGrabbingGameObject(jobHeadset))
			{
				t += Time.deltaTime;
				if (t >= 5f && jobHeadset != null && jobHeadset.activeInHierarchy)
				{
					chazControl.GrabObject(jobHeadset);
					t = 0f;
				}
			}
			else
			{
				t = 0f;
			}
			if (!chazControl.isActiveAndEnabled)
			{
				yield break;
			}
			yield return null;
		}
		yield return null;
	}

	private void Update()
	{
		UpdateText();
	}

	private void UpdateText()
	{
		ticker += Time.deltaTime;
		if (ticker < 1f)
		{
			return;
		}
		if (!jobTimerStarted && !searchingUI.activeInHierarchy)
		{
			nextJobText.UseAltString("- : - -");
		}
		else if (!jobTimerStarted && searchingUI.activeInHierarchy)
		{
			nextJobText.UseAltString(string.Empty);
		}
		else if (nextJobTimer >= 0f)
		{
			if (seconds != Mathf.FloorToInt(Time.realtimeSinceStartup % 60f))
			{
				clockTic.Play();
			}
			minutes = Mathf.FloorToInt(nextJobTimer / 60f);
			seconds = Mathf.FloorToInt(nextJobTimer % 60f);
			if (seconds < 10)
			{
				nextJobText.UseAltString(minutes + ":0" + seconds);
			}
			else
			{
				nextJobText.UseAltString(minutes + ":" + seconds);
			}
			ticker = 0f;
		}
		else
		{
			nextJobText.SetNewKey("TEXT_328");
			nextJobText.UseAltString(alt: false);
		}
		minutes = Mathf.FloorToInt(Time.realtimeSinceStartup / 60f);
		seconds = Mathf.FloorToInt(Time.realtimeSinceStartup % 60f);
		if (seconds < 10)
		{
			sessionTimerText.text = minutes + ":0" + seconds;
		}
		else
		{
			sessionTimerText.text = minutes + ":" + seconds;
		}
	}
}
