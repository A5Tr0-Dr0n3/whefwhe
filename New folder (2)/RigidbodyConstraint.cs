using UnityEngine;

public class RigidbodyConstraint : MonoBehaviour
{
	[SerializeField]
	private float _maxSeparationDistance = 0.65f;

	[SerializeField]
	private Vector3 _localConstraintAxis = new Vector3(0f, -1f, 0f);

	[SerializeField]
	private float _rigidity;

	private Rigidbody _rigidbody;

	private Vector3 _startLocalPos;

	private Quaternion _startLocalRot;

	private Vector3 StartGlobalPos => transform.parent.TransformPoint(_startLocalPos);

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_startLocalPos = transform.localPosition;
		_startLocalRot = transform.localRotation;
	}

	private void FixedUpdate()
	{
		ConstrainToAxis();
		_rigidbody.velocity = _rigidity * (StartGlobalPos - transform.position) * Time.deltaTime;
	}

	private void ConstrainToAxis()
	{
		transform.localRotation = _startLocalRot;
		Vector3 lhs = transform.position - StartGlobalPos;
		Vector3 vector = transform.rotation * _localConstraintAxis;
		float num = Vector3.Dot(lhs, vector);
		if (num > 0f)
		{
			transform.position = StartGlobalPos + Vector3.ClampMagnitude(num * vector, _maxSeparationDistance * 1.05f);
		}
		else
		{
			transform.position = StartGlobalPos;
		}
	}

	private void Update()
	{
		ConstrainToAxis();
	}

	private void LateUpdate()
	{
		ConstrainToAxis();
	}

	private void OnValidate()
	{
		_localConstraintAxis = _localConstraintAxis.normalized;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Vector3 vector = (Application.isPlaying ? StartGlobalPos : transform.position);
		Vector3 vector2 = vector + transform.rotation * (_localConstraintAxis * _maxSeparationDistance);
		Gizmos.DrawSphere(vector, 0.015f);
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(vector + transform.rotation * (_localConstraintAxis * _maxSeparationDistance), 0.01f);
		Gizmos.DrawLine(vector, vector2);
		Mesh sharedMesh = GetComponent<MeshFilter>().sharedMesh;
		if (sharedMesh != null)
		{
			Gizmos.DrawWireMesh(sharedMesh, vector2, transform.rotation, transform.lossyScale);
		}
	}
}
