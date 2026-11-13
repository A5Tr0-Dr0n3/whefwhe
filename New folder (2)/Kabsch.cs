using UnityEngine;

public class Kabsch : MonoBehaviour
{
	public Transform[] inPoints;

	public Transform[] referencePoints;

	private Vector3[] points;

	private Vector4[] refPoints;

	private KabschSolver solver = new KabschSolver();

	private void Start()
	{
		points = new Vector3[inPoints.Length];
		refPoints = new Vector4[inPoints.Length];
		for (int i = 0; i < inPoints.Length; i++)
		{
			ref Vector3 reference = ref points[i];
			reference = inPoints[i].position;
		}
	}

	private void Update()
	{
		for (int i = 0; i < inPoints.Length; i++)
		{
			ref Vector4 reference = ref refPoints[i];
			reference = new Vector4(referencePoints[i].position.x, referencePoints[i].position.y, referencePoints[i].position.z, referencePoints[i].localScale.x);
		}
		Matrix4x4 matrix4x = solver.SolveKabsch(points, refPoints);
		for (int j = 0; j < inPoints.Length; j++)
		{
			inPoints[j].position = matrix4x.MultiplyPoint3x4(points[j]);
		}
	}
}
