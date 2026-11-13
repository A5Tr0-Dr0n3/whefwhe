using UnityEngine;

public static class MathHelpers
{
	public static float ScaleInputBySymmetricCurve(float input, AnimationCurve curve)
	{
		return curve.Evaluate(Mathf.Abs(input)) * Mathf.Sign(input);
	}

	public static float ScaleInputByAsymmetricCurve(float input, AnimationCurve curve)
	{
		return curve.Evaluate(input);
	}

	public static float LinMap(float inputStart, float inputEnd, float outputStart, float outputEnd, float inputValue)
	{
		float num = inputEnd - inputStart;
		float num2 = outputEnd - outputStart;
		return (inputValue - inputStart) / num * num2 + outputStart;
	}

	public static bool AlmostEquals(this float a, float b)
	{
		return a.AlmostEquals(b, 1E-05f);
	}

	public static bool AlmostEquals(this float a, float b, float precision)
	{
		return Mathf.Abs(a - b) <= precision;
	}

	public static float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 axisNorm)
	{
		return Mathf.Atan2(Vector3.Dot(Vector3.Cross(b, a), -axisNorm), Vector3.Dot(a, b)) * 57.29578f;
	}
}
