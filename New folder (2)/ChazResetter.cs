using UnityEngine;

public class ChazResetter : MonoBehaviour
{
	public ChazController chaz;

	public HingeJoint leftLeg;

	public HingeJoint rightLeg;

	public HingeJoint leftFoot;

	public HingeJoint rightFoot;

	private Vector3 originalMouthPosition;

	private Vector3 originalLeftHipPosition;

	private Vector3 originalRightHipPosition;

	private Vector3 originalLeftLegPosition;

	private Vector3 originalRightLegPosition;

	private Vector3 originalLeftFootPosition;

	private Vector3 originalRightFootPosition;

	private Vector3 originalHedPosition;

	private Vector3 originalControllerPos;

	private Quaternion originalMouthRotation;

	private Quaternion originalLeftHipRotation;

	private Quaternion originalRightHipRotation;

	private Quaternion originalLeftLegRotation;

	private Quaternion originalRightLegRotation;

	private Quaternion originalLeftFootRotation;

	private Quaternion originalRightFootRotation;

	private Quaternion originalHedRotation;

	private Quaternion originalControllerRotation;

	public void ResetChaz(Vector3 pos, bool move = true)
	{
		chaz.DisablePhysics();
		chaz.ResetDrag();
		chaz.ResetSpring();
		chaz.rootPositionerRB.isKinematic = true;
		chaz.rootPositionerRB.velocity = Vector3.zero;
		chaz.rootPositionerRB.angularVelocity = Vector3.zero;
		chaz.DropChaz();
		chaz.StopCoroutines();
		chaz.mouth.transform.localPosition = originalMouthPosition;
		chaz.leftHip.transform.localPosition = originalLeftHipPosition;
		chaz.rightHip.transform.localPosition = originalRightHipPosition;
		leftLeg.transform.localPosition = originalLeftLegPosition;
		rightLeg.transform.localPosition = originalRightLegPosition;
		leftFoot.transform.localPosition = originalLeftFootPosition;
		rightFoot.transform.localPosition = originalRightFootPosition;
		chaz.head.transform.localPosition = originalHedPosition;
		chaz.transform.localPosition = originalControllerPos;
		chaz.mouth.transform.localRotation = originalMouthRotation;
		chaz.leftHip.transform.localRotation = originalLeftHipRotation;
		chaz.rightHip.transform.localRotation = originalRightHipRotation;
		leftLeg.transform.localRotation = originalLeftLegRotation;
		rightLeg.transform.localRotation = originalRightLegRotation;
		leftFoot.transform.localRotation = originalLeftFootRotation;
		rightFoot.transform.localRotation = originalRightFootRotation;
		chaz.head.transform.localRotation = originalHedRotation;
		chaz.transform.localRotation = originalControllerRotation;
		if (move)
		{
			chaz.MoveChaz(pos);
		}
		chaz.SetChazMode(ChazController.chazMode.standing);
		chaz.canStep = true;
		chaz.rootPositionerRB.isKinematic = false;
	}

	private void Awake()
	{
		originalMouthPosition = chaz.mouth.transform.localPosition;
		originalLeftHipPosition = chaz.leftHip.transform.localPosition;
		originalRightHipPosition = chaz.rightHip.transform.localPosition;
		originalLeftLegPosition = leftLeg.transform.localPosition;
		originalRightLegPosition = rightLeg.transform.localPosition;
		originalRightFootPosition = rightFoot.transform.localPosition;
		originalLeftFootPosition = leftFoot.transform.localPosition;
		originalHedPosition = chaz.head.transform.localPosition;
		originalControllerPos = chaz.transform.localPosition;
		originalMouthRotation = chaz.mouth.transform.localRotation;
		originalLeftHipRotation = chaz.leftHip.transform.localRotation;
		originalRightHipRotation = chaz.rightHip.transform.localRotation;
		originalLeftLegRotation = leftLeg.transform.localRotation;
		originalRightLegRotation = rightLeg.transform.localRotation;
		originalRightFootRotation = rightFoot.transform.localRotation;
		originalLeftFootRotation = leftFoot.transform.localRotation;
		originalHedRotation = chaz.head.transform.localRotation;
		originalControllerRotation = chaz.transform.localRotation;
	}

	private void Update()
	{
	}
}
