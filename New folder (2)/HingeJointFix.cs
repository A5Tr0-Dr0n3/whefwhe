using UnityEngine;

public class HingeJointFix : MonoBehaviour
{
	private Quaternion initialLocalRotation;

	private Vector3 initialLocalPosition;

	private Quaternion localRotationOnDisable;

	private Vector3 localPositionOnDisable;

	private bool hasDisabled;

	private void Awake()
	{
		initialLocalRotation = transform.localRotation;
		initialLocalPosition = transform.localPosition;
	}

	private void OnDisable()
	{
		ResetToInitialValues();
		hasDisabled = true;
	}

	private void OnEnable()
	{
		if (hasDisabled)
		{
			hasDisabled = false;
			transform.localRotation = localRotationOnDisable;
			transform.localPosition = localPositionOnDisable;
		}
	}

	public void ResetToInitialValues()
	{
		localRotationOnDisable = transform.localRotation;
		transform.localRotation = initialLocalRotation;
		localPositionOnDisable = transform.localPosition;
		transform.localPosition = initialLocalPosition;
	}
}
