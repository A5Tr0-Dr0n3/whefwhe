using System.Collections;
using System.Collections.Generic;
using TButt;
using UnityEngine;

public class BoidController : MonoBehaviour
{
	public GameObject boidPrefab;

	[Header("Audio Settings")]
	public bool playsMusic = true;

	public bool playsSound = true;

	private int flyingSoundCount = 1;

	public List<ObjectBoid> flyingSoundBoids;

	public RoostController roostController;

	public int spawnCount = 10;

	public int desktopVrSpawnCount = 20;

	public float spawnRadius = 4f;

	[Range(0.1f, 20f)]
	public float velocity = 2.5f;

	[Range(0f, 0.9f)]
	public float velocityVariation = 0.5f;

	[Range(0.1f, 20f)]
	public float rotationCoeff = 5.4f;

	[Range(0.1f, 20f)]
	public float neighborDist = 2f;

	public List<ObjectBoid> boidList;

	public ObjectBoid.behaviorMode swarmBehavior;

	public bool canAttack;

	public LayerMask searchLayer;

	public bool ambientRoostAlternate;

	public List<ConsumableList> deconstructLists = new List<ConsumableList>();

	public List<ConsumableList> constructLists = new List<ConsumableList>();

	public List<VVRConsumable> toDeconstruct;

	public List<VVRConsumable> toConstruct;

	private TCPlatformScene spawnedBoidScene;

	[HideInInspector]
	public Transform t;

	private int boidNumber;

	private ObjectBoid objecctBoid;

	public Material alertMaterial;

	public Material calmMaterial;

	private GameObject boidParent;

	public int GetFlyingSoundCount()
	{
		return flyingSoundCount;
	}

	public bool deconstructUpdate()
	{
		if (toDeconstruct.Count == 0)
		{
			if (deconstructLists.Count == 0)
			{
				return false;
			}
			toDeconstruct = deconstructLists[0].Consumables;
			deconstructLists.RemoveAt(0);
			return true;
		}
		return true;
	}

	public bool constructUpdate()
	{
		if (toConstruct.Count == 0)
		{
			if (constructLists.Count == 0)
			{
				return false;
			}
			toConstruct = constructLists[0].Consumables;
			constructLists.RemoveAt(0);
			return true;
		}
		return true;
	}

	private void OnEnable()
	{
		if (toDeconstruct.Count == 0 && deconstructLists.Count > 0)
		{
			toDeconstruct = deconstructLists[0].Consumables;
			deconstructLists.RemoveAt(0);
		}
		if (toConstruct.Count == 0 && constructLists.Count > 0)
		{
			toConstruct = constructLists[0].Consumables;
			constructLists.RemoveAt(0);
		}
		SendReferencesToAudioPool();
	}

	private void OnDisable()
	{
		RemoveReferenceToAudioPool();
	}

	private void Awake()
	{
		t = transform;
		flyingSoundBoids = new List<ObjectBoid>();
	}

	private void Start()
	{
		if (boidList == null || boidList.Count < spawnCount)
		{
			Invoke("SpawnAllBoids", 0.25f);
		}
		else if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
		{
			Invoke("SpawnDesktopBoids", 0.3f);
		}
	}

	public void SpawnDesktopBoids()
	{
		if (desktopVrSpawnCount == 0)
		{
			return;
		}
		if (boidList != null)
		{
			Headset headset = null;
			if (boidList.Count > 0)
			{
				headset = boidList[0].GetComponent<Headset>();
			}
			if (headset != null)
			{
				spawnedBoidScene = headset.Scene;
			}
			for (int i = 0; i < desktopVrSpawnCount; i++)
			{
				Spawn();
			}
		}
		else
		{
			SpawnAllBoids();
			for (int j = 0; j < desktopVrSpawnCount; j++)
			{
				Spawn();
			}
		}
	}

	public void SpawnAllBoids()
	{
		boidParent = Object.Instantiate(new GameObject());
		if (EagerScenes.instance.ActiveScene != null)
		{
			boidParent.transform.parent = EagerScenes.instance.ActiveScene.transform;
		}
		else
		{
			boidParent.transform.parent = transform.parent;
		}
		boidParent.name = "BOIDSWARM";
		boidList = new List<ObjectBoid>(spawnCount);
		for (int i = 0; i < spawnCount; i++)
		{
			Spawn();
		}
		if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile && boidList.Count < desktopVrSpawnCount)
		{
			SpawnDesktopBoids();
		}
	}

	public GameObject Spawn()
	{
		return Spawn(t.position + Random.insideUnitSphere * spawnRadius);
	}

	public GameObject Spawn(Vector3 position)
	{
		if (boidPrefab == null)
		{
			return null;
		}
		Quaternion rotation = Quaternion.Slerp(t.rotation, Random.rotation, 0.3f);
		GameObject gameObject = Object.Instantiate(boidPrefab, position, rotation);
		gameObject.name = "boid__";
		gameObject.name += boidNumber;
		if (boidParent == null)
		{
			boidParent = Object.Instantiate(new GameObject());
			if (EagerScenes.instance.ActiveScene != null)
			{
				boidParent.transform.parent = EagerScenes.instance.ActiveScene.transform;
			}
			else
			{
				boidParent.transform.parent = transform.parent;
			}
		}
		gameObject.transform.parent = boidParent.transform;
		if (spawnedBoidScene != null && (bool)gameObject.GetComponent<Headset>())
		{
			gameObject.GetComponent<Headset>().Scene = spawnedBoidScene;
		}
		boidNumber++;
		objecctBoid = gameObject.GetComponent<ObjectBoid>();
		objecctBoid.controller = this;
		objecctBoid.startBehavior = swarmBehavior;
		objecctBoid.SetBehaviorMode(swarmBehavior);
		boidList.Add(objecctBoid);
		return gameObject;
	}

	public void SetSwarmBehavior(ObjectBoid.behaviorMode newBehavior)
	{
		if (newBehavior == swarmBehavior)
		{
			return;
		}
		swarmBehavior = newBehavior;
		for (int i = 0; i < boidList.Count; i++)
		{
			if (boidList[i] != null)
			{
				boidList[i].SetBehaviorMode(swarmBehavior);
			}
			else
			{
				boidList.RemoveAt(i);
			}
		}
	}

	public IEnumerator ScatterSwarm(float scale, float duration)
	{
		AnimationCurve distCurve = AnimationCurve.EaseInOut(0f, neighborDist, duration / 2f, neighborDist * scale);
		float t = 0f;
		float origVelVar = velocityVariation;
		velocityVariation = velocity * 0.4f;
		bool hasShaker = false;
		bool hasFollow = false;
		if (GetComponent<TransformShaker>() != null && GetComponent<TransformShaker>().isActiveAndEnabled)
		{
			GetComponent<TransformShaker>().enabled = false;
			hasShaker = true;
		}
		if (GetComponent<TransformFollow>() != null && GetComponent<TransformFollow>().isActiveAndEnabled)
		{
			GetComponent<TransformFollow>().enabled = false;
			hasFollow = true;
		}
		neighborDist *= scale;
		while (t < duration)
		{
			if (t > duration / 2f)
			{
				neighborDist = distCurve.Evaluate(duration - t);
			}
			t += Time.deltaTime;
			yield return null;
		}
		if (hasShaker)
		{
			GetComponent<TransformShaker>().enabled = true;
		}
		if (hasFollow)
		{
			GetComponent<TransformFollow>().enabled = true;
		}
		neighborDist = distCurve.Evaluate(0f);
		velocityVariation = origVelVar;
		yield return null;
	}

	public void AddToConsumableList(bool toConstruct, List<VVRConsumable> list, string listName)
	{
		ConsumableList consumableList = new ConsumableList();
		consumableList.Label = listName;
		consumableList.Consumables = list;
		if (toConstruct)
		{
			constructLists.Add(consumableList);
		}
		else
		{
			deconstructLists.Add(consumableList);
		}
	}

	public void PowerUpAll()
	{
		GlobalCoroutineContainer.AddThread(PowerAll(power: true), this);
	}

	public void PowerDownAll()
	{
		GlobalCoroutineContainer.AddThread(PowerAll(power: false), this);
	}

	private IEnumerator PowerAll(bool power)
	{
		for (int i = 0; i < boidList.Count; i++)
		{
			if (boidList[i] != null)
			{
				Vector3 pos1 = boidList[i].transform.position;
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.06f, this);
				Vector3 pos2 = boidList[i].transform.position;
				boidList[i].rb.velocity = (pos2 - pos1) * 20f;
				boidList[i].Power = power;
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(Random.Range(0.1f, 0.3f), this);
			}
		}
	}

	private void SendReferencesToAudioPool()
	{
		if (BoidSoundPool.instance != null && (playsMusic || playsSound) && !BoidSoundPool.instance.boidControllers.Contains(this) && gameObject.activeInHierarchy)
		{
			BoidSoundPool.instance.boidControllers.Add(this);
		}
	}

	private void RemoveReferenceToAudioPool()
	{
		if (BoidSoundPool.instance != null && BoidSoundPool.instance.boidControllers.Contains(this))
		{
			BoidSoundPool.instance.boidControllers.Remove(this);
		}
	}
}
