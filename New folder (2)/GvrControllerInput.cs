using System;
using Gvr.Internal;
using UnityEngine;

public class GvrControllerInput : MonoBehaviour
{
	public delegate void OnStateChangedEvent(GvrConnectionState state, GvrConnectionState oldState);

	public enum EmulatorConnectionMode
	{
		OFF,
		USB,
		WIFI
	}

	private static GvrControllerInput instance;

	private static IControllerProvider controllerProvider;

	private ControllerState controllerState = new ControllerState();

	private Vector2 touchPosCentered = Vector2.zero;

	private int lastUpdatedFrameCount = -1;

	[GvrInfo("Hold Shift to use the Mouse as the controller.\n\nControls:  Shift +\n   • Move Mouse = Change Orientation\n   • Left Mouse Button = ClickButton\n   • Right Mouse Button = AppButton\n   • Middle Mouse Button = HomeButton/Recenter\n   • Ctrl = IsTouching\n   • Ctrl + Move Mouse = Change TouchPos", 8)]
	[Tooltip("How to connect to the emulator: USB cable (recommended) or WIFI.")]
	public EmulatorConnectionMode emulatorConnectionMode = EmulatorConnectionMode.USB;

	public static GvrConnectionState State
	{
		get
		{
			if (instance == null)
			{
				return GvrConnectionState.Error;
			}
			instance.Update();
			return instance.controllerState.connectionState;
		}
	}

	public static GvrControllerApiStatus ApiStatus
	{
		get
		{
			if (instance == null)
			{
				return GvrControllerApiStatus.Error;
			}
			instance.Update();
			return instance.controllerState.apiStatus;
		}
	}

	public static bool SupportsBatteryStatus
	{
		get
		{
			if (controllerProvider == null || instance == null)
			{
				return false;
			}
			instance.Update();
			return controllerProvider.SupportsBatteryStatus;
		}
	}

	public static Quaternion Orientation
	{
		get
		{
			if (instance == null)
			{
				return Quaternion.identity;
			}
			instance.Update();
			return instance.controllerState.orientation;
		}
	}

	public static Vector3 Gyro
	{
		get
		{
			if (instance == null)
			{
				return Vector3.zero;
			}
			instance.Update();
			return instance.controllerState.gyro;
		}
	}

	public static Vector3 Accel
	{
		get
		{
			if (instance == null)
			{
				return Vector3.zero;
			}
			instance.Update();
			return instance.controllerState.accel;
		}
	}

	public static bool IsTouching
	{
		get
		{
			if (instance == null)
			{
				return false;
			}
			instance.Update();
			return instance.controllerState.isTouching;
		}
	}

	public static bool TouchDown
	{
		get
		{
			if (instance == null)
			{
				return false;
			}
			instance.Update();
			return instance.controllerState.touchDown;
		}
	}

	public static bool TouchUp
	{
		get
		{
			if (instance == null)
			{
				return false;
			}
			instance.Update();
			return instance.controllerState.touchUp;
		}
	}

	public static Vector2 TouchPos
	{
		get
		{
			if (instance == null)
			{
				return new Vector2(0.5f, 0.5f);
			}
			instance.Update();
			return instance.controllerState.touchPos;
		}
	}

	public static Vector2 TouchPosCentered
	{
		get
		{
			if (instance == null)
			{
				return Vector2.zero;
			}
			instance.Update();
			return instance.touchPosCentered;
		}
	}

	[Obsolete("Use Recentered to detect when user has completed the recenter gesture.")]
	public static bool Recentering => false;

	public static bool Recentered
	{
		get
		{
			if (instance == null)
			{
				return false;
			}
			instance.Update();
			return instance.controllerState.recentered;
		}
	}

	public static bool ClickButton
	{
		get
		{
			if (instance == null)
			{
				return false;
			}
			instance.Update();
			return instance.controllerState.clickButtonState;
		}
	}

	public static bool ClickButtonDown
	{
		get
		{
			if (instance == null)
			{
				return false;
			}
			instance.Update();
			return instance.controllerState.clickButtonDown;
		}
	}

	public static bool ClickButtonUp
	{
		get
		{
			if (instance == null)
			{
				return false;
			}
			instance.Update();
			return instance.controllerState.clickButtonUp;
		}
	}

	public static bool AppButton
	{
		get
		{
			if (instance == null)
			{
				return false;
			}
			instance.Update();
			return instance.controllerState.appButtonState;
		}
	}

	public static bool AppButtonDown
	{
		get
		{
			if (instance == null)
			{
				return false;
			}
			instance.Update();
			return instance.controllerState.appButtonDown;
		}
	}

	public static bool AppButtonUp
	{
		get
		{
			if (instance == null)
			{
				return false;
			}
			instance.Update();
			return instance.controllerState.appButtonUp;
		}
	}

	public static bool HomeButtonDown
	{
		get
		{
			if (instance == null)
			{
				return false;
			}
			instance.Update();
			return instance.controllerState.homeButtonDown;
		}
	}

	public static bool HomeButtonState
	{
		get
		{
			if (instance == null)
			{
				return false;
			}
			instance.Update();
			return instance.controllerState.homeButtonState;
		}
	}

	public static string ErrorDetails
	{
		get
		{
			if (instance != null)
			{
				instance.Update();
				return (instance.controllerState.connectionState != GvrConnectionState.Error) ? string.Empty : instance.controllerState.errorDetails;
			}
			return "GvrController instance not found in scene. It may be missing, or it might not have initialized yet.";
		}
	}

	public static IntPtr StatePtr
	{
		get
		{
			if (instance == null)
			{
				return IntPtr.Zero;
			}
			instance.Update();
			return instance.controllerState.gvrPtr;
		}
	}

	public static bool IsCharging
	{
		get
		{
			if (instance == null)
			{
				return false;
			}
			instance.Update();
			return instance.controllerState.isCharging;
		}
	}

	public static GvrControllerBatteryLevel BatteryLevel
	{
		get
		{
			if (instance == null)
			{
				return GvrControllerBatteryLevel.Error;
			}
			instance.Update();
			return instance.controllerState.batteryLevel;
		}
	}

	public static event Action OnControllerInputUpdated;

	public static event Action OnPostControllerInputUpdated;

	public static event OnStateChangedEvent OnStateChanged;

	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogError("More than one GvrController instance was found in your scene. Ensure that there is only one GvrControllerInput.");
			enabled = false;
			return;
		}
		instance = this;
		if (controllerProvider == null)
		{
			controllerProvider = ControllerProviderFactory.CreateControllerProvider(this);
		}
		Screen.sleepTimeout = -1;
	}

	private void Update()
	{
		if (lastUpdatedFrameCount != Time.frameCount)
		{
			lastUpdatedFrameCount = Time.frameCount;
			GvrConnectionState state = State;
			controllerProvider.ReadState(controllerState);
			UpdateTouchPosCentered();
			if (OnStateChanged != null && State != state)
			{
				OnStateChanged(State, state);
			}
			if (OnControllerInputUpdated != null)
			{
				OnControllerInputUpdated();
			}
			if (OnPostControllerInputUpdated != null)
			{
				OnPostControllerInputUpdated();
			}
		}
	}

	private void OnDestroy()
	{
		instance = null;
	}

	private void OnApplicationPause(bool paused)
	{
		if (controllerProvider != null)
		{
			if (paused)
			{
				controllerProvider.OnPause();
			}
			else
			{
				controllerProvider.OnResume();
			}
		}
	}

	private void UpdateTouchPosCentered()
	{
		if (!(instance == null))
		{
			touchPosCentered.x = (instance.controllerState.touchPos.x - 0.5f) * 2f;
			touchPosCentered.y = (0f - (instance.controllerState.touchPos.y - 0.5f)) * 2f;
			float magnitude = touchPosCentered.magnitude;
			if (magnitude > 1f)
			{
				touchPosCentered.x /= magnitude;
				touchPosCentered.y /= magnitude;
			}
		}
	}
}
