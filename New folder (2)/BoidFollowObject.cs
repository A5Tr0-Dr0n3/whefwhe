using UnityEngine;

public class BoidFollowObject : MonoBehaviour
{
	public VVRConsumable follow;

	public VVRConsumable follow2;

	public float height;

	private void Update()
	{
		if (follow != null)
		{
			transform.position = follow.transform.position + Vector3.up * height;
		}
	}
}
