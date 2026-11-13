using UnityEngine;

public class ConstrainedCircleSimulation : MonoBehaviour
{
	private Vector3 _prevPos;

	private Vector3 _currPos;

	[SerializeField]
	private Transform _target;

	[SerializeField]
	private float _targetRadius = 0.1f;

	[SerializeField]
	private float _constraintRadius = 1f;

	[SerializeField]
	private float _gravityScale = 1f;

	private void Awake()
	{
		_currPos = _target.position;
		_prevPos = _currPos;
	}

	private void Update()
	{
		Vector3 prevPos = _prevPos;
		_prevPos = _currPos;
		Vector3 vector = Physics.gravity * _gravityScale;
		_currPos += _currPos - prevPos + vector * Time.deltaTime * Time.deltaTime;
		_currPos = ConstrainPosition(_currPos);
		_target.position = _currPos;
		_target.rotation = transform.rotation;
	}

	private Vector3 ConstrainPosition(Vector3 pos)
	{
		Vector3 vector = Vector3.ProjectOnPlane(pos - transform.position, transform.up);
		vector = Vector3.ClampMagnitude(vector, _constraintRadius - _targetRadius);
		return transform.position + vector;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position + transform.forward * _constraintRadius);
		Gizmos.DrawLine(transform.position, transform.position + transform.right * _constraintRadius);
		Gizmos.DrawLine(transform.position, transform.position - transform.forward * _constraintRadius);
		Gizmos.DrawLine(transform.position, transform.position - transform.right * _constraintRadius);
		Gizmos.color = Color.black;
		Gizmos.DrawSphere(_target.position, _targetRadius);
	}
}
