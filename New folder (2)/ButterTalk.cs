using UnityEngine;

public class ButterTalk : MonoBehaviour
{
	public AudioSourceLoudness asl;

	public bool particles;

	public bool scaling = true;

	private Vector3 startScale;

	private Vector3 maxScale;

	private Quaternion startBodyRotation;

	private Quaternion startMouthRotatoin;

	private Quaternion targetBodyRotation;

	private Quaternion targetMouthRotation;

	private ParticleSystem.EmissionModule em;

	private ParticleSystem.MinMaxCurve rate;

	private float defaultRateConstant;

	private float defaultStartSpeed;

	public float particleScalar = 1f;

	public float particleSpeedScalar = 15f;

	public GameObject mouth;

	public GameObject body;

	public GameObject pool;

	public ParticleSystem speakParticles;

	private ParticleSystem.MainModule psMain;

	private void Awake()
	{
		startScale = pool.transform.localScale;
		maxScale = startScale * 1.2f;
		if (asl == null)
		{
			asl = GetComponentInParent<AudioSourceLoudness>();
		}
		startBodyRotation = body.transform.localRotation;
		startMouthRotatoin = mouth.transform.localRotation;
		targetBodyRotation = Quaternion.Euler(startBodyRotation.eulerAngles.x - 5f, startBodyRotation.eulerAngles.y, startBodyRotation.eulerAngles.z);
		targetMouthRotation = Quaternion.Euler(startMouthRotatoin.eulerAngles.x + 30f, startMouthRotatoin.eulerAngles.y, startMouthRotatoin.eulerAngles.z);
		speakParticles = GetComponentInChildren<ParticleSystem>();
		if (!(speakParticles == null))
		{
			em = speakParticles.emission;
			psMain = speakParticles.main;
			defaultRateConstant = em.rateOverTime.constant;
			defaultStartSpeed = psMain.startSpeed.constant;
		}
	}

	private void Update()
	{
		if (scaling && pool != null)
		{
			pool.transform.localScale = Vector3.Lerp(startScale, maxScale, asl.lerpLoudness * 2f);
		}
		if (body != null)
		{
			body.transform.localRotation = Quaternion.Lerp(startBodyRotation, targetBodyRotation, asl.lerpLoudness * 2.5f);
		}
		if (mouth != null)
		{
			mouth.transform.localRotation = Quaternion.Lerp(startMouthRotatoin, targetMouthRotation, asl.lerpLoudness * 2.5f);
		}
		if (particles)
		{
			if (!(speakParticles == null))
			{
				if (speakParticles.isStopped)
				{
					speakParticles.Play();
				}
				rate = em.rateOverTime;
				rate.constant = defaultRateConstant * asl.lerpLoudness * 10f * particleScalar;
				em.rateOverTime = rate;
				psMain.startSpeed = defaultStartSpeed * asl.lerpLoudness * particleSpeedScalar * particleScalar;
			}
		}
		else if (speakParticles != null)
		{
			speakParticles.Stop();
		}
	}
}
