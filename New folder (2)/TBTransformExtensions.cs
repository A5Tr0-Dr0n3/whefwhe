using TButt;
using UnityEngine;

public static class TBTransformExtensions
{
	public static void ResetLocalPositionAndRotation(this Transform t)
	{
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
	}

	public static void MakeZeroedChildOf(this Transform t, Transform parent)
	{
		t.parent = parent;
		t.ResetLocalPositionAndRotation();
	}

	public static void FaceCamera(this Transform billboard)
	{
		billboard.LookAt(billboard.position + (billboard.position - TBCameraRig.instance.GetCenter().position));
	}
}
