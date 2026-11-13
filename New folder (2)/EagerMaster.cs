using TButt;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;

public class EagerMaster : MonoBehaviour
{
	public static EagerMaster instance;

	public GameObject player;

	public Camera mainCamera;

	public AudioMixer gvrMixer;

	public AudioMixer flatMixer;

	public bool isSwitchingScenes;

	public bool shouldRaycast = true;

	public int leashRayMask;

	public LayerMask edibleOnlyMask;

	public LayerMask fillableOnlyMask;

	public LayerMask edibleFillableMask;

	public bool playerUnderAttack;

	public Vector3 vacuuumPosition;

	public ModelDatabaseObject modelDatabase;

	private float fill;

	public PostProcessVolume globalPostProcessVolume;

	public EagerHeightManger headsetHeightManager;

	internal int shadowDivider = 1;

	public AudioEvent adminOverrideSound;

	public AudioSource flatSource;

	public GvrAudioSource defaultGVRSource;

	public float vacuumFill
	{
		get
		{
			return fill;
		}
		set
		{
			fill = Mathf.Max(0f, value);
		}
	}

	private void Awake()
	{
		if (globalPostProcessVolume == null)
		{
			globalPostProcessVolume = gameObject.AddComponent<PostProcessVolume>();
			globalPostProcessVolume.isGlobal = true;
		}
		SafeDebug.Log("EagerMaster Awake");
		instance = this;
		leashRayMask = ~(1 << LayerMask.NameToLayer("Vacuumed"));
		player = GameObject.FindGameObjectWithTag("Player");
		mainCamera = Camera.main;
	}

	private void Update()
	{
		if (TBCore.GetActivePlatform() == VRPlatform.Daydream)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Application.Quit();
			}
		}
		else if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile && !Input.GetKeyDown(KeyCode.Escape))
		{
		}
		if (ToolManager.AdminCheatTimer >= 10f)
		{
			PersistentDataManager.OverwriteHasAdmin(hasAdminIn: true);
			if (adminOverrideSound != null)
			{
				adminOverrideSound.Play();
			}
			ToolManager.ResetAdminTimer();
		}
	}

	public void SetShadowDivider(int s)
	{
		shadowDivider = s;
		mainCamera.GetComponent<ValveCamera>().enabled = false;
		mainCamera.GetComponent<ValveCamera>().m_valveShadowTextureHeight = TBCameraRig.instance.valveCameraSettings._valveShadowTextureHeight / shadowDivider;
		mainCamera.GetComponent<ValveCamera>().m_valveShadowTextureWidth = TBCameraRig.instance.valveCameraSettings._valveShadowTextureWidth / shadowDivider;
		mainCamera.GetComponent<ValveCamera>().enabled = true;
	}
}
