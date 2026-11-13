using UnityEngine;

public class ObjectBroom : ObjectBase
{
	private ParticleSystem dirt;

	public AudioClip broomSweeping;

	private float velocity;

	[HideInInspector]
	public bool broomCanSweep = true;

	private ParticleSystem.EmissionModule em;

	private Rigidbody r;

	protected override void Awake()
	{
		base.Awake();
		dirt = transform.Find("SoilSweep").GetComponentInChildren<ParticleSystem>();
		em = dirt.emission;
		r = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (dirt.isPlaying)
		{
			dirt.emissionRate = rb.velocity.magnitude * 30f;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (!(other.tag == "Dirt"))
		{
			return;
		}
		if (!dirt.isPlaying)
		{
			dirt.Play();
		}
		velocity = r.velocity.sqrMagnitude;
		if (broomCanSweep)
		{
			other.transform.localScale = Utility.TCVRLerp(other.transform.localScale, Vector3.zero, 0.25f * velocity * 0.25f);
			if (other.GetComponent<Renderer>().bounds.size.magnitude < 0.4f)
			{
				other.gameObject.SetActive(value: false);
				dirt.Stop();
			}
			em.rateOverTime = velocity * 10f;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Dirt")
		{
			dirt.Stop();
		}
	}
}
