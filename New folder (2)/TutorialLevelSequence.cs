using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Edelweiss.Coroutine;
using TButt;
using TMPro;
using UnityEngine;

public class TutorialLevelSequence : SceneBehaviorManager
{
	public enum TutorialStates
	{
		Intro,
		Grab,
		Move,
		Headset,
		Graduation,
		Finished
	}

	public enum AestheticObjects
	{
		Plant,
		Ball,
		Fish
	}

	[SerializeField]
	private TutorialStates tutorialState;

	[SerializeField]
	private LoginSequenceScriptableObject tweakables;

	[Header("References:")]
	[SerializeField]
	private VVRDialogueSource chazDialogueSource;

	[SerializeField]
	public Material buttonPress;

	[SerializeField]
	public Material buttonUp;

	[SerializeField]
	public Renderer controllerApp;

	[SerializeField]
	public Renderer controllerTouch;

	[SerializeField]
	public Renderer controllerTouchPos;

	[SerializeField]
	private GameObject controller;

	[SerializeField]
	private GameObject glowArrow;

	[SerializeField]
	private Transform cursorEyeTracker;

	[SerializeField]
	private GvrAudioSource successSound;

	[Header("Intro Sequence References:")]
	[SerializeField]
	private GameObject activitudeLogoMesh;

	[SerializeField]
	private TextMeshPro employeeNumber;

	[SerializeField]
	private GameObject chazNametag;

	[SerializeField]
	private GameObject boids;

	[SerializeField]
	private ObjectBoid eaterBoid;

	[SerializeField]
	private AudioLoop employeeNumberTick;

	[Header("Grab Sequence References:")]
	[SerializeField]
	private ChazController chazControl;

	[SerializeField]
	private ObjectBase chazObj;

	[SerializeField]
	private GameObject nervousEyeShaker;

	[SerializeField]
	private ObjectBase shoeObj;

	[SerializeField]
	private ObjectBase fishObj;

	[SerializeField]
	private ObjectBase plantObj;

	[SerializeField]
	private GameObject plantDiagram;

	[SerializeField]
	private GameObject fishDiagram;

	[SerializeField]
	private GameObject[] grabTutorialPedestals;

	[SerializeField]
	private GameObject grabTutorialCoffeeTableStuff;

	[SerializeField]
	private GameObject fishFoodObj;

	[SerializeField]
	private GameObject wateringCanObj;

	[SerializeField]
	private GameObject grabTutorialFishWaterPedestal;

	[SerializeField]
	private TutorialCoffeeTableSensor coffeeTableSensor;

	[SerializeField]
	private ObjectRespawningDome[] objectCatchers;

	[Header("Move Sequence References:")]
	[SerializeField]
	private GameObject teleportTarget;

	[SerializeField]
	private List<GameObject> headsets;

	[SerializeField]
	private GameObject headsetBox;

	[SerializeField]
	private Headset headsetHacked;

	[SerializeField]
	private AudioLoop headsetHackedSound;

	[Header("Graduation Sequence References:")]
	[SerializeField]
	private GameObject toKitchenHeadset;

	[SerializeField]
	private GameObject confettiParticles;

	[SerializeField]
	private GameObject gradGifts;

	[Header("Chaz Blocking")]
	[SerializeField]
	private Transform RightDown;

	[SerializeField]
	private Transform CenterUp;

	[SerializeField]
	private Transform CenterDown;

	[SerializeField]
	private Transform LeftDown;

	[Space]
	public MaterialUpdateController lightProperties;

	private AestheticObjects aestheticChoice;

	private GameObject[] grabTutorialObjects;

	private GameObject[] pourTutorialObjects;

	private float nourishSelectionTimer;

	[SerializeField]
	private TCPlatformScene _tutorialScene;

	[SerializeField]
	private TCPlatformScene _demoScene1;

	[SerializeField]
	private TCPlatformScene _demoScene2;

	[SerializeField]
	private TCPlatformScene _demoScene3;

	[SerializeField]
	private TCPlatformScene _demoScene4;

	private SafeCoroutine retrieveCoroutine;

	private bool returnedFromDemo1;

	private bool returnedFromDemo2;

	private bool returnedFromDemo3;

	private bool returnedFromDemo4;

	private bool returnedFromKitchen;

	internal bool rotateForTeleport;

	internal bool rotateForGrab;

	internal bool dontDisableTouchPos;

	internal Vector3 teleportEuler;

	internal Vector3 grabEuler;

	internal Quaternion startControllerRot;

	private bool userExitedButter;

	private Vector3 getNearPlayer => EagerMaster.instance.mainCamera.transform.position + Vector3.forward * 5f;

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		startControllerRot = controller.transform.rotation;
		if (PersistentDataManager.HasAccountLoaded())
		{
			tutorialState = PersistentDataManager.GetSavedTutorialState();
		}
		else
		{
			tutorialState = TutorialStates.Intro;
		}
		EagerScenes.instance.OnSceneActivated += reactToSceneActivation;
		setUpTutorialObjects();
		GlobalCoroutineContainer.AddThread(tutorialSequence(), this);
	}

	protected override void OnSceneActivated()
	{
		lightProperties.PushMaterials();
		GetComponent<EagerScene>().defaultMaterialProperties = lightProperties;
	}

	private void setUpTutorialObjects()
	{
		List<GameObject> list = new List<GameObject>();
		list.Add(shoeObj.gameObject);
		list.Add(fishObj.gameObject);
		list.Add(plantObj.gameObject);
		List<GameObject> list2 = list;
		GameObject[] array = grabTutorialPedestals;
		foreach (GameObject item in array)
		{
			list2.Add(item);
		}
		grabTutorialObjects = list2.ToArray();
		pourTutorialObjects = new GameObject[3] { fishFoodObj, wateringCanObj, grabTutorialFishWaterPedestal };
		ObjectRespawningDome[] array2 = objectCatchers;
		foreach (ObjectRespawningDome objectRespawningDome in array2)
		{
			objectRespawningDome.gameObject.SetActive(value: false);
		}
		activitudeLogoMesh.SetActive(value: false);
		boids.SetActive(value: false);
		grabTutorialCoffeeTableStuff.SetActive(value: false);
		toKitchenHeadset.SetActive(value: false);
		gradGifts.SetActive(value: false);
		confettiParticles.SetActive(value: false);
		employeeNumber.gameObject.SetActive(value: false);
		setGameObjectsEnabled(ref grabTutorialObjects, enable: false);
		setGameObjectsEnabled(ref pourTutorialObjects, enable: false);
		controller.SetActive(value: false);
		headsetBox.SetActive(value: false);
		glowArrow.SetActive(value: false);
		chazNametag.SetActive(value: false);
	}

	private IEnumerator tutorialSequence()
	{
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_PAUSE);
		if (tutorialState < TutorialStates.Graduation)
		{
			EagerScenes.instance.LoadQueue(_demoScene1);
			EagerScenes.instance.LoadQueue(_demoScene2);
			EagerScenes.instance.LoadQueue(_demoScene3);
			EagerScenes.instance.LoadQueue(_demoScene4);
			EagerScenes.instance.LoadQueue(TCScenes.S.VVR_LVL_PHONEBOOTH);
			EagerScenes.instance.LoadQueue(TCScenes.S.VVR_LVL_RADIOBAIT);
		}
		Teleporter.SetTeleportEnabled(enabledIn: false);
		if (!PersistentDataManager.HasAccountLoaded() || !PersistentDataManager.GetHasAdmin())
		{
			HeadsetRemovalUIManager.instance.canRemove = false;
		}
		if (tutorialState == TutorialStates.Intro)
		{
			PersistentDataManager.OverwriteLastLoadedScene(TCScenes.S.VVR_LVL_LOGIN);
			activitudeLogoMesh.SetActive(value: true);
			boids.SetActive(value: true);
			chazControl.WalkTo(LeftDown.position);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
			chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
			chazDialogueSource.PlayClip(tweakables.dialogue.iSequenceIntro);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
			eaterBoid.SetBehaviorMode(ObjectBoid.behaviorMode.construct);
			while (chazDialogueSource.isPlaying)
			{
				yield return null;
			}
			chazDialogueSource.PlayClip(tweakables.dialogue.iHereAtActivitude);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(9f, this);
			chazControl.WalkTo(RightDown.position);
			while (chazDialogueSource.isPlaying)
			{
				yield return null;
			}
			activitudeLogoMesh.SetActive(value: false);
			boids.SetActive(value: false);
			chazDialogueSource.PlayClip(tweakables.dialogue.iEmployeeNumber);
			while (chazDialogueSource.isPlaying)
			{
				yield return null;
			}
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
			chazControl.EyeLookAt(employeeNumber.transform, 2.5f, ChazController.eyeColor.Default, immediately: false);
			employeeNumber.gameObject.SetActive(value: true);
			List<int> randomizedIndices = Enumerable.Range(0, 8).ToList();
			string animatingUsernameString = string.Empty;
			float digitLockInInterval = 0.4f;
			float digitUpdateInterval = 0.015f;
			float digitUpdateT = 0f;
			float digitLockInT = 0f;
			employeeNumberTick.Play();
			while (randomizedIndices.Count > 0)
			{
				digitUpdateT += Time.deltaTime;
				digitLockInT += Time.deltaTime;
				if (digitUpdateT >= digitUpdateInterval)
				{
					if (digitLockInT >= digitLockInInterval)
					{
						randomizedIndices.RemoveAt(Random.Range(0, randomizedIndices.Count));
						digitLockInT = 0f;
					}
					animatingUsernameString = UsernameGenerator.GetRandomDigitedUsername(PersistentDataManager.GetUsername(), randomizedIndices, animatingUsernameString);
					employeeNumber.text = UsernameGenerator.GetPrefixedUsername(animatingUsernameString);
					digitUpdateT = 0f;
				}
				yield return null;
			}
			employeeNumberTick.Stop();
			while (chazDialogueSource.isPlaying)
			{
				yield return null;
			}
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
			chazDialogueSource.PlayClip(tweakables.dialogue.iLetsCallYouBee);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(2.55f, this);
			employeeNumber.fontSize *= 2f;
			employeeNumber.GetComponent<TextSwapper>().enabled = true;
			employeeNumber.alignment = TextAlignmentOptions.Center;
			employeeNumber.transform.position = new Vector3(0f, employeeNumber.transform.position.y, employeeNumber.transform.position.z);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(2.2f, this);
			chazNametag.SetActive(value: true);
			while (chazDialogueSource.isPlaying)
			{
				yield return null;
			}
			chazDialogueSource.PlayClip(tweakables.dialogue.iLetsStartTraining);
			while (chazDialogueSource.isPlaying)
			{
				yield return null;
			}
			employeeNumber.gameObject.SetActive(value: false);
			chazNametag.SetActive(value: false);
			AdvanceAndSaveTutorialState(TutorialStates.Grab);
		}
		if (tutorialState == TutorialStates.Grab)
		{
			ToolManager.AllManipulators.UserCanGrab = false;
			setGameObjectsEnabled(ref grabTutorialObjects, enable: true);
			chazDialogueSource.PlayClip(tweakables.dialogue.gSequenceIntro);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
			chazControl.WalkTo(CenterDown.position);
			chazControl.EyeLookAt(cursorEyeTracker, 0f, ChazController.eyeColor.Default, immediately: true);
			controller.SetActive(value: true);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(5f, this);
			ToolManager.AllManipulators.UserCanGrab = true;
			if (rotateForGrab)
			{
				GlobalCoroutineContainer.AddThread(rotateControllerGrab(1f), this);
			}
			controllerTouch.sharedMaterial = buttonPress;
			bool firedFlatteredLine = false;
			bool letGoLine = false;
			float chazHoldTimer = 0f;
			while (!ToolManager.PlayerGrabbingObjectBase(plantObj) && !ToolManager.PlayerGrabbingObjectBase(shoeObj) && !ToolManager.PlayerGrabbingObjectBase(fishObj))
			{
				if (ToolManager.PlayerGrabbingObjectType<ObjectChazLeg>())
				{
					if (!firedFlatteredLine)
					{
						chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 3f, ChazController.eyeColor.Happy, immediately: true);
						chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.gChazFlattered);
						firedFlatteredLine = true;
					}
					else
					{
						if (!chazDialogueSource.isPlaying)
						{
							chazHoldTimer += Time.deltaTime;
						}
						if (chazHoldTimer > 4.5f && !letGoLine)
						{
							chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.gChazLetGo);
							letGoLine = true;
						}
					}
				}
				yield return null;
			}
			ObjectBase selectedObj = null;
			if (ToolManager.PlayerGrabbingObjectBase(plantObj))
			{
				selectedObj = plantObj;
			}
			else if (ToolManager.PlayerGrabbingObjectBase(shoeObj))
			{
				selectedObj = shoeObj;
			}
			else if (ToolManager.PlayerGrabbingObjectBase(fishObj))
			{
				selectedObj = fishObj;
			}
			if (plantObj.GetComponent<SpringJoint>() != null)
			{
				Object.Destroy(plantObj.GetComponent<SpringJoint>());
			}
			if (shoeObj.GetComponent<SpringJoint>() != null)
			{
				Object.Destroy(shoeObj.GetComponent<SpringJoint>());
			}
			if (fishObj.GetComponent<SpringJoint>() != null)
			{
				Object.Destroy(fishObj.GetComponent<SpringJoint>());
			}
			controller.GetComponent<SpawnScale>().Disable();
			controllerTouch.sharedMaterial = buttonUp;
			selectedObj.undroppable = true;
			ToolManager.AllManipulators.UserCanScroll = false;
			setGameObjectsEnabled(ref grabTutorialObjects, enable: false);
			selectedObj.gameObject.SetActive(value: true);
			chazControl.EyeLookAt(selectedObj.transform, 1f, ChazController.eyeColor.Angry, immediately: false);
			chazControl.WalkTo(LeftDown.position);
			if (selectedObj == plantObj)
			{
				aestheticChoice = AestheticObjects.Plant;
				chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.gPlant);
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(2.5f, this);
				fishDiagram.gameObject.SetActive(value: true);
				chazControl.EyeLookAt(fishDiagram.transform, 2f, ChazController.eyeColor.Default, immediately: false);
			}
			else if (selectedObj == shoeObj || selectedObj == fishObj)
			{
				if (selectedObj == shoeObj)
				{
					aestheticChoice = AestheticObjects.Ball;
				}
				else
				{
					aestheticChoice = AestheticObjects.Fish;
				}
				chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.gShoeOrFish);
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(2.5f, this);
				plantDiagram.gameObject.SetActive(value: true);
				chazControl.EyeLookAt(plantDiagram.transform, 2f, ChazController.eyeColor.Default, immediately: false);
			}
			else
			{
				SafeDebug.LogError("Tutorial Error: GrabbedObject should be plant, shoe, or fish, nothing else!");
			}
			PersistentDataManager.OverwriteAestheticObject(aestheticChoice);
			chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
			while (chazDialogueSource.isPlaying)
			{
				yield return null;
			}
			plantDiagram.GetComponent<SpawnScale>().Disable();
			fishDiagram.GetComponent<SpawnScale>().Disable();
			ObjectRespawningDome[] array = objectCatchers;
			foreach (ObjectRespawningDome objectRespawningDome in array)
			{
				objectRespawningDome.gameObject.SetActive(value: true);
			}
			selectedObj.undroppable = false;
			grabTutorialCoffeeTableStuff.SetActive(value: true);
			coffeeTableSensor.SetTargetObject(selectedObj.colliders);
			chazControl.EyeLookAt(selectedObj.transform, 0f, ChazController.eyeColor.Default, immediately: true);
			chazDialogueSource.PlayClip(tweakables.dialogue.gPutOnCoffeeTable);
			while (ToolManager.PlayerGrabbingSomething())
			{
				yield return null;
			}
			if (chazDialogueSource.isPlaying)
			{
				chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0.5f, ChazController.eyeColor.Happy, immediately: false);
				chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.gThereYouGo);
			}
			ToolManager.AllManipulators.UserCanGrab = false;
			ToolManager.AllManipulators.UserCanScroll = false;
			successSound.Play();
			chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
			while (!coffeeTableSensor.DidPlayerPutObjectOnCoffeeTable())
			{
				for (int j = 0; j < objectCatchers.Length; j++)
				{
					if (objectCatchers[j].objectHasRespawned)
					{
						chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0.5f, ChazController.eyeColor.Angry, immediately: false);
						chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.gRespectTrainingMaterials);
						break;
					}
				}
				yield return null;
			}
			while (chazDialogueSource.isPlaying)
			{
				yield return null;
			}
			grabTutorialFishWaterPedestal.SetActive(value: true);
			if (selectedObj == plantObj)
			{
				fishFoodObj.SetActive(value: true);
				chazControl.EyeLookAt(fishFoodObj.transform, 0f, ChazController.eyeColor.Default, immediately: true);
				chazDialogueSource.PlayClip(tweakables.dialogue.gPlantChastise);
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
				controller.SetActive(value: true);
				if (rotateForGrab)
				{
					GlobalCoroutineContainer.AddThread(rotateControllerGrab(1f), this);
				}
				controllerTouch.sharedMaterial = buttonPress;
				ToolManager.AllManipulators.UserCanGrab = true;
				chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
				chazDialogueSource.PlayClip(tweakables.dialogue.gPickUpFishFood);
			}
			else if (selectedObj == shoeObj || selectedObj == fishObj)
			{
				wateringCanObj.SetActive(value: true);
				chazControl.EyeLookAt(wateringCanObj.transform, 0f, ChazController.eyeColor.Default, immediately: true);
				chazDialogueSource.PlayClip(tweakables.dialogue.gFishShoeChastise);
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
				controller.SetActive(value: true);
				if (rotateForGrab)
				{
					GlobalCoroutineContainer.AddThread(rotateControllerGrab(1f), this);
				}
				controllerTouch.sharedMaterial = buttonPress;
				ToolManager.AllManipulators.UserCanGrab = true;
				chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
				chazDialogueSource.PlayClip(tweakables.dialogue.gPickUpWateringCan);
			}
			ToolManager.AllManipulators.UserCanScroll = true;
			while (chazDialogueSource.isPlaying)
			{
				yield return null;
			}
			controllerTouch.sharedMaterial = buttonUp;
			controllerTouchPos.sharedMaterial = buttonPress;
			controllerTouchPos.gameObject.SetActive(value: true);
			while (!ToolManager.PlayerGrabbingGameObject(wateringCanObj) && !ToolManager.PlayerGrabbingGameObject(fishFoodObj))
			{
				yield return null;
			}
			wateringCanObj.GetComponent<ObjectBase>().undroppable = true;
			fishFoodObj.GetComponent<ObjectBase>().undroppable = true;
			if (rotateForGrab)
			{
				GlobalCoroutineContainer.AddThread(rotateToStartRot(1f), this);
			}
			if (selectedObj == plantObj)
			{
				chazControl.EyeLookAt(fishFoodObj.transform, 0f, ChazController.eyeColor.Default, immediately: true);
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
				chazDialogueSource.PlayClip(tweakables.dialogue.gPlantNourish);
			}
			else if (selectedObj == shoeObj || selectedObj == fishObj)
			{
				chazControl.EyeLookAt(wateringCanObj.transform, 0f, ChazController.eyeColor.Default, immediately: true);
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
				chazDialogueSource.PlayClip(tweakables.dialogue.gFishShoeNourish);
			}
			while (chazDialogueSource.isPlaying)
			{
				yield return null;
			}
			bool firedNourishHurryUp = false;
			bool nourishLimit = false;
			while (!selectedObj.isSatisfied)
			{
				nourishSelectionTimer += Time.deltaTime;
				if (!firedNourishHurryUp && nourishSelectionTimer > tweakables.pourSequenceHurryUpDelay)
				{
					firedNourishHurryUp = true;
					if (selectedObj == plantObj)
					{
						chazDialogueSource.PlayClip(tweakables.dialogue.gTakingTooLongToFeed);
						chazDialogueSource.PlayClip(tweakables.dialogue.gLeashLength);
						chazDialogueSource.PlayClip(tweakables.dialogue.gFishFoodUpsideDown);
					}
					else
					{
						chazDialogueSource.PlayClip(tweakables.dialogue.gTakingTooLongToWater);
						chazDialogueSource.PlayClip(tweakables.dialogue.gLeashLength);
						chazDialogueSource.PlayClip(tweakables.dialogue.gFishShoeNourish);
					}
				}
				if (firedNourishHurryUp && nourishSelectionTimer > tweakables.pourSequenceTargetCompletionTime)
				{
					nourishLimit = true;
				}
				yield return null;
			}
			successSound.Play();
			chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
			if (chazDialogueSource.isPlaying)
			{
				chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0.5f, ChazController.eyeColor.Happy, immediately: false);
				chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.gNowYouveGotIt);
				while (chazDialogueSource.isPlaying)
				{
					yield return null;
				}
			}
			if (nourishLimit)
			{
				chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.gSlowCompletion);
			}
			else if (!firedNourishHurryUp)
			{
				chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.gFastCompletion);
			}
			if (!dontDisableTouchPos)
			{
				controllerTouchPos.gameObject.SetActive(value: false);
			}
			else
			{
				controllerTouchPos.sharedMaterial = buttonUp;
			}
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
			ToolManager.DropEverything();
			ToolManager.AllManipulators.UserCanScroll = true;
			setGameObjectsEnabled(ref grabTutorialObjects, enable: false);
			grabTutorialCoffeeTableStuff.SetActive(value: false);
			setGameObjectsEnabled(ref pourTutorialObjects, enable: false);
			while (chazDialogueSource.isPlaying)
			{
				yield return null;
			}
			AdvanceAndSaveTutorialState(TutorialStates.Move);
			PersistentDataManager.WriteActiveSaveToDisk();
		}
		if (tutorialState == TutorialStates.Move)
		{
			float originalMinHoldTime = ToolManager.AllTeleporters.MinHoldTime;
			ToolManager.AllTeleporters.MinHoldTime = 0.4f;
			yield return null;
			Vector3 originalPos = EagerMaster.instance.player.transform.position;
			chazControl.WalkTo(CenterUp.position);
			chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
			bool early = true;
			bool finished = false;
			bool gtfo = false;
			chazDialogueSource.PlayClip(tweakables.dialogue.mPracticeTeleporting);
			while (chazDialogueSource.isPlaying)
			{
				yield return null;
			}
			Teleporter.SetTeleportEnabled(enabledIn: true);
			teleportTarget.SetActive(value: true);
			if (!gtfo)
			{
				chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 2f, ChazController.eyeColor.Happy, immediately: false);
				chazDialogueSource.PlayClip(tweakables.dialogue.mFavoritePartMonologue);
				while (chazDialogueSource.isPlaying)
				{
					if (originalPos != EagerMaster.instance.player.transform.position)
					{
						gtfo = true;
						break;
					}
					yield return null;
				}
			}
			if (!gtfo)
			{
				chazDialogueSource.PlayClip(tweakables.dialogue.mTheKeyMonologue);
				float timer = 0f;
				while (chazDialogueSource.isPlaying)
				{
					timer += Time.deltaTime;
					if (timer > 6f)
					{
						controller.SetActive(value: true);
						if (rotateForTeleport)
						{
							GlobalCoroutineContainer.AddThread(rotateControllerTeleport(1f), this);
						}
					}
					if (originalPos != EagerMaster.instance.player.transform.position)
					{
						gtfo = true;
						break;
					}
					yield return null;
				}
			}
			if (!gtfo)
			{
				controllerApp.sharedMaterial = buttonPress;
				chazDialogueSource.PlayClip(tweakables.dialogue.mDoYouFeelIt);
				chazDialogueSource.PlayClip(tweakables.dialogue.mThereItIs);
				bool thereYouGo = false;
				bool keepHolding = false;
				while (chazDialogueSource.isPlaying)
				{
					if (ToolManager.AnyControllerGetButtonDown(TBInput.Button.Action2))
					{
						yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
						if (ToolManager.AnyControllerGetButton(TBInput.Button.Action2) && !thereYouGo)
						{
							chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.mButtonPressed);
							thereYouGo = true;
						}
						else if (!keepHolding)
						{
							chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.mKeepHolding);
							keepHolding = true;
						}
					}
					if (originalPos != EagerMaster.instance.player.transform.position)
					{
						successSound.Play();
						yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
						chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 2f, ChazController.eyeColor.Happy, immediately: false);
						chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.mSoProud);
						while (chazDialogueSource.isPlaying)
						{
							yield return null;
						}
						finished = true;
						gtfo = true;
						break;
					}
					yield return null;
				}
				early = false;
			}
			if (!gtfo)
			{
				chazDialogueSource.PlayClip(tweakables.dialogue.mFinalInstructions);
				while (chazDialogueSource.isPlaying)
				{
					if (originalPos != EagerMaster.instance.player.transform.position)
					{
						gtfo = true;
						break;
					}
					yield return null;
				}
			}
			if (!gtfo)
			{
				chazControl.EyeLookAt(nervousEyeShaker.transform, 4f, ChazController.eyeColor.Thinking, immediately: false);
				chazDialogueSource.PlayClip(tweakables.dialogue.mFearOfLava);
				chazDialogueSource.PlayClip(tweakables.dialogue.mStandingInLava);
				while (chazDialogueSource.isPlaying)
				{
					if (originalPos != EagerMaster.instance.player.transform.position)
					{
						gtfo = true;
						break;
					}
					yield return null;
				}
				finished = true;
			}
			controller.GetComponent<SpawnScale>().Disable();
			controllerApp.sharedMaterial = buttonUp;
			ToolManager.AllTeleporters.MinHoldTime = originalMinHoldTime;
			if (!finished)
			{
				successSound.Play();
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
				teleportTarget.GetComponent<SpawnScale>().Disable();
				if (early)
				{
					chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 4f, ChazController.eyeColor.Angry, immediately: false);
					chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.mFiguredItOut);
					while (chazDialogueSource.isPlaying)
					{
						yield return null;
					}
				}
				else
				{
					chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 2f, ChazController.eyeColor.Happy, immediately: false);
					chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.mRoughInTheMiddle);
					while (chazDialogueSource.isPlaying)
					{
						yield return null;
					}
				}
			}
			if (teleportTarget.GetComponent<SpawnScale>().enabled)
			{
				teleportTarget.GetComponent<SpawnScale>().Disable();
			}
			chazControl.WalkTo(getNearPlayer);
			AdvanceAndSaveTutorialState(TutorialStates.Headset);
		}
		Teleporter.SetTeleportEnabled(enabledIn: true);
		if (tutorialState == TutorialStates.Headset)
		{
			chazControl.WalkTo(getNearPlayer);
			chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
			chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.hSequenceIntro);
			while (chazDialogueSource.isPlaying)
			{
				yield return null;
			}
			HeadsetRemovalUIManager.instance.canRemove = true;
			int spawnedHeadsets = 0;
			chazControl.PresentObject(headsets[spawnedHeadsets]);
			headsets[spawnedHeadsets].GetComponent<Headset>().isActivated = false;
			headsets[spawnedHeadsets].SetActive(value: true);
			headsets[spawnedHeadsets].GetComponent<Headset>().isActivated = true;
			spawnedHeadsets++;
			if (Vector3.Distance(EagerMaster.instance.mainCamera.transform.position, chazControl.head.transform.position) < 5f)
			{
				chazControl.WalkTowards(EagerMaster.instance.mainCamera.transform.position, -1.5f);
			}
			else
			{
				chazControl.WalkTo(getNearPlayer);
			}
			chazDialogueSource.PlayClip(tweakables.dialogue.hPleaseSelectAnAccessPoint);
			controller.SetActive(value: true);
			controllerTouch.sharedMaterial = buttonPress;
			glowArrow.SetActive(value: true);
			SafeDebug.Log("WAITING FOR HEADSET SELECTION");
			chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
			float headsetWaitTimer = 0f;
			bool hurryUpFlag = false;
			bool inMyMouthFlag = false;
			bool furtherInstruction = false;
			float t = 0f;
			while (!returnedFromDemo1)
			{
				if (ToolManager.PlayerGrabbingGameObject(headsets[0]) && !furtherInstruction)
				{
					controllerTouch.sharedMaterial = buttonUp;
					controllerTouchPos.gameObject.SetActive(value: true);
					glowArrow.SetActive(value: false);
					chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.hFurtherInstruction);
					furtherInstruction = true;
					inMyMouthFlag = true;
				}
				if (!chazControl.isPresenting && !ToolManager.PlayerGrabbingGameObject(headsets[0]))
				{
					t += Time.deltaTime;
					if (hurryUpFlag)
					{
						if (t > 3.5f && !ToolManager.PlayerGrabbingGameObject(headsets[0]))
						{
							chazControl.GrabObject(headsets[0]);
							t = 0f;
						}
					}
					else if (t > 2f && !ToolManager.PlayerGrabbingGameObject(headsets[0]))
					{
						chazControl.GrabObject(headsets[0]);
						chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.hFailToSelectAccessPoint);
						hurryUpFlag = true;
						t = 0f;
					}
				}
				else
				{
					t = 0f;
				}
				if (!chazDialogueSource.isPlaying)
				{
					headsetWaitTimer += Time.deltaTime;
				}
				if (headsetWaitTimer > tweakables.headsetHurryUpDelay && !hurryUpFlag && !ToolManager.PlayerGrabbingGameObject(headsets[0]))
				{
					hurryUpFlag = true;
					chazDialogueSource.PlayClip(tweakables.dialogue.hFailToSelectAccessPoint);
				}
				else if (!inMyMouthFlag && hurryUpFlag && headsetWaitTimer > 18f && !ToolManager.PlayerGrabbingGameObject(headsets[0]))
				{
					inMyMouthFlag = true;
					chazDialogueSource.PlayClip(tweakables.dialogue.hItsInMyMouth);
				}
				yield return null;
			}
			controller.GetComponent<SpawnScale>().Disable();
			if (!dontDisableTouchPos)
			{
				controllerTouchPos.gameObject.SetActive(value: false);
			}
			else
			{
				controllerTouchPos.sharedMaterial = buttonUp;
			}
			SafeDebug.Log("BACK FROM TURBINE");
			chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.hWelcomeBack1);
			SafeDebug.Log("BACK FROM TURBINE AND Played clip");
			chazControl.WalkTo(getNearPlayer);
			chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
			headsetBox.SetActive(value: true);
			headsetBox.transform.position = EagerMaster.instance.player.transform.position + EagerMaster.instance.mainCamera.transform.forward * 2.5f + EagerMaster.instance.mainCamera.transform.right * 1.5f + Vector3.up * 4f;
			for (int k = 1; k < headsets.Count; k++)
			{
				headsets[k].SetActive(value: true);
			}
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.25f, this);
			for (int l = 1; l < headsets.Count; l++)
			{
				headsets[l].GetComponent<Headset>().isActivated = true;
				headsets[l].GetComponent<Headset>().Scene = _demoScene2;
			}
			while (!returnedFromDemo2)
			{
				yield return null;
			}
			SafeDebug.Log("BACK FROM THE DEEP");
			headsetHacked.gameObject.SetActive(value: true);
			if (Vector3.Distance(headsetBox.transform.position, EagerMaster.instance.player.transform.position) < 5f)
			{
				headsetHacked.transform.position = (headsetBox.transform.position - EagerMaster.instance.player.transform.position) * 0.6f + EagerMaster.instance.player.transform.position - Vector3.up * 1.4f - EagerMaster.instance.mainCamera.transform.right * 0.3f;
			}
			else
			{
				headsetHacked.transform.position = EagerMaster.instance.player.transform.position + EagerMaster.instance.mainCamera.transform.forward * 1.4f - Vector3.up * 1.4f - EagerMaster.instance.mainCamera.transform.right * 0.3f;
			}
			chazControl.WalkTo(getNearPlayer);
			chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
			chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.hWelcomeBack2);
			for (int m = 1; m < headsets.Count; m++)
			{
				Headset component = headsets[m].GetComponent<Headset>();
				if (!component.hasBeenUsed)
				{
					component.Scene = _demoScene3;
				}
			}
			bool skippedToRadio = false;
			while (!returnedFromDemo3 && !returnedFromDemo4)
			{
				if (returnedFromDemo4)
				{
					skippedToRadio = true;
				}
				yield return null;
			}
			if (!skippedToRadio)
			{
				SafeDebug.Log("BACK FROM GARDEN");
				chazControl.WalkTo(getNearPlayer);
				chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
				headsetHackedSound.Play();
				TransformShaker hackShake = headsetHacked.GetComponent<TransformShaker>();
				hackShake.rotation.amount = 10f;
				hackShake.rotation.frequency = 8f;
				chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.hAnother[0]);
				for (int n = 1; n < headsets.Count; n++)
				{
					Headset component2 = headsets[n].GetComponent<Headset>();
					if (!component2.hasBeenUsed)
					{
						component2.Scene = _demoScene4;
					}
				}
				float anotherTimer = 0f;
				int anotherIndex = 1;
				while (!returnedFromDemo4)
				{
					anotherTimer += Time.deltaTime;
					if (anotherTimer > 5f)
					{
						chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.hAnother[anotherIndex]);
						anotherTimer = 0f;
						anotherIndex++;
						if (anotherIndex >= tweakables.dialogue.hAnother.Length)
						{
							anotherIndex = 0;
						}
					}
					yield return null;
				}
			}
			SafeDebug.Log("BACK FROM RADIO");
			if (EagerScenes.instance.lastHeadsetRemoved != headsetHacked)
			{
				headsetHacked.transform.position = EagerScenes.instance.lastHeadsetRemoved.transform.position;
				headsetHacked.transform.rotation = EagerScenes.instance.lastHeadsetRemoved.transform.rotation;
				headsetHacked.rb.velocity = EagerScenes.instance.lastHeadsetRemoved.rb.velocity;
				headsetHacked.rb.angularVelocity = EagerScenes.instance.lastHeadsetRemoved.rb.angularVelocity;
				EagerScenes.instance.lastHeadsetRemoved.gameObject.SetActive(value: false);
			}
			chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.hReactToBackstageSlipUp);
			chazControl.WalkTo(getNearPlayer);
			chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
			headsetHacked.isActivated = false;
			for (int num = 0; num < headsets.Count; num++)
			{
				headsets[num].GetComponent<Headset>().Scene = EagerScenes.instance.ActiveScene.Scene;
				headsets[num].GetComponent<Headset>().isActivated = false;
			}
			EagerScenes.instance.DestroyScene(_demoScene1);
			EagerScenes.instance.DestroyScene(_demoScene2);
			EagerScenes.instance.DestroyScene(_demoScene3);
			EagerScenes.instance.DestroyScene(_demoScene4);
			EagerScenes.instance.DestroyScene(TCScenes.S.VVR_LVL_PHONEBOOTH);
			EagerScenes.instance.DestroyScene(TCScenes.S.VVR_LVL_RADIOBAIT);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.7f, this);
			chazControl.LazerObject(headsetHacked.gameObject);
			while (chazDialogueSource.isPlaying)
			{
				yield return null;
			}
			chazControl.EyeLookAt(nervousEyeShaker.transform, 2f, ChazController.eyeColor.Thinking, immediately: false);
			chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.hSkipAheadToGraduation);
			while (chazDialogueSource.isPlaying)
			{
				yield return null;
			}
			ToolManager.DropEverything();
			for (int num2 = 0; num2 < headsets.Count; num2++)
			{
				headsets[num2].gameObject.SetActive(value: false);
			}
			headsetBox.SetActive(value: false);
			AdvanceAndSaveTutorialState(TutorialStates.Graduation);
		}
		HeadsetRemovalUIManager.instance.canRemove = true;
		if (tutorialState != TutorialStates.Graduation)
		{
			yield break;
		}
		Teleporter.SetTeleportEnabled(enabledIn: true);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_LVL_JOBKITCHEN);
		chazControl.WalkTo(getNearPlayer);
		chazControl.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.gCongratulationsFirstClient);
		confettiParticles.transform.parent = chazObj.transform;
		confettiParticles.transform.localPosition = Vector3.zero + Vector3.up * 0.2f;
		confettiParticles.transform.localRotation = Quaternion.identity;
		confettiParticles.SetActive(value: true);
		gradGifts.transform.position = chazObj.transform.position + Vector3.up * 1.5f;
		gradGifts.SetActive(value: true);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		confettiParticles.GetComponent<GvrAudioSource>().Stop();
		ParticleSystem[] componentsInChildren = confettiParticles.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			particleSystem.Stop();
		}
		confettiParticles.SetActive(value: false);
		chazDialogueSource.PlayClip(tweakables.dialogue.gFirstTask1);
		chazDialogueSource.PlayClip(tweakables.dialogue.gFirstTask2);
		chazDialogueSource.PlayClip(tweakables.dialogue.gFirstTask3);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		chazControl.PresentObject(toKitchenHeadset);
		float hurryTimer = 0f;
		bool hurryUpFired = false;
		while (!returnedFromKitchen)
		{
			if (!chazControl.isPresenting && !ToolManager.PlayerGrabbingGameObject(toKitchenHeadset))
			{
				if (hurryUpFired)
				{
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(5f, this);
					if (!returnedFromKitchen && !ToolManager.PlayerGrabbingGameObject(toKitchenHeadset))
					{
						chazControl.GrabObject(toKitchenHeadset);
					}
				}
				else
				{
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
					if (!returnedFromKitchen && !ToolManager.PlayerGrabbingGameObject(toKitchenHeadset))
					{
						chazControl.GrabObject(toKitchenHeadset);
						chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.gHurryUp);
						hurryUpFired = true;
					}
				}
			}
			hurryTimer += Time.deltaTime;
			if (!hurryUpFired && hurryTimer > 10f)
			{
				chazDialogueSource.PlayClip(tweakables.dialogue.gHurryUp);
				hurryUpFired = true;
			}
			yield return null;
		}
	}

	private IEnumerator retrieveUntilTrigger(GameObject toRetrieve, NPCDialogueClip clip, bool trigger)
	{
		bool dialogueTriggered = false;
		while (!trigger)
		{
			SafeDebug.Log("Retrieve coroutine iteration.");
			if (!chazControl.isPresenting && !ToolManager.PlayerGrabbingGameObject(toRetrieve))
			{
				if (dialogueTriggered)
				{
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(5f, this);
					if (!returnedFromDemo1 && !ToolManager.PlayerGrabbingGameObject(toRetrieve))
					{
						chazControl.GrabObject(headsets[0]);
					}
				}
				else
				{
					dialogueTriggered = true;
					yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
					chazDialogueSource.ForcePlayClipOverwrite(clip);
					if (!returnedFromDemo1 && !ToolManager.PlayerGrabbingGameObject(toRetrieve))
					{
						chazControl.GrabObject(toRetrieve);
					}
				}
			}
			yield return null;
		}
		SafeDebug.Log("Retrieve coroutine completed");
		yield return null;
	}

	private void reactToSceneActivation(EagerScene newActiveScene, EagerScene oldActiveScene)
	{
		if (newActiveScene.Scene == _tutorialScene && oldActiveScene != null)
		{
			if (oldActiveScene.Scene == _demoScene1)
			{
				returnedFromDemo1 = true;
			}
			else if (oldActiveScene.Scene == _demoScene2)
			{
				returnedFromDemo2 = true;
			}
			else if (oldActiveScene.Scene == _demoScene3)
			{
				returnedFromDemo3 = true;
			}
			else if (oldActiveScene.Scene == _demoScene4)
			{
				returnedFromDemo4 = true;
			}
			else
			{
				if (!(oldActiveScene.Scene == TCScenes.S.VVR_LVL_JOBKITCHEN))
				{
					return;
				}
				returnedFromKitchen = true;
				if (!toKitchenHeadset.activeInHierarchy)
				{
					toKitchenHeadset = EagerScenes.instance.lastHeadsetRemoved.gameObject;
				}
				ToolManager.AllManipulators.UserCanGrab = true;
				if (!userExitedButter)
				{
					chazControl.GrabObject(toKitchenHeadset);
					chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.eUserExitedButter1);
					userExitedButter = true;
					retrieveCoroutine = GlobalCoroutineContainer.AddThread(retrieve(), this);
					return;
				}
				if (retrieveCoroutine.IsRunning)
				{
					retrieveCoroutine.Stop();
				}
				chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.eUserExitedButter2);
				EagerHeadsetManager.SetCanRemoveHeadset(canRemove: false);
				retrieveCoroutine = GlobalCoroutineContainer.AddThread(headsetAttack(), this);
			}
		}
		else if (oldActiveScene == null && tutorialState == TutorialStates.Finished)
		{
			toKitchenHeadset.SetActive(value: true);
			chazControl.GrabObject(toKitchenHeadset);
			chazDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.gHurryUp);
		}
	}

	private IEnumerator retrieve()
	{
		float t = 3f;
		while (true)
		{
			if (!chazControl.isPresenting && !ToolManager.PlayerGrabbingGameObject(toKitchenHeadset))
			{
				t += Time.deltaTime;
				if (t >= 4f)
				{
					chazControl.GrabObject(toKitchenHeadset);
					t = 0f;
				}
			}
			else
			{
				t = 0f;
			}
			yield return null;
		}
	}

	private IEnumerator headsetAttack()
	{
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		if (!ToolManager.PlayerGrabbingGameObject(toKitchenHeadset, dropIfGrabbing: true) && chazControl.GetComponentInChildren<FixedJoint>() != null)
		{
			chazControl.GetComponentInChildren<FixedJoint>().breakForce = 10f;
			chazControl.GetComponentInChildren<FixedJoint>().breakTorque = 10f;
		}
		toKitchenHeadset.GetComponent<Headset>().SendToEagerHeadset();
	}

	private IEnumerator rotateControllerTeleport(float time)
	{
		Quaternion startRot = controller.transform.rotation;
		Quaternion endRot = Quaternion.Euler(teleportEuler);
		float timer = 0f;
		while (timer < 1f)
		{
			timer += Time.deltaTime / time;
			controller.transform.rotation = Quaternion.Slerp(startRot, endRot, timer);
			yield return null;
		}
		yield return null;
	}

	private IEnumerator rotateControllerGrab(float time)
	{
		Quaternion startRot = controller.transform.rotation;
		Quaternion endRot = Quaternion.Euler(grabEuler);
		float timer = 0f;
		while (timer < 1f)
		{
			timer += Time.deltaTime / time;
			controller.transform.rotation = Quaternion.Slerp(startRot, endRot, timer);
			yield return null;
		}
		yield return null;
	}

	private IEnumerator rotateToStartRot(float time)
	{
		Quaternion startRot = controller.transform.rotation;
		float timer = 0f;
		while (timer < 1f)
		{
			timer += Time.deltaTime / time;
			controller.transform.rotation = Quaternion.Slerp(startRot, startControllerRot, timer);
			yield return null;
		}
		yield return null;
	}

	public void AdvanceAndSaveTutorialState(TutorialStates newState)
	{
		tutorialState = newState;
		PersistentDataManager.OverwriteTutorialState(newState);
	}

	private void setGameObjectsEnabled(ref GameObject[] objList, bool enable)
	{
		for (int i = 0; i < objList.Length; i++)
		{
			objList[i].SetActive(enable);
		}
	}
}
