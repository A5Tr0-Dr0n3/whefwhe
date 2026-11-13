using UnityEngine;

public class VVR_STAMPER_13 : SceneBehaviorManager
{
	public ChazController fishChaz;

	public ChazController plantChaz;

	public ChazController beachBallChaz;

	public GameObject fishHeadset;

	public GameObject plantHeadset;

	public GameObject beachBallHeadset;

	public GameObject deathHeadset;

	public GameObject headsetPlatform;

	public GameObject headsetRoot;

	private bool fishAlive;

	private bool plantAlive;

	private bool beachBallAlive;

	public GameObject skull;

	public float killThreshhold = -10f;

	private bool headsetSpawned;

	private float t;

	private void Update()
	{
		t += Time.deltaTime;
		if (t > 3f)
		{
			t = 0f;
			UpdateChazGoals();
		}
		if (checkChazesAlive() == 1)
		{
			if (fishAlive)
			{
				fishHeadset.SetActive(value: true);
				headsetPlatform.SetActive(value: true);
				if (!headsetSpawned)
				{
					headsetRoot.transform.position = Vector3.MoveTowards(EagerMaster.instance.mainCamera.transform.position, Vector3.up * 4f, 3f);
					headsetSpawned = true;
				}
			}
			else if (plantAlive)
			{
				plantHeadset.SetActive(value: true);
				headsetPlatform.SetActive(value: true);
				if (!headsetSpawned)
				{
					headsetRoot.transform.position = Vector3.MoveTowards(EagerMaster.instance.mainCamera.transform.position, Vector3.up * 4f, 3f);
					headsetSpawned = true;
				}
			}
			else if (beachBallAlive)
			{
				beachBallHeadset.SetActive(value: true);
				headsetPlatform.SetActive(value: true);
				if (!headsetSpawned)
				{
					headsetRoot.transform.position = Vector3.MoveTowards(EagerMaster.instance.mainCamera.transform.position, Vector3.up * 4f, 3f);
					headsetSpawned = true;
				}
			}
		}
		else if (checkChazesAlive() < 1)
		{
			fishHeadset.SetActive(value: false);
			plantHeadset.SetActive(value: false);
			beachBallHeadset.SetActive(value: false);
			deathHeadset.SetActive(value: true);
			headsetPlatform.SetActive(value: true);
			if (!headsetSpawned)
			{
				headsetRoot.transform.position = Vector3.MoveTowards(EagerMaster.instance.mainCamera.transform.position, Vector3.up * 4f, 3f);
				headsetSpawned = true;
			}
		}
		else
		{
			fishHeadset.SetActive(value: false);
			plantHeadset.SetActive(value: false);
			beachBallHeadset.SetActive(value: false);
			deathHeadset.SetActive(value: false);
			headsetPlatform.SetActive(value: false);
		}
	}

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		fishAlive = true;
		beachBallAlive = true;
		plantAlive = true;
		headsetSpawned = false;
		UpdateChazGoals();
	}

	public void UpdateChazGoals()
	{
		if (fishChaz != null)
		{
			if (Random.value > 0.33f)
			{
				fishChaz.WalkTo(skull.transform.position);
			}
		}
		else
		{
			fishAlive = false;
		}
		if (plantChaz != null)
		{
			if (Random.value > 0.33f)
			{
				plantChaz.WalkTo(skull.transform.position);
			}
		}
		else
		{
			plantAlive = false;
		}
		if (beachBallChaz != null)
		{
			if (Random.value > 0.33f)
			{
				beachBallChaz.WalkTo(skull.transform.position);
			}
		}
		else
		{
			beachBallAlive = false;
		}
	}

	private int checkChazesAlive()
	{
		if (fishChaz.head.transform.position.y < killThreshhold)
		{
			fishAlive = false;
		}
		if (plantChaz.head.transform.position.y < killThreshhold)
		{
			plantAlive = false;
		}
		if (beachBallChaz.head.transform.position.y < killThreshhold)
		{
			beachBallAlive = false;
		}
		int num = 0;
		if (fishAlive)
		{
			num++;
		}
		if (plantAlive)
		{
			num++;
		}
		if (beachBallAlive)
		{
			num++;
		}
		return num;
	}
}
