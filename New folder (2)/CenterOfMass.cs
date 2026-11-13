using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
	public Vector3 centerOfMass;

	private void Start()
	{
		if ((bool)GetComponent<Rigidbody>())
		{
			gameObject.GetComponent<Rigidbody>().centerOfMass = centerOfMass;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(transform.TransformPoint(centerOfMass), 0.01f);
	}
}
