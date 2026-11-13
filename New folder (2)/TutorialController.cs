using UnityEngine;

public class TutorialController : MonoBehaviour
{
	public GameObject appButton;

	public GameObject touchPad;

	public GameObject touchPadPos;

	public Material buttonPress;

	public Material buttonUp;

	public bool rotateControllerTeleport = true;

	public bool rotateControllerGrab;

	public bool dontDisableTouchPad;

	public Vector3 teleportEulerAngles;

	public Vector3 grabEulerAngles;

	private void Awake()
	{
		if (grabEulerAngles == Vector3.zero)
		{
			grabEulerAngles = transform.rotation.eulerAngles;
		}
	}
}
