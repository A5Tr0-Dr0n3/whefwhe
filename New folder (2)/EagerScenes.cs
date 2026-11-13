using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Edelweiss.Coroutine;
using GVR.Gfx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EagerScenes : MonoBehaviour
{
	public delegate void SceneActivatedEventHandler(EagerScene newActiveScene, EagerScene FromScene);

	private enum PupilOutOrFadeOut
	{
		pupilOut,
		fadeOut
	}

	private TCPlatformScene[] _BACKING_underhubSceneStack;

	public static EagerScenes instance;

	private static bool _skipPreloadMenu;

	public EagerScene ActiveScene;

	public List<TCPlatformScene> SceneSequence = new List<TCPlatformScene>();

	public List<Headset> headsets = new List<Headset>();

	public Headset lastHeadsetRemoved;

	public Headset activeHeadset;

	public bool rootLoaded;

	private Transform _worldRoot;

	private List<EagerScene> _loadedEagerScenes = new List<EagerScene>();

	private List<TCPlatformScene> _loadedScenes = new List<TCPlatformScene>();

	private List<TCPlatformScene> _unloadedScenes = new List<TCPlatformScene>();

	private List<TCPlatformScene> _loadQueueList = new List<TCPlatformScene>();

	private Dictionary<TCPlatformScene, AsyncOperation> _loadedScenesSuppressed = new Dictionary<TCPlatformScene, AsyncOperation>();

	private bool _suppressLoadedSceneActivation;

	private bool _isClearingMemory;

	private AsyncOperation _loadSceneAsyncOperation;

	private AsyncOperation _aoDelete;

	private TCPlatformScene _nextScene;

	private int _lastSceneIndex;

	private bool _isLoadingScenes;

	public string _rootLoadedName;

	public PupilTransitionController pauseMenu;

	public StartupManager startupManager;

	public GameObject headsetPrefab;

	public ControllerConnectivityChecker controllerChecker;

	private Coroutine _loadCoroutine;

	private Headset _baseHeadset;

	private int _loadSceneBarCount = 1;

	private Coroutine _pauseThread;

	public GraphicsSelectionMenu graphicsSettings;

	private TCPlatformScene[] _scenesUnderStart;

	private TCPlatformScene _startingScene;

	private List<TCPlatformScene> _startingSceneStack = new List<TCPlatformScene>();

	private Coroutine loadQueueRef;

	private Vector3 playerDisplacement;

	private EagerScene oldScene;

	private Coroutine cleanUpCoroutine;

	private List<EagerScene> newScenes;

	private EagerScene newSceneObject;

	private WaitForSeconds bootingTime = new WaitForSeconds(0.5f);

	private Coroutine removeHeadsetsCoroutine;

	private TCPlatformScene[] UNDERHUB_SCENE_STACK
	{
		get
		{
			if (_BACKING_underhubSceneStack == null)
			{
				_BACKING_underhubSceneStack = new TCPlatformScene[5]
				{
					TCScenes.S.VVR_UNDERHUB_01_SCENESELECT,
					TCScenes.S.VVR_UNDERHUB_03_FAKEDROID,
					TCScenes.S.VVR_UNDERHUB_04_FAKEDEBUG,
					TCScenes.S.VVR_UNDERHUB_06_TENDEROFFICE_CREDITS,
					TCScenes.S.VVR_UNDERHUB_07_INFINITETUMBALL
				};
			}
			return _BACKING_underhubSceneStack;
		}
	}

	public static TCPlatformScene HUB_LEVEL => TCScenes.S.VVR_LVL__HUB;

	public static TCPlatformScene JAIL_LEVEL => TCScenes.S.VVR_LVL_JAIL;

	public static TCPlatformScene BACKSTAGE_LEVEL => TCScenes.S.VVR_LVL__BACKSTAGE;

	public static TCPlatformScene LOGIN_LEVEL => TCScenes.S.VVR_LVL_LOGIN;

	public event SceneActivatedEventHandler OnSceneActivated;

	public static void LogOut()
	{
		_skipPreloadMenu = false;
		instance.StartCoroutine(instance.reloadToStartupSequence(PupilOutOrFadeOut.fadeOut));
	}

	public static void ResetAndSkipMenu()
	{
		_skipPreloadMenu = true;
		instance.StartCoroutine(instance.reloadToStartupSequence(PupilOutOrFadeOut.fadeOut));
	}

	private IEnumerator reloadToStartupSequence(PupilOutOrFadeOut transition)
	{
		ToolManager.ToggleManipulator(toggle: false);
		if (transition == PupilOutOrFadeOut.pupilOut)
		{
			yield return pauseMenu.AnimateEyes(openEyes: false);
		}
		else
		{
			float timer = 0f;
			while (true)
			{
				float num;
				timer = (num = timer + Time.deltaTime);
				if (!(num < 0.6f))
				{
					break;
				}
				FadeBubbleTransitionController.SetFadeT(Mathf.Clamp01(timer / 0.6f));
				yield return null;
			}
		}
		instance = null;
		SceneManager.LoadScene(TCScenes.S.VVR_ROOT.Name, LoadSceneMode.Single);
	}

	private void Awake()
	{
		if (instance != null)
		{
			throw new UnityException("Only one instance of EagerScenes in a scene is allowed.");
		}
		instance = this;
		_worldRoot = transform;
		if (SceneManager.GetActiveScene().isLoaded && SceneManager.GetActiveScene().GetRootGameObjects().ToList()
			.Any((GameObject g) => g.GetComponent<EagerScene>() != null))
		{
			if (SaveLoadUtility.GetAllAccountSaves().Length <= 0)
			{
				PersistentDataManager.CreateNewSceneAccount(SceneManager.GetActiveScene().name != LOGIN_LEVEL.Name);
			}
			else
			{
				PersistentDataManager.SetActiveSaveSlot(0);
			}
		}
	}

	private void Start()
	{
		FadeBubbleTransitionController.MakeFullyOpaque();
		StartCoroutine(SetupWorld());
	}

	private IEnumerator SetupWorld()
	{
		rootLoaded = true;
		StartCoroutine(RootInitialLoad());
		yield break;
	}

	private IEnumerator RootInitialLoad()
	{
		if (!_skipPreloadMenu)
		{
			startupManager.InitializeAsLoginMenu();
			if (controllerChecker != null)
			{
				controllerChecker.EnableObjectEnabler();
			}
			while (!startupManager.userHasLeftMenu)
			{
				yield return null;
			}
		}
		else
		{
			if (controllerChecker != null)
			{
				controllerChecker.DisableObjectEnabler();
			}
			graphicsSettings.SetGraphicsMode(PersistentDataManager.GetGraphicsMode());
			startupManager.InitializeAsInstantLoader();
			yield return null;
		}
		_skipPreloadMenu = false;
		_startingScene = PersistentDataManager.GetLastLoadedScene();
		_startingSceneStack.Add(_startingScene);
		_scenesUnderStart = GetScenesUnderFirstLoaded(_startingScene);
		_suppressLoadedSceneActivation = true;
		for (int num = _scenesUnderStart.Length - 1; num >= 0; num--)
		{
			_startingSceneStack.Insert(0, _scenesUnderStart[num]);
		}
		for (int i = 0; i < _startingSceneStack.Count; i++)
		{
			LoadQueue(_startingSceneStack[i]);
		}
		LoadQueue(JAIL_LEVEL);
		calibrateLoadProgressCalculator();
		yield return StartCoroutine(startupManager.LoadingBarTransition());
		while (_isLoadingScenes)
		{
			yield return null;
		}
		for (int j = 0; j < _startingSceneStack.Count; j++)
		{
			Headset headset = CreateNewHeadset(_startingSceneStack[j]);
			if (_startingSceneStack[j] == TCScenes.S.VVR_UNDERHUB_01_SCENESELECT)
			{
				headset.spawnPos = new Vector3(1.6f, 0f, -3f);
				headset.hasBeenUsed = true;
			}
			if (j == 0)
			{
				_baseHeadset = headset;
			}
			activeHeadset = headset;
			headsets.Add(headset);
			SceneSequence.Add(_startingSceneStack[j]);
		}
		yield return StartCoroutine(startupManager.ExitPreloadTransition());
		PersistentDataManager.SetUseHacktivitudeMenu(useHacktivitude: false);
		yield return null;
		if (_suppressLoadedSceneActivation)
		{
			_suppressLoadedSceneActivation = false;
			yield return ActivateSuppressedScenesRoutine();
		}
		SetActiveScene(SceneToEagerScene(_startingScene));
		MovePlayerSafe(ActiveScene.playerPos, headAdjust: false, ActiveScene.setHeadPos);
		activeHeadset.hasBeenUsed = true;
		CheckLoadUnderhubScenes();
		startupManager.Disable();
		yield return new WaitForSeconds(1f);
		instance.pauseMenu.AnimateEyes(openEyes: true);
		yield return new WaitForSeconds(0.75f);
		ToolManager.ToggleManipulator(toggle: true);
		ToolManager.AllCursorReticles.HideCursor = false;
		CharacterFlip.SetCanFlip(newValue: true);
	}

	private TCPlatformScene[] GetScenesUnderFirstLoaded(TCPlatformScene levelIn)
	{
		if (levelIn == TCScenes.S.VVR_LVL__BACKSTAGE)
		{
			return new TCPlatformScene[1] { TCScenes.S.VVR_LVL__HUB };
		}
		if (levelIn == TCScenes.S.VVR_LVL__HUB)
		{
			if (PersistentDataManager.GetHasAdmin())
			{
				return new TCPlatformScene[1] { TCScenes.S.VVR_UNDERHUB_01_SCENESELECT };
			}
			return new TCPlatformScene[0];
		}
		if (levelIn == TCScenes.S.VVR_LVL_JOBKITCHEN)
		{
			return new TCPlatformScene[1] { TCScenes.S.VVR_LVL_LOGIN };
		}
		if (levelIn == TCScenes.S.VVR_LVL_LOGIN)
		{
			return new TCPlatformScene[0];
		}
		return new TCPlatformScene[0];
	}

	public void LoadQueue(TCPlatformScene scene)
	{
		SafeDebug.Log("LoadQueue called on " + scene.Name);
		if (!_isClearingMemory && !SceneIsLoaded(scene) && !_loadQueueList.Contains(scene))
		{
			_isLoadingScenes = true;
			SafeDebug.Log("Added " + scene.Name + " to loadQueueList");
			_loadQueueList.Add(scene);
			if (loadQueueRef == null)
			{
				loadQueueRef = StartCoroutine(LoadQueueCoroutine());
			}
		}
	}

	private IEnumerator LoadQueueCoroutine()
	{
		while (_loadQueueList.Count > 0)
		{
			_isLoadingScenes = true;
			if ((!_suppressLoadedSceneActivation && SceneIsLoaded(_loadQueueList[0])) || (_suppressLoadedSceneActivation && _loadedScenesSuppressed.ContainsKey(_loadQueueList[0])))
			{
				SafeDebug.Log(string.Concat("Loaded scene ", _loadQueueList[0], "; ", _loadQueueList.Count - 1, " remain."));
				_loadQueueList.RemoveAt(0);
			}
			else
			{
				_nextScene = _loadQueueList[0];
				yield return StartCoroutine(AsyncLoadAdditiveRoutine(_nextScene));
			}
			yield return null;
		}
		SafeDebug.Log("All Scenes Loaded");
		_isLoadingScenes = false;
		loadQueueRef = null;
	}

	private IEnumerator ActivateSuppressedScenesRoutine()
	{
		TCPlatformScene[] scenes = _loadedScenesSuppressed.Keys.ToArray();
		for (int i = 0; i < scenes.Length; i++)
		{
			_loadedScenesSuppressed[scenes[i]].allowSceneActivation = true;
			yield return StartCoroutine(MoveLoadedObjectsToActiveScene(SceneManager.GetSceneByName(scenes[i].Name)));
		}
		_loadedScenesSuppressed.Clear();
	}

	private IEnumerator AsyncLoadAdditiveRoutine(TCPlatformScene scene)
	{
		SafeDebug.Log("Beginning Async Load on scene " + scene.Name);
		_loadSceneAsyncOperation = SceneManager.LoadSceneAsync(scene.Name, LoadSceneMode.Additive);
		_loadSceneAsyncOperation.allowSceneActivation = false;
		while (_loadSceneAsyncOperation.progress < 0.9f)
		{
			yield return null;
		}
		if (!_suppressLoadedSceneActivation)
		{
			_loadSceneAsyncOperation.allowSceneActivation = true;
			yield return StartCoroutine(MoveLoadedObjectsToActiveScene(SceneManager.GetSceneByName(_nextScene.Name)));
		}
		else
		{
			_loadedScenesSuppressed.Add(scene, _loadSceneAsyncOperation);
		}
	}

	private IEnumerator MoveLoadedObjectsToActiveScene(Scene sceneIn)
	{
		while (!sceneIn.isLoaded)
		{
			yield return null;
		}
		GameObject[] rootGameObjects = sceneIn.GetRootGameObjects();
		foreach (GameObject gameObject in rootGameObjects)
		{
			EagerScene component = gameObject.GetComponent<EagerScene>();
			if ((bool)component)
			{
				SafeDebug.Log("LOADED SCENE: " + component.Scene.Name);
				_loadedEagerScenes.Add(component);
				_loadedScenes.Add(component.Scene);
				SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
				gameObject.transform.parent = _worldRoot;
				gameObject.SetActive(value: false);
			}
		}
	}

	public static void CheckLoadUnderhubScenes()
	{
		if (PersistentDataManager.GetHasAdmin())
		{
			instance.LoadScenesUnderneathCurrentScene(instance.UNDERHUB_SCENE_STACK);
		}
	}

	public SafeCoroutine LoadScenesUnderneathCurrentScene(TCPlatformScene[] scenesIn, bool allowDuplicates = false)
	{
		return GlobalCoroutineContainer.AddThread(loadScenesBelowRoot(scenesIn, allowDuplicates), this);
	}

	private void calibrateLoadProgressCalculator()
	{
		_loadSceneBarCount = _loadQueueList.Count;
	}

	public float GetLoadProgress(AnimationCurve smoothProgressCurve = null)
	{
		float num = _loadSceneBarCount - _loadQueueList.Count;
		if (_loadSceneAsyncOperation != null && _loadSceneAsyncOperation.progress < 0.9f)
		{
			num = ((smoothProgressCurve != null) ? (num + smoothProgressCurve.Evaluate(_loadSceneAsyncOperation.progress)) : (num + _loadSceneAsyncOperation.progress));
		}
		return num / (float)_loadSceneBarCount;
	}

	public bool GetIsLoading()
	{
		return _isLoadingScenes;
	}

	private IEnumerator loadScenesBelowRoot(TCPlatformScene[] scenesIn, bool allowDuplicates)
	{
		for (int i = 0; i < scenesIn.Length; i++)
		{
			LoadQueue(scenesIn[i]);
		}
		while (_isLoadingScenes)
		{
			yield return null;
		}
		for (int j = 0; j < scenesIn.Length; j++)
		{
			if (!allowDuplicates && SceneSequence.Contains(scenesIn[j]))
			{
				SafeDebug.Log("Skippingalready loaded scene");
				continue;
			}
			Headset headset = CreateNewHeadset(scenesIn[j]);
			headset.spawnPos = SceneToEagerScene(scenesIn[j]).playerPos;
			headset.hasBeenUsed = true;
			AddHeadsetAtIndex(headset, 0, forceAboveAdmin: false);
		}
	}

	public void GoToScene(TCPlatformScene scene, bool puttingOnHeadset)
	{
		if (_isClearingMemory)
		{
			return;
		}
		if (_loadedScenes.Contains(scene))
		{
			if (puttingOnHeadset)
			{
				if (headsets.Count > 0)
				{
					headsets[headsets.Count - 1].spawnPos = EagerMaster.instance.player.transform.position;
				}
				headsets.Add(activeHeadset);
				SceneSequence.Add(scene);
				PlayerScaler.instance.ScaleHead(Vector3.one * activeHeadset.spawnScale);
				if (activeHeadset.hasBeenUsed)
				{
					MovePlayerSafe(activeHeadset.spawnPos, headAdjust: true);
				}
				else
				{
					EagerMaster.instance.player.transform.rotation = SceneToEagerScene(scene).playerRot;
					MovePlayerSafe(SceneToEagerScene(scene).playerPos, headAdjust: true, SceneToEagerScene(scene).setHeadPos);
					activeHeadset.hasBeenUsed = true;
				}
			}
			else
			{
				lastHeadsetRemoved = headsets[headsets.Count - 1];
				headsets[headsets.Count - 1].spawnPos = EagerMaster.instance.player.transform.position;
				headsets.RemoveAt(headsets.Count - 1);
				MovePlayerSafe(headsets[headsets.Count - 1].spawnPos, headAdjust: true);
				PlayerScaler.instance.ScaleHead(Vector3.one * headsets[headsets.Count - 1].spawnScale);
				SceneSequence.RemoveAt(SceneSequence.Count - 1);
			}
			SetActiveScene(SceneToEagerScene(scene));
		}
		else if (_loadCoroutine == null)
		{
			SafeDebug.LogError("Scene not loaded before headset used!");
			_nextScene = scene;
			_loadCoroutine = StartCoroutine(AsyncLoadAdditiveRoutine(scene));
		}
	}

	public void OverwriteScene(TCPlatformScene scene, bool destroyOldScene)
	{
		if (_isClearingMemory)
		{
			return;
		}
		if (_loadedScenes.Contains(scene))
		{
			TCPlatformScene scene2 = ActiveScene.Scene;
			activeHeadset = headsets[headsets.Count - 1];
			activeHeadset.transform.parent = EagerHeadsetManager.instance.transform;
			activeHeadset.Scene = scene;
			SceneSequence[SceneSequence.Count - 1] = scene;
			MovePlayerSafe(SceneToEagerScene(scene).playerPos, headAdjust: false, SceneToEagerScene(scene).setHeadPos);
			activeHeadset.hasBeenUsed = true;
			activeHeadset.gameObject.SetActive(value: false);
			SetActiveScene(SceneToEagerScene(scene));
			if (destroyOldScene)
			{
				DestroyScene(scene2);
			}
		}
		else if (_loadCoroutine == null)
		{
			SafeDebug.LogError("Scene not loaded before headset used!");
			_nextScene = scene;
			_loadCoroutine = StartCoroutine(AsyncLoadAdditiveRoutine(scene));
		}
	}

	public void SetActiveScene(EagerScene sceneIn, bool disableOldScene = true)
	{
		EagerScene activeScene = ActiveScene;
		if (activeScene != null)
		{
			activeScene.playerRot = EagerMaster.instance.player.transform.rotation;
		}
		if (ActiveScene != null && disableOldScene)
		{
			ActiveScene.gameObject.SetActive(value: false);
			ActiveScene.OnSceneUnloaded();
			EagerMaster.instance.player.transform.rotation = sceneIn.playerRot;
		}
		ActiveScene = sceneIn;
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(ActiveScene.Scene.Name));
		ActiveScene.gameObject.SetActive(value: true);
		ActiveScene.StartSceneBehavior();
		ToolManager.CancelDisableTeleport();
		if (PlayerScaler.instance.scaled)
		{
			ToolManager.AllManipulators.MaxGrabLength = ActiveScene.sceneMaxGrabDistance * PlayerScaler.instance.currentScale.x;
		}
		else
		{
			ToolManager.AllManipulators.MaxGrabLength = ActiveScene.sceneMaxGrabDistance;
		}
		ToolManager.AllManipulators.SceneCanVacuum = ActiveScene.sceneCanUseVacuum;
		ToolManager.AllVacuums.VacMaxLength = ActiveScene.sceneMaxVacDistance;
		HeadsetRemovalUIManager.instance.currentFisheyeScalar = ActiveScene.fisheyeScalar;
		EagerHeadsetManager.instance.farClipPlane.SetActive(ActiveScene.useFarClipPlane);
		EagerMaster.instance.mainCamera.farClipPlane = ActiveScene.farClipDistance;
		EagerHeadsetManager.instance.farClipPlane.transform.localPosition = Vector3.forward * Mathf.Abs(ActiveScene.farClipDistance - 2f);
		if (OnSceneActivated != null)
		{
			OnSceneActivated(ActiveScene, activeScene);
		}
	}

	public void MovePlayerSafe(Vector3 startPosition, bool headAdjust = false, bool moveHead = false)
	{
		ToolManager.StopTeleport();
		EagerMaster.instance.player.transform.position = startPosition;
		if (moveHead)
		{
			Vector3 position = EagerMaster.instance.player.transform.position;
			position.x -= (EagerMaster.instance.player.transform.localRotation * EagerMaster.instance.mainCamera.transform.localPosition).x;
			position.z -= (EagerMaster.instance.player.transform.localRotation * EagerMaster.instance.mainCamera.transform.localPosition).z;
			EagerMaster.instance.player.transform.position = position;
		}
		if (headAdjust)
		{
			EagerMaster.instance.headsetHeightManager.NormalizePlayerHeight();
		}
	}

	public void MergeScene(TCPlatformScene scene)
	{
		if (scene == BACKSTAGE_LEVEL && _loadedScenes.Contains(scene))
		{
			if (headsets.Count > 0)
			{
				headsets[headsets.Count - 1].spawnPos = EagerMaster.instance.player.transform.position;
			}
			if (PersistentDataManager.GetJobState() == VVR_HUB.JobStates.LOOP)
			{
				ActiveScene.backstageSpawnPos = Random.Range(0, Backstage.instance.jobGlobes.Length - 1);
				SafeDebug.Log("Randomize Globe Spawn");
			}
			headsets[headsets.Count - 1].Scene = BACKSTAGE_LEVEL;
			SceneSequence[SceneSequence.Count - 1] = BACKSTAGE_LEVEL;
			ActiveScene.playerPos = EagerMaster.instance.player.transform.position;
			ActiveScene.GetComponentInChildren<EnvironmentProperties>().gameObject.SetActive(value: false);
			Backstage.instance.isLoadingFromJob = true;
			Backstage.instance.jobLoadIndex = ActiveScene.backstageSpawnPos;
			playerDisplacement = ActiveScene.transform.position - Backstage.instance.jobGlobes[ActiveScene.backstageSpawnPos].transform.position;
			MovePlayerSafe(EagerMaster.instance.player.transform.position - playerDisplacement);
			oldScene = ActiveScene;
			SetActiveScene(_loadedEagerScenes[_loadedScenes.IndexOf(scene)], disableOldScene: false);
			oldScene.transform.parent = Backstage.instance.transform;
			oldScene.transform.position = Backstage.instance.jobGlobes[oldScene.backstageSpawnPos].transform.position;
		}
	}

	public void DestroyScene(TCPlatformScene scene)
	{
		if ((!(ActiveScene != null) || !(ActiveScene.Scene == scene)) && SceneIsLoaded(scene))
		{
			int index = _loadedScenes.IndexOf(scene);
			if (_loadedEagerScenes[index] != null)
			{
				Object.Destroy(_loadedEagerScenes[index].gameObject);
			}
			SceneManager.UnloadSceneAsync(scene.Name);
			_loadedEagerScenes.RemoveAt(index);
			_loadedScenes.RemoveAt(index);
			while (SceneSequence.IndexOf(scene) != -1)
			{
				int index2 = SceneSequence.IndexOf(scene);
				SceneSequence.RemoveAt(index2);
				headsets.RemoveAt(index2);
			}
			_unloadedScenes.Add(scene);
		}
	}

	public void DestroyAllScenesExcept(TCPlatformScene protectedScene)
	{
		if (ActiveScene.Scene != protectedScene)
		{
			SafeDebug.LogError("Player needs to be in the scene they don't want to destroy");
			return;
		}
		for (int num = _loadedScenes.Count - 1; num >= 0; num--)
		{
			if (_loadedScenes[num] != protectedScene && _loadedScenes[num] != TCScenes.S.VVR_PAUSE && SceneToEagerScene(_loadedScenes[num]) != null && !SceneToEagerScene(_loadedScenes[num]).adminScene)
			{
				DestroyScene(_loadedScenes[num]);
			}
		}
	}

	public void CleanUpAssets()
	{
		if (cleanUpCoroutine == null)
		{
			cleanUpCoroutine = StartCoroutine(AsyncDeleteScene());
			_isClearingMemory = true;
		}
	}

	private IEnumerator AsyncDeleteScene()
	{
		while (_loadCoroutine != null || loadQueueRef != null)
		{
			yield return null;
		}
		SafeDebug.Log("EagerScenes: Started UnloadUnusedAssets");
		yield return null;
		_aoDelete = Resources.UnloadUnusedAssets();
		bool doneLoading = false;
		while (!doneLoading)
		{
			if (_aoDelete.isDone)
			{
				doneLoading = true;
				_unloadedScenes = new List<TCPlatformScene>();
				SafeDebug.Log("EagerScenes: Completed UnloadUnusedAssets");
			}
			yield return null;
		}
		_isClearingMemory = false;
		cleanUpCoroutine = null;
	}

	public void GoToPauseScene()
	{
		if (_pauseThread == null)
		{
			StartCoroutine(PauseSequence());
		}
	}

	private IEnumerator PauseSequence()
	{
		EagerScene pauseScene = SceneToEagerScene(TCScenes.S.VVR_PAUSE);
		if (pauseScene != null)
		{
			ToolManager.ToggleManipulator(toggle: false);
			yield return pauseMenu.AnimateEyes(openEyes: false);
			SetActiveScene(pauseScene);
			yield return new WaitForSeconds(0.1f);
			pauseMenu.AnimateEyes(openEyes: true);
		}
	}

	public void LeavePauseScene()
	{
		if (_pauseThread == null)
		{
			StartCoroutine(UnPauseSequence());
		}
	}

	private IEnumerator UnPauseSequence()
	{
		yield return pauseMenu.AnimateEyes(openEyes: false, immediate: true);
		SetActiveScene(SceneToEagerScene(SceneSequence[SceneSequence.Count - 1]));
		yield return pauseMenu.AnimateEyes(openEyes: true);
		ToolManager.ToggleManipulator(toggle: true);
	}

	public TCPlatformScene GetSceneUnderCurrent()
	{
		_lastSceneIndex = SceneSequence.Count - 2;
		if (_lastSceneIndex < 0)
		{
			return null;
		}
		return SceneSequence[_lastSceneIndex];
	}

	public static bool SceneIsLoaded(TCPlatformScene level)
	{
		return level != null && instance._loadedScenes.Contains(level);
	}

	public static EagerScene SceneToEagerScene(TCPlatformScene scene)
	{
		if (SceneIsLoaded(scene))
		{
			return instance._loadedEagerScenes[instance._loadedScenes.IndexOf(scene)];
		}
		return null;
	}

	public void TakeOffAllHeadsets()
	{
		if (removeHeadsetsCoroutine == null)
		{
			removeHeadsetsCoroutine = StartCoroutine(BootToRoot());
		}
	}

	private IEnumerator BootToRoot()
	{
		EagerHeadsetManager.SetCanRemoveHeadset(canRemove: false);
		EagerHeadsetManager.SetLockPlayerInScene(lockPlayer: true);
		EagerMaster.instance.playerUnderAttack = true;
		while (GetSceneUnderCurrent() != null && !SceneToEagerScene(GetSceneUnderCurrent()).adminScene)
		{
			EagerHeadsetManager.instance.RemoveHeadset();
			yield return bootingTime;
		}
		EagerHeadsetManager.SetCanRemoveHeadset(canRemove: true);
		EagerHeadsetManager.SetLockPlayerInScene(lockPlayer: false);
		EagerMaster.instance.playerUnderAttack = false;
		removeHeadsetsCoroutine = null;
		yield return null;
	}

	public Headset CreateNewHeadset(TCPlatformScene scene)
	{
		GameObject gameObject = Object.Instantiate(headsetPrefab, EagerHeadsetManager.instance.transform);
		gameObject.name = scene.Name + " Headset";
		gameObject.SetActive(value: false);
		gameObject.GetComponent<Headset>().Scene = scene;
		return gameObject.GetComponent<Headset>();
	}

	public void AddHeadsetAtIndex(Headset newHeadset, int headsetIndex, bool forceAboveAdmin = true)
	{
		while (forceAboveAdmin && SceneToEagerScene(headsets[headsetIndex].Scene).adminScene)
		{
			headsetIndex++;
		}
		headsets.Insert(headsetIndex, newHeadset);
		SceneSequence.Insert(headsetIndex, newHeadset.Scene);
		if (headsetIndex == 0)
		{
			_baseHeadset = newHeadset;
		}
	}
}
