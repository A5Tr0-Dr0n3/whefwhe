using UnityEngine;

public class RotateBoidFlock : MonoBehaviour
{
	public Space relativeTo;

	public Vector3 rotation;

	private Transform myTransform;

	public PanopticonController panoController;

	private void Start()
	{
		myTransform = transform;
	}

	private void Update()
	{
		myTransform.Rotate(rotation * panoController.rotationScalar * Time.deltaTime, relativeTo);
	}
}
