using UnityEngine;

namespace Gvr.Internal;

internal static class KeyboardProviderFactory
{
	internal static IKeyboardProvider CreateKeyboardProvider(GvrKeyboard owner)
	{
		Debug.LogWarning("GVR Keyboard not supported on " + Application.platform);
		return new DummyKeyboardProvider();
	}
}
