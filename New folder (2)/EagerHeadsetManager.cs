using System.Collections;
using GVR.Gfx;
using TButt;
using UnityEngine;

public class EagerHeadsetManager : MonoBehaviour
{
	public static EagerHeadsetManager instance;

	public GameObject headsetPrefab;

	public Headset activeHeadset;

	private Headset lastRemovedHeadset;

	public AudioEvent headsetOnSound;

	public AudioEvent headsetOffSound;

	private bool headsetRemovalEnabledWithoutAdmin = true;

	private bool lockPlayerInScene;

	public Mesh faceMesh;

	public GameObject farClipPlane;

	public bool isAddingHeadset;

	private bool changedScene;

	private bool usingLocalHeadset;

	public Material lensStencilMaterial;

	public GameObject headset;

	public GameObject headsetParent;

	private GameObject localHeadset;

	public Transform headsetStart;

	public Transform headsetMiddle;

	public Transform headsetEnd;

	private float lerpVal;

	private float lerpSpeedUp;

	public float lerpValSpeed = 7.5f;

	public float headsetAddSpeed = 30f;

	public float fisheyeLerpSpeed = 1.3f;

	public float fisheyeStrength = -3f;

	public bool isLoadingSequence;

	public TCPlatformScene Scene;

	private bool fatalError;

	private Vector3 emptyLerp;

	private bool waitToLoad;

	private bool headsetHasBeenInFront;

	[SerializeField]
	[CurveRange(0f, 1f, -1f, 1f)]
	private AnimationCurve fisheyeCurve;

	private float fishEyeLerp;

	public float headsetEjectForce = 100f;

	public float headsetEjectTorque = 0.25f;

	private float scaledRemoveVelocity;

	private void Awake()
	{
		if (instance != null)
		{
			throw new UnityException("Only one instance of EagerHeadsetManager in a scene is allowed.");
		}
		instance = this;
	}

	private void Start()
	{
		if (headset != null)
		{
			headset.SetActive(value: false);
			localHeadset = headset;
		}
	}

	private void Update()
	{
		if (isAddingHeadset && headset != null)
		{
			UpdateAddHeadset();
		}
	}

	public void AddHeadset()
	{
		ObjectBoid component = headset.GetComponent<ObjectBoid>();
		if (component != null)
		{
			component.enabled = false;
			for (int i = 0; i < headset.transform.childCount; i++)
			{
				headset.transform.GetChild(i).localRotation = Quaternion.identity;
			}
			if (component.eyeBlack != null)
			{
				component.eyeBlack.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			}
		}
		headsetParent.transform.parent = EagerMaster.instance.mainCamera.transform;
		if (headset.transform.IsChildOf(headsetParent.transform))
		{
			usingLocalHeadset = true;
			headset.transform.position = headsetStart.position;
		}
		else
		{
			headset.transform.parent = headsetParent.transform;
		}
		lensStencilMaterial.color = Color.white;
		if (!EagerScenes.SceneIsLoaded(Scene))
		{
			SafeDebug.LogError("DONT TRY TO ADD A HEADSET IF THE SCENE IS NOT LOADED");
		}
		headset.SetActive(value: true);
		isAddingHeadset = true;
		changedScene = false;
		headsetOnSound.Play();
		lerpVal = 0f;
		headsetHasBeenInFront = false;
	}

	private void UpdateAddHeadset()
	{
		if (!waitToLoad && (headset.transform.localPosition.z > 0.08f || !headsetHasBeenInFront))
		{
			activeHeadset.rb.isKinematic = true;
			if (headset.transform.localPosition.z > 0.08f)
			{
				headsetHasBeenInFront = true;
			}
			lerpVal = Utility.TCVRLerp(lerpVal, 1f, lerpValSpeed);
			emptyLerp = Vector3.Lerp(headsetMiddle.position, headsetEnd.position, lerpVal);
			headset.transform.position = Utility.TCVRLerp(headset.transform.position, emptyLerp, headsetAddSpeed * lerpVal);
			headset.transform.rotation = Quaternion.Lerp(headset.transform.rotation, EagerMaster.instance.mainCamera.transform.rotation, headsetAddSpeed * 2f * lerpVal);
			EagerMaster.instance.mainCamera.nearClipPlane = Mathf.Lerp(0.1f, 0.03f, lerpVal);
		}
		if ((!(headset.transform.localPosition.z < 0.08f) || !headsetHasBeenInFront) && !changedScene)
		{
			return;
		}
		if (!changedScene)
		{
			if (EagerScenes.SceneIsLoaded(Scene))
			{
				SafeDebug.Log("Active headset: " + activeHeadset.name);
				EagerScenes.instance.GoToScene(Scene, puttingOnHeadset: true);
				waitToLoad = false;
				EnableFisheye();
				EagerMaster.instance.mainCamera.nearClipPlane = 0.1f;
				changedScene = true;
				headset.SetActive(value: false);
				headset = localHeadset;
			}
			else
			{
				SafeDebug.LogError("TRYING TO PUT ON HEADSET WHEN THE SCENE HASN'T BEEN LOADED, TSK TSK");
				waitToLoad = true;
			}
			return;
		}
		fishEyeLerp += fisheyeLerpSpeed * Time.deltaTime;
		EnvironmentProperties.fishEyeStrength = 5f * fisheyeStrength * fisheyeCurve.Evaluate(fishEyeLerp);
		if (fishEyeLerp > 1f)
		{
			EnvironmentProperties.fishEyeStrength = 0f;
			isAddingHeadset = false;
			EagerMaster.instance.isSwitchingScenes = false;
			if (usingLocalHeadset)
			{
				headset.SetActive(value: false);
			}
		}
	}

	private void EnableFisheye()
	{
		fishEyeLerp = 0f;
		EnvironmentProperties.fishEyeStrength = 5f * fisheyeStrength;
	}

	public void SetEnableFisheye()
	{
		EnableFisheye();
	}

	public void SetRemoveFisheye()
	{
		StartCoroutine(RemoveFisheye());
	}

	public void SetForceFatalError(bool s)
	{
		fatalError = s;
	}

	public void RemoveHeadset(Manipulator grabber = null)
	{
		ToolManager.StopTeleport();
		EagerScene eagerScene = EagerScenes.SceneToEagerScene(EagerScenes.instance.GetSceneUnderCurrent());
		if ((bool)grabber && lockPlayerInScene)
		{
			HeadsetRemovalUIManager.instance.CannotTakeOffHeadset(HeadsetRemovalUIManager.HeadsetRemovalFailureType.forceDisabled);
			return;
		}
		if ((bool)grabber && !PersistentDataManager.GetHasAdmin() && (eagerScene == null || eagerScene.adminScene))
		{
			HeadsetRemovalUIManager.instance.CannotTakeOffHeadset(HeadsetRemovalUIManager.HeadsetRemovalFailureType.needAdmin);
			return;
		}
		if ((bool)grabber && !PersistentDataManager.GetHasAdmin() && !headsetRemovalEnabledWithoutAdmin)
		{
			HeadsetRemovalUIManager.instance.CannotTakeOffHeadset(HeadsetRemovalUIManager.HeadsetRemovalFailureType.needAdmin);
			return;
		}
		if ((bool)grabber && !PersistentDataManager.GetHasAdmin() && eagerScene != null && eagerScene.Scene == EagerScenes.JAIL_LEVEL)
		{
			HeadsetRemovalUIManager.instance.CannotTakeOffHeadset(HeadsetRemovalUIManager.HeadsetRemovalFailureType.needAdmin);
			return;
		}
		if (eagerScene == null || fatalError)
		{
			HeadsetRemovalUIManager.instance.CannotTakeOffHeadset(HeadsetRemovalUIManager.HeadsetRemovalFailureType.forceDisabled);
			SafeDebug.LogError("There's no scene below you or you are scaled.");
			return;
		}
		EagerScenes.instance.GoToScene(EagerScenes.instance.GetSceneUnderCurrent(), puttingOnHeadset: false);
		lastRemovedHeadset = EagerScenes.instance.lastHeadsetRemoved;
		lastRemovedHeadset.ResetHeadset();
		lastRemovedHeadset.transform.position = EagerMaster.instance.mainCamera.transform.position + EagerMaster.instance.mainCamera.transform.forward * 0.02f;
		lastRemovedHeadset.transform.rotation = EagerMaster.instance.mainCamera.transform.rotation;
		lastRemovedHeadset.rb.AddForce(lastRemovedHeadset.transform.forward * headsetEjectForce / 60f, ForceMode.Impulse);
		lastRemovedHeadset.rb.AddForce(lastRemovedHeadset.transform.up * headsetEjectForce * 0.25f / 60f, ForceMode.Impulse);
		lastRemovedHeadset.rb.AddTorque(Random.insideUnitSphere * headsetEjectTorque * (1f / 60f / Time.fixedDeltaTime));
		headsetOffSound.Play();
		ToolManager.DropEverything(playSFX: false);
		if ((bool)grabber)
		{
			float num = ((TBInput.Get3DOFHandedness() != TBInput.Mobile3DOFHandedness.Left) ? 1f : (-1f));
			if (TBCore.GetActivePlatform() == VRPlatform.OculusPC || TBCore.GetActivePlatform() == VRPlatform.SteamVR || TBCore.GetActivePlatform() == VRPlatform.PlayStationVR)
			{
				num = ((grabber.toolGroup.Controller != TBInput.Controller.LHandController) ? 1f : (-1f));
			}
			grabber.Grab(lastRemovedHeadset.GetComponent<ObjectBase>(), lastRemovedHeadset.transform.position + lastRemovedHeadset.transform.right * 0.1f * num, playSound: false);
			lastRemovedHeadset.headsetHasBeenSafelyRemoved = false;
			lastRemovedHeadset.timeSinceRemoved = 0f;
			grabber.AnimateGrabToLength(1.25f, 0.35f, dropObject: false);
		}
		ObjectBoid component = lastRemovedHeadset.GetComponent<ObjectBoid>();
		if (component != null)
		{
			component.OnRemovedHeadset();
		}
		EnableFisheye();
		StartCoroutine(RemoveFisheye());
	}

	public static void SetCanRemoveHeadset(bool canRemove)
	{
		instance.headsetRemovalEnabledWithoutAdmin = canRemove;
	}

	public static void SetLockPlayerInScene(bool lockPlayer)
	{
		instance.lockPlayerInScene = lockPlayer;
	}

	public static bool GetPlayerLockedInScene()
	{
		return instance.lockPlayerInScene;
	}

	private IEnumerator RemoveFisheye()
	{
		float startFish = EnvironmentProperties.fishEyeStrength;
		while (fishEyeLerp < 1f)
		{
			yield return null;
			fishEyeLerp += fisheyeLerpSpeed * Time.deltaTime;
			EnvironmentProperties.fishEyeStrength = startFish * fisheyeCurve.Evaluate(fishEyeLerp);
		}
		EnvironmentProperties.fishEyeStrength = 0f;
		yield return null;
	}
}
