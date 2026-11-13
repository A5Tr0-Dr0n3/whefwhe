using UnityEngine;

public class ObjectTumbleweed : ObjectBase
{
	public float maxDistance = 70f;

	private float maxDistanceSqr;

	private Vector3 windForce = new Vector3(0.5f, 0f, 0.5f);

	private float deltaTime;

	public float gustIntervalMin = 0.3f;

	public float gustIntervalMax = 1f;

	private float gustInterval = 0.5f;

	private bool beingHeld;

	protected override void Awake()
	{
		base.Awake();
		rb = GetComponent<Rigidbody>();
		rb.centerOfMass = new Vector3(rb.centerOfMass.x, rb.centerOfMass.y - 0.05f, rb.centerOfMass.z);
		maxDistanceSqr = Mathf.Pow(maxDistance, 2f);
	}

	private void Update()
	{
		if (!beingHeld)
		{
			deltaTime += Time.deltaTime;
			rb.AddForceAtPosition(windForce * Time.deltaTime, transform.position);
			if (deltaTime > gustInterval)
			{
				windForce = -1f * new Vector3(Random.Range(5f, 70f), Random.Range(-50f, 160f - transform.position.y * 10f), Random.Range(5f, 50f));
				gustInterval = Random.Range(gustIntervalMin, gustIntervalMax);
				deltaTime = 0f;
			}
			if (transform.position.sqrMagnitude > maxDistanceSqr)
			{
				transform.position = new Vector3(Random.Range(30f, 50f), 2f, Random.Range(-25f, 50f));
			}
		}
	}

	public override void GrabWith(Manipulator grabber)
	{
		beingHeld = true;
		base.GrabWith(grabber);
	}

	public override void GrabRelease(Manipulator grabber)
	{
		base.GrabRelease(grabber);
		beingHeld = false;
	}
}
