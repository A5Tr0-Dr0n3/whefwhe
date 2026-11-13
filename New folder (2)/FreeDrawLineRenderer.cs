using System.Collections.Generic;
using UnityEngine;

public class FreeDrawLineRenderer : MonoBehaviour
{
	private List<Vector3> localLineRendererPoints;

	private Vector3[] worldLineRendererPoints;

	private LineRenderer mLineRenderer;

	private Transform strokeParent;

	private static int idCounter;

	private Vector3 oldParentPos;

	private Quaternion oldParentRot;

	public int id { get; private set; }

	public void Initialize(float widthIn, Color colorIn, int cornerVertsIn, Material matIn, Transform parentIn)
	{
		localLineRendererPoints = new List<Vector3>();
		mLineRenderer = gameObject.AddComponent<LineRenderer>();
		mLineRenderer.numCornerVertices = cornerVertsIn;
		mLineRenderer.startColor = colorIn;
		mLineRenderer.endColor = colorIn;
		mLineRenderer.startWidth = widthIn;
		mLineRenderer.endWidth = widthIn;
		mLineRenderer.sharedMaterial = matIn;
		strokeParent = parentIn;
		id = idCounter++;
	}

	private void Update()
	{
		if (strokeParent != null && (oldParentPos != strokeParent.position || oldParentRot != strokeParent.rotation))
		{
			recalculatePoints();
			strokeParent.hasChanged = false;
		}
		oldParentPos = strokeParent.position;
		oldParentRot = strokeParent.rotation;
	}

	public void AddPoint(Vector3 worldSpacePoint)
	{
		Vector3 item = strokeParent.InverseTransformPoint(worldSpacePoint);
		localLineRendererPoints.Add(item);
		recalculatePoints();
	}

	private void recalculatePoints()
	{
		worldLineRendererPoints = new Vector3[localLineRendererPoints.Count];
		for (int i = 0; i < worldLineRendererPoints.Length; i++)
		{
			ref Vector3 reference = ref worldLineRendererPoints[i];
			reference = strokeParent.TransformPoint(localLineRendererPoints[i]);
		}
		mLineRenderer.positionCount = worldLineRendererPoints.Length;
		mLineRenderer.SetPositions(worldLineRendererPoints);
	}

	public void CacheParentTransform(Transform copyParent)
	{
		Transform transform = new GameObject("Cached " + strokeParent.name + " Transform copy").transform;
		transform.parent = strokeParent.parent;
		transform.position = strokeParent.position;
		transform.rotation = strokeParent.rotation;
		transform.localScale = strokeParent.localScale;
		transform.parent = copyParent;
		strokeParent = transform;
	}

	public bool TickUndrawTrail(float deltaTime, float trailUndrawSpeed)
	{
		float num = trailUndrawSpeed * deltaTime;
		while (num > 0f)
		{
			if (localLineRendererPoints.Count < 2)
			{
				return true;
			}
			Vector3 vector = localLineRendererPoints[0];
			Vector3 vector2 = localLineRendererPoints[1];
			float num2 = Vector3.Distance(vector, vector2);
			if (num2 < num)
			{
				localLineRendererPoints.RemoveAt(0);
				num -= num2;
			}
			else
			{
				localLineRendererPoints[0] = vector2 + (vector - vector2).normalized * num;
				num = 0f;
			}
		}
		recalculatePoints();
		return false;
	}

	public Vector3 GetLastPoint()
	{
		return worldLineRendererPoints[worldLineRendererPoints.Length - 1];
	}
}
