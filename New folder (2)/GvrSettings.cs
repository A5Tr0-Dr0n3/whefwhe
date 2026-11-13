using System;
using UnityEngine;
using UnityEngine.XR;

public static class GvrSettings
{
	public enum ViewerPlatformType
	{
		Error = -1,
		Cardboard,
		Daydream
	}

	public enum UserPrefsHandedness
	{
		Error = -1,
		Right,
		Left
	}

	public const string VR_SDK_NONE = "None";

	public const string VR_SDK_DAYDREAM = "daydream";

	public const string VR_SDK_CARDBOARD = "cardboard";

	private const string METHOD_GET_WINDOW = "getWindow";

	private const string METHOD_RUN_ON_UI_THREAD = "runOnUiThread";

	private const string METHOD_SET_SUSTAINED_PERFORMANCE_MODE = "setSustainedPerformanceMode";

	public static ViewerPlatformType ViewerPlatform => ViewerPlatformType.Error;

	public static bool SustainedPerformanceMode
	{
		set
		{
			SetSustainedPerformanceMode(value);
		}
	}

	public static UserPrefsHandedness Handedness => UserPrefsHandedness.Error;

	private static void SetSustainedPerformanceMode(bool enabled)
	{
	}

	public static IntPtr GetValidGvrNativePtrOrLogError()
	{
		if (!XRSettings.enabled)
		{
			Debug.LogError("VR is disabled");
			return IntPtr.Zero;
		}
		if (XRSettings.loadedDeviceName != "daydream" && XRSettings.loadedDeviceName != "cardboard")
		{
			Debug.LogErrorFormat("Loaded VR SDK '{0}' must be '{1}' or '{2}'", XRSettings.loadedDeviceName, "daydream", "cardboard");
			return IntPtr.Zero;
		}
		IntPtr nativePtr = XRDevice.GetNativePtr();
		if (nativePtr == IntPtr.Zero)
		{
			Debug.LogError("Unexpected zero GVR native context pointer");
			return nativePtr;
		}
		return nativePtr;
	}
}
