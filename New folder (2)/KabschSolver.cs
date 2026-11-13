using UnityEngine;

public class KabschSolver
{
	private Vector3[] QuatBasis = new Vector3[3];

	private Vector3[] DataCovariance = new Vector3[3];

	private Quaternion OptimalRotation = Quaternion.identity;

	public float scaleRatio = 1f;

	public Matrix4x4 SolveKabsch(Vector3[] inPoints, Vector3[] refPoints, bool solveRotation = true, bool solveScale = false)
	{
		Vector4[] array = new Vector4[refPoints.Length];
		for (int i = 0; i < array.Length; i++)
		{
			ref Vector4 reference = ref array[i];
			reference = new Vector4(refPoints[i].x, refPoints[i].y, refPoints[i].z, 1f);
		}
		return SolveKabsch(inPoints, array, solveRotation, solveScale);
	}

	public Matrix4x4 SolveKabsch(Vector3[] inPoints, Vector4[] refPoints, bool solveRotation = true, bool solveScale = false)
	{
		if (inPoints.Length != refPoints.Length)
		{
			return Matrix4x4.identity;
		}
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < inPoints.Length; i++)
		{
			zero += new Vector3(inPoints[i].x, inPoints[i].y, inPoints[i].z) * refPoints[i].w;
			num += refPoints[i].w;
			zero2 += new Vector3(refPoints[i].x, refPoints[i].y, refPoints[i].z) * refPoints[i].w;
			num2 += refPoints[i].w;
		}
		zero /= num;
		zero2 /= num2;
		if (solveScale)
		{
			float num3 = 0f;
			float num4 = 0f;
			for (int j = 0; j < inPoints.Length; j++)
			{
				num3 += (new Vector3(inPoints[j].x, inPoints[j].y, inPoints[j].z) - zero).magnitude;
				num4 += (new Vector3(refPoints[j].x, refPoints[j].y, refPoints[j].z) - zero2).magnitude;
			}
			scaleRatio = num4 / num3;
		}
		if (solveRotation)
		{
			extractRotation(TransposeMultSubtract(inPoints, refPoints, zero, zero2, DataCovariance), ref OptimalRotation);
		}
		return Matrix4x4.TRS(zero2, Quaternion.identity, Vector3.one * scaleRatio) * Matrix4x4.TRS(Vector3.zero, OptimalRotation, Vector3.one) * Matrix4x4.TRS(-zero, Quaternion.identity, Vector3.one);
	}

	private void extractRotation(Vector3[] A, ref Quaternion q)
	{
		for (int i = 0; i < 9; i++)
		{
			q.FillMatrixFromQuaternion(ref QuatBasis);
			Vector3 vector = (Vector3.Cross(QuatBasis[0], A[0]) + Vector3.Cross(QuatBasis[1], A[1]) + Vector3.Cross(QuatBasis[2], A[2])) * (1f / Mathf.Abs(Vector3.Dot(QuatBasis[0], A[0]) + Vector3.Dot(QuatBasis[1], A[1]) + Vector3.Dot(QuatBasis[2], A[2]) + 1E-09f));
			float magnitude = vector.magnitude;
			if (magnitude < 1E-09f)
			{
				break;
			}
			q = Quaternion.AngleAxis(magnitude * 57.29578f, vector / magnitude) * q;
			q = Quaternion.Lerp(q, q, 0f);
		}
	}

	public static Vector3[] TransposeMultSubtract(Vector3[] vec1, Vector4[] vec2, Vector3 vec1Centroid, Vector3 vec2Centroid, Vector3[] covariance)
	{
		for (int i = 0; i < 3; i++)
		{
			ref Vector3 reference = ref covariance[i];
			reference = Vector3.zero;
		}
		for (int j = 0; j < vec1.Length; j++)
		{
			Vector3 vector = (vec1[j] - vec1Centroid) * vec2[j].w;
			Vector3 vector2 = (new Vector3(vec2[j].x, vec2[j].y, vec2[j].z) - vec2Centroid) * Mathf.Abs(vec2[j].w);
			covariance[0][0] += vector[0] * vector2[0];
			covariance[1][0] += vector[1] * vector2[0];
			covariance[2][0] += vector[2] * vector2[0];
			covariance[0][1] += vector[0] * vector2[1];
			covariance[1][1] += vector[1] * vector2[1];
			covariance[2][1] += vector[2] * vector2[1];
			covariance[0][2] += vector[0] * vector2[2];
			covariance[1][2] += vector[1] * vector2[2];
			covariance[2][2] += vector[2] * vector2[2];
		}
		return covariance;
	}
}
