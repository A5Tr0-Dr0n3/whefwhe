using UnityEngine;

public class RandomLaunch : MonoBehaviour
{
	private RequireComponent Rigidbody;

	public Rigidbody myRB;

	public Vector3 forward = Vector3.forward;

	private void Start()
	{
		if (myRB == null)
		{
			myRB = GetComponent<Rigidbody>();
		}
		myRB.AddForce(forward * Random.Range(2f, 3f), ForceMode.Impulse);
	}
}
