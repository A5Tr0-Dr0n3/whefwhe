namespace TButt.InputMaps;

public class TBPSVRInput : TBSDKInputBase<TBInput.Button>
{
	protected static TBPSVRInput _instance;

	public static TBPSVRInput instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new TBPSVRInput();
			}
			return _instance;
		}
	}
}
