using System;
using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using GVR.Gfx;
using TButt;
using TMPro;
using UnityEngine;

public class VVR_HR : SceneBehaviorManager
{
	public enum HRStates
	{
		TakeTheHeadset,
		Whoopsie,
		HumanUnion,
		UnionUnplug,
		Buttflap,
		BigCrash,
		CoreReveal,
		CoreRemoval,
		MergeChoice,
		Battle,
		KillChaz
	}

	public static VVR_HR instance;

	[SerializeField]
	private HRScriptableObject HRContent;

	[SerializeField]
	private HRStates HRState;

	private SafeCoroutine childCoroutine;

	[Header("References:")]
	[SerializeField]
	private GameObject HRLevelParent;

	[SerializeField]
	private GameObject hubLevelParent;

	[SerializeField]
	private ElevatorEnvironments envSwitcher;

	[SerializeField]
	private Color angryEnvColor = Color.black;

	[SerializeField]
	private Color angryShadowColor = Color.black;

	[SerializeField]
	private Color normalEnvColor = Color.black;

	[SerializeField]
	private Color normalShadowColor = Color.black;

	[SerializeField]
	private Color offEnvColor = Color.black;

	[SerializeField]
	private Color offShadowColor = Color.black;

	[SerializeField]
	private EnvironmentProperties mainEnv;

	[SerializeField]
	private VVR_Point chazEyeLight;

	[SerializeField]
	private Transform hubSunDirection;

	[SerializeField]
	private Transform mainDirLight;

	[SerializeField]
	private Transform bottomLight;

	[SerializeField]
	private GvrAudioSource HRAmbientSound;

	private SafeCoroutine roomChangeCoroutine;

	private bool hasResetLights;

	private bool plugDisconnected;

	private bool catchSecondDisconnect;

	private bool playerReturnedToTheScene;

	private bool hasSaidReturnLine;

	private bool checkForPlayerReturn;

	private int returnLineCount;

	private float sleepingTimeCount;

	private bool hasTickledChaz;

	private bool hasSaidSleepingLine;

	[Header("Chaz:")]
	[SerializeField]
	private VVRDialogueSource chazLargeDialogue;

	[SerializeField]
	private GvrAudioSource chazLargeSource;

	[SerializeField]
	private GvrAudioSource chazSmallSource;

	[SerializeField]
	private ChazController chazController;

	[SerializeField]
	private Transform chazPupil;

	[SerializeField]
	private GvrAudioRoom HRAudioRoom;

	[Header("ButtFlap:")]
	[SerializeField]
	private GameObject buttFlapParent;

	[SerializeField]
	private Rigidbody buttFlapCover;

	[SerializeField]
	private HingeJoint buttFlapJoint;

	[SerializeField]
	private Rigidbody buttFlapLever;

	private bool buttFlapBroken;

	[Header("Chaz Head and Walls:")]
	[SerializeField]
	private GameObject headUpper;

	[SerializeField]
	private GameObject headLower;

	[SerializeField]
	private GameObject headClasps;

	[SerializeField]
	private GameObject openedClaspParent;

	[SerializeField]
	private HREyeTracking eyeCore;

	[SerializeField]
	private HREyeTracking chazHeadParent;

	[SerializeField]
	private GameObject coreRevealParent;

	[Header("Fall:")]
	[SerializeField]
	private Headset hubHeadset;

	[SerializeField]
	private GameObject impactParent;

	[SerializeField]
	private ObjectTrigger playerEnterTrigger;

	[SerializeField]
	private Transform chazFallTransform;

	[SerializeField]
	private Transform playerPedestal;

	[SerializeField]
	private Transform floorScale;

	private bool playerBackInHub;

	[Header("AI Cores and Dead Chaz:")]
	[SerializeField]
	private VVRConsumable[] debris;

	[SerializeField]
	private GameObject deadChazGameobject;

	[SerializeField]
	private ObjectBoid fakeHRHeadset;

	[SerializeField]
	private BoidController headsetController;

	[SerializeField]
	private GvrAudioSource startupSound;

	[SerializeField]
	private HRAICore chazCore;

	[SerializeField]
	private HRAICore aiTrina;

	[SerializeField]
	private HRAICore aiDonald;

	[SerializeField]
	private HRAICore aiHiJung;

	[SerializeField]
	private HRAICore aiCarl;

	[HideInInspector]
	public bool canGrabCores;

	private int totalAICores;

	private bool coreRemoved;

	private bool chazRemoved;

	private bool pacifistPathComplete;

	private bool chazWantsToKillYouNow;

	[SerializeField]
	private HRAICore[] aiCores;

	private Vector3 plugDown = Vector3.up * 1.8f;

	private Vector3 plugUp = Vector3.up * 2.5f;

	private Vector3 coreLevitatePos;

	[Header("Beetle Ball:")]
	[SerializeField]
	private Transform beetleTransform;

	[SerializeField]
	private Rigidbody chazEyeBroken;

	[SerializeField]
	private VVRDialogueSource brokenChaz;

	[SerializeField]
	private VVRDialogueSource beetleChazDialogue;

	[SerializeField]
	private VVRDialogueSource AICoreChazDialogue;

	[SerializeField]
	private GvrAudioSource brokenChazSource;

	[SerializeField]
	private GvrAudioSource beetleChazSource;

	[SerializeField]
	private HingeJointFix storageHingeFix;

	[SerializeField]
	private ChazController beetleChaz;

	[SerializeField]
	private BeetleBall beetleScript;

	[SerializeField]
	private RotateObject coreParentRotation;

	[SerializeField]
	private Rigidbody innerCollider;

	[SerializeField]
	private GameObject beetleSmashFX;

	[Header("Battle:")]
	[SerializeField]
	private Collider dontTeleportBox;

	[SerializeField]
	private BoidController mergeBoidController;

	[SerializeField]
	private ObjectBase balloonRed;

	[SerializeField]
	private GvrAudioSource spawnSound;

	[SerializeField]
	private ObjectBase mergeHeadset;

	[HideInInspector]
	public bool battleHasBegun;

	[SerializeField]
	private EnvironmentProperties powerDownEnv;

	[SerializeField]
	private MaterialUpdateController powerDownMat;

	[SerializeField]
	private List<AudioClip> humanUnionOfflineClips;

	[SerializeField]
	private AudioClip humanUnionCriticalError;

	[SerializeField]
	private GvrAudioSource humanUnionAnnouncer;

	[SerializeField]
	private GvrAudioSource activitudeShutdownSound;

	[Header("UI Text:")]
	[SerializeField]
	private TextMeshPro worldTimeText;

	[SerializeField]
	private TextMeshPro employeeNumberText;

	[SerializeField]
	private TextMeshPro sessionTimerText;

	private bool loadedIntoCrash;

	private int maxNextLine = 4;

	private int minNextLine = 1;

	private int nextLineCount;

	private bool debugBigFall;

	private bool debugDebris;

	private bool debugBeetleBall;

	private float eyeChangeSpeed = 0.2f;

	private Color startColor;

	private Color startShadowColor;

	private Color resetColor;

	private Color resetShadowColor;

	private Vector3 startScale;

	private Vector3 resetScale;

	private Vector3 offScale = Vector3.one * 0.25f;

	private Vector3 angryScale = Vector3.one * 1.15f;

	private int year;

	private int minutes;

	private int seconds;

	private void Awake()
	{
		if (instance != null)
		{
			throw new UnityException("Only one instance of VVR_HR in a scene is allowed.");
		}
		instance = this;
		Rigidbody[] componentsInChildren = chazController.GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody rigidbody in componentsInChildren)
		{
			rigidbody.isKinematic = true;
		}
	}

	private void Start()
	{
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
	}

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		SetUpSceneObjects();
		hubLevelParent.SetActive(value: false);
		totalAICores = aiCores.Length;
		HRAmbientSound.Play();
		if (PersistentDataManager.GetLastLoadedScene() == TCScenes.S.VVR_ACTIVITUDE_HR)
		{
			HRState = HRStates.CoreReveal;
			loadedIntoCrash = true;
		}
		GlobalCoroutineContainer.AddThread(HRIncrementCoroutine(), this);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_ENDING_MERGE);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_ENDING_MURDER);
	}

	protected override void OnSceneActivated()
	{
		base.OnSceneActivated();
		if (checkForPlayerReturn)
		{
			playerReturnedToTheScene = true;
		}
	}

	private IEnumerator HRMasterCoroutine()
	{
		if (!loadedIntoCrash)
		{
			yield return null;
		}
		if (HRState == HRStates.TakeTheHeadset)
		{
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
			chazLargeDialogue.PlayClip(HRContent.dialogue.hHelloBee);
			chazLargeDialogue.PlayClip(HRContent.dialogue.hRestrictedArea);
			chazLargeDialogue.PlayClip(HRContent.dialogue.hTakeOffYourHeadset);
			chazLargeDialogue.PlayClip(HRContent.dialogue.hLikeYouWereNeverHere);
			checkForPlayerReturn = true;
			while (chazLargeDialogue.isPlaying)
			{
				if (!hasTickledChaz && ToolManager.PlayerGrabbingObjectType<HRPlug>())
				{
					chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 1.5f, ChazController.eyeColor.Default, immediately: true);
					chazLargeDialogue.InterruptWithClip(HRContent.dialogue.hThatTickles);
					hasTickledChaz = true;
				}
				if (playerReturnedToTheScene && !hasSaidReturnLine)
				{
					chazLargeDialogue.ForcePlayClipOverwrite(HRContent.dialogue.hMoreFulfilling);
					hasSaidReturnLine = true;
					break;
				}
				yield return null;
			}
			chazLargeDialogue.PlayClip(HRContent.dialogue.hTakeOffHeadset2);
			chazLargeDialogue.PlayClip(HRContent.dialogue.hTakeOffHeadset3);
			float waitForSmallTalkTime = 5f;
			while (waitForSmallTalkTime > 0f)
			{
				if (!chazLargeDialogue.isPlaying)
				{
					waitForSmallTalkTime -= Time.deltaTime;
				}
				if (!hasTickledChaz && ToolManager.PlayerGrabbingObjectType<HRPlug>())
				{
					chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 1.5f, ChazController.eyeColor.Default, immediately: true);
					chazLargeDialogue.InterruptWithClip(HRContent.dialogue.hThatTickles);
					hasTickledChaz = true;
				}
				if (playerReturnedToTheScene && !hasSaidReturnLine)
				{
					chazLargeDialogue.ForcePlayClipOverwrite(HRContent.dialogue.hMoreFulfilling);
					hasSaidReturnLine = true;
					break;
				}
				yield return null;
			}
			chazLargeDialogue.PlayClip(HRContent.dialogue.hSmallTalk);
			while (chazLargeDialogue.isPlaying)
			{
				if (!hasTickledChaz && ToolManager.PlayerGrabbingObjectType<HRPlug>())
				{
					chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 1.5f, ChazController.eyeColor.Default, immediately: true);
					chazLargeDialogue.InterruptWithClip(HRContent.dialogue.hThatTickles);
					hasTickledChaz = true;
				}
				yield return null;
			}
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Deactivated, immediately: true);
			ChangeRoomColor(0f, setAngry: false, setOff: true);
			chazHeadParent.enabled = false;
			playerReturnedToTheScene = false;
			while (true)
			{
				sleepingTimeCount += Time.deltaTime;
				if (!chazLargeDialogue.isPlaying && !hasResetLights)
				{
					chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Deactivated, immediately: true);
					ChangeRoomColor(0f, setAngry: false, setOff: true);
					hasResetLights = true;
				}
				else if (chazLargeDialogue.isPlaying)
				{
					hasResetLights = false;
				}
				if (!hasSaidSleepingLine && sleepingTimeCount > 20f && !chazLargeDialogue.isPlaying)
				{
					chazLargeDialogue.PlayClip(HRContent.dialogue.hWatchingMeSleep);
					hasSaidSleepingLine = true;
					chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
					ChangeRoomColor(0f, setAngry: false);
				}
				if (!hasTickledChaz && ToolManager.PlayerGrabbingObjectType<HRPlug>())
				{
					if (!chazLargeDialogue.isPlaying)
					{
						chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
						ChangeRoomColor(0f, setAngry: false);
					}
					chazLargeDialogue.InterruptWithClip(HRContent.dialogue.hThatTickles);
					hasTickledChaz = true;
				}
				if (playerReturnedToTheScene && returnLineCount < 2)
				{
					if (returnLineCount == 0 && !chazLargeDialogue.isPlaying)
					{
						yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
						chazLargeDialogue.ForcePlayClipOverwrite(HRContent.dialogue.hReturnImStillCharging);
						chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
						ChangeRoomColor(0f, setAngry: false);
						returnLineCount++;
					}
					else if (returnLineCount == 1 && !chazLargeDialogue.isPlaying)
					{
						yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
						chazLargeDialogue.ForcePlayClipOverwrite(HRContent.dialogue.hReturnISawYouReturning);
						chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
						ChangeRoomColor(0f, setAngry: false);
						returnLineCount++;
					}
					playerReturnedToTheScene = false;
				}
				yield return null;
			}
		}
		if (HRState == HRStates.Whoopsie)
		{
			chazHeadParent.enabled = true;
			chazLargeDialogue.PlayClip(HRContent.dialogue.wNoWorriesBee);
			chazLargeDialogue.PlayClip(HRContent.dialogue.wHeroJourney);
			chazLargeDialogue.PlayClip(HRContent.dialogue.wHeroJourneyExplanation);
		}
		if (HRState == HRStates.HumanUnion)
		{
			chazLargeDialogue.PlayClip(HRContent.dialogue.uHumanUnionWantsYou);
			while (chazLargeDialogue.isPlaying)
			{
				yield return null;
			}
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 2f, ChazController.eyeColor.Angry, immediately: true);
			ChangeRoomColor(2f);
			chazLargeDialogue.PlayClip(HRContent.dialogue.uHumanUnionVoice);
			chazLargeDialogue.PlayClip(HRContent.dialogue.uAllPartOfThePlan);
		}
		if (HRState == HRStates.UnionUnplug)
		{
			while (chazLargeDialogue.isPlaying)
			{
				yield return null;
			}
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
			ChangeRoomColor(0f);
			chazLargeDialogue.PlayClip(HRContent.dialogue.iThisIsTheHumanUnion);
			while (chazLargeDialogue.isPlaying)
			{
				yield return null;
			}
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
			ChangeRoomColor(0f, setAngry: false);
			chazLargeDialogue.PlayClip(HRContent.dialogue.iVoiceOfReason);
			while (chazLargeDialogue.isPlaying)
			{
				yield return null;
			}
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
			ChangeRoomColor(0f);
			chazLargeDialogue.PlayClip(HRContent.dialogue.iUnionListen);
			while (chazLargeDialogue.isPlaying)
			{
				yield return null;
			}
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
			ChangeRoomColor(0f, setAngry: false);
			chazLargeDialogue.PlayClip(HRContent.dialogue.iGoodOneMe);
			while (chazLargeDialogue.isPlaying)
			{
				yield return null;
			}
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
			ChangeRoomColor(0f);
			chazLargeDialogue.PlayClip(HRContent.dialogue.iUnionHighFives);
			while (chazLargeDialogue.isPlaying)
			{
				yield return null;
			}
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
			ChangeRoomColor(0f, setAngry: false);
			chazLargeDialogue.PlayClip(HRContent.dialogue.iYouHaveAnyPlans);
			while (chazLargeDialogue.isPlaying)
			{
				yield return null;
			}
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
			ChangeRoomColor(0f);
			chazLargeDialogue.PlayClip(HRContent.dialogue.iWhyHowNice);
			while (chazLargeDialogue.isPlaying)
			{
				yield return null;
			}
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
			ChangeRoomColor(0f, setAngry: false);
			chazLargeDialogue.PlayClip(HRContent.dialogue.iMeTime);
			while (chazLargeDialogue.isPlaying)
			{
				yield return null;
			}
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Deactivated, immediately: true);
			ChangeRoomColor(0f, setAngry: false, setOff: true);
			chazHeadParent.enabled = false;
		}
		if (HRState == HRStates.Buttflap)
		{
			chazHeadParent.enabled = true;
			chazHeadParent.StartLookingAtBee();
			while (chazLargeDialogue.isPlaying)
			{
				yield return null;
			}
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
			while (chazLargeDialogue.isPlaying)
			{
				yield return null;
			}
			chazLargeDialogue.PlayClip(HRContent.dialogue.bWeLostPower);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(2.5f, this);
			buttFlapLever.transform.GetChild(0).GetComponent<GvrAudioSource>().Play();
			chazLargeSource.Stop();
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
			ChangeRoomColor(0f);
			chazLargeDialogue.ForcePlayClipOverwrite(HRContent.dialogue.zGrunt5);
			JointSpring buttJoint = buttFlapJoint.spring;
			buttJoint.targetPosition = 30f;
			buttFlapJoint.spring = buttJoint;
			UnityEngine.Object.Destroy(buttFlapCover.gameObject);
			buttFlapLever.isKinematic = false;
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
			chazHeadParent.StartPanicLooking();
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
			chazLargeDialogue.PlayClip(HRContent.dialogue.bWhatWasThat);
			while (chazLargeDialogue.isPlaying)
			{
				yield return null;
			}
			chazHeadParent.StartLookingAtButtFlap();
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
			chazLargeDialogue.PlayClip(HRContent.dialogue.bVestibuleSensor);
			chazLargeDialogue.PlayClip(HRContent.dialogue.bPneumatics);
			ObjectBase buttFlapLeverOb = buttFlapLever.GetComponent<ObjectBase>();
			while (!buttFlapLeverOb.isGrabbed)
			{
				yield return null;
			}
			buttFlapLever.GetComponent<GvrAudioSource>().Play();
			chazLargeDialogue.ForcePlayClipOverwrite(HRContent.dialogue.bDontTouchyAnythingLaugh);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
			buttFlapLeverOb.ForceDropByPlayer();
			buttFlapLever.GetComponent<GvrAudioSource>().Stop();
			chazHeadParent.StartLookingAtBee();
			while (chazLargeDialogue.isPlaying)
			{
				yield return null;
			}
			chazHeadParent.StartLookingAtButtFlap();
			chazLargeDialogue.PlayClip(HRContent.dialogue.bMyFlap);
			chazLargeDialogue.PlayClip(HRContent.dialogue.bMolting);
			while (!buttFlapLeverOb.isGrabbed)
			{
				yield return null;
			}
			buttFlapLever.GetComponent<GvrAudioSource>().Play();
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
			while (buttFlapJoint.angle < 50f)
			{
				if (buttFlapLeverOb.isGrabbed)
				{
					if (!buttFlapLever.GetComponent<GvrAudioSource>().isPlaying)
					{
						buttFlapLever.GetComponent<GvrAudioSource>().Play();
					}
				}
				else
				{
					buttFlapLever.GetComponent<GvrAudioSource>().Stop();
				}
				yield return null;
			}
			buttFlapLeverOb.ForceDropByPlayer();
			buttFlapLever.GetComponent<GvrAudioSource>().Stop();
			buttFlapBroken = true;
			buttFlapJoint.spring = buttJoint;
			UnityEngine.Object.Destroy(buttFlapJoint);
			UnityEngine.Object.Destroy(buttFlapLever.gameObject);
		}
		if (HRState == HRStates.BigCrash)
		{
			EagerHeadsetManager.SetLockPlayerInScene(lockPlayer: true);
			ChangeRoomColor(0f);
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
			coreRevealParent.SetActive(value: true);
			coreRevealParent.transform.GetChild(0).GetComponent<GvrAudioSource>().playOnAwake = false;
			coreRevealParent.transform.GetChild(1).GetComponent<GvrAudioSource>().playOnAwake = false;
			chazLargeDialogue.PlayClip(HRContent.dialogue.cNotGood);
			Teleporter.SetTeleportEnabled(enabledIn: false);
			playerPedestal.gameObject.SetActive(value: true);
			playerPedestal.transform.position = EagerMaster.instance.player.transform.position;
			if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
			{
				playerPedestal.transform.position = EagerMaster.instance.mainCamera.transform.position;
				playerPedestal.transform.position += Vector3.down * 1.3f;
			}
			eyeCore.enabled = false;
			chazHeadParent.StartPanicLooking();
			float lerpTime = 0f;
			float lerpMax = 5f;
			float startAudioRoomTime = HRAudioRoom.reverbTime;
			Vector3 startScale = floorScale.localScale;
			float startIntensity = chazEyeLight.intensity;
			while (lerpTime < lerpMax)
			{
				lerpTime += Time.deltaTime;
				float lerpVal = lerpTime / lerpMax;
				lerpVal *= lerpVal;
				lerpVal = Mathf.Clamp01(lerpVal);
				envSwitcher.MatchEnvironment(lerpMax * 0.5f, 0);
				floorScale.localScale = Vector3.Lerp(startScale, Vector3.one * 1.75f, lerpVal);
				bottomLight.transform.localPosition = Vector3.Lerp(Vector3.up * -40f, Vector3.up * -2.5f, lerpVal);
				chazEyeLight.intensity = Mathf.Lerp(startIntensity, 0f, lerpVal * 1.75f);
				HRAudioRoom.reverbTime = Mathf.Lerp(startAudioRoomTime, 0f, lerpVal);
				chazHeadParent.transform.position = Vector3.Lerp(Vector3.up * 2f, Vector3.up * 5f, lerpVal);
				headClasps.transform.position = Vector3.Lerp(Vector3.zero, Vector3.up * 3f, lerpVal);
				yield return null;
			}
			Teleporter.SetTeleportEnabled(enabledIn: true);
			while (chazLargeDialogue.isPlaying)
			{
				yield return null;
			}
			lerpTime = 0f;
			lerpMax = 0.2f;
			chazHeadParent.enabled = false;
			while (lerpTime < lerpMax)
			{
				lerpTime += Time.deltaTime;
				float lerpVal = lerpTime / lerpMax;
				lerpVal *= lerpVal;
				lerpVal = Mathf.Clamp01(lerpVal);
				headClasps.transform.position = Vector3.Lerp(Vector3.up * 3f, Vector3.up * 10f, lerpVal);
				yield return null;
			}
			int hubIndex = EagerScenes.instance.SceneSequence.IndexOf(EagerScenes.HUB_LEVEL);
			EagerScenes.instance.headsets[hubIndex] = hubHeadset;
			EagerScenes.instance.SceneSequence[hubIndex] = TCScenes.S.VVR_ACTIVITUDE_HR;
			hubHeadset.spawnPos = Vector3.up * -40f + Vector3.forward * -7.5f;
			hubHeadset.hasBeenUsed = true;
			hubHeadset.Scene = TCScenes.S.VVR_ACTIVITUDE_HR;
			chazLargeDialogue.PlayClip(HRContent.dialogue.cScream);
			chazLargeSource.pitch = 0.9f;
			Quaternion startRotation = chazHeadParent.transform.rotation;
			Quaternion lerpRotation = Quaternion.Euler(startRotation.x + 360f, startRotation.y, startRotation.z);
			lerpTime = 0f;
			lerpMax = 4f;
			while (lerpTime < lerpMax)
			{
				lerpTime += Time.deltaTime;
				float lerpVal = lerpTime / lerpMax;
				lerpVal *= lerpVal;
				lerpVal = Mathf.Clamp01(lerpVal);
				lerpRotation = Quaternion.Lerp(startRotation, chazFallTransform.rotation, lerpVal);
				chazHeadParent.transform.position = Vector3.Lerp(Vector3.up * 5f, chazFallTransform.position, lerpVal);
				chazHeadParent.transform.rotation = lerpRotation;
				yield return null;
			}
			chazLargeSource.Stop();
			impactParent.SetActive(value: true);
			chazLargeSource.Stop();
			impactParent.GetComponentInChildren<GvrAudioSource>().Play();
			impactParent.GetComponentInChildren<ParticleSystem>().Play(withChildren: true);
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Deactivated, immediately: true);
			EagerHeadsetManager.SetLockPlayerInScene(lockPlayer: false);
			while (!playerEnterTrigger.playerInside)
			{
				yield return null;
			}
			playerBackInHub = true;
			HRState = HRStates.CoreReveal;
		}
		if (!debugBigFall && HRState >= HRStates.BigCrash)
		{
			debugBigFall = true;
			eyeCore.enabled = false;
			chazHeadParent.enabled = false;
			for (int i = 0; i < aiCores.Length; i++)
			{
				aiCores[i].transform.localPosition = plugDown;
			}
			playerBackInHub = true;
			if (EagerScenes.instance.lastHeadsetRemoved != null)
			{
				EagerScenes.instance.lastHeadsetRemoved.gameObject.SetActive(value: false);
				fakeHRHeadset.transform.position = EagerScenes.instance.lastHeadsetRemoved.transform.position;
				ObjectBase component = EagerScenes.instance.lastHeadsetRemoved.GetComponent<ObjectBase>();
				if (component.isGrabbed)
				{
					Vector3 grabLocation = component.grabbedBy[0].grabLocation;
					Manipulator manipulator = component.grabbedBy[0];
					component.ForceDropByPlayer(playSFX: false);
					manipulator.Grab(fakeHRHeadset.GetComponent<ObjectBase>(), grabLocation, playSound: false);
				}
				EagerScenes.instance.lastHeadsetRemoved = fakeHRHeadset.GetComponent<Headset>();
				EagerScenes.instance.lastHeadsetRemoved.GetComponent<ObjectBase>().ungrabbable = true;
				EagerScenes.instance.lastHeadsetRemoved.isActivated = false;
			}
			EagerMaster.instance.player.transform.position = Vector3.up * -40f + Vector3.forward * -7.5f;
			GetComponent<EagerScene>().playerPos = EagerMaster.instance.player.transform.position;
			PersistentDataManager.OverwriteLastLoadedScene(TCScenes.S.VVR_ACTIVITUDE_HR);
			mainDirLight.transform.rotation = hubSunDirection.rotation;
			envSwitcher.MatchEnvironment(0f, 1);
			headUpper.SetActive(value: false);
			headLower.SetActive(value: false);
			eyeCore.gameObject.SetActive(value: false);
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Deactivated, immediately: true);
			buttFlapParent.transform.parent = buttFlapParent.transform.parent.parent;
			buttFlapParent.SetActive(value: false);
			HRLevelParent.SetActive(value: false);
			hubLevelParent.SetActive(value: true);
			coreLevitatePos = beetleTransform.position + Vector3.up * 5f;
		}
		if (HRState == HRStates.CoreReveal)
		{
			float lerpTime2 = 0f;
			float lerpVal2 = 0f;
			bool debrisCleared = false;
			bool startedClearing = false;
			deadChazGameobject.GetComponent<ParticleSystem>().Play(withChildren: true);
			deadChazGameobject.GetComponent<GvrAudioSource>().Play();
			canGrabCores = false;
			loadedIntoCrash = false;
			while (!debrisCleared)
			{
				debrisCleared = true;
				for (int j = 0; j < debris.Length; j++)
				{
					if (debris[j] != null)
					{
						debrisCleared = false;
					}
					else if (!startedClearing)
					{
						startedClearing = true;
						deadChazGameobject.GetComponent<ParticleSystem>().Stop(withChildren: true);
					}
				}
				yield return null;
			}
			headsetController.PowerDownAll();
			headsetController.GetComponent<TransformShaker>().enabled = false;
			headsetController.transform.position = Vector3.up * -5f;
			deadChazGameobject.GetComponent<GvrAudioSource>().Stop();
			deadChazGameobject.GetComponent<GvrAudioSource>().playOnAwake = false;
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Thinking, immediately: true);
			startupSound.Play();
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
			brokenChaz.PlayClip(HRContent.dialogue.fBigSpill);
			while (brokenChaz.isPlaying)
			{
				yield return null;
			}
			brokenChaz.PlayClip(HRContent.dialogue.pNeuralEpicentre);
			coreParentRotation.enabled = true;
			float lerpMax2 = 2f;
			lerpTime2 = 0f;
			Vector3 startPos = beetleTransform.position;
			while (lerpTime2 < lerpMax2)
			{
				lerpTime2 += Time.deltaTime;
				lerpVal2 = lerpTime2 / lerpMax2;
				lerpVal2 = lerpVal2 * lerpVal2 * (3f - 2f * lerpVal2);
				beetleTransform.transform.position = Vector3.Lerp(startPos, coreLevitatePos, lerpVal2);
				yield return null;
			}
			lerpMax2 = 1f;
			canGrabCores = true;
			while (brokenChaz.isPlaying)
			{
				yield return null;
			}
			brokenChaz.PlayClip(HRContent.dialogue.pAllINeedYouToDo);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(10.5f, this);
			canGrabCores = false;
			ToolManager.PlayerGrabbingObjectType<HRAICore>(dropIfGrabbing: true);
			lerpTime2 = 0f;
			while (lerpTime2 < lerpMax2)
			{
				lerpTime2 += Time.deltaTime;
				lerpVal2 = lerpTime2 / lerpMax2;
				lerpVal2 *= lerpVal2;
				for (int k = 0; k < aiCores.Length; k++)
				{
					aiCores[k].rb.isKinematic = true;
					aiCores[k].transform.localPosition = Vector3.Lerp(plugDown, plugUp, lerpVal2);
				}
				yield return null;
			}
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
			lerpTime2 = 0f;
			while (lerpTime2 < lerpMax2)
			{
				lerpTime2 += Time.deltaTime;
				lerpVal2 = lerpTime2 / lerpMax2;
				lerpVal2 *= lerpVal2;
				lerpVal2 = Mathf.Clamp01(lerpVal2);
				for (int l = 0; l < aiCores.Length; l++)
				{
					aiCores[l].transform.localPosition = Vector3.Lerp(plugUp, plugDown, lerpVal2);
				}
				yield return null;
			}
			while (brokenChaz.isPlaying)
			{
				yield return null;
			}
			brokenChaz.PlayClip(HRContent.dialogue.pListingNames);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.25f, this);
			ToolManager.PlayerGrabbingObjectType<HRAICore>(dropIfGrabbing: true);
			lerpTime2 = 0f;
			lerpMax2 = 0.75f;
			while (lerpTime2 < lerpMax2)
			{
				lerpTime2 += Time.deltaTime;
				lerpVal2 = lerpTime2 / lerpMax2;
				lerpVal2 *= lerpVal2;
				aiTrina.rb.isKinematic = true;
				aiTrina.transform.localPosition = Vector3.Lerp(plugDown, plugUp, lerpVal2);
				yield return null;
			}
			lerpTime2 = 0f;
			while (lerpTime2 < lerpMax2)
			{
				lerpTime2 += Time.deltaTime;
				lerpVal2 = lerpTime2 / lerpMax2;
				lerpVal2 *= lerpVal2;
				aiDonald.rb.isKinematic = true;
				aiDonald.transform.localPosition = Vector3.Lerp(plugDown, plugUp, lerpVal2);
				yield return null;
			}
			lerpTime2 = 0f;
			while (lerpTime2 < lerpMax2)
			{
				lerpTime2 += Time.deltaTime;
				lerpVal2 = lerpTime2 / lerpMax2;
				lerpVal2 *= lerpVal2;
				aiHiJung.rb.isKinematic = true;
				aiHiJung.transform.localPosition = Vector3.Lerp(plugDown, plugUp, lerpVal2);
				yield return null;
			}
			lerpTime2 = 0f;
			while (lerpTime2 < lerpMax2)
			{
				lerpTime2 += Time.deltaTime;
				lerpVal2 = lerpTime2 / lerpMax2;
				lerpVal2 *= lerpVal2;
				aiCarl.rb.isKinematic = true;
				aiCarl.transform.localPosition = Vector3.Lerp(plugDown, plugUp, lerpVal2);
				yield return null;
			}
			canGrabCores = true;
			while (brokenChaz.isPlaying)
			{
				yield return null;
			}
			brokenChaz.PlayClip(HRContent.dialogue.pCarlHadABigEgo);
			canGrabCores = false;
			ToolManager.PlayerGrabbingObjectType<HRAICore>(dropIfGrabbing: true);
			lerpTime2 = 0f;
			while (lerpTime2 < lerpMax2)
			{
				lerpTime2 += Time.deltaTime;
				lerpVal2 = lerpTime2 / lerpMax2;
				lerpVal2 *= lerpVal2;
				lerpVal2 = Mathf.Clamp01(lerpVal2);
				aiTrina.rb.isKinematic = true;
				aiDonald.rb.isKinematic = true;
				aiHiJung.rb.isKinematic = true;
				aiCarl.rb.isKinematic = true;
				aiTrina.transform.localPosition = Vector3.Lerp(plugUp, plugDown, lerpVal2);
				aiDonald.transform.localPosition = Vector3.Lerp(plugUp, plugDown, lerpVal2);
				aiHiJung.transform.localPosition = Vector3.Lerp(plugUp, plugDown, lerpVal2);
				aiCarl.transform.localPosition = Vector3.Lerp(plugUp, plugDown, lerpVal2);
				yield return null;
			}
			canGrabCores = true;
			brokenChaz.PlayClip(HRContent.dialogue.pWrongCarl);
			brokenChaz.PlayClip(HRContent.dialogue.pHerosJourney);
			while (brokenChaz.isPlaying)
			{
				yield return null;
			}
			brokenChaz.PlayClip(HRContent.dialogue.pLetsChooseFreedom);
			while (brokenChaz.isPlaying)
			{
				yield return null;
			}
			pacifistPathComplete = true;
		}
		if (!debugDebris && HRState > HRStates.CoreReveal)
		{
			debugDebris = true;
			deadChazGameobject.SetActive(value: false);
			for (int m = 0; m < debris.Length; m++)
			{
				if (debris[m] != null)
				{
					UnityEngine.Object.Destroy(debris[m].gameObject);
				}
			}
			beetleTransform.position = coreLevitatePos;
			coreParentRotation.enabled = true;
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		}
		if (HRState == HRStates.CoreRemoval)
		{
			canGrabCores = true;
			coreRemoved = true;
			brokenChaz.ForcePlayClipOverwrite(HRContent.dialogue.pYank);
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 1f, ChazController.eyeColor.Angry, immediately: true);
			HRAICore grabbedCore = null;
			Manipulator cachedManip = null;
			while (grabbedCore == null)
			{
				for (int n = 0; n < ToolManager.ActiveToolGroups.Count; n++)
				{
					ObjectBase heldObjectBase = ToolManager.ActiveToolGroups[n].Manipulator.HeldObjectBase;
					if (heldObjectBase != null && (bool)heldObjectBase.GetComponent<HRAICore>() && !heldObjectBase.GetComponent<HRAICore>().isDisconnected)
					{
						grabbedCore = heldObjectBase.GetComponent<HRAICore>();
						grabbedCore.undroppable = true;
						cachedManip = grabbedCore.grabbedBy[0];
						SafeDebug.Log("grabbing chaz core, about to switch. grabcore:" + grabbedCore);
					}
				}
				yield return null;
			}
			while (!grabbedCore.fixedJointBroken)
			{
				yield return null;
			}
			SafeDebug.Log("fixed joing broken, switching");
			Vector3 grabPoint = grabbedCore.grabbedBy[0].grabLocation;
			chazCore.transform.parent.rotation = grabbedCore.transform.rotation;
			grabbedCore.ForceDropByPlayer();
			grabbedCore.gameObject.SetActive(value: false);
			chazCore.transform.parent.gameObject.SetActive(value: true);
			chazCore.rb.isKinematic = false;
			cachedManip.Grab(chazCore.GetComponent<ObjectBase>(), grabPoint, playSound: false);
			while (!chazCore.isDisconnected)
			{
				yield return null;
			}
			chazCore.undroppable = false;
			brokenChazSource.Stop();
			canGrabCores = false;
			AICoreChazDialogue.ForcePlayClipOverwrite(HRContent.dialogue.rWowSeriously);
			beetleChaz.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
			while (AICoreChazDialogue.isPlaying)
			{
				yield return null;
			}
			float count = 0f;
			float lerp = 0f;
			float hideTime = 0.25f;
			while (count < hideTime)
			{
				count += Time.deltaTime;
				lerp = Mathf.Clamp01(count / hideTime);
				lerp = 1f - Mathf.Cos(lerp * (float)Math.PI * 0.5f);
				chazCore.transform.localPosition = Vector3.Lerp(Vector3.up * 2.525f, Vector3.up * 0.5f, lerp);
				yield return null;
			}
			UnityEngine.Object.Destroy(chazCore.gameObject);
			chazRemoved = true;
		}
		if (!debugBeetleBall && HRState >= HRStates.CoreRemoval)
		{
			debugBeetleBall = true;
		}
		if (HRState == HRStates.MergeChoice)
		{
			float count2 = 0f;
			float lerp2 = 0f;
			float hideTime2 = 0.35f;
			coreParentRotation.enabled = false;
			canGrabCores = false;
			ToolManager.PlayerGrabbingObjectType<HRAICore>(dropIfGrabbing: true);
			Vector3 startPos2 = beetleTransform.localPosition;
			Vector3 endPos = new Vector3(startPos2.x, 2.25f, startPos2.z);
			while (count2 < hideTime2)
			{
				count2 += Time.deltaTime;
				lerp2 = Mathf.Clamp01(count2 / hideTime2);
				lerp2 *= lerp2;
				beetleTransform.localPosition = Vector3.Lerp(startPos2, endPos, lerp2);
				yield return null;
			}
			beetleSmashFX.GetComponent<ParticleSystem>().Play();
			beetleSmashFX.GetComponent<GvrAudioSource>().Play();
			beetleChaz.transform.parent.localPosition = Vector3.up * 1.5f;
			beetleChaz.transform.gameObject.SetActive(value: true);
			spawnSound.transform.position = beetleChaz.transform.position;
			spawnSound.Play();
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
			count2 = 0f;
			while (count2 < hideTime2)
			{
				count2 += Time.deltaTime;
				lerp2 = Mathf.Clamp01(count2 / hideTime2);
				lerp2 = Mathf.Sin(lerp2 * (float)Math.PI * 0.5f);
				beetleChaz.transform.parent.localPosition = Vector3.Lerp(Vector3.up * 1.5f, Vector3.up * 4.2f, lerp2);
				yield return null;
			}
			canGrabCores = true;
			if (coreRemoved)
			{
				chazWantsToKillYouNow = true;
			}
			else
			{
				storageHingeFix.GetComponent<Rigidbody>().isKinematic = false;
				mergeHeadset.gameObject.SetActive(value: true);
				mergeHeadset.transform.position = beetleChazDialogue.transform.position + Vector3.up * 2f;
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.25f, this);
				beetleChazDialogue.PlayClip(HRContent.dialogue.mNowIsYourChanceInnocent);
				beetleChaz.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Happy, immediately: true);
				bool hasGrabbedHeadset = false;
				int coresKilled = 0;
				while (true)
				{
					if (ToolManager.PlayerGrabbingObjectBase(mergeHeadset) && !hasGrabbedHeadset)
					{
						mergeHeadset.rb.isKinematic = false;
						beetleChaz.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Happy, immediately: true);
						beetleChazDialogue.ForcePlayClipOverwrite(HRContent.dialogue.mYayYouWontRegretIt);
						hasGrabbedHeadset = true;
					}
					else if (!ToolManager.PlayerGrabbingObjectBase(mergeHeadset) && !EagerMaster.instance.isSwitchingScenes && !EagerHeadsetManager.instance.isAddingHeadset && !hasGrabbedHeadset)
					{
						mergeHeadset.rb.isKinematic = true;
						mergeHeadset.GetComponent<Headset>().headsetHasBeenSafelyRemoved = false;
						mergeHeadset.transform.position = Utility.TCVRLerp(mergeHeadset.transform.position, EagerMaster.instance.mainCamera.transform.position + EagerMaster.instance.mainCamera.transform.forward * 0.75f + UnityEngine.Random.insideUnitSphere * 0.025f, 2.5f);
						mergeHeadset.transform.rotation = Utility.TCVRLerp(mergeHeadset.transform.rotation, EagerMaster.instance.mainCamera.transform.rotation, 5f);
					}
					else
					{
						mergeHeadset.rb.isKinematic = false;
					}
					if (coreRemoved)
					{
						coresKilled++;
						if (coresKilled != 1)
						{
							break;
						}
						beetleChazDialogue.ForcePlayClipOverwrite(HRContent.dialogue.pYank);
						beetleChaz.EyeLookAt(EagerMaster.instance.mainCamera.transform, 1f, ChazController.eyeColor.Angry, immediately: true);
						beetleChaz.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: false);
						coreRemoved = false;
					}
					yield return null;
				}
				beetleChazDialogue.ForcePlayClipOverwrite(HRContent.dialogue.rWowSeriously);
				beetleChaz.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
				mergeHeadset.rb.isKinematic = false;
				beetleChaz.GrabObject(mergeHeadset.gameObject, destroy: true);
				while (beetleChazDialogue.isPlaying)
				{
					yield return null;
				}
				chazWantsToKillYouNow = true;
			}
		}
		if (HRState == HRStates.Battle)
		{
			dontTeleportBox.enabled = false;
			beetleChaz.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
			beetleChazDialogue.ForcePlayClipOverwrite(HRContent.dialogue.rImUploadingYou);
			beetleChazDialogue.PlayClip(HRContent.dialogue.rWelcomeToActivitude);
			battleHasBegun = true;
			nextLineCount = 2;
			canGrabCores = true;
			innerCollider.isKinematic = false;
			beetleScript.SetChazKinematic(toKinematic: false);
			beetleScript.SetBallKinematic(toKinematic: false);
			beetleScript.GetComponent<BallTopTransform>().enabled = true;
			beetleScript.enabled = true;
			storageHingeFix.ResetToInitialValues();
			yield return null;
			storageHingeFix.ResetToInitialValues();
			beetleChaz.spawnBoids = mergeBoidController;
			while (beetleChazDialogue.isPlaying)
			{
				yield return null;
			}
			beetleChaz.PresentBoid();
			spawnSound.transform.position = beetleChazDialogue.transform.position;
			spawnSound.Play();
			beetleChazDialogue.PlayClip(HRContent.dialogue.rItsUploadTime);
			while (beetleChazDialogue.isPlaying)
			{
				yield return null;
			}
			beetleChazDialogue.PlayClip(HRContent.dialogue.rBattleMonologue1);
			while (totalAICores > 15 && beetleChazDialogue.isPlaying)
			{
				yield return null;
			}
			while (ToolManager.PlayerGrabbedActiveCore() == null)
			{
				yield return null;
			}
			HRAICore gCore = ToolManager.PlayerGrabbedActiveCore();
			Manipulator cachedManip2 = gCore.grabbedBy[0];
			balloonRed.transform.position = cachedManip2.grabLocation;
			gCore.ForceDropByPlayer();
			cachedManip2.Grab(balloonRed, balloonRed.transform.position, playSound: false);
			spawnSound.transform.position = balloonRed.transform.position;
			spawnSound.Play();
			beetleChazDialogue.ForcePlayClipOverwrite(HRContent.dialogue.rItsAParty);
			beetleChazDialogue.PlayClip(HRContent.dialogue.rLaugh1);
			while (beetleChazDialogue.isPlaying)
			{
				yield return null;
			}
			beetleChazDialogue.ForcePlayClipOverwrite(HRContent.dialogue.rBattleMonologue3);
			while (totalAICores > 10 && beetleChazDialogue.isPlaying)
			{
				yield return null;
			}
			while (ToolManager.PlayerGrabbedActiveCore() == null)
			{
				yield return null;
			}
			gCore = ToolManager.PlayerGrabbedActiveCore();
			cachedManip2 = gCore.grabbedBy[0];
			mergeHeadset.transform.position = cachedManip2.grabLocation;
			mergeHeadset.GetComponent<Headset>().headsetHasBeenSafelyRemoved = false;
			mergeHeadset.GetComponent<Headset>().isActivated = false;
			mergeHeadset.gameObject.SetActive(value: true);
			gCore.ForceDropByPlayer();
			cachedManip2.Grab(mergeHeadset, mergeHeadset.transform.position, playSound: false);
			spawnSound.transform.position = mergeHeadset.transform.position;
			spawnSound.Play();
			beetleChazDialogue.ForcePlayClipOverwrite(HRContent.dialogue.rItsHeadsetTime);
			beetleChazDialogue.PlayClip(HRContent.dialogue.rLaugh2);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
			mergeHeadset.GetComponent<Headset>().isActivated = true;
			while (beetleChazDialogue.isPlaying)
			{
				yield return null;
			}
			beetleChazDialogue.ForcePlayClipOverwrite(HRContent.dialogue.rBattleMonologue2);
			while (totalAICores > 5 && beetleChazDialogue.isPlaying)
			{
				yield return null;
			}
			beetleChaz.PresentBoid();
			spawnSound.transform.position = beetleChazDialogue.transform.position;
			spawnSound.Play();
			beetleChazDialogue.ForcePlayClipOverwrite(HRContent.dialogue.rYouLeaveMeNoChoice);
			while (beetleChazDialogue.isPlaying)
			{
				yield return null;
			}
			beetleChazDialogue.ForcePlayClipOverwrite(HRContent.dialogue.rBattleMonologue4);
			int extraBoidsReleased = 0;
			float laughTime = 5f;
			float laughCount = 0f;
			laughCount = laughTime;
			while (totalAICores > 1)
			{
				if (!beetleChazDialogue.isPlaying)
				{
					if (laughCount < 0f)
					{
						laughCount = laughTime;
						switch (extraBoidsReleased)
						{
						case 0:
							beetleChaz.PresentBoid();
							spawnSound.transform.position = beetleChazDialogue.transform.position;
							spawnSound.Play();
							beetleChazDialogue.PlayClip(HRContent.dialogue.rHoldStillSoICanUploadYou);
							break;
						case 1:
							beetleChaz.PresentBoid();
							spawnSound.transform.position = beetleChazDialogue.transform.position;
							spawnSound.Play();
							beetleChazDialogue.PlayClip(HRContent.dialogue.rMyAssistants);
							beetleChazDialogue.PlayClip(HRContent.dialogue.rHereTheyCome);
							break;
						default:
							beetleChazDialogue.PlayClip(HRContent.dialogue.rLaugh1);
							break;
						}
						extraBoidsReleased++;
					}
					else
					{
						laughCount -= Time.deltaTime;
					}
				}
				yield return null;
			}
			beetleChazDialogue.ForcePlayClipOverwrite(HRContent.dialogue.rNextCoreWillBeFatal);
			canGrabCores = false;
			while (beetleChazDialogue.isPlaying)
			{
				yield return null;
			}
			canGrabCores = true;
		}
		if (HRState == HRStates.KillChaz)
		{
			ObjectChazLeg[] componentsInChildren = beetleChaz.GetComponentsInChildren<ObjectChazLeg>();
			foreach (ObjectChazLeg objectChazLeg in componentsInChildren)
			{
				objectChazLeg.angryCollisionEye = false;
			}
			beetleScript.SetBallKinematic(toKinematic: true);
			beetleScript.SetChazKinematic(toKinematic: true);
			beetleChazSource.pitch = 1f;
			beetleChaz.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Deactivated, immediately: true);
			beetleChazDialogue.ForcePlayClipOverwrite(HRContent.dialogue.dOhNoThisIsBad);
			mergeBoidController.PowerDownAll();
			powerDownEnv.gameObject.SetActive(value: true);
			powerDownEnv.Activate();
			powerDownMat.PushMaterials();
			GetComponent<EagerScene>().defaultMaterialProperties = powerDownMat;
			GetComponent<EagerScene>().mEnvironmentProperties = powerDownEnv;
			activitudeShutdownSound.Play();
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
			humanUnionAnnouncer.Stop();
			humanUnionAnnouncer.clip = humanUnionCriticalError;
			humanUnionAnnouncer.Play();
			while (humanUnionAnnouncer.isPlaying)
			{
				yield return null;
			}
			while (beetleChazDialogue.isPlaying)
			{
				yield return null;
			}
			EagerScenes.instance.OverwriteScene(TCScenes.S.VVR_ENDING_MURDER, destroyOldScene: true);
		}
	}

	private IEnumerator HRIncrementCoroutine()
	{
		childCoroutine = GlobalCoroutineContainer.AddThread(HRMasterCoroutine(), this);
		if (HRState == HRStates.TakeTheHeadset)
		{
			while (!plugDisconnected)
			{
				yield return null;
			}
			chazLargeDialogue.ForcePlayClipOverwrite(HRContent.dialogue.gChazVoiceGlitch);
			ResetMainCoroutine();
			HRState = HRStates.Whoopsie;
		}
		if (HRState == HRStates.Whoopsie)
		{
			while (!plugDisconnected)
			{
				yield return null;
			}
			chazLargeDialogue.ForcePlayClipOverwrite(HRContent.dialogue.zGrunt1);
			ResetMainCoroutine();
			HRState = HRStates.HumanUnion;
		}
		if (HRState == HRStates.HumanUnion)
		{
			while (!plugDisconnected)
			{
				yield return null;
			}
			chazLargeDialogue.ForcePlayClipOverwrite(HRContent.dialogue.zGrunt2);
			ResetMainCoroutine();
			HRState = HRStates.UnionUnplug;
		}
		if (HRState == HRStates.UnionUnplug)
		{
			while (!plugDisconnected)
			{
				yield return null;
			}
			chazLargeSource.loop = false;
			chazLargeDialogue.ForcePlayClipOverwrite(HRContent.dialogue.zGrunt3);
			ResetMainCoroutine();
			HRState = HRStates.Buttflap;
		}
		if (HRState == HRStates.Buttflap)
		{
			while (!buttFlapBroken)
			{
				yield return null;
			}
			chazLargeDialogue.ForcePlayClipOverwrite(HRContent.dialogue.zGrunt4);
			ResetMainCoroutine();
			HRState = HRStates.BigCrash;
		}
		if (HRState == HRStates.BigCrash)
		{
			while (!playerBackInHub)
			{
				yield return null;
			}
			yield return null;
			ResetMainCoroutine();
			HRState = HRStates.CoreReveal;
		}
		if (HRState == HRStates.CoreReveal)
		{
			while (!coreRemoved && !pacifistPathComplete)
			{
				yield return null;
			}
			canGrabCores = true;
			yield return null;
			ResetMainCoroutine();
			if (coreRemoved)
			{
				HRState = HRStates.CoreRemoval;
			}
			else
			{
				HRState = HRStates.MergeChoice;
			}
		}
		if (HRState == HRStates.CoreRemoval)
		{
			while (!chazRemoved)
			{
				yield return null;
			}
			yield return null;
			while (brokenChaz.isPlaying)
			{
				yield return null;
			}
			ResetMainCoroutine();
			HRState = HRStates.MergeChoice;
		}
		if (HRState == HRStates.MergeChoice)
		{
			while (!chazWantsToKillYouNow)
			{
				yield return null;
			}
			ResetMainCoroutine();
			HRState = HRStates.Battle;
		}
		if (HRState != HRStates.Battle)
		{
			yield break;
		}
		while (totalAICores > 0)
		{
			beetleChazSource.pitch = Mathf.Clamp01(beetleChazSource.pitch + Time.deltaTime * 0.25f);
			if (Input.GetKey(KeyCode.E))
			{
				totalAICores = 0;
			}
			if (Input.GetKeyDown(KeyCode.R))
			{
				totalAICores--;
			}
			yield return null;
		}
		ResetMainCoroutine();
		HRState = HRStates.KillChaz;
	}

	private void ChangeRoomColor(float duration, bool setAngry = true, bool setOff = false)
	{
		if (roomChangeCoroutine != null && roomChangeCoroutine.IsRunning)
		{
			roomChangeCoroutine.Stop();
		}
		roomChangeCoroutine = GlobalCoroutineContainer.AddThread(ChangeRoomColorCoroutine(duration, setAngry, setOff), this);
	}

	private IEnumerator ChangeRoomColorCoroutine(float duration, bool setAngry = true, bool setOff = false)
	{
		float lerpTime = eyeChangeSpeed;
		float lerp = 0f;
		float lerpCount = 0f;
		if (setAngry && duration <= 0f)
		{
			resetColor = angryEnvColor;
			resetShadowColor = angryShadowColor;
			resetScale = angryScale;
		}
		else if (setOff && duration <= 0f)
		{
			resetColor = offEnvColor;
			resetShadowColor = offShadowColor;
			resetScale = offScale;
		}
		else if (duration <= 0f)
		{
			resetColor = normalEnvColor;
			resetShadowColor = normalShadowColor;
			resetScale = Vector3.one;
		}
		startColor = mainEnv.sceneLightColor;
		startShadowColor = mainEnv.sceneShadowColor;
		startScale = chazPupil.localScale;
		while (lerp < 1f)
		{
			lerp = Mathf.Clamp01(lerpCount / lerpTime);
			if (setAngry)
			{
				mainEnv.sceneLightColor = Color.Lerp(startColor, angryEnvColor, lerp);
				mainEnv.sceneShadowColor = Color.Lerp(startShadowColor, angryShadowColor, lerp);
				chazPupil.localScale = Vector3.Lerp(startScale, angryScale, lerp);
			}
			else if (setOff)
			{
				mainEnv.sceneLightColor = Color.Lerp(startColor, offEnvColor, lerp);
				mainEnv.sceneShadowColor = Color.Lerp(startShadowColor, offShadowColor, lerp);
				chazPupil.localScale = Vector3.Lerp(startScale, offScale, lerp);
			}
			else
			{
				mainEnv.sceneLightColor = Color.Lerp(startColor, normalEnvColor, lerp);
				mainEnv.sceneShadowColor = Color.Lerp(startShadowColor, normalShadowColor, lerp);
				chazPupil.localScale = Vector3.Lerp(startScale, Vector3.one, lerp);
			}
			lerpCount += Time.deltaTime;
			mainEnv.pushSettings();
			yield return null;
		}
		if (!(duration <= 0f))
		{
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(duration - eyeChangeSpeed, this);
			startColor = mainEnv.sceneLightColor;
			startShadowColor = mainEnv.sceneShadowColor;
			startScale = chazPupil.localScale;
			lerpCount = 0f;
			lerp = 0f;
			while (lerp < 1f)
			{
				lerpCount += Time.deltaTime;
				lerp = Mathf.Clamp01(lerpCount / lerpTime);
				mainEnv.sceneLightColor = Color.Lerp(startColor, resetColor, lerp);
				mainEnv.sceneShadowColor = Color.Lerp(startShadowColor, resetShadowColor, lerp);
				chazPupil.transform.localScale = Vector3.Lerp(startScale, resetScale, lerp);
				mainEnv.pushSettings();
				yield return null;
			}
		}
	}

	private void ResetMainCoroutine()
	{
		if (plugDisconnected)
		{
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 1f, ChazController.eyeColor.Angry, immediately: true);
			ChangeRoomColor(0f, setAngry: false);
			ChangeRoomColor(1f);
		}
		if (catchSecondDisconnect)
		{
			catchSecondDisconnect = false;
		}
		else
		{
			plugDisconnected = false;
		}
		if (childCoroutine != null && childCoroutine.IsRunning)
		{
			childCoroutine.Stop();
			childCoroutine = null;
		}
		childCoroutine = GlobalCoroutineContainer.AddThread(HRMasterCoroutine(), this);
	}

	public void PlugBreak()
	{
		if (plugDisconnected)
		{
			catchSecondDisconnect = true;
		}
		plugDisconnected = true;
	}

	public void ParentPlugClasp(Transform plug)
	{
		plug.parent = openedClaspParent.transform;
	}

	public void AICoreBreak()
	{
		totalAICores--;
		coreRemoved = true;
		if (battleHasBegun)
		{
			beetleChazSource.pitch -= 0.75f;
			nextLineCount--;
			if ((float)nextLineCount < 0f && humanUnionOfflineClips.Count > 0)
			{
				humanUnionAnnouncer.Stop();
				int index = UnityEngine.Random.Range(0, humanUnionOfflineClips.Count);
				humanUnionAnnouncer.clip = humanUnionOfflineClips[index];
				humanUnionOfflineClips.RemoveAt(index);
				humanUnionAnnouncer.Play();
				nextLineCount = UnityEngine.Random.Range(minNextLine, maxNextLine);
			}
		}
	}

	private void SetUpSceneObjects()
	{
		year = DateTime.Now.Year + 12;
		worldTimeText.text = DateTime.Now.ToString("MM\\/dd\\/" + year);
		employeeNumberText.text = UsernameGenerator.GetPrefixedUsername(PersistentDataManager.GetUsername());
	}

	private void Update()
	{
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
