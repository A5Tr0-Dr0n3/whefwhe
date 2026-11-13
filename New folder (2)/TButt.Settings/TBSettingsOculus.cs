using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace TButt.Settings;

public class TBSettingsOculus : TBSettingsBase
{
	public override void SetRenderscale(float newScale)
	{
		if (TBCore.GetActivePlatform() != VRPlatform.OculusPC || !TBSettings.GetDisplaySettings().useAdaptiveResolution)
		{
			base.SetRenderscale(newScale);
		}
	}

	public override TBSettings.TBQualityLevel GetConfiguredQualityLevel()
	{
		if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
		{
			return TBSettings.GetDisplaySettings().minQualityLevel;
		}
		if (GetGearVRVersion() == OculusMobilePlatform.Galaxy2014)
		{
			return TBSettings.GetDisplaySettings().minQualityLevel;
		}
		return TBSettings.GetDisplaySettings().maxQualityLevel;
	}

	public static TBSettings.TBDisplaySettings GetDefaultDisplaySettings(TBSettings.TBDisplaySettings defaultDisplaySettings)
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusMobile:
			defaultDisplaySettings.minQualityLevel = TBSettings.TBQualityLevel.VeryLow;
			defaultDisplaySettings.maxQualityLevel = TBSettings.TBQualityLevel.Low;
			defaultDisplaySettings.maxTimestep = TBSettings.TBMaxTimestep.Half;
			defaultDisplaySettings.opaqueSortMode = OpaqueSortMode.NoDistanceSort;
			defaultDisplaySettings.maxRenderScale = 1.2f;
			defaultDisplaySettings.minRenderScale = 1f;
			break;
		case VRPlatform.OculusPC:
			defaultDisplaySettings.useAdaptiveResolution = true;
			defaultDisplaySettings.maxRenderScale = 1.2f;
			defaultDisplaySettings.minRenderScale = 1f;
			break;
		}
		return defaultDisplaySettings;
	}

	public static OculusMobilePlatform GetGearVRVersion()
	{
		switch (GetGearVRDeviceName())
		{
		case "Galaxy Note 4":
			return OculusMobilePlatform.Galaxy2014;
		case "Galaxy S6":
		case "Galaxy S6 Edge":
		case "Galaxy S6 Edge+":
		case "Galaxy Note 5":
			return OculusMobilePlatform.Galaxy2014;
		case "Galaxy S7 Edge":
		case "Galaxy S7":
			return OculusMobilePlatform.Galaxy2016;
		case "Galaxy S8":
		case "Galaxy S8+":
			return OculusMobilePlatform.Galaxy2016;
		case "Oculus Go":
			return OculusMobilePlatform.Go;
		default:
			return OculusMobilePlatform.Go;
		}
	}

	public static string GetGearVRDeviceName()
	{
		string deviceName = SystemInfo.deviceName;
		string deviceModel = SystemInfo.deviceModel;
		if (deviceModel.Contains("SM-N910"))
		{
			return "Galaxy Note 4";
		}
		if (deviceModel.Contains("SM-G920"))
		{
			return "Galaxy S6";
		}
		if (deviceModel.Contains("SM-G925"))
		{
			return "Galaxy S6 Edge";
		}
		if (deviceModel.Contains("SM-G928"))
		{
			return "Galaxy S6 Edge+";
		}
		if (deviceModel.Contains("SM-N920"))
		{
			return "Galaxy Note 5";
		}
		if (deviceModel.Contains("SM-G935"))
		{
			return "Galaxy S7 Edge";
		}
		if (deviceModel.Contains("SM-G930"))
		{
			return "Galaxy S7";
		}
		if (deviceModel.Contains("SM-G950"))
		{
			return "Galaxy S8";
		}
		if (deviceModel.Contains("SM-G955"))
		{
			return "Galaxy S8+";
		}
		if (deviceModel.Contains("Oculus Pacific"))
		{
			return "Oculus Go";
		}
		return "Other 2017+ Device";
	}

	public override float GetRefreshRate()
	{
		if (base.GetRefreshRate() == 0f)
		{
			if (XRDevice.model.Contains("DK2"))
			{
				return 75f;
			}
			if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
			{
				return 90f;
			}
			return 60f;
		}
		return base.GetRefreshRate();
	}
}
