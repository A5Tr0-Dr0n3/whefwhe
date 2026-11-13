namespace TButt.InputMaps;

public class TBXInput : TBSDKInputBase<TBXInput.TBXInputButton>
{
	public enum TBXInputButton
	{
		ButtonA,
		ButtonB,
		ButtonX,
		ButtonY,
		LeftStick,
		RightStick,
		DpadUp,
		DpadRight,
		DpadDown,
		DpadLeft,
		Back,
		Guide,
		RightBumper,
		LeftBumper,
		LeftTrigger,
		RightTrigger,
		Start
	}

	protected static TBXInput _instance;

	public static TBXInput instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new TBXInput();
			}
			return _instance;
		}
	}
}
