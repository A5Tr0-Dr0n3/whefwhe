using System;
using UnityEngine;
using UnityEngine.EventSystems;

public static class GvrMathHelpers
{
	public static Vector3 GetIntersectionPosition(Camera cam, RaycastResult raycastResult)
	{
		if (cam == null)
		{
			return Vector3.zero;
		}
		float num = raycastResult.distance + cam.nearClipPlane;
		return cam.transform.position + cam.transform.forward * num;
	}

	public static Vector2 NormalizedCartesianToSpherical(Vector3 cartCoords)
	{
		cartCoords.Normalize();
		if (cartCoords.x == 0f)
		{
			cartCoords.x = Mathf.Epsilon;
		}
		float num = Mathf.Atan(cartCoords.z / cartCoords.x);
		if (cartCoords.x < 0f)
		{
			num += (float)Math.PI;
		}
		float y = Mathf.Asin(cartCoords.y);
		return new Vector2(num, y);
	}

	public static float EaseOutCubic(float min, float max, float value)
	{
		if (min > max)
		{
			Debug.LogError("Invalid values passed to EaseOutCubic, max must be greater than min. min: " + min + ", max: " + max);
			return value;
		}
		value = Mathf.Clamp01(value);
		value--;
		float num = max - min;
		return num * (value * value * value + 1f) + min;
	}

	public static Matrix4x4 ConvertFloatArrayToMatrix(float[] floatArray)
	{
		Matrix4x4 result = default;
		if (floatArray == null || floatArray.Length != 16)
		{
			throw new ArgumentException("floatArray must not be null and have a length of 16.");
		}
		result[0, 0] = floatArray[0];
		result[1, 0] = floatArray[1];
		result[2, 0] = floatArray[2];
		result[3, 0] = floatArray[3];
		result[0, 1] = floatArray[4];
		result[1, 1] = floatArray[5];
		result[2, 1] = floatArray[6];
		result[3, 1] = floatArray[7];
		result[0, 2] = floatArray[8];
		result[1, 2] = floatArray[9];
		result[2, 2] = floatArray[10];
		result[3, 2] = floatArray[11];
		result[0, 3] = floatArray[12];
		result[1, 3] = floatArray[13];
		result[2, 3] = floatArray[14];
		result[3, 3] = floatArray[15];
		return result;
	}
}
