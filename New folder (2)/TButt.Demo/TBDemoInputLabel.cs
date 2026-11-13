using UnityEngine;
using UnityEngine.UI;

namespace TButt.Demo;

public class TBDemoInputLabel : MonoBehaviour
{
	public enum DemoInputLabel
	{
		SupportedInputTypes,
		ActiveInputType,
		ControllerName,
		SupportsRumble,
		ButtonList,
		TouchList,
		Axis1DList,
		Axis2DList
	}

	public DemoInputLabel label;

	public TBInput.Controller controller;

	private Text text;

	private TBInput.Button[] buttonArray;

	private void Start()
	{
		buttonArray = GetButtonArray();
		text = GetComponent<Text>();
	}

	private void Update()
	{
		text.text = GetLabel(label);
	}

	private string GetLabel(DemoInputLabel label)
	{
		switch (label)
		{
		case DemoInputLabel.SupportedInputTypes:
			return GetSupportedInputTypes();
		case DemoInputLabel.ActiveInputType:
			return TBInput.GetActiveController().ToString();
		case DemoInputLabel.ControllerName:
			return TBInput.GetControllerName(controller);
		case DemoInputLabel.SupportsRumble:
			return TBInput.ControllerHasRumbleSupport(TBInput.Controller.Active).ToString();
		case DemoInputLabel.ButtonList:
		{
			string text2 = string.Empty;
			for (int j = 0; j < buttonArray.Length; j++)
			{
				text2 += GetBoolState(TBInput.GetButton(buttonArray[j], controller));
			}
			return text2;
		}
		case DemoInputLabel.TouchList:
		{
			string text4 = string.Empty;
			for (int l = 0; l < buttonArray.Length; l++)
			{
				text4 += GetBoolState(TBInput.GetTouch(buttonArray[l], controller));
			}
			return text4;
		}
		case DemoInputLabel.Axis1DList:
		{
			string text3 = string.Empty;
			for (int k = 0; k < buttonArray.Length; k++)
			{
				text3 = text3 + TBInput.GetAxis1D(buttonArray[k], controller).ToString("#.##") + "\n";
			}
			return text3;
		}
		case DemoInputLabel.Axis2DList:
		{
			string text = string.Empty;
			for (int i = 0; i < buttonArray.Length; i++)
			{
				text = text + TBInput.GetAxis2D(buttonArray[i], controller).ToString("#.##") + "\n";
			}
			return text;
		}
		default:
			return string.Empty;
		}
	}

	private TBInput.Button[] GetButtonArray()
	{
		return new TBInput.Button[18]
		{
			TBInput.Button.Action1,
			TBInput.Button.Action2,
			TBInput.Button.Action3,
			TBInput.Button.Action4,
			TBInput.Button.PrimaryTrigger,
			TBInput.Button.SecondaryTrigger,
			TBInput.Button.Start,
			TBInput.Button.Options,
			TBInput.Button.DpadUp,
			TBInput.Button.DpadDown,
			TBInput.Button.DpadLeft,
			TBInput.Button.DpadRight,
			TBInput.Button.Touchpad,
			TBInput.Button.Joystick,
			TBInput.Button.LeftTrigger,
			TBInput.Button.RightTrigger,
			TBInput.Button.LeftBumper,
			TBInput.Button.RightBumper
		};
	}

	private string GetBoolState(bool on)
	{
		if (on)
		{
			return "ON\n";
		}
		return "\n";
	}

	private string GetSupportedInputTypes()
	{
		string text = string.Empty;
		if (TBSettings.GetControlSettings().supportsHandControllers)
		{
			text += "   Hand Controllers";
		}
		if (TBSettings.GetControlSettings().supports3DOFControllers)
		{
			text += "   3DOF Controllers";
		}
		if (TBSettings.GetControlSettings().supportsClickRemote)
		{
			text += "   Click Remotes";
		}
		if (TBSettings.GetControlSettings().supportsGamepad)
		{
			text += "   Gamepads";
		}
		return text;
	}
}
