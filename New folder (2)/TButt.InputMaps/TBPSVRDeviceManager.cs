using UnityEngine;

namespace TButt.InputMaps;

public class TBPSVRDeviceManager : MonoBehaviour
{
	protected static TBPSVRDeviceManager _instance;

	public static TBPSVRDeviceManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = TBCore.instance.gameObject.AddComponent<TBPSVRDeviceManager>();
			}
			return _instance;
		}
	}
}
