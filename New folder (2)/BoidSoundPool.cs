using System.Collections.Generic;
using UnityEngine;

public class BoidSoundPool : MonoBehaviour
{
	public static BoidSoundPool instance;

	[Header("Audio Events")]
	[SerializeField]
	private AudioLoop[] boidMusic;

	[SerializeField]
	private AudioLoop boidFlying;

	[SerializeField]
	private AudioLoop boidConsuming;

	[SerializeField]
	private AudioEvent boidOn;

	[SerializeField]
	private AudioEvent boidOff;

	[Header("Settings")]
	[SerializeField]
	private GvrAudioSource audioSettingsSource;

	[SerializeField]
	private GvrAudioSource musicSettingsSource;

	[SerializeField]
	private int poolSize = 15;

	[SerializeField]
	private int maxNearbyBoids = 6;

	[SerializeField]
	private float distanceBuffer = 3f;

	[Header("Debug")]
	public List<BoidController> boidControllers;

	protected List<BoidSoundEmitter> pool;

	public List<BoidSoundObject> nearbyBoids;

	public List<BoidSoundObject> flyingBoids;

	[HideInInspector]
	public List<BoidSoundEmitter> musicEmitter;

	private int musicState;

	private void Awake()
	{
		Init();
	}

	private void Update()
	{
		deleteNullBoids();
		sortAndUpdateDistance();
		determineActiveBoids();
	}

	public void PlayBoidSound(BoidSoundType type, ObjectBoid emitter)
	{
		bool flag = false;
		if (type == BoidSoundType.music)
		{
			if (nearbyBoids.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < musicEmitter.Count; i++)
			{
				if (!musicEmitter[i].isPlaying)
				{
					musicEmitter[i].gameObject.SetActive(value: true);
					musicEmitter[i].SetAttachedObject(nearbyBoids[0].boid);
					musicEmitter[i].PlaySound(boidMusic[i]);
					if (i > musicState)
					{
						musicEmitter[i].mAudioSource.volume = 0f;
					}
				}
			}
			for (int j = 0; j <= musicState; j++)
			{
				musicEmitter[j].mAudioSource.volume = boidMusic[j].loop.volume;
			}
			return;
		}
		for (int k = 0; k < pool.Count; k++)
		{
			if (!pool[k].gameObject.activeInHierarchy)
			{
				pool[k].gameObject.SetActive(value: true);
				pool[k].SetAttachedObject(emitter);
				switch (type)
				{
				case BoidSoundType.flying:
					pool[k].PlaySound(boidFlying);
					break;
				case BoidSoundType.consuming:
					pool[k].PlaySound(boidConsuming);
					break;
				case BoidSoundType.on:
					pool[k].PlaySound(boidOn);
					break;
				case BoidSoundType.off:
					pool[k].PlaySound(boidOff);
					break;
				}
				flag = true;
				return;
			}
		}
		if (flag)
		{
		}
	}

	public void StopBoidSound(ObjectBoid emitter, float time)
	{
		BoidSoundEmitter attachedEmitter = getAttachedEmitter(emitter);
		if (attachedEmitter != null)
		{
			attachedEmitter.StopSound(time);
		}
	}

	public void StopBoidMusic(float time)
	{
		for (int i = 0; i < boidControllers.Count; i++)
		{
			for (int j = 0; j < boidControllers[i].boidList.Count; j++)
			{
				if (boidControllers[i].boidList[j].currentBehavior == ObjectBoid.behaviorMode.search || boidControllers[i].boidList[j].currentBehavior == ObjectBoid.behaviorMode.chase || boidControllers[i].boidList[j].currentAction == ObjectBoid.actionMode.hugging)
				{
					return;
				}
			}
		}
		if (musicEmitter[0].isPlaying || musicEmitter[2].isPlaying || musicEmitter[2].isPlaying)
		{
			for (int k = 0; k < 3; k++)
			{
				musicEmitter[k].StopSound(time);
			}
			musicState = 0;
		}
	}

	private void determineActiveBoids()
	{
		bool flag = false;
		if (boidControllers.Count > 0)
		{
			for (int i = 0; i < boidControllers.Count; i++)
			{
				for (int j = 0; j < boidControllers[i].boidList.Count; j++)
				{
					ObjectBoid objectBoid = boidControllers[i].boidList[j];
					flag = objectBoid.currentAction == ObjectBoid.actionMode.hugging || objectBoid.currentBehavior == ObjectBoid.behaviorMode.chase || objectBoid.currentBehavior == ObjectBoid.behaviorMode.search;
					float num = SqDistance(EagerMaster.instance.mainCamera.transform.position, objectBoid.transform.position);
					if (nearbyBoidsContains(objectBoid))
					{
						continue;
					}
					if (nearbyBoids.Count < maxNearbyBoids)
					{
						BoidSoundObject item = new BoidSoundObject(num, objectBoid);
						nearbyBoids.Add(item);
						continue;
					}
					int num2 = -1;
					for (int k = 0; k < nearbyBoids.Count; k++)
					{
						float distance = nearbyBoids[k].distance;
						if (num + distanceBuffer < distance)
						{
							if (num2 == -1)
							{
								num2 = k;
							}
							else if (distance > nearbyBoids[num2].distance)
							{
								num2 = k;
							}
						}
					}
					if (num2 != -1)
					{
						nearbyBoids[num2].playingFlying = false;
						nearbyBoids.RemoveAt(num2);
						BoidSoundObject item = new BoidSoundObject(num, objectBoid);
						nearbyBoids.Add(item);
					}
				}
			}
		}
		if (!flag)
		{
			StopBoidMusic(0.5f);
		}
	}

	private void sortAndUpdateDistance()
	{
		for (int i = 0; i < nearbyBoids.Count - 1; i++)
		{
			nearbyBoids[i].distance = SqDistance(EagerMaster.instance.mainCamera.transform.position, nearbyBoids[i].boid.transform.position);
			nearbyBoids[i + 1].distance = SqDistance(EagerMaster.instance.mainCamera.transform.position, nearbyBoids[i + 1].boid.transform.position);
			if (nearbyBoids[i].distance > nearbyBoids[i + 1].distance)
			{
				BoidSoundObject value = nearbyBoids[i];
				nearbyBoids[i] = nearbyBoids[i + 1];
				nearbyBoids[i + 1] = value;
			}
		}
		if (nearbyBoids.Count <= 0)
		{
			return;
		}
		for (int j = 0; j < musicEmitter.Count; j++)
		{
			musicEmitter[j].SetAttachedObject(nearbyBoids[0].boid);
		}
		if (boidControllers.Count == 0)
		{
			for (int k = 0; k < musicEmitter.Count; k++)
			{
				StopBoidMusic(0.5f);
				SetMusicState(0);
			}
		}
	}

	private ObjectBoid findNearestBoid(Transform emitter)
	{
		ObjectBoid boid = nearbyBoids[0].boid;
		float num = 1000f;
		for (int i = 0; i < nearbyBoids.Count; i++)
		{
			float num2 = SqDistance(nearbyBoids[i].boid.transform.position, emitter.position);
			if (num2 < num)
			{
				num = num2;
				boid = nearbyBoids[i].boid;
			}
		}
		return boid;
	}

	public void ChangeAttachedObject(ObjectBoid oldBoid, ObjectBoid newBoid)
	{
		BoidSoundEmitter attachedEmitter = getAttachedEmitter(oldBoid);
		if (attachedEmitter != null)
		{
			attachedEmitter.SetAttachedObject(newBoid);
		}
	}

	public BoidSoundEmitter getAttachedEmitter(ObjectBoid b)
	{
		for (int i = 0; i < pool.Count; i++)
		{
			if (pool[i].GetAttachedObject() == b)
			{
				return pool[i];
			}
		}
		return null;
	}

	private bool nearbyBoidsContains(ObjectBoid boid)
	{
		for (int i = 0; i < nearbyBoids.Count; i++)
		{
			if (nearbyBoids[i].boid == boid)
			{
				return true;
			}
		}
		return false;
	}

	private void deleteNullBoids()
	{
		for (int i = 0; i < pool.Count; i++)
		{
			if (pool[i].GetAttachedObject() != null && !pool[i].GetAttachedObject().isActiveAndEnabled)
			{
				StopBoidSound(pool[i].GetAttachedObject(), 0f);
			}
		}
		for (int j = 0; j < nearbyBoids.Count; j++)
		{
			if (nearbyBoids[j].boid == null || !nearbyBoids[j].boid.isActiveAndEnabled)
			{
				nearbyBoids.RemoveAt(j);
			}
		}
	}

	public void SetMusicState(int s)
	{
		musicState = s;
	}

	private float SqDistance(Vector3 a, Vector3 b)
	{
		return (b - a).sqrMagnitude;
	}

	public bool IsPlayingMusic()
	{
		if (musicEmitter[0].isPlaying)
		{
			return true;
		}
		return false;
	}

	protected void Init()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			SafeDebug.LogWarning("Boid Sound Pool already exists. Deleting myself");
			Object.Destroy(base.gameObject);
		}
		if (audioSettingsSource == null)
		{
			audioSettingsSource = GetComponent<GvrAudioSource>();
			if (audioSettingsSource == null)
			{
				SafeDebug.LogWarning("You are missing your settings source. Fix the reference in the scene");
				audioSettingsSource = base.gameObject.AddComponent<GvrAudioSource>();
			}
		}
		pool = new List<BoidSoundEmitter>();
		musicEmitter = new List<BoidSoundEmitter>();
		for (int i = 0; i <= poolSize; i++)
		{
			GameObject gameObject = new GameObject();
			GvrAudioSource newSource = gameObject.AddComponent<GvrAudioSource>();
			CopyGVRAudioSource(ref newSource, audioSettingsSource);
			BoidSoundEmitter boidSoundEmitter = gameObject.AddComponent<BoidSoundEmitter>();
			gameObject.transform.SetParent(transform);
			gameObject.gameObject.SetActive(value: false);
			gameObject.name = "BoidSoundPool" + i;
			boidSoundEmitter.owner = this;
			pool.Add(boidSoundEmitter);
		}
		for (int j = 0; j < 3; j++)
		{
			GameObject gameObject2 = new GameObject();
			GvrAudioSource newSource2 = gameObject2.AddComponent<GvrAudioSource>();
			CopyGVRAudioSource(ref newSource2, musicSettingsSource);
			newSource2.dopplerLevel = 0f;
			BoidSoundEmitter boidSoundEmitter2 = gameObject2.AddComponent<BoidSoundEmitter>();
			gameObject2.transform.SetParent(transform);
			gameObject2.SetActive(value: false);
			gameObject2.name = "Music Emitter: " + j;
			boidSoundEmitter2.owner = this;
			musicEmitter.Add(boidSoundEmitter2);
		}
		boidControllers = new List<BoidController>();
		nearbyBoids = new List<BoidSoundObject>();
		CheckReferences();
	}

	private void CopyGVRAudioSource(ref GvrAudioSource newSource, GvrAudioSource originalSource)
	{
		newSource.volume = originalSource.volume;
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

	private void CheckReferences()
	{
		if (boidMusic == null)
		{
			SafeDebug.LogWarning("You haven't assigned the boidMusic audio event in the scene. This will not work.");
		}
		if (boidFlying == null)
		{
			SafeDebug.LogWarning("You haven't assigned the boidFlying aduio even in the scene. This will not work");
		}
		if (boidConsuming == null)
		{
			SafeDebug.LogWarning("You haven't assigned the boidConsuming audio event in the scene. This will not work");
		}
	}
}
