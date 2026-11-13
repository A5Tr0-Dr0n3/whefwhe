using System.Collections;
using UnityEngine;

public class UnionStapler : MonoBehaviour
{
	public enum StaplerLocation
	{
		Stamper,
		InfoDesk,
		Backstage,
		Vacuum,
		Archive,
		Other
	}

	public bool ring = true;

	public ObjectBase stapler;

	public TransformShaker shaker;

	public AudioLoop phoneRing;

	public AudioEvent phonePickup;

	public GvrAudioSource phoneMessage;

	public AudioSourceLoudness sourceMeter;

	public SubtitleRenderer subtitleSource;

	public HeadsetWrangler phoneWrangler;

	public void RingStapler(AudioClip message, StaplerLocation loc, float wait = 0f)
	{
		GlobalCoroutineContainer.AddThread(RingStaplerCoroutine(message, loc, wait), this);
	}

	private IEnumerator RingStaplerCoroutine(AudioClip message, StaplerLocation loc, float wait)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(wait, this);
		if (ring)
		{
			if (subtitleSource != null)
			{
				subtitleSource.SetAudioSource(phoneRing.gvrSource);
				subtitleSource.SetIsPlaying(s: true);
			}
			phoneRing.Play();
		}
		sourceMeter.audioSource = phoneRing.gvrSource;
		TransformShaker stapleShake = stapler.GetComponent<TransformShaker>();
		stapleShake.enabled = true;
		stapleShake.useRotation = true;
		stapleShake.rotation.amount = 15f;
		float baseFreq = 10f;
		stapleShake.rotation.frequency = 0f;
		while (!ToolManager.PlayerGrabbingObjectBase(stapler))
		{
			if (ring && !phoneRing.gvrSource.isPlaying)
			{
				if (subtitleSource != null)
				{
					subtitleSource.SetAudioSource(phoneRing.gvrSource);
					subtitleSource.SetIsPlaying(s: true);
				}
				phoneRing.Play();
			}
			if (sourceMeter.lerpLoudness > 0.01f)
			{
				stapleShake.rotation.frequency = baseFreq;
			}
			else
			{
				stapleShake.rotation.frequency = 0f;
			}
			yield return null;
		}
		stapler.undroppable = true;
		stapleShake.useRotation = false;
		stapleShake.enabled = false;
		phoneRing.Stop();
		if (subtitleSource != null)
		{
			subtitleSource.SetIsPlaying(s: false);
		}
		phonePickup.Play();
		phoneMessage.clip = message;
		phonePickup.Play();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2.1f, this);
		phoneMessage.Play();
		if (subtitleSource != null)
		{
			subtitleSource.SetAudioSource(phoneMessage);
			subtitleSource.SetIsPlaying(s: true);
		}
		while (phoneMessage.isPlaying)
		{
			yield return null;
		}
		if (subtitleSource != null)
		{
			subtitleSource.SetIsPlaying(s: false);
		}
		switch (loc)
		{
		case StaplerLocation.InfoDesk:
			PersistentDataManager.MarkInfoMessageHeard();
			break;
		case StaplerLocation.Stamper:
			PersistentDataManager.MarkStamperMessageHeard();
			break;
		case StaplerLocation.Vacuum:
			PersistentDataManager.MarkVacuumMessageHeard();
			break;
		case StaplerLocation.Archive:
			PersistentDataManager.MarkArchiveMessageHeard();
			break;
		}
		stapler.undroppable = false;
	}

	private void OnDisable()
	{
		if (subtitleSource != null)
		{
			subtitleSource.SetIsPlaying(s: false);
		}
	}
}
