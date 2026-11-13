namespace TButt.InputMaps;

public class TBGoogleInput : TBSDKInputBase<TBGoogleInput.Button>
{
	public enum Button
	{
		None,
		Touchpad,
		App,
		CardboardTrigger,
		Home
	}

	protected static TBGoogleInput _instance;

	public static TBGoogleInput instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new TBGoogleInput();
			}
			return _instance;
		}
	}
}
