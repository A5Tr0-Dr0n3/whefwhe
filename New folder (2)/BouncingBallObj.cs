using UnityEngine;

[RequireComponent(typeof(GvrAudioSource))]
public class BouncingBallObj : ObjectBase
{
	[SerializeField]
	private AudioClip[] bounceSfxPool;

	private const float maxVolumeVelocity = 10f;

	private ConstantForce cForce;

	private GvrAudioSource mAudioSource;

	protected override void Start()
	{
		base.Start();
		cForce = GetComponent<ConstantForce>();
		mAudioSource = GetComponent<GvrAudioSource>();
		Vector3 b = Random.rotation * Vector3.forward;
		cForce.force = cForce.force.magnitude * Vector3.Slerp(cForce.force.normalized, b, 0.1f).normalized;
	}

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		if (cForce != null)
		{
			Object.Destroy(cForce);
		}
	}

	protected override void OnCollisionEnter(Collision other)
	{
		float t = Mathf.InverseLerp(0f, 10f, other.relativeVelocity.magnitude);
		if (mAudioSource != null && !mAudioSource.isPlaying)
		{
			mAudioSource.volume = Mathf.Lerp(0f, 1f, t);
			mAudioSource.pitch = Random.Range(0.85f, 1.15f);
			mAudioSource.PlayOneShot(bounceSfxPool[Random.Range(0, bounceSfxPool.Length)]);
		}
	}
}
