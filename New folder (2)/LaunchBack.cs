using UnityEngine;

[RequireComponent(typeof(ObjectBase))]
[RequireComponent(typeof(Rigidbody))]
public class LaunchBack : MonoBehaviour
{
	public Transform target;

	public float launchPower = 1f;

	public float jumpHeightCutoff = 0.3f;

	[Header("Garden Non-Progression Fix")]
	public float extremeHeightCutoff = 10f;

	public bool overrideCutoffWithTimer;

	public float overrideCutoffMinTime = 2f;

	public float overrideCutoffMaxTime = 3.5f;

	private float overrideCutoffCountdown;

	private Rigidbody rb;

	private ObjectBase ob;

	private float lateralSpeed = 45f;

	private float verticalSpeedMin = 65f;

	private float verticalSpeedMax = 95f;

	private float distanceMod = 1f;

	private float distance;

	private Vector3 dirToTarget = Vector3.zero;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		ob = GetComponent<ObjectBase>();
		if (target == null)
		{
			target = Camera.main.transform;
		}
	}

	private void FixedUpdate()
	{
		if (!overrideCutoffWithTimer)
		{
			if (!ob.isGrabbed && transform.position.y < jumpHeightCutoff && rb.velocity.magnitude < 0.1f)
			{
				GetComponent<AudioEvent>().Play();
				if (Vector3.SqrMagnitude(transform.position - target.transform.position) < 8f)
				{
					Hop();
				}
				else
				{
					Launch();
				}
			}
		}
		else if (transform.position.y >= extremeHeightCutoff)
		{
			if (ob.isGrabbed)
			{
				return;
			}
			overrideCutoffCountdown -= Time.deltaTime;
			if (overrideCutoffCountdown <= 0f && rb.velocity.magnitude < 0.1f)
			{
				overrideCutoffCountdown = Random.Range(overrideCutoffMinTime, overrideCutoffMaxTime);
				GetComponent<AudioEvent>().Play();
				if (Vector3.SqrMagnitude(transform.position - target.transform.position) < 8f)
				{
					Hop();
				}
				else
				{
					Launch();
				}
			}
		}
		else if (overrideCutoffCountdown <= 0f)
		{
			if (!ob.isGrabbed && transform.position.y < jumpHeightCutoff && rb.velocity.magnitude < 0.1f)
			{
				GetComponent<AudioEvent>().Play();
				if (Vector3.SqrMagnitude(transform.position - target.transform.position) < 8f)
				{
					Hop();
				}
				else
				{
					Launch();
				}
			}
		}
		else
		{
			overrideCutoffCountdown -= Time.deltaTime;
		}
	}

	private void Hop()
	{
		GetComponent<Rigidbody>().AddForce(Vector3.up * Random.Range(verticalSpeedMin, verticalSpeedMax) * launchPower * 0.5f);
	}

	private void Launch()
	{
		distance = Vector3.Distance(target.position, transform.position);
		if (distance > 30f)
		{
			distanceMod = distance / 10f;
		}
		else
		{
			distanceMod = 1f;
		}
		dirToTarget = (target.position - Vector3.forward - transform.position).normalized;
		GetComponent<Rigidbody>().AddForce((Vector3.up * Random.Range(verticalSpeedMin, verticalSpeedMax) * launchPower + dirToTarget * lateralSpeed * launchPower * distanceMod) / 60f, ForceMode.Impulse);
	}
}
