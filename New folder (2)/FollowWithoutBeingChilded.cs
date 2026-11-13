using UnityEngine;

public class FollowWithoutBeingChilded : MonoBehaviour
{
	[SerializeField]
	private Transform followedObject;

	public Vector3 localOffset;

	[SerializeField]
	private bool followRotation;

	private void LateUpdate()
	{
		if (followedObject != null)
		{
			transform.position = followedObject.position + localOffset;
			if (followRotation)
			{
				transform.rotation = followedObject.transform.rotation;
			}
		}
	}
}
