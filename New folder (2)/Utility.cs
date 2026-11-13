using UnityEngine;

public class Utility
{
	public static float TCVRLerp(float val, float target, float t)
	{
		return Mathf.Lerp(val, target, 1f - Mathf.Pow(1f - t / 60f, 60f * Time.deltaTime));
	}

	public static Vector3 TCVRLerp(Vector3 val, Vector3 target, float t)
	{
		return Vector3.Lerp(val, target, 1f - Mathf.Pow(1f - t / 60f, 60f * Time.deltaTime));
	}

	public static Quaternion TCVRLerp(Quaternion val, Quaternion target, float t)
	{
		return Quaternion.Lerp(val, target, 1f - Mathf.Pow(1f - t / 60f, 60f * Time.deltaTime));
	}
}
