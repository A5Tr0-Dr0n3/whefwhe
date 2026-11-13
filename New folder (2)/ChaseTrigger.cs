using UnityEngine;

public class ChaseTrigger : MonoBehaviour
{
	public BoidController aggroSwarm;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "MainCamera")
		{
			aggroSwarm.SetSwarmBehavior(ObjectBoid.behaviorMode.chase);
		}
	}
}
