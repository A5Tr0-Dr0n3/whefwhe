using UnityEngine;

namespace Gvr.Internal;

internal static class ControllerProviderFactory
{
	internal static IControllerProvider CreateControllerProvider(GvrControllerInput owner)
	{
		Debug.LogWarning("No controller support on this platform.");
		return new DummyControllerProvider();
	}
}
