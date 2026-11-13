using UnityEngine;

public class BaitScene : MonoBehaviour
{
	public GameObject follow;

	public GameObject swarm;

	public float height;

	private void Update()
	{
		if (follow != null)
		{
			swarm.transform.position = follow.transform.position + Vector3.up * height;
		}
	}
}
