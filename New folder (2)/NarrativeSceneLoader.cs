using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using UnityEngine;

public class NarrativeSceneLoader : MonoBehaviour
{
	[SerializeField]
	private ObjectToggleGroup mToggleGroup;

	[SerializeField]
	private TextSwapper sceneLabelTitle;

	[SerializeField]
	private TextSwapper sceneLabelHeader;

	[SerializeField]
	private TextSwapper sceneLabelBody;

	[SerializeField]
	private ObjectButton goButton;

	[SerializeField]
	private Transform goButtonParent;

	[SerializeField]
	private GameObject hrMurderSkipButton;

	[SerializeField]
	private GameObject goButtonOn;

	[SerializeField]
	private GameObject goButtonOff;

	[SerializeField]
	private GvrAudioSource resetSound;

	[SerializeField]
	private GvrAudioSource buttonSound;

	private SafeCoroutine resetThread;

	private VVR_SAVEDATA currentSave;

	private void Start()
	{
		goButton.onClickEvent.AddListener(reloadReset);
		goButtonOff.SetActive(value: true);
		goButtonOn.SetActive(value: false);
	}

	public void SetUpNarrativeControlsByContext()
	{
		if (!(hrMurderSkipButton != null) || HRMurder.instance != null)
		{
		}
		goButtonOff.SetActive(value: true);
		goButtonOn.SetActive(value: false);
		LoadCurrentSaveState();
	}

	public void ClearState()
	{
		mToggleGroup.ClearState();
	}

	private void reloadReset()
	{
		if (resetThread == null)
		{
			resetThread = GlobalCoroutineContainer.AddThread(reloadSequence(), this);
		}
	}

	private IEnumerator reloadSequence()
	{
		yield return null;
		resetSound.Play();
		buttonSound.Play();
		goButtonOff.SetActive(value: true);
		goButtonOn.SetActive(value: false);
		PersistentDataManager.ReplaceActiveSaveData(currentSave);
		EagerScenes.ResetAndSkipMenu();
	}

	public void OverwriteSaveWithButton(VVR_SAVEDATA saveIn, NarrativeToggleButton toggleIn, bool playerTriggered)
	{
		saveIn.WriteMissingDictionaryValues(PersistentDataManager.GetBeeslot());
		bool hasAdmin = PersistentDataManager.GetHasAdmin();
		TutorialLevelSequence.AestheticObjects aestheticObject = PersistentDataManager.GetAestheticObject();
		List<string> playedCoroutines = PersistentDataManager.GetPlayedCoroutines();
		string versionNumber = PersistentDataManager.GetVersionNumber();
		string username = PersistentDataManager.GetUsername();
		string lastTimePlayedString = PersistentDataManager.GetLastTimePlayedString();
		int beeslot = PersistentDataManager.GetBeeslot();
		currentSave = saveIn;
		currentSave.hasAdminAccess = hasAdmin;
		currentSave.aestheticObject = aestheticObject;
		currentSave.playedCoroutines = playedCoroutines;
		currentSave.arbitraryStringDictionary["VERSION NUMBER"] = versionNumber;
		currentSave.arbitraryStringDictionary["USERNAME"] = username;
		currentSave.arbitraryStringDictionary["LASTPLAYED"] = lastTimePlayedString;
		currentSave.arbitraryIntDictionary["BEE SLOT"] = beeslot;
		sceneLabelTitle.SetNewKey(toggleIn.label);
		sceneLabelHeader.SetNewKey(toggleIn.subTitle);
		sceneLabelBody.SetNewKey(toggleIn.description);
		if (playerTriggered)
		{
			activateGoButton();
		}
	}

	private void activateGoButton()
	{
		goButtonOff.SetActive(value: false);
		goButtonOn.SetActive(value: true);
	}

	private void LoadCurrentSaveState()
	{
		mToggleGroup.ClearState();
		if (PersistentDataManager.GetSavedTutorialState() != TutorialLevelSequence.TutorialStates.Finished)
		{
			mToggleGroup.ToggleAtIndex(0);
		}
		else if (PersistentDataManager.GetSavedHubState() == VVR_HUB.HubStates.Intro && PersistentDataManager.GetLastLoadedScene() == TCScenes.S.VVR_LVL_JOBKITCHEN)
		{
			mToggleGroup.ToggleAtIndex(1);
		}
		else if (PersistentDataManager.GetSavedHubState() == VVR_HUB.HubStates.Intro && PersistentDataManager.GetLastLoadedScene() == TCScenes.S.VVR_LVL__BACKSTAGE)
		{
			mToggleGroup.ToggleAtIndex(2);
		}
		else if (PersistentDataManager.GetSavedHubState() == VVR_HUB.HubStates.Intro)
		{
			mToggleGroup.ToggleAtIndex(3);
		}
		else if (PersistentDataManager.GetJobState() == VVR_HUB.JobStates.Garden)
		{
			mToggleGroup.ToggleAtIndex(4);
		}
		else if (PersistentDataManager.GetJobState() == VVR_HUB.JobStates.Sunset)
		{
			mToggleGroup.ToggleAtIndex(5);
		}
		else if (PersistentDataManager.GetJobState() == VVR_HUB.JobStates.Tumbleweed1)
		{
			mToggleGroup.ToggleAtIndex(6);
		}
		else if (PersistentDataManager.GetJobState() == VVR_HUB.JobStates.Union2Vacuum)
		{
			mToggleGroup.ToggleAtIndex(7);
		}
		else if (!PersistentDataManager.GetHasBeenToArchives() && PersistentDataManager.GetElevatorData().keyState == ElevatorSaveData.KeyStates.Unlocked)
		{
			mToggleGroup.ToggleAtIndex(12);
		}
		else if (PersistentDataManager.GetHasBeenToArchives() && PersistentDataManager.GetArchiveState() == VVR_Archive.ArchiveStates.Bernice)
		{
			mToggleGroup.ToggleAtIndex(13);
		}
		else if (PersistentDataManager.GetArchiveState() == VVR_Archive.ArchiveStates.Hernandez)
		{
			mToggleGroup.ToggleAtIndex(14);
		}
		else if (PersistentDataManager.GetArchiveState() == VVR_Archive.ArchiveStates.Eliza)
		{
			mToggleGroup.ToggleAtIndex(15);
		}
		else if (PersistentDataManager.GetArchiveState() == VVR_Archive.ArchiveStates.ArchivesFinished && (PersistentDataManager.GetLastLoadedScene() == EagerScenes.BACKSTAGE_LEVEL || PersistentDataManager.GetLastLoadedScene() == EagerScenes.HUB_LEVEL))
		{
			mToggleGroup.ToggleAtIndex(16);
		}
		else if (PersistentDataManager.GetLastLoadedScene() == TCScenes.S.VVR_ACTIVITUDE_HR)
		{
			mToggleGroup.ToggleAtIndex(17);
		}
		else if (PersistentDataManager.GetLastLoadedScene() == TCScenes.S.VVR_ENDING_MERGE || VVR_SceneSelect.instance.isJoinEnding)
		{
			mToggleGroup.ToggleAtIndex(18);
		}
		else if (PersistentDataManager.GetLastLoadedScene() == TCScenes.S.VVR_ENDING_MURDER || VVR_SceneSelect.instance.isMurderEnding)
		{
			mToggleGroup.ToggleAtIndex(19);
		}
		else if (PersistentDataManager.GetLastLoadedScene() == TCScenes.S.VVR_UNDERHUB_01_SCENESELECT)
		{
			mToggleGroup.ToggleAtIndex(18);
		}
		else if (PersistentDataManager.GetJobState() == VVR_HUB.JobStates.ButterSnack)
		{
			mToggleGroup.ToggleAtIndex(8);
		}
		else if (PersistentDataManager.GetJobState() == VVR_HUB.JobStates.GardenNight)
		{
			mToggleGroup.ToggleAtIndex(9);
		}
		else if (PersistentDataManager.GetJobState() == VVR_HUB.JobStates.Tumbleweed2)
		{
			mToggleGroup.ToggleAtIndex(10);
		}
		else if (PersistentDataManager.GetJobState() == VVR_HUB.JobStates.LOOP)
		{
			mToggleGroup.ToggleAtIndex(11);
		}
		else if (PersistentDataManager.GetJobState() == VVR_HUB.JobStates.Macy)
		{
			mToggleGroup.ToggleAtIndex(21);
		}
		else if (PersistentDataManager.GetJobState() == VVR_HUB.JobStates.Tina)
		{
			mToggleGroup.ToggleAtIndex(20);
		}
	}
}
