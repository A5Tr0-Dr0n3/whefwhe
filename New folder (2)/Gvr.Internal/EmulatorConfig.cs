using UnityEngine;

namespace Gvr.Internal;

internal class EmulatorConfig : MonoBehaviour
{
	public enum Mode
	{
		OFF,
		USB,
		WIFI
	}

	private static EmulatorConfig instance;

	public Mode PHONE_EVENT_MODE = Mode.USB;

	public static readonly string USB_SERVER_IP = "127.0.0.1";

	public static readonly string WIFI_SERVER_IP = "192.168.43.1";

	public static EmulatorConfig Instance
	{
		get
		{
			if (instance == null)
			{
				EmulatorConfig[] array = (EmulatorConfig[])Object.FindObjectsOfType(typeof(EmulatorConfig));
				if (array.Length == 1)
				{
					instance = array[0];
				}
				else if (array.Length > 1)
				{
					Debug.LogError("Multiple PhoneRemote/Config objects in scene. Ignoring all.");
				}
			}
			if (instance == null)
			{
				GameObject gameObject = new GameObject("PhoneRemoteConfig");
				instance = gameObject.AddComponent<EmulatorConfig>();
				Object.DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}
}
