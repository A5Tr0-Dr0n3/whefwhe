using UnityEngine;

namespace TButt.InputMaps;

public class TBSteamVRInput : TBSDKInputBase<TBInput.Button>
{
	protected static TBSteamVRInput _instance;

	protected Vector3 lastLeftControllerVelocity = Vector3.one;

	protected Vector3 lastRightControllerVelocity = Vector3.one;

	public static TBSteamVRInput instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new TBSteamVRInput();
			}
			return _instance;
		}
	}
}
