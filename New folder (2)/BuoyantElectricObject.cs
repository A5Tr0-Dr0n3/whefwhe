using UnityEngine;

public class BuoyantElectricObject : BuoyantObject
{
	protected GvrAudioSource[] allAudio;

	[SerializeField]
	protected GvrAudioSource brokenNoise;

	[SerializeField]
	protected GameObject friedParticle;

	[SerializeField]
	protected GameObject disableObject;

	public bool broken;

	protected override void Awake()
	{
		base.Awake();
		allAudio = GetComponents<GvrAudioSource>();
	}

	public override void SetWater(SunsetWater set)
	{
		mRigidBody.velocity = Vector3.zero;
		mRigidBody.AddForce(Vector3.up * 5f);
		floating = true;
		water = set;
		mRigidBody.useGravity = false;
		for (int i = 0; i < allAudio.Length; i++)
		{
			allAudio[i].Stop();
		}
		disableObject.SetActive(value: false);
		splashSound.Play();
		if (brokenNoise != null && !brokenNoise.isPlaying)
		{
			brokenNoise.Play();
		}
		if (friedParticle != null && !friedParticle.activeInHierarchy)
		{
			friedParticle.SetActive(value: true);
		}
		broken = true;
	}
}
