using System.Collections.Generic;
using UnityEngine;

namespace TButt.InputMaps;

public class TBController_Oculus_TouchLeft : TBControllerBase<OVRInput.RawButton>
{
	protected static TBController_Oculus_TouchLeft _instance;

	public static TBController_Oculus_TouchLeft instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new TBController_Oculus_TouchLeft();
				_instance.name = "Oculus Touch (Left)";
				_instance.fileName = "Maps_Oculus_TouchLeft";
				_instance.type = TBInput.Controller.LHandController;
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
			rawButton = OVRInput.RawButton.X,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.Action1 },
			name = "X Button",
			supportsTouch = true
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.Y,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.Action2 },
			name = "Y Button",
			supportsTouch = true
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.LThumbstick,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.Joystick },
			name = "Joystick",
			supportsTouch = true,
			supportsAxis2D = true
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.LIndexTrigger,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.PrimaryTrigger },
			name = "Index Trigger",
			supportsTouch = true,
			supportsAxis1D = true
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.LHandTrigger,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.SecondaryTrigger },
			name = "Hand Trigger",
			supportsTouch = false,
			supportsAxis1D = true
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.Start,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.Options },
			name = "Menu"
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
