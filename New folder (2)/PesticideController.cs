using UnityEngine;

public class PesticideController : MonoBehaviour
{
	public ParticleSystem waterParticleSystem;

	private ParticleSystem.EmissionModule em;

	private ParticleSystem.MainModule psMain;

	private void Start()
	{
		em = waterParticleSystem.emission;
		psMain = waterParticleSystem.main;
	}

	private void Update()
	{
		float num = Mathf.Clamp(Vector3.Dot(transform.up, Vector3.up), 0f, 1f);
		em.rateOverTime = 50f * num;
		psMain.startSpeed = 1f * num;
	}
}
