using UnityEngine;

public static class FromMatrixExtension
{
	public static Vector3 GetVector3(this Matrix4x4 m)
	{
		return m.GetColumn(3);
	}

	public static Quaternion GetQuaternion(this Matrix4x4 m)
	{
		if (m.GetColumn(2) == m.GetColumn(1))
		{
			return Quaternion.identity;
		}
		return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
	}

	public static void FillMatrixFromQuaternion(this Quaternion q, ref Vector3[] covariance)
	{
		ref Vector3 reference = ref covariance[0];
		reference = q * Vector3.right;
		ref Vector3 reference2 = ref covariance[1];
		reference2 = q * Vector3.up;
		ref Vector3 reference3 = ref covariance[2];
		reference3 = q * Vector3.forward;
	}

	public static Matrix4x4 Lerp(Matrix4x4 a, Matrix4x4 b, float alpha)
	{
		return Matrix4x4.TRS(Vector3.Lerp(a.GetVector3(), b.GetVector3(), alpha), Quaternion.Slerp(a.GetQuaternion(), b.GetQuaternion(), alpha), Vector3.one);
	}
}
