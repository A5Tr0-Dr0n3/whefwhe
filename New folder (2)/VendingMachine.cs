using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{
	[SerializeField]
	private GameObject poolObject;

	[SerializeField]
	private Transform machineRoot;

	[SerializeField]
	private Transform spawnPos;

	[SerializeField]
	private GvrAudioSource dispenseSound;

	[SerializeField]
	private HubScriptableObject tweakables;

	[SerializeField]
	private VVRDialogueSource chazTalker;

	[SerializeField]
	private Transform sodaButton;

	[SerializeField]
	private Transform poolParent;

	public AudioEvent sfx;

	public ParticleSystem particles;

	[SerializeField]
	private float poolSize = 5f;

	[SerializeField]
	private float dispenseTime = 2.8f;

	private List<PooledSmashObject> BottlePool;

	private SafeCoroutine dispenseThread;

	private bool dispenseDialogueHasPlayed;

	protected void Start()
	{
		init();
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		ItemDisplaySwapper.OnFlip += DisableAllBottles;
	}

	private void OnDisable()
	{
		ItemDisplaySwapper.OnFlip -= DisableAllBottles;
	}

	public void DispenseBottle()
	{
		if (dispenseThread == null)
		{
			dispenseThread = GlobalCoroutineContainer.AddThread(DispenseBottleCoroutine(), this);
		}
	}

	public IEnumerator DispenseBottleCoroutine()
	{
		dispenseSound.Play();
		Vector3 buttonStartPos = sodaButton.localPosition;
		Vector3 buttonEndPos = buttonStartPos;
		buttonEndPos.x -= 0.015f;
		float timer = 0f;
		while (timer <= 1f)
		{
			timer += Time.deltaTime * 3f;
			if (timer <= 0.5f)
			{
				sodaButton.transform.localPosition = Vector3.Lerp(buttonStartPos, buttonEndPos, timer * 2f);
			}
			else
			{
				sodaButton.transform.localPosition = Vector3.Lerp(buttonEndPos, buttonStartPos, timer * 2f);
			}
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(dispenseTime, this);
		for (int i = 0; (float)i < poolSize; i++)
		{
			if (!BottlePool[i].active)
			{
				Rigidbody rb = BottlePool[i].rb;
				Vector3 zero = Vector3.zero;
				BottlePool[i].rb.angularVelocity = zero;
				rb.velocity = zero;
				BottlePool[i].bottle.position = spawnPos.position;
				BottlePool[i].transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
				BottlePool[i].Enable();
				i = (int)poolSize + 1;
			}
		}
		if (!dispenseDialogueHasPlayed)
		{
			VVR_HUB.instance.GetComponent<HubExt>().CommentSnackSoda();
			dispenseDialogueHasPlayed = true;
		}
		dispenseThread = null;
	}

	private void init()
	{
		BottlePool = new List<PooledSmashObject>();
		for (int i = 0; (float)i < poolSize; i++)
		{
			GameObject gameObject = Object.Instantiate(poolObject, machineRoot);
			gameObject.transform.SetParent(poolParent, worldPositionStays: true);
			PooledSmashObject component = gameObject.GetComponent<PooledSmashObject>();
			component.scone.sfx = sfx;
			component.scone.particles = particles;
			component.SetBottle(gameObject);
			BottlePool.Add(component);
			component.Disable();
		}
	}

	public void DisableAllBottles()
	{
		if (BottlePool != null)
		{
			for (int i = 0; i < BottlePool.Count; i++)
			{
				BottlePool[i].Disable();
			}
		}
	}
}
