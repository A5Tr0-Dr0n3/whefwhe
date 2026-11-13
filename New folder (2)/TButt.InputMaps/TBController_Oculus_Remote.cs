using System.Collections.Generic;

namespace TButt.InputMaps;

public class TBController_Oculus_Remote : TBControllerBase<OVRInput.RawButton>
{
	protected static TBController_Oculus_Remote _instance;

	public static TBController_Oculus_Remote instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new TBController_Oculus_Remote();
				_instance.name = "Oculus Remote";
				_instance.fileName = "Maps_Oculus_Remote";
				_instance.type = TBInput.Controller.ClickRemote;
				_instance.supportsRumble = false;
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
			rawButton = OVRInput.RawButton.Start,
			virtualButtons = new TBInput.Button[2]
			{
				TBInput.Button.Touchpad,
				TBInput.Button.Action1
			},
			name = "Select Button"
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.Back,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.Options },
			name = "Back Button"
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.DpadUp,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.DpadUp },
			name = "Up"
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.DpadDown,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.DpadDown },
			name = "Down",
			supportsTouch = true,
			supportsAxis1D = true
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.DpadLeft,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.DpadLeft },
			name = "Left"
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.DpadRight,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.DpadRight },
			name = "Right"
		});
		return list;
	}
}
