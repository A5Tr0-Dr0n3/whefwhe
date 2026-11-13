using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("StixGames/Displacement Trail Renderer")]
public class DisplacementTrailRenderer : MonoBehaviour
{
	public float minVertexDistance = 0.5f;

	public float lifetime = 5f;

	public AnimationCurve width = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	public AnimationCurve strength = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

	public Material material;

	[HideInInspector]
	public int layer;

	private Vector3 upDir = Vector3.up;

	private List<TrailPoint> points = new List<TrailPoint>();

	private Mesh mesh;

	private List<Vector3> vertices = new List<Vector3>();

	private List<int> triangles = new List<int>();

	private List<Vector2> uvs = new List<Vector2>();

	private List<Vector3> normals = new List<Vector3>();

	private List<Vector4> tangents = new List<Vector4>();

	private List<Color> colors = new List<Color>();

	private void Start()
	{
		mesh = new Mesh();
	}

	private void Update()
	{
		while (points.Count > 0 && Time.time - points[0].creationTime > lifetime)
		{
			points.RemoveAt(0);
		}
		Vector3 position = transform.position;
		bool flag = false;
		if (points.Count == 0 || Vector3.Distance(points[points.Count - 1].pos, position) > minVertexDistance)
		{
			points.Add(new TrailPoint(position, Time.time));
			flag = true;
		}
		List<TrailPoint> list = new List<TrailPoint>(points);
		if (!flag)
		{
			list.Add(new TrailPoint(position, Time.time));
		}
		if (list.Count >= 2)
		{
			UpdateMesh(list);
			Graphics.DrawMesh(mesh, Matrix4x4.identity, material, layer);
		}
	}

	private void UpdateMesh(List<TrailPoint> renderPoints)
	{
		mesh.Clear();
		if (renderPoints.Count < 2)
		{
			return;
		}
		vertices.Clear();
		triangles.Clear();
		uvs.Clear();
		normals.Clear();
		tangents.Clear();
		colors.Clear();
		float num = 1f / (float)(renderPoints.Count - 1);
		for (int i = 0; i < renderPoints.Count; i++)
		{
			TrailPoint point = renderPoints[i];
			if (i == 0)
			{
				AddPoint(point, renderPoints[i + 1].pos - point.pos, 0f);
				continue;
			}
			TrailPoint trailPoint = renderPoints[i - 1];
			if (i == renderPoints.Count - 1)
			{
				AddPoint(point, point.pos - trailPoint.pos, 1f);
				break;
			}
			TrailPoint trailPoint2 = renderPoints[i + 1];
			AddPoint(point, trailPoint2.pos - trailPoint.pos, (float)i * num);
		}
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.normals = normals.ToArray();
		mesh.tangents = tangents.ToArray();
		mesh.colors = colors.ToArray();
	}

	private void AddPoint(TrailPoint point, Vector3 direction, float uv)
	{
		float time = (Time.time - point.creationTime) / lifetime;
		float num = width.Evaluate(time);
		float num2 = strength.Evaluate(time);
		Color item = new Color(num2, num2, num2, num2);
		direction.Normalize();
		Vector3 pos = point.pos;
		Vector3 vector = Vector3.Cross(upDir, direction);
		vertices.Add(pos - vector * num);
		vertices.Add(pos + vector * num);
		uvs.Add(new Vector2(0f, uv));
		uvs.Add(new Vector2(1f, uv));
		normals.Add(upDir);
		normals.Add(upDir);
		tangents.Add(new Vector4(direction.x, direction.y, direction.z, 1f));
		tangents.Add(new Vector4(direction.x, direction.y, direction.z, 1f));
		colors.Add(item);
		colors.Add(item);
		int num3 = vertices.Count - 1;
		if (num3 >= 3)
		{
			triangles.Add(num3 - 1);
			triangles.Add(num3);
			triangles.Add(num3 - 2);
			triangles.Add(num3 - 2);
			triangles.Add(num3 - 3);
			triangles.Add(num3 - 1);
		}
	}
}
