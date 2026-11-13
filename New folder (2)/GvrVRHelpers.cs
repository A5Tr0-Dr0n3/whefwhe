using UnityEngine;
using UnityEngine.XR;

public static class GvrVRHelpers
{
	public static Vector2 GetViewportCenter()
	{
		int num = Screen.width;
		int num2 = Screen.height;
		if (XRSettings.enabled)
		{
			num = XRSettings.eyeTextureWidth;
			num2 = XRSettings.eyeTextureHeight;
		}
		return new Vector2(0.5f * (float)num, 0.5f * (float)num2);
	}

	public static Vector3 GetHeadForward()
	{
		return GetHeadRotation() * Vector3.forward;
	}

	public static Quaternion GetHeadRotation()
	{
		return InputTracking.GetLocalRotation(XRNode.Head);
	}

	public static Vector3 GetHeadPosition()
	{
		return InputTracking.GetLocalPosition(XRNode.Head);
	}
}
