using System;
using UnityEngine;

public class GvrDaydreamApi : IDisposable
{
	private const string METHOD_CREATE = "create";

	private const string METHOD_LAUNCH_VR_HOMESCREEN = "launchVrHomescreen";

	private const string METHOD_RUN_ON_UI_THREAD = "runOnUiThread";

	private const string PACKAGE_DAYDREAM_API = "com.google.vr.ndk.base.DaydreamApi";

	private static GvrDaydreamApi m_instance;

	public static bool IsCreated => m_instance != null;

	private static void EnsureCreated(Action<bool> callback)
	{
		if (!IsCreated)
		{
			CreateAsync(callback);
		}
		else
		{
			callback(obj: true);
		}
	}

	public void Dispose()
	{
		m_instance = null;
	}

	[Obsolete("Create() without arguments is deprecated. Use CreateAsync(callback) instead.")]
	public static void Create()
	{
		CreateAsync(null);
	}

	public static void CreateAsync(Action<bool> callback)
	{
		if (m_instance == null)
		{
			m_instance = new GvrDaydreamApi();
		}
	}

	[Obsolete("LaunchVrHome() deprecated. Use LaunchVrHomeAsync(callback) instead.")]
	public static void LaunchVrHome()
	{
		LaunchVrHomeAsync(null);
	}

	public static void LaunchVrHomeAsync(Action<bool> callback)
	{
		EnsureCreated((bool success) =>
		{
			if (success)
			{
				Debug.LogWarning("Launching VR Home is only possible on Android devices.");
			}
			if (callback != null)
			{
				callback(success);
			}
		});
	}
}
