using UnityEngine;

public class ARSwarmPoint : MonoBehaviour
{
	[HideInInspector]
	public bool showGizmo = true;

	[HideInInspector]
	public float gizmoSize = 0.1f;

	[HideInInspector]
	public Color gizmoColor = new Color(1f, 0f, 0f, 0.5f);

	private void OnDrawGizmos()
	{
		if (showGizmo)
		{
			Gizmos.color = gizmoColor;
			Gizmos.DrawSphere(transform.position, gizmoSize);
		}
	}
}
