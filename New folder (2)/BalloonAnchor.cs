using UnityEngine;

public class BalloonAnchor : MonoBehaviour
{
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, 0.01f);
	}
}
