using UnityEngine;

public class HernandezBarbellController : ObjectBase
{
	[SerializeField]
	private AudioClip liftSoundUp;

	[SerializeField]
	private AudioClip liftSoundDown;

	private GvrAudioSource mAudioSource;

	private ParticleSystem mParticleSystem;

	private bool liftingUp = true;

	private float upstrokeComparisonY;

	private float downstrokeComparisonY;

	private float timeSinceLastStrokeChange;

	private float oldNumLifts;

	private const float minBarbellStrokeDistance = 1.3f;

	private const float barbellInactiveTime = 1f;

	private bool useTopStrokeParticle;

	private bool useBotStrokeParticle;

	private bool useStrokeSound;

	public float numLifts { get; private set; }

	public void SetLiftBehaviour(bool topParticle, bool bottomParticle, bool strokeSound)
	{
		useTopStrokeParticle = topParticle;
		useBotStrokeParticle = bottomParticle;
		useStrokeSound = strokeSound;
	}

	protected override void Start()
	{
		base.Start();
		mAudioSource = GetComponent<GvrAudioSource>();
		mParticleSystem = GetComponentInChildren<ParticleSystem>();
	}

	public bool NewLiftThisFrame()
	{
		return Mathf.Floor(numLifts) > Mathf.Floor(oldNumLifts);
	}

	public void ResetBarbellData()
	{
		upstrokeComparisonY = 0f;
		downstrokeComparisonY = 0f;
		timeSinceLastStrokeChange = 0f;
		liftingUp = true;
		numLifts = 0f;
	}

	private void Update()
	{
		timeSinceLastStrokeChange += Time.deltaTime;
		downstrokeComparisonY = Mathf.Max(downstrokeComparisonY, transform.position.y);
		upstrokeComparisonY = Mathf.Min(upstrokeComparisonY, transform.position.y);
		if (liftingUp)
		{
			if (transform.position.y >= upstrokeComparisonY + 1.3f)
			{
				liftingUp = false;
				numLifts += 0.5f;
				timeSinceLastStrokeChange = 0f;
				downstrokeComparisonY = transform.position.y;
				if (useTopStrokeParticle)
				{
					fireLiftEffect(up: true);
				}
			}
		}
		else if (transform.position.y <= downstrokeComparisonY - 1.3f)
		{
			liftingUp = true;
			numLifts += 0.5f;
			timeSinceLastStrokeChange = 0f;
			upstrokeComparisonY = transform.position.y;
			if (useBotStrokeParticle)
			{
				fireLiftEffect(up: false);
			}
		}
		oldNumLifts = numLifts;
	}

	private void fireLiftEffect(bool up)
	{
		if (useStrokeSound)
		{
			mAudioSource.PlayOneShot((!up) ? liftSoundDown : liftSoundUp);
		}
		mParticleSystem.Emit(10);
	}

	public bool IsNotBeingLifted()
	{
		return timeSinceLastStrokeChange > 1f;
	}
}
