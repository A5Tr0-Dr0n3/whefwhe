using UnityEngine;

public class WaterBoiling : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem boilingWater;

	[SerializeField]
	private GameObject boilingPoint;

	[SerializeField]
	private ObjectSun sunObj;

	private float startY;

	private Transform sunPos;

	public bool boiling;

	[SerializeField]
	private ParticleSystem.ShapeModule mShape;

	public float radius;

	public float distanceFromBottom;

	private float d = 17f;

	private float o = 2f;

	private void Start()
	{
		startY = transform.position.y;
		mShape = boilingWater.shape;
	}

	private void Update()
	{
		if (boiling)
		{
			Vector3 position = sunPos.position;
			position = new Vector3(position.x, startY, position.z);
			boilingPoint.transform.position = position;
			radius = Vector3.Distance(sunObj.middle.position, sunObj.rightEdge.position) + o;
			distanceFromBottom = Mathf.Abs(sunObj.middle.position.y - transform.position.y);
			radius = Mathf.Lerp(radius, 0f, Mathf.Clamp01(distanceFromBottom / d));
			mShape.radius = radius;
		}
	}

	protected void OnTriggerEnter(Collider other)
	{
		ObjectSun component = other.GetComponent<ObjectSun>();
		if (component != null)
		{
			sunPos = component.transform;
			boiling = true;
			boilingWater.gameObject.SetActive(value: true);
			if (!boilingWater.isPlaying)
			{
				boilingWater.Play();
			}
		}
	}

	protected void OnTriggerExit(Collider other)
	{
		ObjectSun component = other.GetComponent<ObjectSun>();
		if (component != null)
		{
			boiling = false;
			sunPos = null;
			boilingWater.gameObject.SetActive(value: false);
			if (boilingWater.isPlaying)
			{
				boilingWater.Stop();
			}
		}
	}
}
