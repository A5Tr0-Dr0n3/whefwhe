using TButt;
using UnityEngine;

public class OculusCreditsManager : MonoBehaviour
{
	public GameObject[] objectsToDisable;

	public GameObject[] objectsToEnable;

	private void Start()
	{
		if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile || TBCore.GetActivePlatform() == VRPlatform.OculusPC || TBCore.GetActivePlatform() == VRPlatform.SteamVR)
		{
			for (int i = 0; i < objectsToDisable.Length; i++)
			{
				objectsToDisable[i].SetActive(value: false);
			}
			for (int j = 0; j < objectsToEnable.Length; j++)
			{
				objectsToEnable[j].SetActive(value: true);
			}
		}
		else
		{
			for (int k = 0; k < objectsToEnable.Length; k++)
			{
				objectsToEnable[k].SetActive(value: false);
			}
		}
	}
}
