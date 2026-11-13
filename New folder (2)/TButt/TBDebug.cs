using UnityEngine;

namespace TButt;

public static class TBDebug
{
	public static void DrawBounds(Bounds b)
	{
		DrawBounds(b, Color.white, Time.deltaTime);
	}

	public static void DrawBounds(Bounds b, Color c)
	{
		DrawBounds(b, c, Time.deltaTime);
	}

	public static void DrawBounds(Bounds b, float duration)
	{
		DrawBounds(b, Color.white, duration);
	}

	public static void DrawBounds(Bounds bounds, Color color, float duration)
	{
		Vector3 center = bounds.center;
		Vector3 extents = bounds.extents;
		Vector3 vector = new Vector3(center.x - extents.x, center.y + extents.y, center.z - extents.z);
		Vector3 vector2 = new Vector3(center.x + extents.x, center.y + extents.y, center.z - extents.z);
		Vector3 vector3 = new Vector3(center.x - extents.x, center.y - extents.y, center.z - extents.z);
		Vector3 vector4 = new Vector3(center.x + extents.x, center.y - extents.y, center.z - extents.z);
		Vector3 vector5 = new Vector3(center.x - extents.x, center.y + extents.y, center.z + extents.z);
		Vector3 vector6 = new Vector3(center.x + extents.x, center.y + extents.y, center.z + extents.z);
		Vector3 vector7 = new Vector3(center.x - extents.x, center.y - extents.y, center.z + extents.z);
		Vector3 vector8 = new Vector3(center.x + extents.x, center.y - extents.y, center.z + extents.z);
		Debug.DrawLine(vector, vector2, color, duration);
		Debug.DrawLine(vector2, vector4, color, duration);
		Debug.DrawLine(vector4, vector3, color, duration);
		Debug.DrawLine(vector3, vector, color, duration);
		Debug.DrawLine(vector5, vector6, color, duration);
		Debug.DrawLine(vector6, vector8, color, duration);
		Debug.DrawLine(vector8, vector7, color, duration);
		Debug.DrawLine(vector7, vector5, color, duration);
		Debug.DrawLine(vector, vector5, color, duration);
		Debug.DrawLine(vector2, vector6, color, duration);
		Debug.DrawLine(vector4, vector8, color, duration);
		Debug.DrawLine(vector3, vector7, color, duration);
	}
}
