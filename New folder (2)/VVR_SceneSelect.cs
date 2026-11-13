using TMPro;
using UnityEngine;

public class VVR_SceneSelect : SceneBehaviorManager
{
	public static VVR_SceneSelect instance;

	public bool isJoinEnding;

	public bool isMurderEnding;

	private bool hasDetectedGameEndMode;

	[SerializeField]
	private Transform usabilitySpawnPoint;

	[SerializeField]
	private EndingSurvey survey;

	[SerializeField]
	private Headset realDaydream;

	[SerializeField]
	private GameObject gameOverNote;

	[SerializeField]
	private NarrativeSceneLoader loader;

	[SerializeField]
	private UnderhubHeadsetWrangler grabberControlRoom;

	[SerializeField]
	private TextMeshPro versionNumber;

	private bool cachedCanTeleport;

	private bool cachedCanGrab;

	private bool cachedCanDrop;

	private bool cachedCanScroll;

	private void Awake()
	{
		if (instance != null)
		{
			throw new UnityException("Only one instance of VVR_SceneSelect in a scene is allowed.");
		}
		instance = this;
	}

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		versionNumber.text = "v. " + Application.version;
		survey.SetupSurveyRoom();
		EagerScenes.instance.OnSceneActivated += manageCachedRestrictions;
	}

	protected override void OnSceneActivated()
	{
		base.OnSceneActivated();
		ToolManager.ToggleManipulator(toggle: true);
		ToolManager.AllTeleporters.TeleportEnabled = true;
		if (VVR_HUB.instance != null)
		{
			VVR_HUB.instance.ignoreNextReentry = true;
		}
		SetupControlRoom();
	}

	private void manageCachedRestrictions(EagerScene newScene, EagerScene oldScene)
	{
		if (oldScene != null)
		{
			if (newScene.Scene == TCScenes.S.VVR_UNDERHUB_01_SCENESELECT && !oldScene.adminScene)
			{
				cachedCanTeleport = Teleporter.GetTeleportEnabled();
				cachedCanGrab = ToolManager.AllManipulators.UserCanGrab;
				cachedCanDrop = ToolManager.AllManipulators.UserCanDrop;
				cachedCanScroll = ToolManager.AllManipulators.UserCanScroll;
				Teleporter.SetTeleportEnabled(enabledIn: true);
				ToolManager.AllManipulators.UserCanDrop = true;
				ToolManager.AllManipulators.UserCanGrab = true;
				ToolManager.AllManipulators.UserCanScroll = true;
			}
			else if (oldScene.Scene == TCScenes.S.VVR_UNDERHUB_01_SCENESELECT && !newScene.adminScene)
			{
				Teleporter.SetTeleportEnabled(cachedCanTeleport);
				ToolManager.AllManipulators.UserCanDrop = cachedCanDrop;
				ToolManager.AllManipulators.UserCanGrab = cachedCanGrab;
				ToolManager.AllManipulators.UserCanScroll = cachedCanScroll;
			}
		}
	}

	private void SetupControlRoom()
	{
		if (EagerScenes.instance.lastHeadsetRemoved != null && !hasDetectedGameEndMode)
		{
			if (EagerScenes.instance.lastHeadsetRemoved.Scene == TCScenes.S.VVR_ENDING_MERGE)
			{
				isJoinEnding = true;
				isMurderEnding = false;
				hasDetectedGameEndMode = true;
			}
			else if (EagerScenes.instance.lastHeadsetRemoved.Scene == TCScenes.S.VVR_ENDING_MURDER && HRMurder.instance != null && HRMurder.instance.murderSceneComplete)
			{
				isJoinEnding = false;
				isMurderEnding = true;
				hasDetectedGameEndMode = true;
			}
			else
			{
				isJoinEnding = false;
				isMurderEnding = false;
			}
			if (hasDetectedGameEndMode)
			{
				PersistentDataManager.OverwriteLastLoadedScene(TCScenes.S.VVR_UNDERHUB_01_SCENESELECT);
				PersistentDataManager.SetUseHacktivitudeMenu(useHacktivitude: true);
				gameOverNote.SetActive(value: true);
				grabberControlRoom.enabled = false;
				survey.isJoinEnding = isJoinEnding;
				survey.isMurderEnding = isMurderEnding;
				survey.SetupSurveyRoom();
				EagerScenes.instance.DestroyAllScenesExcept(TCScenes.S.VVR_UNDERHUB_01_SCENESELECT);
				realDaydream.isActivated = false;
				EagerMaster.instance.player.transform.position = usabilitySpawnPoint.position;
				ToolManager.ToggleManipulator(toggle: true);
				EagerHeadsetManager.SetLockPlayerInScene(lockPlayer: false);
			}
		}
		else if (EagerScenes.instance.lastHeadsetRemoved == null)
		{
			survey.SetupSurveyRoom();
			realDaydream.isActivated = false;
		}
		if (EagerScenes.instance.lastHeadsetRemoved != null && realDaydream != EagerScenes.instance.lastHeadsetRemoved && EagerScenes.instance.SceneSequence.IndexOf(EagerScenes.instance.lastHeadsetRemoved.Scene) == -1)
		{
			realDaydream.spawnPos = EagerScenes.instance.lastHeadsetRemoved.spawnPos;
			realDaydream.Scene = EagerScenes.instance.lastHeadsetRemoved.Scene;
			realDaydream.spawnScale = EagerScenes.instance.lastHeadsetRemoved.spawnScale;
			realDaydream.transform.position = EagerScenes.instance.lastHeadsetRemoved.transform.position;
			realDaydream.transform.rotation = EagerScenes.instance.lastHeadsetRemoved.transform.rotation;
			realDaydream.hasBeenUsed = true;
			EagerScenes.instance.lastHeadsetRemoved.gameObject.SetActive(value: false);
			EagerScenes.instance.lastHeadsetRemoved = realDaydream;
			realDaydream.gameObject.SetActive(value: true);
		}
		loader.SetUpNarrativeControlsByContext();
		if (!hasDetectedGameEndMode)
		{
			grabberControlRoom.ReactToSceneActivated();
		}
	}
}
