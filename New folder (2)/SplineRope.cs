using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SplineRope : MonoBehaviour
{
	public List<GameObject> controlPoints = new List<GameObject>();

	public Color color = Color.white;

	public int numberOfPoints = 20;

	public LineRenderer lineRenderer;

	private Vector3 p0;

	private Vector3 p1;

	private Vector3 m0;

	private Vector3 m1;

	private Vector3 position;

	private void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
		if (!lineRenderer)
		{
			lineRenderer = gameObject.AddComponent<LineRenderer>();
			lineRenderer.useWorldSpace = true;
			lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
		}
	}

	private void Update()
	{
		if (null == lineRenderer || controlPoints == null || controlPoints.Count < 2)
		{
			return;
		}
		lineRenderer.startColor = color;
		lineRenderer.endColor = color;
		if (numberOfPoints < 2)
		{
			numberOfPoints = 2;
		}
		lineRenderer.positionCount = numberOfPoints * (controlPoints.Count - 1);
		for (int i = 0; i < controlPoints.Count - 1 && !(controlPoints[i] == null) && !(controlPoints[i + 1] == null) && (i <= 0 || !(controlPoints[i - 1] == null)) && (i >= controlPoints.Count - 2 || !(controlPoints[i + 2] == null)); i++)
		{
			p0 = controlPoints[i].transform.position;
			p1 = controlPoints[i + 1].transform.position;
			if (i > 0)
			{
				m0 = 0.5f * (controlPoints[i + 1].transform.position - controlPoints[i - 1].transform.position);
			}
			else
			{
				m0 = controlPoints[i + 1].transform.position - controlPoints[i].transform.position;
			}
			if (i < controlPoints.Count - 2)
			{
				m1 = 0.5f * (controlPoints[i + 2].transform.position - controlPoints[i].transform.position);
			}
			else
			{
				m1 = controlPoints[i + 1].transform.position - controlPoints[i].transform.position;
			}
			float num = 1f / (float)numberOfPoints;
			if (i == controlPoints.Count - 2)
			{
				num = 1f / ((float)numberOfPoints - 1f);
			}
			for (int j = 0; j < numberOfPoints; j++)
			{
				float num2 = (float)j * num;
				position = (2f * num2 * num2 * num2 - 3f * num2 * num2 + 1f) * p0 + (num2 * num2 * num2 - 2f * num2 * num2 + num2) * m0 + (-2f * num2 * num2 * num2 + 3f * num2 * num2) * p1 + (num2 * num2 * num2 - num2 * num2) * m1;
				lineRenderer.SetPosition(j + i * numberOfPoints, position);
			}
		}
	}

	public Vector3 PointOnSpline(float t)
	{
		int num = Mathf.FloorToInt((float)controlPoints.Count * t);
		if (num == controlPoints.Count - 1)
		{
			return controlPoints[controlPoints.Count - 1].transform.position;
		}
		p0 = controlPoints[num].transform.position;
		p1 = controlPoints[num + 1].transform.position;
		if (num > 0)
		{
			m0 = 0.5f * (controlPoints[num + 1].transform.position - controlPoints[num - 1].transform.position);
		}
		else
		{
			m0 = controlPoints[num + 1].transform.position - controlPoints[num].transform.position;
		}
		if (num < controlPoints.Count - 2)
		{
			m1 = 0.5f * (controlPoints[num + 2].transform.position - controlPoints[num].transform.position);
		}
		else
		{
			m1 = controlPoints[num + 1].transform.position - controlPoints[num].transform.position;
		}
		float num2 = Mathf.InverseLerp((float)num / (float)controlPoints.Count, (float)(num + 1) / (float)controlPoints.Count, t);
		return (2f * num2 * num2 * num2 - 3f * num2 * num2 + 1f) * p0 + (num2 * num2 * num2 - 2f * num2 * num2 + num2) * m0 + (-2f * num2 * num2 * num2 + 3f * num2 * num2) * p1 + (num2 * num2 * num2 - num2 * num2) * m1;
	}
}
