using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DetachedRigidBody : MonoBehaviour
{
	[SerializeField]
	public Transform _transform;

	private Rigidbody _rb;

	private void Start()
	{
		_rb = GetComponent<Rigidbody>();
		transform.parent = null;
	}

	private void Update()
	{
		if (_transform == null)
		{
			return;
		}
		if (_transform.gameObject.activeInHierarchy)
		{
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(value: true);
			}
			_rb.MovePosition(_transform.position);
			_rb.MoveRotation(_transform.rotation);
		}
		else if (gameObject.activeSelf)
		{
			gameObject.SetActive(value: false);
		}
	}
}
