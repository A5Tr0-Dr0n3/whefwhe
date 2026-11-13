using UnityEngine;

public class RotateUntilJointBroken : MonoBehaviour
{
	public float angVel = 90f;

	private void Update()
	{
		transform.Rotate(Vector3.up, angVel * Time.deltaTime);
	}

	private void OnJointBreak(float breakForce)
	{
		Object.Destroy(this);
	}
}
