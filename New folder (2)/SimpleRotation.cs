using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
	public float angularVelocity = 10f;

	private void Update()
	{
		transform.localRotation = Quaternion.AngleAxis(angularVelocity * Time.deltaTime, Vector3.up) * transform.localRotation;
	}
}
