using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VVR_Eliza : SceneBehaviorManager
{
	public enum ElizaStates
	{
		ElizaOff,
		ElizaBootingUp,
		ElizaOn,
		ElizaWalkToSun,
		ElizaLookAtSun,
		ElizaShellSection,
		ElizaEnding,
		PostAdmin
	}

	[SerializeField]
	private ElizaStates elizaState;

	public ElizaSequenceScriptableObject tweakables;

	public static VVR_Eliza instance;

	[Header("Fill-Ins for Environments")]
	public VVRConsumable[] itemsToFillSunset;

	public VVRConsumer sunsetVacuum;

	public Transform sunsetStagePosition;

	public VVRConsumable[] itemsToFillSpace;

	public VVRConsumer spaceVacuum;

	public Transform spaceStagePosition;

	public VVRConsumable[] itemsToFillCanyon;

	public VVRConsumer canyonVacuum;

	public Transform canyonStagePosition;

	public VVRConsumable[] itemsToFillCave;

	public VVRConsumer caveVacuum;

	public Transform caveStagePosition;

	public VVRConsumable[] itemsToFillVolcano;

	public VVRConsumer volcanoVacuum;

	public Transform volcanoStagePosition;

	private Vector3 elizaStageCenterPosition;

	[Header("Eliza References")]
	public GameObject eliza;

	public Collider[] elizaColliders;

	public GameObject elizaTeleportTarget;

	public Rigidbody elizaPositioner;

	public ElizaEyeStates elizaEyes;

	public ElizaMouth elizaMouth;

	public Transform elizaSunSetTarget;

	public AudioLoop backgroundAdvertisement;

	public AudioLoop windLoop;

	public AudioLoop rumbleLoop;

	public AudioEvent success;

	public AudioEvent errorAudioEvent;

	public AudioEvent onAudioEvent;

	public AudioEvent offAudioEvent;

	public AudioEvent teleportAudioEvent;

	public SeaShell[] seaShells;

	public GvrAudioSource finalSeaShell;

	private Animator finalSeaShellAnimator;

	public GameObject fakeFinalSeaShell;

	public Transform shellHolder;

	public Vector3 shellRotateRate = new Vector3(0f, 10f, 0f);

	private bool rotateShells;

	public GameObject glitchSubtitles;

	[Header("Environment Complete References")]
	public GameObject elizaRideBalloon;

	public Animator[] elizaCanyonBalloons;

	public Animator[] canyonBigBalloonIdleAnim;

	public Animator sunsetFlower;

	public Animator caveFlashlight;

	public Animator volcanoUmbrella;

	public AudioEvent volcanoBoom;

	public GameObject[] volcanoMoveLetters;

	public Transform[] volcanoMoveTargets;

	public Vector3[] volcanoLetterStartPositions;

	public Vector3[] volcanoLetterStartEuler;

	public Transform volcanoLava;

	private float volcanoT;

	private float volcanoResetTime;

	private float mermanT;

	private float mermanResetTime;

	public GameObject[] caveFires;

	public GameObject caveColliders;

	public ParticleSystem eruptionShockwave;

	public FireFlicker[] eruptionLightFlicker;

	private bool _elizaIsOn;

	private bool startSunsetSequence;

	private int shellsListenedTo;

	private bool shellResponseQueued;

	private float paddingTimer;

	private float moveLerp;

	private float rotateTime;

	private float rotateT;

	private float moveT;

	private float onTime;

	private float offTime;

	private float onT;

	private Vector3 riseStartPosition;

	private Vector3 basePosition;

	private Vector3 startPosition;

	private Vector3 jointDifferencePosition;

	private Quaternion startRotation;

	private ElizaRotationFollow elizaFollow;

	private ElizaEnvironments dynamicEnvironmentManager;

	public ElizaEnvironmentTrigger endingTrigger;

	private ElizaMusicHandler _musicHandler;

	private bool allowElizaToContinue = true;

	private bool skipToNextMonologueBeat;

	private bool glitchingIn;

	private bool glitchingOut;

	private bool glitching;

	private float glitchTime = 1f;

	private float glitchCount;

	private float blinkCountdown;

	private float blinkTriggerTimeMin = 1.2f;

	private float blinkTriggerTimeMax = 4f;

	private ElizaTeleportEffect elizaTeleport;

	private bool bgmFadingIn;

	private bool bgmFadingOut;

	private float bgmCurrentVolume;

	private float bgmMaxVolume;

	private bool bgmPitchingUp;

	private int environmentsFilled;

	private bool sunsetFilled;

	private float sunsetFillMax;

	private bool queueSunset;

	private bool canyonFilled;

	private float canyonFillMax;

	private bool queueCanyon;

	private bool volcanoFilled;

	private float volcanoFillMax;

	private bool queueVolcano;

	private bool caveFilled;

	private float caveFillMax;

	private bool queueCave;

	private bool spaceFilled;

	private float spaceFillMax;

	private bool queueSpace;

	private bool canAdvanceToEnd;

	private int currentShellSample;

	private bool hoverUp = true;

	private float elizaHoverTime;

	private float hoverT;

	private Vector3 elizaHoverPosition;

	[SerializeField]
	private VVRDialogueSource elizaDialogueSource;

	[SerializeField]
	public VVRDialogueSource saturnDialogueSource;

	[SerializeField]
	public VVRDialogueSource vesuviusDialogueSource;

	[SerializeField]
	public VVRDialogueSource mermanDialogueSource;

	[SerializeField]
	public VVRDialogueSource canyonDialogueSource;

	[SerializeField]
	private SubtitleRenderer finalShellSubtitles;

	[SerializeField]
	private GameObject canyonSubtitle;

	[SerializeField]
	private GameObject spaceSubtitle;

	[SerializeField]
	private GameObject beachSubtitle;

	[SerializeField]
	private GameObject volcanoSubtitle;

	[SerializeField]
	private GameObject canyonRoot;

	[SerializeField]
	private GameObject spaceRoot;

	[SerializeField]
	private GameObject beachRoot;

	[SerializeField]
	private GameObject volcanoRoot;

	[SerializeField]
	private GameObject caveRoot;

	[SerializeField]
	private GameObject[] allCenterObjects;

	[SerializeField]
	private GameObject chazGlitchRoot;

	[SerializeField]
	private GameObject[] chazBots;

	private Renderer[] oRenderers;

	private GvrAudioSource elizaGVR;

	[HideInInspector]
	public bool elizaUnlocked;

	[HideInInspector]
	public bool allowEnvironmentChange = true;

	private bool warmup = true;

	private VVRConsumable[] consumableComponents;

	private bool hasSetSunsetUp;

	[SerializeField]
	private Material startMat;

	[SerializeField]
	private Material mouthMat;

	[SerializeField]
	private Material glitchMat;

	[HideInInspector]
	public bool glitchExitCondition;

	private VVRConsumable[] beachConsumables;

	private VVRConsumable[] planetConsumables;

	private VVRConsumable[] caveConsumables;

	private VVRConsumable[] volcanoConsumables;

	private VVRConsumable[] canyonConsumables;

	private ElizaEnvironmentProperties propPreGlitch;

	private bool storySequence;

	public ElizaStates currentElizaState => elizaState;

	public bool elizaIsOn => _elizaIsOn;

	public ElizaMusicHandler musicHandler => _musicHandler;

	private void setUpEliza()
	{
		if (PersistentDataManager.GetElizaData() != ElizaStates.ElizaOff)
		{
			eliza.SetActive(value: false);
			if (consumableComponents == null)
			{
				consumableComponents = eliza.GetComponentsInChildren<VVRConsumable>();
			}
			for (int i = 0; i < consumableComponents.Length; i++)
			{
				consumableComponents[i].currentObjectState = VVRConsumable.objectState.wholeStatic;
				consumableComponents[i].partialStartPercent = 1f;
				consumableComponents[i].unconsumable = true;
			}
		}
		elizaFollow = eliza.GetComponentInChildren<ElizaRotationFollow>();
		elizaFollow.SetElizaObject(eliza.GetComponent<ObjectBase>());
		eliza.SetActive(value: true);
	}

	private void Awake()
	{
		if (instance != null)
		{
			throw new UnityException("Only one instance of VVR_Eliza in a scene is allowed.");
		}
		instance = this;
	}

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		beachConsumables = beachRoot.GetComponentsInChildren<VVRConsumable>();
		canyonConsumables = canyonRoot.GetComponentsInChildren<VVRConsumable>();
		planetConsumables = spaceRoot.GetComponentsInChildren<VVRConsumable>();
		caveConsumables = caveRoot.GetComponentsInChildren<VVRConsumable>();
		volcanoConsumables = volcanoRoot.GetComponentsInChildren<VVRConsumable>();
		consumableComponents = eliza.GetComponentsInChildren<VVRConsumable>();
		environmentsFilled = PersistentDataManager.GetElizaEnvironmentsFilled();
		elizaState = PersistentDataManager.GetElizaData();
		setUpEliza();
		SetUpSceneObjects();
		ElizaBlockyCreation[] componentsInChildren = eliza.GetComponentsInChildren<ElizaBlockyCreation>();
		oRenderers = new Renderer[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			oRenderers[i] = componentsInChildren[i].oRenderer;
		}
		if (elizaState != ElizaStates.ElizaOff)
		{
			enableScenes();
			allowEnvironmentChange = true;
			dynamicEnvironmentManager.SetUseCamera(newC: true);
		}
		if (elizaState == ElizaStates.ElizaWalkToSun || elizaState == ElizaStates.ElizaShellSection || elizaState == ElizaStates.ElizaLookAtSun)
		{
			elizaState = ElizaStates.ElizaWalkToSun;
			_elizaIsOn = true;
			PersistentDataManager.OverwriteElizaData(ElizaStates.ElizaWalkToSun);
			allowEnvironmentChange = true;
			dynamicEnvironmentManager.SetNewArea(-1);
			eliza.GetComponent<SpringJoint>().breakForce = 0f;
			eliza.GetComponent<Rigidbody>().isKinematic = true;
			if (elizaFollow.elizaObjReference != null)
			{
				elizaFollow.elizaObjReference.ungrabbable = true;
			}
		}
		GlobalCoroutineContainer.AddThread(ElizaCoroutine(), this);
	}

	private void SetUpSceneObjects()
	{
		elizaDialogueSource = eliza.GetComponent<VVRDialogueSource>();
		elizaGVR = elizaDialogueSource.GetComponent<GvrAudioSource>();
		elizaTeleport = eliza.GetComponent<ElizaTeleportEffect>();
		_musicHandler = GetComponent<ElizaMusicHandler>();
		elizaStageCenterPosition = elizaTeleportTarget.transform.position;
		elizaEyes.SetFollowReference(elizaFollow);
		elizaEyes.TurnEyesOffInstant();
		basePosition = elizaPositioner.transform.localPosition;
		elizaPositioner.detectCollisions = false;
		shellsListenedTo = 0;
		for (int i = 0; i < seaShells.Length; i++)
		{
			seaShells[i].gameObject.SetActive(value: false);
		}
		finalSeaShell.gameObject.SetActive(value: false);
		finalSeaShellAnimator = finalSeaShell.GetComponentInChildren<Animator>();
		finalSeaShellAnimator.enabled = false;
		startPosition = elizaPositioner.transform.localPosition;
		jointDifferencePosition = eliza.transform.position - elizaPositioner.transform.position;
		bgmCurrentVolume = (bgmMaxVolume = 1f);
		backgroundAdvertisement.SetSourcePitch(tweakables.brokenMusicPitch);
		elizaOffImmediate();
		dynamicEnvironmentManager = GetComponent<ElizaEnvironments>();
		dynamicEnvironmentManager.SetNewArea(11);
		dynamicEnvironmentManager.SetUseCamera(newC: false);
		allowEnvironmentChange = false;
		windLoop.Play();
		rumbleLoop.Play();
		for (int j = 0; j < canyonBigBalloonIdleAnim.Length; j++)
		{
			canyonBigBalloonIdleAnim[j].enabled = false;
		}
		for (int k = 0; k < elizaCanyonBalloons.Length; k++)
		{
			elizaCanyonBalloons[k].gameObject.SetActive(value: false);
		}
		for (int l = 0; l < volcanoLetterStartPositions.Length; l++)
		{
			ref Vector3 reference = ref volcanoLetterStartPositions[l];
			reference = volcanoMoveLetters[l].transform.position;
			ref Vector3 reference2 = ref volcanoLetterStartEuler[l];
			reference2 = volcanoMoveLetters[l].transform.rotation.eulerAngles;
			volcanoMoveTargets[l].gameObject.SetActive(value: false);
		}
		volcanoUmbrella.gameObject.SetActive(value: false);
		sunsetFlower.gameObject.SetActive(value: false);
		caveFlashlight.gameObject.SetActive(value: false);
		sunsetVacuum.gameObject.SetActive(value: false);
		spaceVacuum.gameObject.SetActive(value: false);
		volcanoVacuum.gameObject.SetActive(value: false);
		canyonVacuum.gameObject.SetActive(value: false);
		caveVacuum.gameObject.SetActive(value: false);
		setFillChecks();
	}

	private void Update()
	{
		if (_elizaIsOn)
		{
			elizaHover();
			elizaBlink();
			if (rotateShells)
			{
				ShellsFloat();
			}
		}
		else
		{
			handleGrabPitch();
		}
		if (bgmPitchingUp)
		{
			PitchBGM();
		}
		HandleBGMVolume();
	}

	private IEnumerator ElizaCoroutine()
	{
		elizaUnlocked = true;
		elizaState = PersistentDataManager.GetElizaData();
		if (elizaState == ElizaStates.ElizaOff)
		{
			dynamicEnvironmentManager.SetNewArea(13, immediate: true);
			while (warmup)
			{
				if (IsFull())
				{
					warmup = false;
				}
				yield return null;
			}
			MakeElizaUnconsumable();
			storySequence = true;
			elizaFollow.elizaObjReference.ungrabbable = true;
			elizaRideBalloon.gameObject.SetActive(value: false);
			allowEnvironmentChange = true;
			musicHandler.playCenterLoop = true;
			FadeInMusic();
			success.Play();
			elizaDialogueSource.PlayClip(tweakables.dialogue.aElizaGreetingsSection01[0]);
			elizaOn();
			float startTime = Time.time;
			GlobalCoroutineContainer.AddThread(IntroSequence(), this);
			bool setUp = false;
			while (onTime < tweakables.turnOnTime)
			{
				if (Time.time - startTime > tweakables.introSceneFlashInterval + tweakables.introInitialGlitchTime && !setUp)
				{
					dynamicEnvironmentManager.SetNewArea(14);
					allowEnvironmentChange = false;
					setUp = true;
				}
				onTime += Time.deltaTime;
				elizaRise();
				yield return null;
			}
			while (!setUp)
			{
				if (Time.time - startTime > tweakables.introSceneFlashInterval + tweakables.introInitialGlitchTime)
				{
					dynamicEnvironmentManager.SetNewArea(14);
					allowEnvironmentChange = false;
					elizaFollow.elizaObjReference.ungrabbable = true;
					setUp = true;
				}
				yield return null;
			}
			_elizaIsOn = true;
			elizaFollow.ReevaluateTarget();
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(pitchDownTime(tweakables.dialogue.aElizaGreetingsSection01[0].clip) + 0.2f, this);
			makeScenesConsumable();
			storySequence = false;
			if (environmentsFilled < 3 || PersistentDataManager.GetElizaSunsetState() != 1)
			{
				elizaFollow.elizaObjReference.ForceDropByPlayer();
				elizaOff(allowEnviroChange: false);
				allowEnvironmentChange = true;
				dynamicEnvironmentManager.SetNewArea(11);
				while (offTime < tweakables.turnOffTime)
				{
					elizaFall();
					yield return null;
				}
				elizaFollow.elizaObjReference.ungrabbable = false;
				FadeOutMusic();
				elizaState = ElizaStates.ElizaBootingUp;
				elizaDialogueSource.PurgeAllDialogue();
				elizaDialogueSource.mAudioSource.Stop();
			}
			else if (PersistentDataManager.GetElizaSunsetState() == 1)
			{
				elizaState = ElizaStates.ElizaOn;
			}
		}
		if (elizaState == ElizaStates.ElizaBootingUp)
		{
			enableScenes();
			allowEnvironmentChange = true;
			PersistentDataManager.OverwriteElizaData(elizaState);
			if (PersistentDataManager.GetElizaSunsetState() == 1)
			{
				FillInSunset();
				dynamicEnvironmentManager.CompleteBeach();
				elizaOff();
				ActivateSubtitles();
			}
			if (PersistentDataManager.GetElizaCaveState() == 1)
			{
				FillInCave();
				dynamicEnvironmentManager.CompleteCave();
				elizaOff();
			}
			if (PersistentDataManager.GetElizaCanyonState() == 1)
			{
				FillInCanyon();
				dynamicEnvironmentManager.CompleteCanyon();
				elizaOff();
				ActivateSubtitles();
			}
			if (PersistentDataManager.GetElizaSpaceState() == 1)
			{
				FillInSpace();
				dynamicEnvironmentManager.CompleteSpace();
				elizaOff();
				ActivateSubtitles();
			}
			if (PersistentDataManager.GetElizaVolcanoState() == 1)
			{
				FillInVolcano();
				dynamicEnvironmentManager.CompleteVolcano();
				elizaOff();
				ActivateSubtitles();
			}
			elizaFollow.elizaObjReference.ungrabbable = false;
			while (!canAdvanceToEnd)
			{
				storySequence = false;
				if (!sunsetFilled && percentFilledSunset() >= tweakables.bootSunsetPercentage)
				{
					queueSunset = true;
					sunsetFilled = true;
				}
				if (!caveFilled && percentFilledCave() >= tweakables.bootCavePercentage)
				{
					queueCave = true;
					caveFilled = true;
				}
				if (!canyonFilled && percentFilledCanyon() >= tweakables.bootCanyonPercentage)
				{
					queueCanyon = true;
					canyonFilled = true;
				}
				if (!spaceFilled && percentFilledSpace() >= tweakables.bootSpacePercentage)
				{
					queueSpace = true;
					spaceFilled = true;
				}
				if (!volcanoFilled && percentFilledVolcano() >= tweakables.bootVolcanoPercentage)
				{
					queueVolcano = true;
					volcanoFilled = true;
				}
				ActivateSubtitles();
				if (queueSunset && PersistentDataManager.GetElizaSunsetState() != 1)
				{
					storySequence = true;
					FillInSunset();
					success.Play();
					eliza.gameObject.SetActive(value: false);
					elizaDialogueSource.mSubtitleRenderer.SetIsPlaying(s: false);
					if (elizaDialogueSource.GetClipQueue().Count > 0)
					{
						elizaDialogueSource.PurgePlaybackClipFromQueue(elizaDialogueSource.GetClipQueue()[0]);
					}
					elizaTeleportTarget.transform.position = sunsetStagePosition.position;
					dynamicEnvironmentManager.SetNewArea(0);
					allowEnvironmentChange = false;
					dynamicEnvironmentManager.CompleteBeach();
					FadeInMusic();
					eliza.gameObject.SetActive(value: true);
					teleportAudioEvent.Play();
					elizaTeleport.TeleportIn(1f);
					sunsetFlower.gameObject.SetActive(value: true);
					for (int i = 0; i < tweakables.dialogue.aSunsetDialogue.Length; i++)
					{
						elizaDialogueSource.PlayClip(tweakables.dialogue.aSunsetDialogue[i]);
						if (i == 0)
						{
							elizaEyes.SetEyesExcited();
							elizaOn();
							while (onTime < tweakables.turnOnTime)
							{
								elizaRise();
								yield return null;
							}
							_elizaIsOn = true;
							elizaFollow.elizaObjReference.ungrabbable = false;
						}
						else
						{
							elizaEyes.SetEyesResting();
						}
						while (elizaDialogueSource.isPlaying)
						{
							yield return null;
						}
					}
					elizaEyes.SetEyesResting();
					sunsetFlower.SetTrigger("Off");
					if (environmentsFilled < 4)
					{
						if (environmentsFilled == 0)
						{
							for (int j = 0; j < tweakables.dialogue.aElizaGreetingsSection02.Length; j++)
							{
								elizaDialogueSource.PlayClip(tweakables.dialogue.aElizaGreetingsSection02[j]);
								if (j == 0)
								{
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.cRelaxTimeStartTime, this);
									elizaEyes.SetEyesRelaxed();
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.cExcitementStartTime, this);
									elizaEyes.SetEyesExcited();
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(pitchDownTime(tweakables.dialogue.aElizaGreetingsSection02[j].clip) - tweakables.eyeTimings.cRelaxTimeStartTime - tweakables.eyeTimings.cExcitementStartTime + 2f, this);
								}
							}
							elizaEyes.SetEyesResting();
						}
						if (environmentsFilled == 1)
						{
							yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.turnPadding, this);
							for (int k = 0; k < tweakables.dialogue.aElizaGreetingsSection03.Length; k++)
							{
								elizaDialogueSource.PlayClip(tweakables.dialogue.aElizaGreetingsSection03[k]);
								if (k == 0)
								{
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.dMishapLookStartTime, this);
									elizaEyes.SetEyesLookLeft();
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.dMishapLookEndTime, this);
									elizaEyes.SetEyesResting();
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(pitchDownTime(tweakables.dialogue.aElizaGreetingsSection03[k].clip) - tweakables.eyeTimings.dMishapLookStartTime - tweakables.eyeTimings.dMishapLookEndTime, this);
								}
							}
							elizaEyes.SetEyesResting();
						}
						if (environmentsFilled == 2)
						{
							for (int l = 0; l < tweakables.dialogue.aElizaGreetingsSection04.Length; l++)
							{
								elizaDialogueSource.PlayClip(tweakables.dialogue.aElizaGreetingsSection04[l]);
								yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.hDollaStartTime, this);
								elizaEyes.SetEyesMoney();
								if (!sunsetFilled)
								{
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(pitchDownTime(tweakables.dialogue.aElizaGreetingsSection04[l].clip) - tweakables.eyeTimings.hDollaStartTime + 2f, this);
									continue;
								}
								while (elizaDialogueSource.isPlaying)
								{
									yield return null;
								}
							}
							elizaEyes.SetEyesResting();
						}
						if (environmentsFilled == 3)
						{
							for (int m = 0; m < tweakables.dialogue.aElizaGreetingsSection05.Length; m++)
							{
								elizaDialogueSource.PlayClip(tweakables.dialogue.aElizaGreetingsSection05[m]);
								if (m == 0)
								{
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.eProcessingStartTime, this);
									elizaEyes.SetEyesProcessing();
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.eProcessingEndTime, this);
									elizaEyes.SetEyesResting();
									if (!sunsetFilled)
									{
										yield return GlobalCoroutineContainer.SafeWaitForSeconds(pitchDownTime(tweakables.dialogue.aElizaGreetingsSection05[m].clip) - tweakables.eyeTimings.eProcessingStartTime - tweakables.eyeTimings.eProcessingEndTime + 2f, this);
									}
								}
								if (sunsetFilled)
								{
									while (elizaDialogueSource.isPlaying)
									{
										yield return null;
									}
								}
							}
							elizaEyes.SetEyesResting();
						}
					}
					if (!sunsetFilled || environmentsFilled < 2)
					{
						if (elizaFollow.elizaObjReference.isGrabbed)
						{
							elizaFollow.elizaObjReference.ForceDropByPlayer();
							elizaFollow.elizaObjReference.ungrabbable = true;
						}
						elizaTeleport.TeleportOut(tweakables.turnOffTime - 0.1f);
						elizaOff();
						while (offTime < tweakables.turnOffTime)
						{
							elizaFall();
							yield return null;
						}
						elizaFollow.elizaObjReference.ungrabbable = false;
					}
					sunsetFlower.gameObject.SetActive(value: false);
					queueSunset = false;
					environmentsFilled++;
					PersistentDataManager.SetElizaEnvironmentsFilled(environmentsFilled);
					PersistentDataManager.SetElizaSunsetState(1);
					allowEnvironmentChange = true;
				}
				if (queueCave && PersistentDataManager.GetElizaCaveState() != 1)
				{
					storySequence = true;
					FillInCave();
					success.Play();
					teleportAudioEvent.Play();
					elizaTeleport.TeleportIn(1f);
					elizaTeleportTarget.transform.position = caveStagePosition.position;
					dynamicEnvironmentManager.SetNewArea(6);
					allowEnvironmentChange = false;
					dynamicEnvironmentManager.CompleteCave();
					FadeInMusic();
					for (int n = 0; n < caveFires.Length; n++)
					{
						caveFires[n].SetActive(value: true);
					}
					for (int num = 0; num < tweakables.dialogue.aCaveDialogue.Length; num++)
					{
						elizaDialogueSource.PlayClip(tweakables.dialogue.aCaveDialogue[num]);
						if (num == 0)
						{
							elizaOn();
							while (onTime < tweakables.turnOnTime)
							{
								elizaRise();
								yield return null;
							}
							_elizaIsOn = true;
							elizaFollow.elizaObjReference.ungrabbable = false;
							caveFlashlight.gameObject.SetActive(value: true);
							yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.aEyesNarrowStartTime, this);
							elizaEyes.SetEyesNarrow();
							yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.aEyesNarrowEndTime, this);
							elizaEyes.SetEyesResting();
						}
						while (elizaDialogueSource.isPlaying)
						{
							yield return null;
						}
					}
					caveFlashlight.SetTrigger("Off");
					if (!sunsetFilled || environmentsFilled < 7)
					{
						if (elizaFollow.elizaObjReference.isGrabbed)
						{
							elizaFollow.elizaObjReference.ForceDropByPlayer();
							elizaFollow.elizaObjReference.ungrabbable = true;
						}
						elizaTeleport.TeleportOut(tweakables.turnOffTime - 0.1f);
						elizaOff();
						while (offTime < tweakables.turnOffTime)
						{
							elizaFall();
							yield return null;
						}
						elizaFollow.elizaObjReference.ungrabbable = false;
					}
					queueCave = false;
					PersistentDataManager.SetElizaCaveState(1);
					allowEnvironmentChange = true;
				}
				if (queueCanyon && PersistentDataManager.GetElizaCanyonState() != 1)
				{
					storySequence = true;
					FillInCanyon();
					success.Play();
					teleportAudioEvent.Play();
					elizaTeleport.TeleportIn(1f);
					elizaTeleportTarget.transform.position = canyonStagePosition.position;
					elizaTeleportTarget.transform.parent = canyonStagePosition.transform;
					dynamicEnvironmentManager.SetNewArea(9);
					allowEnvironmentChange = false;
					dynamicEnvironmentManager.CompleteCanyon();
					FadeInMusic();
					for (int num2 = 0; num2 < tweakables.dialogue.aCanyonDialogue.Length; num2++)
					{
						elizaDialogueSource.PlayClip(tweakables.dialogue.aCanyonDialogue[num2]);
						if (num2 == 0)
						{
							elizaEyes.SetEyesHappy();
							elizaOn();
							while (onTime < tweakables.turnOnTime)
							{
								elizaRise();
								yield return null;
							}
							_elizaIsOn = true;
							elizaFollow.elizaObjReference.ungrabbable = false;
							for (int num3 = 0; num3 < canyonBigBalloonIdleAnim.Length; num3++)
							{
								canyonBigBalloonIdleAnim[num3].enabled = true;
								yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.canyonBigBalloonOnTime, this);
							}
						}
						if (num2 == 1)
						{
							for (int num4 = 0; num4 < elizaCanyonBalloons.Length; num4++)
							{
								elizaCanyonBalloons[num4].gameObject.SetActive(value: true);
								yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.canyonSmallBalloonOnTime, this);
							}
						}
						while (elizaDialogueSource.isPlaying)
						{
							yield return null;
						}
					}
					elizaEyes.SetEyesResting();
					if (environmentsFilled < 4)
					{
						if (environmentsFilled == 0)
						{
							for (int num5 = 0; num5 < tweakables.dialogue.aElizaGreetingsSection02.Length; num5++)
							{
								elizaDialogueSource.PlayClip(tweakables.dialogue.aElizaGreetingsSection02[num5]);
								if (num5 == 0)
								{
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.cRelaxTimeStartTime, this);
									elizaEyes.SetEyesRelaxed();
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.cExcitementStartTime, this);
									elizaEyes.SetEyesExcited();
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(pitchDownTime(tweakables.dialogue.aElizaGreetingsSection02[num5].clip) - tweakables.eyeTimings.cRelaxTimeStartTime - tweakables.eyeTimings.cExcitementStartTime + 2f, this);
								}
							}
							elizaEyes.SetEyesResting();
						}
						if (environmentsFilled == 1)
						{
							yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.turnPadding, this);
							for (int num6 = 0; num6 < tweakables.dialogue.aElizaGreetingsSection03.Length; num6++)
							{
								elizaDialogueSource.PlayClip(tweakables.dialogue.aElizaGreetingsSection03[num6]);
								if (num6 == 0)
								{
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.dMishapLookStartTime, this);
									elizaEyes.SetEyesLookLeft();
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.dMishapLookEndTime, this);
									elizaEyes.SetEyesResting();
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(pitchDownTime(tweakables.dialogue.aElizaGreetingsSection03[num6].clip) - tweakables.eyeTimings.dMishapLookStartTime - tweakables.eyeTimings.dMishapLookEndTime, this);
								}
							}
							elizaEyes.SetEyesResting();
						}
						if (environmentsFilled == 2)
						{
							for (int num7 = 0; num7 < tweakables.dialogue.aElizaGreetingsSection04.Length; num7++)
							{
								elizaDialogueSource.PlayClip(tweakables.dialogue.aElizaGreetingsSection04[num7]);
								yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.hDollaStartTime, this);
								elizaEyes.SetEyesMoney();
								if (!sunsetFilled)
								{
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(pitchDownTime(tweakables.dialogue.aElizaGreetingsSection04[num7].clip) - tweakables.eyeTimings.hDollaStartTime + 2f, this);
									continue;
								}
								while (elizaDialogueSource.isPlaying)
								{
									yield return null;
								}
							}
							elizaEyes.SetEyesResting();
						}
						if (environmentsFilled == 3)
						{
							for (int num8 = 0; num8 < tweakables.dialogue.aElizaGreetingsSection05.Length; num8++)
							{
								elizaDialogueSource.PlayClip(tweakables.dialogue.aElizaGreetingsSection05[num8]);
								if (num8 == 0)
								{
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.eProcessingStartTime, this);
									elizaEyes.SetEyesProcessing();
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.eProcessingEndTime, this);
									elizaEyes.SetEyesResting();
									if (!sunsetFilled)
									{
										yield return GlobalCoroutineContainer.SafeWaitForSeconds(pitchDownTime(tweakables.dialogue.aElizaGreetingsSection05[num8].clip) - tweakables.eyeTimings.eProcessingStartTime - tweakables.eyeTimings.eProcessingEndTime + 2f, this);
									}
								}
								if (sunsetFilled)
								{
									while (elizaDialogueSource.isPlaying)
									{
										yield return null;
									}
								}
							}
							elizaEyes.SetEyesResting();
						}
					}
					if (!sunsetFilled || environmentsFilled < 2)
					{
						if (elizaFollow.elizaObjReference.isGrabbed)
						{
							elizaFollow.elizaObjReference.ForceDropByPlayer();
							elizaFollow.elizaObjReference.ungrabbable = true;
						}
						elizaTeleport.TeleportOut(tweakables.turnOffTime - 0.1f);
						elizaOff();
						while (offTime < tweakables.turnOffTime)
						{
							elizaFall();
							yield return null;
						}
						elizaFollow.elizaObjReference.ungrabbable = false;
					}
					queueCanyon = false;
					environmentsFilled++;
					PersistentDataManager.SetElizaEnvironmentsFilled(environmentsFilled);
					elizaTeleportTarget.transform.parent = transform;
					PersistentDataManager.SetElizaCanyonState(1);
					allowEnvironmentChange = true;
				}
				if (queueVolcano && PersistentDataManager.GetElizaVolcanoState() != 1)
				{
					storySequence = true;
					FillInVolcano();
					success.Play();
					eliza.gameObject.SetActive(value: false);
					elizaDialogueSource.mSubtitleRenderer.SetIsPlaying(s: false);
					elizaTeleportTarget.transform.position = volcanoStagePosition.position;
					dynamicEnvironmentManager.SetNewArea(7);
					allowEnvironmentChange = false;
					dynamicEnvironmentManager.CompleteVolcano();
					FadeInMusic();
					VolcanoErupt();
					teleportAudioEvent.Play();
					eliza.gameObject.SetActive(value: true);
					elizaTeleport.TeleportIn(1f);
					while (volcanoResetTime < tweakables.vesuviusLetterTime)
					{
						ResetVesuvius();
						yield return null;
					}
					for (int num9 = 0; num9 < tweakables.dialogue.aVolcanoDialogue.Length; num9++)
					{
						elizaDialogueSource.PlayClip(tweakables.dialogue.aVolcanoDialogue[num9]);
						if (num9 == 0)
						{
							elizaOn();
							while (onTime < tweakables.turnOnTime)
							{
								elizaRise();
								yield return null;
							}
							_elizaIsOn = true;
							elizaFollow.elizaObjReference.ungrabbable = false;
							yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
						}
						while (elizaDialogueSource.isPlaying)
						{
							yield return null;
						}
					}
					volcanoUmbrella.SetTrigger("Off");
					if (environmentsFilled < 4)
					{
						if (environmentsFilled == 0)
						{
							for (int num10 = 0; num10 < tweakables.dialogue.aElizaGreetingsSection02.Length; num10++)
							{
								elizaDialogueSource.PlayClip(tweakables.dialogue.aElizaGreetingsSection02[num10]);
								if (num10 == 0)
								{
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.cRelaxTimeStartTime, this);
									elizaEyes.SetEyesRelaxed();
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.cExcitementStartTime, this);
									elizaEyes.SetEyesExcited();
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(pitchDownTime(tweakables.dialogue.aElizaGreetingsSection02[num10].clip) - tweakables.eyeTimings.cRelaxTimeStartTime - tweakables.eyeTimings.cExcitementStartTime + 2f, this);
								}
							}
							elizaEyes.SetEyesResting();
						}
						if (environmentsFilled == 1)
						{
							yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.turnPadding, this);
							for (int num11 = 0; num11 < tweakables.dialogue.aElizaGreetingsSection03.Length; num11++)
							{
								elizaDialogueSource.PlayClip(tweakables.dialogue.aElizaGreetingsSection03[num11]);
								if (num11 == 0)
								{
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.dMishapLookStartTime, this);
									elizaEyes.SetEyesLookLeft();
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.dMishapLookEndTime, this);
									elizaEyes.SetEyesResting();
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(pitchDownTime(tweakables.dialogue.aElizaGreetingsSection03[num11].clip) - tweakables.eyeTimings.dMishapLookStartTime - tweakables.eyeTimings.dMishapLookEndTime, this);
								}
							}
							elizaEyes.SetEyesResting();
						}
						if (environmentsFilled == 2)
						{
							for (int num12 = 0; num12 < tweakables.dialogue.aElizaGreetingsSection04.Length; num12++)
							{
								elizaDialogueSource.PlayClip(tweakables.dialogue.aElizaGreetingsSection04[num12]);
								yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.hDollaStartTime, this);
								elizaEyes.SetEyesMoney();
								if (!sunsetFilled)
								{
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(pitchDownTime(tweakables.dialogue.aElizaGreetingsSection04[num12].clip) - tweakables.eyeTimings.hDollaStartTime + 2f, this);
									continue;
								}
								while (elizaDialogueSource.isPlaying)
								{
									yield return null;
								}
							}
							elizaEyes.SetEyesResting();
						}
						if (environmentsFilled == 3)
						{
							for (int num13 = 0; num13 < tweakables.dialogue.aElizaGreetingsSection05.Length; num13++)
							{
								elizaDialogueSource.PlayClip(tweakables.dialogue.aElizaGreetingsSection05[num13]);
								if (num13 == 0)
								{
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.eProcessingStartTime, this);
									elizaEyes.SetEyesProcessing();
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.eProcessingEndTime, this);
									elizaEyes.SetEyesResting();
									if (!sunsetFilled)
									{
										yield return GlobalCoroutineContainer.SafeWaitForSeconds(pitchDownTime(tweakables.dialogue.aElizaGreetingsSection05[num13].clip) - tweakables.eyeTimings.eProcessingStartTime - tweakables.eyeTimings.eProcessingEndTime + 2f, this);
									}
								}
								if (sunsetFilled)
								{
									while (elizaDialogueSource.isPlaying)
									{
										yield return null;
									}
								}
							}
							elizaEyes.SetEyesResting();
						}
					}
					if (!sunsetFilled || environmentsFilled < 2)
					{
						if (elizaFollow.elizaObjReference.isGrabbed)
						{
							elizaFollow.elizaObjReference.ForceDropByPlayer();
							elizaFollow.elizaObjReference.ungrabbable = true;
						}
						elizaTeleport.TeleportOut(tweakables.turnOffTime - 0.1f);
						elizaOff();
						while (offTime < tweakables.turnOffTime)
						{
							elizaFall();
							yield return null;
						}
						elizaFollow.elizaObjReference.ungrabbable = false;
					}
					volcanoUmbrella.gameObject.SetActive(value: false);
					queueVolcano = false;
					environmentsFilled++;
					PersistentDataManager.SetElizaEnvironmentsFilled(environmentsFilled);
					PersistentDataManager.SetElizaVolcanoState(1);
					allowEnvironmentChange = true;
				}
				if (queueSpace && PersistentDataManager.GetElizaSpaceState() != 1)
				{
					storySequence = true;
					FillInSpace();
					success.Play();
					eliza.gameObject.SetActive(value: false);
					elizaDialogueSource.mSubtitleRenderer.SetIsPlaying(s: false);
					if (elizaDialogueSource.GetClipQueue().Count > 0)
					{
						elizaDialogueSource.PurgePlaybackClipFromQueue(elizaDialogueSource.GetClipQueue()[0]);
					}
					elizaTeleportTarget.transform.position = spaceStagePosition.position;
					dynamicEnvironmentManager.SetNewArea(8);
					allowEnvironmentChange = false;
					dynamicEnvironmentManager.CompleteSpace();
					FadeInMusic();
					teleportAudioEvent.Play();
					eliza.gameObject.SetActive(value: true);
					elizaTeleport.TeleportIn(1f);
					for (int num14 = 0; num14 < tweakables.dialogue.aSpaceDialogue.Length; num14++)
					{
						elizaDialogueSource.PlayClip(tweakables.dialogue.aSpaceDialogue[num14]);
						if (num14 == 0)
						{
							elizaOn();
							while (onTime < tweakables.turnOnTime)
							{
								elizaRise();
								yield return null;
							}
							_elizaIsOn = true;
							elizaFollow.elizaObjReference.ungrabbable = false;
							yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.bOutOfThisWorldWinkTime, this);
							elizaEyes.SetEyesWink();
						}
						while (elizaDialogueSource.isPlaying)
						{
							yield return null;
						}
					}
					elizaEyes.SetEyesResting();
					if (environmentsFilled < 4)
					{
						if (environmentsFilled == 0)
						{
							for (int num15 = 0; num15 < tweakables.dialogue.aElizaGreetingsSection02.Length; num15++)
							{
								elizaDialogueSource.PlayClip(tweakables.dialogue.aElizaGreetingsSection02[num15]);
								if (num15 == 0)
								{
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.cRelaxTimeStartTime, this);
									elizaEyes.SetEyesRelaxed();
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.cExcitementStartTime, this);
									elizaEyes.SetEyesExcited();
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(pitchDownTime(tweakables.dialogue.aElizaGreetingsSection02[num15].clip) - tweakables.eyeTimings.cRelaxTimeStartTime - tweakables.eyeTimings.cExcitementStartTime + 2f, this);
								}
							}
							elizaEyes.SetEyesResting();
						}
						if (environmentsFilled == 1)
						{
							yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.turnPadding, this);
							for (int num16 = 0; num16 < tweakables.dialogue.aElizaGreetingsSection03.Length; num16++)
							{
								elizaDialogueSource.PlayClip(tweakables.dialogue.aElizaGreetingsSection03[num16]);
								if (num16 == 0)
								{
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.dMishapLookStartTime, this);
									elizaEyes.SetEyesLookLeft();
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.dMishapLookEndTime, this);
									elizaEyes.SetEyesResting();
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(pitchDownTime(tweakables.dialogue.aElizaGreetingsSection03[num16].clip) - tweakables.eyeTimings.dMishapLookStartTime - tweakables.eyeTimings.dMishapLookEndTime, this);
								}
							}
							elizaEyes.SetEyesResting();
						}
						if (environmentsFilled == 2)
						{
							for (int num17 = 0; num17 < tweakables.dialogue.aElizaGreetingsSection04.Length; num17++)
							{
								elizaDialogueSource.PlayClip(tweakables.dialogue.aElizaGreetingsSection04[num17]);
								yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.hDollaStartTime, this);
								elizaEyes.SetEyesMoney();
								if (!sunsetFilled)
								{
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(pitchDownTime(tweakables.dialogue.aElizaGreetingsSection04[num17].clip) - tweakables.eyeTimings.hDollaStartTime + 2f, this);
									continue;
								}
								while (elizaDialogueSource.isPlaying)
								{
									yield return null;
								}
							}
							elizaEyes.SetEyesResting();
						}
						if (environmentsFilled == 3)
						{
							for (int num18 = 0; num18 < tweakables.dialogue.aElizaGreetingsSection05.Length; num18++)
							{
								elizaDialogueSource.PlayClip(tweakables.dialogue.aElizaGreetingsSection05[num18]);
								if (num18 == 0)
								{
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.eProcessingStartTime, this);
									elizaEyes.SetEyesProcessing();
									yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.eProcessingEndTime, this);
									elizaEyes.SetEyesResting();
									if (!sunsetFilled)
									{
										yield return GlobalCoroutineContainer.SafeWaitForSeconds(pitchDownTime(tweakables.dialogue.aElizaGreetingsSection05[num18].clip) - tweakables.eyeTimings.eProcessingStartTime - tweakables.eyeTimings.eProcessingEndTime + 2f, this);
									}
								}
								if (sunsetFilled)
								{
									while (elizaDialogueSource.isPlaying)
									{
										yield return null;
									}
								}
							}
							elizaEyes.SetEyesResting();
						}
					}
					if (!sunsetFilled || environmentsFilled < 2)
					{
						if (elizaFollow.elizaObjReference.isGrabbed)
						{
							elizaFollow.elizaObjReference.ForceDropByPlayer();
							elizaFollow.elizaObjReference.ungrabbable = true;
						}
						elizaTeleport.TeleportOut(tweakables.turnOffTime - 0.1f);
						elizaOff();
						while (offTime < tweakables.turnOffTime)
						{
							elizaFall();
							yield return null;
						}
						elizaFollow.elizaObjReference.ungrabbable = false;
					}
					queueSpace = false;
					environmentsFilled++;
					PersistentDataManager.SetElizaEnvironmentsFilled(environmentsFilled);
					PersistentDataManager.SetElizaSpaceState(1);
					allowEnvironmentChange = true;
				}
				if (environmentsFilled >= 3 && sunsetFilled)
				{
					canAdvanceToEnd = true;
				}
				yield return null;
			}
			elizaFollow.elizaObjReference.ungrabbable = true;
			allowEnvironmentChange = false;
			_elizaIsOn = true;
			elizaState = ElizaStates.ElizaOn;
		}
		if (elizaState == ElizaStates.ElizaOn)
		{
			enableScenes();
			if (elizaFollow.elizaObjReference.isGrabbed)
			{
				elizaFollow.elizaObjReference.ForceDropByPlayer();
				basePosition = elizaPositioner.position + jointDifferencePosition;
			}
			else
			{
				basePosition = eliza.transform.position;
			}
			eliza.GetComponent<SpringJoint>().breakForce = 0f;
			eliza.GetComponent<Rigidbody>().isKinematic = true;
			elizaFollow.elizaObjReference.ungrabbable = true;
			hasSetSunsetUp = true;
			for (int num19 = 0; num19 < tweakables.dialogue.bOnMonologue.Length; num19++)
			{
				elizaDialogueSource.PlayClip(tweakables.dialogue.bOnMonologue[num19]);
				if (num19 == 0)
				{
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.fDrabOfficesTime, this);
					elizaEyes.SetEyesHappy();
				}
				if (num19 == 1)
				{
					elizaEyes.SetEyesResting();
					elizaFollow.SetLookAway(newLookAway: true);
				}
				if (num19 == 2)
				{
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.fThoughtfulStartTime, this);
					elizaEyes.SetEyesConfused();
					elizaFollow.SetLookAway(newLookAway: false);
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.dialogue.bOnMonologue[num19].clip.length - tweakables.eyeTimings.fThoughtfulStartTime, this);
					elizaEyes.SetEyesResting();
					backgroundAdvertisement.Stop();
					glitchSubtitles.SetActive(value: true);
					errorAudioEvent.Play();
					propPreGlitch = dynamicEnvironmentManager.currentPropperties;
					GlobalCoroutineContainer.AddThread(GlitchWorld(1.5f, 0.25f, 1, finalGlitch: true), this);
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.turnPadding, this);
					_musicHandler.StopAllAds();
					elizaOff(allowEnviroChange: false);
					while (offTime < tweakables.turnOffTime)
					{
						elizaFall();
						yield return null;
					}
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.turnPadding, this);
					glitchSubtitles.SetActive(value: false);
					chazGlitchRoot.SetActive(value: false);
					elizaGVR.Stop();
					elizaDialogueSource.mSubtitleRenderer.SetIsPlaying(s: false);
					if (elizaDialogueSource.GetClipQueue().Count > 0)
					{
						elizaDialogueSource.PurgePlaybackClipFromQueue(elizaDialogueSource.GetClipQueue()[0]);
					}
					jointDifferencePosition = eliza.transform.position - elizaPositioner.transform.position;
					elizaOn(noAudio: true);
					ActivateSubtitles();
					while (onTime < tweakables.turnOnTime)
					{
						elizaRise();
						yield return null;
					}
					_elizaIsOn = true;
					basePosition = eliza.transform.position;
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
				}
				while (elizaDialogueSource.isPlaying)
				{
					yield return null;
				}
				if (num19 == 1)
				{
					elizaEyes.SetEyesProcessing();
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.fProcessingStartTime, this);
				}
			}
			elizaState = ElizaStates.ElizaWalkToSun;
		}
		if (elizaState == ElizaStates.ElizaWalkToSun)
		{
			if (!hasSetSunsetUp)
			{
				allowEnvironmentChange = true;
				dynamicEnvironmentManager.SetNewArea(-1);
				eliza.GetComponent<SpringJoint>().breakForce = 0f;
				eliza.GetComponent<Rigidbody>().isKinematic = true;
				if (elizaFollow.elizaObjReference != null)
				{
					elizaFollow.elizaObjReference.ungrabbable = true;
				}
			}
			enableScenes();
			elizaFollow.lookEnabled = false;
			turnOffElizaCollision();
			_musicHandler.StartPlayingFinal();
			rotateTime = tweakables.walkToSunTime * 0.3f;
			moveLerp = 0f;
			startPosition = elizaPositioner.transform.position;
			startRotation = eliza.transform.rotation;
			dynamicEnvironmentManager.SetNewArea(-1);
			while (moveLerp < tweakables.walkToSunTime)
			{
				moveToSun();
				yield return null;
			}
			turnOnElizaCollision();
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
			basePosition = eliza.transform.position;
			elizaState = ElizaStates.ElizaLookAtSun;
		}
		if (elizaState == ElizaStates.ElizaLookAtSun)
		{
			while (!startSunsetSequence)
			{
				if (endingTrigger.playerInRange)
				{
					startSunsetSequence = true;
				}
				yield return null;
			}
			elizaState = ElizaStates.ElizaShellSection;
		}
		if (elizaState == ElizaStates.ElizaShellSection)
		{
			PersistentDataManager.OverwriteElizaData(elizaState);
			elizaDialogueSource.PlayClip(tweakables.dialogue.cPresentFirstShellLine);
			for (int num20 = 0; num20 < seaShells.Length; num20++)
			{
				seaShells[num20].gameObject.SetActive(value: true);
				if (num20 < seaShells.Length - 1)
				{
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.timeBetweenShellsOn, this);
				}
			}
			while (elizaDialogueSource.isPlaying)
			{
				if (!rotateShells)
				{
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
					rotateShells = true;
				}
				yield return null;
			}
			if (!rotateShells)
			{
				rotateShells = true;
			}
			while (!allowElizaToContinue)
			{
				yield return null;
			}
			for (int num21 = 0; num21 < tweakables.dialogue.cShellSections; num21++)
			{
				if (num21 == 0)
				{
					for (int num22 = 0; num22 < tweakables.dialogue.cShellMonologueSection01.elizaMonologueClips.Length; num22++)
					{
						elizaDialogueSource.PlayClip(tweakables.dialogue.cShellMonologueSection01.elizaMonologueClips[num22]);
						while (elizaDialogueSource.isPlaying)
						{
							yield return null;
						}
						while (!allowElizaToContinue)
						{
							if (tweakables.dialogue.cShellMonologueSection01.allowSkipAtClip > -1 && tweakables.dialogue.cShellMonologueSection01.allowSkipAtClip <= num22)
							{
								skipToNextMonologueBeat = true;
							}
							yield return null;
						}
						if (skipToNextMonologueBeat)
						{
							num22 = tweakables.dialogue.cShellMonologueSection01.elizaMonologueClips.Length;
							skipToNextMonologueBeat = false;
						}
						if (shellResponseQueued)
						{
							shellResponseQueued = false;
						}
						paddingTimer = tweakables.encourageShellTime;
					}
					while (shellsListenedTo < tweakables.numShellsToAdvanceMonologue[num21])
					{
						if (shellResponseQueued)
						{
							shellResponseQueued = false;
						}
						if (paddingTimer > 0f)
						{
							if (!elizaDialogueSource.isPlaying && !shellsArePlaying())
							{
								paddingTimer -= Time.deltaTime;
							}
						}
						else
						{
							if (shellsListenedTo > 0)
							{
								elizaDialogueSource.PlayClip(tweakables.dialogue.cShellEncouragements.FireFeedbackClip());
							}
							else
							{
								elizaDialogueSource.PlayClip(tweakables.dialogue.cPreShellEncouragements.FireFeedbackClip());
							}
							paddingTimer = tweakables.encourageShellTime;
						}
						yield return null;
					}
				}
				if (num21 == 1)
				{
					for (int num23 = 0; num23 < tweakables.dialogue.cShellMonologueSection02.elizaMonologueClips.Length; num23++)
					{
						elizaDialogueSource.PlayClip(tweakables.dialogue.cShellMonologueSection02.elizaMonologueClips[num23]);
						while (elizaDialogueSource.isPlaying)
						{
							yield return null;
						}
						while (!allowElizaToContinue)
						{
							if (tweakables.dialogue.cShellMonologueSection02.allowSkipAtClip > -1 && tweakables.dialogue.cShellMonologueSection02.allowSkipAtClip <= num23)
							{
								skipToNextMonologueBeat = true;
							}
							yield return null;
						}
						if (skipToNextMonologueBeat)
						{
							num23 = tweakables.dialogue.cShellMonologueSection02.elizaMonologueClips.Length;
							skipToNextMonologueBeat = false;
						}
						if (shellResponseQueued)
						{
							shellResponseQueued = false;
						}
						paddingTimer = tweakables.encourageShellTime;
					}
					while (shellsListenedTo < tweakables.numShellsToAdvanceMonologue[num21])
					{
						if (shellResponseQueued)
						{
							shellResponseQueued = false;
						}
						if (paddingTimer > 0f)
						{
							if (!elizaDialogueSource.isPlaying && !shellsArePlaying())
							{
								paddingTimer -= Time.deltaTime;
							}
						}
						else
						{
							elizaDialogueSource.PlayClip(tweakables.dialogue.cShellEncouragements.FireFeedbackClip());
							paddingTimer = tweakables.encourageShellTime;
						}
						yield return null;
					}
				}
				if (num21 == 2)
				{
					for (int num24 = 0; num24 < tweakables.dialogue.cShellMonologueSection03.elizaMonologueClips.Length; num24++)
					{
						elizaDialogueSource.PlayClip(tweakables.dialogue.cShellMonologueSection03.elizaMonologueClips[num24]);
						while (elizaDialogueSource.isPlaying)
						{
							yield return null;
						}
						while (!allowElizaToContinue)
						{
							if (tweakables.dialogue.cShellMonologueSection03.allowSkipAtClip > -1 && tweakables.dialogue.cShellMonologueSection03.allowSkipAtClip <= num24)
							{
								skipToNextMonologueBeat = true;
							}
							yield return null;
						}
						if (skipToNextMonologueBeat)
						{
							num24 = tweakables.dialogue.cShellMonologueSection03.elizaMonologueClips.Length;
							skipToNextMonologueBeat = false;
						}
						if (shellResponseQueued)
						{
							shellResponseQueued = false;
						}
						paddingTimer = tweakables.encourageShellTime;
					}
					while (shellsListenedTo < tweakables.numShellsToAdvanceMonologue[num21])
					{
						if (shellResponseQueued)
						{
							shellResponseQueued = false;
						}
						if (paddingTimer > 0f)
						{
							if (!elizaDialogueSource.isPlaying && !shellsArePlaying())
							{
								paddingTimer -= Time.deltaTime;
							}
						}
						else
						{
							elizaDialogueSource.PlayClip(tweakables.dialogue.cShellEncouragements.FireFeedbackClip());
							paddingTimer = tweakables.encourageShellTime;
						}
						yield return null;
					}
				}
				while (shellsArePlaying())
				{
					yield return null;
				}
			}
			finalSeaShell.gameObject.SetActive(value: true);
			finalSeaShell.Stop();
			elizaDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.cPresentFinalShellLine);
			elizaFollow.SlowLook(newLook: true);
			elizaFollow.lookEnabled = true;
			elizaFollow.ReevaluateTarget();
			while (elizaDialogueSource.isPlaying)
			{
				yield return null;
			}
			elizaFollow.SlowLook(newLook: false);
			finalSeaShell.clip = tweakables.dialogue.cShellTedTalks[tweakables.dialogue.cShellTedTalks.Length - 1].shellAudio;
			finalSeaShell.Play();
			finalShellSubtitles.SetIsPlaying(s: true);
			finalSeaShellAnimator.enabled = true;
			_musicHandler.StopPlayingFinal();
			allowEnvironmentChange = true;
			dynamicEnvironmentManager.SetNewArea(12);
			allowEnvironmentChange = false;
			while (finalSeaShell.isPlaying)
			{
				yield return null;
			}
			finalSeaShell.Stop();
			finalShellSubtitles.SetIsPlaying(s: false);
			fakeFinalSeaShell.transform.position = finalSeaShellAnimator.transform.position;
			fakeFinalSeaShell.transform.rotation = finalSeaShellAnimator.transform.rotation;
			finalSeaShell.gameObject.SetActive(value: false);
			fakeFinalSeaShell.gameObject.SetActive(value: true);
			allowEnvironmentChange = true;
			dynamicEnvironmentManager.SetNewArea(-1);
			elizaState = ElizaStates.ElizaEnding;
		}
		if (elizaState == ElizaStates.ElizaEnding)
		{
			for (int num25 = 0; num25 < tweakables.dialogue.dEndMonologue.Length; num25++)
			{
				elizaDialogueSource.PlayClip(tweakables.dialogue.dEndMonologue[num25]);
				if (num25 == 0)
				{
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.gThinkStartTime, this);
					elizaEyes.SetEyesProcessing();
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.eyeTimings.gThinkEndTime, this);
					elizaEyes.SetEyesResting();
				}
				while (elizaDialogueSource.isPlaying)
				{
					yield return null;
				}
			}
			elizaState = ElizaStates.PostAdmin;
			PersistentDataManager.OverwriteHasAdmin(hasAdminIn: true);
			PersistentDataManager.OverwriteElizaData(elizaState);
			PersistentDataManager.OverwriteArchiveState(VVR_Archive.ArchiveStates.ArchivesFinished);
		}
		if (elizaState == ElizaStates.PostAdmin)
		{
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		EagerScenes.instance.TakeOffAllHeadsets();
	}

	private void setFillChecks()
	{
		sunsetFillMax = 0f;
		for (int i = 0; i < itemsToFillSunset.Length; i++)
		{
			if (itemsToFillSunset[i] != null)
			{
				sunsetFillMax += itemsToFillSunset[i].totalVacuumValue;
			}
		}
		canyonFillMax = 0f;
		for (int j = 0; j < itemsToFillCanyon.Length; j++)
		{
			if (itemsToFillCanyon[j] != null)
			{
				canyonFillMax += itemsToFillCanyon[j].totalVacuumValue;
			}
		}
		spaceFillMax = 0f;
		for (int k = 0; k < itemsToFillSpace.Length; k++)
		{
			if (itemsToFillSpace[k] != null)
			{
				spaceFillMax += itemsToFillSpace[k].totalVacuumValue;
			}
		}
		volcanoFillMax = 0f;
		for (int l = 0; l < itemsToFillVolcano.Length; l++)
		{
			if (itemsToFillVolcano[l] != null)
			{
				volcanoFillMax += itemsToFillVolcano[l].totalVacuumValue;
			}
		}
		caveFillMax = 0f;
		for (int m = 0; m < itemsToFillCave.Length; m++)
		{
			if (itemsToFillCave[m] != null)
			{
				caveFillMax += itemsToFillCave[m].totalVacuumValue;
			}
		}
	}

	private float percentFilledSunset()
	{
		float num = 0f;
		if (itemsToFillSunset.Length > 0)
		{
			for (int i = 0; i < itemsToFillSunset.Length; i++)
			{
				if (itemsToFillSunset[i] != null)
				{
					num += itemsToFillSunset[i].currentVacuumValueCapped;
				}
			}
			num /= sunsetFillMax;
		}
		return num;
	}

	private void FillInSunset()
	{
		makeScenesConsumable();
		sunsetVacuum.gameObject.SetActive(value: true);
		sunsetVacuum.isSucking = false;
		for (int i = 0; i < itemsToFillSunset.Length; i++)
		{
			if (itemsToFillSunset[i] != null)
			{
				itemsToFillSunset[i].OneShotTransfer(sunsetVacuum, tweakables.environmentFillInRate);
			}
		}
		ActivateSubtitles();
		sunsetVacuum.gameObject.SetActive(value: false);
	}

	private float percentFilledCanyon()
	{
		float num = 0f;
		if (itemsToFillCanyon.Length > 0)
		{
			for (int i = 0; i < itemsToFillCanyon.Length; i++)
			{
				if (itemsToFillCanyon[i] != null)
				{
					num += itemsToFillCanyon[i].currentVacuumValueCapped;
				}
			}
			num /= canyonFillMax;
		}
		return num;
	}

	private void FillInCanyon()
	{
		makeScenesConsumable();
		elizaRideBalloon.gameObject.SetActive(value: true);
		canyonVacuum.gameObject.SetActive(value: true);
		canyonVacuum.isSucking = false;
		for (int i = 0; i < itemsToFillCanyon.Length; i++)
		{
			if (itemsToFillCanyon[i] != null)
			{
				itemsToFillCanyon[i].OneShotTransfer(canyonVacuum, tweakables.environmentFillInRate);
			}
		}
		ActivateSubtitles();
		canyonVacuum.gameObject.SetActive(value: false);
	}

	private float percentFilledCave()
	{
		float num = 0f;
		if (itemsToFillCave.Length > 0)
		{
			for (int i = 0; i < itemsToFillCave.Length; i++)
			{
				if (itemsToFillCave[i] != null)
				{
					num += itemsToFillCave[i].currentVacuumValueCapped;
				}
			}
			num /= caveFillMax;
		}
		return num;
	}

	private void FillInCave()
	{
		makeScenesConsumable();
		caveVacuum.gameObject.SetActive(value: true);
		caveVacuum.isSucking = false;
		for (int i = 0; i < itemsToFillCave.Length; i++)
		{
			if (itemsToFillCave[i] != null)
			{
				itemsToFillCave[i].OneShotTransfer(caveVacuum, tweakables.environmentFillInRate);
			}
		}
		caveVacuum.gameObject.SetActive(value: false);
	}

	private float percentFilledSpace()
	{
		float num = 0f;
		if (itemsToFillSpace.Length > 0)
		{
			for (int i = 0; i < itemsToFillSpace.Length; i++)
			{
				if (itemsToFillSpace[i] != null)
				{
					num += itemsToFillSpace[i].currentVacuumValueCapped;
				}
			}
			num /= caveFillMax;
		}
		return num;
	}

	private void FillInSpace()
	{
		makeScenesConsumable();
		spaceVacuum.gameObject.SetActive(value: true);
		spaceVacuum.isSucking = false;
		for (int i = 0; i < itemsToFillSpace.Length; i++)
		{
			if (itemsToFillSpace[i] != null)
			{
				itemsToFillSpace[i].OneShotTransfer(spaceVacuum, tweakables.environmentFillInRate);
			}
		}
		ActivateSubtitles();
		spaceVacuum.gameObject.SetActive(value: false);
	}

	private float percentFilledVolcano()
	{
		makeScenesConsumable();
		float num = 0f;
		if (itemsToFillVolcano.Length > 0)
		{
			for (int i = 0; i < itemsToFillVolcano.Length; i++)
			{
				if (itemsToFillVolcano[i] != null)
				{
					num += itemsToFillVolcano[i].currentVacuumValueCapped;
				}
			}
			num /= volcanoFillMax;
		}
		return num;
	}

	private void VolcanoErupt()
	{
		eruptionShockwave.Play(withChildren: true);
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < eruptionLightFlicker.Length; i++)
		{
			eruptionLightFlicker[i].enabled = true;
		}
		volcanoBoom.Play();
	}

	private void FillInVolcano()
	{
		volcanoVacuum.gameObject.SetActive(value: true);
		volcanoVacuum.isSucking = false;
		for (int i = 0; i < itemsToFillVolcano.Length; i++)
		{
			if (itemsToFillVolcano[i] != null)
			{
				itemsToFillVolcano[i].OneShotTransfer(volcanoVacuum, tweakables.environmentFillInRate);
			}
		}
		volcanoVacuum.gameObject.SetActive(value: false);
	}

	public bool IsFull()
	{
		if (consumableComponents == null)
		{
			return true;
		}
		bool result = true;
		for (int i = 0; i < consumableComponents.Length; i++)
		{
			if (consumableComponents[i].currentObjectState != VVRConsumable.objectState.wholeStatic && consumableComponents[i].currentObjectState != VVRConsumable.objectState.wholeShaking)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public void MakeElizaUnconsumable()
	{
		for (int i = 0; i < consumableComponents.Length; i++)
		{
			consumableComponents[i].unconsumable = true;
		}
	}

	private void turnOnElizaCollision()
	{
		for (int i = 0; i < elizaColliders.Length; i++)
		{
			elizaColliders[i].enabled = true;
		}
	}

	private void turnOffElizaCollision()
	{
		for (int i = 0; i < elizaColliders.Length; i++)
		{
			elizaColliders[i].enabled = false;
		}
	}

	private void elizaHover()
	{
		elizaHoverTime += Time.deltaTime;
		if (elizaHoverTime > tweakables.elizaHoverCycleTime)
		{
			elizaHoverTime = 0f;
			hoverUp = !hoverUp;
		}
		elizaHoverPosition = Vector3.zero;
		hoverT = elizaHoverTime / tweakables.elizaHoverCycleTime;
		if (hoverUp)
		{
			elizaHoverPosition.y = Mathf.SmoothStep(0f, tweakables.elizaHoverDistance, hoverT);
		}
		else
		{
			elizaHoverPosition.y = Mathf.SmoothStep(tweakables.elizaHoverDistance, 0f, hoverT);
		}
		if (elizaState != ElizaStates.ElizaWalkToSun)
		{
			if (elizaState == ElizaStates.ElizaOn || elizaState == ElizaStates.ElizaLookAtSun || elizaState == ElizaStates.ElizaShellSection || elizaState == ElizaStates.ElizaEnding || elizaState == ElizaStates.PostAdmin)
			{
				eliza.transform.position = basePosition + elizaHoverPosition;
			}
			else
			{
				elizaPositioner.transform.localPosition = basePosition + elizaHoverPosition;
			}
		}
	}

	private void elizaBlink()
	{
		if (elizaEyes.currentState == ElizaEyeStates.ElizaEyeState.EyesOn)
		{
			blinkCountdown -= Time.deltaTime;
			if (blinkCountdown <= 0f)
			{
				blinkCountdown = UnityEngine.Random.Range(blinkTriggerTimeMin, blinkTriggerTimeMax);
				elizaEyes.ElizaBlink("_");
			}
		}
	}

	private void moveToSun()
	{
		moveLerp += Time.deltaTime;
		moveT = moveLerp / tweakables.walkToSunTime;
		moveT = Mathf.Sin(moveT * (float)Math.PI * 0.5f);
		elizaPositioner.transform.position = Vector3.Lerp(startPosition, elizaSunSetTarget.position, moveT) + elizaHoverPosition;
		eliza.transform.position = elizaPositioner.transform.position + jointDifferencePosition;
		if (moveLerp < rotateTime)
		{
			rotateT = moveLerp / rotateTime;
			rotateT = Mathf.Sin(rotateT * (float)Math.PI * 0.5f);
			eliza.transform.rotation = Quaternion.Lerp(startRotation, elizaSunSetTarget.rotation, rotateT);
		}
		basePosition = elizaPositioner.transform.position;
	}

	private void elizaRise()
	{
		onTime += Time.deltaTime;
		onT = onTime / tweakables.turnOnTime;
		onT = Mathf.Sin(onT * (float)Math.PI * 0.5f);
		if (elizaState == ElizaStates.ElizaOn || elizaState == ElizaStates.ElizaLookAtSun || elizaState == ElizaStates.ElizaShellSection || elizaState == ElizaStates.ElizaEnding || elizaState == ElizaStates.PostAdmin)
		{
			elizaPositioner.transform.position = Vector3.Lerp(riseStartPosition, basePosition, onT);
			if (elizaState == ElizaStates.ElizaOn)
			{
				eliza.transform.position = elizaPositioner.transform.position + jointDifferencePosition;
			}
		}
		else
		{
			elizaPositioner.transform.localPosition = Vector3.Lerp(riseStartPosition, basePosition, onT);
		}
		elizaGVR.pitch = tweakables.pitchUpCurve.Evaluate(onTime / tweakables.turnOnTime);
		backgroundAdvertisement.SetSourcePitch(elizaGVR.pitch);
	}

	private void elizaFall()
	{
		offTime += Time.deltaTime;
		onT = offTime / tweakables.turnOffTime;
		onT = Mathf.Sin(onT * (float)Math.PI * 0.5f);
		if (elizaState == ElizaStates.ElizaOn || elizaState == ElizaStates.ElizaLookAtSun || elizaState == ElizaStates.ElizaShellSection || elizaState == ElizaStates.ElizaEnding || elizaState == ElizaStates.PostAdmin)
		{
			elizaPositioner.transform.position = Vector3.Lerp(startPosition, riseStartPosition, onT);
			if (elizaState == ElizaStates.ElizaOn)
			{
				eliza.transform.position = elizaPositioner.transform.position + jointDifferencePosition;
			}
		}
		else
		{
			elizaPositioner.transform.localPosition = Vector3.Lerp(startPosition, riseStartPosition, onT);
		}
		if (offTime >= tweakables.turnOffTime && allowEnvironmentChange)
		{
			if (elizaState != ElizaStates.ElizaOff)
			{
				teleportAudioEvent.Play();
				elizaTeleport.TeleportIn(1f);
			}
			elizaTeleportTarget.transform.position = elizaStageCenterPosition;
		}
		elizaGVR.pitch = tweakables.pitchDownCurve.Evaluate(offTime / tweakables.turnOffTime);
		backgroundAdvertisement.SetSourcePitch(elizaGVR.pitch);
	}

	private void elizaOn(bool noAudio = false)
	{
		if (elizaState == ElizaStates.ElizaOn || elizaState == ElizaStates.ElizaLookAtSun || elizaState == ElizaStates.ElizaShellSection || elizaState == ElizaStates.ElizaEnding || elizaState == ElizaStates.PostAdmin)
		{
			riseStartPosition = elizaPositioner.transform.position;
		}
		else
		{
			riseStartPosition = elizaPositioner.transform.localPosition;
		}
		elizaFollow.RiseTurn(tweakables.turnOnTime);
		onTime = 0f;
		elizaEyes.StartEyesOn();
		elizaMouth.TurnMouthOn();
		blinkCountdown = UnityEngine.Random.Range(blinkTriggerTimeMin, blinkTriggerTimeMax);
		if (!noAudio)
		{
			onAudioEvent.Play();
		}
		if (elizaDialogueSource.isPlaying)
		{
			elizaGVR.Stop();
		}
		glitching = false;
	}

	private void elizaOff(bool allowEnviroChange = true)
	{
		_elizaIsOn = false;
		elizaFollow.elizaObjReference.ungrabbable = true;
		elizaHoverTime = 0f;
		hoverUp = true;
		if (elizaState == ElizaStates.ElizaOn || elizaState == ElizaStates.ElizaLookAtSun || elizaState == ElizaStates.ElizaShellSection || elizaState == ElizaStates.ElizaEnding || elizaState == ElizaStates.PostAdmin)
		{
			basePosition = (startPosition = (riseStartPosition = elizaPositioner.transform.position));
		}
		else
		{
			basePosition = (startPosition = (riseStartPosition = elizaPositioner.transform.localPosition));
		}
		riseStartPosition.y -= 0.5f;
		offTime = 0f;
		elizaFollow.lookEnabled = false;
		elizaEyes.StartEyesOff();
		elizaMouth.TurnMouthOff();
		allowEnvironmentChange = allowEnviroChange;
		dynamicEnvironmentManager.SetUseCamera(newC: true);
		offAudioEvent.Play();
	}

	private void elizaOffImmediate()
	{
		_elizaIsOn = false;
		eliza.GetComponent<ObjectBase>().ungrabbable = true;
		elizaHoverTime = 0f;
		hoverUp = true;
		startPosition = (basePosition = (riseStartPosition = elizaPositioner.transform.localPosition));
		riseStartPosition.y -= 0.5f;
		elizaPositioner.transform.localPosition = riseStartPosition;
		elizaFollow.lookEnabled = false;
		elizaMouth.TurnMouthOff(instant: true);
	}

	private void ShellsFloat()
	{
		shellHolder.transform.Rotate(shellRotateRate * Time.deltaTime);
	}

	private void handleGrabPitch()
	{
		if (!glitching)
		{
			return;
		}
		if (glitchingIn || glitchingOut)
		{
			glitchCount += Time.deltaTime;
			if (glitchCount >= glitchTime)
			{
				glitchCount = glitchTime;
			}
			if (glitchingIn)
			{
				elizaGVR.pitch = tweakables.pitchUpCurve.Evaluate(glitchCount / glitchTime);
			}
			else if (glitchingOut)
			{
				elizaGVR.pitch = tweakables.pitchDownCurve.Evaluate(glitchCount / glitchTime);
			}
			if (glitchCount >= glitchTime)
			{
				if (glitchingOut)
				{
					elizaGVR.Stop();
					glitching = false;
				}
				glitchCount = 0f;
				glitchingIn = (glitchingOut = false);
			}
		}
		else if (elizaDialogueSource.isPlaying)
		{
			glitchCount += Time.deltaTime;
			if (glitchCount >= glitchTime)
			{
				glitchingOut = true;
				glitchCount = 0f;
				elizaEyes.StartEyesOff();
			}
		}
	}

	private bool shellsArePlaying()
	{
		bool result = false;
		for (int i = 0; i < seaShells.Length; i++)
		{
			if (seaShells[i].ShellIsPlaying())
			{
				result = true;
			}
		}
		return result;
	}

	private void ResetVesuvius()
	{
		volcanoResetTime += Time.deltaTime;
		if (volcanoResetTime > tweakables.vesuviusLetterTime)
		{
			volcanoResetTime = tweakables.vesuviusLetterTime;
		}
		volcanoT = volcanoResetTime / tweakables.vesuviusLetterTime;
		volcanoT = Mathf.Sin(volcanoT * (float)Math.PI * 0.5f);
		volcanoLava.localScale = Vector3.Lerp(Vector3.one * 0.5f + Vector3.up * 0.45f, Vector3.one * 0.9f + Vector3.up * 0.05f, volcanoT);
		for (int i = 0; i < volcanoMoveLetters.Length; i++)
		{
			volcanoMoveLetters[i].transform.position = Vector3.Lerp(volcanoLetterStartPositions[i], volcanoMoveTargets[i].position, volcanoT);
			volcanoMoveLetters[i].transform.rotation = Quaternion.Euler(Vector3.Lerp(volcanoLetterStartEuler[i], volcanoMoveTargets[i].rotation.eulerAngles, volcanoT));
		}
	}

	private void ResetMerman()
	{
		mermanResetTime += Time.deltaTime;
		if (mermanResetTime > tweakables.vesuviusLetterTime)
		{
			mermanResetTime = tweakables.vesuviusLetterTime;
		}
		mermanT = mermanResetTime / tweakables.vesuviusLetterTime;
		mermanT = Mathf.Sin(mermanT * (float)Math.PI * 0.5f);
	}

	private void PitchBGM()
	{
		bgmPitchingUp = false;
	}

	private void HandleBGMVolume()
	{
		if (bgmFadingIn)
		{
			bgmCurrentVolume += tweakables.volumeChangeRate * Time.deltaTime;
			if (bgmCurrentVolume >= bgmMaxVolume)
			{
				bgmCurrentVolume = bgmMaxVolume;
				bgmFadingIn = false;
			}
			backgroundAdvertisement.SetSourceVolume(bgmCurrentVolume);
		}
		if (bgmFadingOut)
		{
			bgmCurrentVolume -= tweakables.volumeChangeRate * Time.deltaTime;
			if (bgmCurrentVolume <= 0f)
			{
				bgmCurrentVolume = 0f;
				bgmFadingOut = false;
			}
			backgroundAdvertisement.SetSourceVolume(bgmCurrentVolume);
		}
	}

	private float pitchDownTime(AudioClip targetClip)
	{
		return targetClip.length - (tweakables.turnOffTime + tweakables.turnOnTime / 2f);
	}

	private float offPitchDownTime(AudioClip targetClip)
	{
		return targetClip.length - tweakables.turnOffTime;
	}

	public void StartSunsetMonologue()
	{
		startSunsetSequence = true;
	}

	public void AddShellListened()
	{
		shellsListenedTo++;
		SetShellAudioSamples(0);
	}

	public void PlayShellIntro(NPCDialogueClip clip)
	{
		elizaDialogueSource.ForcePlayClipInterrupt(clip);
	}

	public void PlayShellResponse(NPCDialogueClip clip)
	{
		if (!shellResponseQueued)
		{
			elizaDialogueSource.InterruptWithClip(clip);
			shellResponseQueued = true;
		}
	}

	public AudioClip GetNextShellDialogue()
	{
		int num = shellsListenedTo;
		if (num >= tweakables.dialogue.cShellTedTalks.Length)
		{
			num = tweakables.dialogue.cShellTedTalks.Length - 1;
		}
		return tweakables.dialogue.cShellTedTalks[num].shellAudio;
	}

	public void SetShellAudioSamples(int newSample)
	{
		currentShellSample = newSample;
	}

	public int GetShellDialoguePosition()
	{
		return currentShellSample;
	}

	public bool GetCurrentShellSkipState()
	{
		int num = shellsListenedTo;
		if (num >= tweakables.dialogue.cShellTedTalks.Length)
		{
			num = tweakables.dialogue.cShellTedTalks.Length - 1;
		}
		return tweakables.dialogue.cShellTedTalks[num].canBeSkipped;
	}

	public float GetCurrentShellSkipPercent()
	{
		int num = shellsListenedTo;
		if (num >= tweakables.dialogue.cShellTedTalks.Length)
		{
			num = tweakables.dialogue.cShellTedTalks.Length - 1;
		}
		return tweakables.dialogue.cShellTedTalks[num].allowSkipPercent;
	}

	public void SetAllowElizaToContinue(bool newContinue)
	{
		allowElizaToContinue = newContinue;
	}

	public void FadeInMusic()
	{
		bgmFadingIn = true;
		bgmFadingOut = false;
	}

	public void FadeOutMusic()
	{
		bgmFadingOut = true;
		bgmFadingIn = false;
	}

	public void PlayElizaGrabFeedback()
	{
		if (!_elizaIsOn && !elizaFollow.doingRise && !glitching)
		{
			elizaEyes.StartEyesOn();
			glitchingIn = (glitching = true);
			glitchingOut = false;
			glitchCount = 0f;
			elizaGVR.Stop();
			DialoguePlaybackInstance dialoguePlaybackInstance = elizaDialogueSource.PlayClip(tweakables.dialogue.aElizaGlitchAds.FireFeedbackClip());
			GlobalCoroutineContainer.AddThread(EndGrabWorldGlitchonDelay(dialoguePlaybackInstance.mClipData.clip.length / 3f, offPitchDownTime(dialoguePlaybackInstance.mClipData.clip)), this);
		}
	}

	public void EndElizaGrabFeedback()
	{
		if (glitching)
		{
			elizaEyes.StartEyesOff();
			glitchingOut = true;
			glitchingIn = false;
			glitchCount = 0f;
		}
	}

	public void SetShellsUngrabbable(bool newGrab)
	{
		for (int i = 0; i < seaShells.Length; i++)
		{
			if (seaShells[i] != null)
			{
				if ((seaShells[i].sentFinishedReference && newGrab) || !seaShells[i].isGrabbed)
				{
				}
				seaShells[i].ungrabbable = newGrab;
			}
		}
	}

	public void ActivateSubtitles()
	{
		if (PersistentDataManager.GetSubtitleLanguage() != VVR_METASAVEDATA.SubtitlesState.NONE && PersistentDataManager.GetSubtitleLanguage() != VVR_METASAVEDATA.SubtitlesState.ENGLISH)
		{
			if (sunsetFilled || elizaState >= ElizaStates.ElizaOn)
			{
				beachSubtitle.SetActive(value: true);
			}
			if (canyonFilled || elizaState >= ElizaStates.ElizaOn)
			{
				canyonSubtitle.SetActive(value: true);
			}
			if (spaceFilled || elizaState >= ElizaStates.ElizaOn)
			{
				spaceSubtitle.SetActive(value: true);
			}
			if (volcanoFilled || elizaState >= ElizaStates.ElizaOn)
			{
				volcanoSubtitle.SetActive(value: true);
			}
		}
	}

	private void OnEnable()
	{
		FadeInMusic();
		windLoop.Play();
		rumbleLoop.Play();
	}

	private void makeScenesUnconsumable(int ignoreVal = -1)
	{
		if (ignoreVal != 0)
		{
			for (int i = 0; i < beachConsumables.Length; i++)
			{
				beachConsumables[i].unconsumable = true;
			}
		}
		if (ignoreVal != 1)
		{
			for (int j = 0; j < canyonConsumables.Length; j++)
			{
				canyonConsumables[j].unconsumable = true;
			}
		}
		if (ignoreVal != 2)
		{
			for (int k = 0; k < caveConsumables.Length; k++)
			{
				caveConsumables[k].unconsumable = true;
			}
		}
		if (ignoreVal != 3)
		{
			for (int l = 0; l < volcanoConsumables.Length; l++)
			{
				volcanoConsumables[l].unconsumable = true;
			}
		}
		if (ignoreVal != 4)
		{
			for (int m = 0; m < planetConsumables.Length; m++)
			{
				planetConsumables[m].unconsumable = true;
			}
		}
	}

	private void makeScenesConsumable()
	{
		for (int i = 0; i < beachConsumables.Length; i++)
		{
			beachConsumables[i].unconsumable = false;
		}
		for (int j = 0; j < planetConsumables.Length; j++)
		{
			planetConsumables[j].unconsumable = false;
		}
		for (int k = 0; k < caveConsumables.Length; k++)
		{
			caveConsumables[k].unconsumable = false;
		}
		for (int l = 0; l < volcanoConsumables.Length; l++)
		{
			volcanoConsumables[l].unconsumable = false;
		}
		for (int m = 0; m < canyonConsumables.Length; m++)
		{
			canyonConsumables[m].unconsumable = false;
		}
	}

	private IEnumerator IntroSequence()
	{
		makeScenesUnconsumable();
		elizaFollow.elizaObjReference.ungrabbable = true;
		float timer = 0f;
		float sceneFlashT = 0f;
		List<int> lastNumbers = new List<int> { 5 };
		float elizaGlitchT = 0f;
		MaterialPropertyBlock matBlock = new MaterialPropertyBlock();
		int shaderID = Shader.PropertyToID("_BlockScale");
		List<int> pickedNums = new List<int>();
		while (timer <= tweakables.introInitialGlitchTime)
		{
			if (sceneFlashT > tweakables.introSceneFlashInterval)
			{
				pickedNums.Clear();
				elizaGVR.pitch = UnityEngine.Random.Range(0.85f, 1.05f);
				int num = UnityEngine.Random.Range(0, 5);
				while (lastNumbers.Contains(num))
				{
					num = UnityEngine.Random.Range(0, 5);
				}
				lastNumbers.Add(num);
				if (lastNumbers.Count > 4)
				{
					lastNumbers.Clear();
				}
				disableScenes();
				sceneFlashT = 0f;
				switch (num)
				{
				case 0:
					dynamicEnvironmentManager.MatchEnvironment(tweakables.introSceneFlashInterval, dynamicEnvironmentManager.enviroPropertiesBeach);
					beachRoot.SetActive(value: true);
					break;
				case 1:
					dynamicEnvironmentManager.MatchEnvironment(tweakables.introSceneFlashInterval, dynamicEnvironmentManager.enviroPropertiesCanyon);
					canyonRoot.SetActive(value: true);
					break;
				case 2:
					dynamicEnvironmentManager.MatchEnvironment(tweakables.introSceneFlashInterval, dynamicEnvironmentManager.enviroPropertiesCave);
					caveRoot.SetActive(value: true);
					break;
				case 3:
					dynamicEnvironmentManager.MatchEnvironment(tweakables.introSceneFlashInterval, dynamicEnvironmentManager.enviroPropertiesVolcano);
					volcanoRoot.SetActive(value: true);
					break;
				case 4:
					dynamicEnvironmentManager.MatchEnvironment(tweakables.introSceneFlashInterval, dynamicEnvironmentManager.enviroPropertiesSpace);
					spaceRoot.SetActive(value: true);
					break;
				case 5:
					dynamicEnvironmentManager.MatchEnvironment(tweakables.introSceneFlashInterval, dynamicEnvironmentManager.enviroPropertiesCenterSpotLight);
					disableScenes();
					sceneFlashT = -0.5f;
					break;
				default:
					dynamicEnvironmentManager.MatchEnvironment(tweakables.introSceneFlashInterval, dynamicEnvironmentManager.enviroPropertiesCenterSpotLight);
					sceneFlashT = -0.5f;
					break;
				}
				if (num < 5)
				{
					int num2 = UnityEngine.Random.Range(1, 4);
					for (int i = 0; i <= num2; i++)
					{
						int num3 = UnityEngine.Random.Range(0, allCenterObjects.Length);
						while (pickedNums.Contains(num3))
						{
							num3 = UnityEngine.Random.Range(0, allCenterObjects.Length);
						}
						pickedNums.Add(num3);
						allCenterObjects[num3].SetActive(value: true);
					}
				}
			}
			if (elizaGlitchT > tweakables.introSceneFlashInterval / 2f)
			{
				for (int j = 0; j < oRenderers.Length; j++)
				{
					oRenderers[j].sharedMaterial = glitchMat;
					matBlock.SetFloat(shaderID, UnityEngine.Random.Range(0f, 50f));
					oRenderers[j].SetPropertyBlock(matBlock);
				}
				elizaGlitchT = 0f;
			}
			elizaGlitchT += Time.deltaTime;
			sceneFlashT += Time.deltaTime;
			timer += Time.deltaTime;
			yield return null;
		}
		enableScenes();
		for (int k = 0; k < oRenderers.Length; k++)
		{
			matBlock.SetFloat(shaderID, 0f);
			oRenderers[k].sharedMaterial = glitchMat;
			oRenderers[k].SetPropertyBlock(matBlock);
			oRenderers[k].sharedMaterial = startMat;
		}
		elizaFollow.elizaObjReference.ungrabbable = false;
		elizaGVR.pitch = 1f;
		allowEnvironmentChange = true;
	}

	private IEnumerator GlitchWorld(float time, float glitchInterval, int ignoreVal = -1, bool finalGlitch = false)
	{
		makeScenesUnconsumable(ignoreVal);
		elizaFollow.elizaObjReference.ungrabbable = true;
		float timer = 0f;
		float sceneFlashT = 0f;
		List<int> lastNumbers = new List<int> { 5 };
		float elizaGlitchT = 0f;
		MaterialPropertyBlock matBlock = new MaterialPropertyBlock();
		Renderer[] oRenderers = new Renderer[consumableComponents.Length];
		int shaderID = Shader.PropertyToID("_BlockScale");
		for (int i = 0; i < oRenderers.Length; i++)
		{
			oRenderers[i] = consumableComponents[i].oRenderer;
		}
		List<int> pickedNums = new List<int>();
		chazGlitchRoot.SetActive(value: true);
		chazBots[0].SetActive(value: false);
		chazBots[1].SetActive(value: false);
		while (timer <= time)
		{
			pickedNums.Clear();
			if (sceneFlashT > glitchInterval)
			{
				elizaGVR.pitch = UnityEngine.Random.Range(0.85f, 1.05f);
				int num = UnityEngine.Random.Range(0, 6);
				while (lastNumbers.Contains(num))
				{
					num = UnityEngine.Random.Range(0, 6);
				}
				lastNumbers.Add(num);
				if (lastNumbers.Count > 5)
				{
					lastNumbers.Clear();
				}
				disableScenes(ignoreVal);
				sceneFlashT = 0f;
				switch (num)
				{
				case 0:
					dynamicEnvironmentManager.MatchEnvironment(glitchInterval, dynamicEnvironmentManager.enviroPropertiesBeach);
					beachRoot.SetActive(value: true);
					break;
				case 1:
					dynamicEnvironmentManager.MatchEnvironment(glitchInterval, dynamicEnvironmentManager.enviroPropertiesCanyon);
					canyonRoot.SetActive(value: true);
					break;
				case 2:
					dynamicEnvironmentManager.MatchEnvironment(glitchInterval, dynamicEnvironmentManager.enviroPropertiesCave);
					caveRoot.SetActive(value: true);
					break;
				case 3:
					dynamicEnvironmentManager.MatchEnvironment(glitchInterval, dynamicEnvironmentManager.enviroPropertiesVolcano);
					volcanoRoot.SetActive(value: true);
					break;
				case 4:
					dynamicEnvironmentManager.MatchEnvironment(glitchInterval, dynamicEnvironmentManager.enviroPropertiesSpace);
					spaceRoot.SetActive(value: true);
					break;
				case 5:
					dynamicEnvironmentManager.MatchEnvironment(glitchInterval, dynamicEnvironmentManager.enviroPropertiesCenterSpotLight);
					sceneFlashT = -0.5f;
					break;
				default:
					dynamicEnvironmentManager.MatchEnvironment(glitchInterval, dynamicEnvironmentManager.enviroPropertiesCenterSpotLight);
					sceneFlashT = -0.5f;
					break;
				}
			}
			if (elizaGlitchT > glitchInterval / 2f)
			{
				for (int j = 0; j < oRenderers.Length; j++)
				{
					oRenderers[j].sharedMaterial = glitchMat;
					matBlock.SetFloat(shaderID, UnityEngine.Random.Range(0f, 50f));
					oRenderers[j].SetPropertyBlock(matBlock);
				}
				elizaGlitchT = 0f;
			}
			elizaGlitchT += Time.deltaTime;
			sceneFlashT += Time.deltaTime;
			timer += Time.deltaTime;
			yield return null;
		}
		if (finalGlitch)
		{
			dynamicEnvironmentManager.MatchEnvironment(0.15f, propPreGlitch, resetLights: true);
		}
		enableScenes();
		for (int k = 0; k < oRenderers.Length; k++)
		{
			matBlock.SetFloat(shaderID, 0f);
			oRenderers[k].sharedMaterial = glitchMat;
			oRenderers[k].SetPropertyBlock(matBlock);
			if (oRenderers[k].GetComponent<ElizaMouth>() != null)
			{
				oRenderers[k].sharedMaterial = mouthMat;
			}
			else
			{
				oRenderers[k].sharedMaterial = startMat;
			}
		}
		elizaFollow.elizaObjReference.ungrabbable = false;
		elizaGVR.pitch = 1f;
		makeScenesConsumable();
		chazGlitchRoot.SetActive(value: false);
		yield return null;
	}

	private IEnumerator GlitchWorld(float time, float glitchInterval, ElizaEnvironmentProperties endstate)
	{
		makeScenesUnconsumable();
		elizaFollow.elizaObjReference.ungrabbable = true;
		float timer = 0f;
		float sceneFlashT = 0f;
		List<int> lastNumbers = new List<int> { 5 };
		float elizaGlitchT = 0f;
		MaterialPropertyBlock matBlock = new MaterialPropertyBlock();
		Renderer[] oRenderers = new Renderer[consumableComponents.Length];
		int shaderID = Shader.PropertyToID("_BlockScale");
		for (int i = 0; i < oRenderers.Length; i++)
		{
			oRenderers[i] = consumableComponents[i].oRenderer;
		}
		while (timer <= time)
		{
			if (sceneFlashT > glitchInterval)
			{
				elizaGVR.pitch = UnityEngine.Random.Range(0.85f, 1.05f);
				int num = UnityEngine.Random.Range(0, 6);
				while (lastNumbers.Contains(num))
				{
					num = UnityEngine.Random.Range(0, 6);
				}
				lastNumbers.Add(num);
				if (lastNumbers.Count > 5)
				{
					lastNumbers.Clear();
				}
				disableScenes();
				sceneFlashT = 0f;
				switch (num)
				{
				case 0:
					dynamicEnvironmentManager.MatchEnvironment(glitchInterval, dynamicEnvironmentManager.enviroPropertiesBeach);
					beachRoot.SetActive(value: true);
					break;
				case 1:
					dynamicEnvironmentManager.MatchEnvironment(glitchInterval, dynamicEnvironmentManager.enviroPropertiesCanyon);
					canyonRoot.SetActive(value: true);
					break;
				case 2:
					dynamicEnvironmentManager.MatchEnvironment(glitchInterval, dynamicEnvironmentManager.enviroPropertiesCave);
					caveRoot.SetActive(value: true);
					break;
				case 3:
					dynamicEnvironmentManager.MatchEnvironment(glitchInterval, dynamicEnvironmentManager.enviroPropertiesVolcano);
					volcanoRoot.SetActive(value: true);
					break;
				case 4:
					dynamicEnvironmentManager.MatchEnvironment(glitchInterval, dynamicEnvironmentManager.enviroPropertiesSpace);
					spaceRoot.SetActive(value: true);
					break;
				case 5:
					dynamicEnvironmentManager.MatchEnvironment(glitchInterval, dynamicEnvironmentManager.enviroPropertiesCenterSpotLight);
					sceneFlashT = -0.5f;
					break;
				default:
					dynamicEnvironmentManager.MatchEnvironment(glitchInterval, dynamicEnvironmentManager.enviroPropertiesCenterSpotLight);
					sceneFlashT = -0.5f;
					break;
				}
			}
			if (elizaGlitchT > glitchInterval / 2f)
			{
				for (int j = 0; j < oRenderers.Length; j++)
				{
					oRenderers[j].sharedMaterial = glitchMat;
					matBlock.SetFloat(shaderID, UnityEngine.Random.Range(0f, 50f));
					oRenderers[j].SetPropertyBlock(matBlock);
				}
				elizaGlitchT = 0f;
			}
			elizaGlitchT += Time.deltaTime;
			sceneFlashT += Time.deltaTime;
			timer += Time.deltaTime;
			dynamicEnvironmentManager.MatchEnvironment(glitchInterval, endstate);
			yield return null;
		}
		dynamicEnvironmentManager.MatchEnvironment(0.15f, dynamicEnvironmentManager.enviroPropertiesCenter, resetLights: true);
		enableScenes();
		for (int k = 0; k < oRenderers.Length; k++)
		{
			matBlock.SetFloat(shaderID, 0f);
			oRenderers[k].sharedMaterial = glitchMat;
			oRenderers[k].SetPropertyBlock(matBlock);
			oRenderers[k].sharedMaterial = startMat;
		}
		elizaFollow.elizaObjReference.ungrabbable = false;
		elizaGVR.pitch = 1f;
		makeScenesConsumable();
		yield return null;
	}

	private IEnumerator GlitchWorldWhileGrabbed()
	{
		propPreGlitch = dynamicEnvironmentManager.currentPropperties;
		makeScenesUnconsumable();
		float sceneFlashT = 0f;
		float sceneFlashInterval = 0.15f;
		int lastNum = 5;
		while (glitchExitCondition)
		{
			if (sceneFlashT > sceneFlashInterval)
			{
				elizaGVR.pitch = UnityEngine.Random.Range(0.85f, 1.05f);
				int num;
				for (num = UnityEngine.Random.Range(0, 5); num == lastNum; num = UnityEngine.Random.Range(0, 5))
				{
				}
				lastNum = num;
				disableScenes();
				sceneFlashT = 0f;
				switch (num)
				{
				case 0:
					dynamicEnvironmentManager.MatchEnvironment(sceneFlashInterval, dynamicEnvironmentManager.enviroPropertiesBeach);
					beachRoot.SetActive(value: true);
					break;
				case 1:
					dynamicEnvironmentManager.MatchEnvironment(sceneFlashInterval, dynamicEnvironmentManager.enviroPropertiesCanyon);
					canyonRoot.SetActive(value: true);
					break;
				case 2:
					dynamicEnvironmentManager.MatchEnvironment(sceneFlashInterval, dynamicEnvironmentManager.enviroPropertiesCave);
					caveRoot.SetActive(value: true);
					break;
				case 3:
					dynamicEnvironmentManager.MatchEnvironment(sceneFlashInterval, dynamicEnvironmentManager.enviroPropertiesVolcano);
					volcanoRoot.SetActive(value: true);
					break;
				case 4:
					dynamicEnvironmentManager.MatchEnvironment(sceneFlashInterval, dynamicEnvironmentManager.enviroPropertiesSpace);
					spaceRoot.SetActive(value: true);
					break;
				case 5:
					dynamicEnvironmentManager.MatchEnvironment(sceneFlashInterval, dynamicEnvironmentManager.enviroPropertiesCenterSpotLight);
					sceneFlashT = -0.5f;
					break;
				default:
					dynamicEnvironmentManager.MatchEnvironment(sceneFlashInterval, dynamicEnvironmentManager.enviroPropertiesCenterSpotLight);
					sceneFlashT = -0.5f;
					break;
				}
			}
			sceneFlashT += Time.deltaTime;
			yield return null;
		}
		dynamicEnvironmentManager.MatchEnvironment(0.15f, dynamicEnvironmentManager.enviroPropertiesCenterSpotLight, resetLights: true);
		enableScenes();
		elizaFollow.elizaObjReference.ungrabbable = false;
		elizaGVR.pitch = 1f;
		makeScenesConsumable();
	}

	public void StartGrabWorldGlitch()
	{
		if (!storySequence)
		{
			glitchExitCondition = true;
			GlobalCoroutineContainer.AddThread(GlitchWorldWhileGrabbed(), this);
		}
	}

	public void EndGrabWorldGlitch()
	{
		if (glitchExitCondition)
		{
			glitchExitCondition = false;
			GlobalCoroutineContainer.AddThread(EndGrabEnvironment(0.5f), this);
		}
	}

	private IEnumerator EndGrabEnvironment(float s)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(s, this);
		dynamicEnvironmentManager.MatchEnvironment(1f, propPreGlitch, resetLights: true);
	}

	private IEnumerator EndGrabWorldGlitchonDelay(float endGlitchtime, float endTalkTime)
	{
		float startTime = Time.time;
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(endGlitchtime, this);
		if (glitchExitCondition)
		{
			glitchExitCondition = false;
			while (startTime + endTalkTime > Time.time)
			{
				yield return null;
			}
			dynamicEnvironmentManager.MatchEnvironment(0.5f, propPreGlitch, resetLights: true);
		}
	}

	private void disableScenes(int ignoreVal = -1)
	{
		makeScenesUnconsumable(ignoreVal);
		if (ignoreVal != 0)
		{
			beachRoot.SetActive(value: false);
		}
		if (ignoreVal != 3)
		{
			volcanoRoot.SetActive(value: false);
		}
		if (ignoreVal != 4)
		{
			spaceRoot.SetActive(value: false);
		}
		if (ignoreVal != 1)
		{
			canyonRoot.SetActive(value: false);
		}
		if (ignoreVal != 2)
		{
			caveRoot.SetActive(value: false);
		}
		for (int i = 0; i < allCenterObjects.Length; i++)
		{
			allCenterObjects[i].SetActive(value: false);
		}
	}

	private void enableScenes()
	{
		makeScenesConsumable();
		beachRoot.SetActive(value: true);
		volcanoRoot.SetActive(value: true);
		spaceRoot.SetActive(value: true);
		canyonRoot.SetActive(value: true);
		caveRoot.SetActive(value: true);
		for (int i = 0; i < allCenterObjects.Length; i++)
		{
			allCenterObjects[i].SetActive(value: false);
		}
	}
}
