using TButt;
using UnityEngine;

public class UnderhubOculusSwapper : MonoBehaviour
{
	public GameObject GoMesh;

	public GameObject GearMesh;

	public GameObject DaydreamMesh;

	public GameObject OculusDesktopMesh;

	private void Start()
	{
		if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile)
		{
			if (OVRPlugin.productName == "Oculus Go")
			{
				if (GoMesh != null)
				{
					GoMesh.SetActive(value: true);
				}
				if (GearMesh != null)
				{
					GearMesh.SetActive(value: false);
				}
			}
			else
			{
				if (GearMesh != null)
				{
					GearMesh.SetActive(value: true);
				}
				if (GoMesh != null)
				{
					GoMesh.SetActive(value: false);
				}
			}
			if (DaydreamMesh != null)
			{
				DaydreamMesh.SetActive(value: false);
			}
			if (OculusDesktopMesh != null)
			{
				OculusDesktopMesh.SetActive(value: false);
			}
		}
		else if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
		{
			if (DaydreamMesh != null)
			{
				DaydreamMesh.SetActive(value: false);
			}
			if (GoMesh != null)
			{
				GoMesh.SetActive(value: false);
			}
			if (GearMesh != null)
			{
				GearMesh.SetActive(value: false);
			}
			if (OculusDesktopMesh != null)
			{
				OculusDesktopMesh.SetActive(value: true);
			}
		}
		else
		{
			if (DaydreamMesh != null)
			{
				DaydreamMesh.SetActive(value: true);
			}
			if (GearMesh != null)
			{
				GearMesh.SetActive(value: false);
			}
			if (GoMesh != null)
			{
				GoMesh.SetActive(value: false);
			}
			if (OculusDesktopMesh != null)
			{
				OculusDesktopMesh.SetActive(value: false);
			}
		}
	}
}
