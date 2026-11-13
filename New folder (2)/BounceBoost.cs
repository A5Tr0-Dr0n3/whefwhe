using UnityEngine;

public class BounceBoost : MonoBehaviour
{
	private Rigidbody myRB;

	private ObjectBase bounceBase;

	private void Start()
	{
		myRB = GetComponent<Rigidbody>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		bounceBase = collision.collider.transform.GetComponent<ObjectBase>();
		if (bounceBase != null && bounceBase.rb != null && bounceBase.rb.mass > 100f)
		{
			myRB.AddForce(collision.relativeVelocity * 100f, ForceMode.Impulse);
			myRB.AddForce(collision.relativeVelocity.magnitude * Vector3.up * 50f, ForceMode.Impulse);
		}
	}
}
