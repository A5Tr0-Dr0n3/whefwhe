using System.Collections;
using UnityEngine;
using UnityEngine.XR;

namespace TButt;

public class TBCore : MonoBehaviour
{
	public delegate void TBCoreEvent();

	public static class Events
	{
		public delegate void TBCoreSystemEvent(bool on);

		public static TBCoreSystemEvent OnHMDConnected;

		public static TBCoreSystemEvent OnSystemMenu;

		public static TBCoreSystemEvent OnHMDMounted;

		public static TBCoreEvent OnTrackingReset;
	}

	public static class Internal
	{
		public static void InitializeStartup()
		{
			if (GetActivePlatform() == VRPlatform.None)
			{
				instance.StartCheckingForHMD();
				return;
			}
			TBSettings.Initialize();
			TBInput.Initialize(GetActivePlatform());
			instance.InitializePerScene();
			Object.DontDestroyOnLoad(instance.gameObject);
			_initialized = true;
		}

		public static void ResetTracking()
		{
			if (GetActivePlatform() != VRPlatform.None)
			{
				InputTracking.Recenter();
			}
			if (Events.OnTrackingReset != null)
			{
				Events.OnTrackingReset();
			}
		}

		public static void SetHMDConnected(bool on)
		{
			if (Events.OnHMDConnected != null)
			{
				Events.OnHMDConnected(on);
			}
		}

		public static void SetSystemMenu(bool on)
		{
			if (Events.OnSystemMenu != null)
			{
				Events.OnSystemMenu(on);
			}
		}

		public static void SetHMDMounted(bool on)
		{
			if (Events.OnHMDMounted != null)
			{
				Events.OnHMDMounted(on);
			}
		}
	}

	public static TBCore instance;

	private static VRPlatform _activePlatform;

	private static VRService _activeService;

	private static bool _initialized;

	public static TBCoreEvent OnNewScene;

	public static TBCoreEvent OnUpdate;

	public static TBCoreEvent OnFixedUpdate;

	public static TBCoreEvent OnLateUpdate;

	private void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(gameObject);
			if (OnNewScene != null)
			{
				OnNewScene();
			}
			InitializePerScene();
		}
		else
		{
			instance = this;
			SetActivePlatform();
			Internal.InitializeStartup();
		}
	}

	private void InitializePerScene()
	{
		if (GetActivePlatform() != VRPlatform.None)
		{
			Object.FindObjectOfType<TBCameraRig>().Initialize();
			TBTracking.Initialize();
		}
	}

	private void Update()
	{
		if (OnUpdate != null)
		{
			OnUpdate();
		}
	}

	private void LateUpdate()
	{
		if (OnLateUpdate != null)
		{
			OnLateUpdate();
		}
	}

	private void FixedUpdate()
	{
		if (OnFixedUpdate != null)
		{
			OnFixedUpdate();
		}
	}

	private static void SetActivePlatform()
	{
		switch (XRSettings.loadedDeviceName)
		{
		case "Oculus":
			if (Application.isMobilePlatform)
			{
				_activePlatform = VRPlatform.OculusMobile;
			}
			else
			{
				_activePlatform = VRPlatform.OculusPC;
			}
			break;
		case "OpenVR":
			_activePlatform = VRPlatform.SteamVR;
			break;
		case "daydream":
			_activePlatform = VRPlatform.Daydream;
			break;
		case "cardboard":
			_activePlatform = VRPlatform.Cardboard;
			break;
		case "PlayStationVR":
			_activePlatform = VRPlatform.PlayStationVR;
			break;
		case "WindowsMR":
			_activePlatform = VRPlatform.WindowsMR;
			break;
		default:
			Debug.LogError("Detected " + XRSettings.loadedDeviceName);
			Debug.LogError("The connected HMD / SDK pairing doesn't match any known TButt preset!");
			_activePlatform = VRPlatform.None;
			XRSettings.enabled = false;
			break;
		}
	}

	private void StartCheckingForHMD()
	{
		StartCoroutine(WaitForHMDConnection());
	}

	private IEnumerator WaitForHMDConnection()
	{
		Object.FindObjectOfType<TBCameraRig>().Initialize();
		while (!_initialized)
		{
			yield return null;
		}
		SetActivePlatform();
		Internal.InitializeStartup();
	}

	public void ForceInit()
	{
		_initialized = true;
	}

	public static VRPlatform GetActivePlatform()
	{
		return _activePlatform;
	}

	public static VRService GetActiveService()
	{
		return _activeService;
	}

	public static bool UsingEditorMode()
	{
		return false;
	}

	public static bool IsNativeIntegration()
	{
		GetActivePlatform();
		return true;
	}

	private void OnApplicationFocus(bool focus)
	{
		if (Events.OnSystemMenu != null)
		{
			Events.OnSystemMenu(focus);
		}
	}
}
