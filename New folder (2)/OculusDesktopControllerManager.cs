using TMPro;
using UnityEngine;

public class OculusDesktopControllerManager : MultiplatformControllerManager
{
	[Header("Rift specific refs")]
	internal GameObject activeJoystick;

	public GameObject leftHackedJoystick;

	public GameObject rightHackedJoystick;

	public GameObject leftControllerRoot;

	public GameObject rightControllerRoot;

	[Header("Right Controller Refts")]
	public GameObject rightControllerGO;

	public AnimationClip rightTransformRightAnimation;

	public AnimationClip rightUnTransformRightAnimation;

	public GameObject rightHackedControllerRoot;

	public GameObject rightVacFillUi;

	public TextMeshPro rightFillText;

	[Header("Scene Tweakables")]
	public bool left = true;

	private void Start()
	{
		if (left)
		{
			activeJoystick = leftHackedJoystick;
			rightControllerRoot.SetActive(value: false);
			leftControllerRoot.SetActive(value: true);
			return;
		}
		activeJoystick = rightHackedJoystick;
		rightControllerRoot.SetActive(value: true);
		leftControllerRoot.SetActive(value: false);
		hackedControllerRoot = rightHackedControllerRoot;
		vacFillUi = rightVacFillUi;
		transformRightAnimation = rightTransformRightAnimation;
		unTransformRightAnimation = rightUnTransformRightAnimation;
		controllerGo = rightControllerGO;
		fillText = rightFillText;
	}
}
