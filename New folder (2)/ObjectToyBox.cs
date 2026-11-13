using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using UnityEngine;
using UnityEngine.Events;

public class ObjectToyBox : ObjectBase
{
	[SerializeField]
	private float openTime = 1f;

	[SerializeField]
	private AnimationCurve flapCurve;

	[SerializeField]
	private Vector3 displacementFactor = Vector3.one;

	[SerializeField]
	private float interruptThreshhold = 0.5f;

	public UnityEvent OnPlayerEntersThreshhold;

	public float currentPlayerDistance;

	private float lastFramePlayerDistance;

	public Transform itemSpawnPos;

	[SerializeField]
	public List<DollhouseItem> itemsToSpawn = new List<DollhouseItem>();

	[SerializeField]
	protected GameObject[] flaps;

	[SerializeField]
	private GvrAudioSource audioSource;

	[SerializeField]
	private JobTina sceneRef;

	public BezierCurve grabCurve;

	protected override void Awake()
	{
		base.Awake();
		init();
	}

	private void Update()
	{
		currentPlayerDistance = Vector3.Distance(Camera.main.transform.position, transform.position);
		if (currentPlayerDistance < interruptThreshhold && lastFramePlayerDistance > interruptThreshhold)
		{
			OnPlayerEntersThreshhold.Invoke();
		}
		lastFramePlayerDistance = currentPlayerDistance;
	}

	public DollhouseItem SpawnDollToy(int index)
	{
		itemsToSpawn[index].obj.transform.position = itemSpawnPos.transform.position;
		itemsToSpawn[index].obj.gameObject.SetActive(value: true);
		itemsToSpawn[index].obj.rb.velocity = Vector3.zero;
		return itemsToSpawn[index];
	}

	public List<ObjectBase> GetToys(int[] indices)
	{
		List<ObjectBase> list = new List<ObjectBase>();
		for (int i = 0; i < indices.Length; i++)
		{
			list.Add(itemsToSpawn[indices[i]].obj);
		}
		return list;
	}

	public ObjectBase GetToy(int index)
	{
		return itemsToSpawn[index].obj;
	}

	public DollhouseItem DespawnToy(DollhouseItem obj)
	{
		if (itemsToSpawn.Contains(obj))
		{
			for (int i = 0; i < itemsToSpawn.Count; i++)
			{
				if (itemsToSpawn[i].obj.gameObject.GetInstanceID() == obj.obj.gameObject.GetInstanceID())
				{
					itemsToSpawn[i].obj.gameObject.SetActive(value: false);
					itemsToSpawn[i].obj.gameObject.transform.position = itemSpawnPos.position;
				}
			}
		}
		else
		{
			obj.obj.gameObject.SetActive(value: false);
			obj.obj.gameObject.transform.position = itemSpawnPos.position;
		}
		return obj;
	}

	public DollhouseItem DespawnToy(GameObject o)
	{
		bool flag = false;
		for (int i = 0; i < itemsToSpawn.Count; i++)
		{
			if (itemsToSpawn[i].obj != null && itemsToSpawn[i].obj.gameObject.GetInstanceID() == o.GetInstanceID())
			{
				flag = true;
				itemsToSpawn[i].obj.gameObject.SetActive(value: false);
				itemsToSpawn[i].obj.gameObject.transform.position = itemSpawnPos.position;
				return itemsToSpawn[i];
			}
		}
		if (!flag)
		{
			o.SetActive(value: false);
			o.transform.position = itemSpawnPos.position;
			DollhouseItem dollhouseItem = new DollhouseItem
			{
				obj = o.GetComponent<ObjectBase>()
			};
			itemsToSpawn.Add(dollhouseItem);
			return dollhouseItem;
		}
		SafeDebug.LogError("Despawning an object that doesnt exist your return value is gonna be weird ");
		return itemsToSpawn[0];
	}

	public SafeCoroutine openFlaps()
	{
		return GlobalCoroutineContainer.AddThread(moveFlapCoroutine(), this);
	}

	public SafeCoroutine closeFlaps()
	{
		return GlobalCoroutineContainer.AddThread(moveFlapCoroutine(open: false), this);
	}

	public void SetBoxKinematic(bool s)
	{
		rb.isKinematic = s;
	}

	public SafeCoroutine DespawnAndMoveBox()
	{
		return GlobalCoroutineContainer.AddThread(DespawnAndMoveBoxCoroutine(), this);
	}

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		GlobalCoroutineContainer.AddThread(DropOnDelay(0.25f), this);
	}

	private void init()
	{
		for (int i = 0; i < itemsToSpawn.Count; i++)
		{
			if (itemsToSpawn[i].obj != null)
			{
				itemsToSpawn[i].obj.gameObject.SetActive(value: false);
				itemsToSpawn[i].obj.gameObject.transform.position = itemSpawnPos.transform.position;
			}
			else
			{
				SafeDebug.LogWarning(i + " doesn't have an object assigned");
			}
		}
	}

	private IEnumerator moveFlapCoroutine(bool open = true)
	{
		Quaternion startAng0 = flaps[0].transform.rotation;
		Quaternion startAng1 = flaps[1].transform.rotation;
		float t = 0f;
		if (open)
		{
			Quaternion endAng0 = Quaternion.AngleAxis(-180f, flaps[0].transform.forward) * startAng0;
			Quaternion endAng1 = Quaternion.AngleAxis(-180f, flaps[1].transform.forward) * startAng1;
			while (t <= 1f)
			{
				t += Time.deltaTime / openTime;
				flaps[0].transform.rotation = Quaternion.Slerp(startAng0, endAng0, flapCurve.Evaluate(t));
				flaps[1].transform.rotation = Quaternion.Slerp(startAng1, endAng1, flapCurve.Evaluate(t));
				yield return null;
			}
			yield break;
		}
		Quaternion endAng2 = Quaternion.AngleAxis(180f, flaps[0].transform.forward) * startAng0;
		Quaternion endAng3 = Quaternion.AngleAxis(180f, flaps[1].transform.forward) * startAng1;
		while (t <= 1f)
		{
			t += Time.deltaTime / openTime;
			flaps[0].transform.rotation = Quaternion.Slerp(startAng0, endAng2, flapCurve.Evaluate(t));
			flaps[1].transform.rotation = Quaternion.Slerp(startAng1, endAng3, flapCurve.Evaluate(t));
			yield return null;
		}
		yield return null;
	}

	private IEnumerator DespawnAndMoveBoxCoroutine()
	{
		audioSource.Play();
		yield return null;
		Vector3 newPos = transform.position + displacementFactor;
		displacementFactor *= -1f;
		transform.position = newPos;
	}

	private IEnumerator DropOnDelay(float t)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(t, this);
		ForceDropByPlayer();
	}
}
