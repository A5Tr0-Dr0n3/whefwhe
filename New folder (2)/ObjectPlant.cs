using UnityEngine;

public class ObjectPlant : ObjectWater
{
	public bool isCarrot;

	public AudioClip pop;

	public float growthSpeed = 0.1f;

	public float growthLength = 0.2f;

	public float growthCount;

	private Vector3 newPos;

	private Vector3 originalScale;

	private Vector3 originalSmallScale;

	private Vector3 originalPos;

	[SerializeField]
	private ParticleSystem dirt;

	[SerializeField]
	private GameObject sapling;

	[HideInInspector]
	public bool dontStopGrow;

	private bool autoGrowStarted;

	private float growLerp;

	protected override void Awake()
	{
		base.Awake();
		rb.isKinematic = true;
		if (!isCarrot)
		{
			originalScale = transform.localScale;
			originalSmallScale = originalScale * 0.15f;
			transform.localScale = originalSmallScale;
		}
		else
		{
			originalPos = transform.position;
			newPos = originalPos;
		}
	}

	private void Update()
	{
		if (dontStopGrow && growthCount > 0f && !isSatisfied)
		{
			WaterPlant(inUpdate: true);
		}
	}

	public override void WaterObject()
	{
		base.WaterObject();
		WaterPlant();
	}

	public void WaterPlant(bool inUpdate = false)
	{
		if (isSatisfied)
		{
			return;
		}
		if (!dirt.isPlaying)
		{
			dirt.Play();
		}
		if ((!inUpdate && !dontStopGrow) || (dontStopGrow && inUpdate && autoGrowStarted) || (dontStopGrow && !inUpdate && !autoGrowStarted))
		{
			growthCount += growthSpeed * Time.deltaTime;
			if (dontStopGrow)
			{
				autoGrowStarted = true;
			}
		}
		if (isCarrot)
		{
			newPos.y = originalPos.y + growthCount;
			transform.position = newPos;
		}
		else
		{
			growLerp = Mathf.Clamp01(growthCount / growthLength);
			transform.localScale = originalSmallScale + originalScale * growLerp;
			sapling.transform.localScale = Vector3.one * (1f - growLerp);
		}
		if (growthCount > growthLength)
		{
			DoneWatering();
		}
	}

	private void DoneWatering()
	{
		if (isCarrot)
		{
			rb.isKinematic = false;
			rb.AddForce(Vector3.up * 25f);
			rb.AddTorque(Random.insideUnitSphere * 1f);
		}
		else
		{
			Object.Destroy(sapling);
		}
		base.Satisfaction();
		AudioSource.PlayClipAtPoint(pop, transform.position);
		if ((bool)GetComponent<ObjectPottable>())
		{
			GetComponent<ObjectPottable>().enabled = true;
			Object.Destroy(this);
		}
	}

	public void TriggerDontStop()
	{
		if (growthCount <= 0f)
		{
			dontStopGrow = true;
		}
	}
}
