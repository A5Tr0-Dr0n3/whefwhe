using System;
using UnityEngine;

[Obsolete("GvrAudioListener is deprecated. Please upgrade to Resonance Audio (https://developers.google.com/resonance-audio/migrate).")]
[AddComponentMenu("GoogleVR/Audio/Deprecated/GvrAudioListener")]
public class GvrAudioListener : MonoBehaviour
{
	public float globalGainDb;

	public LayerMask occlusionMask = -1;

	[SerializeField]
	private GvrAudio.Quality quality = GvrAudio.Quality.High;

	private void Awake()
	{
		GvrAudio.Initialize(this, quality);
	}

	private void OnEnable()
	{
		GvrAudio.UpdateAudioListener(globalGainDb, occlusionMask);
	}

	private void OnDestroy()
	{
		GvrAudio.Shutdown(this);
	}

	private void Update()
	{
		GvrAudio.UpdateAudioListener(globalGainDb, occlusionMask);
	}
}
