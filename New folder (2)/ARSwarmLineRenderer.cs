using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ARSwarmLineRenderer : MonoBehaviour
{
	public float lineSegmentSize = 0.15f;

	public float lineWidth = 0.1f;

	private ARSwarmPoint[] linePoints = new ARSwarmPoint[0];

	private Vector3[] linePositions = new Vector3[0];

	private Vector3[] linePositionsOld = new Vector3[0];

	public void CreateLine(LineRenderer line)
	{
		GetPoints();
		SetPointsToLine(line);
	}

	private void GetPoints()
	{
		linePoints = GetComponentsInChildren<ARSwarmPoint>();
		linePositions = new Vector3[linePoints.Length];
		for (int i = 0; i < linePoints.Length; i++)
		{
			ref Vector3 reference = ref linePositions[i];
			reference = linePoints[i].transform.position;
		}
	}

	private void SetPointsToLine(LineRenderer lineRef)
	{
		Vector3[] array = ARSwarmLineSmoother.SmoothLine(linePositions, lineSegmentSize);
		lineRef.SetVertexCount(array.Length);
		lineRef.SetPositions(array);
		lineRef.SetWidth(lineWidth, lineWidth);
	}
}
