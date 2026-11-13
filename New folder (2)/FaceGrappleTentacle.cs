using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FaceGrappleTentacle : MonoBehaviour
{
	[SerializeField]
	private int tentaclePointResolution = 12;

	private LineRenderer mLineRenderer;

	private void Awake()
	{
		mLineRenderer = GetComponent<LineRenderer>();
	}

	private void generatePhysicsJoints()
	{
	}

	public void UpdateTentaclePositions(Vector3 targetPosition, float progressT)
	{
		mLineRenderer.positionCount = tentaclePointResolution;
		for (int i = 0; i < tentaclePointResolution; i++)
		{
			float t = progressT * ((float)i / (float)(tentaclePointResolution - 1));
			Vector3 position = Vector3.Lerp(transform.position, targetPosition, t);
			mLineRenderer.SetPosition(i, position);
		}
	}
}
