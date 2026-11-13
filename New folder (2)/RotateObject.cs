using UnityEngine;

public class RotateObject : MonoBehaviour
{
	public Space relativeTo;

	public Vector3 rotation;

	private Transform myTransform;

	private void Start()
	{
		myTransform = transform;
	}

	private void Update()
	{
		myTransform.Rotate(rotation * Time.deltaTime, relativeTo);
	}
}
