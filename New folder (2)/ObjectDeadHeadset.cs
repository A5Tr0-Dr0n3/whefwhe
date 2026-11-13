using System.Collections;
using UnityEngine;

public class ObjectDeadHeadset : ObjectBase
{
	public float minLaunchTime;

	public float maxLaunchTime;

	public float minLaunchForce;

	public float maxLaunchForce;

	public float minLaunchDuration;

	public float maxLaunchDuration;

	private float launchForce;

	private float launchDuration;

	private Vector3 direction;

	private Rigidbody r;

	private Quaternion oldRot;

	public Renderer eyeRenderer;

	protected override void Start()
	{
		r = GetComponent<Rigidbody>();
		StartCoroutine(HeadsetFlop());
		eyeRenderer.sharedMaterial = eyeRenderer.material;
	}

	private IEnumerator HeadsetFlop()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(minLaunchTime, maxLaunchTime));
			launchForce = Random.Range(minLaunchForce, maxLaunchForce);
			launchDuration = Random.Range(minLaunchDuration, maxLaunchDuration);
			direction = (Random.onUnitSphere - transform.forward * 0.5f + Vector3.up * 0.25f).normalized;
			while (launchDuration > 0f)
			{
				launchDuration -= Time.deltaTime;
				r.AddForce(direction * launchForce * Time.deltaTime);
				yield return null;
			}
		}
	}

	private void Update()
	{
		oldRot = transform.rotation;
		transform.LookAt(transform.position + -r.velocity);
		transform.rotation = Quaternion.Lerp(oldRot, transform.rotation, 2f * Time.deltaTime);
		eyeRenderer.sharedMaterial.color = Color.white * r.velocity.magnitude * 5f * Random.Range(0.25f, 1f);
	}
}
