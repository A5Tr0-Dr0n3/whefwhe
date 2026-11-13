using UnityEngine;

public class TumbleweedTalk : MonoBehaviour
{
	public AudioSourceLoudness asl;

	private Vector3 startScale;

	public Transform mouth;

	public float maxScaleVal = 1.25f;

	public float LoudnessMult = 10f;

	public float particleEmitThreshold;

	public float minTimeBetweenParticleEmit;

	public ParticleSystem particleSystem;

	private ParticleSystem.MainModule psMain;

	private float pLoudness;

	private float timeLastParticleEmit;

	private float loudness;

	private void Start()
	{
		if (mouth == null)
		{
			mouth = gameObject.transform;
		}
		if (asl == null)
		{
			asl = GetComponentInParent<AudioSourceLoudness>();
		}
		startScale = mouth.localScale;
		psMain = particleSystem.main;
	}

	private void Update()
	{
		loudness = asl.lerpLoudness * LoudnessMult;
		if (pLoudness < particleEmitThreshold && loudness > particleEmitThreshold && Time.time > timeLastParticleEmit + minTimeBetweenParticleEmit)
		{
			psMain.startColor = new Color(psMain.startColor.color.r, psMain.startColor.color.g, psMain.startColor.color.b, Mathf.Clamp(loudness * 6f, 0.3f, 0.7f));
			particleSystem.Play();
			timeLastParticleEmit = Time.time;
		}
		pLoudness = loudness;
		if (mouth != null)
		{
			mouth.transform.localScale = Vector3.Lerp(startScale, startScale * maxScaleVal, loudness);
		}
	}
}
