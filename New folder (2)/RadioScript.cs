using System.Collections;
using UnityEngine;

public class RadioScript : SceneBehaviorManager
{
	public ObjectBlackHole vacuum;

	public GameObject nestingDoll;

	public AudioClip lightsOn;

	public AudioLoop radio;

	public SubtitleRenderer subtitles;

	private bool dollOff = true;

	private Vector3 initialDollScale;

	private float dollLerp;

	protected override void OnFirstTimeSceneActivated()
	{
		EagerHeadsetManager.SetCanRemoveHeadset(canRemove: false);
		initialDollScale = nestingDoll.transform.localScale;
	}

	private void Update()
	{
		if (vacuum.isGrabbed && dollOff)
		{
			dollOff = false;
			GlobalCoroutineContainer.AddThread(TurnOnDoll(), this);
		}
	}

	private IEnumerator TurnOnDoll()
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(6f, this);
		AudioSource.PlayClipAtPoint(lightsOn, transform.position, 1f);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.35f, this);
		nestingDoll.transform.localScale = Vector3.zero;
		nestingDoll.SetActive(value: true);
		dollLerp = 0f;
		while (dollLerp < 1f)
		{
			dollLerp = Mathf.Clamp01(dollLerp + Time.deltaTime * 4f);
			nestingDoll.transform.localScale = Vector3.Lerp(Vector3.zero, initialDollScale, dollLerp);
			yield return null;
		}
		subtitles.gameObject.SetActive(value: true);
		radio.Play();
		subtitles.SetIsPlaying(s: true);
	}
}
