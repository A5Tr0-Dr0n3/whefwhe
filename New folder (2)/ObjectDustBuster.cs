using TButt;
using UnityEngine;

[RequireComponent(typeof(VVRConsumer))]
public class ObjectDustBuster : ObjectBase
{
	[HideInInspector]
	public VVRConsumer vacConsumer;

	private GvrAudioSource vacuumSound;

	private float blowForce;

	private float targetVolume;

	public bool highPower;

	public AudioEvent EatSFX;

	public ParticleSystem particleIn;

	private Collider[] consumableColliders;

	private VVRConsumable eatObject;

	private float minEatingWorking;

	private float minTimeBetweenEating = 0.1f;

	private float minEatingNoiseWorking;

	private float minTimeBetweenEatingNoise = 0.05f;

	private bool firstGrab = true;

	private const int MAX_BUFFER = 16;

	private int cnt;

	public float _maxSuctionForce = 100f;

	[Tooltip("Should probably start at 0,1 and end at 1,0\n to make attraction near the suctionOrigin highest")]
	[SerializeField]
	[CurveRange]
	private AnimationCurve _suctionForceDistanceCurve;

	[Tooltip("Suction will only occur if the direction towards the center of an edible is >= this\nWatch the white gizmo lines for reference")]
	[Range(-1f, 1f)]
	public float _suctionForceMaxFieldOfRange = 0.5f;

	[Tooltip("Should probably start at 0,0 and end at 1,1 to make attraction most in the direction of the suctionOrigin highest\nNote that this curve operates as the normalized cosine of the angle rather than the normalized angle itself!")]
	[SerializeField]
	[CurveRange]
	private AnimationCurve _shakeRangeCurve;

	private bool isActive;

	private float forceWaver;

	private Vector3 toSuctionOrigin;

	private Vector3 toSuctionOriginNormalized;

	private float force;

	private float directionFraction;

	private float closenessToRange;

	private float distanceLerp;

	private float finalForce;

	private float distance;

	private float shakeforce;

	private float velocityLerp;

	private float massCutoff = 0.5f;

	private float minVacTime = 0.05f;

	private float minVacWorking;

	private VVRConsumable edible;

	protected override void Awake()
	{
		base.Awake();
		vacuumSound = GetComponent<GvrAudioSource>();
		vacConsumer = gameObject.GetComponent<VVRConsumer>();
		SetHighPower(high: false);
	}

	public void OneShotEatSounds(bool isSucking, VVRConsumable consumable)
	{
		EatSFX.Play();
	}

	private void OnEnable()
	{
		vacConsumer.OneShotEvent += OneShotEatSounds;
	}

	private void OnDisable()
	{
		vacConsumer.OneShotEvent -= OneShotEatSounds;
	}

	public override void GrabWith(Manipulator grabber)
	{
		ForceDropByPlayer(playSFX: false);
		base.GrabWith(grabber);
		targetVolume = 0f;
		vacConsumer._nearFoodTrigger.enabled = true;
		blowForce = -1f;
		undroppable = true;
		gameObject.layer = LayerMask.NameToLayer("Vacuum");
		gameObject.GetComponent<BoxCollider>().enabled = false;
		grabber.orientDuringGrab = true;
		float z = 0f - gameObject.GetComponent<MeshFilter>().sharedMesh.bounds.extents.z;
		grabber.heldHighVector = gameObject.GetComponent<MeshFilter>().sharedMesh.bounds.center + new Vector3(0f, 0f, z) / transform.localScale.y;
		vacConsumer.tg = grabber.toolGroup;
	}

	public override void GrabRelease(Manipulator grabber)
	{
		base.GrabRelease(grabber);
		gameObject.layer = LayerMask.NameToLayer("Grabbable");
		gameObject.GetComponent<BoxCollider>().enabled = true;
		isActive = false;
		vacConsumer.StopTransfer();
		if ((bool)vacuumSound)
		{
			vacuumSound.Stop();
		}
		targetVolume = 0f;
		particleIn.Stop();
		vacConsumer.tg = null;
	}

	public void SetHighPower(bool high)
	{
		if (high)
		{
			if (TBInput.GetButton(TBInput.Button.Action1, grabbedBy[0].GrabbingToolGroup.Controller))
			{
				vacConsumer.StartTransfer(suckBool: true, oneShot: true);
			}
			highPower = true;
			vacConsumer.activePower = 2f;
			vacConsumer.activeRange = 3f;
			vacConsumer.restPower = 0.5f;
			vacConsumer.restRange = 1f;
			vacConsumer._mouthRadius = 0.8f;
			massCutoff = 100f;
		}
		else
		{
			if (isGrabbed && TBInput.GetButton(TBInput.Button.Action1, grabbedBy[0].GrabbingToolGroup.Controller))
			{
				vacConsumer.StopTransfer();
			}
			highPower = false;
			vacConsumer.restPower = 0.15f;
			vacConsumer.restRange = 0.5f;
			vacConsumer._mouthRadius = 0.25f;
			massCutoff = 0.5f;
		}
	}

	private void physicsEat()
	{
		if (minEatingWorking > minTimeBetweenEating)
		{
			if (consumableColliders == null)
			{
				consumableColliders = new Collider[16];
			}
			cnt = Physics.OverlapSphereNonAlloc(vacConsumer.mouthTransform.position, vacConsumer._mouthRadius, consumableColliders, EagerMaster.instance.edibleOnlyMask, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < cnt; i++)
			{
				eatObject = consumableColliders[i].GetComponentInParent<VVRConsumable>();
				if ((bool)eatObject && (bool)eatObject.GetComponent<ObjectToast>())
				{
					vacConsumer.removeFromEdibleList(eatObject);
					Object.Destroy(eatObject.gameObject);
					if (minEatingNoiseWorking > minTimeBetweenEatingNoise)
					{
						EatSFX.Play();
						minEatingNoiseWorking = 0f;
					}
				}
			}
		}
		minEatingWorking += Time.deltaTime;
		minEatingNoiseWorking += Time.deltaTime;
	}

	public void Update()
	{
		if (isGrabbed)
		{
			if (TBInput.GetButtonDown(TBInput.Button.Action1, grabbedBy[0].GrabbingToolGroup.Controller) && !firstGrab)
			{
				VacuumParticles();
				if (highPower)
				{
					vacConsumer.StartTransfer(suckBool: true, oneShot: true);
				}
				isActive = true;
				if (!highPower)
				{
					vacConsumer.restPower = 0.8f;
					vacConsumer.restRange = 1.2f;
				}
			}
			else if (TBInput.GetButtonUp(TBInput.Button.Action1, grabbedBy[0].GrabbingToolGroup.Controller))
			{
				firstGrab = false;
				vacConsumer.StopTransfer();
				targetVolume = 0f;
				particleIn.Stop();
				isActive = false;
				if (!highPower)
				{
					vacConsumer.restPower = 0.15f;
					vacConsumer.restRange = 0.5f;
				}
			}
		}
		if (isActive)
		{
			targetVolume = ((!highPower) ? (0.4f * Mathf.PerlinNoise(Time.time * 2f, 0f)) : (0.7f * Mathf.PerlinNoise(Time.time * 2f, 0f)));
			if (!highPower)
			{
				physicsEat();
			}
		}
		vacuumSound.volume = Mathf.Lerp(vacuumSound.volume, targetVolume, 5f * Time.deltaTime);
	}

	private void FixedUpdate()
	{
		if (!isActive)
		{
			return;
		}
		minVacWorking += Time.fixedDeltaTime;
		if (!(minVacWorking > minVacTime))
		{
			return;
		}
		minVacWorking = 0f;
		for (int i = 0; i < vacConsumer._nearbyEdibles.Count; i++)
		{
			edible = vacConsumer._nearbyEdibles[i];
			if (edible != null && edible.rb != null && edible.rb.mass < massCutoff)
			{
				toSuctionOrigin = edible.transform.position - vacConsumer.mouthTransform.position;
				distance = toSuctionOrigin.magnitude;
				distanceLerp = Mathf.Clamp01(distance / 2.5f);
				velocityLerp = 1f - Mathf.Clamp01(distance / 1f);
				toSuctionOriginNormalized = Vector3.Normalize(toSuctionOrigin);
				force = _suctionForceDistanceCurve.Evaluate(distanceLerp);
				shakeforce = _shakeRangeCurve.Evaluate(distanceLerp);
				directionFraction = Vector3.Dot(toSuctionOriginNormalized, vacConsumer.mouthTransform.forward);
				closenessToRange = Mathf.SmoothStep(_suctionForceMaxFieldOfRange, _suctionForceMaxFieldOfRange, directionFraction);
				force *= closenessToRange;
				forceWaver = Mathf.PerlinNoise(Time.time * 2f, 0f);
				finalForce = force * forceWaver * _maxSuctionForce * blowForce;
				edible.rb.AddForce(toSuctionOriginNormalized * finalForce);
				edible.rb.maxAngularVelocity = float.MaxValue;
				edible.rb.angularVelocity = Random.onUnitSphere * shakeforce * 12f;
				edible.rb.velocity = Vector3.Lerp(edible.rb.velocity, toSuctionOriginNormalized * finalForce, velocityLerp);
			}
		}
	}

	private void VacuumParticles()
	{
		if (blowForce < 0f)
		{
			if (!particleIn.isPlaying)
			{
				particleIn.Play();
			}
		}
		else
		{
			particleIn.Stop();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		if ((bool)vacConsumer && (bool)vacConsumer.mouthTransform)
		{
			Gizmos.DrawWireSphere(vacConsumer.mouthTransform.position, vacConsumer._mouthRadius);
			float num = Mathf.Acos(_suctionForceMaxFieldOfRange) * 57.29578f;
			Quaternion quaternion = Quaternion.AngleAxis(0f - num, vacConsumer.mouthTransform.up);
			Quaternion quaternion2 = Quaternion.Inverse(quaternion);
			Quaternion quaternion3 = Quaternion.AngleAxis(0f - num, vacConsumer.mouthTransform.right);
			Quaternion quaternion4 = Quaternion.Inverse(quaternion3);
			Gizmos.DrawRay(vacConsumer.mouthTransform.position, quaternion * vacConsumer.mouthTransform.forward * 2f);
			Gizmos.DrawRay(vacConsumer.mouthTransform.position, quaternion2 * vacConsumer.mouthTransform.forward * 2f);
			Gizmos.DrawRay(vacConsumer.mouthTransform.position, quaternion3 * vacConsumer.mouthTransform.forward * 2f);
			Gizmos.DrawRay(vacConsumer.mouthTransform.position, quaternion4 * vacConsumer.mouthTransform.forward * 2f);
		}
	}
}
