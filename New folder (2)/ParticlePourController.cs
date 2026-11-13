using UnityEngine;

public class ParticlePourController : MonoBehaviour
{
	public ParticleSystem flowingParticleSystem;

	public GvrAudioSource waterAudioLoop;

	private ParticleSystem.EmissionModule em;

	private ParticleSystem.MainModule psMain;

	private ParticleSystem.MinMaxCurve rate;

	private float defaultRateConstant;

	private float defaultStartSpeed;

	private Vector3 flowVector;

	private float volScale = 1f;

	private void Start()
	{
		em = flowingParticleSystem.emission;
		psMain = flowingParticleSystem.main;
		defaultRateConstant = em.rateOverTime.constant;
		defaultStartSpeed = psMain.startSpeed.constant;
		flowVector = flowingParticleSystem.transform.forward;
		if (waterAudioLoop != null)
		{
			volScale = waterAudioLoop.volume;
		}
	}

	private void Update()
	{
		if (!(flowingParticleSystem == null))
		{
			if (flowingParticleSystem.isStopped)
			{
				flowingParticleSystem.Play();
			}
			flowVector = flowingParticleSystem.transform.forward;
			float num = Mathf.Clamp(Vector3.Dot(flowVector, -Vector3.up), 0f, 1f);
			if (num < 0.4f)
			{
				num = 0f;
			}
			rate = em.rateOverTime;
			rate.constant = defaultRateConstant * num;
			em.rateOverTime = rate;
			if (waterAudioLoop != null)
			{
				waterAudioLoop.volume = num * volScale;
			}
			psMain.startSpeed = defaultStartSpeed * num;
		}
	}
}
