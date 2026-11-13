using TButt;
using UnityEngine;

public class VacuumTutorialSwapper : MonoBehaviour
{
	[SerializeField]
	private bool firstController;

	[SerializeField]
	internal WorldControllerSwapper controllerRefs;

	[SerializeField]
	private bool vacuumIn = true;

	internal VacuumController worldController;

	private bool hasDoneLateStart;

	private void Update()
	{
		if (!hasDoneLateStart)
		{
			lateStart();
		}
	}

	private void lateStart()
	{
		if (TBCore.GetActivePlatform() == VRPlatform.Daydream)
		{
			hasDoneLateStart = true;
			return;
		}
		if (controllerRefs.GetWorldControllerModel() != null)
		{
			worldController = controllerRefs.GetWorldControllerModel().GetComponent<VacuumController>();
		}
		hasDoneLateStart = true;
		if (!firstController)
		{
			worldController.helloBeeText.SetActive(value: true);
			if (vacuumIn)
			{
				worldController.touchpadDown.SetActive(value: true);
				worldController.touchpadUp.SetActive(value: false);
				worldController.blueGlowCard.SetActive(value: true);
				worldController.originalTouchpad[0].SetActive(value: false);
				if (worldController.rotatesForSuck)
				{
					worldController.transform.localRotation = Quaternion.Euler(worldController.suckEuler);
				}
			}
			else
			{
				worldController.touchpadUp.SetActive(value: true);
				worldController.touchpadDown.SetActive(value: false);
				worldController.redGlowCard.SetActive(value: true);
				worldController.originalTouchpad[1].SetActive(value: false);
				if (worldController.rotatesForBlow)
				{
					worldController.transform.localRotation = Quaternion.Euler(worldController.blowEuler);
				}
			}
		}
		else
		{
			worldController.helloBeeText.SetActive(value: true);
		}
	}
}
