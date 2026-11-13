using TButt;
using UnityEngine;

public class ControllerConnectivityChecker : MonoBehaviour
{
	public GameObject connectControllerText;

	public GameObject loginMenu;

	public GameObject gearController;

	public GameObject goController;

	public GameObject activitudeLogo;

	public GameObject hacktivitudeLogo;

	public GameObject logoGlowCard;

	private bool dontSetThingsUp;

	private void Update()
	{
		if (dontSetThingsUp || TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
		{
			return;
		}
		if (OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote) || OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
		{
			connectControllerText.SetActive(value: false);
			ToolManager.ToggleManipulator(toggle: true);
			loginMenu.SetActive(value: true);
			activitudeLogo.SetActive(value: true);
			gearController.SetActive(value: false);
			goController.SetActive(value: false);
			hacktivitudeLogo.SetActive(value: true);
			logoGlowCard.SetActive(value: true);
			return;
		}
		connectControllerText.SetActive(value: true);
		ToolManager.ToggleManipulator(toggle: false);
		loginMenu.SetActive(value: false);
		activitudeLogo.SetActive(value: false);
		hacktivitudeLogo.SetActive(value: false);
		logoGlowCard.SetActive(value: false);
		if (OVRPlugin.productName == "Oculus Go")
		{
			goController.SetActive(value: true);
			gearController.SetActive(value: false);
		}
		else
		{
			gearController.SetActive(value: true);
			goController.SetActive(value: false);
		}
	}

	private void Awake()
	{
		if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
		{
			DisableObjectEnabler();
			Object.Destroy(gameObject);
		}
	}

	public void DisableObjectEnabler()
	{
		dontSetThingsUp = true;
	}

	public void EnableObjectEnabler()
	{
		if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
		{
			DisableObjectEnabler();
		}
		else
		{
			dontSetThingsUp = false;
		}
	}
}
