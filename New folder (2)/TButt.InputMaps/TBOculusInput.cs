using UnityEngine;

namespace TButt.InputMaps;

public class TBOculusInput : TBSDKInputBase<OVRInput.RawButton>
{
	protected static TBOculusInput _instance;

	private OVRHapticsClip _leftRumble;

	private OVRHapticsClip _rightRumble;

	private int _numHapticSamples;

	public static TBOculusInput instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new TBOculusInput();
			}
			return _instance;
		}
	}

	protected override void LoadHandControllers()
	{
		if (TBCore.UsingEditorMode() || TBCore.GetActivePlatform() == VRPlatform.OculusPC)
		{
			_numHapticSamples = (int)(320f / TBSettings.GetRefreshRate()) + 1;
			_leftRumble = new OVRHapticsClip(_numHapticSamples);
			_rightRumble = new OVRHapticsClip(_numHapticSamples);
			controller_LHand = TBController_Oculus_TouchLeft.instance;
			controller_RHand = TBController_Oculus_TouchRight.instance;
		}
		else
		{
			base.LoadHandControllers();
		}
	}

	protected override void Load3DOFControllers()
	{
		if (!TBCore.UsingEditorMode() && TBCore.GetActivePlatform() == VRPlatform.OculusMobile)
		{
			controller_3DOF = TBController_Oculus_GearVRController.instance;
			TBInput.SetActiveControlType(TBInput.ControlType.Mobile3DOFController);
		}
		else if (TBCore.UsingEditorMode() || TBCore.GetActivePlatform() == VRPlatform.OculusPC)
		{
			if (TBSettings.GetControlSettings().handedness3DOF == TBSettings.TBHardwareHandedness.Left)
			{
				controller_3DOF = TBController_Oculus_TouchLeft.instance;
			}
			else
			{
				controller_3DOF = TBController_Oculus_TouchRight.instance;
			}
		}
		else
		{
			base.Load3DOFControllers();
		}
	}

	protected override void LoadClickRemotes()
	{
		if (TBCore.UsingEditorMode() || TBCore.GetActivePlatform() == VRPlatform.OculusPC)
		{
			controller_ClickRemote = TBController_Oculus_Remote.instance;
		}
		else if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile)
		{
			controller_ClickRemote = TBController_Oculus_GearVRTouchpad.instance;
		}
	}

	protected override void LoadGamepads()
	{
		controller_Gamepad = TBController_Oculus_Gamepad.instance;
	}

	public override void Update()
	{
	}

	public override void FixedUpdate()
	{
	}

	public static OVRInput.Controller GetOculusControllerID(TBInput.Controller controller)
	{
		switch (controller)
		{
		case TBInput.Controller.LHandController:
			return OVRInput.Controller.LTouch;
		case TBInput.Controller.RHandController:
			return OVRInput.Controller.RTouch;
		case TBInput.Controller.Mobile3DOFController:
			if (TBCore.UsingEditorMode() || TBCore.GetActivePlatform() == VRPlatform.OculusPC)
			{
				if (TBSettings.GetControlSettings().handedness3DOF == TBSettings.TBHardwareHandedness.Left)
				{
					return OVRInput.Controller.LTouch;
				}
				return OVRInput.Controller.RTouch;
			}
			if (OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote))
			{
				return OVRInput.Controller.LTrackedRemote;
			}
			return OVRInput.Controller.RTrackedRemote;
		case TBInput.Controller.ClickRemote:
			return OVRInput.Controller.Remote;
		case TBInput.Controller.Gamepad:
			return OVRInput.Controller.Gamepad;
		default:
			return OVRInput.Controller.None;
		}
	}

	public override TBInput.Mobile3DOFHandedness Get3DOFHandedness()
	{
		if (OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote) || OVRInput.IsControllerConnected(OVRInput.Controller.LTouch))
		{
			return TBInput.Mobile3DOFHandedness.Left;
		}
		if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote) || OVRInput.IsControllerConnected(OVRInput.Controller.RTouch))
		{
			return TBInput.Mobile3DOFHandedness.Right;
		}
		return TBInput.Mobile3DOFHandedness.Right;
	}

	public TBInput.Mobile3DOFHandedness Get3DOFHandednessErrorTest(out bool success)
	{
		if (OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote) || OVRInput.IsControllerConnected(OVRInput.Controller.LTouch))
		{
			success = true;
			return TBInput.Mobile3DOFHandedness.Left;
		}
		if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote) || OVRInput.IsControllerConnected(OVRInput.Controller.RTouch))
		{
			success = true;
			return TBInput.Mobile3DOFHandedness.Right;
		}
		success = false;
		return TBInput.Mobile3DOFHandedness.Right;
	}

	public override bool StartRumbleClip(TBInput.Controller controller, AudioClip clip)
	{
		switch (controller)
		{
		case TBInput.Controller.LHandController:
			_leftRumble = new OVRHapticsClip(clip);
			OVRHaptics.LeftChannel.Mix(_leftRumble);
			return true;
		case TBInput.Controller.RHandController:
			_rightRumble = new OVRHapticsClip(clip);
			OVRHaptics.RightChannel.Mix(_rightRumble);
			return true;
		default:
			return false;
		}
	}

	public override bool SetRumble(TBInput.Controller controller, float strength)
	{
		switch (controller)
		{
		case TBInput.Controller.LHandController:
		{
			_leftRumble = new OVRHapticsClip(_numHapticSamples);
			for (int j = 0; j < _numHapticSamples; j++)
			{
				_leftRumble.WriteSample((byte)(255f * strength));
			}
			OVRHaptics.LeftChannel.Preempt(_leftRumble);
			return true;
		}
		case TBInput.Controller.RHandController:
		{
			_rightRumble = new OVRHapticsClip(_numHapticSamples);
			for (int i = 0; i < _numHapticSamples; i++)
			{
				_rightRumble.WriteSample((byte)(255f * strength));
			}
			OVRHaptics.RightChannel.Preempt(_rightRumble);
			return true;
		}
		default:
			return false;
		}
	}

	public override bool ResolveButtonDown(OVRInput.RawButton button, TBInput.Controller controller)
	{
		if (OVRInput.GetDown(button, GetOculusControllerID(controller)))
		{
			return true;
		}
		return false;
	}

	public override bool ResolveButtonUp(OVRInput.RawButton button, TBInput.Controller controller)
	{
		if (OVRInput.GetUp(button, GetOculusControllerID(controller)))
		{
			return true;
		}
		return false;
	}

	public override bool ResolveButton(OVRInput.RawButton button, TBInput.Controller controller)
	{
		if (OVRInput.Get(button, GetOculusControllerID(controller)))
		{
			return true;
		}
		return false;
	}

	public override bool ResolveTouchDown(OVRInput.RawButton button, TBInput.Controller controller)
	{
		if (OVRInput.GetDown(GetTouchEnum(button), GetOculusControllerID(controller)))
		{
			return true;
		}
		return false;
	}

	public override bool ResolveTouchUp(OVRInput.RawButton button, TBInput.Controller controller)
	{
		if (OVRInput.GetUp(GetTouchEnum(button), GetOculusControllerID(controller)))
		{
			return true;
		}
		return false;
	}

	public override bool ResolveTouch(OVRInput.RawButton button, TBInput.Controller controller)
	{
		if (OVRInput.Get(GetTouchEnum(button), GetOculusControllerID(controller)))
		{
			return true;
		}
		return false;
	}

	public override float ResolveAxis1D(OVRInput.RawButton button, TBInput.Controller controller)
	{
		return OVRInput.Get(GetAxis1DEnum(button), GetOculusControllerID(controller));
	}

	public override Vector2 ResolveAxis2D(OVRInput.RawButton button, TBInput.Controller controller)
	{
		return OVRInput.Get(GetAxis2DEnum(button), GetOculusControllerID(controller));
	}

	public override Quaternion GetRawRotation(TBInput.Controller controller)
	{
		return OVRInput.GetLocalControllerRotation(GetOculusControllerID(controller));
	}

	public override Vector3 GetRawPosition(TBInput.Controller controller)
	{
		return OVRInput.GetLocalControllerPosition(GetOculusControllerID(controller));
	}

	public override Vector3 GetVelocity(TBInput.Controller controller)
	{
		return OVRInput.GetLocalControllerVelocity(GetOculusControllerID(controller));
	}

	public override Vector3 GetAngularVelocity(TBInput.Controller controller)
	{
		return OVRInput.GetLocalControllerAngularVelocity(GetOculusControllerID(controller));
	}

	public override Vector3 GetAcceleration(TBInput.Controller controller)
	{
		return OVRInput.GetLocalControllerAcceleration(GetOculusControllerID(controller));
	}

	public static OVRInput.RawTouch GetTouchEnum(OVRInput.RawButton button)
	{
		return button switch
		{
			OVRInput.RawButton.A => OVRInput.RawTouch.A, 
			OVRInput.RawButton.B => OVRInput.RawTouch.B, 
			OVRInput.RawButton.X => OVRInput.RawTouch.X, 
			OVRInput.RawButton.Y => OVRInput.RawTouch.Y, 
			OVRInput.RawButton.RThumbstick => OVRInput.RawTouch.RThumbstick, 
			OVRInput.RawButton.LThumbstick => OVRInput.RawTouch.LThumbstick, 
			OVRInput.RawButton.RIndexTrigger => OVRInput.RawTouch.RIndexTrigger, 
			OVRInput.RawButton.LIndexTrigger => OVRInput.RawTouch.LIndexTrigger, 
			OVRInput.RawButton.LTouchpad => OVRInput.RawTouch.LTouchpad, 
			OVRInput.RawButton.RTouchpad => OVRInput.RawTouch.RTouchpad, 
			_ => OVRInput.RawTouch.None, 
		};
	}

	public static OVRInput.RawAxis1D GetAxis1DEnum(OVRInput.RawButton button)
	{
		return button switch
		{
			OVRInput.RawButton.LHandTrigger => OVRInput.RawAxis1D.LHandTrigger, 
			OVRInput.RawButton.LIndexTrigger => OVRInput.RawAxis1D.LIndexTrigger, 
			OVRInput.RawButton.RIndexTrigger => OVRInput.RawAxis1D.RIndexTrigger, 
			OVRInput.RawButton.RHandTrigger => OVRInput.RawAxis1D.RHandTrigger, 
			_ => OVRInput.RawAxis1D.None, 
		};
	}

	public static OVRInput.RawAxis2D GetAxis2DEnum(OVRInput.RawButton button)
	{
		return button switch
		{
			OVRInput.RawButton.LThumbstick => OVRInput.RawAxis2D.LThumbstick, 
			OVRInput.RawButton.RThumbstick => OVRInput.RawAxis2D.RThumbstick, 
			OVRInput.RawButton.RTouchpad => OVRInput.RawAxis2D.RTouchpad, 
			OVRInput.RawButton.LTouchpad => OVRInput.RawAxis2D.LTouchpad, 
			_ => OVRInput.RawAxis2D.None, 
		};
	}
}
