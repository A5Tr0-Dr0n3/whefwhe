using UnityEngine;

public class SelfDestructingAudioSource : MonoBehaviour
{
	private AudioSource mSource;

	private bool setup;

	public void PlayPitchBendedClip(AudioClip clipIn, Vector3 positionIn, float volumeIn, float pitchIn)
	{
		setup = true;
		gameObject.name = "Self-Destructing Audio Source";
		transform.position = positionIn;
		mSource = gameObject.AddComponent<AudioSource>();
		mSource.clip = clipIn;
		mSource.volume = volumeIn;
		mSource.pitch = pitchIn;
		mSource.Play();
	}

	private void Update()
	{
		if (setup && !mSource.isPlaying)
		{
			Object.Destroy(gameObject);
		}
	}
}
