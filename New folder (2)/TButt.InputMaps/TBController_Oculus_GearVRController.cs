using System.Collections.Generic;

namespace TButt.InputMaps;

public class TBController_Oculus_GearVRController : TBControllerBase<OVRInput.RawButton>
{
	protected static TBController_Oculus_GearVRController _instance;

	public static TBController_Oculus_GearVRController instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new TBController_Oculus_GearVRController();
				_instance.name = "Gear VR Controller";
				_instance.fileName = "Maps_Oculus_GearVRController";
				_instance.type = TBInput.Controller.Mobile3DOFController;
				_instance.supportsRumble = false;
				_instance.Initialize();
			}
			return _instance;
		}
	}

	protected override void Initialize()
	{
		loadedButtonDefs = TBInput.LoadButtonDefs(GetDefaultDefs(), fileName);
		loadedButtonDefs.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.LTouchpad,
			virtualButtons = loadedButtonDefs[0].virtualButtons,
			name = "Touchpad",
			supportsTouch = true,
			supportsAxis2D = true
		});
		loadedButtonDefs.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.LIndexTrigger,
			virtualButtons = loadedButtonDefs[1].virtualButtons,
			name = "Trigger",
			supportsTouch = true,
			supportsAxis2D = true
		});
		lookupTable = TBInput.NewLookupTableFromDefs(loadedButtonDefs);
		loaded = true;
	}

	public override List<TBInput.ButtonDef<OVRInput.RawButton>> GetDefaultDefs()
	{
		List<TBInput.ButtonDef<OVRInput.RawButton>> list = new List<TBInput.ButtonDef<OVRInput.RawButton>>();
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.RTouchpad,
			virtualButtons = new TBInput.Button[2]
			{
				TBInput.Button.Touchpad,
				TBInput.Button.Action1
			},
			name = "Touchpad",
			supportsTouch = true,
			supportsAxis2D = true
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.RIndexTrigger,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.PrimaryTrigger },
			name = "Trigger"
		});
		list.Add(new TBInput.ButtonDef<OVRInput.RawButton>
		{
			rawButton = OVRInput.RawButton.Back,
			virtualButtons = new TBInput.Button[1] { TBInput.Button.Options },
			name = "Back"
		});
		return list;
	}
}
