using UnityEngine;

public class VVR_STAMPER_12 : SceneBehaviorManager
{
	[Header("Chaz Reference")]
	public ChazController chazOne;

	public ChazController chazTwo;

	public float chazKillY = -100f;

	private bool chazOneCanSpawn;

	private bool chazOneQueueSpawn = true;

	private bool chazTwoCanSpawn;

	private bool chazTwoQueueSpawn = true;

	[Header("Timings")]
	public float spawnBoidTime = 9f;

	private float spawnBoidCountdown = 9f;

	[Header("Audio")]
	public AudioLoop startLoop;

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		chazOne.WalkTo(EagerMaster.instance.mainCamera.transform.position);
		chazTwo.WalkTo(EagerMaster.instance.mainCamera.transform.position);
		spawnBoidCountdown = spawnBoidTime;
	}

	private void Update()
	{
		if (chazOne.gameObject.activeSelf && chazOne.transform.position.y < chazKillY)
		{
			chazOne.transform.parent.gameObject.SetActive(value: false);
		}
		if (chazTwo.gameObject.activeSelf && chazTwo.transform.position.y < chazKillY)
		{
			chazTwo.transform.parent.gameObject.SetActive(value: false);
		}
		UpdateChazMovement();
		UpdateChazSpawns();
	}

	private void UpdateChazMovement()
	{
		if (chazOne.gameObject.activeSelf && !chazOneCanSpawn && Random.value > 0.6f)
		{
			chazOne.WalkTo(EagerMaster.instance.mainCamera.transform.position);
		}
		if (chazTwo.gameObject.activeSelf && !chazTwoCanSpawn && Random.value > 0.6f)
		{
			chazTwo.WalkTo(EagerMaster.instance.mainCamera.transform.position);
		}
	}

	private void UpdateChazSpawns()
	{
		spawnBoidCountdown -= Time.deltaTime;
		if (!(spawnBoidCountdown <= 0f))
		{
			return;
		}
		spawnBoidCountdown = spawnBoidTime;
		if (chazOne.gameObject.activeSelf && Random.value < 0.6f)
		{
			if (chazOneCanSpawn)
			{
				chazOne.PresentBoid();
			}
			else
			{
				chazOneQueueSpawn = true;
			}
		}
		if (chazTwo.gameObject.activeSelf && Random.value < 0.6f)
		{
			if (chazTwoCanSpawn)
			{
				chazTwo.PresentBoid();
			}
			else
			{
				chazTwoQueueSpawn = true;
			}
		}
	}

	public void SetChazOneCanSpawn(bool newSet)
	{
		chazOneCanSpawn = newSet;
		if (chazOneCanSpawn)
		{
			chazOne.WalkTo(chazOne.rootPositioner.transform.position);
			if (chazOneQueueSpawn)
			{
				chazOne.PresentBoid();
				chazOneQueueSpawn = false;
			}
		}
	}

	public void SetChazTwoCanSpawn(bool newSet)
	{
		chazTwoCanSpawn = newSet;
		if (chazTwoCanSpawn)
		{
			chazTwo.WalkTo(chazTwo.rootPositioner.transform.position);
			if (chazTwoQueueSpawn)
			{
				chazTwo.PresentBoid();
				chazTwoQueueSpawn = false;
			}
		}
	}

	protected override void OnSceneActivated()
	{
		if (startLoop != null)
		{
			startLoop.Play();
		}
	}

	private void OnDisable()
	{
		if (startLoop != null)
		{
			startLoop.Stop();
		}
	}
}
