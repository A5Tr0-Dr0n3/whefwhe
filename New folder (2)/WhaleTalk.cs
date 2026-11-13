using UnityEngine;

public class WhaleTalk : MonoBehaviour
{
	public AudioSourceLoudness asl;

	public bool particles;

	private ParticleSystem.EmissionModule em;

	private ParticleSystem.MinMaxCurve rate;

	private float defaultRateConstant;

	private float defaultStartSpeed;

	public float particleScalar = 1f;

	public float secondaryThreshold;

	public ParticleSystem speakParticles;

	private ParticleSystem.MainModule psMain;

	public ParticleSystem secondaryParticles;

	private void Awake()
	{
		if (asl == null)
		{
			asl = GetComponentInParent<AudioSourceLoudness>();
		}
		em = speakParticles.emission;
		psMain = speakParticles.main;
		defaultRateConstant = em.rateOverTime.constant;
		defaultStartSpeed = psMain.startSpeed.constant;
	}

	private void Update()
	{
		if (particles)
		{
			if (!(speakParticles == null) && !(secondaryParticles == null))
			{
				if (speakParticles.isStopped)
				{
					speakParticles.Play();
				}
				rate = em.rateOverTime;
				rate.constant = defaultRateConstant * asl.lerpLoudness * 10f * particleScalar;
				em.rateOverTime = rate;
				psMain.startSpeed = defaultStartSpeed * asl.lerpLoudness * 17f * particleScalar;
				if (asl.lerpLoudness > secondaryThreshold && !secondaryParticles.isPlaying)
				{
					secondaryParticles.Play();
				}
			}
		}
		else
		{
			speakParticles.Stop();
			secondaryParticles.Stop();
		}
	}
}
