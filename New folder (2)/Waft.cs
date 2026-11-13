using UnityEngine;

public class Waft : MonoBehaviour
{
	[HideInInspector]
	public Vector3 windVector = Vector3.forward;

	[HideInInspector]
	public float timeOffset;

	[HideInInspector]
	public float lifeSpan;

	private float startTime;

	[HideInInspector]
	public Vector3 curviness = new Vector3(0f, 0f, 0f);

	private TrailRenderer windRenderer;

	private void Start()
	{
		startTime = Time.time;
		windRenderer = GetComponent<TrailRenderer>();
	}

	private void Update()
	{
		Vector3 vector = new Vector3(windVector.x + Mathf.Cos(Time.time - startTime + timeOffset) * curviness.x, windVector.y + Mathf.Sin(Time.time - startTime + timeOffset) * curviness.y, windVector.z + Mathf.Sin(Time.time - startTime + timeOffset) * curviness.z);
		transform.Translate(vector * Time.deltaTime);
		if (Time.time - startTime >= lifeSpan)
		{
			windRenderer.time -= 0.02f;
			windRenderer.startWidth -= 0.001f;
			if (windRenderer.startWidth < 0.001f || windRenderer.time < 0.02f)
			{
				Object.Destroy(gameObject);
			}
		}
	}
}
