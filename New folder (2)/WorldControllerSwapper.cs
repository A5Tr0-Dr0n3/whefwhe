using TButt;
using UnityEngine;

public class WorldControllerSwapper : MonoBehaviour
{
	[Header("SceneReferences")]
	[SerializeField]
	private Transform originalControllerModel;

	[SerializeField]
	private ModelDatabaseObject modelDatabase;

	[SerializeField]
	private TutorialLevelSequence tutorialSequence;

	[SerializeField]
	private GameObject[] objectsToDisable;

	[Header("Tweakables")]
	[SerializeField]
	private bool hacked;

	[SerializeField]
	private float scaleConstant = 1f;

	[SerializeField]
	private Vector3 positionOffset = Vector3.zero;

	[SerializeField]
	private bool rotates;

	[SerializeField]
	private RotateObject rotateSettings;

	internal GameObject worldControllerModel;

	private void Start()
	{
		if (TBCore.GetActivePlatform() == VRPlatform.Daydream)
		{
			return;
		}
		if (objectsToDisable != null)
		{
			for (int i = 0; i < objectsToDisable.Length; i++)
			{
				objectsToDisable[i].SetActive(value: false);
			}
		}
		if (!hacked && modelDatabase.WorldModel != null)
		{
			if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile)
			{
				if (OVRPlugin.productName == "Oculus Go")
				{
					worldControllerModel = Object.Instantiate(modelDatabase.WorldModel);
				}
				else
				{
					worldControllerModel = Object.Instantiate(modelDatabase.GearWorldModel);
				}
			}
			else
			{
				worldControllerModel = Object.Instantiate(modelDatabase.WorldModel);
			}
		}
		else if (hacked && modelDatabase.WorldModelHacked != null)
		{
			if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile)
			{
				if (OVRPlugin.productName == "Oculus Go")
				{
					worldControllerModel = Object.Instantiate(modelDatabase.WorldModelHacked);
				}
				else
				{
					worldControllerModel = Object.Instantiate(modelDatabase.GearWorldModelHacked);
				}
			}
			else
			{
				worldControllerModel = Object.Instantiate(modelDatabase.WorldModelHacked);
			}
		}
		if (worldControllerModel != null)
		{
			MeshRenderer[] componentsInChildren = originalControllerModel.GetComponentsInChildren<MeshRenderer>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].enabled = false;
			}
			worldControllerModel.transform.SetParent(originalControllerModel);
			if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile)
			{
				if (OVRPlugin.productName == "Oculus Go")
				{
					if (!hacked)
					{
						worldControllerModel.transform.localPosition = Vector3.zero + modelDatabase.LocalPosOffset + positionOffset;
						worldControllerModel.transform.localScale = worldControllerModel.transform.localScale * scaleConstant;
						worldControllerModel.transform.localRotation = Quaternion.identity;
					}
					else
					{
						worldControllerModel.transform.localPosition = Vector3.zero + modelDatabase.HackedLocalPosOffset + positionOffset;
						worldControllerModel.transform.localScale = worldControllerModel.transform.localScale * scaleConstant;
						worldControllerModel.transform.localRotation = Quaternion.identity;
					}
				}
				else if (!hacked)
				{
					worldControllerModel.transform.localPosition = Vector3.zero + modelDatabase.GearLocalPosOffset + positionOffset;
					worldControllerModel.transform.localScale = worldControllerModel.transform.localScale * modelDatabase.GearLocalScaleScaler * scaleConstant;
					worldControllerModel.transform.localRotation = Quaternion.identity;
				}
				else
				{
					worldControllerModel.transform.localPosition = Vector3.zero + modelDatabase.HackedGearLocalPosOffset + positionOffset;
					worldControllerModel.transform.localScale = worldControllerModel.transform.localScale * modelDatabase.HackedGearLocalScaleScaler * scaleConstant;
					worldControllerModel.transform.localRotation = Quaternion.identity;
				}
			}
			else if (!hacked)
			{
				worldControllerModel.transform.localPosition = Vector3.zero + modelDatabase.LocalPosOffset + positionOffset;
				worldControllerModel.transform.localScale = worldControllerModel.transform.localScale * scaleConstant;
				worldControllerModel.transform.localRotation = Quaternion.identity;
				if (rotates)
				{
					RotateObject rotateObject = worldControllerModel.AddComponent<RotateObject>();
					rotateObject.relativeTo = rotateSettings.relativeTo;
					rotateObject.rotation = rotateSettings.rotation;
					rotateSettings.enabled = false;
				}
			}
			else
			{
				worldControllerModel.transform.localScale = worldControllerModel.transform.localScale * scaleConstant;
				worldControllerModel.transform.localRotation = Quaternion.identity;
				worldControllerModel.transform.localPosition = Vector3.zero;
				if (rotates)
				{
					RotateObject rotateObject2 = worldControllerModel.AddComponent<RotateObject>();
					rotateObject2.relativeTo = rotateSettings.relativeTo;
					rotateObject2.rotation = rotateSettings.rotation;
					rotateSettings.enabled = false;
				}
			}
		}
		if (!(worldControllerModel != null))
		{
			return;
		}
		TutorialController componentInChildren = worldControllerModel.GetComponentInChildren<TutorialController>();
		if (tutorialSequence != null && componentInChildren != null)
		{
			tutorialSequence.controllerApp.gameObject.SetActive(value: false);
			tutorialSequence.controllerTouch.gameObject.SetActive(value: false);
			tutorialSequence.controllerTouchPos.gameObject.SetActive(value: false);
			tutorialSequence.controllerApp = componentInChildren.appButton.GetComponent<Renderer>();
			tutorialSequence.controllerTouch = componentInChildren.touchPad.GetComponent<Renderer>();
			if (componentInChildren.touchPadPos != null)
			{
				tutorialSequence.controllerTouchPos = componentInChildren.touchPadPos.GetComponent<Renderer>();
			}
			if (componentInChildren.buttonPress != null)
			{
				tutorialSequence.buttonPress = componentInChildren.buttonPress;
			}
			if (componentInChildren.buttonUp != null)
			{
				tutorialSequence.buttonUp = componentInChildren.buttonUp;
			}
			tutorialSequence.rotateForTeleport = componentInChildren.rotateControllerTeleport;
			tutorialSequence.rotateForGrab = componentInChildren.rotateControllerGrab;
			tutorialSequence.teleportEuler = componentInChildren.teleportEulerAngles;
			tutorialSequence.grabEuler = componentInChildren.grabEulerAngles;
			tutorialSequence.dontDisableTouchPos = componentInChildren.dontDisableTouchPad;
		}
	}

	public GameObject GetWorldControllerModel()
	{
		return worldControllerModel;
	}
}
