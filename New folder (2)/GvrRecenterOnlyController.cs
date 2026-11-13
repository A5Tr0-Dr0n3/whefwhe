using UnityEngine;

public class GvrRecenterOnlyController : MonoBehaviour
{
	private Quaternion lastAppliedYawCorrection = Quaternion.identity;

	private Quaternion yawCorrection = Quaternion.identity;

	private void Update()
	{
		if (GvrControllerInput.State == GvrConnectionState.Connected)
		{
			if (GvrControllerInput.Recentered)
			{
				ApplyYawCorrection();
			}
			else if (!GvrControllerInput.HomeButtonDown && !GvrControllerInput.HomeButtonState)
			{
				yawCorrection = GetYawCorrection();
			}
		}
	}

	private void OnDisable()
	{
		yawCorrection = Quaternion.identity;
		RemoveLastYawCorrection();
	}

	private void ApplyYawCorrection()
	{
		RemoveLastYawCorrection();
		transform.localRotation *= yawCorrection;
		lastAppliedYawCorrection = yawCorrection;
	}

	private void RemoveLastYawCorrection()
	{
		transform.localRotation *= Quaternion.Inverse(lastAppliedYawCorrection);
		lastAppliedYawCorrection = Quaternion.identity;
	}

	private Quaternion GetYawCorrection()
	{
		Vector3 eulerAngles = GvrVRHelpers.GetHeadRotation().eulerAngles;
		return lastAppliedYawCorrection * Quaternion.Euler(0f, eulerAngles.y, 0f);
	}
}
