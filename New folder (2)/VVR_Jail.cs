using System.Collections;
using UnityEngine;

public class VVR_Jail : SceneBehaviorManager
{
	public static VVR_Jail instance;

	private VVR_HUB.JobStates jobState;

	[Header("References:")]
	[SerializeField]
	private ChazController chazController;

	[SerializeField]
	private GameObject nervousEyeShaker;

	[SerializeField]
	private VVRDialogueSource chazDialogueSource;

	[SerializeField]
	private JailScriptableObject tweakables;

	[Header("Blocking:")]
	public Transform start;

	public Transform downCenter;

	public Transform downRight;

	public Transform downLeft;

	[Space]
	public MaterialUpdateController lightProperties;

	[HideInInspector]
	public bool postHernandez;

	private TutorialLevelSequence.AestheticObjects aestheticObject;

	protected override void OnSceneActivated()
	{
		lightProperties.PushMaterials();
		GetComponent<EagerScene>().defaultMaterialProperties = lightProperties;
	}

	private void Awake()
	{
		if (instance != null)
		{
			throw new UnityException("Only one instance of VVR_Jail in a scene is allowed.");
		}
		instance = this;
	}

	protected override void OnFirstTimeSceneActivated()
	{
		jobState = PersistentDataManager.GetJobState();
		EagerHeadsetManager.SetCanRemoveHeadset(canRemove: false);
		PersistentDataManager.WriteSaveDelay(1f);
		if (jobState == VVR_HUB.JobStates.Macy && PersistentDataManager.GetSavedHubState() == VVR_HUB.HubStates.Intro)
		{
			JailIntro();
			return;
		}
		if (postHernandez)
		{
			VVR_Archive.DisarmChazCaptureSequence();
			VVR_HUB.instance.postHernandez = true;
			PersistentDataManager.OverwriteHernandezState(VVR_Hernandez.HernandezLevelState.done);
			Hernandez();
			return;
		}
		PersistentDataManager.IncrementBackstageCaughtCounter();
		SafeDebug.Log("Jail capture number " + PersistentDataManager.GetBackstageCaughtCounter());
		if (VVR_HUB.instance != null)
		{
			VVR_HUB.instance.wasInJail = true;
			VVR_HUB.instance.hasCompletedJob = true;
			VVR_HUB.instance.wasInBackstage = true;
		}
		switch (PersistentDataManager.GetBackstageCaughtCounter())
		{
		case 1:
			CaptureOne();
			return;
		case 2:
			CaptureTwo();
			return;
		case 3:
			CaptureThree();
			return;
		case 4:
			CaptureFour();
			return;
		case 5:
			CaptureFive();
			return;
		case 6:
			CaptureEleven();
			return;
		case 7:
			CaptureSeven();
			return;
		case 8:
			CaptureEight();
			return;
		case 9:
			CaptureNine();
			return;
		case 10:
			CaptureTen();
			return;
		case 11:
			CaptureEleven();
			return;
		case 12:
			CaptureTwelve();
			return;
		}
		switch (Random.Range(1, 5))
		{
		case 1:
			CaptureFour();
			break;
		case 2:
			CaptureSix();
			break;
		case 3:
			CaptureTen();
			break;
		case 4:
			CaptureEleven();
			break;
		}
	}

	private void JailIntro()
	{
		GlobalCoroutineContainer.AddThread(JailIntroCoroutine(), this);
	}

	private IEnumerator JailIntroCoroutine()
	{
		yield return null;
		EagerHeadsetManager.SetCanRemoveHeadset(canRemove: false);
		if (!EagerScenes.SceneIsLoaded(EagerScenes.HUB_LEVEL))
		{
			EagerScenes.instance.LoadQueue(EagerScenes.HUB_LEVEL);
			EagerScenes.instance.AddHeadsetAtIndex(EagerScenes.instance.CreateNewHeadset(EagerScenes.HUB_LEVEL), 0);
		}
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		chazDialogueSource.PlayClip(tweakables.dialogue.helloAgain);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(5f, this);
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		while (!EagerScenes.SceneIsLoaded(EagerScenes.HUB_LEVEL))
		{
			yield return null;
		}
		PersistentDataManager.OverwriteLastLoadedScene(EagerScenes.HUB_LEVEL);
		PersistentDataManager.OverwriteBackstageState(Backstage.BackstageStates.JobIntrusion);
		EagerScenes.instance.TakeOffAllHeadsets();
	}

	private void CaptureOne()
	{
		GlobalCoroutineContainer.AddThread(CaptureOneCoroutine(), this);
	}

	private IEnumerator CaptureOneCoroutine()
	{
		yield return null;
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.umBee);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(6f, this);
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 1.5f, ChazController.eyeColor.Angry, immediately: true);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		chazController.EyeLookAt(nervousEyeShaker.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.breakingJobCycle);
		chazController.WalkTo(downRight.position);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.isThisPersonal);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		chazController.EyeLookAt(nervousEyeShaker.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.honestlyIDK);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.uberSupportive);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.tooDifficult);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.entryLevel);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		chazController.EyeLookAt(nervousEyeShaker.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.reinventTheWheel);
		chazController.WalkTo(start.position);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.interestsYou);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.dontAnswerThat);
		chazController.ReStraighten();
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		ExitToHub();
	}

	private void CaptureTwo()
	{
		GlobalCoroutineContainer.AddThread(CaptureTwoCoroutine(), this);
	}

	private IEnumerator CaptureTwoCoroutine()
	{
		yield return null;
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.noNoNo);
		chazController.WalkTo(downCenter.position);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		ExitToHub();
	}

	private void CaptureThree()
	{
		GlobalCoroutineContainer.AddThread(CaptureThreeCoroutine(), this);
	}

	private IEnumerator CaptureThreeCoroutine()
	{
		yield return null;
		chazController.EyeLookAt(nervousEyeShaker.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.iveBeenThining);
		aestheticObject = PersistentDataManager.GetAestheticObject();
		if (aestheticObject == TutorialLevelSequence.AestheticObjects.Plant)
		{
			chazDialogueSource.PlayClip(tweakables.dialogue.aFish);
		}
		else
		{
			chazDialogueSource.PlayClip(tweakables.dialogue.aPlant);
		}
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		ExitToHub();
	}

	private void CaptureFour()
	{
		GlobalCoroutineContainer.AddThread(CaptureFourCoroutine(), this);
	}

	private IEnumerator CaptureFourCoroutine()
	{
		yield return null;
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.capture4);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(4f, this);
		chazController.WalkTo(downLeft.position);
		chazController.EyeLookAt(downLeft, 0f, ChazController.eyeColor.Default, immediately: true);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		ExitToHub();
	}

	private void CaptureFive()
	{
		GlobalCoroutineContainer.AddThread(CaptureFiveCoroutine(), this);
	}

	private IEnumerator CaptureFiveCoroutine()
	{
		yield return null;
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.capture5);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
		chazController.ReStraighten();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
		chazController.ReStraighten();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.7f, this);
		chazController.ReStraighten();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.7f, this);
		chazController.ReStraighten();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2.5f, this);
		chazController.WalkTo(downCenter.position);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		ExitToHub();
	}

	private void CaptureSix()
	{
		GlobalCoroutineContainer.AddThread(CaptureSixCoroutine(), this);
	}

	private IEnumerator CaptureSixCoroutine()
	{
		yield return null;
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.capture6);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(4.5f, this);
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 1f, ChazController.eyeColor.Angry, immediately: true);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		ExitToHub();
	}

	private void CaptureSeven()
	{
		GlobalCoroutineContainer.AddThread(CaptureSevenCoroutine(), this);
	}

	private IEnumerator CaptureSevenCoroutine()
	{
		yield return null;
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.capture7);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(4f, this);
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 1f, ChazController.eyeColor.Angry, immediately: true);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		ExitToHub();
	}

	private void CaptureEight()
	{
		GlobalCoroutineContainer.AddThread(CaptureEightCoroutine(), this);
	}

	private IEnumerator CaptureEightCoroutine()
	{
		yield return null;
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.capture8);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		chazController.WalkTo(downRight.position);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(5f, this);
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 1f, ChazController.eyeColor.Angry, immediately: true);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		ExitToHub();
	}

	private void CaptureNine()
	{
		GlobalCoroutineContainer.AddThread(CaptureNineCoroutine(), this);
	}

	private IEnumerator CaptureNineCoroutine()
	{
		yield return null;
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.capture9);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 3f, ChazController.eyeColor.Angry, immediately: true);
		chazController.ReStraighten();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		chazController.ReStraighten();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		chazController.ReStraighten();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(4f, this);
		chazController.WalkTo(downCenter.position);
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 4f, ChazController.eyeColor.Angry, immediately: true);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		ExitToHub();
	}

	private void CaptureTen()
	{
		GlobalCoroutineContainer.AddThread(CaptureTenCoroutine(), this);
	}

	private IEnumerator CaptureTenCoroutine()
	{
		yield return null;
		chazController.EyeLookAt(nervousEyeShaker.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.capture10);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		chazController.WalkTo(downLeft.position);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		ExitToHub();
	}

	private void CaptureEleven()
	{
		GlobalCoroutineContainer.AddThread(CaptureElevenCoroutine(), this);
	}

	private IEnumerator CaptureElevenCoroutine()
	{
		yield return null;
		chazController.WalkTo(downCenter.position);
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.capture11);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		ExitToHub();
	}

	private void CaptureTwelve()
	{
		GlobalCoroutineContainer.AddThread(CaptureTwelveCoroutine(), this);
	}

	private IEnumerator CaptureTwelveCoroutine()
	{
		yield return null;
		chazController.EyeLookAt(nervousEyeShaker.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		chazDialogueSource.PlayClip(tweakables.dialogue.capture12);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(6f, this);
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(6.5f, this);
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 2f, ChazController.eyeColor.Angry, immediately: true);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		ExitToHub();
	}

	private void Hernandez()
	{
		GlobalCoroutineContainer.AddThread(HernandezCoroutine(), this);
	}

	private IEnumerator HernandezCoroutine()
	{
		yield return null;
		chazDialogueSource.PlayClip(tweakables.dialogue.iMeanBig);
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		chazDialogueSource.PlayClip(tweakables.dialogue.forHisOwnGood);
		chazController.EyeLookAt(nervousEyeShaker.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		chazDialogueSource.PlayClip(tweakables.dialogue.nevermind);
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		ExitToHub();
	}

	private void ExitToHub()
	{
		GlobalCoroutineContainer.AddThread(ExitToHubCoroutine(), this);
	}

	private IEnumerator ExitToHubCoroutine()
	{
		if (!EagerScenes.SceneIsLoaded(EagerScenes.HUB_LEVEL))
		{
			EagerScenes.instance.LoadQueue(EagerScenes.HUB_LEVEL);
			EagerScenes.instance.AddHeadsetAtIndex(EagerScenes.instance.CreateNewHeadset(EagerScenes.HUB_LEVEL), 0);
		}
		while (!EagerScenes.SceneIsLoaded(EagerScenes.HUB_LEVEL))
		{
			yield return null;
		}
		VVR_HUB.instance.wasInJail = true;
		VVR_HUB.instance.hasCompletedJob = true;
		VVR_HUB.instance.wasInBackstage = true;
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		PersistentDataManager.OverwriteLastLoadedScene(EagerScenes.HUB_LEVEL);
		EagerScenes.instance.TakeOffAllHeadsets();
	}
}
