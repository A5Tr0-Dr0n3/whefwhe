using TButt;
using UnityEngine;

[RequireComponent(typeof(VVRConsumer))]
public class ObjectLeafBlower : ObjectBase
{
	private VVRConsumer vacConsumer;

	private GvrAudioSource vacuumSound;

	private float targetVolume;

	private float startup;

	private ParticleSystem.ShapeModule shape;

	private ParticleSystem.MainModule psMain;

	public ParticleSystem particleOut;

	public ParticleSystem dustCloud;

	public float ParticleFadeOutDist;

	public AnimationCurve ParticleDistanceCurve;

	public AnimationCurve ParticleAlphaDistanceCurve;

	public float DustNoiseSpeed = 1f;

	public float DustNoiseMin = 0.2f;

	public float ParticleEndSphereRad;

	public float ParticleEndMaxScale;

	private float particleStartMaxScale;

	private float particleStartSphereRad;

	private float particleStartAlpha;

	private Collider[] consumableColliders;

	private VVRConsumable eatObject;

	public float _maxBlowForce = 100f;

	[Tooltip("Should probably start at 0,1 and end at 1,0\n to make attraction near the suctionOrigin highest")]
	[SerializeField]
	[CurveRange]
	private AnimationCurve _suctionForceDistanceCurve;

	[Tooltip("Suction will only occur if the direction towards the center of an edible is >= this\nWatch the white gizmo lines for reference")]
	[Range(-1f, 1f)]
	public float _suctionForceMaxFieldOfRange = 0.5f;

	[Tooltip("Should probably start at 0,0 and end at 1,1 to make attraction most in the direction of the suctionOrigin highest\nNote that this curve operates as the normalized cosine of the angle rather than the normalized angle itself!")]
	private bool isActive;

	private Vector3 toSuctionOrigin;

	private Vector3 toSuctionOriginNormalized;

	private float force;

	private float directionFraction;

	private float closenessToRange;

	private float distanceLerp;

	private float finalForce;

	private float distance;

	public float maxBlowDistance = 3f;

	public float initialActiveFloat;

	public float shake;

	public bool IsBlowing => isActive;

	protected override void Awake()
	{
		base.Awake();
		vacuumSound = GetComponent<GvrAudioSource>();
		vacConsumer = gameObject.GetComponent<VVRConsumer>();
		shape = dustCloud.shape;
		psMain = dustCloud.main;
		particleStartAlpha = psMain.startColor.color.a;
		particleStartSphereRad = shape.radius;
		particleStartMaxScale = psMain.startSize.constant;
	}

	public override void GrabWith(Manipulator grabber)
	{
		ForceDropByPlayer(playSFX: false);
		base.GrabWith(grabber);
		targetVolume = 0f;
		vacConsumer._nearFoodTrigger.enabled = true;
		if ((bool)vacuumSound && !vacuumSound.isPlaying)
		{
			vacuumSound.Play();
		}
		undroppable = true;
		grabber.orientDuringGrab = true;
		grabber.heldHighVector = Vector3.zero;
		gameObject.layer = LayerMask.NameToLayer("Vacuum");
		float z = gameObject.GetComponent<MeshFilter>().sharedMesh.bounds.extents.z;
		grabber.heldHighVector = gameObject.GetComponent<MeshFilter>().sharedMesh.bounds.center + new Vector3(0f, 0f, z) / transform.localScale.y;
		grabber.grabbedTargetRotation = Quaternion.AngleAxis(180f, Vector3.up);
	}

	public override void GrabRelease(Manipulator grabber)
	{
		base.GrabRelease(grabber);
		isActive = false;
		if ((bool)vacuumSound)
		{
			vacuumSound.Stop();
		}
		targetVolume = 0f;
		particleOut.Stop();
	}

	public void Update()
	{
		if (isGrabbed)
		{
			startup += Time.deltaTime;
			if (TBInput.GetButtonDown(TBInput.Button.Action1, grabbedBy[0].GrabbingToolGroup.Controller) && startup > 0.4f)
			{
				if (!particleOut.isPlaying)
				{
					particleOut.Play();
				}
				isActive = true;
				vacConsumer.restPower = 0f;
				vacConsumer.restRange = 1.2f;
				initialActiveFloat = 0f;
			}
			else if (TBInput.GetButtonUp(TBInput.Button.Action1, grabbedBy[0].GrabbingToolGroup.Controller))
			{
				targetVolume = 0f;
				particleOut.Stop();
				isActive = false;
				vacConsumer.restPower = 0f;
				vacConsumer.restRange = 0.5f;
			}
		}
		if (isActive)
		{
			targetVolume = Mathf.PerlinNoise(Time.time * 2f, 0f);
			if (Physics.Raycast(new Ray(vacConsumer.mouthTransform.position, vacConsumer.mouthTransform.forward), out var hitInfo))
			{
				dustCloud.transform.position = hitInfo.point + Vector3.up * 0.25f;
				float num = Vector3.Distance(dustCloud.transform.position, vacConsumer.mouthTransform.position);
				float t = ParticleDistanceCurve.Evaluate(Mathf.Clamp01(num / ParticleFadeOutDist));
				float num2 = 1f - ParticleAlphaDistanceCurve.Evaluate(Mathf.Clamp01(num / ParticleFadeOutDist));
				float num3 = DustNoiseMin + (1f - DustNoiseMin) * Mathf.Pow(1.5f * Mathf.PerlinNoise(Time.time * DustNoiseSpeed, 0f), 3f);
				psMain.startColor = new Color(psMain.startColor.color.r, psMain.startColor.color.g, psMain.startColor.color.b, particleStartAlpha * num2 * num3);
				shape.radius = Mathf.Lerp(particleStartSphereRad, ParticleEndSphereRad, t);
				psMain.startSize = Mathf.Lerp(particleStartMaxScale, ParticleEndMaxScale, t);
				if (isActive && !dustCloud.isPlaying)
				{
					dustCloud.Play();
				}
			}
			else if (dustCloud.isPlaying)
			{
				dustCloud.Stop();
			}
		}
		else if (dustCloud.isPlaying)
		{
			dustCloud.Stop();
		}
		vacuumSound.volume = Mathf.Lerp(vacuumSound.volume, targetVolume, 5f * Time.deltaTime);
	}

	private void FixedUpdate()
	{
		if (!isActive)
		{
			return;
		}
		initialActiveFloat = Mathf.Clamp01(initialActiveFloat + Time.deltaTime * 2f);
		foreach (VVRConsumable nearbyEdible in vacConsumer._nearbyEdibles)
		{
			if (nearbyEdible != null)
			{
				toSuctionOrigin = nearbyEdible.transform.position - vacConsumer.mouthTransform.position;
				distance = toSuctionOrigin.magnitude;
				distanceLerp = Mathf.Clamp01(distance / maxBlowDistance);
				toSuctionOriginNormalized = Vector3.Normalize(toSuctionOrigin);
				force = _suctionForceDistanceCurve.Evaluate(distanceLerp);
				directionFraction = Vector3.Dot(toSuctionOriginNormalized, vacConsumer.mouthTransform.forward);
				closenessToRange = Mathf.SmoothStep(_suctionForceMaxFieldOfRange, 1f, directionFraction);
				force *= closenessToRange;
				finalForce = force * _maxBlowForce * initialActiveFloat;
				if (nearbyEdible.rb != null)
				{
					nearbyEdible.rb.AddForce(toSuctionOriginNormalized * finalForce);
					nearbyEdible.rb.AddForce(Vector3.up * finalForce * 0.7f * Mathf.Clamp01(1f - initialActiveFloat * 3f));
				}
				else if (nearbyEdible.usePhysicsVacuum)
				{
					shake = (Mathf.PerlinNoise(Time.time * 15f, 0f) * 6f - 3f) * initialActiveFloat * force;
					nearbyEdible.transform.localEulerAngles = new Vector3(shake, nearbyEdible.transform.localEulerAngles.y, nearbyEdible.transform.localEulerAngles.z);
				}
			}
			else if (!(nearbyEdible != null))
			{
			}
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
