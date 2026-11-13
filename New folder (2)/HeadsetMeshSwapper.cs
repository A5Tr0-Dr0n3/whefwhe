using TButt;
using UnityEngine;

public class HeadsetMeshSwapper : MonoBehaviour
{
	public GameObject daydreamObject;

	public GameObject goObject;

	public GameObject gearObject;

	private void Start()
	{
		if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile)
		{
			daydreamObject.SetActive(value: false);
			if (OVRPlugin.productName == "Oculus Go")
			{
				goObject.SetActive(value: true);
				gearObject.SetActive(value: false);
			}
			else
			{
				goObject.SetActive(value: false);
				gearObject.SetActive(value: true);
			}
		}
		else
		{
			goObject.SetActive(value: false);
			gearObject.SetActive(value: false);
		}
	}
}
