using System.Collections.Generic;
using UnityEngine;

namespace TButt.InputMaps;

public class TBController_Oculus_TouchRight : TBControllerBase<OVRInput.RawButton>
{
	protected static TBController_Oculus_TouchRight _instance;

	public static TBController_Oculus_TouchRight instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new TBController_Oculus_TouchRight();
				_instance.name = "Oculus Touch (Right)";
				_instance.fileName = "Maps_Oculus_TouchRight";
				_instance.type = TBInput.Controller.RHandController;
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
			name = "A Button",
			supportsTouch = true
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.B,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.Action2 },
			name = "B Button",
			supportsTouch = true
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.RThumbstick,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.Joystick },
			name = "Joystick",
			supportsTouch = true,
			supportsAxis2D = true
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.RIndexTrigger,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.PrimaryTrigger },
			name = "Index Trigger",
			supportsTouch = true,
			supportsAxis1D = true
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.RHandTrigger,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.SecondaryTrigger },
			name = "Hand Trigger",
			supportsTouch = false,
			supportsAxis1D = true
		});
		return list;
	}

	protected override HandTrackingOffsets GetDefaultTrackingOffsets()
	{
		return new HandTrackingOffsets
		{
			positionOffset = new Vector3(0f, -0.02f, -0.07f),
			rotationOffset = Vector3.zero
		};
	}
}
