using UnityEngine;

public class TransformTargetRocketScottsdale : MonoBehaviour
{
	private enum ForceOrVel
	{
		Force,
		Velocity,
		None
	}

	[SerializeField]
	private Rigidbody _rb;

	[SerializeField]
	private Transform _target;

	[Space(10f)]
	[SerializeField]
	private ForceOrVel _positionMode;

	[Header("\tPosition - Force")]
	[SerializeField]
	private float _maxForce = 100f;

	[SerializeField]
	private float _counterForce = 0.01f;

	[SerializeField]
	private float _reductionFactor = 0.09f;

	[Header("\tPosition - Velocity")]
	[SerializeField]
	private float _maxPositionVelocity = 1f;

	[Space(10f)]
	[SerializeField]
	private ForceOrVel _rotationMode;

	[Header("\tRotation - Force")]
	[SerializeField]
	private float rotationStrength = 10f;

	[SerializeField]
	[Range(0f, 1f)]
	private float counterRotationStrength = 0.25f;

	[Header("\tRotation - Velocity")]
	[SerializeField]
	private float _massCounterForMaxRotPerFrame = 11f;

	private void FixedUpdate()
	{
		float num = 1f;
		Vector3 position = _rb.position;
		Quaternion rotation = _rb.rotation;
		switch (_positionMode)
		{
		case ForceOrVel.Force:
		{
			Vector3 vector2 = _target.position - position;
			Vector3 vector3 = vector2 / Time.deltaTime / Time.deltaTime - _rb.velocity / Time.deltaTime;
			Vector3 vector4 = vector3 * _reductionFactor;
			Vector3 vector5 = _counterForce * -_rb.velocity / Time.deltaTime * 1f / (vector2.magnitude + 0.1f);
			Vector3 vector6 = Vector3.ClampMagnitude(vector4, _maxForce) + vector5;
			_rb.AddForce(vector6 * num, ForceMode.Force);
			break;
		}
		case ForceOrVel.Velocity:
		{
			Vector3 vector = _target.position - position;
			_rb.velocity = Vector3.ClampMagnitude(vector / Time.deltaTime, _maxPositionVelocity);
			break;
		}
		}
		switch (_rotationMode)
		{
		case ForceOrVel.Velocity:
		{
			(_target.rotation * Quaternion.Inverse(rotation)).ToAngleAxis(out var angle2, out var axis2);
			if (angle2 > 180f)
			{
				angle2 -= 360f;
			}
			if (angle2 != 0f)
			{
				Vector3 target = angle2 * axis2;
				_rb.angularVelocity = Vector3.MoveTowards(_rb.angularVelocity, target, num * (_massCounterForMaxRotPerFrame - _rb.mass));
			}
			_rb.maxAngularVelocity = float.MaxValue;
			break;
		}
		case ForceOrVel.Force:
		{
			Quaternion quaternion = _target.rotation * Quaternion.Inverse(rotation);
			float num2 = Quaternion.Angle(_target.rotation, _rb.rotation) / 180f;
			quaternion.ToAngleAxis(out var angle, out var axis);
			if (angle > 180f)
			{
				angle -= 360f;
			}
			if (angle != 0f)
			{
				Vector3 vector7 = angle * axis;
				_rb.AddTorque(vector7 * Time.deltaTime * rotationStrength * num2, ForceMode.Impulse);
			}
			_rb.AddTorque(-_rb.angularVelocity * (1f - num2) * counterRotationStrength);
			_rb.maxAngularVelocity = float.MaxValue;
			break;
		}
		}
	}
}
