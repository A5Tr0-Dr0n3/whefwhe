using TMPro;
using UnityEngine;

public class OculusMobileControllerManager : MultiplatformControllerManager
{
	[Header("Gear Controllers")]
	public GameObject gearControllerGO;

	public AnimationClip transformRightAnimationGear;

	public AnimationClip unTransformRightAnimationGear;

	public MeshRenderer hackedControllerTopGear;

	public MeshRenderer hackedControllerBottomGear;

	public GameObject gearHackedControllerRoot;

	public GameObject vacFillUiGear;

	public TextMeshPro fillTextGear;

	public Vector3 gearTipOffset = Vector3.zero;

	private void Awake()
	{
		if (OVRPlugin.productName == "Oculus Go")
		{
			controllerGo.SetActive(value: true);
			gearControllerGO.SetActive(value: false);
			return;
		}
		controllerGo.SetActive(value: false);
		gearControllerGO.SetActive(value: true);
		hackedControllerRoot = gearHackedControllerRoot;
		hackedControllerTop = hackedControllerTopGear;
		hackedControllerBottom = hackedControllerBottomGear;
		vacFillUi = vacFillUiGear;
		transformRightAnimation = transformRightAnimationGear;
		unTransformRightAnimation = unTransformRightAnimationGear;
		controllerGo = gearControllerGO;
		fillText = fillTextGear;
	}
}
