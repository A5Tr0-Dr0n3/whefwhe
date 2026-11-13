using UnityEngine;

public class WindController : MonoBehaviour
{
	public WindWakerWindSpawner myGenerator;

	private Rigidbody propellers;

	private float activeGustForce;

	public Vector3 calmCurvinessAmount = new Vector3(0.5f, 1.5f, 1.5f);

	public Vector3 gustCurvinessAmount = new Vector3(0.5f, 4f, 1f);

	private void Start()
	{
		propellers = gameObject.GetComponent<Rigidbody>();
	}

	private void Update()
	{
		activeGustForce = Mathf.Abs(propellers.angularVelocity.z / 1f);
		myGenerator._windVector = new Vector3(0f, 0f, propellers.angularVelocity.z + 0.2f);
		lerpActiveGustForce();
	}

	private void lerpActiveGustForce()
	{
		myGenerator._windSpeedMin = Mathf.Lerp(1f, 5f, activeGustForce);
		myGenerator._windSpeedMax = Mathf.Lerp(7f, 25f, activeGustForce);
		myGenerator.lifespanMin = Mathf.Lerp(0.75f, 0.75f, activeGustForce);
		myGenerator.lifespanMax = Mathf.Lerp(5f, 3.5f, activeGustForce);
		myGenerator.spawnTimeRangeSecondsMin = Mathf.Lerp(0.2f, 0.01f, activeGustForce);
		myGenerator.spawnTimeRangeSecondsMax = Mathf.Lerp(1f, 0.1f, activeGustForce);
		myGenerator.asynchronicity = Mathf.Lerp(2f, 0.5f, activeGustForce);
		myGenerator.curvinessMax = Vector3.Lerp(calmCurvinessAmount, gustCurvinessAmount, activeGustForce);
	}
}
