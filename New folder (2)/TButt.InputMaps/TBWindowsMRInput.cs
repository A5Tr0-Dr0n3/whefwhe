using UnityEngine;

namespace TButt.InputMaps;

public class TBWindowsMRInput : TBSDKInputBase<TBInput.Button>
{
	public enum Button
	{
		SelectTrigger,
		Grip,
		Menu,
		Thumbstick,
		Touchpad
	}

	protected static TBWindowsMRInput _instance;

	protected Vector3 lastLeftControllerVelocity = Vector3.one;

	protected Vector3 lastRightControllerVelocity = Vector3.one;

	public static TBWindowsMRInput instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new TBWindowsMRInput();
			}
			return _instance;
		}
	}
}
