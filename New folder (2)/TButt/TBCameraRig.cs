using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace TButt;

public class TBCameraRig : MonoBehaviour
{
	public enum CameraMode
	{
		Single,
		Dual
	}

	public static class Events
	{
		public delegate void CameraSettingChangeEvent();

		public static CameraSettingChangeEvent OnBackgroundColorChanged;

		public static CameraSettingChangeEvent OnNearClipPlaneChanged;

		public static CameraSettingChangeEvent OnFarClipPlaneChanged;

		public static CameraSettingChangeEvent OnClearFlagsChanged;

		public static CameraSettingChangeEvent OnOcclusionCullingChanged;

		public static CameraSettingChangeEvent OnSortModeChanged;

		public static CameraSettingChangeEvent OnCullingMaskChanged;

		public static CameraSettingChangeEvent OnDepthChanged;

		public static CameraSettingChangeEvent OnStereoConvergenceChanged;

		public static CameraSettingChangeEvent OnStereoSeparationChanged;
	}

	[SerializeField]
	private LayerMask _audioOcclusionMask;

	public static TBCameraRig instance;

	[HideInInspector]
	private CameraClearFlags _clearFlags;

	[HideInInspector]
	private LayerMask _cullingMask;

	[HideInInspector]
	private Color _backgroundColor;

	[HideInInspector]
	private float _nearClipPlane = 1f;

	[HideInInspector]
	private float _farClipPlane = 250f;

	[HideInInspector]
	private bool _useOcclusionCulling;

	[HideInInspector]
	private float _depth = -1f;

	[HideInInspector]
	private float _stereoConvergence;

	[HideInInspector]
	private float _stereoSeparation;

	[HideInInspector]
	private OpaqueSortMode _sortMode;

	private CameraMode _cameraMode;

	private Camera _primaryCamera;

	private Camera _rightCamera;

	private Camera _leftCamera;

	private Transform _centerEyeTransform;

	private Transform _leftEyeTransform;

	private Transform _rightEyeTransform;

	private TBCameraBase _baseCamera;

	private Transform _trackingVolume;

	public ValveCameraSettingsObject valveCameraSettings;

	public PostProcessResources postProcessResources;

	public LayerMask poastProcessLayer;

	[HideInInspector]
	public CameraClearFlags clearFlags
	{
		get
		{
			return _clearFlags;
		}
		set
		{
			_clearFlags = value;
			if (Events.OnClearFlagsChanged != null)
			{
				Events.OnClearFlagsChanged();
			}
		}
	}

	[HideInInspector]
	public bool useOcclusionCulling
	{
		get
		{
			return _useOcclusionCulling;
		}
		set
		{
			_useOcclusionCulling = value;
			if (Events.OnOcclusionCullingChanged != null)
			{
				Events.OnOcclusionCullingChanged();
			}
		}
	}

	[HideInInspector]
	public LayerMask cullingMask
	{
		get
		{
			return _cullingMask;
		}
		set
		{
			_cullingMask = value;
			if (Events.OnCullingMaskChanged != null)
			{
				Events.OnCullingMaskChanged();
			}
		}
	}

	[HideInInspector]
	public Color backgroundColor
	{
		get
		{
			return _backgroundColor;
		}
		set
		{
			_backgroundColor = value;
			if (Events.OnBackgroundColorChanged != null)
			{
				Events.OnBackgroundColorChanged();
			}
		}
	}

	[HideInInspector]
	public float nearClipPlane
	{
		get
		{
			return _nearClipPlane;
		}
		set
		{
			_nearClipPlane = value;
			if (Events.OnNearClipPlaneChanged != null)
			{
				Events.OnNearClipPlaneChanged();
			}
		}
	}

	[HideInInspector]
	public float farClipPlane
	{
		get
		{
			return _farClipPlane;
		}
		set
		{
			_farClipPlane = value;
			if (Events.OnFarClipPlaneChanged != null)
			{
				Events.OnFarClipPlaneChanged();
			}
		}
	}

	[HideInInspector]
	public OpaqueSortMode sortMode
	{
		get
		{
			return _sortMode;
		}
		set
		{
			_sortMode = value;
			if (Events.OnSortModeChanged != null)
			{
				Events.OnSortModeChanged();
			}
		}
	}

	[HideInInspector]
	public float stereoConvergence
	{
		get
		{
			return _stereoConvergence;
		}
		set
		{
			_stereoConvergence = value;
			if (Events.OnStereoConvergenceChanged != null)
			{
				Events.OnStereoConvergenceChanged();
			}
		}
	}

	[HideInInspector]
	public float stereoSeparation
	{
		get
		{
			return _stereoSeparation;
		}
		set
		{
			_stereoSeparation = value;
			if (Events.OnStereoSeparationChanged != null)
			{
				Events.OnStereoSeparationChanged();
			}
		}
	}

	[HideInInspector]
	public float depth
	{
		get
		{
			return _depth;
		}
		set
		{
			_depth = value;
			if (Events.OnDepthChanged != null)
			{
				Events.OnDepthChanged();
			}
		}
	}

	public void Initialize()
	{
		instance = this;
		Debug.Log("Assigned new TBCameraRig instace...");
		ReadInitialCameraSettings();
		if (TBCore.GetActivePlatform() != VRPlatform.None)
		{
			_trackingVolume = new GameObject().transform;
			_trackingVolume.gameObject.name = "Tracking Volume";
			_trackingVolume.MakeZeroedChildOf(transform);
		}
		SetupCameraForPlatform(TBCore.GetActivePlatform());
	}

	private void SetupCameraForPlatform(VRPlatform platform)
	{
		if (platform != VRPlatform.None)
		{
			SetupNativeCamera(platform);
			DestroyTempCamera();
			_centerEyeTransform.gameObject.AddComponent<AudioListener>();
			GvrAudioListener gvrAudioListener = _centerEyeTransform.gameObject.AddComponent<GvrAudioListener>();
			gvrAudioListener.occlusionMask = _audioOcclusionMask;
		}
		else
		{
			_primaryCamera = GetComponent<Camera>();
		}
	}

	private void ReadInitialCameraSettings()
	{
		Camera component = GetComponent<Camera>();
		clearFlags = component.clearFlags;
		cullingMask = component.cullingMask;
		backgroundColor = component.backgroundColor;
		nearClipPlane = component.nearClipPlane;
		farClipPlane = component.farClipPlane;
		useOcclusionCulling = component.useOcclusionCulling;
		depth = component.depth;
		stereoConvergence = component.stereoConvergence;
		stereoSeparation = component.stereoSeparation;
	}

	private void SetupNativeCamera(VRPlatform platform)
	{
		_centerEyeTransform = new GameObject().transform;
		_centerEyeTransform.MakeZeroedChildOf(_trackingVolume);
		_centerEyeTransform.gameObject.name = "Standard VR Camera";
		_centerEyeTransform.gameObject.tag = "MainCamera";
		SphereCollider sphereCollider = _centerEyeTransform.gameObject.AddComponent(typeof(SphereCollider)) as SphereCollider;
		sphereCollider.radius = 0.2f;
		sphereCollider.isTrigger = true;
		_primaryCamera = _centerEyeTransform.gameObject.AddComponent<Camera>();
		switch (platform)
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
		{
			_cameraMode = CameraMode.Single;
			_baseCamera = _primaryCamera.gameObject.AddComponent<TBCameraOculus>();
			_baseCamera.GetComponent<TBCameraOculus>().SetPostProcessingResources(postProcessResources, poastProcessLayer);
			ValveCamera valveCamera2 = _primaryCamera.gameObject.AddComponent<ValveCamera>();
			valveCamera2.CopySettingsFromScriptableObject(valveCameraSettings);
			break;
		}
		case VRPlatform.SteamVR:
		{
			_cameraMode = CameraMode.Single;
			_baseCamera = _primaryCamera.gameObject.AddComponent<TBCameraSteamVR>();
			_baseCamera.GetComponent<TBCameraSteamVR>().SetPostProcessingResources(postProcessResources, poastProcessLayer);
			ValveCamera valveCamera = _primaryCamera.gameObject.AddComponent<ValveCamera>();
			valveCamera.CopySettingsFromScriptableObject(valveCameraSettings);
			break;
		}
		case VRPlatform.PlayStationVR:
			_cameraMode = CameraMode.Single;
			_baseCamera = _primaryCamera.gameObject.AddComponent<TBCameraPSVR>();
			break;
		default:
			_cameraMode = CameraMode.Single;
			_baseCamera = _primaryCamera.gameObject.AddComponent<TBCameraBase>();
			break;
		}
		_baseCamera.tag = "MainCamera";
		_baseCamera.Initialize();
	}

	private void DestroyTempCamera()
	{
		if (gameObject.CompareTag("MainCamera"))
		{
			gameObject.tag = "Untagged";
		}
		Transform child = transform.GetChild(0);
		child.SetParent(Camera.main.transform);
		FlareLayer component = GetComponent<FlareLayer>();
		if (component != null)
		{
			Object.Destroy(component);
		}
		AudioListener component2 = GetComponent<AudioListener>();
		if (component2 != null)
		{
			Object.Destroy(component2);
		}
		Camera component3 = GetComponent<Camera>();
		component3.stereoTargetEye = StereoTargetEyeMask.None;
		Object.Destroy(component3);
	}

	public CameraMode GetCameraMode()
	{
		return _cameraMode;
	}

	public Camera GetRightEyeCamera()
	{
		if (_rightCamera == null)
		{
			return _primaryCamera;
		}
		return _rightCamera;
	}

	public Camera GetCenterEyeCamera()
	{
		return _primaryCamera;
	}

	public Camera GetLeftEyeCamera()
	{
		if (_leftCamera == null)
		{
			return _primaryCamera;
		}
		return _leftCamera;
	}

	public Transform GetCenter()
	{
		return _centerEyeTransform;
	}

	public Transform GetTrackingVolume()
	{
		return _trackingVolume;
	}
}
