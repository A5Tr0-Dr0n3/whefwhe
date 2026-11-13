using UnityEngine;

public class VVR_STAMPER_05 : MonoBehaviour
{
	public ObjectWater waterTrigger;

	public BoidController swarm1;

	public BoidController swarm2;

	public BoidController swarm3;

	private bool firstBool = true;

	private void Start()
	{
	}

	private void Update()
	{
		if (waterTrigger.isSatisfied && firstBool)
		{
			firstBool = false;
			swarm1.PowerDownAll();
			swarm2.PowerDownAll();
			swarm3.PowerDownAll();
		}
	}
}
