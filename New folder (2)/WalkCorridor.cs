using UnityEngine;

public class WalkCorridor : MonoBehaviour
{
	private bool walking = true;

	private void Start()
	{
	}

	private void Update()
	{
		if (walking)
		{
			transform.position = Vector3.Lerp(transform.position, new Vector3(0f, 0.5f, -0.2f), 0.4f * Time.deltaTime);
			if (transform.position.z > -0.19f)
			{
				walking = false;
			}
		}
	}
}
