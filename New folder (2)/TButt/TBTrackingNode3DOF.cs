namespace TButt;

public class TBTrackingNode3DOF : TBTrackingNodeBase
{
	protected override void OnUpdatePoses()
	{
		transform.localPosition = TB3DOFArmModel.instance.pointerPosition + TBCameraRig.instance.GetCenter().localPosition;
		transform.localRotation = TB3DOFArmModel.instance.pointerRotation;
	}
}
