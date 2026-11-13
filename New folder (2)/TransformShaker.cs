using UnityEngine;

public class TransformShaker : MonoBehaviour
{
	public bool usePosition = true;

	public bool useRotation = true;

	public bool useScale;

	public Shaker position;

	public Shaker rotation;

	public Shaker scale;

	private Vector3 initialPosition;

	private Quaternion initialRotation;

	private Vector3 initialScale;

	private void Awake()
	{
		if (!usePosition)
		{
			position.frequency = 0f;
			position.amount = 0f;
		}
		if (!useRotation)
		{
			rotation.frequency = 0f;
			rotation.amount = 0f;
		}
		if (!useScale)
		{
			scale.frequency = 0f;
			scale.amount = 0f;
		}
		SetInitialValues();
	}

	public void SetInitialValues()
	{
		initialPosition = transform.localPosition;
		initialRotation = transform.localRotation;
		initialScale = transform.localScale;
	}

	private void Update()
	{
		if (usePosition && position.frequency > 0f)
		{
			position.Update(Time.deltaTime);
			transform.localPosition = initialPosition + position.Position;
		}
		if (useRotation && rotation.frequency > 0f)
		{
			rotation.Update(Time.deltaTime);
			transform.localRotation = rotation.YawPitch * initialRotation;
		}
		if (useScale && scale.frequency > 0f)
		{
			scale.Update(Time.deltaTime);
			transform.localScale = initialScale + scale.Scalar * Vector3.one;
		}
	}

	public void Reshake()
	{
		position.Reset();
		rotation.Reset();
		scale.Reset();
	}
}
