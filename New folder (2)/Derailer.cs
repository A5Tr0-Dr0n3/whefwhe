using UnityEngine;

public class Derailer : MonoBehaviour
{
	public float forceBackMin = 300f;

	public float forceBackMax = 800f;

	public float torquer = 50f;

	private void OnTriggerEnter(Collider c)
	{
		if (c.gameObject.name == "Flat")
		{
			Rigidbody componentInParent = c.GetComponentInParent<Rigidbody>();
			componentInParent.isKinematic = false;
			componentInParent.useGravity = true;
			componentInParent.AddForce(Vector3.up * Random.Range(forceBackMin, forceBackMax));
			componentInParent.AddTorque(new Vector3(Random.Range(0f - torquer, torquer), Random.Range(0f - torquer, torquer), Random.Range(0f - torquer, torquer)));
			c.gameObject.GetComponentInParent<ConveyorLink>().broken = true;
		}
	}
}
