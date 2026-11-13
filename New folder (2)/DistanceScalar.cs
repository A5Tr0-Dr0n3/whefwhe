using UnityEngine;

public class DistanceScalar : MonoBehaviour
{
	public Transform referencePoint;

	public float minScale;

	public float maxScale;

	public float minDistance;

	public float maxDistance;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve scalar;

	private float currDistance;

	private float currScale;

	private void Start()
	{
		if (referencePoint == null)
		{
			referencePoint = EagerMaster.instance.mainCamera.transform;
		}
	}

	private void Update()
	{
		if (referencePoint == null)
		{
			referencePoint = EagerMaster.instance.mainCamera.transform;
		}
		currDistance = Mathf.Clamp(Vector3.Distance(transform.position, referencePoint.position), minDistance, maxDistance);
		currScale = MapValue(scalar.Evaluate(MapValue(currDistance, minDistance, maxDistance, 1f, 0f)), 0f, 1f, minScale, maxScale);
		transform.localScale = new Vector3(currScale, currScale, currScale);
	}

	public float MapValue(float oldVal, float oldMin, float oldMax, float newMin, float newMax)
	{
		return (oldVal - oldMin) * (newMax - newMin) / (oldMax - oldMin) + newMin;
	}
}
