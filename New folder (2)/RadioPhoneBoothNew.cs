using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class RadioPhoneBoothNew : SceneBehaviorManager
{
	[SerializeField]
	private ObjectMorphToHeadset phoneHeadsetMorpher;

	public GvrAudioSource phoneRingSound;

	public GvrAudioSource phoneSounds;

	public GvrAudioSource phoneGlitch;

	public AudioClip phonePickUp;

	public AudioClip phoneDialTone;

	public AudioClip phonePutDown;

	public AudioClip phoneMessage;

	public SubtitleRenderer subtitles;

	public SubtitleRenderer ringSubtitles;

	protected override void OnFirstTimeSceneActivated()
	{
		GlobalCoroutineContainer.AddThread(PhoneBoothSequence(), this);
	}

	private IEnumerator PhoneBoothSequence()
	{
		phoneHeadsetMorpher.mObjectBase.ungrabbable = true;
		yield return new WaitForSeconds(10f);
		phoneHeadsetMorpher.mObjectBase.ungrabbable = false;
		phoneRingSound.Play();
		ringSubtitles.SetIsPlaying(s: true);
		while (!ToolManager.PlayerGrabbingObjectBase(phoneHeadsetMorpher.mObjectBase))
		{
			yield return null;
		}
		if (phoneHeadsetMorpher.GetComponent<Rigidbody>() != null)
		{
			phoneHeadsetMorpher.GetComponent<Rigidbody>().isKinematic = false;
		}
		phoneHeadsetMorpher.mObjectBase.undroppable = true;
		phoneRingSound.Stop();
		ringSubtitles.SetIsPlaying(s: false);
		phoneSounds.clip = phonePickUp;
		phoneSounds.Play();
		while (phoneSounds.isPlaying)
		{
			yield return null;
		}
		phoneSounds.Stop();
		phoneSounds.clip = phoneDialTone;
		phoneSounds.loop = true;
		phoneSounds.Play();
		yield return new WaitForSeconds(1.5f);
		SafeCoroutine morphThread = phoneHeadsetMorpher.MorphIntoHeadset(immediatelyForceOn: false);
		phoneGlitch.PlayDelayed(0.25f);
		while (morphThread.IsRunning)
		{
			yield return null;
		}
		phoneGlitch.Stop();
		phoneGlitch.clip = phoneMessage;
		subtitles.SetIsPlaying(s: true);
		phoneGlitch.Play();
		phoneGlitch.volume = 1f;
		while (phoneGlitch.isPlaying)
		{
			yield return null;
		}
		subtitles.SetIsPlaying(s: false);
		yield return new WaitForSeconds(5f);
		phoneHeadsetMorpher.ForceHeadsetOntoFace();
	}
}
