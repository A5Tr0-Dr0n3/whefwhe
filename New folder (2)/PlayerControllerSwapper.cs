using TButt;
using UnityEngine;

public class PlayerControllerSwapper : MonoBehaviour
{
	[Header("Oculus Specific Variables")]
	[SerializeField]
	private bool leftHand;

	[Header("Scene References")]
	[SerializeField]
	private Transform originalPlayerController;

	[SerializeField]
	private ModelDatabaseObject controllerDatabase;

	[SerializeField]
	private Manipulator manipulator;

	[SerializeField]
	private DetachedRigidBody oculusRB;

	[Header("Internal References")]
	internal MultiplatformControllerManager playerController;

	private GameObject realPlayerControllerModel;

	private Vector3 startControllerPos;

	private bool hasDoneLateStart;

	private void Awake()
	{
		if (TBCore.GetActivePlatform() == VRPlatform.Daydream)
		{
			return;
		}
		if (controllerDatabase.ControllerModel != null)
		{
			startControllerPos = originalPlayerController.transform.localPosition;
			originalPlayerController.gameObject.SetActive(value: false);
			Object.Destroy(originalPlayerController.gameObject);
			realPlayerControllerModel = Object.Instantiate(controllerDatabase.ControllerModel.gameObject);
			if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
			{
				realPlayerControllerModel.GetComponent<OculusDesktopControllerManager>().left = leftHand;
			}
			playerController = realPlayerControllerModel.GetComponent<MultiplatformControllerManager>();
			playerController.transform.position = startControllerPos;
		}
		if (realPlayerControllerModel != null && controllerDatabase.ControllerModel != null)
		{
			realPlayerControllerModel.transform.SetParent(originalPlayerController.parent);
			realPlayerControllerModel.transform.localPosition = Vector3.zero;
			realPlayerControllerModel.transform.rotation = Quaternion.identity;
		}
	}

	private void LateStart()
	{
		swapOutControllerRefs();
	}

	private void Update()
	{
		if (!hasDoneLateStart)
		{
			LateStart();
			hasDoneLateStart = true;
		}
	}

	private void swapOutControllerRefs()
	{
		if (!(playerController != null))
		{
			return;
		}
		if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
		{
			manipulator.controllerGO = playerController.controllerGo;
			manipulator.transformRightAnim = playerController.transformRightAnimation;
			manipulator.unTransformRightAnim = playerController.unTransformRightAnimation;
			manipulator.vacFillUI = playerController.vacFillUi;
			manipulator.fillText = playerController.fillText;
			oculusRB._transform = playerController.transform;
			if (playerController.leashTipOffset != Vector3.zero)
			{
				manipulator.tipMesh.transform.localPosition = playerController.leashTipOffset;
			}
		}
		else if (OVRPlugin.productName == "Oculus Go")
		{
			manipulator.controllerGO = playerController.controllerGo;
			manipulator.transformRightAnim = playerController.transformRightAnimation;
			manipulator.unTransformRightAnim = playerController.unTransformRightAnimation;
			manipulator.HackedTouchpadTop = playerController.hackedControllerTop;
			manipulator.HackedTouchpadBottom = playerController.hackedControllerBottom;
			manipulator.vacFillUI = playerController.vacFillUi;
			manipulator.fillText = playerController.fillText;
			if (playerController.leashTipOffset != Vector3.zero)
			{
				manipulator.tipMesh.transform.localPosition = playerController.leashTipOffset;
			}
		}
		else if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile)
		{
			manipulator.controllerGO = playerController.GetComponent<OculusMobileControllerManager>().gearControllerGO;
			manipulator.transformRightAnim = playerController.GetComponent<OculusMobileControllerManager>().transformRightAnimationGear;
			manipulator.unTransformRightAnim = playerController.GetComponent<OculusMobileControllerManager>().unTransformRightAnimationGear;
			manipulator.HackedTouchpadTop = playerController.GetComponent<OculusMobileControllerManager>().hackedControllerTopGear;
			manipulator.HackedTouchpadBottom = playerController.GetComponent<OculusMobileControllerManager>().hackedControllerBottomGear;
			manipulator.vacFillUI = playerController.GetComponent<OculusMobileControllerManager>().vacFillUiGear;
			manipulator.fillText = playerController.GetComponent<OculusMobileControllerManager>().fillText;
			if (playerController.leashTipOffset != Vector3.zero)
			{
				manipulator.tipMesh.transform.localPosition = playerController.GetComponent<OculusMobileControllerManager>().gearTipOffset;
			}
		}
		else
		{
			manipulator.controllerGO = playerController.controllerGo;
			manipulator.transformRightAnim = playerController.transformRightAnimation;
			manipulator.unTransformRightAnim = playerController.unTransformRightAnimation;
			manipulator.HackedTouchpadTop = playerController.hackedControllerTop;
			manipulator.HackedTouchpadBottom = playerController.hackedControllerBottom;
			manipulator.vacFillUI = playerController.vacFillUi;
			manipulator.fillText = playerController.fillText;
			if (playerController.leashTipOffset != Vector3.zero)
			{
				manipulator.tipMesh.transform.localPosition = playerController.leashTipOffset;
			}
		}
	}
}
