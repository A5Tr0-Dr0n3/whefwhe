using UnityEngine;

public class AudioGlitch : MonoBehaviour
{
	public GvrAudioSource audioSource;

	public bool pitch;

	public float pitchMin;

	public float pitchMax;

	public float pitchFreq;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve pitchCurve;

	public bool pause;

	public float pauseLen;

	public float pauseFreq;

	private float pT;

	private void Update()
	{
		if (!(audioSource != null))
		{
			return;
		}
		if (pitch)
		{
			audioSource.pitch = pitchCurve.Evaluate(Time.time % pitchFreq / pitchFreq) * pitchMax + pitchMin;
		}
		if (!pause)
		{
			return;
		}
		pT += Time.deltaTime;
		if (audioSource.isPlaying)
		{
			if (pT > pauseFreq)
			{
				audioSource.Pause();
				pT = 0f;
			}
		}
		else if (pT > pauseLen)
		{
			audioSource.UnPause();
			pT = 0f;
		}
	}
}
