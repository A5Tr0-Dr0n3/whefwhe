using UnityEngine;

public class CageEnforcer : MonoBehaviour
{
	public Transform headsetTransform;

	public Collider cageCollider;

	private void Update()
	{
		if (!cageCollider.bounds.Contains(headsetTransform.position))
		{
			headsetTransform.position = cageCollider.bounds.center;
		}
	}
}
