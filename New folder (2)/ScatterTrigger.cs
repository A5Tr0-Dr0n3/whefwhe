using System.Collections.Generic;
using UnityEngine;

public class ScatterTrigger : MonoBehaviour
{
	public enum TriggerType
	{
		Swarm,
		Proximity
	}

	public TriggerType triggerType;

	public List<BoidController> swarms = new List<BoidController>();

	[Range(0.1f, 20f)]
	public float distance = 5f;

	public bool onlyTriggerOnPlayerCollide;

	private void OnTriggerEnter(Collider other)
	{
		if (onlyTriggerOnPlayerCollide && !(other.gameObject.name == "Camera"))
		{
			return;
		}
		if (triggerType == TriggerType.Swarm)
		{
			foreach (BoidController swarm in swarms)
			{
				swarm.ScatterSwarm(4f, 3f);
			}
			return;
		}
		if (triggerType != TriggerType.Proximity)
		{
		}
	}
}
