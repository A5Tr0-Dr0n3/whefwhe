using System.Collections;
using UnityEngine;

public class AmbientAudioStagger : MonoBehaviour
{
	[SerializeField]
	private GvrAudioSource[] _sources;

	[SerializeField]
	private float initWait = 0.1f;

	[SerializeField]
	private float iterWait = 0.25f;

	private float volumeChangeDurr = 0.7f;

	private void Start()
	{
		GlobalCoroutineContainer.AddThread(AudioStaggerCoroutine(), this);
	}

	private IEnumerator AudioStaggerCoroutine()
	{
		yield return new WaitForSeconds(initWait);
		for (int i = 0; i < _sources.Length; i++)
		{
			float setVol = _sources[i].volume;
			_sources[i].volume = 0f;
			_sources[i].Play();
			GlobalCoroutineContainer.AddThread(ChangeSourceVolume(_sources[i], setVol, volumeChangeDurr), this);
			yield return new WaitForSeconds(iterWait);
		}
	}

	private IEnumerator ChangeSourceVolume(GvrAudioSource source, float newVol, float durr)
	{
		float t = 0f;
		AnimationCurve volumeChangeCurve = AnimationCurve.EaseInOut(0f, 0f, durr, newVol);
		while (t < volumeChangeDurr)
		{
			t += Time.deltaTime;
			source.volume = volumeChangeCurve.Evaluate(t);
			yield return null;
		}
		source.volume = newVol;
	}
}
