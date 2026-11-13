using UnityEngine;

namespace StixGames;

[AddComponentMenu("StixGames/Camera Follow")]
public class CameraFollow : MonoBehaviour
{
	public Transform follow;

	public float followHeight = 20f;

	private void Update()
	{
		if (follow == null)
		{
			follow = Camera.main.transform;
		}
		else
		{
			transform.position = follow.position + Vector3.up * followHeight;
		}
	}
}
