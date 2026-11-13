using UnityEngine;

namespace Gvr.Internal;

internal static class HeadsetProviderFactory
{
	internal static IHeadsetProvider CreateProvider()
	{
		Debug.LogWarning(string.Concat("No Google VR standalone headset / 6DOF support on ", Application.platform, " platform."));
		return new DummyHeadsetProvider();
	}
}
