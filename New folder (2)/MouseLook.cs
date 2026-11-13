using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
	public enum RotationAxes
	{
		MouseX = 1,
		MouseY
	}

	public RotationAxes axes = RotationAxes.MouseX;

	public bool invertY;

	public float sensitivityX = 10f;

	public float sensitivityY = 9f;

	public float minimumX = -360f;

	public float maximumX = 360f;

	public float minimumY = -85f;

	public float maximumY = 85f;

	private float rotationX;

	private float rotationY;

	private List<float> rotArrayX = new List<float>();

	private float rotAverageX;

	private List<float> rotArrayY = new List<float>();

	private float rotAverageY;

	public float framesOfSmoothing = 5f;

	private Quaternion originalRotation;

	private void Start()
	{
		if ((bool)GetComponent<Rigidbody>())
		{
			GetComponent<Rigidbody>().freezeRotation = true;
		}
		originalRotation = transform.localRotation;
	}

	public void SetSensitivity(float s)
	{
		sensitivityX = s;
		sensitivityY = s;
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		angle %= 360f;
		if (angle >= -360f && angle <= 360f)
		{
			if (angle < -360f)
			{
				angle += 360f;
			}
			if (angle > 360f)
			{
				angle -= 360f;
			}
		}
		return Mathf.Clamp(angle, min, max);
	}
}
