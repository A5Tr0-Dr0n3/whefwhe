using UnityEngine;

public class Roost : MonoBehaviour
{
	public RoostController roostController;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(transform.position, 0.05f);
		Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.3f);
	}

	private void OnEnable()
	{
		if (!roostController.unoccupiedRoostPositions.Contains(gameObject) && !roostController.occupiedRoostPositions.Contains(gameObject))
		{
			roostController.unoccupiedRoostPositions.Add(gameObject);
		}
	}
}
