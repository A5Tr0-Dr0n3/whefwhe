using UnityEngine;

public class ChazMover : MonoBehaviour
{
	private Rigidbody rb;

	public float moveForce = 1f;

	public Transform targetTransform;

	private Vector3 targetPosition;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (!(targetTransform == null))
		{
			targetPosition = new Vector3(targetTransform.position.x, 2f, targetTransform.position.z);
			if (Vector3.Distance(rb.position, targetPosition) > 1.2f)
			{
				rb.AddForce((targetPosition - rb.position).normalized * Time.deltaTime * moveForce);
			}
		}
	}
}
