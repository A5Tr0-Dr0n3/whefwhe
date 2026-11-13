using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GvrAudioSource))]
public class ImpactSoundEmitter : MonoBehaviour
{
	private GvrAudioSource mAudioSource;

	private ObjectBase attachedObject;

	public ImpactSoundPool owner;

	protected void Start()
	{
		init();
	}

	private void OnDisable()
	{
	}

	private void OnEnable()
	{
		init();
	}

	private IEnumerator Deactivate(float time)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(time, this);
		gameObject.SetActive(value: false);
	}

	public void PlayImpactSound(AudioEvent aEvent)
	{
		mAudioSource.pitch = Random.Range(0.995f, 1.005f);
		aEvent.spatializedSource = mAudioSource;
		transform.position = attachedObject.transform.position;
		float num = aEvent.Play();
		if (owner.GetGrabbedEmitter() != this && owner.GetLastGrabbedEmitter() != this)
		{
			if (num > 0f)
			{
				GlobalCoroutineContainer.AddThread(Deactivate(num), this);
			}
			else
			{
				GlobalCoroutineContainer.AddThread(Deactivate(0.15f), this);
			}
		}
	}

	public void SetAttachedObject(ObjectBase set)
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

	public ObjectBase GetAttachedObject()
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
}
