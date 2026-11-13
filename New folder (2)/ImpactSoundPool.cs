using System.Collections.Generic;
using TButt;
using UnityEngine;

public class ImpactSoundPool : MonoBehaviour
{
	public static ImpactSoundPool instance;

	protected ImpactSoundEmitter grabbedObjectSoundEmitter;

	protected ImpactSoundEmitter lastGrabbedObjectSoundEmitter;

	protected List<ImpactSoundEmitter> pool;

	public bool expandable = true;

	public int poolSize;

	[SerializeField]
	private AudioEvent[] audioEvents;

	[SerializeField]
	private GvrAudioSource baseGVRAudioSource;

	protected void Start()
	{
		Init();
	}

	public void PlayImpactSound(float vol, ObjectBase emitter)
	{
		if (emitter.soundType == ObjectSoundType.none)
		{
			return;
		}
		int soundType = (int)emitter.soundType;
		if (grabbedObjectSoundEmitter != null)
		{
			if (emitter == grabbedObjectSoundEmitter.GetAttachedObject())
			{
				audioEvents[soundType].randVolume = vol * 1.7f;
				for (int i = 0; i < emitter.grabbedBy.Count; i++)
				{
					TBInput.SetRumble(emitter.grabbedBy[i].toolGroup.Controller, vol);
				}
				grabbedObjectSoundEmitter.PlayImpactSound(audioEvents[soundType]);
				return;
			}
		}
		else if (lastGrabbedObjectSoundEmitter != null && emitter == lastGrabbedObjectSoundEmitter.GetAttachedObject())
		{
			audioEvents[soundType].randVolume = vol * 1.3f;
			lastGrabbedObjectSoundEmitter.PlayImpactSound(audioEvents[soundType]);
			return;
		}
		bool flag = false;
		for (int j = 0; j < pool.Count; j++)
		{
			if (pool[j] != null && !pool[j].gameObject.activeInHierarchy)
			{
				pool[j].gameObject.SetActive(value: true);
				pool[j].SetAttachedObject(emitter);
				audioEvents[soundType].randVolume = vol * 0.7f;
				pool[j].PlayImpactSound(audioEvents[soundType]);
				flag = true;
				return;
			}
		}
		if (!flag && expandable)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "Generated Emitter";
			ImpactSoundEmitter impactSoundEmitter = gameObject.AddComponent<ImpactSoundEmitter>();
			gameObject.transform.SetParent(transform);
			impactSoundEmitter.SetAttachedObject(emitter);
			pool.Add(impactSoundEmitter);
			gameObject.gameObject.SetActive(value: true);
			impactSoundEmitter.PlayImpactSound(audioEvents[soundType]);
		}
		else if (flag)
		{
		}
	}

	public void SetGrabbedObject(ObjectBase set)
	{
		if (grabbedObjectSoundEmitter != null)
		{
			ReleaseGrabbedObject();
		}
		if (pool.Count > 0)
		{
			for (int i = 0; i < pool.Count; i++)
			{
				if (pool[i] != null && !pool[i].gameObject.activeSelf)
				{
					pool[i].SetAttachedObject(set);
					grabbedObjectSoundEmitter = pool[i];
					pool.Remove(pool[i]);
					grabbedObjectSoundEmitter.gameObject.SetActive(value: true);
					break;
				}
			}
		}
		else if (expandable)
		{
			GameObject gameObject = new GameObject();
			ImpactSoundEmitter impactSoundEmitter = gameObject.AddComponent<ImpactSoundEmitter>();
			gameObject.name = "Generated Emitter";
			grabbedObjectSoundEmitter = impactSoundEmitter;
			impactSoundEmitter.owner = this;
			impactSoundEmitter.owner = this;
			grabbedObjectSoundEmitter.gameObject.SetActive(value: true);
		}
		else if (pool[0] != null)
		{
			pool[0].SetAttachedObject(set);
			grabbedObjectSoundEmitter = pool[0];
			pool.Remove(pool[0]);
			grabbedObjectSoundEmitter.gameObject.SetActive(value: true);
		}
		else
		{
			SafeDebug.LogWarning("The pool was fully used. forcibly reserving an emitter for the grabbed object");
		}
	}

	public void ReleaseGrabbedObject()
	{
		if (lastGrabbedObjectSoundEmitter != null)
		{
			lastGrabbedObjectSoundEmitter.gameObject.SetActive(value: false);
		}
		pool.Add(lastGrabbedObjectSoundEmitter);
		if (lastGrabbedObjectSoundEmitter != null)
		{
			lastGrabbedObjectSoundEmitter.SetAttachedObject(null);
		}
		lastGrabbedObjectSoundEmitter = grabbedObjectSoundEmitter;
		grabbedObjectSoundEmitter = null;
	}

	public ImpactSoundEmitter GetLastGrabbedEmitter()
	{
		return lastGrabbedObjectSoundEmitter;
	}

	public ImpactSoundEmitter GetGrabbedEmitter()
	{
		return grabbedObjectSoundEmitter;
	}

	protected void Init()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			SafeDebug.LogWarning("Impact Sound Pool already exists. Deleting myself");
			Object.Destroy(base.gameObject);
		}
		if (baseGVRAudioSource == null)
		{
			baseGVRAudioSource = GetComponent<GvrAudioSource>();
		}
		pool = new List<ImpactSoundEmitter>();
		for (int i = 0; i <= poolSize; i++)
		{
			GameObject gameObject = new GameObject();
			GvrAudioSource newSource = gameObject.AddComponent<GvrAudioSource>();
			if (newSource == null)
			{
				SafeDebug.LogError("WTF");
			}
			CopyGVRAudioSource(ref newSource, baseGVRAudioSource);
			ImpactSoundEmitter impactSoundEmitter = gameObject.AddComponent<ImpactSoundEmitter>();
			gameObject.transform.SetParent(transform);
			gameObject.gameObject.SetActive(value: false);
			gameObject.name = "ImpactSoundEmitter" + i;
			impactSoundEmitter.owner = this;
			pool.Add(impactSoundEmitter);
		}
		if (audioEvents == null)
		{
			audioEvents = GetComponentsInChildren<AudioEvent>();
			SafeDebug.LogWarning("Audio Events array wasn't assigned in editor. You may have issues, but its likely fine.");
		}
	}

	private void CopyGVRAudioSource(ref GvrAudioSource newSource, GvrAudioSource originalSource)
	{
		if (originalSource == null)
		{
			SafeDebug.LogError("Original source broken");
		}
		if (newSource == null)
		{
			SafeDebug.LogError("New source broken");
		}
		newSource.volume = originalSource.volume;
		newSource.pitch = 1f;
		newSource.mute = originalSource.mute;
		newSource.playOnAwake = originalSource.playOnAwake;
		newSource.bypassRoomEffects = originalSource.bypassRoomEffects;
		newSource.gainDb = originalSource.gainDb;
		newSource.dopplerLevel = originalSource.dopplerLevel;
		newSource.spread = originalSource.spread;
		newSource.rolloffMode = originalSource.rolloffMode;
		newSource.minDistance = originalSource.minDistance;
		newSource.maxDistance = originalSource.maxDistance;
		newSource.directivityAlpha = originalSource.directivityAlpha;
		newSource.directivitySharpness = originalSource.directivitySharpness;
		newSource.listenerDirectivityAlpha = originalSource.listenerDirectivityAlpha;
		newSource.listenerDirectivitySharpness = originalSource.listenerDirectivitySharpness;
		newSource.priority = originalSource.priority;
	}
}
