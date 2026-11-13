using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GvrAudioSource))]
public class BoidSoundEmitter : MonoBehaviour
{
	public GvrAudioSource mAudioSource;

	private ObjectBoid attachedObject;

	public BoidSoundPool owner;

	public bool isPlaying;

	private void Awake()
	{
		init();
	}

	private void OnEnable()
	{
		init();
	}

	private void OnDisable()
	{
		attachedObject = null;
		isPlaying = false;
	}

	private void Update()
	{
		if (attachedObject != null)
		{
			transform.position = Vector3.Lerp(transform.position, attachedObject.transform.position, 1f - Mathf.Pow(0.5f, Time.deltaTime));
		}
	}

	public void PlaySound(AudioEvent aEvent)
	{
		if (isPlaying)
		{
			SafeDebug.LogWarning("Already playing pool object: " + gameObject.name);
			return;
		}
		mAudioSource.volume = 1f;
		aEvent.spatializedSource = mAudioSource;
		transform.position = attachedObject.transform.position;
		aEvent.Play();
		GlobalCoroutineContainer.AddThread(Deactivate(aEvent.alwaysPlay[0].clip.length), this);
		isPlaying = true;
	}

	public void PlaySound(AudioLoop aEvent)
	{
		if (isPlaying)
		{
			SafeDebug.LogWarning("Already playing pool object: " + gameObject.name);
			return;
		}
		aEvent.gvrSource = mAudioSource;
		if (attachedObject != null)
		{
			transform.position = attachedObject.transform.position;
		}
		aEvent.Play();
		isPlaying = true;
	}

	public void StopSound(float time)
	{
		isPlaying = false;
		GlobalCoroutineContainer.AddThread(FadeOut(time), this);
		attachedObject = null;
		GlobalCoroutineContainer.AddThread(Deactivate(time), this);
	}

	public void SetAttachedObject(ObjectBoid set)
	{
		if (set != null)
		{
			attachedObject = set;
		}
		else
		{
			attachedObject = null;
		}
	}

	public ObjectBoid GetAttachedObject()
	{
		return attachedObject;
	}

	private void init()
	{
		if (mAudioSource == null)
		{
			mAudioSource = GetComponent<GvrAudioSource>();
		}
		mAudioSource.playOnAwake = false;
	}

	private IEnumerator Deactivate(float time)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(time, this);
		attachedObject = null;
		isPlaying = false;
		gameObject.SetActive(value: false);
	}

	private IEnumerator FadeOut(float time)
	{
		float t = 0f;
		while (t <= 1f)
		{
			t += Time.deltaTime / time;
			mAudioSource.volume = Mathf.Lerp(mAudioSource.volume, 0f, t);
			yield return null;
		}
		mAudioSource.Stop();
	}
}
