using UnityEngine;

public class PhysicsRotate : MonoBehaviour
{
	public Space relativeTo;

	public Vector3 axis;

	public float angle;

	public Rigidbody myRB;

	private float rotatingangle;

	private void FixedUpdate()
	{
		rotatingangle += angle * Time.deltaTime;
		myRB.MoveRotation(Quaternion.AngleAxis(rotatingangle, axis));
	}
}
