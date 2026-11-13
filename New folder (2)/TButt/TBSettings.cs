using System;
using System.Diagnostics;
using TButt.Settings;
using UnityEngine;
using UnityEngine.Rendering;

namespace TButt;

public static class TBSettings
{
	[Serializable]
	public struct TBDisplaySettings
	{
		public VRPlatform platform;

		public bool useDefaultTimestep;

		public bool lockTimestepToRefreshRate;

		public float minRenderScale;

		public float maxRenderScale;

		public float[] renderScales;

		public int[] cpuLevels;

		public int[] gpuLevels;

		public int[] foveatedLevels;

		public TBMSAA[] antiAliasingLevels;

		public bool useAdaptiveResolution;

		public OpaqueSortMode opaqueSortMode;

		public TBMaxTimestep maxTimestep;

		public TBQualityLevel minQualityLevel;

		public TBQualityLevel maxQualityLevel;

		public TBRefreshRate minRefreshRate;

		public TBRefreshRate maxRefreshRate;

		public bool mirrorDisplay;
	}

	[Serializable]
	public struct TBCameraSettings
	{
		public TBTrackingOrigin trackingOrigin;

		public KeyCode calibrationHotkey;

		public float uncalibratedFloorHeight;

		public bool disablePositionTracking;
	}

	[Serializable]
	public struct TBControlSettings
	{
		public bool supportsHandControllers;

		public bool supports3DOFControllers;

		public bool emulate3DOFArmModel;

		public TBHardwareHandedness handedness3DOF;

		public bool supportsClickRemote;

		public bool supportsGamepad;

		public bool useInputEvents;

		public TBInput.ControlType defaultEditorControlType;
	}

	[Serializable]
	public enum TBQualityLevel
	{
		VeryLow,
		Low,
		Medium,
		High,
		VeryHigh,
		Ultra
	}

	[Serializable]
	public enum TBMaxTimestep
	{
		Locked,
		Half,
		Unlocked
	}

	[Serializable]
	public enum TBTrackingOrigin
	{
		Eye,
		Floor
	}

	[Serializable]
	public enum TBMSAA
	{
		Disabled = 0,
		TwoXMultiSampling = 2,
		FourXMultiSampling = 4,
		EightXMultiSampling = 8
	}

	public enum TBRefreshRate
	{
		FPS_60 = 1,
		FPS_75 = 2,
		FPS_90 = 4,
		FPS_120 = 8
	}

	public enum TBGoogleRefreshRate
	{
		FPS_60 = 1,
		FPS_90 = 4
	}

	public enum TBPSVRRefreshRate
	{
		FPS_60 = 1,
		FPS_90 = 4,
		FPS_120 = 8
	}

	public enum TBHardwareHandedness
	{
		Left,
		Right
	}

	public class VRDeviceNames
	{
		public const string Oculus = "Oculus";

		public const string SteamVR = "OpenVR";

		public const string Daydream = "daydream";

		public const string Cardboard = "cardboard";

		public const string Unknown = "Unknown";

		public const string PlayStationVR = "PlayStationVR";

		public const string WindowsMR = "WindowsMR";

		public const string None = "None";
	}

	public static readonly string settingsFolder = "TButtSettings/";

	public static readonly string controlSettingsFileName = "ControlSettings";

	public static readonly string displaySettingsFilename = "DisplaySettings";

	public static readonly string cameraSettingsFilename = "CameraSettings";

	private static TBControlSettings _controlSettings;

	private static TBDisplaySettings _displaySettings;

	private static TBDisplaySettings _globalDisplaySettings;

	private static TBCameraSettings _cameraSettings;

	private static TBSettingsBase _settingsBase;

	private static bool _loadedSettings;

	public static void Initialize()
	{
		if (!_loadedSettings)
		{
			LoadSettings();
		}
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			_settingsBase = TBCore.instance.gameObject.AddComponent<TBSettingsOculus>();
			break;
		case VRPlatform.PlayStationVR:
			_settingsBase = TBCore.instance.gameObject.AddComponent<TBSettingsPSVR>();
			break;
		case VRPlatform.WindowsMR:
			_settingsBase = TBCore.instance.gameObject.AddComponent<TBSettingsWindowsMR>();
			break;
		case VRPlatform.Daydream:
			_settingsBase = TBCore.instance.gameObject.AddComponent<TBSettingsDaydream>();
			break;
		default:
			_settingsBase = TBCore.instance.gameObject.AddComponent<TBSettingsBase>();
			break;
		}
		_settingsBase.Initialize();
		TBDisplaySettings tBDisplaySettings = _displaySettings;
		if (_displaySettings.useDefaultTimestep)
		{
			tBDisplaySettings = _globalDisplaySettings;
		}
		float num = 1f / 60f;
		float maxStep = 1f / 30f;
		if (tBDisplaySettings.lockTimestepToRefreshRate)
		{
			num = 1f / _settingsBase.GetRefreshRate();
		}
		switch (tBDisplaySettings.maxTimestep)
		{
		case TBMaxTimestep.Locked:
			maxStep = num;
			break;
		case TBMaxTimestep.Half:
			maxStep = num * 2f;
			break;
		case TBMaxTimestep.Unlocked:
			maxStep = 1f;
			break;
		}
		SetTimestep(num, maxStep);
	}

	public static void LoadSettings()
	{
		if (!_loadedSettings)
		{
			_controlSettings = TBDataManager.DeserializeFromFile<TBControlSettings>(settingsFolder + controlSettingsFileName, TBDataManager.PathType.ResourcesFolder);
			_displaySettings = LoadDisplaySettingsFromJSON();
			_cameraSettings = TBDataManager.DeserializeFromFile<TBCameraSettings>(settingsFolder + cameraSettingsFilename, TBDataManager.PathType.ResourcesFolder);
			_loadedSettings = true;
		}
	}

	public static TBDisplaySettings GetDisplaySettings()
	{
		return _displaySettings;
	}

	public static TBControlSettings GetControlSettings()
	{
		return _controlSettings;
	}

	public static TBQualityLevel GetCurrentQualityLevel()
	{
		return _settingsBase.GetConfiguredQualityLevel();
	}

	public static TBCameraSettings GetCameraSettings()
	{
		return _cameraSettings;
	}

	public static float GetConfiguredRenderscale()
	{
		return _settingsBase.GetConfiguredRenderscale();
	}

	public static float GetRefreshRate()
	{
		return _settingsBase.GetRefreshRate();
	}

	[Conditional("TB_ENABLE_LOGS")]
	private static void PrintControlSettings()
	{
		string text = "Supported inputs: ";
		if (_controlSettings.supportsHandControllers)
		{
			text += " Hand Controllers. ";
		}
		if (_controlSettings.supports3DOFControllers)
		{
			text += " 3DOF Controllers. ";
		}
		if (_controlSettings.supportsClickRemote)
		{
			text += " Click Remotes. ";
		}
		if (_controlSettings.supportsGamepad)
		{
			text += " Gamepads. ";
		}
	}

	public static TBDisplaySettings GetDefaultDisplaySettings(VRPlatform platform)
	{
		TBDisplaySettings tBDisplaySettings = new TBDisplaySettings
		{
			platform = VRPlatform.None,
			useDefaultTimestep = false,
			lockTimestepToRefreshRate = true,
			maxRenderScale = 1f,
			maxTimestep = TBMaxTimestep.Locked,
			minRenderScale = 1f,
			useAdaptiveResolution = false,
			opaqueSortMode = OpaqueSortMode.NoDistanceSort,
			maxQualityLevel = TBQualityLevel.VeryHigh,
			mirrorDisplay = true
		};
		if (platform == VRPlatform.OculusMobile || platform == VRPlatform.OculusPC)
		{
			return TBSettingsOculus.GetDefaultDisplaySettings(tBDisplaySettings);
		}
		return tBDisplaySettings;
	}

	public static TBDisplaySettings LoadDisplaySettingsFromJSON()
	{
		TBDisplaySettings[] array = TBDataManager.FromJsonWrapper(TBDataManager.DeserializeFromFile<TBDataManager.Wrapper<TBDisplaySettings>>(settingsFolder + displaySettingsFilename, TBDataManager.PathType.ResourcesFolder));
		if (array.Length > 0)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].platform == VRPlatform.None)
				{
					_globalDisplaySettings = array[i];
				}
				if (array[i].platform == TBCore.GetActivePlatform())
				{
					return array[i];
				}
			}
		}
		UnityEngine.Debug.LogError("TButt couldn't find saved DisplaySettings! Make sure you've run the Core Settings tool. Using defaults...");
		return GetDefaultDisplaySettings(TBCore.GetActivePlatform());
	}

	public static void SetRenderscale(float newScale)
	{
		_settingsBase.SetRenderscale(newScale);
	}

	public static void SetTimestep(float step, float maxStep)
	{
		Time.fixedDeltaTime = step;
		SafeDebug.Log("DANNY Setting Timestep to: " + step);
		Time.maximumDeltaTime = maxStep;
	}
}
