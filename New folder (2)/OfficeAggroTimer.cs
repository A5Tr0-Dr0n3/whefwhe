using UnityEngine;

public class OfficeAggroTimer : AggroTimer
{
	public float conditionalTimer;

	public bool condition = true;

	private void Start()
	{
	}

	protected override void Update()
	{
		searchTimer -= Time.deltaTime;
		conditionalTimer -= Time.deltaTime;
		failsafeTimer -= Time.deltaTime;
		if (condition)
		{
			if (conditionalTimer < 0f && !triggeredAggro)
			{
				aggroSwarm.SetSwarmBehavior(ObjectBoid.behaviorMode.chase);
				triggeredAggro = true;
			}
		}
		else if (searchTimer < 0f && !triggeredAggro)
		{
			aggroSwarm.SetSwarmBehavior(ObjectBoid.behaviorMode.chase);
			triggeredAggro = true;
		}
		if (failsafeTimer < 0f && !triggeredFailsafe)
		{
			triggeredFailsafe = true;
			if (aggroSwarm.boidList[0] != null && !hasAdded)
			{
				hasAdded = true;
				aggroSwarm.boidList[0].transform.position = Camera.main.transform.position + Vector3.up + Vector3.forward;
				aggroSwarm.boidList[0].SetBehaviorMode(ObjectBoid.behaviorMode.chase);
			}
		}
	}
}
