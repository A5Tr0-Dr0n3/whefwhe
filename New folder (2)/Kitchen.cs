using System.Collections;
using System.Collections.Generic;
using GVR.Gfx;
using UnityEngine;

public class Kitchen : SceneBehaviorManager
{
	public enum KitchenStates
	{
		StartSequence,
		GrabAndPlaceButter,
		ToasterTutorial,
		ToasterSpawn,
		ToasterStop,
		SpawnVacuum,
		VacuumDestruction,
		ChangeScenes
	}

	[SerializeField]
	private KitchenStates kitchenState;

	public KitchenSequenceScriptableObject tweakables;

	public VVRConsumable[] invincibles;

	public ObjectToaster tutorialToaster;

	public ObjectToaster fireToaster;

	public ObjectButter objectButter;

	public ObjectDustBuster vacuum;

	public Transform butterPlaceTarget;

	public GameObject sceneDeleteSounds;

	[SerializeField]
	private KitchenFridgeDoorTrigger kitchenDoor;

	[SerializeField]
	private VVRDialogueSource butterDialogueSource;

	[SerializeField]
	private ObjectToaster[] kitchenToasters;

	[SerializeField]
	private AudioClip toasterPopInClip;

	[SerializeField]
	private GvrAudioSource radioMusicSource;

	[SerializeField]
	private AudioClip radioStatic;

	[SerializeField]
	private AudioClip radioSmoothMusic;

	[SerializeField]
	private AudioClip staticHit;

	[SerializeField]
	private GameObject roomWalls;

	[SerializeField]
	private GameObject[] loadOnChange;

	[SerializeField]
	private GameObject UI;

	[SerializeField]
	private GameObject matUpdate;

	[SerializeField]
	private VVRConsumable lamp;

	[SerializeField]
	private EnvironmentProperties kitchenEnviroProperties;

	[SerializeField]
	private MaterialUpdateController kitchenMaterialProperties;

	[SerializeField]
	public MaterialUpdateController kitchenMaterialPropertiesDark;

	[SerializeField]
	private GameObject radioSubtitles;

	[SerializeField]
	private VVRConsumable tableConsumable;

	private bool radioPlaying;

	public GameObject butterEatTrigger;

	public SpawnScalePulse busterSpawn;

	public AudioEvent SpawnFX;

	public FixedJoint busterJoint;

	public List<VVRConsumable> boidCleanup;

	public List<GameObject> deleteOnBackstage;

	private bool toastPendingJudgment;

	private ObjectToaster.Toast lastToastEvaluation;

	private int lightFlickerCount = 19;

	public static Kitchen instance;

	private Rigidbody toastFloatRB;

	[HideInInspector]
	public int[] toastCount = new int[3];

	private void Awake()
	{
		if (instance != null)
		{
			throw new UnityException("Only one instance of Kitchen in a scene is allowed.");
		}
		instance = this;
	}

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		PersistentDataManager.WriteSaveDelay(5f);
		EagerScenes.instance.LoadQueue(EagerScenes.BACKSTAGE_LEVEL);
		PersistentDataManager.OverwriteTutorialState(TutorialLevelSequence.TutorialStates.Finished);
		SetUpSceneObjects();
		GlobalCoroutineContainer.AddThread(KitchenCoroutine(), this);
	}

	private void SetUpSceneObjects()
	{
		ObjectToaster[] array = kitchenToasters;
		foreach (ObjectToaster objectToaster in array)
		{
			objectToaster.gameObject.SetActive(value: false);
		}
		tutorialToaster.gameObject.SetActive(value: false);
		vacuum.gameObject.SetActive(value: false);
		for (int j = 0; j < loadOnChange.Length; j++)
		{
			loadOnChange[j].SetActive(value: false);
		}
		UI.SetActive(value: false);
		butterEatTrigger.SetActive(value: false);
	}

	private IEnumerator KitchenCoroutine()
	{
		if (kitchenState == KitchenStates.StartSequence)
		{
			radioMusicSource.clip = radioStatic;
			radioMusicSource.Play();
			objectButter.sticky = false;
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
			int attentionGrabberIndex = 0;
			float paddingTimer = 0f;
			while (!kitchenDoor.playerOpened)
			{
				if (!butterDialogueSource.isPlaying)
				{
					if (paddingTimer > 0f)
					{
						paddingTimer -= Time.deltaTime;
					}
					else if (tweakables.dialogue.aHeyOverHereAttentionGrabbers.Length > attentionGrabberIndex)
					{
						butterDialogueSource.PlayClip(tweakables.dialogue.aHeyOverHereAttentionGrabbers[attentionGrabberIndex]);
						attentionGrabberIndex++;
						paddingTimer = tweakables.attentionGrabberPadding;
					}
					else
					{
						butterDialogueSource.PlayClip(tweakables.dialogue.aAdditionalAttentionGrabbers[Mathf.FloorToInt(Random.Range(0, tweakables.dialogue.aAdditionalAttentionGrabbers.Length))]);
						paddingTimer = tweakables.additionalAttentionGrabberPadding;
					}
				}
				yield return null;
			}
			while (butterDialogueSource.isPlaying)
			{
				yield return null;
			}
			kitchenState = KitchenStates.GrabAndPlaceButter;
		}
		if (kitchenState == KitchenStates.GrabAndPlaceButter)
		{
			butterDialogueSource.PlayClip(tweakables.dialogue.aYoureNewHere);
			while (butterDialogueSource.isPlaying && !ToolManager.PlayerGrabbingObjectBase(objectButter))
			{
				yield return null;
			}
			if (!ToolManager.PlayerGrabbingObjectBase(objectButter))
			{
				butterDialogueSource.PlayClip(tweakables.dialogue.aPickMeUp);
			}
			while (!ToolManager.PlayerGrabbingObjectBase(objectButter))
			{
				yield return null;
			}
			while (butterDialogueSource.isPlaying)
			{
				yield return null;
			}
			butterDialogueSource.PlayClip(tweakables.dialogue.aPutMeOnTable);
			float butterIsDroppedTimer = 0f;
			bool hasGrabbedButterOnce = false;
			float butterTableDistance = 10f;
			while (butterTableDistance > 0.025f)
			{
				if (ToolManager.PlayerGrabbingObjectBase(objectButter))
				{
					hasGrabbedButterOnce = true;
					butterIsDroppedTimer = 0f;
				}
				if (hasGrabbedButterOnce && !ToolManager.PlayerGrabbingObjectBase(objectButter))
				{
					butterIsDroppedTimer += Time.deltaTime;
					if (butterIsDroppedTimer > tweakables.chatiseDelayAfterButterDrop)
					{
						butterDialogueSource.PlayClip(tweakables.dialogue.aTableAreYouIncapable);
						while (butterDialogueSource.isPlaying)
						{
							yield return null;
						}
						objectButter.transform.position = butterPlaceTarget.position;
						objectButter.transform.rotation = butterPlaceTarget.rotation;
					}
				}
				butterTableDistance = Vector3.Distance(objectButter.transform.position, butterPlaceTarget.position);
				if (butterTableDistance < 1f)
				{
					Quaternion a = Quaternion.identity;
					if (objectButter.rb.constraints == RigidbodyConstraints.None)
					{
						a = objectButter.transform.rotation;
					}
					objectButter.rb.constraints = RigidbodyConstraints.FreezeAll;
					objectButter.transform.rotation = Quaternion.Lerp(a, butterPlaceTarget.transform.rotation, 1f - butterTableDistance);
					objectButter.transform.position = Vector3.Lerp(objectButter.transform.position, butterPlaceTarget.transform.position, 2.5f * Time.deltaTime);
				}
				yield return null;
			}
			objectButter.ForceDropByPlayer();
			objectButter.ungrabbable = true;
			butterDialogueSource.PlayClip(tweakables.dialogue.aSlipSomethingMoreComfortable);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.5f, this);
			butterPlaceTarget.gameObject.SetActive(value: true);
			objectButter.GetComponent<ButterTalk>().scaling = false;
			butterPlaceTarget.transform.localScale = Vector3.zero;
			while (butterPlaceTarget.transform.localScale.x < 9.4f)
			{
				objectButter.transform.localScale = Vector3.Lerp(objectButter.transform.localScale, Vector3.one * 7.5f, 1.5f * Time.deltaTime);
				butterPlaceTarget.transform.localScale = Vector3.Lerp(butterPlaceTarget.transform.localScale, Vector3.one * 9.6f, 1f * Time.deltaTime);
				yield return null;
			}
			kitchenState = KitchenStates.ToasterTutorial;
		}
		SafeDebug.Log("on to the next");
		objectButter.ungrabbable = true;
		butterPlaceTarget.gameObject.SetActive(value: true);
		objectButter.rb.isKinematic = true;
		objectButter.GetComponent<ButterTalk>().particles = true;
		objectButter.GetComponent<ButterTalk>().particleScalar = 0.4f;
		objectButter.transform.localScale = Vector3.one * 7.5f;
		butterPlaceTarget.transform.localScale = Vector3.one * 9.4f;
		objectButter.GetComponent<ButterTalk>().scaling = true;
		objectButter.transform.position = butterPlaceTarget.position;
		objectButter.transform.rotation = butterPlaceTarget.rotation;
		objectButter.sticky = true;
		while (butterDialogueSource.isPlaying)
		{
			yield return null;
		}
		if (kitchenState == KitchenStates.ToasterTutorial)
		{
			butterDialogueSource.PlayClip(tweakables.dialogue.aChangeRadio);
			radioMusicSource.clip = radioSmoothMusic;
			radioMusicSource.Play();
			radioPlaying = true;
			radioSubtitles.SetActive(value: true);
			while (butterDialogueSource.isPlaying)
			{
				yield return null;
			}
			UI.SetActive(value: true);
			float userHoldingShitTimer = 0f;
			bool interrupted = false;
			for (int i = 0; i < tweakables.dialogue.bIndulgeMeTutorialSoliloquy.Length; i++)
			{
				butterDialogueSource.PlayClip(tweakables.dialogue.bIndulgeMeTutorialSoliloquy[i]);
				while (butterDialogueSource.isPlaying)
				{
					if (!interrupted && ToolManager.PlayerGrabbingSomething())
					{
						userHoldingShitTimer += Time.deltaTime;
					}
					if (!interrupted && userHoldingShitTimer > tweakables.chatiseDelayAfterUserIsGrabbingShit)
					{
						interrupted = true;
						ToolManager.DropEverything();
						ToolManager.AllManipulators.UserCanGrab = false;
						butterDialogueSource.InterruptWithClip(tweakables.dialogue.bPayAttentionInterruption);
					}
					yield return null;
				}
				yield return null;
			}
			tutorialToaster.gameObject.SetActive(value: true);
			ToolManager.AllManipulators.UserCanGrab = true;
			objectButter.ungrabbable = true;
			objectButter.objReactions = true;
			butterDialogueSource.PlayClip(tweakables.dialogue.bClickTheToaster);
			tutorialToaster.StartCookingToast();
			while (!tutorialToaster.tutorialDone)
			{
				yield return null;
			}
			toastPendingJudgment = false;
			bool tutorialFinished = false;
			bool toastReadyHelper = false;
			bool toastReadyIndicated = false;
			int toastReadyIndicatorIndex = 0;
			float anotherToastTimer = 0f;
			lastToastEvaluation = ObjectToaster.Toast.TBD;
			butterDialogueSource.PlayClip(tweakables.dialogue.bPickUpTheToast);
			while (butterDialogueSource.isPlaying)
			{
				yield return null;
			}
			while (!tutorialFinished)
			{
				if (toastReadyHelper && !toastReadyIndicated && tutorialToaster.toast == ObjectToaster.Toast.Toasted && tutorialToaster.startedToasting)
				{
					butterDialogueSource.PlayClip(tweakables.dialogue.bToastReady[toastReadyIndicatorIndex]);
					toastReadyIndicatorIndex++;
					if (toastReadyIndicatorIndex >= tweakables.dialogue.bToastReady.Length)
					{
						toastReadyIndicatorIndex = 0;
					}
					toastReadyIndicated = true;
				}
				if (toastPendingJudgment)
				{
					switch (lastToastEvaluation)
					{
					case ObjectToaster.Toast.Uncooked:
						butterDialogueSource.PlayClip(tweakables.dialogue.bFeedbackUnderdone.FireFeedbackClip());
						break;
					case ObjectToaster.Toast.Toasted:
						butterDialogueSource.PlayClip(tweakables.dialogue.bFeedbackPerfect.FireFeedbackClip());
						tutorialFinished = true;
						break;
					case ObjectToaster.Toast.Burning:
						butterDialogueSource.PlayClip(tweakables.dialogue.bFeedbackBurnt.FireFeedbackClip());
						break;
					case ObjectToaster.Toast.SOS:
						butterDialogueSource.PlayClip(tweakables.dialogue.bFeedbackBurnt.FireFeedbackClip());
						break;
					}
					toastPendingJudgment = false;
					anotherToastTimer = 0f;
					while (butterDialogueSource.isPlaying)
					{
						yield return null;
					}
					tutorialToaster.Reload(generateNewToast: true);
					toastReadyHelper = true;
					toastReadyIndicated = false;
				}
				else
				{
					anotherToastTimer += Time.deltaTime;
					if (anotherToastTimer > 30f)
					{
						tutorialToaster.Reload(generateNewToast: true);
						anotherToastTimer = 0f;
						toastReadyIndicated = false;
						toastReadyHelper = true;
					}
				}
				yield return null;
			}
			butterDialogueSource.PlayClip(tweakables.dialogue.bButWhyHaveOneToaster);
			while (butterDialogueSource.isPlaying)
			{
				yield return null;
			}
			kitchenState = KitchenStates.ToasterSpawn;
		}
		butterDialogueSource.PlayClip(tweakables.dialogue.cHowRich);
		if (kitchenState == KitchenStates.ToasterSpawn)
		{
			tutorialToaster.tutorialToaster = false;
			float toasterSpawnInterval = tweakables.toasterPopInTime / (float)Mathf.Max(1, kitchenToasters.Length - 1);
			for (int j = 0; j < kitchenToasters.Length; j++)
			{
				kitchenToasters[j].gameObject.SetActive(value: true);
				float spawnT = (float)j / (float)Mathf.Max(1, kitchenToasters.Length - 1);
				SelfDestructingAudioSource popSfxObj = new GameObject().AddComponent<SelfDestructingAudioSource>();
				popSfxObj.PlayPitchBendedClip(toasterPopInClip, kitchenToasters[j].transform.position, 0.2f, Mathf.Lerp(tweakables.toasterPopInMinPitch, tweakables.toasterPopInMaxPitch, spawnT));
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(toasterSpawnInterval, this);
			}
			toastPendingJudgment = false;
			for (int k = 0; k < tweakables.dialogue.cMainToastSoliloquy.Length; k++)
			{
				butterDialogueSource.PlayClip(tweakables.dialogue.cMainToastSoliloquy[k]);
				if (k == 6)
				{
					objectButter.absorbingToast = false;
				}
				if (k == 11)
				{
					objectButter.GetComponent<ButterTalk>().particleScalar = 0.7f;
				}
				if (k > 13)
				{
					radioMusicSource.pitch *= 1.1f;
					radioMusicSource.volume *= 1.1f;
				}
				if (k > 14)
				{
					int popperCount = k - 14;
					for (int l = 0; l < popperCount; l++)
					{
						int count = 0;
						ObjectToaster pop;
						do
						{
							count++;
							pop = kitchenToasters[Random.Range(0, tweakables.dialogue.cMainToastSoliloquy.Length - 1)];
						}
						while (count < kitchenToasters.Length && (pop.finalToast || pop.fireToaster) && pop.startedToasting);
						pop.EjectToast(popLastToast: true);
						yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.2f, this);
					}
				}
				if (k + 2 == tweakables.dialogue.cMainToastSoliloquy.Length)
				{
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(2.5f, this);
					for (int m = 0; m < kitchenToasters.Length; m++)
					{
						if (!kitchenToasters[m].finalToast && !kitchenToasters[m].fireToaster && kitchenToasters[m].startedToasting)
						{
							kitchenToasters[m].EjectToast(popLastToast: true);
							yield return GlobalCoroutineContainer.SafeWaitForSeconds(Random.Range(0.03f, 0.125f), this);
						}
					}
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.2f, this);
					radioMusicSource.Stop();
					radioSubtitles.SetActive(value: false);
					radioPlaying = false;
					radioMusicSource.PlayOneShot(staticHit);
					int freezeToastStart = Mathf.Max(0, KitchenToastManager.GetKitchenToast().Count - 30);
					for (int n = freezeToastStart; n < KitchenToastManager.GetKitchenToast().Count; n++)
					{
						Rigidbody component = KitchenToastManager.GetKitchenToast()[n].GetComponent<Rigidbody>();
						if ((bool)component)
						{
							component.useGravity = false;
							component.velocity *= 0.03f;
							component.angularVelocity *= 0.06f;
						}
					}
				}
				while (butterDialogueSource.isPlaying)
				{
					if (toastPendingJudgment && tweakables.dialogue.cMainToastSoliloquy[k].mInterruptability != Interruptability.uninterruptable)
					{
						butterDialogueSource.InterruptWithClip(getToastFeedbackLine(lastToastEvaluation), enqueueIfCantInterrupt: true);
						toastPendingJudgment = false;
					}
					yield return null;
				}
			}
			kitchenState = KitchenStates.ToasterStop;
		}
		if (kitchenState == KitchenStates.ToasterStop)
		{
			objectButter.GetComponentInChildren<ButterTalk>().particleScalar = 0.4f;
			objectButter.objReactions = false;
			butterDialogueSource.PlayClip(tweakables.dialogue.dJustStop);
			tutorialToaster.Deactivate();
			for (int num = 0; num < kitchenToasters.Length; num++)
			{
				if (!kitchenToasters[num].fireToaster)
				{
					kitchenToasters[num].Deactivate();
				}
			}
			while (butterDialogueSource.isPlaying)
			{
				yield return null;
			}
			fireToaster.ActivateFireToaster();
			butterDialogueSource.PlayClip(tweakables.dialogue.dHighHopes);
			while (butterDialogueSource.isPlaying)
			{
				yield return null;
			}
			kitchenState = KitchenStates.SpawnVacuum;
		}
		int startingButterToastCount = KitchenToastManager.GetButterToast().Count;
		if (kitchenState == KitchenStates.SpawnVacuum)
		{
			for (int num2 = 0; num2 < invincibles.Length; num2++)
			{
				if (invincibles[num2] != null)
				{
					invincibles[num2].unconsumable = true;
				}
			}
			butterEatTrigger.SetActive(value: true);
			vacuum.gameObject.SetActive(value: true);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.3f, this);
			busterSpawn.gameObject.SetActive(value: true);
			SpawnFX.Play();
			butterDialogueSource.PlayClip(tweakables.dialogue.dVacuumUpThisMess);
			while (butterDialogueSource.isPlaying)
			{
				if (vacuum.isGrabbed)
				{
					busterSpawn.Disable();
				}
				if ((bool)busterJoint)
				{
					busterJoint.breakForce = 15f;
				}
				yield return null;
			}
			float vacuumTimer = 0f;
			while (!vacuum.isGrabbed)
			{
				vacuumTimer += Time.deltaTime;
				if (vacuumTimer > 10f)
				{
					butterDialogueSource.PlayClip(tweakables.dialogue.dHurryVacuumMeUp);
					while (butterDialogueSource.isPlaying)
					{
						if (vacuum.isGrabbed)
						{
							busterSpawn.Disable();
						}
						if ((bool)busterJoint)
						{
							busterJoint.breakForce = 15f;
						}
						yield return null;
					}
					break;
				}
				yield return null;
			}
			busterSpawn.Disable();
			if ((bool)busterJoint)
			{
				busterJoint.breakForce = 15f;
			}
			for (int num3 = 0; num3 < tweakables.dialogue.dIneptMonologue.Length; num3++)
			{
				butterDialogueSource.PlayClip(tweakables.dialogue.dIneptMonologue[num3]);
				while (butterDialogueSource.isPlaying)
				{
					yield return null;
				}
				yield return null;
			}
			float butterCleanPercentage = 1f - getRemainingButterToastPercentage(startingButterToastCount);
			EagerHeadsetManager.SetCanRemoveHeadset(canRemove: false);
			EagerScenes.instance.DestroyScene(TCScenes.S.VVR_LVL_LOGIN);
			if (butterCleanPercentage > tweakables.cutoffButterToastVacuumPercentage)
			{
				butterDialogueSource.PlayClip(tweakables.dialogue.dActuallyThatDoesntFeelSoBad);
				while (butterDialogueSource.isPlaying)
				{
					yield return null;
				}
			}
			else
			{
				butterDialogueSource.PlayClip(tweakables.dialogue.dCleanMeUpAndYouCanGo);
				while (butterDialogueSource.isPlaying)
				{
					yield return null;
				}
				vacuumTimer = 0f;
				while (!objectButter.GetComponent<VVRConsumable>().isBeingAffectedByVacuum && vacuumTimer < 15f)
				{
					vacuumTimer += Time.deltaTime;
					yield return null;
				}
			}
			butterDialogueSource.PlayClip(tweakables.dialogue.dRightInTheGlobules);
			while (butterDialogueSource.isPlaying)
			{
				yield return null;
			}
			butterDialogueSource.PlayClip(tweakables.dialogue.dOneLodgedDeep);
			while (butterDialogueSource.isPlaying)
			{
				yield return null;
			}
			kitchenState = KitchenStates.VacuumDestruction;
		}
		vacuum.gameObject.SetActive(value: true);
		vacuum.SetHighPower(high: true);
		if (kitchenState == KitchenStates.VacuumDestruction)
		{
			while (butterEatTrigger != null)
			{
				yield return null;
			}
			tableConsumable.unconsumable = false;
			butterDialogueSource.PlayClip(tweakables.dialogue.dScream);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.4f, this);
			objectButter.GetComponent<VVRConsumable>().unconsumable = false;
			objectButter.mouth.unconsumable = false;
			objectButter.GetComponent<VVRConsumable>().OneShotTransfer(vacuum.vacConsumer, -15f);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.4f, this);
			kitchenState = KitchenStates.ChangeScenes;
		}
		for (int num4 = 0; num4 < invincibles.Length; num4++)
		{
			if (invincibles[num4] != null)
			{
				invincibles[num4].unconsumable = false;
			}
		}
		if (kitchenState != KitchenStates.ChangeScenes)
		{
			yield break;
		}
		while (!(EagerScenes.instance.ActiveScene != null) || !EagerScenes.SceneIsLoaded(EagerScenes.BACKSTAGE_LEVEL))
		{
			yield return null;
		}
		for (int num5 = 0; num5 < loadOnChange.Length; num5++)
		{
			loadOnChange[num5].SetActive(value: true);
		}
		UI.SetActive(value: false);
		lamp.unconsumable = false;
		kitchenEnviroProperties = GetComponentInChildren<EnvironmentProperties>();
		EagerScene backstage = EagerScenes.SceneToEagerScene(EagerScenes.BACKSTAGE_LEVEL);
		bool kitchenBackstageSwapper = false;
		while (lightFlickerCount > 0)
		{
			lightFlickerCount--;
			kitchenBackstageSwapper = !kitchenBackstageSwapper;
			if (kitchenBackstageSwapper)
			{
				kitchenEnviroProperties.Activate();
				if ((bool)kitchenMaterialProperties)
				{
					kitchenMaterialProperties.PushMaterials();
				}
			}
			else
			{
				backstage.mEnvironmentProperties.Activate();
				if ((bool)kitchenMaterialPropertiesDark)
				{
					kitchenMaterialPropertiesDark.PushMaterials();
				}
			}
			yield return null;
		}
		matUpdate.SetActive(value: true);
		if (deleteOnBackstage != null && deleteOnBackstage.Count > 0)
		{
			for (int num6 = 0; num6 < deleteOnBackstage.Count; num6++)
			{
				if (deleteOnBackstage[num6] != null)
				{
					deleteOnBackstage[num6].SetActive(value: false);
				}
			}
		}
		backstage.GetComponent<Backstage>().mainSwarmController.AddToConsumableList(toConstruct: false, boidCleanup, "KitchenCleanup");
		SafeDebug.Log("MERGE SCENE");
		backstage.mEnvironmentProperties.Activate();
		EagerScenes.instance.MergeScene(EagerScenes.BACKSTAGE_LEVEL);
		sceneDeleteSounds.SetActive(value: true);
		roomWalls.SetActive(value: false);
		enabled = false;
	}

	public void NewToastAttachedToButter(ObjectToast toastIn)
	{
		lastToastEvaluation = toastIn.toastState;
		toastPendingJudgment = true;
		if (lastToastEvaluation == ObjectToaster.Toast.Uncooked)
		{
			toastCount[0]++;
		}
		else if (lastToastEvaluation == ObjectToaster.Toast.Toasted)
		{
			toastCount[1]++;
		}
		else if (lastToastEvaluation == ObjectToaster.Toast.Burning)
		{
			toastCount[2]++;
		}
	}

	private AudioClip getToastFeedbackLine(ObjectToaster.Toast toastIn)
	{
		return toastIn switch
		{
			ObjectToaster.Toast.Uncooked => tweakables.dialogue.cFeedbackUnderdone.FireFeedbackClip(), 
			ObjectToaster.Toast.Toasted => tweakables.dialogue.cFeedbackPerfect.FireFeedbackClip(), 
			ObjectToaster.Toast.Burning => tweakables.dialogue.cFeedbackBurnt.FireFeedbackClip(), 
			ObjectToaster.Toast.SOS => tweakables.dialogue.cFeedbackBurnt.FireFeedbackClip(), 
			_ => null, 
		};
	}

	private float getRemainingButterToastPercentage(int originalButterToastCount)
	{
		if (originalButterToastCount <= 0)
		{
			return 0f;
		}
		return (float)KitchenToastManager.GetButterToast().Count / (float)originalButterToastCount;
	}

	private float getRemainingKitchenToastPercentage(int originalKitchenToastCount)
	{
		if (originalKitchenToastCount <= 0)
		{
			return 0f;
		}
		return (float)KitchenToastManager.GetKitchenToast().Count / (float)originalKitchenToastCount;
	}

	private void OnEnable()
	{
		if (radioPlaying)
		{
			radioMusicSource.Play();
		}
	}
}
