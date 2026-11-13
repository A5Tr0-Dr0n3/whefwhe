using System;
using System.Collections.Generic;
using TButt.InputMaps;
using UnityEngine;

namespace TButt;

public static class TBInput
{
	[Flags]
	public enum Button
	{
		None = 0,
		PrimaryTrigger = 1,
		SecondaryTrigger = 2,
		Start = 0x10,
		Options = 0x20,
		Action1 = 0x100,
		Action2 = 0x200,
		Action3 = 0x400,
		Action4 = 0x800,
		DpadUp = 0x1000,
		DpadDown = 0x2000,
		DpadLeft = 0x4000,
		DpadRight = 0x8000,
		Touchpad = 0x10000,
		Joystick = 0x20000,
		LeftStick = 0x40000,
		RightStick = 0x80000,
		LeftTrigger = 0x100000,
		RightTrigger = 0x200000,
		LeftBumper = 0x400000,
		RightBumper = 0x800000,
		Any = -1
	}

	[Flags]
	public enum Controller
	{
		None = 0,
		LHandController = 1,
		RHandController = 2,
		ClickRemote = LHandController | RHandController,
		Mobile3DOFController = 4,
		Gamepad = LHandController | Mobile3DOFController,
		Active = RHandController | Mobile3DOFController
	}

	[Flags]
	public enum ControlType
	{
		None = 0,
		Gamepad = 1,
		HandControllers = 2,
		Mobile3DOFController = Gamepad | HandControllers,
		ClickRemote = 4
	}

	public enum Mobile3DOFHandedness
	{
		Left,
		Right,
		Center
	}

	public enum RumblePusleLength
	{
		Short,
		Medium,
		Long
	}

	public enum RumblePulseStrength
	{
		Low,
		Medium,
		High
	}

	public static class Events
	{
		public delegate void ButtonEvent();

		public delegate void ControlTypeEvent(ControlType type);

		public static class Internal
		{
			public static void SendControllerConnectionEvent(ControlType controlType, bool connected)
			{
				if (connected)
				{
					if (OnControllerConnected != null)
					{
						OnControllerConnected(controlType);
					}
				}
				else if (OnControllerDisconnected != null)
				{
					OnControllerDisconnected(controlType);
				}
			}

			public static void UpdateControlType(ControlType type)
			{
				if (OnControlTypeChanged != null)
				{
					OnControlTypeChanged(type);
				}
			}
		}

		public static class Button
		{
			public static event ButtonEvent OnPrimaryTriggerButtonDown;

			public static event ButtonEvent OnPrimaryTriggerButtonUp;

			public static event ButtonEvent OnSecondaryTriggerButtonDown;

			public static event ButtonEvent OnSecondaryTriggerButtonUp;

			public static event ButtonEvent OnStartButtonDown;

			public static event ButtonEvent OnStartButtonUp;

			public static event ButtonEvent OnOptionsButtonDown;

			public static event ButtonEvent OnOptionsButtonUp;

			public static event ButtonEvent OnAction1ButtonDown;

			public static event ButtonEvent OnAction1ButtonUp;

			public static event ButtonEvent OnAction2ButtonDown;

			public static event ButtonEvent OnAction2ButtonUp;

			public static event ButtonEvent OnAction3ButtonDown;

			public static event ButtonEvent OnAction3ButtonUp;

			public static event ButtonEvent OnAction4ButtonDown;

			public static event ButtonEvent OnAction4ButtonUp;

			public static void UpdateButtonEvents(Controller controller)
			{
				if (OnAction1ButtonDown != null && GetButtonDown(TBInput.Button.Action1, controller))
				{
					OnAction1ButtonDown();
				}
				if (OnAction1ButtonUp != null && GetButtonUp(TBInput.Button.Action1, controller))
				{
					OnAction1ButtonUp();
				}
				if (OnAction2ButtonDown != null && GetButtonDown(TBInput.Button.Action2, controller))
				{
					OnAction2ButtonDown();
				}
				if (OnAction2ButtonUp != null && GetButtonUp(TBInput.Button.Action2, controller))
				{
					OnAction2ButtonUp();
				}
				if (OnAction3ButtonDown != null && GetButtonDown(TBInput.Button.Action3, controller))
				{
					OnAction3ButtonDown();
				}
				if (OnAction3ButtonUp != null && GetButtonUp(TBInput.Button.Action3, controller))
				{
					OnAction3ButtonUp();
				}
				if (OnAction4ButtonDown != null && GetButtonDown(TBInput.Button.Action4, controller))
				{
					OnAction4ButtonDown();
				}
				if (OnAction4ButtonUp != null && GetButtonUp(TBInput.Button.Action4, controller))
				{
					OnAction4ButtonUp();
				}
				if (OnPrimaryTriggerButtonDown != null && GetButtonDown(TBInput.Button.PrimaryTrigger, controller))
				{
					OnPrimaryTriggerButtonDown();
				}
				if (OnPrimaryTriggerButtonUp != null && GetButtonUp(TBInput.Button.PrimaryTrigger, controller))
				{
					OnPrimaryTriggerButtonUp();
				}
				if (OnSecondaryTriggerButtonDown != null && GetButtonDown(TBInput.Button.SecondaryTrigger, controller))
				{
					OnSecondaryTriggerButtonDown();
				}
				if (OnSecondaryTriggerButtonUp != null && GetButtonUp(TBInput.Button.SecondaryTrigger, controller))
				{
					OnSecondaryTriggerButtonUp();
				}
				if (OnStartButtonDown != null && GetButtonDown(TBInput.Button.Start, controller))
				{
					OnStartButtonDown();
				}
				if (OnStartButtonUp != null && GetButtonUp(TBInput.Button.Start, controller))
				{
					OnStartButtonUp();
				}
				if (OnOptionsButtonDown != null && GetButtonDown(TBInput.Button.Options, controller))
				{
					OnOptionsButtonDown();
				}
				if (OnOptionsButtonUp != null && GetButtonUp(TBInput.Button.Options, controller))
				{
					OnOptionsButtonUp();
				}
			}
		}

		public static class Touch
		{
			public static event ButtonEvent OnPrimaryTriggerTouchDown;

			public static event ButtonEvent OnPrimaryTriggerTouchUp;

			public static event ButtonEvent OnSecondaryTriggerTouchDown;

			public static event ButtonEvent OnSecondaryTriggerTouchUp;

			public static event ButtonEvent OnStartTouchDown;

			public static event ButtonEvent OnStartTouchUp;

			public static event ButtonEvent OnOptionsTouchDown;

			public static event ButtonEvent OnOptionsTouchUp;

			public static event ButtonEvent OnAction1TouchDown;

			public static event ButtonEvent OnAction1TouchUp;

			public static event ButtonEvent OnAction2TouchDown;

			public static event ButtonEvent OnAction2TouchUp;

			public static event ButtonEvent OnAction3TouchDown;

			public static event ButtonEvent OnAction3TouchUp;

			public static event ButtonEvent OnAction4TouchDown;

			public static event ButtonEvent OnAction4TouchUp;

			public static void UpdateButtonEvents(Controller controller)
			{
				if (OnAction1TouchDown != null && GetTouchDown(TBInput.Button.Action1, controller))
				{
					OnAction1TouchDown();
				}
				if (OnAction1TouchUp != null && GetTouchUp(TBInput.Button.Action1, controller))
				{
					OnAction1TouchUp();
				}
				if (OnAction2TouchDown != null && GetTouchDown(TBInput.Button.Action2, controller))
				{
					OnAction2TouchDown();
				}
				if (OnAction2TouchUp != null && GetTouchUp(TBInput.Button.Action2, controller))
				{
					OnAction2TouchUp();
				}
				if (OnAction3TouchDown != null && GetTouchDown(TBInput.Button.Action3, controller))
				{
					OnAction3TouchDown();
				}
				if (OnAction3TouchUp != null && GetTouchUp(TBInput.Button.Action3, controller))
				{
					OnAction3TouchUp();
				}
				if (OnAction4TouchDown != null && GetTouchDown(TBInput.Button.Action4, controller))
				{
					OnAction4TouchDown();
				}
				if (OnAction4TouchUp != null && GetTouchUp(TBInput.Button.Action4, controller))
				{
					OnAction4TouchUp();
				}
				if (OnPrimaryTriggerTouchDown != null && GetTouchDown(TBInput.Button.PrimaryTrigger, controller))
				{
					OnPrimaryTriggerTouchDown();
				}
				if (OnPrimaryTriggerTouchUp != null && GetTouchUp(TBInput.Button.PrimaryTrigger, controller))
				{
					OnPrimaryTriggerTouchUp();
				}
				if (OnSecondaryTriggerTouchDown != null && GetTouchDown(TBInput.Button.SecondaryTrigger, controller))
				{
					OnSecondaryTriggerTouchDown();
				}
				if (OnSecondaryTriggerTouchUp != null && GetTouchUp(TBInput.Button.SecondaryTrigger, controller))
				{
					OnSecondaryTriggerTouchUp();
				}
				if (OnStartTouchDown != null && GetTouchDown(TBInput.Button.Start, controller))
				{
					OnStartTouchDown();
				}
				if (OnStartTouchUp != null && GetTouchUp(TBInput.Button.Start, controller))
				{
					OnStartTouchUp();
				}
				if (OnOptionsTouchDown != null && GetTouchDown(TBInput.Button.Options, controller))
				{
					OnOptionsTouchDown();
				}
				if (OnOptionsTouchUp != null && GetTouchUp(TBInput.Button.Options, controller))
				{
					OnOptionsTouchUp();
				}
			}
		}

		public static event ControlTypeEvent OnControlTypeChanged;

		public static event ControlTypeEvent OnControllerConnected;

		public static event ControlTypeEvent OnControllerDisconnected;

		public static void UpdateEvents()
		{
			ControlType activeControlType = GetActiveControlType();
			if (activeControlType == ControlType.HandControllers)
			{
				Button.UpdateButtonEvents(Controller.LHandController);
				Button.UpdateButtonEvents(Controller.RHandController);
				Touch.UpdateButtonEvents(Controller.LHandController);
				Touch.UpdateButtonEvents(Controller.RHandController);
			}
			else
			{
				Button.UpdateButtonEvents(Controller.Active);
				Touch.UpdateButtonEvents(Controller.Active);
			}
		}
	}

	[Serializable]
	public class ButtonDef<T>
	{
		[SerializeField]
		public T rawButton;

		[SerializeField]
		public Button[] virtualButtons;

		[SerializeField]
		public string name;

		[SerializeField]
		public bool supportsTouch;

		[SerializeField]
		public bool supportsAxis1D;

		[SerializeField]
		public bool supportsAxis2D;

		[SerializeField]
		public bool supportsButton = true;
	}

	[Serializable]
	public class SerializedButtonDef
	{
		[SerializeField]
		public Button[] virtualButtons;
	}

	public struct ButtonLookupTable<T>
	{
		public T[] PrimaryTrigger;

		public T[] SecondaryTrigger;

		public T[] Start;

		public T[] Options;

		public T[] Action1;

		public T[] Action2;

		public T[] Action3;

		public T[] Action4;

		public T[] DpadUp;

		public T[] DpadDown;

		public T[] DpadLeft;

		public T[] DpadRight;

		public T[] Touchpad;

		public T[] Joystick;

		public T[] LeftStick;

		public T[] RightStick;

		public T[] LeftTrigger;

		public T[] RightTrigger;

		public T[] LeftBumper;

		public T[] RightBumper;

		public T[] Any;
	}

	public static ControlType connectedControlTypes;

	private static Transform _leftHand;

	private static Transform _rightHand;

	private static Transform _3DOFHand;

	private static float rumbleStrengthLow = 0.1f;

	private static float rumbleStrengthMed = 0.6f;

	private static float rumbleStrengthHi = 1f;

	private static float rumbleLengthShort = 0.05f;

	private static float rumbleLengthMed = 0.3f;

	private static float rumbleLengthLong = 0.6f;

	private static ControlType _activeControlType;

	public static void Initialize(VRPlatform platform)
	{
		TBCore.OnUpdate = (TBCore.TBCoreEvent)Delegate.Combine(TBCore.OnUpdate, new TBCore.TBCoreEvent(Update));
		switch (platform)
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			TBOculusInput.instance.Initialize();
			TBCore.OnFixedUpdate = (TBCore.TBCoreEvent)Delegate.Combine(TBCore.OnFixedUpdate, new TBCore.TBCoreEvent(TBOculusInput.instance.FixedUpdate));
			TBCore.OnUpdate = (TBCore.TBCoreEvent)Delegate.Combine(TBCore.OnUpdate, new TBCore.TBCoreEvent(TBOculusInput.instance.Update));
			break;
		case VRPlatform.SteamVR:
			TBSteamVRInput.instance.Initialize();
			TBCore.OnFixedUpdate = (TBCore.TBCoreEvent)Delegate.Combine(TBCore.OnFixedUpdate, new TBCore.TBCoreEvent(TBSteamVRInput.instance.FixedUpdate));
			TBCore.OnUpdate = (TBCore.TBCoreEvent)Delegate.Combine(TBCore.OnUpdate, new TBCore.TBCoreEvent(TBSteamVRInput.instance.Update));
			if (TBSettings.GetControlSettings().supportsGamepad)
			{
				TBXInput.instance.Initialize();
				TBCore.OnUpdate = (TBCore.TBCoreEvent)Delegate.Combine(TBCore.OnUpdate, new TBCore.TBCoreEvent(TBXInput.instance.Update));
			}
			break;
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			TBGoogleInput.instance.Initialize();
			TBCore.OnFixedUpdate = (TBCore.TBCoreEvent)Delegate.Combine(TBCore.OnFixedUpdate, new TBCore.TBCoreEvent(TBGoogleInput.instance.FixedUpdate));
			TBCore.OnUpdate = (TBCore.TBCoreEvent)Delegate.Combine(TBCore.OnUpdate, new TBCore.TBCoreEvent(TBGoogleInput.instance.Update));
			break;
		case VRPlatform.PlayStationVR:
			TBPSVRInput.instance.Initialize();
			TBCore.OnFixedUpdate = (TBCore.TBCoreEvent)Delegate.Combine(TBCore.OnFixedUpdate, new TBCore.TBCoreEvent(TBPSVRInput.instance.FixedUpdate));
			TBCore.OnUpdate = (TBCore.TBCoreEvent)Delegate.Combine(TBCore.OnUpdate, new TBCore.TBCoreEvent(TBPSVRInput.instance.Update));
			break;
		case VRPlatform.WindowsMR:
			TBWindowsMRInput.instance.Initialize();
			TBCore.OnFixedUpdate = (TBCore.TBCoreEvent)Delegate.Combine(TBCore.OnFixedUpdate, new TBCore.TBCoreEvent(TBWindowsMRInput.instance.FixedUpdate));
			TBCore.OnUpdate = (TBCore.TBCoreEvent)Delegate.Combine(TBCore.OnUpdate, new TBCore.TBCoreEvent(TBWindowsMRInput.instance.Update));
			if (TBSettings.GetControlSettings().supportsGamepad)
			{
				TBXInput.instance.Initialize();
				TBCore.OnUpdate = (TBCore.TBCoreEvent)Delegate.Combine(TBCore.OnUpdate, new TBCore.TBCoreEvent(TBXInput.instance.Update));
			}
			break;
		}
	}

	public static void Update()
	{
		if (TBSettings.GetControlSettings().useInputEvents)
		{
			Events.UpdateEvents();
		}
	}

	public static Controller GetActiveController()
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.GetActiveController();
		case VRPlatform.SteamVR:
			return TBSteamVRInput.instance.GetActiveController();
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.GetActiveController();
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.GetActiveController();
		case VRPlatform.WindowsMR:
			return TBWindowsMRInput.instance.GetActiveController();
		default:
			return Controller.None;
		}
	}

	public static ControlType GetActiveControlType()
	{
		return _activeControlType;
	}

	public static void SetActiveControlType(ControlType controlType)
	{
		if (_activeControlType != controlType)
		{
			_activeControlType = controlType;
			Events.Internal.UpdateControlType(_activeControlType);
		}
	}

	public static string GetControllerName(Controller controller)
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.GetControllerName(controller);
		case VRPlatform.SteamVR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.GetControllerName(controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.GetControllerName(controller);
				}
				break;
			}
			return TBSteamVRInput.instance.GetControllerName(controller);
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.GetControllerName(controller);
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.GetControllerName(controller);
		case VRPlatform.WindowsMR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.GetControllerName(controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.GetControllerName(controller);
				}
				break;
			}
			return TBWindowsMRInput.instance.GetControllerName(controller);
		default:
			return string.Empty;
		}
	}

	public static bool ControllerHasRumbleSupport(Controller controller)
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.ControllerHasRumbleSupport(controller);
		case VRPlatform.SteamVR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.ControllerHasRumbleSupport(controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.ControllerHasRumbleSupport(controller);
				}
				break;
			}
			return TBSteamVRInput.instance.ControllerHasRumbleSupport(controller);
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.ControllerHasRumbleSupport(controller);
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.ControllerHasRumbleSupport(controller);
		case VRPlatform.WindowsMR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.ControllerHasRumbleSupport(controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.ControllerHasRumbleSupport(controller);
				}
				break;
			}
			return TBWindowsMRInput.instance.ControllerHasRumbleSupport(controller);
		default:
			return false;
		}
	}

	public static Quaternion GetPointerRotation(Controller controller)
	{
		return GetRotation(controller) * GetPointerRotationFromController(controller);
	}

	private static Quaternion GetPointerRotationFromController(Controller controller)
	{
		float angle = 0f;
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			angle = TBOculusInput.instance.GetPointerTiltAngle(controller);
			break;
		case VRPlatform.SteamVR:
			switch (controller)
			{
			case Controller.Gamepad:
				angle = TBXInput.instance.GetPointerTiltAngle(controller);
				break;
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					angle = TBXInput.instance.GetPointerTiltAngle(controller);
				}
				break;
			default:
				angle = TBSteamVRInput.instance.GetPointerTiltAngle(controller);
				break;
			}
			break;
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			angle = TBGoogleInput.instance.GetPointerTiltAngle(controller);
			break;
		case VRPlatform.PlayStationVR:
			angle = TBPSVRInput.instance.GetPointerTiltAngle(controller);
			break;
		case VRPlatform.WindowsMR:
			switch (controller)
			{
			case Controller.Gamepad:
				angle = TBXInput.instance.GetPointerTiltAngle(controller);
				break;
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					angle = TBXInput.instance.GetPointerTiltAngle(controller);
				}
				break;
			default:
				angle = TBWindowsMRInput.instance.GetPointerTiltAngle(controller);
				break;
			}
			break;
		default:
			angle = 0f;
			break;
		}
		return Quaternion.AngleAxis(angle, Vector3.right);
	}

	public static bool GetButton(Button button, Controller controller)
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.GetButton(button, controller);
		case VRPlatform.SteamVR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.GetButton(button, controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.GetButton(button, controller);
				}
				break;
			}
			return TBSteamVRInput.instance.GetButton(button, controller);
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.GetButton(button, controller);
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.GetButton(button, controller);
		case VRPlatform.WindowsMR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.GetButton(button, controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.GetButton(button, controller);
				}
				break;
			}
			return TBWindowsMRInput.instance.GetButton(button, controller);
		default:
			return false;
		}
	}

	public static bool GetButtonUp(Button button, Controller controller)
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.GetButtonUp(button, controller);
		case VRPlatform.SteamVR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.GetButtonUp(button, controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.GetButtonUp(button, controller);
				}
				break;
			}
			return TBSteamVRInput.instance.GetButtonUp(button, controller);
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.GetButtonUp(button, controller);
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.GetButtonUp(button, controller);
		case VRPlatform.WindowsMR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.GetButtonUp(button, controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.GetButtonUp(button, controller);
				}
				break;
			}
			return TBWindowsMRInput.instance.GetButtonUp(button, controller);
		default:
			return false;
		}
	}

	public static bool GetButtonDown(Button button, Controller controller)
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.GetButtonDown(button, controller);
		case VRPlatform.SteamVR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.GetButtonDown(button, controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.GetButtonDown(button, controller);
				}
				break;
			}
			return TBSteamVRInput.instance.GetButtonDown(button, controller);
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.GetButtonDown(button, controller);
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.GetButtonDown(button, controller);
		case VRPlatform.WindowsMR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.GetButtonDown(button, controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.GetButtonDown(button, controller);
				}
				break;
			}
			return TBWindowsMRInput.instance.GetButtonDown(button, controller);
		default:
			return false;
		}
	}

	public static bool GetTouch(Button button, Controller controller)
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.GetTouch(button, controller);
		case VRPlatform.SteamVR:
			return TBSteamVRInput.instance.GetTouch(button, controller);
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.GetTouch(button, controller);
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.GetTouch(button, controller);
		case VRPlatform.WindowsMR:
			return TBWindowsMRInput.instance.GetTouch(button, controller);
		default:
			return false;
		}
	}

	public static bool GetTouchUp(Button button, Controller controller)
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.GetTouchUp(button, controller);
		case VRPlatform.SteamVR:
			return TBSteamVRInput.instance.GetTouchUp(button, controller);
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.GetTouchUp(button, controller);
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.GetTouchUp(button, controller);
		case VRPlatform.WindowsMR:
			return TBWindowsMRInput.instance.GetTouchUp(button, controller);
		default:
			return false;
		}
	}

	public static bool GetTouchDown(Button button, Controller controller)
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.GetTouchDown(button, controller);
		case VRPlatform.SteamVR:
			return TBSteamVRInput.instance.GetTouchDown(button, controller);
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.GetTouchDown(button, controller);
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.GetTouchDown(button, controller);
		case VRPlatform.WindowsMR:
			return TBWindowsMRInput.instance.GetTouchDown(button, controller);
		default:
			return false;
		}
	}

	public static Vector2 GetAxis2D(Button button, Controller controller)
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.GetAxis2D(button, controller);
		case VRPlatform.SteamVR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.GetAxis2D(button, controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.GetAxis2D(button, controller);
				}
				break;
			}
			return TBSteamVRInput.instance.GetAxis2D(button, controller);
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.GetAxis2D(button, controller);
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.GetAxis2D(button, controller);
		case VRPlatform.WindowsMR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.GetAxis2D(button, controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.GetAxis2D(button, controller);
				}
				break;
			}
			return TBWindowsMRInput.instance.GetAxis2D(button, controller);
		default:
			return Vector2.zero;
		}
	}

	public static float GetAxis1D(Button button, Controller controller)
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.GetAxis1D(button, controller);
		case VRPlatform.SteamVR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.GetAxis1D(button, controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.GetAxis1D(button, controller);
				}
				break;
			}
			return TBSteamVRInput.instance.GetAxis1D(button, controller);
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.GetAxis1D(button, controller);
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.GetAxis1D(button, controller);
		case VRPlatform.WindowsMR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.GetAxis1D(button, controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.GetAxis1D(button, controller);
				}
				break;
			}
			return TBWindowsMRInput.instance.GetAxis1D(button, controller);
		default:
			return 0f;
		}
	}

	public static Vector3 GetPosition(Controller controller)
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.GetRawPosition(controller);
		case VRPlatform.SteamVR:
			return TBSteamVRInput.instance.GetRawPosition(controller);
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.GetRawPosition(controller);
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.GetRawPosition(controller);
		case VRPlatform.WindowsMR:
			return TBWindowsMRInput.instance.GetRawPosition(controller);
		default:
			return Vector3.zero;
		}
	}

	public static Quaternion GetRotation(Controller controller)
	{
		if (controller == Controller.Active)
		{
			controller = GetActiveController();
		}
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.GetRawRotation(controller);
		case VRPlatform.SteamVR:
			return TBSteamVRInput.instance.GetRawRotation(controller);
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.GetRawRotation(controller);
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.GetRawRotation(controller);
		case VRPlatform.WindowsMR:
			return TBWindowsMRInput.instance.GetRawRotation(controller);
		default:
			return Quaternion.identity;
		}
	}

	public static Vector3 GetVelocity(Controller controller)
	{
		if (controller == Controller.Active)
		{
			controller = GetActiveController();
		}
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.GetVelocity(controller);
		case VRPlatform.SteamVR:
			return TBSteamVRInput.instance.GetVelocity(controller);
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.GetVelocity(controller);
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.GetVelocity(controller);
		case VRPlatform.WindowsMR:
			return TBWindowsMRInput.instance.GetVelocity(controller);
		default:
			return Vector3.zero;
		}
	}

	public static Vector3 GetAngularVelocity(Controller controller)
	{
		if (controller == Controller.Active)
		{
			controller = GetActiveController();
		}
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.GetAngularVelocity(controller);
		case VRPlatform.SteamVR:
			return TBSteamVRInput.instance.GetAngularVelocity(controller);
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.GetAngularVelocity(controller);
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.GetAngularVelocity(controller);
		case VRPlatform.WindowsMR:
			return TBWindowsMRInput.instance.GetAngularVelocity(controller);
		default:
			return Vector3.zero;
		}
	}

	public static Vector3 GetAcceleration(Controller controller)
	{
		if (controller == Controller.Active)
		{
			controller = GetActiveController();
		}
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.GetAcceleration(controller);
		case VRPlatform.SteamVR:
			return TBSteamVRInput.instance.GetAcceleration(controller);
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.GetAcceleration(controller);
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.GetAcceleration(controller);
		case VRPlatform.WindowsMR:
			return TBWindowsMRInput.instance.GetAcceleration(controller);
		default:
			return Vector3.zero;
		}
	}

	public static bool SupportsButton(Button button, Controller controller)
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.SupportsButton(button, controller);
		case VRPlatform.SteamVR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.SupportsButton(button, controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.SupportsButton(button, controller);
				}
				break;
			}
			return TBSteamVRInput.instance.SupportsButton(button, controller);
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.SupportsButton(button, controller);
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.SupportsButton(button, controller);
		case VRPlatform.WindowsMR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.SupportsButton(button, controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.SupportsButton(button, controller);
				}
				break;
			}
			return TBWindowsMRInput.instance.SupportsButton(button, controller);
		default:
			return false;
		}
	}

	public static bool SupportsTouch(Button button, Controller controller)
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.SupportsTouch(button, controller);
		case VRPlatform.SteamVR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.SupportsTouch(button, controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.SupportsTouch(button, controller);
				}
				break;
			}
			return TBSteamVRInput.instance.SupportsTouch(button, controller);
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.SupportsTouch(button, controller);
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.SupportsTouch(button, controller);
		case VRPlatform.WindowsMR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.SupportsTouch(button, controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.SupportsTouch(button, controller);
				}
				break;
			}
			return TBWindowsMRInput.instance.SupportsTouch(button, controller);
		default:
			return false;
		}
	}

	public static bool SupportsAxis1D(Button button, Controller controller)
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.SupportsAxis1D(button, controller);
		case VRPlatform.SteamVR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.SupportsAxis1D(button, controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.SupportsAxis1D(button, controller);
				}
				break;
			}
			return TBSteamVRInput.instance.SupportsAxis1D(button, controller);
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.SupportsAxis1D(button, controller);
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.SupportsAxis1D(button, controller);
		case VRPlatform.WindowsMR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.SupportsAxis1D(button, controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.SupportsAxis1D(button, controller);
				}
				break;
			}
			return TBWindowsMRInput.instance.SupportsAxis1D(button, controller);
		default:
			return false;
		}
	}

	public static bool SupportsAxis2D(Button button, Controller controller)
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.SupportsAxis2D(button, controller);
		case VRPlatform.SteamVR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.SupportsAxis2D(button, controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.SupportsAxis2D(button, controller);
				}
				break;
			}
			return TBSteamVRInput.instance.SupportsAxis2D(button, controller);
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.SupportsAxis2D(button, controller);
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.SupportsAxis2D(button, controller);
		case VRPlatform.WindowsMR:
			switch (controller)
			{
			case Controller.Gamepad:
				return TBXInput.instance.SupportsAxis2D(button, controller);
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					return TBXInput.instance.SupportsAxis2D(button, controller);
				}
				break;
			}
			return TBWindowsMRInput.instance.SupportsAxis2D(button, controller);
		default:
			return false;
		}
	}

	public static Mobile3DOFHandedness Get3DOFHandedness()
	{
		if (TBCore.UsingEditorMode())
		{
			if (TBSettings.GetControlSettings().handedness3DOF == TBSettings.TBHardwareHandedness.Left)
			{
				return Mobile3DOFHandedness.Left;
			}
			return Mobile3DOFHandedness.Right;
		}
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.Get3DOFHandedness();
		case VRPlatform.SteamVR:
			return TBSteamVRInput.instance.Get3DOFHandedness();
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.Get3DOFHandedness();
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.Get3DOFHandedness();
		default:
			return Mobile3DOFHandedness.Center;
		}
	}

	public static Mobile3DOFHandedness Get3DOFHandedness(out bool success)
	{
		if (TBCore.UsingEditorMode())
		{
			success = true;
			if (TBSettings.GetControlSettings().handedness3DOF == TBSettings.TBHardwareHandedness.Left)
			{
				return Mobile3DOFHandedness.Left;
			}
			return Mobile3DOFHandedness.Right;
		}
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.Get3DOFHandednessErrorTest(out success);
		case VRPlatform.SteamVR:
			success = true;
			return TBSteamVRInput.instance.Get3DOFHandedness();
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			success = true;
			return TBGoogleInput.instance.Get3DOFHandedness();
		case VRPlatform.PlayStationVR:
			success = true;
			return TBPSVRInput.instance.Get3DOFHandedness();
		default:
			success = true;
			return Mobile3DOFHandedness.Center;
		}
	}

	public static HandTrackingOffsets GetControllerTrackingOffsets(Controller controller)
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			return TBOculusInput.instance.GetControllerTrackingOffsets(controller);
		case VRPlatform.SteamVR:
			return TBSteamVRInput.instance.GetControllerTrackingOffsets(controller);
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return TBGoogleInput.instance.GetControllerTrackingOffsets(controller);
		case VRPlatform.PlayStationVR:
			return TBPSVRInput.instance.GetControllerTrackingOffsets(controller);
		case VRPlatform.WindowsMR:
			return TBWindowsMRInput.instance.GetControllerTrackingOffsets(controller);
		default:
			return default;
		}
	}

	public static void StartRumbleClip(Controller controller, AudioClip clip)
	{
		if (controller == Controller.Active)
		{
			controller = GetActiveController();
		}
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			TBOculusInput.instance.StartRumbleClip(controller, clip);
			break;
		case VRPlatform.SteamVR:
			switch (controller)
			{
			case Controller.Gamepad:
				TBXInput.instance.StartRumblePulse(controller, 0.5f, clip.length);
				break;
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					TBXInput.instance.StartRumblePulse(controller, 0.5f, clip.length);
				}
				else
				{
					TBSteamVRInput.instance.StartRumblePulse(controller, 0.5f, clip.length);
				}
				break;
			default:
				TBSteamVRInput.instance.StartRumblePulse(controller, 0.5f, clip.length);
				break;
			}
			break;
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			TBGoogleInput.instance.StartRumblePulse(controller, 0.5f, clip.length);
			break;
		case VRPlatform.PlayStationVR:
			TBPSVRInput.instance.StartRumblePulse(controller, 0.5f, clip.length);
			break;
		case VRPlatform.WindowsMR:
			switch (controller)
			{
			case Controller.Gamepad:
				TBXInput.instance.StartRumblePulse(controller, 0.5f, clip.length);
				break;
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					TBXInput.instance.StartRumblePulse(controller, 0.5f, clip.length);
				}
				else
				{
					TBWindowsMRInput.instance.StartRumblePulse(controller, 0.5f, clip.length);
				}
				break;
			default:
				TBWindowsMRInput.instance.StartRumblePulse(controller, 0.5f, clip.length);
				break;
			}
			break;
		}
	}

	public static void SetRumble(Controller controller, float amount = 0f)
	{
		if (controller == Controller.Active)
		{
			controller = GetActiveController();
		}
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			TBOculusInput.instance.SetRumble(controller, amount);
			break;
		case VRPlatform.SteamVR:
			switch (controller)
			{
			case Controller.Gamepad:
				TBXInput.instance.SetRumble(controller, amount);
				break;
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					TBXInput.instance.SetRumble(controller, amount);
				}
				else
				{
					TBSteamVRInput.instance.SetRumble(controller, amount);
				}
				break;
			default:
				TBSteamVRInput.instance.SetRumble(controller, amount);
				break;
			}
			break;
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			TBGoogleInput.instance.SetRumble(controller, amount);
			break;
		case VRPlatform.PlayStationVR:
			TBPSVRInput.instance.SetRumble(controller, amount);
			break;
		case VRPlatform.WindowsMR:
			switch (controller)
			{
			case Controller.Gamepad:
				TBXInput.instance.SetRumble(controller, amount);
				break;
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					TBXInput.instance.SetRumble(controller, amount);
				}
				else
				{
					TBWindowsMRInput.instance.SetRumble(controller, amount);
				}
				break;
			default:
				TBWindowsMRInput.instance.SetRumble(controller, amount);
				break;
			}
			break;
		}
	}

	public static void StartRumblePulse(Controller controller, RumblePulseStrength strength = RumblePulseStrength.Low, RumblePusleLength length = RumblePusleLength.Short)
	{
		float length2 = 0f;
		float strength2 = 0f;
		switch (strength)
		{
		case RumblePulseStrength.Low:
			strength2 = rumbleStrengthLow;
			break;
		case RumblePulseStrength.Medium:
			strength2 = rumbleStrengthMed;
			break;
		case RumblePulseStrength.High:
			strength2 = rumbleStrengthHi;
			break;
		}
		switch (length)
		{
		case RumblePusleLength.Short:
			length2 = rumbleLengthShort;
			break;
		case RumblePusleLength.Medium:
			length2 = rumbleLengthMed;
			break;
		case RumblePusleLength.Long:
			length2 = rumbleLengthLong;
			break;
		}
		StartRumblePulse(controller, strength2, length2);
	}

	public static void StartRumblePulse(Controller controller, float strength = 0.5f, float length = 0.2f)
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusPC:
		case VRPlatform.OculusMobile:
			TBOculusInput.instance.StartRumblePulse(controller, strength, length);
			break;
		case VRPlatform.SteamVR:
			switch (controller)
			{
			case Controller.Gamepad:
				TBXInput.instance.StartRumblePulse(controller, strength, length);
				break;
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					TBXInput.instance.StartRumblePulse(controller, strength, length);
				}
				else
				{
					TBSteamVRInput.instance.StartRumblePulse(controller, strength, length);
				}
				break;
			default:
				TBSteamVRInput.instance.StartRumblePulse(controller, strength, length);
				break;
			}
			break;
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			TBGoogleInput.instance.StartRumblePulse(controller, strength, length);
			break;
		case VRPlatform.PlayStationVR:
			TBPSVRInput.instance.StartRumblePulse(controller, strength, length);
			break;
		case VRPlatform.WindowsMR:
			switch (controller)
			{
			case Controller.Gamepad:
				TBXInput.instance.StartRumblePulse(controller, strength, length);
				break;
			case Controller.Active:
				if (_activeControlType == ControlType.Gamepad)
				{
					TBXInput.instance.StartRumblePulse(controller, strength, length);
				}
				else
				{
					TBWindowsMRInput.instance.StartRumblePulse(controller, strength, length);
				}
				break;
			default:
				TBWindowsMRInput.instance.StartRumblePulse(controller, strength, length);
				break;
			}
			break;
		}
	}

	public static T[] GetRawButtons<T>(Button virtualButton, ButtonLookupTable<T> lookupTable)
	{
		switch (virtualButton)
		{
		case Button.PrimaryTrigger:
			return lookupTable.PrimaryTrigger;
		case Button.SecondaryTrigger:
			return lookupTable.SecondaryTrigger;
		case Button.Start:
			return lookupTable.Start;
		case Button.Options:
			return lookupTable.Options;
		case Button.Action1:
			return lookupTable.Action1;
		case Button.Action2:
			return lookupTable.Action2;
		case Button.Action3:
			return lookupTable.Action3;
		case Button.Action4:
			return lookupTable.Action4;
		case Button.DpadUp:
			return lookupTable.DpadUp;
		case Button.DpadDown:
			return lookupTable.DpadDown;
		case Button.DpadLeft:
			return lookupTable.DpadLeft;
		case Button.DpadRight:
			return lookupTable.DpadRight;
		case Button.Touchpad:
			return lookupTable.Touchpad;
		case Button.Joystick:
			return lookupTable.Joystick;
		case Button.LeftTrigger:
			return lookupTable.LeftTrigger;
		case Button.RightTrigger:
			return lookupTable.RightTrigger;
		case Button.LeftBumper:
			return lookupTable.LeftBumper;
		case Button.RightBumper:
			return lookupTable.RightBumper;
		case Button.LeftStick:
			return lookupTable.LeftStick;
		case Button.RightStick:
			return lookupTable.RightStick;
		case Button.Any:
			return lookupTable.Any;
		default:
			Debug.LogError(string.Concat("Button ", virtualButton, " isn't defined in the lookup table! This is a core TButt error, please report it."));
			return null;
		}
	}

	public static List<ButtonDef<T>> LoadButtonDefs<T>(List<ButtonDef<T>> buttons, string fileName)
	{
		if (Application.isPlaying)
		{
		}
		SerializedButtonDef[] array = TBDataManager.FromJsonWrapper(TBDataManager.DeserializeFromFile<TBDataManager.Wrapper<SerializedButtonDef>>(TBSettings.settingsFolder + fileName, TBDataManager.PathType.ResourcesFolder));
		if (array == null)
		{
			if (Application.isPlaying)
			{
			}
			return buttons;
		}
		if (array.Length != buttons.Count)
		{
		}
		for (int i = 0; i < buttons.Count; i++)
		{
			buttons[i].virtualButtons = array[i].virtualButtons;
		}
		if (Application.isPlaying)
		{
		}
		return buttons;
	}

	public static ButtonLookupTable<T> NewLookupTableFromDefs<T>(List<ButtonDef<T>> defs)
	{
		ButtonLookupTable<T> table = default;
		for (int i = 0; i < defs.Count; i++)
		{
			SetupVirtualMap(ref table, defs[i].rawButton, Button.Any);
			for (int j = 0; j < defs[i].virtualButtons.Length; j++)
			{
				if (defs[i].virtualButtons[j] != Button.Any)
				{
					SetupVirtualMap(ref table, defs[i].rawButton, defs[i].virtualButtons[j]);
				}
			}
		}
		return table;
	}

	public static void SetupVirtualMap<T>(ref ButtonLookupTable<T> table, T rawButton, Button virtualButton)
	{
		switch (virtualButton)
		{
		case Button.PrimaryTrigger:
			BindVirtualMapToButton(ref table.PrimaryTrigger, rawButton);
			break;
		case Button.SecondaryTrigger:
			BindVirtualMapToButton(ref table.SecondaryTrigger, rawButton);
			break;
		case Button.Start:
			BindVirtualMapToButton(ref table.Start, rawButton);
			break;
		case Button.Options:
			BindVirtualMapToButton(ref table.Options, rawButton);
			break;
		case Button.Action1:
			BindVirtualMapToButton(ref table.Action1, rawButton);
			break;
		case Button.Action2:
			BindVirtualMapToButton(ref table.Action2, rawButton);
			break;
		case Button.Action3:
			BindVirtualMapToButton(ref table.Action3, rawButton);
			break;
		case Button.Action4:
			BindVirtualMapToButton(ref table.Action4, rawButton);
			break;
		case Button.DpadUp:
			BindVirtualMapToButton(ref table.DpadUp, rawButton);
			break;
		case Button.DpadDown:
			BindVirtualMapToButton(ref table.DpadDown, rawButton);
			break;
		case Button.DpadLeft:
			BindVirtualMapToButton(ref table.DpadLeft, rawButton);
			break;
		case Button.DpadRight:
			BindVirtualMapToButton(ref table.DpadRight, rawButton);
			break;
		case Button.Touchpad:
			BindVirtualMapToButton(ref table.Touchpad, rawButton);
			break;
		case Button.Joystick:
			BindVirtualMapToButton(ref table.Joystick, rawButton);
			break;
		case Button.LeftTrigger:
			BindVirtualMapToButton(ref table.LeftTrigger, rawButton);
			break;
		case Button.RightTrigger:
			BindVirtualMapToButton(ref table.RightTrigger, rawButton);
			break;
		case Button.LeftBumper:
			BindVirtualMapToButton(ref table.LeftBumper, rawButton);
			break;
		case Button.RightBumper:
			BindVirtualMapToButton(ref table.RightBumper, rawButton);
			break;
		case Button.LeftStick:
			BindVirtualMapToButton(ref table.LeftStick, rawButton);
			break;
		case Button.RightStick:
			BindVirtualMapToButton(ref table.RightStick, rawButton);
			break;
		case Button.Any:
			BindVirtualMapToButton(ref table.Any, rawButton);
			break;
		}
	}

	public static void BindVirtualMapToButton<T>(ref T[] virtualButtonGroup, T rawButton)
	{
		T[] array;
		if (virtualButtonGroup != null)
		{
			array = new T[virtualButtonGroup.Length + 1];
			for (int i = 0; i < virtualButtonGroup.Length; i++)
			{
				array[i] = virtualButtonGroup[i];
			}
			array[virtualButtonGroup.Length] = rawButton;
		}
		else
		{
			array = new T[1] { rawButton };
		}
		virtualButtonGroup = array;
	}
}
