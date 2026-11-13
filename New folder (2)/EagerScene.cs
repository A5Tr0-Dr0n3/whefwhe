using GVR.Gfx;
using UnityEngine;
using UnityEngine.Events;

public class EagerScene : MonoBehaviour
{
	public TCPlatformScene Scene;

	public Vector3 playerPos;

	public bool setHeadPos = true;

	public Quaternion playerRot = Quaternion.identity;

	public int backstageSpawnPos;

	public bool useFarClipPlane = true;

	public float farClipDistance = 200f;

	[SerializeField]
	private bool isCheckpoint;

	public bool adminScene;

	public float sceneMaxGrabDistance = 10f;

	public float sceneMaxVacDistance = 7f;

	public bool sceneCanUseVacuum = true;

	public bool sustainedPerformanceMode;

	private bool hasBeenActivated;

	[SerializeField]
	private SceneBehaviorManager mSceneBehaviour;

	[HideInInspector]
	public Color mainColor = Color.white;

	[HideInInspector]
	public Color shadowColor = Color.grey;

	public EnvironmentProperties mEnvironmentProperties;

	public MaterialUpdateController defaultMaterialProperties;

	public ConsumableSaveSyncer mConsumableSyncer;

	public UnityEvent onSceneInitialized;

	public float padding;

	[Range(0f, 5f)]
	public float fisheyeScalar = 1f;

	public GvrAudioRoom sceneAudioRoom;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(playerPos, 0.33f);
	}

	public void StartSceneBehavior()
	{
		if (mSceneBehaviour == null)
		{
			mSceneBehaviour = GetComponent<SceneBehaviorManager>();
		}
		if (mSceneBehaviour != null)
		{
			mSceneBehaviour.ActivateSceneBehavior();
		}
		if (mEnvironmentProperties != null)
		{
			mEnvironmentProperties.Activate();
		}
		if (defaultMaterialProperties != null)
		{
			defaultMaterialProperties.PushMaterials();
		}
		if (EagerScenes.instance.ActiveScene != null && EagerScenes.instance.ActiveScene.Scene == Scene && isCheckpoint)
		{
			SetSelfAsCheckpoint();
		}
		if (onSceneInitialized != null)
		{
			onSceneInitialized.Invoke();
		}
		if (!hasBeenActivated)
		{
			if (mConsumableSyncer != null)
			{
				mConsumableSyncer.LoadConsumables();
			}
			hasBeenActivated = true;
		}
	}

	public void OnSceneUnloaded()
	{
		if (mConsumableSyncer != null)
		{
			mConsumableSyncer.SaveConsumables();
		}
		if (mSceneBehaviour != null)
		{
			mSceneBehaviour.OnSceneDeactivated();
		}
	}

	private void Start()
	{
		if (EagerScenes.instance != null && EagerScenes.instance.ActiveScene != null && EagerScenes.instance.ActiveScene.Scene != Scene)
		{
			gameObject.SetActive(value: false);
		}
	}

	public void RemoveCheckpoint()
	{
		isCheckpoint = false;
	}

	private void SetSelfAsCheckpoint()
	{
		PersistentDataManager.OverwriteLastLoadedScene(Scene);
	}
}
