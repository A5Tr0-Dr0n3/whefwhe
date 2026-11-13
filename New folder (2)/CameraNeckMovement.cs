using System;
using UnityEngine;

public class CameraNeckMovement : MonoBehaviour
{
	public float neckRadius = 0.125f;

	private void Start()
	{
	}

	private void Update()
	{
		float num = neckRadius * Mathf.Cos(transform.localEulerAngles.x * ((float)Math.PI / 180f));
		float x = num * Mathf.Sin(transform.localEulerAngles.y * ((float)Math.PI / 180f));
		float z = num * Mathf.Cos(transform.localEulerAngles.y * ((float)Math.PI / 180f));
		transform.localPosition = new Vector3(x, 0f, z);
	}
}
