using System.Collections.Generic;

namespace TButt.InputMaps;

public class TBController_Oculus_Gamepad : TBControllerBase<OVRInput.RawButton>
{
	protected static TBController_Oculus_Gamepad _instance;

	public static TBController_Oculus_Gamepad instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new TBController_Oculus_Gamepad();
				_instance.name = "Xbox One Controller";
				_instance.fileName = "Maps_Oculus_Gamepad";
				_instance.type = TBInput.Controller.Gamepad;
				_instance.supportsRumble = true;
				_instance.Initialize();
			}
			return _instance;
		}
	}

	public override List<TBInput.ButtonDef<OVRInput.RawButton>> GetDefaultDefs()
	{
		List<TBInput.ButtonDef<OVRInput.RawButton>> list = new List<TBInput.ButtonDef<OVRInput.RawButton>>();
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.A,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.Action1 },
			name = "A Button"
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.B,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.Action2 },
			name = "B Button"
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.X,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.Action3 },
			name = "X Button"
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.Y,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.Action4 },
			name = "Y Button"
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.Start,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.Start },
			name = "Start Button"
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.Back,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.Options },
			name = "Back Button"
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.LThumbstick,
			virtualButtons = new TBInput.Button[2]
			{
				TBInput.Button.LeftStick,
				TBInput.Button.Joystick
			},
			name = "Left Stick",
			supportsAxis2D = true
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.RThumbstick,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.RightStick },
			name = "Right Stick",
			supportsAxis2D = true
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.LShoulder,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.LeftBumper },
			name = "Left Shoulder"
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.RShoulder,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.RightBumper },
			name = "Right Shoulder"
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.LIndexTrigger,
			virtualButtons = new TBInput.Button[2]
			{
				TBInput.Button.LeftTrigger,
				TBInput.Button.PrimaryTrigger
			},
			name = "Left Trigger",
			supportsAxis1D = true
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.RIndexTrigger,
			virtualButtons = new TBInput.Button[2]
			{
				TBInput.Button.RightTrigger,
				TBInput.Button.SecondaryTrigger
			},
			name = "Right Trigger",
			supportsAxis1D = true
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.DpadUp,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.DpadUp },
			name = "Dpad Up"
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.DpadDown,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.DpadDown },
			name = "Dpad Down"
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.DpadLeft,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.DpadLeft },
			name = "Dpad Left"
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.DpadRight,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.DpadRight },
			name = "Dpad Right"
		});
		return list;
	}
}
