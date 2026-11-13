using UnityEngine;

public class AggroTimer : MonoBehaviour
{
	public BoidController aggroSwarm;

	public float searchTimer = 30f;

	public float chaseTimer = 120f;

	public float failsafeTimer = 200f;

	protected bool hasAdded;

	protected bool triggeredAggro;

	protected bool triggeredChase;

	protected bool triggeredFailsafe;

	protected virtual void Update()
	{
		searchTimer -= Time.deltaTime;
		failsafeTimer -= Time.deltaTime;
		chaseTimer -= Time.deltaTime;
		if (searchTimer < 0f && !triggeredAggro)
		{
			aggroSwarm.SetSwarmBehavior(ObjectBoid.behaviorMode.search);
			triggeredAggro = true;
		}
		if (chaseTimer < 0f && !triggeredChase)
		{
			aggroSwarm.SetSwarmBehavior(ObjectBoid.behaviorMode.chase);
			triggeredChase = true;
		}
		if (failsafeTimer < 0f && !triggeredFailsafe)
		{
			triggeredFailsafe = true;
			if (aggroSwarm.boidList[0] != null && !hasAdded)
			{
				hasAdded = true;
				aggroSwarm.boidList[0].transform.position = EagerMaster.instance.mainCamera.transform.position + Vector3.up + Vector3.forward;
				aggroSwarm.boidList[0].SetBehaviorMode(ObjectBoid.behaviorMode.chase);
			}
		}
	}
}
