using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using UnityEngine;

public class JobGarden : SceneBehaviorJob
{
	public enum GardenStates
	{
		Seek,
		Watering,
		Potting,
		Placing
	}

	public static JobGarden instance;

	[Header("References:")]
	[SerializeField]
	private GardenScriptableObject gardenScriptable;

	[SerializeField]
	private GardenStates gardenState;

	[SerializeField]
	private VVRDialogueSource sprinklesDialogue;

	[SerializeField]
	private ObjectPinwheel sprinkles;

	[SerializeField]
	private GameObject sprinklesGO;

	[SerializeField]
	private ObjectTrigger greenhouseTrigger;

	[SerializeField]
	private ObjectTrigger wateringTrigger;

	[SerializeField]
	private ObjectTrigger sprinkleRestingTrigger;

	[SerializeField]
	private ObjectTrigger placingTrigger;

	[SerializeField]
	private ObjectTrigger failTrigger;

	[SerializeField]
	private ObjectTrigger placingTriggerShed;

	[SerializeField]
	private ObjectTrigger placingTriggerTableBack;

	[SerializeField]
	private ObjectTrigger placingTriggerTableLeft;

	[SerializeField]
	private SubtitleRenderer subtitles;

	[SerializeField]
	private ParticleSystem bucketParticles;

	[SerializeField]
	private ParticleSystem flowerParticles;

	[SerializeField]
	private GvrAudioSource flowerSong;

	[SerializeField]
	private GvrAudioSource gardenSong;

	[Header("Seeking References:")]
	[SerializeField]
	private GvrAudioSource sprinklesClang;

	[SerializeField]
	private ObjectBase[] seekingWrongObjects;

	private int seekIndex;

	[HideInInspector]
	public bool hasHoveredOver;

	[SerializeField]
	public GameObject bucket;

	[SerializeField]
	private Transform outsidePosition;

	[SerializeField]
	private List<ObjectBase> specialObjects;

	[SerializeField]
	private List<ObjectBase> specialObjectsPlants;

	[SerializeField]
	private List<ObjectBase> specialObjectsHanging;

	[SerializeField]
	private List<ObjectBase> grabbedObjects;

	[SerializeField]
	private GameObject sprinklesRustlingEffects;

	[SerializeField]
	private ObjectBase shedDoor;

	private bool playedShedLine;

	[Header("Watering References:")]
	[SerializeField]
	private ObjectBase wateringCan;

	[SerializeField]
	private Transform sprinklesRestingPosition;

	[SerializeField]
	private GameObject specialFlowerChild;

	[HideInInspector]
	[SerializeField]
	private ObjectPottable specialFlower;

	[SerializeField]
	private List<ObjectPlant> flowers;

	[SerializeField]
	private List<ObjectPlant> carrots;

	[SerializeField]
	private List<Rigidbody> tires;

	[SerializeField]
	private GameObject sprinkleWaterTrigger;

	[SerializeField]
	private List<GameObject> additionalSprinkleWaterTriggers;

	[SerializeField]
	private List<Rigidbody> specialFlowerPetals;

	[SerializeField]
	private List<GameObject> normalFlowerPetals;

	private float petalDestroyForce = 200f;

	private int currentIfWateredIndex;

	private bool waterReactionQueued;

	private float eraseQueueTime = 6f;

	private float eraseQueueCount;

	private float remindPottingTime = 8f;

	private float remindPottingCountdown;

	[Header("Potting References:")]
	[SerializeField]
	private List<Pot> breakablePots;

	[SerializeField]
	private PickyPot unbreakablePot;

	[SerializeField]
	private SubtitleRenderer shatterSubtitle;

	private bool potHasBroken;

	private float delayTimer;

	private bool delayTrigger;

	private float moveLerp;

	private SafeCoroutine moveSprinkleCoroutine;

	private Vector3 sprinkleStart;

	private Vector3 sprinkleEnd;

	private bool sprinklesIsMoving;

	private void Awake()
	{
		if (instance != null)
		{
			throw new UnityException("Only one instance of JobGarden in a scene is allowed.");
		}
		instance = this;
	}

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		wateringCan.gameObject.SetActive(value: false);
		unbreakablePot.gameObject.SetActive(value: false);
		failTrigger.gameObject.SetActive(value: false);
		GlobalCoroutineContainer.AddThread(GardenFirstJobCoroutine(), this);
		bucketParticles.Stop();
		flowerParticles.Stop();
	}

	private void OnEnable()
	{
	}

	private IEnumerator GardenFirstJobCoroutine()
	{
		yield return null;
		sprinklesRustlingEffects.SetActive(value: false);
		if (gardenState == GardenStates.Seek)
		{
			SafeDebug.Log("<Color=green>Seeking</color>");
			sprinklesGO.transform.localScale = Vector3.zero;
			sprinkles.rb.isKinematic = true;
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
			subtitles.SetForceToHead(s: true);
			sprinklesDialogue.PlayClip(gardenScriptable.dialogue.sGigglingStart);
			sprinklesDialogue.PlayClip(gardenScriptable.dialogue.sLetPlayHideNSeek);
			sprinklesDialogue.PlayClip(gardenScriptable.dialogue.sCmonTryAndFindMe);
			int currentHint = 0;
			while (!greenhouseTrigger.playerInside)
			{
				if (shedDoor.isGrabbed && !playedShedLine)
				{
					sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.sSecretSpotPreGreenhouse);
					delayTrigger = true;
					playedShedLine = true;
				}
				if (!sprinklesDialogue.isPlaying)
				{
					delayTimer += Time.deltaTime;
				}
				if (!delayTrigger && delayTimer > 15f)
				{
					switch (currentHint)
					{
					case 0:
						sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.sImInTheGreenhouse);
						delayTimer = 8f;
						currentHint++;
						break;
					case 1:
						sprinklesDialogue.PlayClip(gardenScriptable.dialogue.sImHidingRightNow);
						delayTimer = 8f;
						currentHint++;
						break;
					case 2:
						sprinklesDialogue.PlayClip(gardenScriptable.dialogue.sCmonTryAndFindMe);
						delayTrigger = true;
						break;
					default:
						sprinklesDialogue.PlayClip(gardenScriptable.dialogue.sCmonTryAndFindMe);
						delayTrigger = true;
						break;
					}
				}
				yield return null;
			}
			SafeDebug.Log("<Color=green>Player inside greenhouse</color>");
			delayTimer = 0f;
			while (sprinklesDialogue.isPlaying)
			{
				yield return null;
			}
			sprinklesDialogue.PlayClip(gardenScriptable.dialogue.sBetYouCantFindMe);
			StartSprinkleMove(seekingWrongObjects[seekIndex].transform.position, 0.75f);
			while (sprinklesIsMoving)
			{
				yield return null;
			}
			sprinklesDialogue.PlayClip(gardenScriptable.dialogue.sGuess);
			sprinklesDialogue.PlayClip(gardenScriptable.dialogue.sGuessPickMeRightUp);
			sprinklesDialogue.PlayClip(gardenScriptable.dialogue.sGuessGuessGuess);
			bool grabbedPlant = false;
			bool grabbedHangingPlant = false;
			while (seekIndex < seekingWrongObjects.Length)
			{
				sprinklesGO.transform.LookAt(EagerMaster.instance.mainCamera.transform);
				for (int i = 0; i < ToolManager.ActiveToolGroups.Count; i++)
				{
					if (!ToolManager.ActiveToolGroups[i].Manipulator.isGrabbing() || grabbedObjects.Contains(ToolManager.ActiveToolGroups[i].Manipulator.HeldObjectBase))
					{
						continue;
					}
					if (shedDoor.isGrabbed)
					{
						if (!playedShedLine)
						{
							sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.sSecretSpotPostGreenhouse);
							playedShedLine = true;
						}
						while (sprinklesDialogue.isPlaying)
						{
							yield return null;
						}
						continue;
					}
					grabbedObjects.Add(ToolManager.ActiveToolGroups[i].Manipulator.HeldObjectBase);
					int index = specialObjects.IndexOf(ToolManager.ActiveToolGroups[i].Manipulator.HeldObjectBase);
					if (index == -1)
					{
						if (specialObjectsPlants.Contains(ToolManager.ActiveToolGroups[i].Manipulator.HeldObjectBase) && !grabbedPlant)
						{
							sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.sPlant);
							grabbedPlant = true;
						}
						else if (specialObjectsHanging.Contains(ToolManager.ActiveToolGroups[i].Manipulator.HeldObjectBase) && !grabbedHangingPlant)
						{
							sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.sHangingPlant);
							grabbedHangingPlant = true;
						}
						else
						{
							sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.sGrabAttemptLines[seekIndex]);
						}
					}
					else
					{
						sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.sSpecialObjectLines[index]);
					}
					while (sprinklesDialogue.isPlaying)
					{
						yield return null;
					}
					seekIndex++;
					if (seekIndex == seekingWrongObjects.Length)
					{
						break;
					}
					StartSprinkleMove(seekingWrongObjects[seekIndex].transform.position, 0.75f);
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.25f, this);
					sprinklesClang.transform.position = EagerMaster.instance.mainCamera.transform.position + Random.onUnitSphere;
					sprinklesClang.clip = gardenScriptable.dialogue.clangSounds[seekIndex];
					sprinklesClang.Play();
					while (sprinklesIsMoving)
					{
						yield return null;
					}
				}
				if (!sprinklesDialogue.isPlaying && seekIndex < seekingWrongObjects.Length)
				{
					sprinklesDialogue.PlayClip(gardenScriptable.dialogue.sHidingAmbientGiggle);
				}
				yield return null;
			}
			while (sprinklesDialogue.isPlaying)
			{
				yield return null;
			}
			StartSprinkleMove(bucket.transform.position - Vector3.up * 0.5f, 0.25f);
			while (sprinklesIsMoving)
			{
				yield return null;
			}
			subtitles.SetCharacter(sprinklesGO.transform);
			subtitles.SetForceToHead(s: false);
			sprinklesGO.transform.localScale = Vector3.one;
			sprinklesGO.transform.LookAt(sprinklesGO.transform.position + Vector3.right);
			StartSprinkleMove(bucket.transform.position + Vector3.up * 0.2f, 4f);
			while (sprinklesIsMoving)
			{
				yield return null;
			}
			bucketParticles.Play();
			delayTimer = 0f;
			delayTrigger = false;
			while (!hasHoveredOver)
			{
				delayTimer += Time.deltaTime;
				if (!delayTrigger)
				{
					for (int j = 0; j < ToolManager.ActiveToolGroups.Count; j++)
					{
						if (!ToolManager.ActiveToolGroups[j].Manipulator.isGrabbing() || grabbedObjects.Contains(ToolManager.ActiveToolGroups[j].Manipulator.HeldObjectBase))
						{
							continue;
						}
						if (shedDoor.isGrabbed)
						{
							if (!playedShedLine)
							{
								sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.sSecretSpotPostGreenhouse);
								playedShedLine = true;
							}
							while (sprinklesDialogue.isPlaying)
							{
								yield return null;
							}
							continue;
						}
						grabbedObjects.Add(ToolManager.ActiveToolGroups[j].Manipulator.HeldObjectBase);
						int num = specialObjects.IndexOf(ToolManager.ActiveToolGroups[j].Manipulator.HeldObjectBase);
						if (num == -1)
						{
							if (specialObjectsPlants.Contains(ToolManager.ActiveToolGroups[j].Manipulator.HeldObjectBase) && !grabbedPlant)
							{
								sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.sPlant);
								grabbedPlant = true;
								continue;
							}
							if (specialObjectsHanging.Contains(ToolManager.ActiveToolGroups[j].Manipulator.HeldObjectBase) && !grabbedHangingPlant)
							{
								sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.sHangingPlant);
								grabbedHangingPlant = true;
								continue;
							}
							seekIndex++;
							if (seekIndex >= seekingWrongObjects.Length)
							{
								seekIndex = 0;
							}
							sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.sGrabAttemptLines[seekIndex]);
						}
						else
						{
							sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.sSpecialObjectLines[num]);
						}
					}
					if (delayTimer > 5f)
					{
						sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.sImInTheBucket);
						delayTrigger = true;
					}
				}
				if (!sprinklesDialogue.isPlaying)
				{
					sprinklesDialogue.PlayClip(gardenScriptable.dialogue.sHidingAmbientGiggle);
				}
				yield return null;
			}
			bucketParticles.Stop();
			sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.sHoverCantCatchMe);
			StartSprinkleMove(bucket.transform.position - Vector3.up * 0.6f, 4f);
			while (sprinklesIsMoving)
			{
				yield return null;
			}
			sprinklesGO.transform.localScale = Vector3.zero;
			StartSprinkleMove(outsidePosition.position - Vector3.up, 0.25f);
			while (sprinklesIsMoving)
			{
				yield return null;
			}
			sprinklesGO.transform.localScale = Vector3.one;
			StartSprinkleMove(outsidePosition.position, 4f);
			while (sprinklesIsMoving)
			{
				yield return null;
			}
			while (sprinklesDialogue.isPlaying)
			{
				yield return null;
			}
			sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.wImOutsideNow);
			sprinklesDialogue.PlayClip(gardenScriptable.dialogue.wComeOutsideReminder);
			gardenState = GardenStates.Watering;
		}
		sprinklesGO.transform.position = outsidePosition.position;
		sprinkles.rb.isKinematic = true;
		if (gardenState == GardenStates.Watering)
		{
			SafeDebug.Log("<Color=green>Watering</color>");
			SafeDebug.Log("<Color=green>Player next to planters</color>");
			delayTimer = 0f;
			delayTrigger = false;
			int currentReminder = 0;
			while (!ToolManager.PlayerGrabbingObjectBase(sprinkles))
			{
				if (shedDoor.isGrabbed && !playedShedLine)
				{
					sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.sSecretSpotPostGreenhouse);
				}
				if (!sprinklesDialogue.isPlaying)
				{
					delayTimer += Time.deltaTime;
				}
				if (!delayTrigger && delayTimer > 10f)
				{
					delayTrigger = true;
					sprinklesDialogue.PlayClip(gardenScriptable.dialogue.wPlayWithMe);
				}
				yield return null;
			}
			sprinkles.rb.isKinematic = false;
			sprinkles.undroppable = true;
			subtitles.SetForceToHead(s: true);
			for (int k = 0; k < tires.Count; k++)
			{
				tires[k].isKinematic = false;
			}
			sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.wPleaseWhisperVeryHard);
			for (int l = 0; l < gardenScriptable.dialogue.wBreeze.Length; l++)
			{
				while (sprinkles.rbVelMag < 5f)
				{
					yield return null;
				}
				sprinklesDialogue.PlayClip(gardenScriptable.dialogue.wBreeze[l]);
				while (sprinklesDialogue.isPlaying)
				{
					yield return null;
				}
			}
			sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.wNowThenLetsPlayGarden);
			delayTimer = 0f;
			delayTrigger = false;
			float failsafeTimer = 40f;
			while (!sprinkleRestingTrigger.objectInside)
			{
				if (!sprinklesDialogue.isPlaying)
				{
					delayTimer += Time.deltaTime;
				}
				if (!delayTrigger && delayTimer > 6f)
				{
					if (currentReminder == 1)
					{
						delayTrigger = true;
						sprinklesDialogue.PlayClip(gardenScriptable.dialogue.wHurrySoMuchDependsOnIt);
					}
					else
					{
						delayTimer = 0f;
						sprinklesDialogue.PlayClip(gardenScriptable.dialogue.wPutMeInthePlanterReminder);
						currentReminder++;
					}
				}
				failsafeTimer -= Time.deltaTime;
				if (failsafeTimer <= 0f)
				{
					sprinkles.rb.isKinematic = true;
					Vector3 startCorrectionPosition = bucket.transform.position;
					startCorrectionPosition.x = sprinkles.transform.position.x;
					startCorrectionPosition.z = sprinkles.transform.position.z;
					float sqrDist = Vector3.SqrMagnitude(sprinklesRestingPosition.position - startCorrectionPosition);
					if (sqrDist > 3f)
					{
						StartSprinkleMove(startCorrectionPosition - Vector3.up * 0.6f, 4f);
						while (sprinklesIsMoving)
						{
							yield return null;
						}
						sprinklesGO.transform.localScale = Vector3.zero;
						StartSprinkleMove(sprinklesRestingPosition.position - Vector3.up, 0.25f);
						while (sprinklesIsMoving)
						{
							yield return null;
						}
						sprinklesGO.transform.localScale = Vector3.one;
						StartSprinkleMove(sprinklesRestingPosition.position, 4f);
						while (sprinklesIsMoving)
						{
							yield return null;
						}
					}
					else
					{
						StartSprinkleMove(sprinklesRestingPosition.position, 1f);
						while (sprinklesIsMoving)
						{
							yield return null;
						}
					}
				}
				yield return null;
			}
			sprinkles.undroppable = false;
			sprinkles.ForceDropByPlayer();
			subtitles.SetForceToHead(s: false);
			subtitles.yOffsetChar = 0.5f;
			StartSprinkleMove(sprinklesRestingPosition.position, 2f);
			sprinkles.rb.isKinematic = true;
			sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.wDownToBusinessGardening);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
			wateringCan.gameObject.SetActive(value: true);
			delayTimer = 0f;
			delayTrigger = false;
			while (!ToolManager.PlayerGrabbingObjectBase(wateringCan))
			{
				if (ToolManager.PlayerGrabbingObjectBase(sprinkles, dropIfGrabbing: true) && sprinklesDialogue.InterruptWithClip(gardenScriptable.dialogue.wIfGrabbed) != null)
				{
					sprinkles.ungrabbable = true;
				}
				if (!delayTrigger && !sprinklesDialogue.isPlaying)
				{
					delayTimer += Time.deltaTime;
					if (delayTimer > 8f)
					{
						delayTrigger = true;
						sprinklesDialogue.PlayClip(gardenScriptable.dialogue.wPickUpTheWateringCanReminder);
					}
				}
				yield return null;
			}
			sprinklesDialogue.PlayClip(gardenScriptable.dialogue.wGardenIsMyTreasure);
			delayTimer = 0f;
			delayTrigger = false;
			bool sprinkleLine = false;
			bool carrotLine = false;
			int currentWaterReminder = 0;
			int waterReminders = 6;
			float remindTime = 0f;
			while (flowers.Count > 1 && !delayTrigger)
			{
				delayTimer += Time.deltaTime;
				for (int m = 0; m < flowers.Count; m++)
				{
					if (flowers[m].growthCount > 0f)
					{
						flowers.RemoveAt(m);
					}
				}
				if (!carrotLine)
				{
					for (int n = 0; n < carrots.Count; n++)
					{
						if (!carrots[n].rb.isKinematic)
						{
							sprinklesDialogue.InterruptWithClip(gardenScriptable.dialogue.wDoTheCarrots);
							carrotLine = true;
						}
					}
				}
				if (ToolManager.PlayerGrabbingObjectBase(sprinkles, dropIfGrabbing: true) && sprinklesDialogue.InterruptWithClip(gardenScriptable.dialogue.wIfGrabbed) != null)
				{
					sprinkles.ungrabbable = true;
				}
				if (!sprinkleLine)
				{
					if (currentIfWateredIndex == 0 && sprinkleWaterTrigger.GetComponent<ObjectPlant>() == null)
					{
						sprinklesDialogue.InterruptWithClip(gardenScriptable.dialogue.wIfWatered[currentIfWateredIndex]);
						waterReactionQueued = true;
						eraseQueueCount = eraseQueueTime;
						currentIfWateredIndex++;
					}
					else if (!waterReactionQueued && currentIfWateredIndex >= 1 && additionalSprinkleWaterTriggers[currentIfWateredIndex - 1].GetComponent<ObjectPlant>() == null)
					{
						sprinklesDialogue.InterruptWithClip(gardenScriptable.dialogue.wIfWatered[currentIfWateredIndex]);
						waterReactionQueued = true;
						eraseQueueCount = eraseQueueTime;
						currentIfWateredIndex++;
						if (currentIfWateredIndex >= 3)
						{
							sprinkleLine = true;
						}
					}
					if (waterReactionQueued)
					{
						eraseQueueCount -= Time.deltaTime;
						if (eraseQueueCount <= 0f || !sprinklesDialogue.isPlaying)
						{
							waterReactionQueued = false;
							additionalSprinkleWaterTriggers[currentIfWateredIndex - 1].SetActive(value: true);
						}
					}
				}
				if (delayTimer > 45f)
				{
					delayTrigger = true;
				}
				if (!sprinklesDialogue.isPlaying && currentWaterReminder < waterReminders)
				{
					remindTime += Time.deltaTime;
					if (remindTime > 10f)
					{
						if (currentWaterReminder == 0)
						{
							sprinklesDialogue.PlayClip(gardenScriptable.dialogue.wDripDrop);
						}
						if (currentWaterReminder == 1)
						{
							sprinklesDialogue.PlayClip(gardenScriptable.dialogue.wWateringTooSlow);
						}
						if (currentWaterReminder == 2)
						{
							sprinklesDialogue.PlayClip(gardenScriptable.dialogue.wEqualAmountsOfWater);
						}
						if (currentWaterReminder == 3)
						{
							sprinklesDialogue.PlayClip(gardenScriptable.dialogue.wYouMissedAPatch);
						}
						if (currentWaterReminder == 4)
						{
							sprinklesDialogue.PlayClip(gardenScriptable.dialogue.wDripDropSlow);
						}
						if (currentWaterReminder == 5)
						{
							sprinklesDialogue.PlayClip(gardenScriptable.dialogue.wDontBeTooHasty);
						}
						remindTime = 0f;
						currentWaterReminder++;
					}
				}
				yield return null;
			}
			if (flowers.Count == 0)
			{
				SafeDebug.Log("ooh ur fcked");
			}
			if (flowers.Count == 1)
			{
				flowers[0].GetComponent<MeshFilter>().sharedMesh = null;
				specialFlower = flowers[0].GetComponent<ObjectPottable>();
				specialFlower.shatterSubtitle = shatterSubtitle;
				flowers[0].growthLength = 1E-08f;
				while (flowers[0].growthCount <= 0f)
				{
					yield return null;
				}
			}
			else
			{
				int flowerCount = flowers.Count;
				while (flowers.Count == flowerCount)
				{
					for (int num2 = 0; num2 < flowers.Count; num2++)
					{
						if (flowers[num2].growthCount > 0f)
						{
							flowers[num2].GetComponent<MeshFilter>().sharedMesh = null;
							specialFlower = flowers[num2].GetComponent<ObjectPottable>();
							specialFlower.shatterSubtitle = shatterSubtitle;
							flowers[0].growthLength = 1E-08f;
							flowers[0].WaterPlant();
							flowers.RemoveAt(num2);
						}
						else
						{
							flowers[num2].GetComponent<ObjectPlant>().TriggerDontStop();
						}
					}
					yield return null;
				}
			}
			specialFlowerChild.transform.parent = specialFlower.transform;
			specialFlowerChild.transform.localPosition = Vector3.zero;
			specialFlowerChild.transform.localScale = Vector3.one;
			specialFlowerChild.SetActive(value: true);
			flowerParticles.Play();
			gardenSong.Stop();
			flowerSong.Play();
			wateringCan.GetComponent<LaunchBack>().enabled = false;
			sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.wWowLookAtThatFlower);
			sprinklesDialogue.PlayClip(gardenScriptable.dialogue.wYesYes);
			failTrigger.objectCheck = specialFlower.gameObject;
			failTrigger.gameObject.SetActive(value: true);
			SafeDebug.Log("<Color=green>Special plant watered</color>");
			gardenState = GardenStates.Potting;
		}
		if (gardenState == GardenStates.Potting)
		{
			SafeDebug.Log("<Color=green>Potting</color>");
			sprinklesDialogue.PlayClip(gardenScriptable.dialogue.pPotThatFlowerQuickly);
			sprinklesDialogue.PlayClip(gardenScriptable.dialogue.pPotOfHonour);
			remindPottingCountdown = remindPottingTime;
			bool playFirstReminder = true;
			Vector3 origPotPos = default;
			float failsafeTimer2 = 60f;
			float canBeSmashed = 3f;
			bool flowerLost = false;
			while (!specialFlower.isPotted)
			{
				canBeSmashed -= Time.deltaTime;
				if (canBeSmashed <= 0f)
				{
					specialFlower.canBeSmashed = true;
				}
				if (specialFlower.potHasBroken)
				{
					ToolManager.PlayerGrabbingObjectBase(specialFlower, dropIfGrabbing: true);
					PetalDestroyEffect();
					VVR_HUB.instance.hasCompletedJob = true;
					flowerParticles.Stop();
					flowerSong.Stop();
					sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.cNOOOOO);
					sprinklesDialogue.PlayClip(gardenScriptable.dialogue.cMySoul);
					sprinklesDialogue.PlayClip(gardenScriptable.dialogue.cWhyWhy);
					while (sprinklesDialogue.isPlaying)
					{
						yield return null;
					}
					CompleteJob();
					flowerLost = true;
				}
				for (int num3 = 0; num3 < breakablePots.Count; num3++)
				{
					if (breakablePots[num3] == null || breakablePots[num3].hasPlant())
					{
						breakablePots.RemoveAt(num3);
					}
				}
				if (unbreakablePot.gameObject.activeInHierarchy)
				{
					if (!wateringTrigger.objectInside && !unbreakablePot.GetComponent<ObjectBase>().isGrabbed)
					{
						unbreakablePot.transform.position = origPotPos;
						failsafeTimer2 = 45f;
					}
				}
				else if (breakablePots.Count <= 0)
				{
					unbreakablePot.gameObject.SetActive(value: true);
					unbreakablePot.pickedPottable = specialFlower;
					origPotPos = unbreakablePot.transform.position;
					sprinklesDialogue.PlayClip(gardenScriptable.dialogue.pUseThisPot);
					remindPottingCountdown = remindPottingTime;
					failsafeTimer2 = 45f;
				}
				if (!sprinklesDialogue.isPlaying)
				{
					remindPottingCountdown -= Time.deltaTime;
					if (remindPottingCountdown <= 0f)
					{
						remindPottingCountdown = remindPottingTime;
						if (playFirstReminder)
						{
							sprinklesDialogue.PlayClip(gardenScriptable.dialogue.pPotThatFlowerQuickly);
						}
						else
						{
							sprinklesDialogue.InterruptWithClip(gardenScriptable.dialogue.pPotOfHonour, enqueueIfCantInterrupt: true);
						}
						playFirstReminder = !playFirstReminder;
					}
				}
				if (!specialFlower.isGrabbed)
				{
					failsafeTimer2 -= Time.deltaTime;
					if ((failsafeTimer2 <= 0f || (failsafeTimer2 <= 55f && !failTrigger.objectInside)) && !flowerLost)
					{
						remindPottingCountdown = remindPottingTime;
						VVR_HUB.instance.hasCompletedJob = true;
						flowerParticles.Stop();
						flowerSong.Stop();
						sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.cNOOOOO);
						sprinklesDialogue.PlayClip(gardenScriptable.dialogue.cMySoul);
						sprinklesDialogue.PlayClip(gardenScriptable.dialogue.cWhyWhy);
						while (sprinklesDialogue.isPlaying)
						{
							yield return null;
						}
						CompleteJob();
						flowerLost = true;
					}
				}
				yield return null;
			}
			specialFlower.canBeSmashed = true;
			sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.pGently);
			sprinklesDialogue.PlayClip(gardenScriptable.dialogue.cPutItHighUpNoWorms);
			SafeDebug.Log("<Color=green>Special plant potted</color>");
			gardenState = GardenStates.Placing;
		}
		if (gardenState == GardenStates.Placing)
		{
			SafeDebug.Log("<Color=green>Placing</color>");
			placingTrigger.objectCheck = (placingTriggerShed.objectCheck = (placingTriggerTableLeft.objectCheck = (placingTriggerTableBack.objectCheck = specialFlower.gameObject)));
			delayTimer = 2f;
			delayTrigger = false;
			while (!greenhouseTrigger.playerInside && !specialFlower.potHasBroken)
			{
				if (placingTrigger.objectInside || placingTriggerShed.objectInside || placingTriggerTableLeft.objectInside || placingTrigger.objectInside)
				{
					specialFlower.ForceDropByPlayer();
					specialFlower.myPot.ForceDropByPlayer();
					specialFlower.myPot.smashVelocityMin = 0f;
					specialFlower.myPot.ungrabbable = true;
					specialFlower.ungrabbable = true;
					specialFlower.isPottable = false;
				}
				yield return null;
			}
			if (!specialFlower.potHasBroken)
			{
				sprinklesDialogue.PlayClip(gardenScriptable.dialogue.cShinesDownOnUs);
			}
			while (!placingTrigger.objectInside && !placingTriggerShed.objectInside && !placingTriggerTableLeft.objectInside && !placingTrigger.objectInside)
			{
				if (specialFlower.potHasBroken)
				{
					potHasBroken = true;
					break;
				}
				yield return null;
			}
			while (!potHasBroken && !delayTrigger)
			{
				if (specialFlower.myPot != null)
				{
					specialFlower.myPot.smashVelocityMin = 0f;
					specialFlower.myPot.ungrabbable = true;
					if (!ToolManager.PlayerGrabbingObjectBase(specialFlower.myPot, dropIfGrabbing: true))
					{
						ToolManager.PlayerGrabbingObjectBase(specialFlower, dropIfGrabbing: true);
					}
				}
				else
				{
					ToolManager.PlayerGrabbingObjectBase(specialFlower, dropIfGrabbing: true);
				}
				specialFlower.ungrabbable = true;
				specialFlower.isPottable = false;
				delayTimer -= Time.deltaTime;
				if (delayTimer <= 0f || specialFlower.potHasBroken)
				{
					delayTrigger = true;
				}
				else
				{
					yield return null;
				}
			}
			PetalDestroyEffect();
			VVR_HUB.instance.hasCompletedJob = true;
			flowerParticles.Stop();
			flowerSong.Stop();
			sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.cNOOOOO);
			sprinklesDialogue.PlayClip(gardenScriptable.dialogue.cMySoul);
			sprinklesDialogue.PlayClip(gardenScriptable.dialogue.cWhyWhy);
			while (sprinklesDialogue.isPlaying)
			{
				yield return null;
			}
			CompleteJob();
		}
		yield return null;
	}

	private void StartSprinkleMove(Vector3 dest, float moveSpeed)
	{
		if (moveSprinkleCoroutine == null)
		{
			moveSprinkleCoroutine = GlobalCoroutineContainer.AddThread(MoveSprinkleTooth(dest, moveSpeed), this);
		}
	}

	private void PetalDestroyEffect()
	{
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < specialFlowerPetals.Count; i++)
		{
			normalFlowerPetals[i].SetActive(value: false);
			specialFlowerPetals[i].gameObject.SetActive(value: true);
			specialFlowerPetals[i].isKinematic = false;
			specialFlowerPetals[i].useGravity = true;
			specialFlowerPetals[i].transform.parent = transform;
			zero = Random.insideUnitSphere.normalized * Time.deltaTime * petalDestroyForce;
			if (zero.y < 0f)
			{
				zero.y *= -1f;
			}
			specialFlowerPetals[i].AddForce(zero, ForceMode.Impulse);
		}
	}

	private IEnumerator MoveSprinkleTooth(Vector3 dest, float moveSpeed)
	{
		sprinklesRustlingEffects.SetActive(value: true);
		sprinklesRustlingEffects.GetComponentInChildren<GvrAudioSource>().UnPause();
		sprinklesRustlingEffects.GetComponentInChildren<ParticleSystem>().Play();
		sprinklesIsMoving = true;
		sprinkleStart = sprinklesGO.transform.position;
		moveLerp = 0f;
		while (moveLerp < 1f)
		{
			moveLerp += Time.deltaTime * moveSpeed;
			sprinklesGO.transform.position = Vector3.Lerp(sprinkleStart, dest, moveLerp);
			sprinklesRustlingEffects.transform.position = sprinklesGO.transform.position;
			sprinklesRustlingEffects.GetComponentInChildren<GvrAudioSource>().volume = 1f - moveLerp;
			yield return null;
		}
		sprinklesRustlingEffects.GetComponentInChildren<GvrAudioSource>().Pause();
		sprinklesRustlingEffects.GetComponentInChildren<ParticleSystem>().Stop();
		sprinklesIsMoving = false;
		sprinklesGO.transform.position = dest;
		moveSprinkleCoroutine = null;
	}

	private void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(sprinklesGO.transform.position, 0.1f);
		}
	}
}
