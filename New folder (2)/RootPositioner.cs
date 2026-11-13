using UnityEngine;

public class RootPositioner : MonoBehaviour
{
	public Transform pos1;

	public Transform pos2;

	private float yPos;

	private Vector3 targetPosition;

	private void Update()
	{
		targetPosition = (pos1.position + pos2.position) * 0.5f;
		transform.position = targetPosition;
	}
}
