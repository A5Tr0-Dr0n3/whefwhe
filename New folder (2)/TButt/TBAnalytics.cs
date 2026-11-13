using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace TButt;

public static class TBAnalytics
{
	private static float sceneRealTime;

	private static float sceneGameTime;

	public static void SendEvent(string eventKey, Dictionary<string, object> eventContents)
	{
	}

	public static void OnStartup()
	{
		sceneGameTime = Time.time;
		sceneRealTime = Time.realtimeSinceStartup;
		SendEvent("PlayerSnapshot", new Dictionary<string, object>
		{
			{
				"Platform",
				GetCurrentPlatformName()
			},
			{
				"Language",
				Application.systemLanguage.ToString()
			},
			{
				"HMD",
				XRDevice.model.ToString()
			}
		});
		SendEvent(GetCurrentPlatformName() + "_HardwareSnapshot", new Dictionary<string, object>
		{
			{
				"Store",
				GetCurrentPlatformName()
			},
			{
				"PhoneModel",
				SystemInfo.deviceModel
			},
			{
				"GPU",
				SystemInfo.graphicsDeviceName
			},
			{
				"OperatingSystem",
				GetOSName()
			},
			{
				"Wifi",
				IsConnectedToWifi()
			},
			{
				"InstallMode",
				Application.installMode.ToString()
			},
			{
				"HMD",
				XRDevice.model.ToString()
			}
		});
	}

	public static float GetRealTimeSinceSceneLoad()
	{
		return Time.realtimeSinceStartup - sceneRealTime;
	}

	public static float GetGameTimeSinceSceneLoad()
	{
		return Time.time - sceneGameTime;
	}

	public static void ResetSceneTime()
	{
		sceneRealTime = Time.realtimeSinceStartup;
		sceneGameTime = Time.time;
	}

	private static string GetOSName()
	{
		string operatingSystem = SystemInfo.operatingSystem;
		if (operatingSystem.Contains("Windows 10"))
		{
			return "Windows 10";
		}
		if (operatingSystem.Contains("Windows 7"))
		{
			return "Windows 7";
		}
		if (operatingSystem.Contains("Windows 8"))
		{
			return "Windows 8";
		}
		return "Other";
	}

	private static string GetCurrentPlatformName()
	{
		return TBCore.GetActivePlatform() switch
		{
			VRPlatform.OculusMobile => "GearVR", 
			VRPlatform.OculusPC => "Oculus", 
			VRPlatform.Cardboard => "Cardboard", 
			VRPlatform.SteamVR => "Steam", 
			VRPlatform.PlayStationVR => "PlayStationVR", 
			VRPlatform.Daydream => "Daydream", 
			VRPlatform.WindowsMR => "WindowsMR", 
			_ => "Unknown", 
		};
	}

	public static string GetSemanticTime(float time)
	{
		if (time > 240f)
		{
			return "> 4 hours";
		}
		if ((double)time > 210.0)
		{
			return "> 3.5 hours";
		}
		if (time > 180f)
		{
			return "> 3 hours";
		}
		if ((double)time > 165.0)
		{
			return "> 2.75 hours";
		}
		if ((double)time > 150.0)
		{
			return "> 2.5 hours";
		}
		if ((double)time > 135.0)
		{
			return "> 2.25 hours";
		}
		if (time > 120f)
		{
			return "> 2 hours";
		}
		if ((double)time > 105.0)
		{
			return "> 1.75 hours";
		}
		if ((double)time > 90.0)
		{
			return "> 1.5 hours";
		}
		if ((double)time > 75.0)
		{
			return "> 1.25 hours";
		}
		if (time > 60f)
		{
			return "> 1 hour";
		}
		if (time > 50f)
		{
			return "> 50 minutes";
		}
		if (time > 40f)
		{
			return "> 40 minutes";
		}
		if (time > 30f)
		{
			return "> 30 minutes";
		}
		if (time > 25f)
		{
			return "> 25 minutes";
		}
		if (time > 20f)
		{
			return "> 20 minutes";
		}
		if (time > 15f)
		{
			return "> 15 minutes";
		}
		if (time > 10f)
		{
			return "> 10 minutes";
		}
		if (time > 5f)
		{
			return "> 5 minutes";
		}
		return "under 5 minutes";
	}

	private static bool IsConnectedToWifi()
	{
		return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
	}
}
