using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

namespace GoogleVR.Demos;

public class DemoInputManager : MonoBehaviour
{
	public enum EmulatedPlatformType
	{
		Daydream,
		Cardboard
	}

	private const string MESSAGE_CANVAS_NAME = "MessageCanvas";

	private const string MESSAGE_TEXT_NAME = "MessageText";

	private const string LASER_GAMEOBJECT_NAME = "Laser";

	private const string CONTROLLER_CONNECTING_MESSAGE = "Controller connecting...";

	private const string CONTROLLER_DISCONNECTED_MESSAGE = "Controller disconnected";

	private const string CONTROLLER_SCANNING_MESSAGE = "Controller scanning...";

	private const string NON_GVR_PLATFORM = "Please select a supported Google VR platform via 'Build Settings > Android | iOS > Switch Platform'\n";

	private const string VR_SUPPORT_NOT_CHECKED = "Please make sure 'Player Settings > Virtual Reality Supported' is checked\n";

	private const string EMPTY_VR_SDK_WARNING_MESSAGE = "Please add 'Daydream' or 'Cardboard' under 'Player Settings > Virtual Reality SDKs'\n";

	private const int ANDROID_MIN_DAYDREAM_API = 24;

	private const string FIELD_SDK_INT = "SDK_INT";

	private const string PACKAGE_BUILD_VERSION = "android.os.Build$VERSION";

	private const string PACKAGE_DAYDREAM_API_CLASS = "com.google.vr.ndk.base.DaydreamApi";

	private const string METHOD_IS_DAYDREAM_READY = "isDaydreamReadyPlatform";

	private bool isDaydream;

	[Tooltip("Reference to GvrControllerMain")]
	public GameObject controllerMain;

	public static string CONTROLLER_MAIN_PROP_NAME = "controllerMain";

	[Tooltip("Reference to GvrControllerPointer")]
	public GameObject controllerPointer;

	public static string CONTROLLER_POINTER_PROP_NAME = "controllerPointer";

	[Tooltip("Reference to GvrReticlePointer")]
	public GameObject reticlePointer;

	public static string RETICLE_POINTER_PROP_NAME = "reticlePointer";

	public GameObject messageCanvas;

	public Text messageText;

	[Tooltip("Emulated GVR Platform")]
	public EmulatedPlatformType gvrEmulatedPlatformType;

	public static string EMULATED_PLATFORM_PROP_NAME = "gvrEmulatedPlatformType";

	private void Start()
	{
		if (messageCanvas == null)
		{
			messageCanvas = transform.Find("MessageCanvas").gameObject;
			if (messageCanvas != null)
			{
				messageText = messageCanvas.transform.Find("MessageText").GetComponent<Text>();
			}
		}
		messageCanvas.SetActive(value: false);
		if (playerSettingsHasDaydream() || playerSettingsHasCardboard())
		{
			gvrEmulatedPlatformType = ((!(XRSettings.supportedDevices[0] == "daydream")) ? EmulatedPlatformType.Cardboard : EmulatedPlatformType.Daydream);
		}
		isDaydream = gvrEmulatedPlatformType == EmulatedPlatformType.Daydream;
		SetVRInputMechanism();
	}

	private void Update()
	{
		UpdateStatusMessage();
		UpdateEmulatedPlatformIfPlayerSettingsChanged();
		if ((!isDaydream || gvrEmulatedPlatformType != EmulatedPlatformType.Daydream) && (isDaydream || gvrEmulatedPlatformType != EmulatedPlatformType.Cardboard))
		{
			isDaydream = gvrEmulatedPlatformType == EmulatedPlatformType.Daydream;
			SetVRInputMechanism();
		}
	}

	public bool IsCurrentlyDaydream()
	{
		return isDaydream;
	}

	public static bool playerSettingsHasDaydream()
	{
		string[] supportedDevices = XRSettings.supportedDevices;
		return Array.Exists(supportedDevices, (string element) => element.Equals("daydream"));
	}

	public static bool playerSettingsHasCardboard()
	{
		string[] supportedDevices = XRSettings.supportedDevices;
		return Array.Exists(supportedDevices, (string element) => element.Equals("cardboard"));
	}

	private void UpdateEmulatedPlatformIfPlayerSettingsChanged()
	{
		if (playerSettingsHasDaydream() || playerSettingsHasCardboard())
		{
			if (gvrEmulatedPlatformType == EmulatedPlatformType.Daydream && !playerSettingsHasDaydream())
			{
				gvrEmulatedPlatformType = EmulatedPlatformType.Cardboard;
			}
			else if (gvrEmulatedPlatformType == EmulatedPlatformType.Cardboard && !playerSettingsHasCardboard())
			{
				gvrEmulatedPlatformType = EmulatedPlatformType.Daydream;
			}
		}
	}

	private void UpdateStatusMessage()
	{
		if (!(messageText == null) && !(messageCanvas == null))
		{
			messageText.text = "Please select a supported Google VR platform via 'Build Settings > Android | iOS > Switch Platform'\n";
			messageCanvas.SetActive(value: true);
		}
	}

	private void SetVRInputMechanism()
	{
		SetGazeInputActive(!isDaydream);
		SetControllerInputActive(isDaydream);
	}

	private void SetGazeInputActive(bool active)
	{
		if (reticlePointer == null)
		{
			return;
		}
		reticlePointer.SetActive(active);
		if (active)
		{
			GvrReticlePointer component = reticlePointer.GetComponent<GvrReticlePointer>();
			if (component != null)
			{
				GvrPointerInputModule.Pointer = component;
			}
		}
	}

	private void SetControllerInputActive(bool active)
	{
		if (controllerMain != null)
		{
			controllerMain.SetActive(active);
		}
		if (controllerPointer == null)
		{
			return;
		}
		controllerPointer.SetActive(active);
		if (active)
		{
			GvrLaserPointer componentInChildren = controllerPointer.GetComponentInChildren<GvrLaserPointer>(includeInactive: true);
			if (componentInChildren != null)
			{
				GvrPointerInputModule.Pointer = componentInChildren;
			}
		}
	}
}
